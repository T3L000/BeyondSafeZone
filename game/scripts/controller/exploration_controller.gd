# 探索系统 —— 规则层：进入地点/搜索房间/引诱/离开/大地图探索
class_name ExplorationSystem extends RefCounted

const LocData = preload("res://scripts/data/locations.gd")
const Balance = preload("res://scripts/data/balance.gd")
const _GameState = preload("res://scripts/model/game_state.gd")

static func enter_location(state: _GameState, location_id: String) -> String:
	if state.demo_complete:
		return "Demo 已结束，祁眠日志已解锁。"
	if state.phase not in ["morning", "day"]:
		return "现在不是白天，不能进入新地点。"
	if not state.locations.has(location_id):
		return "这里还没有绘制到地图上。"
	var location: Dictionary = state.locations[location_id]
	if int(location.range) > int(state.bike.range):
		state.lin.fatigue += 1
		state.last_event = "%s 太远了。%s" % [location.name, _get_location_risk_text(state, location_id)]
		return state.last_event
	# Apply route penalties (same as explore())
	var road_penalty := _road_condition_fatigue_penalty(location)
	state.lin.fatigue += int(location.route_time) + road_penalty
	state.lin.stress += max(0, int(location.zombies) - 2)
	state.bike.durability = max(0, int(state.bike.durability) - int(location.range))
	_apply_exploration_risk(state, location)
	state.exploration = {
		"active_location": location_id,
		"time_used": 0,
		"time_limit": max(Balance.EXPLORE_MIN_TIME_LIMIT, int(location.route_time) + Balance.EXPLORE_TIME_EXTRA),
		"noise": 0,
		"searched_rooms": [],
		"lured_rooms": []
	}
	state.phase = "searching"
	var route_note := ""
	if road_penalty > 0:
		route_note = " 路况：%s，额外疲劳 +%d。" % [str(location.road_condition), road_penalty]
	var risk_text := _get_location_risk_text(state, location_id)
	state.last_event = "进入 %s。%s%s先读房间，再决定搜哪里；拖太久会把白天耗完。" % [location.name, risk_text, route_note]
	return state.last_event

static func search_room(state: _GameState, room_id: String, tactic: String = "careful") -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "林行还没有进入可搜索地点。"
	var location: Dictionary = state.locations[state.exploration.active_location]
	if not location.rooms.has(room_id):
		return "这个房间还没有做进灰盒。"
	var room: Dictionary = location.rooms[room_id]
	if bool(room.searched):
		state.last_event = "%s 已经搜过，再翻只会浪费时间。" % room.name
		return state.last_event
	if bool(room.get("locked", false)):
		state.last_event = "%s 门锁着。需要撬棍才能打开。" % room.name
		return state.last_event

	state.exploration.time_used += _search_time_for_tactic(room, tactic)
	var risk_notes := _apply_room_search_risk(state, room_id, room, tactic)
	var found := []
	for resource_name in room.resources.keys():
		var amount: int = int(room.resources[resource_name])
		if amount <= 0:
			continue
		var taken: int = min(Balance.SEARCH_MAX_PER_RESOURCE, amount)
		room.resources[resource_name] = amount - taken
		if resource_name in ["battery", "gasoline", "tire"]:
			state.car_parts[resource_name] = int(state.car_parts.get(resource_name, 0)) + taken
			found.append("汽车零件：%s +%d" % [resource_name, taken])
		else:
			state.resources[resource_name] = int(state.resources.get(resource_name, 0)) + taken
			found.append("%s +%d" % [resource_name, taken])
	room.searched = true
	state.exploration.searched_rooms.append(room_id)
	var flag_notes := _apply_room_flags(state, room)
	if not flag_notes.is_empty():
		found.append("线索：" + " ".join(flag_notes))
	if found.is_empty():
		state.last_event = "搜索 %s。%s这里已经没有能带走的东西。" % [room.name, _room_note_text(risk_notes)]
	else:
		state.last_event = "搜索 %s。%s带回：%s。" % [room.name, _room_note_text(risk_notes), ", ".join(found)]
	return state.last_event

