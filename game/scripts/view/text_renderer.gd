# 文本渲染器 —— View 层，纯格式化，只读 Model
# 从 game_simulation.gd 抽出，实现 View 不改状态的 MVC 原则
class_name TextRenderer extends RefCounted

const LocData = preload("res://scripts/data/locations.gd")

# ---- 林行状态文本 ----
static func get_lin_condition_text(state) -> String:
	var infection_label := "感染风险：低"
	if int(state.lin.infection_risk) >= 5:
		infection_label = "感染风险：危险感染"
	elif int(state.lin.infection_risk) >= 3:
		infection_label = "感染风险：发热风险"
	return "生命 %d / 疲劳 %d / 压力 %d / %s / 希望 %d" % [
		int(state.lin.health),
		int(state.lin.fatigue),
		int(state.lin.stress),
		infection_label,
		int(state.lin.hope)
	]

# ---- 每日独白 ----
static func daily_monologue(state, day: int) -> String:
	var lines: Array[String] = []
	var h := int(state.lin.health)
	var f := int(state.lin.fatigue)
	var s := int(state.lin.stress)
	var hp := int(state.lin.hope)
	var inf := int(state.lin.infection_risk)
	var fd := int(state.resources.food)
	var wt := int(state.resources.water)

	if h <= 3: lines.append("伤口在发烫。每一步都像拖着自己的影子。")
	elif h <= 5: lines.append("身上多了几道疤，但还能撑。")

	if inf >= 5: lines.append("低头能看到手腕上的血管在变暗——不是淤青，是从皮肤底下透出来的。")
	elif inf >= 3: lines.append("体温比昨天高了一点。不是我多心——体温计不会骗人。")

	if fd <= 0: lines.append("胃已经没有什么可以收缩的了。")
	elif fd <= 1: lines.append("最后的食物快没了。再不去找，明天的胃会比今天更空。")
	if wt <= 0: lines.append("嘴唇起皮，舌根发苦。")
	elif wt <= 1: lines.append("水壶快见底了。每一口都得算着喝。")

	if f >= 7: lines.append("站着都能睡着，但躺下反而清醒——脑子里全是对明天的算盘。")
	elif f >= 5: lines.append("眼皮很重。如果能睡一个不被打断的整觉就好了。")

	if s >= 7: lines.append("手指不自觉地抖。不是冷——是脑子里那根弦绷得太久了。")
	elif s >= 5: lines.append("深呼吸。一切都还是可控的。大概。")

	if hp <= 2: lines.append("我把童年那张避难图纸又翻出来看了一遍。纸边已经起毛了。")
	elif hp >= 6: lines.append("收音机还在响。只要它还在说话，就说明外面还有人在维持秩序。")

	if bool(state.car.ready): lines.append("汽车加满了油，停在后院。明天只要拧一下钥匙——我们就走。")
	elif bool(state.car.found) and not bool(state.car.step_engine): lines.append("车库那辆车——线路还断着。得先把引擎的电路接上。")

	if day >= 8 and not state.qimian.public_clues.is_empty(): lines.append("我不是一个人。有什么东西——或者什么人——在跟我走同样的路。")
	if day >= 13: lines.append("不能再等了。每多待一天，出去的路就少一条。")

	if lines.is_empty(): return ""
	return "\n\n" + " ".join(lines)

# ---- 地点标签 ----
static func get_location_label(state, location_id: String) -> String:
	var location: Dictionary = state.locations[location_id]
	var visit_label := "未搜" if not bool(location.visited) else "已搜"
	var stock_label := "已空" if _is_location_depleted(location) else "有物资"
	var range_label := "可达" if int(location.range) <= int(state.bike.range) else "过远（需修车）"
	var blood_moon_warning := ""
	var next_day: int = int(state.day) + 1
	if _is_blood_moon_day(next_day):
		blood_moon_warning = " / [color=red]明晚血月[/color]"
	return "%s / %s / 尸群%s / 危险%s / %s / %s / %s%s" % [
		location.name, location.ring,
		str(location.zombies), str(location.danger_level),
		visit_label, stock_label, range_label, blood_moon_warning
	]

