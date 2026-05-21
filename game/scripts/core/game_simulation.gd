extends RefCounted

const MAX_DEMO_DAY := 14
const FULL_DAY_LIMIT := 30

var state: Dictionary = {}

var _day_events := {
	1: _day_event(1, "林行在家中的旧沙发上醒来，桌上还压着童年画过的末日避难路线。", "tutorial", "收音机里反复出现保护区断续广播。", "", {}),
	2: _day_event(2, "楼下有人翻过垃圾桶，瓶装水比昨天更难找。", "scarcity", "便利店门口的玻璃碎得很整齐。", "", {"water": -1}),
	3: _day_event(3, "清晨有短促敲门声，门外只剩一串拖痕。", "stress", "墙上多了一句保护区方向的粉笔字。", "", {"stress": 1}),
	4: _day_event(4, "自行车链条卡住了，远处广播却催促幸存者尽快转移。", "mobility", "修理铺附近的尸群被什么声音吸引过。", "", {"bike_durability": -1}),
	5: _day_event(5, "雨停后气味闷在楼道里，据点开始暴露生活痕迹；城市另一端有人从感染昏睡中醒来。", "qimian", "楼梯口能闻到潮湿血腥味。", "", {"scent": 1, "stress": 1}),
	6: _day_event(6, "月色比平时更红，收音机要求外围幸存者提前熄灯。", "warning", "保护区广播第一次提到血月。", "明晚血月：门窗、防御、噪音和气味会决定据点能不能撑住。", {"noise": 1}),
	7: _day_event(7, "血月当天，街上几乎没有普通尸群的游荡声，像是在等夜晚。", "blood_moon", "窗外的月亮还没升起，玻璃已经开始轻轻震动。", "今晚血月：这是第一次防守考试。", {"stress": 1}),
	8: _day_event(8, "血月过后，附近街区被翻得乱七八糟。", "aftermath", "保护区广播说中圈仍有通行可能。", "", {"door": -1}),
	9: _day_event(9, "自行车还能撑一段路，但每一次远行都会留下更响的动静。", "mobility", "废弃学校方向飘来断续铃声。", "", {"bike_durability": -1}),
	10: _day_event(10, "社区诊所的门被风吹开，里面安静得不正常。", "foreshadow", "有个药柜像是被人从里面重新锁上。", "", {"stress": 1}),
	11: _day_event(11, "清晨的街道少了一些尸群，林行却感觉有人比自己更早走过。", "qimian", "诊所旧楼留下了一支没用完的消毒液。", "", {"hope": 1}),
	12: _day_event(12, "超市方向没有争抢声，只有货架被拖动后的空响。", "qimian", "最容易保存的食物像是被人有计划地拿走。", "", {"stress": 1}),
	13: _day_event(13, "地铁口的尸群突然稀了，墙上有一道像箭头的划痕。", "qimian", "那道箭头像是在避开探照灯。", "明晚第二次血月：据点撑不住时，只能抓住保护区短暂开放窗口。", {"hope": 1}),
	14: _day_event(14, "第二次血月压到城市上空，保护区短暂开放外圈接收窗口。", "blood_moon", "林行必须在据点失守前赶到保护区大门外。", "今晚血月：撤离、筛查和祁眠日志会在同一夜收束。", {"stress": 2})
}

