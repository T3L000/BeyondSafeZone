extends RefCounted

const MAX_DEMO_DAY := 14
const FULL_DAY_LIMIT := 30

var state: Dictionary = {}

var _day_events := {
	1: _day_event(1, "医院走廊还残留着隔离带，陈醒只能先确认自己还能走。", "tutorial", "收音机里反复出现保护区断续广播。", "", {}),
	2: _day_event(2, "楼下有人翻过垃圾桶，瓶装水比昨天更难找。", "scarcity", "便利店门口的玻璃碎得很整齐。", "", {"water": -1}),
	3: _day_event(3, "清晨有短促敲门声，门外只剩一串拖痕。", "stress", "墙上多了一句保护区方向的粉笔字。", "", {"stress": 1}),
	4: _day_event(4, "自行车链条卡住了，远处广播却催促幸存者尽快转移。", "mobility", "修理铺附近的尸群被什么声音吸引过。", "", {"bike_durability": -1}),
	5: _day_event(5, "雨停后气味闷在楼道里，据点开始暴露生活痕迹。", "shelter", "楼梯口能闻到潮湿血腥味。", "", {"scent": 1, "stress": 1}),
	6: _day_event(6, "月色比平时更红，收音机要求外围幸存者提前熄灯。", "warning", "保护区广播第一次提到血月。", "明晚血月：门窗、防御、噪音和气味会决定据点能不能撑住。", {"noise": 1}),
	7: _day_event(7, "血月当天，街上几乎没有普通尸群的游荡声，像是在等夜晚。", "blood_moon", "窗外的月亮还没升起，玻璃已经开始轻轻震动。", "今晚血月：这是第一次防守考试。", {"stress": 1}),
	8: _day_event(8, "血月过后，附近街区被翻得乱七八糟。", "aftermath", "保护区广播说中圈仍有通行可能。", "", {"door": -1}),
	9: _day_event(9, "自行车还能撑一段路，但每一次远行都会留下更响的动静。", "mobility", "废弃学校方向飘来断续铃声。", "", {"bike_durability": -1}),
	10: _day_event(10, "医院旧楼的门被风吹开，里面安静得不正常。", "foreshadow", "有个药柜像是被人从里面重新锁上。", "", {"stress": 1}),
	11: _day_event(11, "清晨的街道少了一些尸群，陈醒却感觉有人比自己更早醒来。", "qimian", "医院旧楼留下了一支没用完的消毒液。", "", {"hope": 1}),
	12: _day_event(12, "超市方向没有争抢声，只有货架被拖动后的空响。", "qimian", "最容易保存的食物像是被人有计划地拿走。", "", {"stress": 1}),
	13: _day_event(13, "地铁口的尸群突然稀了，墙上有一道像箭头的划痕。", "qimian", "那道箭头像是在给谁指路。", "明晚第二次血月：资源和据点都会被一起考验。", {"hope": 1}),
	14: _day_event(14, "第二次血月压到城市上空，远处金属声断断续续。", "blood_moon", "据点门口多了一个没有署名的药包。", "今晚血月：如果据点撑住，祁眠日志会解锁。", {"stress": 2})
}

