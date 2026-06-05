using System;
using System.Collections.Generic;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Controllers
{
    /// <summary>据点设施系统 —— 对应 Godot shelter_controller.gd</summary>
    public static class ShelterController
    {
        public static string PerformAction(GameState state, string actionId)
        {
            string result;
            switch (actionId)
            {
                case "build_bed":
                    result = BuildFacility(state, "bed", materialsCost: 2, partsCost: 0, "林行用旧门板和布料搭出一张能睡人的床。");
                    break;

                case "build_workbench":
                    result = BuildFacility(state, "workbench", materialsCost: 2, partsCost: 1, "林行把桌板、钳子和几件还能用的工具拼成了工作台。");
                    break;

                case "build_stove":
                    result = BuildFacility(state, "stove", materialsCost: 2, partsCost: 1, "林行用铁桶和管件搭起火炉，屋里终于有了稳定热源。");
                    break;

                case "rest_bed":
                    if (!IsBuilt(state, "bed"))
                    {
                        result = "没有床，林行只能靠墙眯一会儿，身体没有真正恢复。";
                        break;
                    }
                    state.Lin.Fatigue = Math.Max(0, state.Lin.Fatigue - BalanceData.SHELTER_REST_FATIGUE);
                    state.Lin.Stress = Math.Max(0, state.Lin.Stress - BalanceData.SHELTER_REST_STRESS);
                    MarkFacilityUsed(state, "bed");
                    result = "林行在床铺上断续睡了一会儿，疲劳和压力都降下来一点。";
                    break;

                case "workbench_repair":
                    if (!IsBuilt(state, "workbench"))
                    {
                        result = "没有工作台，林行没法稳定修理自行车。";
                        break;
                    }
                    if (Spend(state, "parts", BalanceData.SHELTER_REPAIR_BIKE_PARTS))
                    {
                        state.Bike.Durability += BalanceData.SHELTER_REPAIR_BIKE_DURABILITY;
                        state.Bike.Range = Math.Min(BalanceData.SHELTER_REPAIR_BIKE_MAX_RANGE,
                            state.Bike.Range + BalanceData.SHELTER_REPAIR_BIKE_RANGE);
                        state.Bike.Noise = Math.Max(0, state.Bike.Noise - BalanceData.SHELTER_REPAIR_BIKE_NOISE);
                        MarkFacilityUsed(state, "workbench");
                        if (state.Bike.Range >= BalanceData.SHELTER_REPAIR_BIKE_MAX_RANGE)
                            state.Evacuation.BikeReady = true;
                        result = "林行在工作台修好车链和刹车，自行车更适合远行。";
                    }
                    else
                        result = "没有足够零件，工作台只能摆着拆开的工具。";
                    break;

                case "barricade_windows":
                    if (Spend(state, "materials", BalanceData.SHELTER_BARRICADE_MATERIALS))
                    {
                        state.Shelter.Door += BalanceData.SHELTER_BARRICADE_DOOR;
                        state.Shelter.Defense += BalanceData.SHELTER_BARRICADE_DEFENSE;
                        state.Shelter.Facilities["barricade"].Level += 1;
                        MarkFacilityUsed(state, "barricade");
                        result = "林行把窗框和门缝重新钉死，血月前的防线厚了一层。";
                    }
                    else
                        result = "建材不足，封窗只能停在一半。";
                    break;

                case "radio_broadcast":
                    if (Spend(state, "fuel", BalanceData.SHELTER_RADIO_FUEL))
                    {
                        state.Lin.Hope += BalanceData.SHELTER_RADIO_HOPE;
                        state.Shelter.Noise += BalanceData.SHELTER_RADIO_NOISE;
                        MarkFacilityUsed(state, "radio");
                        if (state.Day >= 3) state.Evacuation.SafezoneConfirmed = true;
                        if (state.Day >= 9) state.Evacuation.AddressKnown = true;
                        result = RadioMessageForDay(state.Day);
                    }
                    else
                        result = "发电机没有燃料，收音机只剩沙沙声。";
                    break;

                case "organize_storage":
                    state.Shelter.SupplyPreservation = Math.Min(BalanceData.SHELTER_STORAGE_MAX_PRESERVATION,
                        state.Shelter.SupplyPreservation + BalanceData.SHELTER_STORAGE_PRESERVATION);
                    state.Bike.Capacity += BalanceData.SHELTER_STORAGE_CAPACITY;
                    MarkFacilityUsed(state, "storage");
                    result = "林行把食物、水和路上要带的东西重新打包，撤离时能少丢一些。";
                    break;

                case "treat_wound":
                    if (Spend(state, "meds", BalanceData.SHELTER_TREAT_MEDS))
                    {
                        state.Lin.Health = Math.Min(10, state.Lin.Health + BalanceData.SHELTER_TREAT_HEALTH);
                        state.Lin.InfectionRisk = Math.Max(0, state.Lin.InfectionRisk - BalanceData.SHELTER_TREAT_INFECTION);
                        result = "林行用药品处理伤口，体温稍微压下去，感染风险降低。";
                    }
                    else
                        result = "没有药品，林行只能用清水压住伤口。";
                    break;

                case "workbench_car":
                    if (!IsBuilt(state, "workbench"))
                    {
                        result = "没有工作台，林行没法拆开汽车线路。";
                        break;
                    }
                    return CarController.Repair(state);

                case "fortify":
                    if (!IsBuilt(state, "workbench"))
                    {
                        result = "没有工作台，林行只能临时堵住门缝，做不了可靠加固。";
                        break;
                    }
                    if (Spend(state, "materials", BalanceData.SHELTER_FORTIFY_MATERIALS))
                    {
                        state.Shelter.Door += BalanceData.SHELTER_FORTIFY_DOOR;
                        state.Shelter.Defense += BalanceData.SHELTER_FORTIFY_DEFENSE;
                        MarkFacilityUsed(state, "workbench");
                        result = "林行用木板和铁丝加固门窗。";
                    }
                    else result = "建材不足，无法加固。";
                    break;

                case "quiet":
                    state.Shelter.Noise = Math.Max(0, state.Shelter.Noise - BalanceData.SHELTER_QUIET_NOISE);
                    state.Lin.Stress += BalanceData.SHELTER_QUIET_STRESS;
                    result = "林行拆掉会响的杂物，据点安静了一些。";
                    break;

                case "mask_scent":
                    if (Spend(state, "materials", BalanceData.SHELTER_MASK_MATERIALS))
                    {
                        state.Shelter.Scent = Math.Max(0, state.Shelter.Scent - BalanceData.SHELTER_MASK_SCENT);
                        result = "林行封住垃圾和血腥味，降低尸群注意。";
                    }
                    else result = "缺少布料和胶带，气味遮蔽失败。";
                    break;

                case "repair_bike":
                    return PerformAction(state, "workbench_repair");
                case "radio":
                    return PerformAction(state, "radio_broadcast");

                default:
                    result = "林行什么也没来得及做。";
                    break;
            }
            state.LastEvent = result;
            state.Phase = "night";
            return state.LastEvent;
        }

        public static Dictionary<string, FacilityState> DefaultFacilities() => FacilityData.Defaults();

        public static void MarkFacilityUsed(GameState state, string facilityId)
        {
            if (state.Shelter.Facilities.TryGetValue(facilityId, out var f))
                f.UsedToday = true;
        }

        public static void ResetFacilityUse(GameState state)
        {
            foreach (var f in state.Shelter.Facilities.Values)
                f.UsedToday = false;
        }

        private static string BuildFacility(GameState state, string facilityId, int materialsCost, int partsCost, string successText)
        {
            if (!state.Shelter.Facilities.TryGetValue(facilityId, out var facility))
                return "林行找不到可以施工的位置。";
            if (facility.Built)
                return $"{facility.Name}已经能用了。";
            if (state.Resources.Materials < materialsCost || state.Resources.Parts < partsCost)
                return $"建造{facility.Name}需要建材 {materialsCost}、零件 {partsCost}，现在材料不够。";

            state.Resources.Materials -= materialsCost;
            state.Resources.Parts -= partsCost;
            facility.Built = true;
            facility.UsedToday = true;
            return successText;
        }

        private static bool IsBuilt(GameState state, string facilityId)
        {
            return state.Shelter.Facilities.TryGetValue(facilityId, out var facility) && facility.Built;
        }

        private static bool Spend(GameState state, string resourceName, int amount)
        {
            if (state.Resources.Get(resourceName) < amount) return false;
            state.Resources.Set(resourceName, state.Resources.Get(resourceName) - amount);
            return true;
        }

        private static string RadioMessageForDay(int day)
        {
            if (day >= 14) return "紧急广播：超大型尸潮逼近，保护区临时开放外圈接收窗口。所有外围幸存者，这是最后撤离机会。";
            if (day >= 11) return "收音机警告：红潮区域扩大，保护区外围筛查站已加固。";
            if (day >= 9) return "广播短暂说清保护区外圈筛查棚地址，但提醒所有人必须接受感染初筛。";
            if (day >= 5) return "广播夹杂着陌生敲击声，有人正在保护区外转移幸存者。";
            return "断续广播提到保护区仍在接收幸存者，但外围路线已经封锁。";
        }
    }
}