var _qimian_plan := {
	5: [
		{
			"title": "祁眠醒来",
			"public_clue": "远处旧楼有一扇门从里面被打开，又被人小心合上。",
			"truth": "祁眠从感染昏睡中醒来，确认普通丧尸不会主动攻击自己，把寻找祁烬定为主任务。",
			"ai_replay": "输入：自身感染异常、附近尸群无攻击反应。规则：先确认身体，再寻找祁烬，避免暴露。",
			"subjective_fragment": "他们没有扑上来。我还活着，或者说，变成了另一种东西。"
		}
	],
	6: [
		{
			"title": "诊所取药",
			"location": "clinic",
			"resource": "meds",
			"amount": -1,
			"public_clue": "社区诊所药柜被重新锁过，锁孔旁有新鲜刮痕。",
			"truth": "祁眠只拿走任务途中需要的药品，留下大部分绷带和消毒物。",
			"ai_replay": "输入：可感知药品、低暴露风险。规则：只拿任务所需，不制造明显抢掠痕迹。",
			"subjective_fragment": "够路上用就行。拿太多，只会让后来的人死得更快。"
		}
	],
	8: [
		{
			"title": "超市夜行",
			"location": "supermarket",
			"resource": "food",
			"amount": -2,
			"public_clue": "超市货架像被人有计划地清过，最容易保存的食物少了一批。",
			"truth": "祁眠拿走一部分便携食物，同时避开会暴露身份的人类巡逻。",
			"ai_replay": "输入：中圈食物、巡逻痕迹、寻找祁烬需要补给。规则：补给优先，避免接触。",
			"subjective_fragment": "货架空一点，总比我被他们拖去筛查要好。"
		}
	],
	11: [
		{
			"title": "尸群偏移",
			"location": "subway",
			"zombie_delta": -2,
			"public_clue": "地铁口的尸群少了，墙上有一条像是刻意留下的箭头。",
			"truth": "祁眠为了避开保护区探照灯，把尸群引到另一条街，也让陌生幸存者有了空隙。",
			"ai_replay": "输入：探照灯、尸群、地铁口可穿行。规则：避免暴露，顺路降低近处幸存者风险。",
			"subjective_fragment": "灯扫过来之前，尸群得先动。"
		}
	],
	14: [
		{
			"title": "尸群藏身",
			"blood_moon_support": 2,
			"public_clue": "保护区外的尸群像被某个看不见的人牵走，给筛查棚前让出一条窄路。",
			"truth": "祁眠藏在尸群里改变路线，目标是避开探照灯和追查线索，间接让林行抵达筛查棚。",
			"ai_replay": "输入：保护区探照灯、尸群密度、可借尸群掩护。规则：避开筛查，继续追查祁烬，不主动接触林行。",
			"subjective_fragment": "那个人擦肩而过。我没有看清他的脸，也不能停。"
		},
		{
			"title": "双层揭示",
			"public_clue": "筛查棚外有人低声说：昨晚那股尸群像被人牵走了。",
			"truth": "林行只听见异常线索，玩家随后看到祁眠完整行动回放。",
			"ai_replay": "输入：林行不可见；系统结算共享世界影响。规则：玩家获得上帝视角，角色仍保持误认。",
			"subjective_fragment": "如果那是旧人，我也已经错过。"
		}
	]
}

func new_game() -> Dictionary:
	state = {
		"day": 1,
		"phase": "morning",
		"goal": "撤离到保护区",
		"demo_complete": false,
		"ending_state": "in_progress",
		"last_event": "林行在家中醒来。收音机里反复出现保护区断续广播。",
		"morning_context": {},
		"applied_day_events": [],
		"resources": {
			"food": 5,
			"water": 5,
			"meds": 2,
			"materials": 4,
			"parts": 1,
			"fuel": 3
		},
		"lin": {
			"health": 10,
			"hunger": 0,
			"thirst": 0,
			"fatigue": 1,
			"stress": 2,
			"infection_risk": 0,
			"hope": 4
		},
		"shelter": {
			"door": 4,
			"noise": 2,
			"scent": 2,
			"light": 2,
			"defense": 1,
			"escape": 0,
			"supply_preservation": 0,
			"facilities": _default_facilities()
		},
		"bike": {
			"durability": 6,
			"capacity": 6,
			"range": 1,
			"noise": 1
		},
		"locations": _default_locations(),
		"qimian": {
			"awake": false,
			"log": [],
			"public_clues": [],
			"personality_card": {
				"main_goal": "寻找祁烬",
				"exposure": "谨慎，避免暴露",
				"moral_rule": "会救近处的人，但不承担大规模救援",
				"resource_rule": "只拿任务需要的资源",
				"safezone_attitude": "靠近观察，但不信任筛查"
			}
		},
		"evacuation": {
			"safezone_confirmed": false,
			"address_known": false,
			"bike_ready": false
		},
		"exploration": _empty_exploration_state(),
		"blood_moons_resolved": [],
		"reveal": {
			"unlocked": false,
			"summary": ""
		}
	}
	start_day(1)
	return state

func is_blood_moon_day(day: int) -> bool:
	return day > 0 and day % 7 == 0

func get_day_event(day: int) -> Dictionary:
	if _day_events.has(day):
		return _day_events[day].duplicate(true)
	return _day_event(day, "这一天还没有写入 Demo。", "unknown", "没有新的线索。", "", {})

