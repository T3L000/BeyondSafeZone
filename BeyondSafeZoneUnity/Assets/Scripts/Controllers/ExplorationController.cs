using System;
using System.Collections.Generic;
using System.Linq;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Controllers
{
    /// <summary>探索系统 —— 对应 Godot exploration_controller.gd</summary>
    public static class ExplorationController
    {
        /// <summary>进入地点</summary>
        public static string EnterLocation(GameState state, string locationId)
        {
            if (state.DemoComplete) return "Demo 已结束，祁眠日志已解锁。";
            if (state.Phase != "morning" && state.Phase != "day")
                return "现在不是白天，不能进入新地点。";
            if (!state.Locations.ContainsKey(locationId))
                return "这里还没有绘制到地图上。";

            var location = state.Locations[locationId];
            if (location.Range > state.Bike.Range)
            {
                state.Lin.Fatigue += 1;
                state.LastEvent = $"{location.Name} 太远了。{GetLocationRiskText(state, locationId)}";
                return state.LastEvent;
            }

            // 应用路况惩罚
            int roadPenalty = RoadConditionFatiguePenalty(location);
            state.Lin.Fatigue += location.RouteTime + roadPenalty;
            state.Lin.Stress += Math.Max(0, location.Zombies - 2);
            state.Bike.Durability = Math.Max(0, state.Bike.Durability - location.Range);
            ApplyExplorationRisk(state, location);

            state.Exploration = new ExplorationState
            {
                ActiveLocation = locationId,
                TimeUsed = 0,
                TimeLimit = Math.Max(BalanceData.EXPLORE_MIN_TIME_LIMIT,
                    location.RouteTime + BalanceData.EXPLORE_TIME_EXTRA),
                Noise = 0,
                SearchedRooms = new List<string>(),
                LuredRooms = new List<string>()
            };
            state.Phase = "searching";

            string routeNote = roadPenalty > 0
                ? $" 路况：{location.RoadCondition}，额外疲劳+{roadPenalty}。"
                : "";
            string riskText = GetLocationRiskText(state, locationId);
            state.LastEvent = $"进入 {location.Name}。{riskText}{routeNote}先读房间，再决定搜哪里；拖太久会把白天耗完。";
            return state.LastEvent;
        }

        /// <summary>搜索房间</summary>
        public static string SearchRoom(GameState state, string roomId, string tactic = "careful")
        {
            if (state.Phase != "searching" || string.IsNullOrEmpty(state.Exploration.ActiveLocation))
                return "林行还没有进入可搜索地点。";

            var location = state.Locations[state.Exploration.ActiveLocation];
            if (!location.Rooms.TryGetValue(roomId, out var room))
                return "这个房间还没有做进灰盒。";

            if (room.Searched)
            {
                state.LastEvent = $"{room.Name} 已经搜过，再翻只会浪费时间。";
                return state.LastEvent;
            }
            if (room.Locked)
            {
                state.LastEvent = $"{room.Name} 门锁着。需要撬棍才能打开。";
                return state.LastEvent;
            }

            state.Exploration.TimeUsed += SearchTimeForTactic(room, tactic);
            var riskNotes = ApplyRoomSearchRisk(state, roomId, room, tactic);
            var found = new List<string>();

            var resourceKeys = new List<string>(room.Resources.Keys);
            foreach (var key in resourceKeys)
            {
                int amount = room.Resources[key];
                if (amount <= 0) continue;
                int taken = Math.Min(BalanceData.SEARCH_MAX_PER_RESOURCE, amount);
                room.Resources[key] = amount - taken;

                if (key == "battery" || key == "gasoline" || key == "tire")
                {
                    state.CarParts.Set(key, state.CarParts.Get(key) + taken);
                    found.Add($"汽车零件：{key} +{taken}");
                }
                else
                {
                    state.Resources.Set(key, state.Resources.Get(key) + taken);
                    found.Add($"{key} +{taken}");
                }
            }

            room.Searched = true;
            state.Exploration.SearchedRooms.Add(roomId);

            var flagNotes = ApplyRoomFlags(state, room);
            if (flagNotes.Count > 0)
                found.Add("线索：" + string.Join(" ", flagNotes));

            string noteText = riskNotes.Count > 0 ? string.Join(" ", riskNotes) + " " : "";
            if (found.Count == 0)
                state.LastEvent = $"搜索 {room.Name}。{noteText}这里已经没有能带走的东西。";
            else
                state.LastEvent = $"搜索 {room.Name}。{noteText}带回：{string.Join("，", found)}。";

            return state.LastEvent;
        }

        /// <summary>制造声音引开</summary>
        public static string LureRoom(GameState state, string roomId)
        {
            if (state.Phase != "searching" || string.IsNullOrEmpty(state.Exploration.ActiveLocation))
                return "林行还没有进入可搜索地点。";

            var location = state.Locations[state.Exploration.ActiveLocation];
            if (!location.Rooms.TryGetValue(roomId, out var room))
                return "这个房间还没有做进灰盒。";

            state.Exploration.TimeUsed += BalanceData.LURE_TIME_COST;
            state.Exploration.Noise += BalanceData.LURE_NOISE;

            if (room.HiddenZombies > 0 && !state.Exploration.LuredRooms.Contains(roomId))
            {
                state.Exploration.LuredRooms.Add(roomId);
                state.LastEvent = $"林行在 {room.Name} 外制造噪音，把里面的动静引向另一侧。";
            }
            else
            {
                state.LastEvent = $"林行在 {room.Name} 外制造噪音，但没有听见明显回应。";
            }
            return state.LastEvent;
        }

        /// <summary>离开探索地点</summary>
        public static string LeaveExploration(GameState state)
        {
            if (state.Phase != "searching" || string.IsNullOrEmpty(state.Exploration.ActiveLocation))
                return "林行还没有进入可离开的地点。";

            string locationId = state.Exploration.ActiveLocation;
            var location = state.Locations[locationId];
            location.Visited = true;

            var notes = ApplyEvacuationClues(state, locationId, location);
            int overTime = Math.Max(0, state.Exploration.TimeUsed - state.Exploration.TimeLimit);
            if (overTime > 0)
            {
                state.Lin.Fatigue += overTime;
                notes.Add($"天色压下来，额外疲劳+{overTime}。");
            }

            state.Phase = "evening";
            string noteText = notes.Count > 0 ? " " + string.Join(" ", notes) : "";
            state.Exploration = new ExplorationState();
            state.LastEvent = $"林行离开 {location.Name}，赶在天黑前回到据点。{noteText}";
            return state.LastEvent;
        }

        /// <summary>大地图探索（fallback，非室内搜索）</summary>
        public static string Explore(GameState state, string locationId)
        {
            if (state.DemoComplete) return "Demo 已结束，祁眠日志已解锁。";
            if (!state.Locations.TryGetValue(locationId, out var location))
                return "这里还没有绘制到地图上。";

            if (location.Range > state.Bike.Range)
            {
                state.Lin.Fatigue += 1;
                state.LastEvent = $"{location.Name} 太远了。{GetLocationRiskText(state, locationId)}";
                return state.LastEvent;
            }

            var found = new List<string>();
            var resKeys = new List<string>(location.Resources.Keys);
            foreach (var rk in resKeys)
            {
                int amount = location.Resources[rk];
                if (amount <= 0) continue;
                int taken = Math.Min(2, amount);
                location.Resources[rk] = amount - taken;

                if (rk == "battery" || rk == "gasoline" || rk == "tire")
                {
                    state.CarParts.Set(rk, state.CarParts.Get(rk) + taken);
                    found.Add($"汽车零件：{rk} +{taken}");
                }
                else
                {
                    state.Resources.Set(rk, state.Resources.Get(rk) + taken);
                    found.Add($"{rk} +{taken}");
                }
            }

            location.Visited = true;
            string riskText = GetLocationRiskText(state, locationId);
            int roadPenalty = RoadConditionFatiguePenalty(location);
            var pressureNotes = new List<string>();
            if (roadPenalty > 0)
                pressureNotes.Add($"路况：{location.RoadCondition}，额外疲劳+{roadPenalty}。");
            pressureNotes.AddRange(ApplyEvacuationClues(state, locationId, location));

            ApplyExplorationRisk(state, location);
            state.Bike.Durability = Math.Max(0, state.Bike.Durability - location.Range);
            state.Lin.Fatigue += location.RouteTime + roadPenalty;
            state.Lin.Stress += Math.Max(0, location.Zombies - 2);
            state.Phase = "evening";

            string pressureNote = pressureNotes.Count > 0 ? " " + string.Join(" ", pressureNotes) : "";
            if (found.Count == 0)
                state.LastEvent = $"探索 {location.Name}。{riskText}{pressureNote} 这里几乎被搜空了，只留下难以解释的翻动痕迹。";
            else
                state.LastEvent = $"探索 {location.Name}。{riskText}{pressureNote} 带回：{string.Join("，", found)}。";

            return state.LastEvent;
        }

        // ---- 私有辅助 ----

        private static int SearchTimeForTactic(RoomState room, string tactic) => tactic switch
        {
            "quick" => Math.Max(1, room.SearchTime - 1),
            _ => room.SearchTime
        };

        private static List<string> ApplyRoomSearchRisk(GameState state, string roomId, RoomState room, string tactic)
        {
            var notes = new List<string>();
            if (room.HiddenZombies <= 0) return notes;

            if (state.Exploration.LuredRooms.Contains(roomId))
            {
                notes.Add("隐藏尸群已被引开。");
                return notes;
            }

            bool darkRoom = room.Visibility == "黑暗";
            if (darkRoom || tactic == "quick")
            {
                state.Lin.Health = Math.Max(0, state.Lin.Health - 1);
                state.Lin.InfectionRisk += 1;
                state.Lin.Stress += 1;
                notes.Add("隐藏尸群从暗处扑出，林行受伤并增加感染风险。");
            }
            else
            {
                state.Lin.Stress += 1;
                notes.Add("房间里有隐藏尸群，谨慎搜索让林行勉强避开。");
            }
            return notes;
        }

        private static List<string> ApplyRoomFlags(GameState state, RoomState room)
        {
            var notes = new List<string>();
            string locationId = state.Exploration.ActiveLocation ?? "";
            foreach (var flag in room.Flags)
            {
                switch (flag)
                {
                    case "plan_found":
                        state.Lin.Hope += 1;
                        notes.Add("童年末日避难计划图纸——三个孩子的笔迹");
                        AddAnomalyDossier(state, locationId, "发现童年避难计划图纸，三个孩子笔迹——林行、祁眠、祁烬。", "推测祁眠也持有这份计划的副本。"); break;
                    case "safezone_hint_1":
                        notes.Add("纸条：「保护区在南边，往军区基地走」"); break;
                    case "rebirth_clue_1":
                        notes.Add("隔离记录：「零号病人已转移至返生计划中心」");
                        AddAnomalyDossier(state, locationId, "诊所隔离记录：零号病人「眠」已转移至返生计划中心。", "零号感染者与祁眠身份重合——返生计划在追查她。"); break;
                    case "rebirth_clue_2":
                        notes.Add("联络名单上画了红圈的名字——「烬」");
                        AddAnomalyDossier(state, locationId, "派出所联络名单：红圈标注名字「烬」——返生计划社区联络处。", "祁烬在返生计划内部担任联络员，可能仍在活动。"); break;
                    case "address_known":
                        state.Evacuation.AddressKnown = true;
                        notes.Add("地图碎片标注了保护区筛查棚位置"); break;
                    case "childhood_memory":
                        state.Lin.Hope += 1;
                        notes.Add("旧笔记：「林行、祁眠、祁烬——末日避难计划」");
                        AddAnomalyDossier(state, locationId, "学校图书馆找到童年笔记——三个人的避难计划仍在。", "祁眠看到了林行的计划副本——在诊所外的药柜上。"); break;
                    case "rebirth_poster":
                        notes.Add("返生计划海报：「人类的下一步」"); break;
                    case "car_found":
                        state.Car.Found = true;
                        notes.Add("旧轿车——需要电瓶、汽油、轮胎"); break;
                    case "crowbar_found":
                        state.Resources.Parts += 1;
                        notes.Add("找到撬棍——可以撬开车库和封锁的门"); break;
                    case "lab_location":
                        notes.Add("哨塔地图：「返生计划第三实验室 东区外环217号」");
                        AddAnomalyDossier(state, locationId, "哨塔地图标注返生计划第三实验室地址。", "祁眠可能在追踪同一个实验室——寻找祁烬的下落。"); break;
                    case "qimian_file":
                        notes.Add("实验日志：「零号感染者 代号:眠 瞳孔银灰反射」");
                        AddAnomalyDossier(state, locationId, "隔离站标本室：零号感染者实验日志，代号「眠」，瞳孔银灰反射异常。", "确认祁眠是返生计划的零号实验体。她的感染状态与普通丧尸不同。"); break;
                    case "apartment_letter":
                        notes.Add("信件：「返生计划第三实验室——不要去那里」");
                        AddAnomalyDossier(state, locationId, "公寓302室信件警告：不要接近返生计划第三实验室。", "有人——可能是祁烬——在警告幸存者远离实验室。"); break;
                    case "qijin_apartment":
                        notes.Add("信封：「返生计划·社区联络处·祁烬」");
                        AddAnomalyDossier(state, locationId, "祁烬在公寓留下的信封——返生计划社区联络处。", "祁烬曾在公寓活动，可能仍在传递情报。祁眠的寻找方向正确。"); break;
                    case "rebirth_insider":
                        state.Lin.Hope += 1;
                        notes.Add("幸存者说出了返生计划内幕情报"); break;
                    default:
                        notes.Add("新的线索"); break;
                }
            }
            return notes;
        }

        /// <summary>从房间旗标写入异常档案</summary>
        private static void AddAnomalyDossier(GameState state, string locationId, string clueText, string conclusion)
        {
            // 避免重复写入同一天同一地点的同一线索
            if (state.AnomalyDossier.Any(e => e.Day == state.Day && e.LocationId == locationId && e.ClueText == clueText))
                return;
            state.AnomalyDossier.Add(new AnomalyDossierEntry
            {
                Day = state.Day,
                LocationId = locationId,
                ClueText = clueText,
                Conclusion = conclusion
            });
        }

        public static string GetLocationRiskText(GameState state, string locationId)
        {
            if (!state.Locations.TryGetValue(locationId, out var location))
                return "风险：未知。";

            int pressure = location.Zombies + location.Range + state.Bike.Noise;
            if (location.Range > state.Bike.Range)
                return "风险：距离过远，今天无法稳定抵达。";
            if (pressure <= BalanceData.EXPLORE_RISK_LOW) return "风险：低，适合搜刮。";
            if (pressure <= BalanceData.EXPLORE_RISK_MID) return "风险：中，可能增加疲劳和压力。";
            return "风险：高，尸群密集，可能受伤。";
        }

        public static int RoadConditionFatiguePenalty(LocationState location) => location.RoadCondition switch
        {
            "熟路" or "碎玻璃" => 0,
            "雨后湿滑" or "积水" or "堵塞" or "路障" => 1,
            "尸群迁移" or "封锁线" => 2,
            _ => 1
        };

        private static void ApplyExplorationRisk(GameState state, LocationState location)
        {
            int pressure = location.Zombies + location.Range + state.Bike.Noise;
            if (pressure >= BalanceData.EXPLORE_RISK_HIGH)
            {
                state.Lin.Health = Math.Max(0, state.Lin.Health - BalanceData.EXPLORE_RISK_HIGH_HEALTH);
                state.Lin.Stress += BalanceData.EXPLORE_RISK_HIGH_STRESS;
            }
            else if (pressure >= 5)
            {
                state.Lin.Stress += 1;
            }
        }

        private static List<string> ApplyEvacuationClues(GameState state, string locationId, LocationState location)
        {
            var notes = new List<string>();
            if (locationId == "police" || locationId == "subway" || locationId == "safezone_edge"
                || location.Icons.Contains("clue") || location.Icons.Contains("route") || location.Icons.Contains("safezone"))
            {
                if (!state.Evacuation.AddressKnown)
                {
                    state.Evacuation.AddressKnown = true;
                    notes.Add("撤离线索：找到保护区筛查棚地址。");
                }
            }
            if (locationId == "safezone_edge" || location.Icons.Contains("safezone"))
            {
                if (!state.Evacuation.SafezoneConfirmed)
                {
                    state.Evacuation.SafezoneConfirmed = true;
                    notes.Add("撤离线索：确认保护区仍在短暂接收。");
                }
            }
            return notes;
        }
    }
}