static func lure_room(state: _GameState, room_id: String) -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "林行还没有进入可搜索地点。"
	var location: Dictionary = state.locations[state.exploration.active_location]
	if not location.rooms.has(room_id):
		return "这个房间还没有做进灰盒。"
	var room: Dictionary = location.rooms[room_id]
	state.exploration.time_used += Balance.LURE_TIME_COST
	state.exploration.noise += Balance.LURE_NOISE
	if int(room.hidden_zombies) > 0 and not state.exploration.lured_rooms.has(room_id):
		state.exploration.lured_rooms.append(room_id)
		state.last_event = "林行在 %s 外制造噪音，把里面的动静引向另一侧。" % room.name
	else:
		state.last_event = "林行在 %s 外制造噪音，但没有听见明显回应。" % room.name
	return state.last_event

static func leave_exploration(state: _GameState) -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "林行还没有进入可离开的地点。"
	var location_id := str(state.exploration.active_location)
	var location: Dictionary = state.locations[location_id]
	location.visited = true
	var notes := _apply_evacuation_clues(state, location_id, location)
	var over_time: int = max(0, int(state.exploration.time_used) - int(state.exploration.time_limit))
	if over_time > 0:
		state.lin.fatigue += over_time
		notes.append("天色压下来，额外疲劳 +%d。" % over_time)
	state.phase = "evening"
	var note_text := ""
	if not notes.is_empty():
		note_text = " %s" % " ".join(notes)
	state.exploration = {"active_location": "", "time_used": 0, "time_limit": 0, "noise": 0, "searched_rooms": [], "lured_rooms": []}
	state.last_event = "林行离开 %s，赶在天黑前回到据点。%s" % [location.name, note_text]
	return state.last_event

static func explore(state: _GameState, location_id: String) -> String:
	if state.demo_complete:
		return "Demo 已结束，祁眠日志已解锁。"
	if not state.locations.has(location_id):
		return "这里还没有绘制到地图上。"
	var location: Dictionary = state.locations[location_id]
	if location.range > state.bike.range:
		state.lin.fatigue += 1
		state.last_event = "%s 太远了。%s" % [location.name, _get_location_risk_text(state, location_id)]
		return state.last_event

	var found := []
	for resource_name in location.resources.keys():
		var amount: int = int(location.resources[resource_name])
		if amount <= 0:
			continue
		var taken: int = min(2, amount)
		location.resources[resource_name] = amount - taken
		if resource_name in ["battery", "gasoline", "tire"]:
			state.car_parts[resource_name] = int(state.car_parts.get(resource_name, 0)) + taken
			found.append("汽车零件：%s +%d" % [resource_name, taken])
		else:
			state.resources[resource_name] = int(state.resources.get(resource_name, 0)) + taken
			found.append("%s +%d" % [resource_name, taken])

	location.visited = true
	var risk_text := _get_location_risk_text(state, location_id)
	var road_penalty := _road_condition_fatigue_penalty(location)
	var pressure_notes := []
	if road_penalty > 0:
		pressure_notes.append("路况：%s，额外疲劳 +%d。" % [str(location.road_condition), road_penalty])
	pressure_notes.append_array(_apply_evacuation_clues(state, location_id, location))
	_apply_exploration_risk(state, location)
	state.bike.durability = max(0, int(state.bike.durability) - location.range)
	state.lin.fatigue += int(location.route_time) + road_penalty
	state.lin.stress += max(0, int(location.zombies) - 2)
	state.phase = "evening"
	var pressure_note := ""
	if not pressure_notes.is_empty():
		pressure_note = " %s" % " ".join(pressure_notes)

	if found.is_empty():
		state.last_event = "探索 %s。%s%s 这里几乎被搜空了，只留下难以解释的翻动痕迹。" % [location.name, risk_text, pressure_note]
	else:
		state.last_event = "探索 %s。%s%s 带回：%s。" % [location.name, risk_text, pressure_note, ", ".join(found)]
	return state.last_event

# --- Private helpers ---

static func _search_time_for_tactic(room: Dictionary, tactic: String) -> int:
	match tactic:
		"quick": return max(1, int(room.search_time) - 1)
		"careful": return int(room.search_time)
		_: return int(room.search_time)

static func _apply_room_search_risk(state: _GameState, room_id: String, room: Dictionary, tactic: String) -> Array:
	var notes := []
	if int(room.hidden_zombies) <= 0:
		return notes
	if state.exploration.lured_rooms.has(room_id):
		notes.append("隐藏尸群已被引开。")
		return notes
	var dark_room := str(room.visibility) == "黑暗"
	if dark_room or tactic == "quick":
		state.lin.health = max(0, int(state.lin.health) - 1)
		state.lin.infection_risk += 1
		state.lin.stress += 1
		notes.append("隐藏尸群从暗处扑出，林行受伤并增加感染风险。")
	else:
		state.lin.stress += 1
		notes.append("房间里有隐藏尸群，谨慎搜索让林行勉强避开。")
	return notes