func start_day(day: int) -> String:
	state.day = day
	if state.phase != "reveal":
		state.phase = "morning"
	if state.has("shelter") and state.shelter.has("facilities"):
		_reset_facility_use()
	var event := get_day_event(day)
	state.morning_context = {
		"day": day,
		"text": event.morning_text,
		"pressure_type": event.pressure_type,
		"clue": event.clue,
		"blood_moon_warning": event.blood_moon_warning
	}
	if not state.applied_day_events.has(day):
		_apply_day_pressure(event)
		state.applied_day_events.append(day)
	state.last_event = "第 %d 天清晨。%s %s" % [day, event.morning_text, event.clue]
	return state.last_event

func get_location_ids() -> Array:
	return state.locations.keys()

func get_lin_condition_text() -> String:
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

func get_location_label(location_id: String) -> String:
	var location: Dictionary = state.locations[location_id]
	var visit_label := "已搜" if bool(location.visited) else "未搜"
	var stock_label := "已空" if _is_location_depleted(location) else "有物资"
	var range_label := "可达" if int(location.range) <= int(state.bike.range) else "太远"
	return "%s / %s / 尸群%s / 危险%s / %s / %s / %s" % [
		location.name,
		location.ring,
		str(location.zombies),
		str(location.danger_level),
		visit_label,
		stock_label,
		range_label
	]

func get_location_card_text(location_id: String) -> String:
	if not state.locations.has(location_id):
		return "未知节点"
	var location: Dictionary = state.locations[location_id]
	var icon_text := ", ".join(location.icons)
	return "%s\n资源：%s / 危险：%s / 路程：%d / 路况：%s\n图标：%s\n%s" % [
		get_location_label(location_id),
		str(location.resource_tendency),
		str(location.danger_level),
		int(location.route_time),
		str(location.road_condition),
		icon_text,
		get_location_risk_text(location_id)
	] + _location_trace_suffix(location)

func get_room_card_text(room_id: String) -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "没有进入可搜索地点。"
	var location: Dictionary = state.locations[state.exploration.active_location]
	if not location.rooms.has(room_id):
		return "未知房间。"
	var room: Dictionary = location.rooms[room_id]
	var searched_label := "已搜" if bool(room.searched) else "未搜"
	var zombie_hint := _room_threat_text(room_id, room)
	var dark_hint := "黑暗风险" if str(room.visibility) == "黑暗" else "可读"
	return "%s / 能见度：%s / 耗时：%d / %s / %s / %s" % [
		room.name,
		room.visibility,
		int(room.search_time),
		dark_hint,
		zombie_hint,
		searched_label
	]

func get_location_risk_text(location_id: String) -> String:
	if not state.locations.has(location_id):
		return "风险：未知。"
	var location: Dictionary = state.locations[location_id]
	var pressure: int = int(location.zombies) + int(location.range) + int(state.bike.noise)
	if int(location.range) > int(state.bike.range):
		return "风险：距离过远，今天无法稳定抵达。"
	if pressure <= 4:
		return "风险：低，适合搜刮。"
	if pressure <= 7:
		return "风险：中，可能增加疲劳和压力。"
	return "风险：高，尸群密集，可能受伤。"

func enter_location(location_id: String) -> String:
	if state.demo_complete:
		return "Demo 已结束，祁眠日志已解锁。"
	if state.phase not in ["morning", "day"]:
		return "现在不是白天，不能进入新地点。"
	if not state.locations.has(location_id):
		return "这里还没有绘制到地图上。"
	var location: Dictionary = state.locations[location_id]
	if int(location.range) > int(state.bike.range):
		state.lin.fatigue += 1
		state.last_event = "%s 太远了。%s 林行需要修好自行车或找到更安全的路线。" % [location.name, get_location_risk_text(location_id)]
		return state.last_event
	state.exploration = {
		"active_location": location_id,
		"time_used": 0,
		"time_limit": max(2, int(location.route_time) + 2),
		"noise": 0,
		"searched_rooms": [],
		"lured_rooms": []
	}
	state.phase = "searching"
	state.last_event = "进入 %s。先读房间，再决定搜哪里；拖太久会把白天耗完。" % location.name
	return state.last_event

