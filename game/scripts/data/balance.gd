# 数值平衡配置 —— 纯数据，零逻辑
# 改数值只改这里，不用翻 controller 代码
class_name BalanceData

# ============ 初始资源 ============
const INIT_RESOURCES := {
	"food": 5,
	"water": 5,
	"meds": 2,
	"materials": 4,
	"parts": 1,
	"fuel": 3
}

# ============ 林行初始状态 ============
const INIT_LIN := {
	"health": 10,
	"hunger": 0,
	"thirst": 0,
	"fatigue": 1,
	"stress": 2,
	"infection_risk": 0,
	"hope": 4
}

# ============ 据点初始状态 ============
const INIT_SHELTER := {
	"door": 4,
	"noise": 2,
	"scent": 2,
	"light": 2,
	"defense": 1,
	"escape": 0,
	"supply_preservation": 0
}

# ============ 自行车初始状态 ============
const INIT_BIKE := {
	"durability": 6,
	"capacity": 6,
	"range": 1,
	"noise": 1
}

# ============ 汽车初始状态 ============
const INIT_CAR := {
	"found": false,
	"ready": false,
	"step_engine": false,
	"step_tire": false,
	"step_battery": false,
	"step_fueled": false,
	"breakdown": ""
}

# ============ 汽车零件初始 ============
const INIT_CAR_PARTS := {
	"battery": 0,
	"gasoline": 0,
	"tire": 0
}

# ============ 撤离初始 ============
const INIT_EVACUATION := {
	"safezone_confirmed": false,
	"address_known": false,
	"car_ready": false,
	"bike_ready": false
}

# ============ 祁眠 AI 初始状态 ============
const INIT_QIMIAN_AI_STATE := {
	"exposure": 0,
	"moto_tier": 1,
	"zone_heat": {"A": 0, "B": 0, "C": 0},
	"qijin_clues": 0,
	"rescued_npc": [],
	"inventory": {"food": 0, "water": 0, "medicine": 1, "materials": 0, "parts": 1, "fuel": 1}
}

# ============ 每日消耗 ============
const DAILY_CONSUME := {
	"food": 1,
	"water": 1
}

# ============ 饥饿/口渴惩罚 ============
const HUNGER_PER_DAY_NO_FOOD := 1
const THIRST_PER_DAY_NO_WATER := 1

# ============ 疲劳恢复 ============
const FATIGUE_RECOVER_PER_NIGHT := 1

# ============ 希望减压 ============
# stress 每夜减少 hope / HOPE_STRESS_DIVISOR
const HOPE_STRESS_DIVISOR := 3

# ============ 噪音传播 ============
# 阈值 > 此值时开始吸引尸群
const NOISE_ATTRACT_THRESHOLD := 2
# 单地点最大吸引数
const NOISE_ATTRACT_MAX := 2
# 吸引范围（近圈）
const NOISE_ATTRACT_RANGE := 1

# ============ 感染恶化 ============
# 感染风险 >= 此值时触发恶化
const INFECTION_CRITICAL_THRESHOLD := 5
const INFECTION_HEALTH_PENALTY := 1
const INFECTION_STRESS_PENALTY := 2

# ============ 血月公式 ============
# pressure = BM_BASE_PRESSURE + day/BM_DAY_DIVISOR * BM_DAY_MULT + noise + scent + light - door - defense - qimian_support
const BM_BASE_PRESSURE := 4
const BM_DAY_DIVISOR := 7
const BM_DAY_MULT := 2

# 血月结果阈值
const BM_LOW_THRESHOLD := 3
# pressure <= BM_LOW_THRESHOLD: hope + BM_LOW_HOPE
const BM_LOW_HOPE := 1

const BM_MID_THRESHOLD := 6
# pressure <= BM_MID_THRESHOLD: door - BM_MID_DOOR, food - BM_MID_FOOD
const BM_MID_DOOR := 1
const BM_MID_FOOD := 1

# else: health - BM_HIGH_HEALTH, door - BM_HIGH_DOOR
const BM_HIGH_HEALTH := 2
const BM_HIGH_DOOR := 2

# ============ 红潮公式 ============
# pressure = (day - RT_DAY_OFFSET) + noise + scent + light - door - defense
const RT_DAY_OFFSET := 10

# 红潮结果阈值
const RT_LOW_THRESHOLD := 3
const RT_LOW_STRESS := 1

const RT_MID_THRESHOLD := 5
const RT_MID_STRESS := 1
const RT_MID_FOOD := 1
const RT_MID_DOOR := 1

# else:
const RT_HIGH_HEALTH := 1
const RT_HIGH_STRESS := 2
const RT_HIGH_DOOR := 1
const RT_HIGH_FOOD := 1

