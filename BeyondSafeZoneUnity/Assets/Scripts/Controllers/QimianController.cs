using System;
using System.Collections.Generic;
using System.Linq;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Controllers
{
    /// <summary>祁眠 AI 决策引擎 —— 对应 Godot qimian_controller.gd</summary>
    public static class QimianController
    {
        public static void ResolveForDay(GameState state, int day)
        {
            if (day < BalanceData.QIMIAN_AWAKE_DAY) return;
            state.Qimian.Awake = true;
            var qs = state.Qimian.AiState;
            RecordPerceivedPlayerMarks(state, day);
            RespondToClinicHelpMark(state, day);

            // Update moto_tier
            if (BalanceData.QIMIAN_MOTO_UPGRADE_DAYS.TryGetValue(day, out int newTier))
            {
                qs.MotoTier = newTier;
                if (day == 8)
                {
                    state.Qimian.Log.Add(new QimianLogEntry
                    {
                        Day = day, Title = "摩托升级",
                        Truth = "祁眠在别墅找到了备用零件和工具，把摩托改装到二级——可以跑更远的路了。",
                        AiReplay = "摩托升级：范围扩大至中远圈。",
                        SubjectiveFragment = "引擎声音更稳了。今晚可以骑远一点。"
                    });
                }
            }

            // Step 1: Scheduled tasks always run first
            if (QimianPlanData.PLAN.TryGetValue(day, out var actions))
            {
                foreach (var action in actions)
                {
                    ApplyAction(state, day, action);
                    UpdateAiState(state, action);
                }
                return;
            }

            // Step 2: AI decision for non-scheduled days (7, 9, 11, 13)
            var perceivable = Perceive(state, day);
            var candidates = CollectTasks(state, day, perceivable);
            if (candidates.Count == 0) return;
            var chosen = RankAndSelect(candidates);
            Execute(state, day, chosen, perceivable);
        }

        private static void ApplyAction(GameState state, int day, QimianAction action)
        {
            if (!string.IsNullOrEmpty(action.Location) && state.Locations.TryGetValue(action.Location, out var location))
            {
                location.QimianTrace = true;
                if (!location.Icons.Contains("qimian"))
                    location.Icons.Add("qimian");
                if (!string.IsNullOrEmpty(action.Resource) && location.Resources.ContainsKey(action.Resource))
                    location.Resources[action.Resource] = Math.Max(0, location.Resources[action.Resource] + action.Amount);
                if (action.ZombieDelta != 0)
                    location.Zombies = Math.Max(0, location.Zombies + action.ZombieDelta);
            }
            if (action.ResourceGain.Count > 0)
            {
                foreach (var kv in action.ResourceGain)
                    state.Resources.Set(kv.Key, state.Resources.Get(kv.Key) + kv.Value);
            }
            state.Qimian.PublicClues.Add(action.PublicClue);
            state.Qimian.Log.Add(new QimianLogEntry
            {
                Day = day,
                Title = action.Title,
                Truth = action.Truth,
                PublicClue = action.PublicClue,
                AiReplay = action.AiReplay,
                SubjectiveFragment = action.SubjectiveFragment
            });
        }

        private static Dictionary<string, object> Perceive(GameState state, int day)
        {
            var qs = state.Qimian.AiState;
            var p = new Dictionary<string, object>
            {
                ["day"] = day,
                ["moon"] = "normal",
                ["weather"] = "clear",
                ["available_zones"] = new List<string>(),
                ["zombie_hotspots"] = new List<string>(),
                ["survivor_in_need"] = false,
                ["qijin_signal_active"] = qs.QijinClues >= 1,
                ["supply_shortage"] = qs.Inventory.Food <= 0 || qs.Inventory.Meds <= 0,
                ["perceived_marks"] = new List<string>()  // 感知到的玩家标记
            };

            if (Constants.BLOOD_MOON_DAYS.Contains(day)) p["moon"] = "blood_moon";
            else if (day >= 11) p["moon"] = "red_tide";

            var availableZones = (List<string>)p["available_zones"];
            if (qs.ZoneHeat["A"] < BalanceData.QIMIAN_ZONE_HEAT_MAX && qs.Exposure < BalanceData.QIMIAN_ZONE_A_EXPOSURE_MAX)
                availableZones.Add("A");
            if (qs.MotoTier >= 2 && qs.ZoneHeat["B"] < BalanceData.QIMIAN_ZONE_HEAT_MAX && qs.Exposure < BalanceData.QIMIAN_ZONE_B_EXPOSURE_MAX)
                availableZones.Add("B");
            if (qs.MotoTier >= 3 && qs.ZoneHeat["C"] < BalanceData.QIMIAN_ZONE_C_HEAT_MAX && qs.Exposure < BalanceData.QIMIAN_ZONE_C_EXPOSURE_MAX)
                availableZones.Add("C");

            var hotspots = (List<string>)p["zombie_hotspots"];
            foreach (var kv in state.Locations)
            {
                if (kv.Value.Zombies >= 4) hotspots.Add(kv.Key);
            }

            if (day >= 6) p["survivor_in_need"] = true;

            // ---- 读取玩家标记 ----
            var perceivedMarks = (List<string>)p["perceived_marks"];
            foreach (var kv in state.PlayerMarks)
            {
                string locId = kv.Key;
                var mark = kv.Value;
                if (mark.Day > day) continue;  // 不能感知未来的标记

                perceivedMarks.Add(locId);

                // help 标记：触发 supply_drop 倾向
                if (mark.Type == "help" && !string.IsNullOrEmpty(locId))
                    p["survivor_in_need"] = true;

                // danger 标记：祁眠避开该地点（降低 zone heat 但可能被巡逻覆盖）
                if (mark.Type == "danger" && state.Locations.TryGetValue(locId, out var dangerLoc))
                {
                    // 如果该地点在 A 区，A 区热度不增加
                    if (locId == "clinic" || locId == "convenience" || locId == "bike_shop")
                        qs.ZoneHeat["A"] = Math.Min(BalanceData.QIMIAN_ZONE_HEAT_MAX, qs.ZoneHeat["A"] + 1);
                }
            }

            return p;
        }

        private static void RecordPerceivedPlayerMarks(GameState state, int day)
        {
            foreach (var kv in state.PlayerMarks)
            {
                string locationId = kv.Key;
                var mark = kv.Value;
                if (mark.Day > day || mark.Type != "help") continue;

                string locationName = state.Locations.TryGetValue(locationId, out var location)
                    ? location.Name
                    : locationId;
                string title = $"感知玩家标记：{locationName}";
                bool alreadyLogged = state.Qimian.Log.Any(entry => entry.Day == day && entry.Title == title);
                if (alreadyLogged) continue;

                string publicClue = $"{locationName}附近的求助标记被人轻轻描深了一笔。";
                state.Qimian.PublicClues.Add(publicClue);
                state.Qimian.Log.Add(new QimianLogEntry
                {
                    Day = day,
                    Title = title,
                    Truth = $"祁眠夜里经过{locationName}附近，读到了林行留下的求助标记。",
                    PublicClue = publicClue,
                    AiReplay = $"感知输入：{locationName}存在求助标记。该地点会提高匿名补给候选行动权重。",
                    SubjectiveFragment = "有人留下了记号。不是给我的，但我看懂了。"
                });
            }
        }

        private static void RespondToClinicHelpMark(GameState state, int day)
        {
            if (!state.PlayerMarks.TryGetValue("clinic", out var mark)) return;
            if (mark.Day > day || mark.Type != "help") return;
            if (!state.Locations.TryGetValue("clinic", out var clinic)) return;

            const string title = "响应玩家标记：社区诊所";
            bool alreadyResponded = state.Qimian.Log.Any(entry => entry.Title == title);
            if (alreadyResponded) return;

            clinic.QimianTrace = true;
            if (!clinic.Icons.Contains("qimian"))
                clinic.Icons.Add("qimian");
            clinic.Resources["meds"] = clinic.Resources.TryGetValue("meds", out int meds) ? meds + 1 : 1;

            const string clueText = "社区诊所门口多了一包匿名药品，旁边有一条很浅的箭头，正压在林行留下的求助标记边上。";
            const string conclusion = "未知行动者能理解标记，并且愿意用匿名方式回应求助。";
            bool dossierExists = state.AnomalyDossier.Any(entry =>
                entry.LocationId == "clinic" &&
                entry.ClueText == clueText &&
                entry.Conclusion == conclusion);
            if (!dossierExists)
            {
                state.AnomalyDossier.Add(new AnomalyDossierEntry
                {
                    Day = day,
                    LocationId = "clinic",
                    ClueText = clueText,
                    Conclusion = conclusion
                });
            }

            string publicClue = "社区诊所出现匿名药品：求助标记旁边多了一条浅箭头，像是有人读懂后留下的回应。";
            state.Qimian.PublicClues.Add(publicClue);
            state.Qimian.Log.Add(new QimianLogEntry
            {
                Day = day,
                Title = title,
                Truth = "祁眠读懂了林行留在诊所的求助标记，趁夜把药品和箭头痕迹放在不会被雨淋到的位置。",
                PublicClue = publicClue,
                AiReplay = "任务：匿名补给。输入：社区诊所存在求助标记。结果：诊所药品+1，写入异常档案，地点出现祁眠痕迹。",
                SubjectiveFragment = "标记还新。有人需要药。我不能留下名字。"
            });
        }

        private static List<Dictionary<string, object>> CollectTasks(GameState state, int day, Dictionary<string, object> p)
        {
            var tasks = new List<Dictionary<string, object>>();
            var qs = state.Qimian.AiState;
            var availableZones = (List<string>)p["available_zones"];
            var perceivedMarks = (List<string>)p["perceived_marks"];

            if (availableZones.Count > 0)
                tasks.Add(new() { ["id"] = "patrol", ["type"] = "routine", ["zone"] = availableZones[0], ["priority"] = 10 });
            if ((bool)p["supply_shortage"] && availableZones.Count > 0)
                tasks.Add(new() { ["id"] = "scavenge", ["type"] = "routine", ["zone"] = availableZones[0], ["location"] = "supermarket", ["priority"] = 50 });
            if ((bool)p["survivor_in_need"] && availableZones.Count > 0)
            {
                // 优先响应 help 标记的地点
                string dropTarget = "bridge_camp";
                foreach (var markLocId in perceivedMarks)
                {
                    if (state.PlayerMarks.TryGetValue(markLocId, out var mark) && mark.Type == "help")
                    {
                        dropTarget = markLocId;
                        break;
                    }
                }
                tasks.Add(new() { ["id"] = "supply_drop", ["type"] = "routine", ["zone"] = "A", ["priority"] = 85, ["location"] = dropTarget, ["player_mark_response"] = dropTarget != "bridge_camp" });
            }
            if ((bool)p["qijin_signal_active"] && availableZones.Contains("B") && qs.QijinClues < 3)
                tasks.Add(new() { ["id"] = "track_qijin", ["type"] = "event", ["zone"] = "B", ["priority"] = 100 });
            if (qs.Exposure >= 5 || tasks.Count == 0)
                tasks.Add(new() { ["id"] = "rest", ["type"] = "routine", ["zone"] = "hideout", ["priority"] = 0 });

            return tasks;
        }

        private static Dictionary<string, object> RankAndSelect(List<Dictionary<string, object>> candidates)
        {
            if (candidates.Count == 0) return new();
            candidates.Sort((a, b) => ((int)b["priority"]).CompareTo((int)a["priority"]));
            return candidates[0];
        }

        private static void Execute(GameState state, int day, Dictionary<string, object> task, Dictionary<string, object> p)
        {
            var qs = state.Qimian.AiState;
            string taskId = (string)task["id"];
            string zone = task.TryGetValue("zone", out var z) ? (string)z : "";

            switch (taskId)
            {
                case "patrol":
                    qs.Exposure = Math.Min(BalanceData.QIMIAN_EXPOSURE_MAX, qs.Exposure + BalanceData.QIMIAN_PATROL_EXPOSURE);
                    state.Qimian.Log.Add(new()
                    {
                        Day = day, Title = "夜间巡逻",
                        Truth = $"祁眠骑摩托在{zone}区巡逻，标记安全路线。",
                        AiReplay = $"任务：巡逻。区域：{zone}。暴露+1。",
                        SubjectiveFragment = "安静的一夜。至少这个方向还安全。"
                    });
                    break;
                case "scavenge":
                    qs.Inventory.Food += BalanceData.QIMIAN_SCAVENGE_FOOD;
                    qs.Inventory.Meds += BalanceData.QIMIAN_SCAVENGE_MEDICINE;
                    qs.Exposure = Math.Min(BalanceData.QIMIAN_EXPOSURE_MAX, qs.Exposure + BalanceData.QIMIAN_SCAVENGE_EXPOSURE);
                    state.Qimian.Log.Add(new()
                    {
                        Day = day, Title = "夜间搜刮",
                        Truth = $"祁眠在{zone}区搜刮补给，拿了食物和药品。",
                        PublicClue = "超市后门被人从里面用铁丝重新别上——上次来的时候不是这样的。",
                        AiReplay = $"任务：搜刮。目标：超市。获得食物+1药品+1。暴露+1。",
                        SubjectiveFragment = "只拿够用的。剩下的——有人比我更需要。"
                    });
                    break;
                case "supply_drop":
                    qs.Exposure = Math.Max(0, qs.Exposure + BalanceData.QIMIAN_DROP_EXPOSURE);
                    bool isMarkResponse = task.TryGetValue("player_mark_response", out var mr) && mr is bool b && b;
                    string dropLocId = task.TryGetValue("location", out var dl) ? (string)dl : "bridge_camp";
                    string dropLocName = state.Locations.TryGetValue(dropLocId, out var dropLoc) ? dropLoc.Name : dropLocId;
                    string clueMsg = $"桥洞营地外多了一包绷带和水——放在不会被雨淋到的位置。";
                    string truthMsg = $"祁眠骑车经过桥洞营地，看到老太太和发烧的小女孩，把绷带和水放在营地外围。";
                    string fragmentMsg = "那个小女孩烧退了。我只是放了东西在那里——不是我治好的。";

                    if (isMarkResponse)
                    {
                        clueMsg = $"{dropLocName}外多了一包未拆封的医疗用品——林行留下的求助标记旁边，多了回应的痕迹。";
                        truthMsg = $"祁眠读取了林行在{dropLocName}留下的求助标记，在夜间骑行经过时留下了匿名药品和绷带。";
                        fragmentMsg = $"有人在这里留下了标记。不是求救——是在告诉后来的人这里有什么。我放了些东西。希望够用。";
                    }

                    state.Qimian.PublicClues.Add(clueMsg);
                    state.Qimian.Log.Add(new()
                    {
                        Day = day,
                        Title = isMarkResponse ? $"响应玩家标记：{dropLocName}" : "匿名补给",
                        Truth = truthMsg,
                        PublicClue = clueMsg,
                        AiReplay = $"任务：匿名补给。目标：{dropLocName}。暴露-1（善意行为未暴露身份）。" +
                                   (isMarkResponse ? " 感知到了林行留下的求助标记。" : ""),
                        SubjectiveFragment = fragmentMsg
                    });
                    break;
                case "track_qijin":
                    qs.QijinClues += 1;
                    qs.Exposure = Math.Min(BalanceData.QIMIAN_EXPOSURE_MAX, qs.Exposure + BalanceData.QIMIAN_TRACK_EXPOSURE);
                    state.Qimian.Log.Add(new()
                    {
                        Day = day, Title = "追踪祁烬信号",
                        Truth = "祁眠追踪返生计划加密频段，找到了祁烬最近活动过的地点——桌上水还是热的。",
                        AiReplay = $"任务：追踪祁烬。区域：{zone}。祁烬线索+{qs.QijinClues}。暴露+2。",
                        SubjectiveFragment = "水还是热的。他十分钟前还在这里。"
                    });
                    break;
                case "rest":
                    qs.Exposure = Math.Max(0, qs.Exposure + BalanceData.QIMIAN_REST_EXPOSURE);
                    state.Qimian.Log.Add(new()
                    {
                        Day = day, Title = "在别墅休整",
                        Truth = "祁眠今晚没有外出，在别墅补睡和整理线索。",
                        AiReplay = "任务：休整。暴露-2。",
                        SubjectiveFragment = "今晚没有任务。睡了一觉——很久没有睡这么死了。"
                    });
                    break;
            }

            if (zone == "A" || zone == "B" || zone == "C")
                qs.ZoneHeat[zone] = Math.Min(BalanceData.QIMIAN_ZONE_HEAT_MAX, qs.ZoneHeat[zone] + 1);
        }

        private static void UpdateAiState(GameState state, QimianAction action)
        {
            var qs = state.Qimian.AiState;
            string title = action.Title;
            if (title.Contains("诊") || title.Contains("取药") || title.Contains("超市") || title.Contains("尸群"))
                qs.Exposure = Math.Min(BalanceData.QIMIAN_EXPOSURE_MAX, qs.Exposure + 1);
            if (title.Contains("清桥"))
            {
                qs.Exposure = Math.Min(BalanceData.QIMIAN_EXPOSURE_MAX, qs.Exposure + 2);
                qs.ZoneHeat["B"] = Math.Min(BalanceData.QIMIAN_ZONE_HEAT_MAX, qs.ZoneHeat["B"] + 1);
            }
            if (title.Contains("观察"))
                qs.Exposure = Math.Max(0, qs.Exposure - 1);
            if (title.Contains("藏身"))
            {
                qs.Exposure = Math.Min(BalanceData.QIMIAN_EXPOSURE_MAX, qs.Exposure + 2);
                qs.ZoneHeat["C"] = Math.Min(BalanceData.QIMIAN_ZONE_HEAT_MAX, qs.ZoneHeat["C"] + 1);
            }
        }
    }
}