var _qimian_plan := {
	11: [
		{
			"title": "祁眠醒来",
			"location": "supermarket",
			"resource": "food",
			"amount": -1,
			"public_clue": "医院旧楼的药柜被人重新锁上，旁边留着一支没用完的消毒液。",
			"truth": "祁眠从昏睡中醒来，确认普通丧尸不会主动攻击自己，并拿走一小包便携食物。"
		}
	],
	12: [
		{
			"title": "超市夜行",
			"location": "supermarket",
			"resource": "food",
			"amount": -3,
			"public_clue": "超市货架像被人有计划地清过，最容易保存的食物少了一批。",
			"truth": "祁眠搬走罐头和压缩饼干，把一部分分给桥洞营地。"
		}
	],
	13: [
		{
			"title": "尸群偏移",
			"location": "subway",
			"zombie_delta": -2,
			"public_clue": "地铁口的尸群少了，墙上有一条像是刻意留下的箭头。",
			"truth": "祁眠用声音把尸群引到另一条街，为陌生幸存者打开路。"
		}
	],
	14: [
		{
			"title": "匿名药包",
			"resource_gain": {"meds": 1},
			"public_clue": "据点门口多了一个没有署名的药包。",
			"truth": "祁眠确认陈醒还活着，但选择不见面，只留下药品。"
		},
		{
			"title": "血月分流",
			"blood_moon_support": 2,
			"public_clue": "血月夜远处突然响起金属敲击声，一部分丧尸被引走。",
			"truth": "祁眠趁血月把尸潮从陈醒据点附近引开。"
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
		"last_event": "陈醒醒来。收音机里反复出现保护区断续广播。",
		"morning_context": {},
		"applied_day_events": [],
		"resources": {
			"food": 5,
			"water": 5,
			"meds": 2,
			"materials": 4,
			"parts": 1,
			"batteries": 2,
			"intel": 0
		},
		"chen": {
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
			"public_clues": []
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
		state.chen.fatigue += 1
		state.last_event = "%s 太远了。%s 陈醒需要修好自行车或找到更安全的路线。" % [location.name, get_location_risk_text(location_id)]
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
	state.chen.fatigue += location.range
	state.chen.stress += max(0, int(location.zombies) - 2)
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
				state.last_event = "陈醒用木板和铁丝加固门窗。"
			else:
				state.last_event = "建材不足，无法加固。"
		"quiet":
			state.shelter.noise = max(0, int(state.shelter.noise) - 1)
			state.chen.stress += 1
			state.last_event = "陈醒拆掉会响的杂物，据点安静了一些。"
		"mask_scent":
			if _spend("materials", 1):
				state.shelter.scent = max(0, int(state.shelter.scent) - 1)
				state.last_event = "陈醒封住垃圾和血腥味，降低尸群注意。"
			else:
				state.last_event = "缺少布料和胶带，气味遮蔽失败。"
		"repair_bike":
			if _spend("parts", 1):
				state.bike.durability += 3
				state.bike.range = min(3, int(state.bike.range) + 1)
				state.last_event = "自行车修好了些，明天能走更远。"
			else:
				state.last_event = "没有足够零件修车。"
		"radio":
			if _spend("batteries", 1):
				state.resources.intel += 1
				state.chen.hope += 1
				state.last_event = _radio_message_for_day(int(state.day))
			else:
				state.last_event = "收音机没电，只剩沙沙声。"
		_:
			state.last_event = "陈醒什么也没来得及做。"
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
		location_id = "hospital"
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
	if day < 11:
		return
	state.qimian.awake = true
	if not _qimian_plan.has(day):
		return
	for action in _qimian_plan[day]:
		_apply_qimian_action(day, action)

func _default_locations() -> Dictionary:
	return {
		"hospital": _location("医院病房", "近圈", 1, 2, {"meds": 3, "water": 3}),
		"convenience": _location("小区便利店", "近圈", 1, 3, {"food": 4, "water": 4, "batteries": 1}),
		"clinic": _location("社区诊所", "近圈", 1, 2, {"meds": 2, "materials": 1}),
		"bike_shop": _location("自行车修理铺", "近圈", 1, 2, {"parts": 3, "materials": 2}),
		"supermarket": _location("超市", "中圈", 2, 4, {"food": 8, "water": 4, "materials": 2}),
		"school": _location("废弃学校", "中圈", 2, 3, {"materials": 4, "intel": 1}),
		"police": _location("派出所", "中圈", 2, 5, {"batteries": 2, "intel": 2}),
		"subway": _location("地铁口", "中圈", 2, 5, {"materials": 2, "intel": 1}),
		"safezone_edge": _location("保护区外围", "远圈", 3, 6, {"intel": 3})
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
				state.chen.stress = max(0, int(state.chen.stress) + amount)
			"hope":
				state.chen.hope = max(0, int(state.chen.hope) + amount)
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
		state.chen.health = max(0, int(state.chen.health) - 1)
		state.chen.stress += 2
	elif pressure >= 5:
		state.chen.stress += 1

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
		"public_clue": action.public_clue
	})

func _resolve_blood_moon(day: int) -> String:
	var support := _qimian_blood_moon_support(day)
	var pressure: int = 4 + int(day / 7) * 2 + int(state.shelter.noise) + int(state.shelter.scent) + int(state.shelter.light)
	pressure -= int(state.shelter.door) + int(state.shelter.defense) + support
	if not state.blood_moons_resolved.has(day):
		state.blood_moons_resolved.append(day)
	if pressure <= 3:
		state.chen.hope += 1
		return "血月被稳稳撑过去，陈醒听见远处尸群被引开的声音。"
	if pressure <= 6:
		state.shelter.door = max(1, int(state.shelter.door) - 1)
		state.resources.food = max(0, int(state.resources.food) - 1)
		return "血月擦着据点过去，门窗受损，食物也少了一些。"
	state.chen.health -= 2
	state.shelter.door = max(0, int(state.shelter.door) - 2)
	return "血月冲破了外层防线，陈醒受伤，但仍撑到了天亮。"

func _determine_ending_state() -> String:
	if int(state.chen.health) <= 0:
		return "collapsed"
	if int(state.chen.hunger) >= 4 and int(state.chen.thirst) >= 4:
		return "collapsed"
	if int(state.shelter.door) <= 0 and int(state.chen.health) <= 2:
		return "collapsed"
	if int(state.chen.health) <= 3 or int(state.shelter.door) <= 1:
		return "barely_survived"
	if int(state.chen.hunger) >= 4 or int(state.chen.thirst) >= 4:
		return "barely_survived"
	return "survived_demo"

func _ending_summary(ending_state: String) -> String:
	match ending_state:
		"collapsed":
			return "陈醒撑到了日志解锁的清晨，却几乎失去继续撤离的能力。祁眠日志揭示：那些空货架、被引走的尸群和匿名药包都来自同一个夜行者。"
		"barely_survived":
			return "陈醒勉强撑过第 14 天血月。祁眠日志揭示：那些空货架、被引走的尸群和匿名药包都来自同一个夜行者。"
		_:
			return "陈醒稳稳撑过第 14 天血月。祁眠日志揭示：那些空货架、被引走的尸群和匿名药包都来自同一个夜行者。"

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
	state.chen.hunger = 0 if int(state.resources.food) > 0 else int(state.chen.hunger) + 1
	state.chen.thirst = 0 if int(state.resources.water) > 0 else int(state.chen.thirst) + 1
	state.chen.fatigue = max(0, int(state.chen.fatigue) - 1)
	state.chen.stress = max(0, int(state.chen.stress) - int(state.chen.hope / 3))

func _spend(resource_name: String, amount: int) -> bool:
	if int(state.resources.get(resource_name, 0)) < amount:
		return false
	state.resources[resource_name] = int(state.resources[resource_name]) - amount
	return true

func _radio_message_for_day(day: int) -> String:
	if is_blood_moon_day(day + 1):
		return "收音机警告：明晚月色异常，保护区要求外围幸存者熄灯静默。"
	if day >= 11:
		return "广播夹杂着陌生敲击声，有人正在保护区外转移幸存者。"
	return "断续广播提到保护区仍在接收幸存者，但外围路线已经封锁。"