func search_room(room_id: String, tactic: String = "careful") -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "林行还没有进入可搜索地点。"
	var location: Dictionary = state.locations[state.exploration.active_location]
	if not location.rooms.has(room_id):
		return "这个房间还没有做进灰盒。"
	var room: Dictionary = location.rooms[room_id]
	if bool(room.searched):
		state.last_event = "%s 已经搜过，再翻只会浪费时间。" % room.name
		return state.last_event

	state.exploration.time_used += _search_time_for_tactic(room, tactic)
	var risk_notes := _apply_room_search_risk(room_id, room, tactic)
	var found := []
	for resource_name in room.resources.keys():
		var amount: int = int(room.resources[resource_name])
		if amount <= 0:
			continue
		var taken: int = min(2, amount)
		room.resources[resource_name] = amount - taken
		state.resources[resource_name] = int(state.resources.get(resource_name, 0)) + taken
		found.append("%s +%d" % [resource_name, taken])
	room.searched = true
	state.exploration.searched_rooms.append(room_id)
	if found.is_empty():
		state.last_event = "搜索 %s。%s这里已经没有能带走的东西。" % [room.name, _room_note_text(risk_notes)]
	else:
		state.last_event = "搜索 %s。%s带回：%s。" % [room.name, _room_note_text(risk_notes), ", ".join(found)]
	return state.last_event

func lure_room(room_id: String) -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "林行还没有进入可搜索地点。"
	var location: Dictionary = state.locations[state.exploration.active_location]
	if not location.rooms.has(room_id):
		return "这个房间还没有做进灰盒。"
	var room: Dictionary = location.rooms[room_id]
	state.exploration.time_used += 1
	state.exploration.noise += 1
	if int(room.hidden_zombies) > 0 and not state.exploration.lured_rooms.has(room_id):
		state.exploration.lured_rooms.append(room_id)
		state.last_event = "林行在 %s 外制造噪音，把里面的动静引向另一侧。" % room.name
	else:
		state.last_event = "林行在 %s 外制造噪音，但没有听见明显回应。" % room.name
	return state.last_event

func leave_exploration() -> String:
	if state.phase != "searching" or str(state.exploration.active_location) == "":
		return "林行还没有进入可离开的地点。"
	var location_id := str(state.exploration.active_location)
	var location: Dictionary = state.locations[location_id]
	location.visited = true
	var notes := _apply_evacuation_clues(location_id, location)
	var over_time: int = max(0, int(state.exploration.time_used) - int(state.exploration.time_limit))
	if over_time > 0:
		state.lin.fatigue += over_time
		notes.append("天色压下来，额外疲劳 +%d。" % over_time)
	state.phase = "evening"
	state.exploration = _empty_exploration_state()
	var note_text := ""
	if not notes.is_empty():
		note_text = " %s" % " ".join(notes)
	state.last_event = "林行离开 %s，赶在天黑前回到据点。%s" % [location.name, note_text]
	return state.last_event

func explore(location_id: String) -> String:
	if state.demo_complete:
		return "Demo 已结束，祁眠日志已解锁。"
	if not state.locations.has(location_id):
		return "这里还没有绘制到地图上。"
	var location: Dictionary = state.locations[location_id]
	if location.range > state.bike.range:
		state.lin.fatigue += 1
		state.last_event = "%s 太远了。%s 林行需要修好自行车或找到更安全的路线。" % [location.name, get_location_risk_text(location_id)]
		return state.last_event

	var found := []
	for resource_name in location.resources.keys():
		var amount: int = int(location.resources[resource_name])
		if amount <= 0:
			continue
		var taken: int = min(2, amount)
		location.resources[resource_name] = amount - taken
		state.resources[resource_name] = int(state.resources.get(resource_name, 0)) + taken
		found.append("%s +%d" % [resource_name, taken])

	location.visited = true
	var risk_text := get_location_risk_text(location_id)
	var road_penalty := _road_condition_fatigue_penalty(location)
	var pressure_notes := []
	if road_penalty > 0:
		pressure_notes.append("路况：%s，额外疲劳 +%d。" % [str(location.road_condition), road_penalty])
	pressure_notes.append_array(_apply_evacuation_clues(location_id, location))
	_apply_exploration_risk(location)
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

