using System.Collections.Generic;

namespace BeyondSafeZone.Data
{
    /// <summary>数值平衡配置 —— 对应 Godot balance.gd</summary>
    public static class BalanceData
    {
        // ============ 初始资源 ============
        public static Dictionary<string, int> InitResources => new()
        {
            {"food", 5}, {"water", 5}, {"meds", 2},
            {"materials", 4}, {"parts", 1}, {"fuel", 3}
        };

        // ============ 林行初始状态 ============
        public static (int health, int hunger, int thirst, int fatigue, int stress, int infectionRisk, int hope) InitLin =>
            (10, 0, 0, 1, 2, 0, 4);

        // ============ 据点初始状态 ============
        public static (int door, int noise, int scent, int light, int defense, int escape, int supplyPreservation) InitShelter =>
            (4, 2, 2, 2, 1, 0, 0);

        // ============ 自行车初始 ============
        public static (int durability, int capacity, int range, int noise) InitBike =>
            (6, 6, 1, 1);

        // ============ 每日消耗 ============
        public const int DAILY_CONSUME_FOOD = 1;
        public const int DAILY_CONSUME_WATER = 1;

        // ============ 饥饿/口渴惩罚 ============
        public const int HUNGER_PER_DAY_NO_FOOD = 1;
        public const int THIRST_PER_DAY_NO_WATER = 1;

        // ============ 疲劳恢复 ============
        public const int FATIGUE_RECOVER_PER_NIGHT = 1;

        // ============ 希望减压 ============
        public const int HOPE_STRESS_DIVISOR = 3;

        // ============ 噪音传播 ============
        public const int NOISE_ATTRACT_THRESHOLD = 2;
        public const int NOISE_ATTRACT_MAX = 2;
        public const int NOISE_ATTRACT_RANGE = 1;

        // ============ 感染恶化 ============
        public const int INFECTION_CRITICAL_THRESHOLD = 5;
        public const int INFECTION_HEALTH_PENALTY = 1;
        public const int INFECTION_STRESS_PENALTY = 2;

        // ============ 血月公式 ============
        public const int BM_BASE_PRESSURE = 4;
        public const int BM_DAY_DIVISOR = 7;
        public const int BM_DAY_MULT = 2;
        public const int BM_LOW_THRESHOLD = 3;
        public const int BM_LOW_HOPE = 1;
        public const int BM_MID_THRESHOLD = 6;
        public const int BM_MID_DOOR = 1;
        public const int BM_MID_FOOD = 1;
        public const int BM_HIGH_HEALTH = 2;
        public const int BM_HIGH_DOOR = 2;

        // ============ 红潮公式 ============
        public const int RT_DAY_OFFSET = 10;
        public const int RT_LOW_THRESHOLD = 3;
        public const int RT_LOW_STRESS = 1;
        public const int RT_MID_THRESHOLD = 5;
        public const int RT_MID_STRESS = 1;
        public const int RT_MID_FOOD = 1;
        public const int RT_MID_DOOR = 1;
        public const int RT_HIGH_HEALTH = 1;
        public const int RT_HIGH_STRESS = 2;
        public const int RT_HIGH_DOOR = 1;
        public const int RT_HIGH_FOOD = 1;

        // ============ 结局判定阈值 ============
        public const int ENDING_HEALTH_DEAD = 0;
        public const int ENDING_HUNGER_CRITICAL = 4;
        public const int ENDING_THIRST_CRITICAL = 4;
        public const int ENDING_HEALTH_BARELY = 3;
        public const int ENDING_DOOR_BARELY = 1;

        // ============ 汽车修理成本 ============
        public const int CAR_REPAIR_ENGINE_MATERIALS = 2;
        public const int CAR_REPAIR_ENGINE_PARTS = 1;
        public const int CAR_REPAIR_TIRE_COUNT = 1;
        public const int CAR_REPAIR_TIRE_PARTS = 1;
        public const int CAR_REPAIR_BATTERY_COUNT = 1;
        public const int CAR_REPAIR_BATTERY_FUEL = 1;
        public const int CAR_REPAIR_GASOLINE_COUNT = 2;