static func _room_note_text(notes: Array) -> String:
	if notes.is_empty(): return ""
	return "%s " % " ".join(notes)

static func _apply_room_flags(state: _GameState, room: Dictionary) -> Array:
	var notes := []
	for flag in room.get("flags", []):
		match str(flag):
			"plan_found":
				state.lin.hope += 1
				notes.append("童年末日避难计划图纸——三个孩子的笔迹")
			"safezone_hint_1":
				notes.append("纸条：「保护区在南边，往军区基地走」")
			"rebirth_clue_1":
				notes.append("隔离记录：「零号病人已转移至返生计划中心」")
			"rebirth_clue_2":
				notes.append("联络名单上画了红圈的名字——「烬」")
			"address_known":
				state.evacuation.address_known = true
				notes.append("地图碎片标注了保护区筛查棚位置")
			"childhood_memory":
				state.lin.hope += 1
				notes.append("旧笔记：「林行、祁眠、祁烬——末日避难计划」")
			"rebirth_poster":
				notes.append("返生计划海报：「人类的下一步」")
			"car_found":
				state.car.found = true
				notes.append("旧轿车——需要电瓶、汽油、轮胎")
			"crowbar_found":
				state.resources["parts"] = int(state.resources.get("parts", 0)) + 1
				notes.append("找到撬棍——可以撬开车库和封锁的门")
			"lab_location":
				notes.append("哨塔地图：「返生计划第三实验室 东区外环217号」")
			"qimian_file":
				notes.append("实验日志：「零号感染者 代号:眠 瞳孔银灰反射」")
			"apartment_letter":
				notes.append("信件：「返生计划第三实验室——不要去那里」")
			"qijin_apartment":
				notes.append("信封：「返生计划·社区联络处·祁烬」")
			"rebirth_insider":
				state.lin.hope += 1
				notes.append("幸存者说出了返生计划内幕情报")
			_:
				notes.append("新的线索")
	return notes

static func _get_location_risk_text(state: _GameState, location_id: String) -> String:
	if not state.locations.has(location_id):
		return "风险：未知。"
	var location: Dictionary = state.locations[location_id]
	var pressure: int = int(location.zombies) + int(location.range) + int(state.bike.noise)
	if int(location.range) > int(state.bike.range):
		return "风险：距离过远，今天无法稳定抵达。"
	if pressure <= Balance.EXPLORE_RISK_LOW: return "风险：低，适合搜刮。"
	if pressure <= Balance.EXPLORE_RISK_MID: return "风险：中，可能增加疲劳和压力。"
	return "风险：高，尸群密集，可能受伤。"

static func _road_condition_fatigue_penalty(location: Dictionary) -> int:
	match str(location.road_condition):
		"熟路", "碎玻璃": return 0
		"雨后湿滑", "积水", "堵塞", "路障": return 1
		"尸群迁移", "封锁线": return 2
		_: return 1

static func _apply_exploration_risk(state: _GameState, location: Dictionary) -> void:
	var pressure: int = int(location.zombies) + int(location.range) + int(state.bike.noise)
	if pressure >= Balance.EXPLORE_RISK_HIGH:
		state.lin.health = max(0, int(state.lin.health) - Balance.EXPLORE_RISK_HIGH_HEALTH)
		state.lin.stress += Balance.EXPLORE_RISK_HIGH_STRESS
	elif pressure >= 5:
		state.lin.stress += 1

static func _apply_evacuation_clues(state: _GameState, location_id: String, location: Dictionary) -> Array:
	var notes := []
	if location_id in ["police", "subway", "safezone_edge"] or location.icons.has("clue") or location.icons.has("route") or location.icons.has("safezone"):
		if not bool(state.evacuation.address_known):
			state.evacuation.address_known = true
			notes.append("撤离线索：找到保护区筛查棚地址。")
	if location_id == "safezone_edge" or location.icons.has("safezone"):
		if not bool(state.evacuation.safezone_confirmed):
			state.evacuation.safezone_confirmed = true
			notes.append("撤离线索：确认保护区仍在短暂接收。")
	return notes
