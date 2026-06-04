# 安全演示路线 —— 纯数据，零逻辑
# 15 天自动演示走的完美路线
class_name SafeRouteData

# 每天对应的探索地点
const DAY_LOCATION := {
	1: "home",
	2: "convenience",
	3: "clinic",
	4: "bike_shop",
	5: "bike_shop",
	6: "police",
	7: "school",
	8: "supermarket",
	9: "bridge_camp",
	10: "gas_station",
	11: "hardware_store",
	12: "subway",
	13: "apartment",
	14: "quarantine",
	15: "safezone_edge"
}

# 每天对应的据点行动
const DAY_SHELTER_ACTION := {
	10: "workbench_car",
	12: "workbench_car",
	13: "workbench_car",
	14: "workbench_car"
}

# 条件化行动规则（按优先级从上到下匹配）
# blood_moon: 血月日行动
# day_mod: 天数为指定倍数的行动
const CONDITIONAL_ACTIONS := [
	{"condition": "blood_moon", "action": "fortify"},
	{"condition": "day_mod", "mod": 3, "action": "radio"},
	{"condition": "day_mod", "mod": 2, "action": "repair_bike"},
	{"condition": "fallback", "action": "quiet"}
]

# 每个地点自动搜索的房间上限
const MAX_ROOMS_PER_LOCATION := 3

# 搜索策略
const SEARCH_TACTIC := "careful"

# ============ 便利方法 ============

static func get_location_for_day(day: int) -> String:
	return DAY_LOCATION.get(day, "home")

static func get_action_for_day(day: int) -> String:
	# 首先查固定行动
	if DAY_SHELTER_ACTION.has(day):
		return DAY_SHELTER_ACTION[day]
	# 再查条件规则
	for rule in CONDITIONAL_ACTIONS:
		var cond: String = rule.condition
		if cond == "blood_moon":
			if _is_blood_moon(day):
				return rule.action
		elif cond == "day_mod":
			if day % int(rule.mod) == 0:
				return rule.action
		elif cond == "fallback":
			return rule.action
	return "quiet"

static func _is_blood_moon(day: int) -> bool:
	return day in [7, 15]