# ---- 地点卡片 ----
static func get_location_card_text(state, location_id: String) -> String:
	if not state.locations.has(location_id): return "未知节点"
	var location: Dictionary = state.locations[location_id]
	var icon_descriptions := _describe_icons(location.icons)
	var route_detail := "路程：%d 小时" % int(location.route_time)
	var road_detail := "路况：%s（%s）" % [str(location.road_condition), _road_condition_note(str(location.road_condition))]
	var range_detail := _range_affordance_text(state, location_id, location)
	return "%s\n资源倾向：%s / 危险等级：%s\n%s / %s\n%s\n地点特征：%s\n%s%s" % [
		get_location_label(state, location_id),
		str(location.resource_tendency), str(location.danger_level),
		route_detail, road_detail, range_detail, icon_descriptions,
		get_location_risk_text(state, location_id), _location_trace_suffix(location)
	]

# ---- 房间卡片 ----
static func get_room_card_text(state, room_id: String) -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "": return "没有进入可搜索地点。"
	var location: Dictionary = state.locations[state.exploration.active_location]
	if not location.rooms.has(room_id): return "未知房间。"
	var room: Dictionary = location.rooms[room_id]
	var searched_label := "已搜" if bool(room.searched) else "可搜"
	if bool(room.get("locked", false)): searched_label = "🔒 上锁"
	var zombie_hint := _room_threat_text(state, room_id, room)
	var visibility_text := _visibility_description(str(room.visibility))
	return "%s / %s / 耗时：%d 小时 / %s / %s" % [room.name, visibility_text, int(room.search_time), zombie_hint, searched_label]

# ---- 风险文本 ----
static func get_location_risk_text(state, location_id: String) -> String:
	if not state.locations.has(location_id): return "风险：未知。"
	var location: Dictionary = state.locations[location_id]
	var pressure: int = int(location.zombies) + int(location.range) + int(state.bike.noise)
	if int(location.range) > int(state.bike.range): return "风险：距离过远，今天无法稳定抵达。"
	if pressure <= 4: return "风险：低，适合搜刮。"
	if pressure <= 7: return "风险：中，可能增加疲劳和压力。"
	return "风险：高，尸群密集，可能受伤。"

# ---- 异常档案文本 ----
static func get_anomaly_dossier_text(state) -> String:
	if state.anomaly_dossier.is_empty(): return "暂无异常记录。"
	var lines: Array[String] = []
	for entry in state.anomaly_dossier:
		lines.append("第%d天 %s：%s" % [int(entry.day), str(entry.get("location_id", "")), str(entry.get("clue_text", ""))])
	return "\n".join(lines)

# ---- Private helpers ----
static func _is_blood_moon_day(day: int) -> bool:
	const BM_DAYS := [7, 15]
	return BM_DAYS.has(day)

static func _is_location_depleted(location: Dictionary) -> bool:
	for resource_name in location.resources.keys():
		if int(location.resources[resource_name]) > 0: return false
	return true

static func _location_trace_suffix(location: Dictionary) -> String:
	if not location.has("qimian_trace") or not bool(location.qimian_trace): return ""
	return " [祁眠异常]"

static func _room_threat_text(state, room_id: String, room: Dictionary) -> String:
	if int(room.hidden_zombies) <= 0: return "安全"
	if str(state.exploration.active_location) != "" and state.exploration.lured_rooms.has(room_id): return "尸群潜伏（已引开）"
	return "尸群潜伏（未排除）"

static func _visibility_description(visibility: String) -> String:
	match visibility:
		"明亮": return "能见度：明亮"
		"昏暗": return "能见度：昏暗"
		"黑暗": return "能见度：黑暗"
		_: return "能见度：%s" % visibility

static func _describe_icons(icons: Array) -> String:
	var labels: Array[String] = []
	for icon in icons:
		labels.append(LocData.ICON_LABELS.get(str(icon), str(icon)))
	return "，".join(labels)

static func _road_condition_note(condition: String) -> String:
	return LocData.ROAD_NOTES.get(str(condition), "路况不明")

static func _range_affordance_text(state, location_id: String, location: Dictionary) -> String:
	var bike_range: int = int(state.bike.range)
	var loc_range: int = int(location.range)
	if loc_range <= bike_range: return "🚲 自行车范围 %d/%d：可抵达" % [bike_range, loc_range]
	var diff := loc_range - bike_range
	return "🚲 自行车范围 %d/%d：距离不足（差 %d），需先修车" % [bike_range, loc_range, diff]