func perform_shelter_action(action_id: String) -> String:
	match action_id:
		"rest_bed":
			state.lin.fatigue = max(0, int(state.lin.fatigue) - 2)
			state.lin.stress = max(0, int(state.lin.stress) - 1)
			_mark_facility_used("bed")
			state.last_event = "林行在床铺上断续睡了一会儿，疲劳和压力都降下来一点。"
		"workbench_repair":
			if _spend("parts", 1):
				state.bike.durability += 3
				state.bike.range = min(3, int(state.bike.range) + 1)
				state.bike.noise = max(0, int(state.bike.noise) - 1)
				_mark_facility_used("workbench")
				if int(state.bike.range) >= 3:
					state.evacuation.bike_ready = true
				state.last_event = "林行在工作台修好车链和刹车，自行车更适合远行。"
			else:
				state.last_event = "没有足够零件，工作台只能摆着拆开的工具。"
		"barricade_windows":
			if _spend("materials", 2):
				state.shelter.door += 1
				state.shelter.defense += 1
				state.shelter.facilities.barricade.level += 1
				_mark_facility_used("barricade")
				state.last_event = "林行把窗框和门缝重新钉死，血月前的防线厚了一层。"
			else:
				state.last_event = "建材不足，封窗只能停在一半。"
		"radio_broadcast":
			if _spend("fuel", 1):
				state.lin.hope += 1
				state.shelter.noise += 1
				_mark_facility_used("radio")
				if int(state.day) >= 3:
					state.evacuation.safezone_confirmed = true
				if int(state.day) >= 9:
					state.evacuation.address_known = true
				state.last_event = _radio_message_for_day(int(state.day))
			else:
				state.last_event = "发电机没有燃料，收音机只剩沙沙声。"
		"organize_storage":
			state.shelter.supply_preservation = min(3, int(state.shelter.supply_preservation) + 1)
			state.bike.capacity += 1
			_mark_facility_used("storage")
			state.last_event = "林行把食物、水和路上要带的东西重新打包，撤离时能少丢一些。"
		"treat_wound":
			if _spend("meds", 1):
				state.lin.health = min(10, int(state.lin.health) + 1)
				state.lin.infection_risk = max(0, int(state.lin.infection_risk) - 1)
				state.last_event = "林行用药品处理伤口，体温稍微压下去，感染风险降低。"
			else:
				state.last_event = "没有药品，林行只能用清水压住伤口。"
		"fortify":
			if _spend("materials", 2):
				state.shelter.door += 2
				state.shelter.defense += 1
				state.last_event = "林行用木板和铁丝加固门窗。"
			else:
				state.last_event = "建材不足，无法加固。"
		"quiet":
			state.shelter.noise = max(0, int(state.shelter.noise) - 1)
			state.lin.stress += 1
			state.last_event = "林行拆掉会响的杂物，据点安静了一些。"
		"mask_scent":
			if _spend("materials", 1):
				state.shelter.scent = max(0, int(state.shelter.scent) - 1)
				state.last_event = "林行封住垃圾和血腥味，降低尸群注意。"
			else:
				state.last_event = "缺少布料和胶带，气味遮蔽失败。"
		"repair_bike":
			return perform_shelter_action("workbench_repair")
		"radio":
			return perform_shelter_action("radio_broadcast")
		_:
			state.last_event = "林行什么也没来得及做。"
	state.phase = "night"
	return state.last_event

func sleep_and_resolve_night() -> String:
	var day := int(state.day)
	var night_events := []
	_consume_daily_resources()
	var infection_event := _resolve_infection_pressure()
	if infection_event != "":
		night_events.append(infection_event)
	if is_blood_moon_day(day):
		night_events.append(_resolve_blood_moon(day))
	resolve_qimian_for_day(day)
	if not state.qimian.public_clues.is_empty():
		night_events.append(state.qimian.public_clues[-1])

	if day >= MAX_DEMO_DAY:
		state.demo_complete = true
		state.reveal.unlocked = true
		state.ending_state = _determine_ending_state()
		state.reveal.summary = _ending_summary(state.ending_state)
		state.phase = "reveal"
		state.last_event = "Demo 结束。祁眠行动日志解锁。"
		return state.last_event

	start_day(day + 1)
	if not night_events.is_empty():
		state.last_event = "%s\n昨夜：%s" % [state.last_event, " ".join(night_events)]
	return state.last_event

func play_safe_demo_day(day: int) -> void:
	start_day(day)
	var location_id := "clinic"
	if day == 1:
		location_id = "home"
	elif day == 2:
		location_id = "convenience"
	elif day == 3:
		location_id = "clinic"
	elif day >= 4:
		location_id = "bike_shop"
	if day >= 8:
		location_id = "supermarket"
	if day >= 12:
		location_id = "subway"
	explore(location_id)
	if is_blood_moon_day(day):
		perform_shelter_action("fortify")
	elif day % 3 == 0:
		perform_shelter_action("radio")
	elif day % 2 == 0:
		perform_shelter_action("repair_bike")
	else:
		perform_shelter_action("quiet")
	sleep_and_resolve_night()

func resolve_qimian_for_day(day: int) -> void:
	if day < 5:
		return
	state.qimian.awake = true
	if not _qimian_plan.has(day):
		return
	for action in _qimian_plan[day]:
		_apply_qimian_action(day, action)