# ============ 结局判定阈值 ============
const ENDING_HEALTH_DEAD := 0
const ENDING_HUNGER_CRITICAL := 4
const ENDING_THIRST_CRITICAL := 4
const ENDING_HEALTH_BARELY := 3
const ENDING_DOOR_BARELY := 1

# ============ 汽车修理成本 ============
# Step 1: 引擎接线
const CAR_REPAIR_ENGINE_MATERIALS := 2
const CAR_REPAIR_ENGINE_PARTS := 1

# Step 2: 换轮胎
const CAR_REPAIR_TIRE_COUNT := 1
const CAR_REPAIR_TIRE_PARTS := 1

# Step 3: 装电瓶
const CAR_REPAIR_BATTERY_COUNT := 1
const CAR_REPAIR_BATTERY_FUEL := 1

# Step 4: 加油
const CAR_REPAIR_GASOLINE_COUNT := 2

# ============ 据点行动成本/效果 ============
# 休息
const SHELTER_REST_FATIGUE := 2
const SHELTER_REST_STRESS := 1

# 修车（工作台修理自行车）
const SHELTER_REPAIR_BIKE_PARTS := 1
const SHELTER_REPAIR_BIKE_DURABILITY := 3
const SHELTER_REPAIR_BIKE_RANGE := 1
const SHELTER_REPAIR_BIKE_NOISE := 1
const SHELTER_REPAIR_BIKE_MAX_RANGE := 3

# 封窗
const SHELTER_BARRICADE_MATERIALS := 2
const SHELTER_BARRICADE_DOOR := 1
const SHELTER_BARRICADE_DEFENSE := 1

# 广播
const SHELTER_RADIO_FUEL := 1
const SHELTER_RADIO_HOPE := 1
const SHELTER_RADIO_NOISE := 1

# 整理
const SHELTER_STORAGE_PRESERVATION := 1
const SHELTER_STORAGE_CAPACITY := 1
const SHELTER_STORAGE_MAX_PRESERVATION := 3

# 治疗
const SHELTER_TREAT_MEDS := 1
const SHELTER_TREAT_HEALTH := 1
const SHELTER_TREAT_INFECTION := 1

# 加固
const SHELTER_FORTIFY_MATERIALS := 2
const SHELTER_FORTIFY_DOOR := 2
const SHELTER_FORTIFY_DEFENSE := 1

# 静默
const SHELTER_QUIET_NOISE := 1
const SHELTER_QUIET_STRESS := 1

# 气味遮蔽
const SHELTER_MASK_MATERIALS := 1
const SHELTER_MASK_SCENT := 1

# ============ 探索系统数值 ============
# 房间搜索单资源最大获取量
const SEARCH_MAX_PER_RESOURCE := 2
# 引诱时间消耗
const LURE_TIME_COST := 1
# 引诱噪音
const LURE_NOISE := 1

# 探索风险阈值
const EXPLORE_RISK_LOW := 4
const EXPLORE_RISK_MID := 7
# pressure >= EXPLORE_RISK_HIGH: health-1, stress+2
const EXPLORE_RISK_HIGH := 8
const EXPLORE_RISK_HIGH_HEALTH := 1
const EXPLORE_RISK_HIGH_STRESS := 2
# pressure >= EXPLORE_RISK_MID: stress+1
const EXPLORE_RISK_MID_STRESS := 1

# 探索时间限制公式
# time_limit = max(EXPLORE_MIN_TIME_LIMIT, route_time + EXPLORE_TIME_EXTRA)
const EXPLORE_MIN_TIME_LIMIT := 2
const EXPLORE_TIME_EXTRA := 2

# 搜索策略影响
# quick: search_time - 1 (min 1)
# careful: search_time 不变

# ============ 祁眠 AI 数值 ============
const QIMIAN_AWAKE_DAY := 5
const QIMIAN_MOTO_UPGRADE_DAYS := {8: 2, 12: 3}

# 区域可用性
const QIMIAN_ZONE_HEAT_MAX := 3
const QIMIAN_ZONE_A_EXPOSURE_MAX := 8
const QIMIAN_ZONE_B_EXPOSURE_MAX := 6
const QIMIAN_ZONE_C_EXPOSURE_MAX := 4
const QIMIAN_ZONE_C_HEAT_MAX := 2
const QIMIAN_EXPOSURE_MAX := 10

# AI 任务效果
const QIMIAN_PATROL_EXPOSURE := 1
const QIMIAN_SCAVENGE_FOOD := 1
const QIMIAN_SCAVENGE_MEDICINE := 1
const QIMIAN_SCAVENGE_EXPOSURE := 1
const QIMIAN_DROP_EXPOSURE := -1
const QIMIAN_TRACK_EXPOSURE := 2
const QIMIAN_REST_EXPOSURE := -2
