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
			"escape": 0
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

func get_location_label(location_id: String) -> String:
	var location: Dictionary = state.locations[location_id]
	var visit_label := "已搜" if bool(location.visited) else "未搜"
	var stock_label := "已空" if _is_location_depleted(location) else "有物资"
	var range_label := "可达" if int(location.range) <= int(state.bike.range) else "太远"
	return "%s / %s / 尸群%s / %s / %s / %s" % [
		location.name,
		location.ring,
		str(location.zombies),
		visit_label,
		stock_label,
		range_label
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
	_apply_exploration_risk(location)
	state.bike.durability = max(0, int(state.bike.durability) - location.range)
	state.lin.fatigue += location.range
	state.lin.stress += max(0, int(location.zombies) - 2)
	state.phase = "evening"

	if found.is_empty():
		state.last_event = "探索 %s。%s 这里几乎被搜空了，只留下难以解释的翻动痕迹。" % [location.name, risk_text]
	else:
		state.last_event = "探索 %s。%s 带回：%s。" % [location.name, risk_text, ", ".join(found)]
	return state.last_event

func perform_shelter_action(action_id: String) -> String:
	match action_id:
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
			if _spend("parts", 1):
				state.bike.durability += 3
				state.bike.range = min(3, int(state.bike.range) + 1)
				if int(state.bike.range) >= 3:
					state.evacuation.bike_ready = true
				state.last_event = "自行车修好了些，明天能走更远。"
			else:
				state.last_event = "没有足够零件修车。"
		"radio":
			if _spend("fuel", 1):
				state.lin.hope += 1
				if int(state.day) >= 3:
					state.evacuation.safezone_confirmed = true
				if int(state.day) >= 9:
					state.evacuation.address_known = true
				state.last_event = _radio_message_for_day(int(state.day))
			else:
				state.last_event = "发电机没有燃料，收音机只剩沙沙声。"
		_:
			state.last_event = "林行什么也没来得及做。"
	state.phase = "night"
	return state.last_event

func sleep_and_resolve_night() -> String:
	var day := int(state.day)
	var night_events := []
	_consume_daily_resources()
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
		"home": _location("林行家", "近圈", 1, 1, {"food": 1, "water": 2, "materials": 1}),
		"convenience": _location("小区便利店", "近圈", 1, 3, {"food": 4, "water": 4, "fuel": 1}),
		"clinic": _location("社区诊所", "近圈", 1, 2, {"meds": 2, "materials": 1}),
		"bike_shop": _location("自行车修理铺", "近圈", 1, 2, {"parts": 3, "materials": 2}),
		"supermarket": _location("超市", "中圈", 2, 4, {"food": 8, "water": 4, "materials": 2}),
		"school": _location("废弃学校", "中圈", 2, 3, {"materials": 4, "fuel": 1}),
		"police": _location("派出所", "中圈", 2, 5, {"fuel": 2, "materials": 1}),
		"subway": _location("地铁口", "中圈", 2, 5, {"materials": 2, "fuel": 1}),
		"safezone_edge": _location("保护区外围", "远圈", 3, 6, {"materials": 1})
	}

func _location(name: String, ring: String, range: int, zombies: int, resources: Dictionary) -> Dictionary:
	return {
		"name": name,
		"ring": ring,
		"range": range,
		"zombies": zombies,
		"resources": resources,
		"visited": false
	}

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

func _apply_qimian_action(day: int, action: Dictionary) -> void:
	if action.has("location"):
		var location: Dictionary = state.locations[action.location]
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
	match ending_state:
		"collapsed":
			return "林行没能稳定抵达保护区，只在崩溃边缘听见尸群路线异常的传闻。祁眠日志揭示：尸群中藏着那个改变路线的人。"
		"barely_reached_gate":
			return "林行勉强抵达保护区大门外，通过初筛后被要求隔离观察。筛查棚外有人低声说，昨晚那股尸群像被人牵走了。"
		_:
			return "林行抵达保护区大门外，通过初筛后被要求隔离观察。玩家随后看到祁眠藏在尸群中改变路线的完整日志：这并非为了林行，却间接救下了他。"

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