func _default_locations() -> Dictionary:
	return {
		"home": _location("home", "林行家", "近圈", 1, 1, {"food": 1, "water": 2, "materials": 1}, "少量补给", "低", 1, "熟路", ["home"]),
		"convenience": _location("convenience", "小区便利店", "近圈", 1, 3, {"food": 4, "water": 4, "fuel": 1}, "食物/水", "中", 1, "碎玻璃", ["food", "water"]),
		"clinic": _location("clinic", "社区诊所", "近圈", 1, 2, {"meds": 2, "materials": 1}, "药品", "中", 1, "雨后湿滑", ["meds", "question"]),
		"bike_shop": _location("bike_shop", "自行车修理铺", "近圈", 1, 2, {"parts": 3, "materials": 2}, "零件/建材", "中", 1, "堵塞", ["parts", "bike"]),
		"supermarket": _location("supermarket", "超市", "中圈", 2, 4, {"food": 8, "water": 4, "materials": 2}, "大量食物", "高", 2, "尸群迁移", ["food", "danger"]),
		"school": _location("school", "废弃学校", "中圈", 2, 3, {"materials": 4, "fuel": 1}, "建材/燃料", "中", 2, "积水", ["materials"]),
		"police": _location("police", "派出所", "中圈", 2, 5, {"fuel": 2, "materials": 1}, "燃料/地图线索", "高", 2, "路障", ["fuel", "clue"]),
		"subway": _location("subway", "地铁口", "中圈", 2, 5, {"materials": 2, "fuel": 1}, "路线线索", "高", 2, "尸群迁移", ["route", "question"]),
		"safezone_edge": _location("safezone_edge", "保护区外围", "远圈", 3, 6, {"materials": 1}, "保护区线索", "极高", 3, "封锁线", ["safezone", "danger"])
	}

func _location(location_id: String, name: String, ring: String, range: int, zombies: int, resources: Dictionary, resource_tendency: String, danger_level: String, route_time: int, road_condition: String, icons: Array) -> Dictionary:
	return {
		"name": name,
		"ring": ring,
		"range": range,
		"zombies": zombies,
		"resources": resources,
		"resource_tendency": resource_tendency,
		"danger_level": danger_level,
		"route_time": route_time,
		"road_condition": road_condition,
		"icons": icons,
		"qimian_trace": false,
		"rooms": _rooms_for_location(location_id),
		"visited": false
	}

func _empty_exploration_state() -> Dictionary:
	return {
		"active_location": "",
		"time_used": 0,
		"time_limit": 0,
		"noise": 0,
		"searched_rooms": [],
		"lured_rooms": []
	}

func _rooms_for_location(location_id: String) -> Dictionary:
	match location_id:
		"convenience":
			return {
				"front_store": _room("前厅货架", "窗光", 1, 0, {"food": 2, "water": 1}),
				"back_room": _room("后仓库", "黑暗", 2, 1, {"water": 1, "fuel": 1})
			}
		"clinic":
			return {
				"front_store": _room("接诊台", "窗光", 1, 0, {"meds": 1, "materials": 1}),
				"pharmacy": _room("药房", "黑暗", 2, 1, {"meds": 2})
			}
		"supermarket":
			return {
				"front_store": _room("收银区", "昏暗", 1, 1, {"food": 1, "water": 1}),
				"back_room": _room("冷库门口", "黑暗", 2, 2, {"food": 3})
			}
		_:
			return {
				"front_store": _room("入口房间", "昏暗", 1, 0, {"materials": 1}),
				"back_room": _room("深处房间", "黑暗", 2, 1, {})
			}

func _room(name: String, visibility: String, search_time: int, hidden_zombies: int, resources: Dictionary) -> Dictionary:
	return {
		"name": name,
		"visibility": visibility,
		"search_time": search_time,
		"hidden_zombies": hidden_zombies,
		"resources": resources,
		"searched": false
	}

func _search_time_for_tactic(room: Dictionary, tactic: String) -> int:
	match tactic:
		"quick":
			return max(1, int(room.search_time) - 1)
		"careful":
			return int(room.search_time)
		_:
			return int(room.search_time)

func _apply_room_search_risk(room_id: String, room: Dictionary, tactic: String) -> Array:
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

func _room_note_text(notes: Array) -> String:
	if notes.is_empty():
		return ""
	return "%s " % " ".join(notes)

