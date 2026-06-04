extends RefCounted

class_name UILabels

static func phase_label(phase: String) -> String:
	match phase:
		"morning": return "清晨"
		"day": return "白天"
		"evening": return "黄昏"
		"night": return "夜晚"
		"searching": return "室内搜索"
		"reveal": return "日志揭示"
		_: return phase

static func pressure_label(pressure_type: String) -> String:
	match pressure_type:
		"tutorial": return "熟悉环境"
		"scarcity": return "资源紧张"
		"stress": return "精神压力"
		"mobility": return "移动受限"
		"shelter": return "据点暴露"
		"warning": return "血月前兆"
		"blood_moon": return "血月"
		"aftermath": return "灾后清点"
		"foreshadow": return "异常伏笔"
		"qimian": return "异常痕迹"
		"red_tide": return "红潮逼近"
		_: return pressure_type

static func ending_label(ending_state: String) -> String:
	match ending_state:
		"reached_gate_quarantine": return "抵达保护区门口，隔离观察"
		"barely_reached_gate": return "勉强抵达保护区门口"
		"collapsed": return "崩溃边缘"
		_: return "进行中"

static func flag_label(value: bool) -> String:
	return "已确认" if value else "未确认"

static func heat_bar(level: int) -> String:
	match level:
		0: return "□"
		1: return "■"
		2: return "■■"
		3: return "■■■⚠️"
		_: return "?"

static func car_step_label(car_state: Dictionary) -> String:
	if bool(car_state.ready):
		return "已完成"
	if not bool(car_state.found):
		return "未发现"
	var steps := []
	if not bool(car_state.step_engine): steps.append("1/4引擎")
	if not bool(car_state.step_tire): steps.append("2/4轮胎")
	if not bool(car_state.step_battery): steps.append("3/4电瓶")
	if not bool(car_state.step_fueled): steps.append("4/4加油")
	if steps.is_empty():
		return "已完成"
	return steps[0]

static func facility_summary(facilities: Dictionary) -> String:
	var labels := []
	for facility_id in ["bed", "workbench", "barricade", "radio", "storage"]:
		var facility: Dictionary = facilities[facility_id]
		var used_label := "*" if bool(facility.used_today) else ""
		labels.append("%s%d%s" % [facility.name, int(facility.level), used_label])
	return "  ".join(labels)
