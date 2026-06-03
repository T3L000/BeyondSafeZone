using System;
using System.Collections.Generic;
using System.Linq;
using BeyondSafeZone.Core;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Controllers
{
    /// <summary>夜晚结算系统 —— 对应 Godot night_controller.gd</summary>
    public static class NightController
    {
        /// <summary>结算夜晚，返回结果文本。day>=15时返回结局。</summary>
        public static string Resolve(GameState state)
        {
            int day = state.Day;
            var nightEvents = new List<string>();

            ConsumeDailyResources(state);
            string noiseEvent = PropagateNoise(state);
            if (!string.IsNullOrEmpty(noiseEvent)) nightEvents.Add(noiseEvent);

            string infectionEvent = ResolveInfectionPressure(state);
            if (!string.IsNullOrEmpty(infectionEvent)) nightEvents.Add(infectionEvent);

            if (Constants.BLOOD_MOON_DAYS.Contains(day))
                nightEvents.Add(ResolveBloodMoon(state, day));
            else if (day >= 11 && day <= 14)
            {
                string redTideEvent = ResolveRedTide(state, day);
                if (!string.IsNullOrEmpty(redTideEvent)) nightEvents.Add(redTideEvent);
            }

            if (state.Qimian.PublicClues.Count > 0)
                nightEvents.Add(state.Qimian.PublicClues.Last());

            if (day >= Constants.MAX_DEMO_DAY)
            {
                state.DemoComplete = true;
                state.Reveal.Unlocked = true;
                if (state.Car.Ready)
                {
                    state.Car.Breakdown = "engine_overheat";
                    nightEvents.Add("左前轮在远郊路面上爆了。汽车滑进路边沟里，引擎熄火，再也发不动。");
                }
                else if (state.Car.Found)
                {
                    state.Car.Breakdown = "not_ready";
                    nightEvents.Add("汽车还没修好。林行只能背起背包，骑自行车到路尽头，然后徒步走向保护区。");
                }
                else
                {
                    nightEvents.Add("没有找到能用的载具。林行把能带的全塞进背包，推开据点的门，走进血月里。");
                }
                state.EndingState = DetermineEndingState(state);
                state.Reveal.Summary = EndingSummary(state, state.EndingState);
                state.Phase = "reveal";
                state.LastEvent = "Demo 结束。祁眠行动日志解锁。";
                return state.LastEvent;
            }

            // 推进到下一天
            string nextDayText = GameSimulation.StartDay(state, day + 1);
            if (nightEvents.Count > 0)
                state.LastEvent = $"{state.LastEvent}\n昨夜：{string.Join(" ", nightEvents)}";
            return state.LastEvent;
        }

        private static void ConsumeDailyResources(GameState state)
        {
            state.Resources.Food = Math.Max(0, state.Resources.Food - BalanceData.DAILY_CONSUME_FOOD);
            state.Resources.Water = Math.Max(0, state.Resources.Water - BalanceData.DAILY_CONSUME_WATER);
            state.Lin.Hunger = state.Resources.Food > 0 ? 0 : state.Lin.Hunger + BalanceData.HUNGER_PER_DAY_NO_FOOD;
            state.Lin.Thirst = state.Resources.Water > 0 ? 0 : state.Lin.Thirst + BalanceData.THIRST_PER_DAY_NO_WATER;
            state.Lin.Fatigue = Math.Max(0, state.Lin.Fatigue - BalanceData.FATIGUE_RECOVER_PER_NIGHT);
            state.Lin.Stress = Math.Max(0, state.Lin.Stress - state.Lin.Hope / BalanceData.HOPE_STRESS_DIVISOR);
        }

        private static string PropagateNoise(GameState state)
        {
            int totalNoise = state.Shelter.Noise + state.Exploration.Noise;
            if (totalNoise <= BalanceData.NOISE_ATTRACT_THRESHOLD) return "";

            int attracted = 0;
            foreach (var locId in new[] { "convenience", "clinic", "bike_shop" })
            {
                if (!state.Locations.TryGetValue(locId, out var loc)) continue;
                if (loc.Range > BalanceData.NOISE_ATTRACT_RANGE) continue;
                int attract = Math.Clamp(totalNoise - BalanceData.NOISE_ATTRACT_THRESHOLD, 0, BalanceData.NOISE_ATTRACT_MAX);
                loc.Zombies += attract;
                attracted += attract;
            }
            if (attracted > 0)
            {
                if (totalNoise >= 6)
                    return $"据点的噪音引来了近圈的尸群（+{attracted}），明天探索风险增加。";
                return $"夜里有些动静吸引了尸群注意（+{attracted}）。";
            }
            return "";
        }

        private static string ResolveInfectionPressure(GameState state)
        {
            if (state.Lin.InfectionRisk < BalanceData.INFECTION_CRITICAL_THRESHOLD) return "";
            state.Lin.Health = Math.Max(0, state.Lin.Health - BalanceData.INFECTION_HEALTH_PENALTY);
            state.Lin.Stress += BalanceData.INFECTION_STRESS_PENALTY;
            return "感染风险恶化，林行发热、伤口发烫，生命和压力都受到影响。";
        }

        private static string ResolveBloodMoon(GameState state, int day)
        {
            int support = QimianBloodMoonSupport(day);
            int pressure = BalanceData.BM_BASE_PRESSURE + (day / BalanceData.BM_DAY_DIVISOR) * BalanceData.BM_DAY_MULT
                + state.Shelter.Noise + state.Shelter.Scent + state.Shelter.Light
                - state.Shelter.Door - state.Shelter.Defense - support;

            if (!state.BloodMoonsResolved.Contains(day))
                state.BloodMoonsResolved.Add(day);

            if (pressure <= BalanceData.BM_LOW_THRESHOLD)
            {
                state.Lin.Hope += BalanceData.BM_LOW_HOPE;
                return "血月被稳稳撑过去，林行听见远处尸群被引开的声音。";
            }
            if (pressure <= BalanceData.BM_MID_THRESHOLD)
            {
                state.Shelter.Door = Math.Max(1, state.Shelter.Door - BalanceData.BM_MID_DOOR);
                state.Resources.Food = Math.Max(0, state.Resources.Food - BalanceData.BM_MID_FOOD);
                return "血月擦着据点过去，门窗受损，食物也少了一些。";
            }
            state.Lin.Health -= BalanceData.BM_HIGH_HEALTH;
            state.Shelter.Door = Math.Max(0, state.Shelter.Door - BalanceData.BM_HIGH_DOOR);
            return "血月冲破了外层防线，林行受伤，但仍撑到了天亮。";
        }

        private static string ResolveRedTide(GameState state, int day)
        {
            int intensity = day - BalanceData.RT_DAY_OFFSET;
            int pressure = intensity + state.Shelter.Noise + state.Shelter.Scent + state.Shelter.Light
                - state.Shelter.Door - state.Shelter.Defense;

            if (pressure <= BalanceData.RT_LOW_THRESHOLD)
            {
                state.Lin.Stress += BalanceData.RT_LOW_STRESS;
                return "红潮在窗外涌动了一夜，但没有突破防线。";
            }
            if (pressure <= BalanceData.RT_MID_THRESHOLD)
            {
                state.Lin.Stress += BalanceData.RT_MID_STRESS;
                state.Resources.Food = Math.Max(0, state.Resources.Food - BalanceData.RT_MID_FOOD);
                state.Shelter.Door = Math.Max(1, state.Shelter.Door - BalanceData.RT_MID_DOOR);
                return "红潮让尸群比预想中密集，据点门窗受损，食物也少了一些。";
            }
            state.Lin.Health = Math.Max(0, state.Lin.Health - BalanceData.RT_HIGH_HEALTH);
            state.Lin.Stress += BalanceData.RT_HIGH_STRESS;
            state.Shelter.Door = Math.Max(0, state.Shelter.Door - BalanceData.RT_HIGH_DOOR);
            state.Resources.Food = Math.Max(0, state.Resources.Food - BalanceData.RT_HIGH_FOOD);
            return "红潮的密度压过了防御，林行被碎片划伤，据点出现缺口。";
        }

        private static string DetermineEndingState(GameState state)
        {
            if (state.Lin.Health <= BalanceData.ENDING_HEALTH_DEAD) return "collapsed";
            if (state.Lin.Hunger >= BalanceData.ENDING_HUNGER_CRITICAL && state.Lin.Thirst >= BalanceData.ENDING_THIRST_CRITICAL)
                return "collapsed";
            if (state.Shelter.Door <= BalanceData.ENDING_HEALTH_DEAD && state.Lin.Health <= 2)
                return "collapsed";
            if (state.Lin.Health <= BalanceData.ENDING_HEALTH_BARELY || state.Shelter.Door <= BalanceData.ENDING_DOOR_BARELY)
                return "barely_reached_gate";
            if (state.Lin.Hunger >= BalanceData.ENDING_HUNGER_CRITICAL || state.Lin.Thirst >= BalanceData.ENDING_THIRST_CRITICAL)
                return "barely_reached_gate";
            if (state.Evacuation.SafezoneConfirmed && state.Evacuation.AddressKnown && state.Evacuation.CarReady)
                return "reached_gate_quarantine";
            return "barely_reached_gate";
        }

        private static string EndingSummary(GameState state, string endingState)
        {
            string supplyPhrase = state.Shelter.SupplyPreservation > 0 ? "他带着整理好的物资" : "";
            switch (endingState)
            {
                case "collapsed":
                    return "林行没能稳定抵达保护区大门。最后的记忆是血月下翻倒的汽车、越来越近的低吼、以及一条再也走不完的路。\n\n祁眠日志揭示：那一夜尸群中藏着改变路线的人——不是为了林行，却间接护送了无数幸存者穿过东线。";
                case "barely_reached_gate":
                    string carNarrative = CarController.EvacuationNarrative(state);
                    return $"{carNarrative}\n\n林行勉强到达保护区大门外，{supplyPhrase}通过初筛。\n他被领到 3 号隔离棚，裹着薄毯坐在折叠床上。透过棚子的塑料窗能看见探照灯扫过铁丝网。\n\n筛查棚外有人低声说——「昨晚那股尸群像被人牵走了。」\n林行想起桥洞营地的老太太说的话、家门口的匿名药品、超市被精准拿走的食物。\n他没有开口问。他只是在日记最后一页写：\n\n「那个人是谁。那个骑摩托的。桥是空的。药放在门口。我不知道。但我欠他。」\n\n翻过一页，夹着童年避难计划的那张泛黄纸。三个人的笔迹还在上面。";
                default:
                    string carNarr = CarController.EvacuationNarrative(state);
                    return $"{carNarr}\n\n林行抵达保护区大门外，{supplyPhrase}通过初筛后被要求隔离观察 48 小时。\n他走进 3 号棚，裹着薄毯坐在折叠床上。\n\n玩家随后看到祁眠藏在尸群中改变路线的完整日志——\n这不是为了林行，却间接救下了他。\n\n祁眠的每一步行动被逐帧回放：\n醒来→取药→超市夜行→骑摩托清桥→红潮夜巡逻→尸群藏身。\n那些被拿走的药品、改道的尸群、留下的箭头——都是同一双手。\n\n最后一行祁眠的日志写着：\n「那个人是谁——往大门走的那个。他看起来走了很久。」";
            }
        }

        private static int QimianBloodMoonSupport(int day)
        {
            if (!QimianPlanData.PLAN.TryGetValue(day, out var actions)) return 0;
            return actions.Sum(a => a.BloodMoonSupport);
        }
    }
}