func _room_threat_text(room_id: String, room: Dictionary) -> String:
	if int(room.hidden_zombies) <= 0:
		return "暂时安静"
	if state.exploration.lured_rooms.has(room_id):
		return "隐藏尸群：已引开"
	return "隐藏尸群：未引开"

func _default_facilities() -> Dictionary:
	return {
		"bed": _facility("床铺", "recover"),
		"workbench": _facility("工作台", "craft_repair"),
		"barricade": _facility("封窗", "blood_moon_defense"),
		"radio": _facility("收音机", "broadcast_clues"),
		"storage": _facility("储物/整理台", "preserve_carry")
	}

func _facility(name: String, role: String) -> Dictionary:
	return {
		"name": name,
		"role": role,
		"level": 1,
		"used_today": false
	}

func _mark_facility_used(facility_id: String) -> void:
	if state.shelter.facilities.has(facility_id):
		state.shelter.facilities[facility_id].used_today = true

func _reset_facility_use() -> void:
	for facility_id in state.shelter.facilities.keys():
		state.shelter.facilities[facility_id].used_today = false

func _day_event(day: int, morning_text: String, pressure_type: String, clue: String, blood_moon_warning: String, modifiers: Dictionary) -> Dictionary:
	return {
		"day": day,
		"morning_text": morning_text,
		"pressure_type": pressure_type,
		"clue": clue,
		"blood_moon_warning": blood_moon_warning,
		"modifiers": modifiers
	}

func _apply_day_pressure(event: Dictionary) -> void:
	var modifiers: Dictionary = event.modifiers
	for key in modifiers.keys():
		var amount: int = int(modifiers[key])
		match String(key):
			"food":
				state.resources.food = max(0, int(state.resources.food) + amount)
			"water":
				state.resources.water = max(0, int(state.resources.water) + amount)
			"stress":
				state.lin.stress = max(0, int(state.lin.stress) + amount)
			"hope":
				state.lin.hope = max(0, int(state.lin.hope) + amount)
			"door":
				state.shelter.door = max(0, int(state.shelter.door) + amount)
			"noise":
				state.shelter.noise = max(0, int(state.shelter.noise) + amount)
			"scent":
				state.shelter.scent = max(0, int(state.shelter.scent) + amount)
			"bike_durability":
				state.bike.durability = max(0, int(state.bike.durability) + amount)

func _is_location_depleted(location: Dictionary) -> bool:
	for resource_name in location.resources.keys():
		if int(location.resources[resource_name]) > 0:
			return false
	return true

func _apply_exploration_risk(location: Dictionary) -> void:
	var pressure: int = int(location.zombies) + int(location.range) + int(state.bike.noise)
	if pressure >= 8:
		state.lin.health = max(0, int(state.lin.health) - 1)
		state.lin.stress += 2
	elif pressure >= 5:
		state.lin.stress += 1

func _road_condition_fatigue_penalty(location: Dictionary) -> int:
	match str(location.road_condition):
		"熟路", "碎玻璃":
			return 0
		"雨后湿滑", "积水", "堵塞", "路障":
			return 1
		"尸群迁移", "封锁线":
			return 2
		_:
			return 1

func _apply_evacuation_clues(location_id: String, location: Dictionary) -> Array:
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

func _apply_qimian_action(day: int, action: Dictionary) -> void:
	if action.has("location"):
		var location: Dictionary = state.locations[action.location]
		location.qimian_trace = true
		if not location.icons.has("qimian"):
			location.icons.append("qimian")
		if action.has("resource"):
			var resource_name: String = action.resource
			location.resources[resource_name] = max(0, int(location.resources.get(resource_name, 0)) + int(action.amount))
		if action.has("zombie_delta"):
			location.zombies = max(0, int(location.zombies) + int(action.zombie_delta))
	if action.has("resource_gain"):
		for resource_name in action.resource_gain.keys():
			state.resources[resource_name] = int(state.resources.get(resource_name, 0)) + int(action.resource_gain[resource_name])
	state.qimian.public_clues.append(action.public_clue)
	state.qimian.log.append({
		"day": day,
		"title": action.title,
		"truth": action.truth,
		"public_clue": action.public_clue,
		"ai_replay": str(action.get("ai_replay", "")),
		"subjective_fragment": str(action.get("subjective_fragment", ""))
	})

func _location_trace_suffix(location: Dictionary) -> String:
	if bool(location.get("qimian_trace", false)):
		return "\n祁眠异常：此处留下了非普通幸存者造成的痕迹。"
	return ""