        // ============ 据点行动 ============
        public const int SHELTER_REST_FATIGUE = 2;
        public const int SHELTER_REST_STRESS = 1;

        public const int SHELTER_REPAIR_BIKE_PARTS = 1;
        public const int SHELTER_REPAIR_BIKE_DURABILITY = 3;
        public const int SHELTER_REPAIR_BIKE_RANGE = 1;
        public const int SHELTER_REPAIR_BIKE_NOISE = 1;
        public const int SHELTER_REPAIR_BIKE_MAX_RANGE = 3;

        public const int SHELTER_BARRICADE_MATERIALS = 2;
        public const int SHELTER_BARRICADE_DOOR = 1;
        public const int SHELTER_BARRICADE_DEFENSE = 1;

        public const int SHELTER_RADIO_FUEL = 1;
        public const int SHELTER_RADIO_HOPE = 1;
        public const int SHELTER_RADIO_NOISE = 1;

        public const int SHELTER_STORAGE_PRESERVATION = 1;
        public const int SHELTER_STORAGE_CAPACITY = 1;
        public const int SHELTER_STORAGE_MAX_PRESERVATION = 3;

        public const int SHELTER_TREAT_MEDS = 1;
        public const int SHELTER_TREAT_HEALTH = 1;
        public const int SHELTER_TREAT_INFECTION = 1;

        public const int SHELTER_FORTIFY_MATERIALS = 2;
        public const int SHELTER_FORTIFY_DOOR = 2;
        public const int SHELTER_FORTIFY_DEFENSE = 1;

        public const int SHELTER_QUIET_NOISE = 1;
        public const int SHELTER_QUIET_STRESS = 1;

        public const int SHELTER_MASK_MATERIALS = 1;
        public const int SHELTER_MASK_SCENT = 1;

        // ============ 探索系统 ============
        public const int SEARCH_MAX_PER_RESOURCE = 2;
        public const int LURE_TIME_COST = 1;
        public const int LURE_NOISE = 1;
        public const int EXPLORE_RISK_LOW = 4;
        public const int EXPLORE_RISK_MID = 7;
        public const int EXPLORE_RISK_HIGH = 8;
        public const int EXPLORE_RISK_HIGH_HEALTH = 1;
        public const int EXPLORE_RISK_HIGH_STRESS = 2;
        public const int EXPLORE_RISK_MID_STRESS = 1;
        public const int EXPLORE_MIN_TIME_LIMIT = 2;
        public const int EXPLORE_TIME_EXTRA = 2;

        // ============ 祁眠 AI 数值 ============
        public const int QIMIAN_AWAKE_DAY = 5;
        public static readonly Dictionary<int, int> QIMIAN_MOTO_UPGRADE_DAYS = new() { {8, 2}, {12, 3} };
        public const int QIMIAN_ZONE_HEAT_MAX = 3;
        public const int QIMIAN_ZONE_A_EXPOSURE_MAX = 8;
        public const int QIMIAN_ZONE_B_EXPOSURE_MAX = 6;
        public const int QIMIAN_ZONE_C_EXPOSURE_MAX = 4;
        public const int QIMIAN_ZONE_C_HEAT_MAX = 2;
        public const int QIMIAN_EXPOSURE_MAX = 10;
        public const int QIMIAN_PATROL_EXPOSURE = 1;
        public const int QIMIAN_SCAVENGE_FOOD = 1;
        public const int QIMIAN_SCAVENGE_MEDICINE = 1;
        public const int QIMIAN_SCAVENGE_EXPOSURE = 1;
        public const int QIMIAN_DROP_EXPOSURE = -1;
        public const int QIMIAN_TRACK_EXPOSURE = 2;
        public const int QIMIAN_REST_EXPOSURE = -2;
    }
}
