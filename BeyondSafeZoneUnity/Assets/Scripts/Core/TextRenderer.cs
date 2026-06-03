using System;
using System.Collections.Generic;
using System.Linq;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Core
{
    /// <summary>文本渲染器 —— 对应 Godot text_renderer.gd。纯格式化，只读 Model。</summary>
    public static class TextRenderer
    {
        // ============ 林行状态文本 ============

        public static string GetLinConditionText(GameState state)
        {
            string infectionLabel = "感染风险：低";
            if (state.Lin.InfectionRisk >= 5)
                infectionLabel = "感染风险：危险感染";
            else if (state.Lin.InfectionRisk >= 3)
                infectionLabel = "感染风险：发热风险";

            return $"生命 {state.Lin.Health} / 疲劳 {state.Lin.Fatigue} / 压力 {state.Lin.Stress} / {infectionLabel} / 希望 {state.Lin.Hope}";
        }

        // ============ 每日独白 ============

        public static string DailyMonologue(GameState state, int day)
        {
            var lines = new List<string>();
            int h = state.Lin.Health;
            int f = state.Lin.Fatigue;
            int s = state.Lin.Stress;
            int hp = state.Lin.Hope;
            int inf = state.Lin.InfectionRisk;
            int fd = state.Resources.Food;
            int wt = state.Resources.Water;

            if (h <= 3) lines.Add("伤口在发烫。每一步都像拖着自己的影子。");
            else if (h <= 5) lines.Add("身上多了几道疤，但还能撑。");

            if (inf >= 5) lines.Add("低头能看到手腕上的血管在变暗——不是淤青，是从皮肤底下透出来的。");
            else if (inf >= 3) lines.Add("体温比昨天高了一点。不是我多心——体温计不会骗人。");

            if (fd <= 0) lines.Add("胃已经没有什么可以收缩的了。");
            else if (fd <= 1) lines.Add("最后的食物快没了。再不去找，明天的胃会比今天更空。");
            if (wt <= 0) lines.Add("嘴唇起皮，舌根发苦。");
            else if (wt <= 1) lines.Add("水壶快见底了。每一口都得算着喝。");

            if (f >= 7) lines.Add("站着都能睡着，但躺下反而清醒——脑子里全是对明天的算盘。");
            else if (f >= 5) lines.Add("眼皮很重。如果能睡一个不被打断的整觉就好了。");

            if (s >= 7) lines.Add("手指不自觉地抖。不是冷——是脑子里那根弦绷得太久了。");
            else if (s >= 5) lines.Add("深呼吸。一切都还是可控的。大概。");

            if (hp <= 2) lines.Add("我把童年那张避难图纸又翻出来看了一遍。纸边已经起毛了。");
            else if (hp >= 6) lines.Add("收音机还在响。只要它还在说话，就说明外面还有人在维持秩序。");

            if (state.Car.Ready) lines.Add("汽车加满了油，停在后院。明天只要拧一下钥匙——我们就走。");
            else if (state.Car.Found && !state.Car.StepEngine) lines.Add("车库那辆车——线路还断着。得先把引擎的电路接上。");

            if (day >= 8 && state.Qimian.PublicClues.Count > 0) lines.Add("我不是一个人。有什么东西——或者什么人——在跟我走同样的路。");
            if (day >= 13) lines.Add("不能再等了。每多待一天，出去的路就少一条。");

            // 玩家标记感知独白
            if (state.PlayerMarks.Count > 0)
            {
                int helpCount = state.PlayerMarks.Values.Count(m => m.Type == "help");
                int dangerCount = state.PlayerMarks.Values.Count(m => m.Type == "danger");
                if (helpCount > 0 && day >= 10)
                    lines.Add("我把求助标记留在诊所门口——如果有人在看着，他们应该能看懂。");
                if (dangerCount > 0 && day >= 7)
                    lines.Add("地图上画了叉的地方越来越多。得记住——不能走那边。");
            }

            if (lines.Count == 0) return "";
            return "\n\n" + string.Join(" ", lines);
        }

        // ============ 地点标签（含玩家标记） ============

        public static string GetLocationLabel(GameState state, string locationId)
        {
            if (!state.Locations.TryGetValue(locationId, out var location)) return "未知地点";
            var visitLabel = location.Visited ? "已搜" : "未搜";
            var stockLabel = IsLocationDepleted(location) ? "已空" : "有物资";
            var rangeLabel = location.Range <= state.Bike.Range ? "可达" : "过远（需修车）";
            var bloodMoonWarning = "";
            int nextDay = state.Day + 1;
            if (IsBloodMoonDay(nextDay))
                bloodMoonWarning = " / 明晚血月";
            var markSuffix = PlayerMarkSuffix(state, locationId);

            return $"{location.Name} / {location.Ring} / 尸群{location.Zombies} / 危险{location.DangerLevel} / {visitLabel} / {stockLabel} / {rangeLabel}{bloodMoonWarning}{markSuffix}";
        }

        // ============ 地点卡片 ============

        public static string GetLocationCardText(GameState state, string locationId)
        {
            if (!state.Locations.TryGetValue(locationId, out var location)) return "未知节点";
            var iconDesc = DescribeIcons(location.Icons);
            var routeDetail = $"路程：{location.RouteTime} 小时";
            var roadDetail = $"路况：{location.RoadCondition}（{RoadConditionNote(location.RoadCondition)}）";
            var rangeDetail = RangeAffordanceText(state, locationId, location);
            var playerMarkNote = PlayerMarkDetailText(state, locationId);

            return $"{GetLocationLabel(state, locationId)}\n" +
                   $"资源倾向：{location.ResourceTendency} / 危险等级：{location.DangerLevel}\n" +
                   $"{routeDetail} / {roadDetail}\n{rangeDetail}\n" +
                   $"地点特征：{iconDesc}{playerMarkNote}\n" +
                   $"{GetLocationRiskText(state, locationId)}{LocationTraceSuffix(location)}";
        }

        // ============ 房间卡片 ============

        public static string GetRoomCardText(GameState state, string roomId)
        {
            if (state.Phase != "searching" || string.IsNullOrEmpty(state.Exploration.ActiveLocation))
                return "没有进入可搜索地点。";

            if (!state.Locations.TryGetValue(state.Exploration.ActiveLocation, out var location))
                return "未知地点。";

            if (!location.Rooms.TryGetValue(roomId, out var room))
                return "未知房间。";

            var searchedLabel = room.Searched ? "已搜" : "可搜";
            if (room.Locked) searchedLabel = "锁 上锁";

            var zombieHint = RoomThreatText(state, roomId, room);
            var visibilityText = VisibilityDescription(room.Visibility);

            return $"{room.Name} / {visibilityText} / 耗时：{room.SearchTime} 小时 / {zombieHint} / {searchedLabel}";
        }

        // ============ 风险文本 ============

        public static string GetLocationRiskText(GameState state, string locationId)
        {
            if (!state.Locations.TryGetValue(locationId, out var location)) return "风险：未知。";
            int pressure = location.Zombies + location.Range + state.Bike.Noise;
            if (location.Range > state.Bike.Range) return "风险：距离过远，今天无法稳定抵达。";
            if (pressure <= 4) return "风险：低，适合搜刮。";
            if (pressure <= 7) return "风险：中，可能增加疲劳和压力。";
            return "风险：高，尸群密集，可能受伤。";
        }

        // ============ 异常档案文本 ============

        public static string GetAnomalyDossierText(GameState state)
        {
            if (state.AnomalyDossier.Count == 0) return "暂无异常记录。";
            var lines = state.AnomalyDossier.Select(entry =>
                $"第{entry.Day}天 {entry.LocationId}：{entry.ClueText}" +
                (string.IsNullOrEmpty(entry.Conclusion) ? "" : $" → {entry.Conclusion}"));
            return string.Join("\n", lines);
        }

        // ============ 玩家标记 → 祁眠感知链文本 ============

        /// <summary>结尾日志：玩家标记如何进入祁眠感知</summary>
        public static string GetPlayerMarkPerceptionChain(GameState state)
        {
            if (state.PlayerMarks.Count == 0) return "";

            var lines = new List<string>();
            lines.Add("═══ 玩家标记 → 祁眠感知链 ═══");

            int helpCount = state.PlayerMarks.Values.Count(m => m.Type == "help");
            int dangerCount = state.PlayerMarks.Values.Count(m => m.Type == "danger");
            int routeCount = state.PlayerMarks.Values.Count(m => m.Type == "route");
            int reserveCount = state.PlayerMarks.Values.Count(m => m.Type == "reserve");

            lines.Add($"林行在地图上留下了 {state.PlayerMarks.Count} 个标记：求助 {helpCount}、危险 {dangerCount}、路线 {routeCount}、储备 {reserveCount}");
            lines.Add("");

            foreach (var kv in state.PlayerMarks)
            {
                string locId = kv.Key;
                var mark = kv.Value;
                string locName = state.Locations.TryGetValue(locId, out var loc) ? loc.Name : locId;
                string icon = mark.Type switch
                {
                    "help" => "[求助]",
                    "danger" => "[危险]",
                    "route" => "[路线]",
                    "reserve" => "[储备]",
                    _ => "[?]"
                };
                lines.Add($"  {icon} 第{mark.Day}天 {locName}：{mark.Note}");

                // 检查祁眠是否在同一天或之后访问了此地
                bool qimianVisited = state.Qimian.Log.Any(log =>
                    log.Day >= mark.Day &&
                    QimianActionTouchesLocation(log, locId, state));
                if (qimianVisited)
                    lines.Add($"    → 祁眠感知到了这个标记，并在后续行动中做出回应。");
                else
                    lines.Add($"    → 祁眠未直接感知此标记，但标记影响了林行的路径选择。");
            }

            // 诊所 help 标记专项
            if (state.PlayerMarks.TryGetValue("clinic", out var clinicMark) && clinicMark.Type == "help")
            {
                lines.Add("");
                lines.Add("▸ 诊所异常 → help 标记 → 祁眠读取 → 匿名药品");
                lines.Add($"  第{clinicMark.Day}天林行在诊所留下求助标记：「{clinicMark.Note}」");
                bool qimianLeftMeds = state.Qimian.Log.Any(log =>
                    log.Day >= clinicMark.Day &&
                    (log.Title.Contains("取药") || log.Title.Contains("观察") || log.Title.Contains("诊")));
                if (qimianLeftMeds)
                    lines.Add("  → 祁眠在后续巡逻中读取了此标记，在诊所门口留下了匿名药品。");
                else
                    lines.Add("  → 祁眠未直接回应此标记，但诊所药柜的翻动痕迹暗示有人来过。");
            }

            return string.Join("\n", lines);
        }

        private static bool QimianActionTouchesLocation(QimianLogEntry log, string locationId, GameState state)
        {
            // 检查祁眠日志条目的 Truth/Title 是否涉及该地点
            string text = $"{log.Title} {log.Truth}";
            var locNameMap = new Dictionary<string, string[]>
            {
                {"clinic", new[]{"诊","药","诊所","clinic"}},
                {"supermarket", new[]{"超市","supermarket","货架","食物"}},
                {"school", new[]{"学校","school","桥","摩托"}},
                {"subway", new[]{"地铁","subway","地铁口"}},
                {"bridge_camp", new[]{"桥洞","营地","camp","绷带"}},
                {"police", new[]{"派出所","police","警"}},
                {"gas_station", new[]{"加油站","gas","汽油"}},
                {"apartment", new[]{"公寓","apartment","祁烬"}},
                {"quarantine", new[]{"隔离","quarantine","标本"}},
                {"safezone_edge", new[]{"哨卡","safezone","保护区"}},
            };

            if (locNameMap.TryGetValue(locationId, out var keywords))
                return keywords.Any(k => text.Contains(k));
            return text.Contains(locationId);
        }

        // ============ Private Helpers ============

        private static bool IsBloodMoonDay(int day) => new[] { 7, 15 }.Contains(day);

        private static bool IsLocationDepleted(LocationState location)
        {
            return location.Resources.Values.All(v => v <= 0);
        }

        private static string LocationTraceSuffix(LocationState location)
        {
            return location.QimianTrace ? " [祁眠异常]" : "";
        }

        private static string PlayerMarkSuffix(GameState state, string locationId)
        {
            if (!state.PlayerMarks.TryGetValue(locationId, out var mark)) return "";
            return mark.Type switch
            {
                "help" => " [求助标记]",
                "danger" => " [危险标记]",
                "route" => " [路线标记]",
                "reserve" => " [储备标记]",
                _ => ""
            };
        }

        private static string PlayerMarkDetailText(GameState state, string locationId)
        {
            if (!state.PlayerMarks.TryGetValue(locationId, out var mark)) return "";
            string icon = mark.Type switch
            {
                "help" => "求助",
                "danger" => "危险",
                "route" => "路线",
                "reserve" => "储备",
                _ => "?"
            };
            return $"\n林行标记：[{icon}] {mark.Note}";
        }

        private static string RoomThreatText(GameState state, string roomId, RoomState room)
        {
            if (room.HiddenZombies <= 0) return "安全";
            if (!string.IsNullOrEmpty(state.Exploration.ActiveLocation) && state.Exploration.LuredRooms.Contains(roomId))
                return "尸群潜伏（已引开）";
            return "尸群潜伏（未排除）";
        }

        private static string VisibilityDescription(string visibility) => visibility switch
        {
            "明亮" => "能见度：明亮",
            "昏暗" => "能见度：昏暗",
            "黑暗" => "能见度：黑暗",
            _ => $"能见度：{visibility}"
        };

        private static string DescribeIcons(List<string> icons)
        {
            if (icons == null || icons.Count == 0) return "无";
            var labels = icons.Select(icon =>
                LocationData.ICON_LABELS != null && LocationData.ICON_LABELS.TryGetValue(icon, out var label) ? label : icon);
            return string.Join("，", labels);
        }

        private static string RoadConditionNote(string condition)
        {
            if (LocationData.ROAD_NOTES != null && LocationData.ROAD_NOTES.TryGetValue(condition, out var note))
                return note;
            return "路况不明";
        }

        private static string RangeAffordanceText(GameState state, string locationId, LocationState location)
        {
            int bikeRange = state.Bike.Range;
            int locRange = location.Range;
            if (locRange <= bikeRange)
                return $"自行车范围 {bikeRange}/{locRange}：可抵达";
            int diff = locRange - bikeRange;
            return $"自行车范围 {bikeRange}/{locRange}：距离不足（差 {diff}），需先修车";
        }
    }
}
