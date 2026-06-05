using System;
using System.Linq;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Controllers
{
    /// <summary>汽车系统 —— 对应 Godot car_controller.gd</summary>
    public static class CarController
    {
        public static string Repair(GameState state)
        {
            if (!state.Car.Found)
            {
                state.LastEvent = "林行还没找到能用的汽车。修理铺后院的车库或许有线索。";
                return state.LastEvent;
            }
            if (state.Car.Ready)
            {
                state.LastEvent = "汽车已经修好。油箱加满，引擎能发动。";
                return state.LastEvent;
            }
            // Step 1: engine wiring
            if (!state.Car.StepEngine)
            {
                if (Spend(state, "materials", BalanceData.CAR_REPAIR_ENGINE_MATERIALS) &&
                    Spend(state, "parts", BalanceData.CAR_REPAIR_ENGINE_PARTS))
                {
                    state.Car.StepEngine = true;
                    MarkWorkbench(state);
                    state.LastEvent = "林行接好引擎线路，仪表盘亮了——电路通了。还需要换轮胎、装电瓶、加汽油。";
                }
                else
                    state.LastEvent = $"引擎线路需要建材×{BalanceData.CAR_REPAIR_ENGINE_MATERIALS}和零件×{BalanceData.CAR_REPAIR_ENGINE_PARTS}，目前的材料不够。";
                return state.LastEvent;
            }
            // Step 2: tire
            if (!state.Car.StepTire)
            {
                if (state.CarParts.Tire >= BalanceData.CAR_REPAIR_TIRE_COUNT &&
                    Spend(state, "parts", BalanceData.CAR_REPAIR_TIRE_PARTS))
                {
                    state.CarParts.Tire -= BalanceData.CAR_REPAIR_TIRE_COUNT;
                    state.Car.StepTire = true;
                    MarkWorkbench(state);
                    state.LastEvent = "林行卸下瘪轮胎换上新的，车身终于不再倾斜。还剩电瓶和汽油。";
                }
                else
                    state.LastEvent = $"需要轮胎×{BalanceData.CAR_REPAIR_TIRE_COUNT}和零件×{BalanceData.CAR_REPAIR_TIRE_PARTS}来换胎。";
                return state.LastEvent;
            }
            // Step 3: battery
            if (!state.Car.StepBattery)
            {
                if (state.CarParts.Battery >= BalanceData.CAR_REPAIR_BATTERY_COUNT)
                {
                    if (Spend(state, "fuel", BalanceData.CAR_REPAIR_BATTERY_FUEL))
                    {
                        state.CarParts.Battery -= BalanceData.CAR_REPAIR_BATTERY_COUNT;
                        state.Car.StepBattery = true;
                        MarkWorkbench(state);
                        state.LastEvent = "林行装上电瓶、调试引擎——发动机咳嗽两声后平稳运转。最后一步：加油。";
                    }
                    else
                        state.LastEvent = $"调试引擎需要燃料×{BalanceData.CAR_REPAIR_BATTERY_FUEL}来测试电路。";
                }
                else
                    state.LastEvent = $"需要电瓶×{BalanceData.CAR_REPAIR_BATTERY_COUNT}（派出所停车场有废弃警车可卸）和燃料×{BalanceData.CAR_REPAIR_BATTERY_FUEL}来调试。";
                return state.LastEvent;
            }
            // Step 4: gasoline
            if (!state.Car.StepFueled)
            {
                if (state.CarParts.Gasoline >= BalanceData.CAR_REPAIR_GASOLINE_COUNT)
                {
                    state.CarParts.Gasoline -= BalanceData.CAR_REPAIR_GASOLINE_COUNT;
                    state.Car.StepFueled = true;
                    state.Car.Ready = true;
                    state.Evacuation.CarReady = true;
                    MarkWorkbench(state);
                    state.Lin.Hope += 1;
                    state.LastEvent = "林行把两桶汽油倒进油箱，拧紧盖子。\n\n他坐进驾驶座，转了一下钥匙。引擎发出一声低沉的轰鸣——像一只野兽醒过来。\n\n汽车就绪。可以去保护区了。";
                }
                else
                    state.LastEvent = $"需要汽油×{BalanceData.CAR_REPAIR_GASOLINE_COUNT}来加满油箱。去哨卡、加油站或地铁口找。";
                return state.LastEvent;
            }
            return state.LastEvent;
        }

        public static string EvacuationNarrative(GameState state)
        {
            var lines = new System.Collections.Generic.List<string>
            {
                "天刚亮。远处的低吼不再是零星叫声——像瀑布一样，持续不断。"
            };
            if (state.Car.Ready)
            {
                lines.Add("林行把最后一口背包扔进后备箱。引擎第一下没着，第二下咳嗽着启动了。排气管吐出黑烟。");
                lines.Add("西线的路很通畅——桥被清理过了。后视镜里，据点的窗户像一只闭着的眼睛。他没有回头。");
                lines.Add("开了四十分钟。远郊的路遍地废弃车辆，得绕。仪表盘上温度指针开始抖。");
                lines.Add("一声刺耳的金属摩擦——左前轮爆了，或者引擎过热熄了火。汽车滑进路边沟里。不动了。");
                lines.Add("林行转动钥匙。没反应。再转——发动机呻吟了一声，像叹息。");
                lines.Add("「……操。」他下车。后备箱里能带走的只有一个背包。他塞进食物和水，背上撬棍。");
            }
            else
            {
                lines.Add("没有汽车。林行只能靠自行车——但自行车到不了远圈。");
                lines.Add("他把能带的全塞进背包，推着自行车走了最后一段能骑的路，然后弃车徒步。");
            }
            lines.Add("保护区大门在正南方约八公里。步行要三个小时。但愿能在天黑前到。但愿尸潮比他慢。");
            lines.Add("柏油路上全是裂缝。路边一辆翻倒的救护车，车门开着，里面是空的——车身上有返生计划的标志。");
            lines.Add("穿过一个无名小镇。商店卷帘门都拉下来。太安静了——只听见自己的脚步和远处不变的尸潮低吼。");
            lines.Add("天快黑了。月亮变成红色。背后能听到它们——不是一只两只，是像风暴一样的声音。不能回头。");
            return string.Join("\n", lines);
        }

        private static bool Spend(GameState state, string resourceName, int amount)
        {
            if (state.Resources.Get(resourceName) < amount) return false;
            state.Resources.Set(resourceName, state.Resources.Get(resourceName) - amount);
            return true;
        }

        private static void MarkWorkbench(GameState state)
        {
            if (state.Shelter.Facilities.TryGetValue("workbench", out var f))
                f.UsedToday = true;
        }
    }
}