func _resolve_blood_moon(day: int) -> String:
	var support := _qimian_blood_moon_support(day)
	var pressure: int = 4 + int(day / 7) * 2 + int(state.shelter.noise) + int(state.shelter.scent) + int(state.shelter.light)
	pressure -= int(state.shelter.door) + int(state.shelter.defense) + support
	if not state.blood_moons_resolved.has(day):
		state.blood_moons_resolved.append(day)
	if pressure <= 3:
		state.lin.hope += 1
		return "血月被稳稳撑过去，林行听见远处尸群被引开的声音。"
	if pressure <= 6:
		state.shelter.door = max(1, int(state.shelter.door) - 1)
		state.resources.food = max(0, int(state.resources.food) - 1)
		return "血月擦着据点过去，门窗受损，食物也少了一些。"
	state.lin.health -= 2
	state.shelter.door = max(0, int(state.shelter.door) - 2)
	return "血月冲破了外层防线，林行受伤，但仍撑到了天亮。"

func _determine_ending_state() -> String:
	if int(state.lin.health) <= 0:
		return "collapsed"
	if int(state.lin.hunger) >= 4 and int(state.lin.thirst) >= 4:
		return "collapsed"
	if int(state.shelter.door) <= 0 and int(state.lin.health) <= 2:
		return "collapsed"
	if int(state.lin.health) <= 3 or int(state.shelter.door) <= 1:
		return "barely_reached_gate"
	if int(state.lin.hunger) >= 4 or int(state.lin.thirst) >= 4:
		return "barely_reached_gate"
	if bool(state.evacuation.safezone_confirmed) and bool(state.evacuation.address_known) and bool(state.evacuation.bike_ready):
		return "reached_gate_quarantine"
	return "barely_reached_gate"

func _ending_summary(ending_state: String) -> String:
	var supply_phrase := ""
	if int(state.shelter.get("supply_preservation", 0)) > 0:
		supply_phrase = "他带着整理好的物资抵达筛查棚，"
	match ending_state:
		"collapsed":
			return "林行没能稳定抵达保护区，只在崩溃边缘听见尸群路线异常的传闻。祁眠日志揭示：尸群中藏着那个改变路线的人。"
		"barely_reached_gate":
			return "林行勉强抵达保护区大门外，%s通过初筛后被要求隔离观察。筛查棚外有人低声说，昨晚那股尸群像被人牵走了。" % supply_phrase
		_:
			return "林行抵达保护区大门外，%s通过初筛后被要求隔离观察。玩家随后看到祁眠藏在尸群中改变路线的完整日志：这并非为了林行，却间接救下了他。" % supply_phrase

func _qimian_blood_moon_support(day: int) -> int:
	var support := 0
	if not _qimian_plan.has(day):
		return support
	for action in _qimian_plan[day]:
		support += int(action.get("blood_moon_support", 0))
	return support

func _consume_daily_resources() -> void:
	state.resources.food = max(0, int(state.resources.food) - 1)
	state.resources.water = max(0, int(state.resources.water) - 1)
	state.lin.hunger = 0 if int(state.resources.food) > 0 else int(state.lin.hunger) + 1
	state.lin.thirst = 0 if int(state.resources.water) > 0 else int(state.lin.thirst) + 1
	state.lin.fatigue = max(0, int(state.lin.fatigue) - 1)
	state.lin.stress = max(0, int(state.lin.stress) - int(state.lin.hope / 3))

func _resolve_infection_pressure() -> String:
	if int(state.lin.infection_risk) < 5:
		return ""
	state.lin.health = max(0, int(state.lin.health) - 1)
	state.lin.stress += 2
	return "感染风险恶化，林行发热、伤口发烫，生命和压力都受到影响。"

func _spend(resource_name: String, amount: int) -> bool:
	if int(state.resources.get(resource_name, 0)) < amount:
		return false
	state.resources[resource_name] = int(state.resources[resource_name]) - amount
	return true

func _radio_message_for_day(day: int) -> String:
	if is_blood_moon_day(day + 1):
		return "收音机警告：明晚月色异常，保护区要求外围幸存者熄灯静默。"
	if day >= 9:
		return "广播短暂说清保护区外圈筛查棚地址，但提醒所有人必须接受感染初筛。"
	if day >= 5:
		return "广播夹杂着陌生敲击声，有人正在保护区外转移幸存者。"
	return "断续广播提到保护区仍在接收幸存者，但外围路线已经封锁。"
