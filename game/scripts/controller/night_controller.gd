# 夜晚结算系统 —— 规则层：噪音传播/感染/血月/红潮/结局/撤离叙事
class_name NightResolver extends RefCounted

const GameConsts = preload("res://scripts/data/constants.gd")
const Balance = preload("res://scripts/data/balance.gd")
const QimianPlan = preload("res://scripts/data/qimian_plan.gd")
const CarSys = preload("res://scripts/controller/car_controller.gd")
const _GameState = preload("res://scripts/model/game_state.gd")

static func resolve(state: _GameState, start_next_day: Callable) -> String:
	var day := int(state.day)
	var night_events := []
	_consume_daily_resources(state)
	var noise_event := _propagate_noise(state, day)
	if noise_event != "":
		night_events.append(noise_event)
	var infection_event := _resolve_infection_pressure(state)
	if infection_event != "":
		night_events.append(infection_event)
	if GameConsts.BLOOD_MOON_DAYS.has(day):
		night_events.append(_resolve_blood_moon(state, day))
	elif day >= 11 and day <= 14:
		var red_tide_event := _resolve_red_tide(state, day)
		if red_tide_event != "":
			night_events.append(red_tide_event)
	# Qimian public clues (resolved externally before calling this)
	if not state.qimian.public_clues.is_empty():
		night_events.append(state.qimian.public_clues[-1])

	if day >= GameConsts.MAX_DEMO_DAY:
		state.demo_complete = true
		state.reveal.unlocked = true
		if bool(state.car.ready):
			state.car.breakdown = "engine_overheat"
			night_events.append("左前轮在远郊路面上爆了。汽车滑进路边沟里，引擎熄火，再也发不动。")
		elif bool(state.car.found):
			state.car.breakdown = "not_ready"
			night_events.append("汽车还没修好。林行只能背起背包，骑自行车到路尽头，然后徒步走向保护区。")
		else:
			night_events.append("没有找到能用的载具。林行把能带的全塞进背包，推开据点的门，走进血月里。")
		state.ending_state = _determine_ending_state(state)
		state.reveal.summary = _ending_summary(state, state.ending_state)
		state.phase = "reveal"
		state.last_event = "Demo 结束。祁眠行动日志解锁。"
		return state.last_event

	start_next_day.call(day + 1)
	if not night_events.is_empty():
		state.last_event = "%s\n昨夜：%s" % [state.last_event, " ".join(night_events)]
	return state.last_event

# --- Private helpers ---

static func _consume_daily_resources(state: _GameState) -> void:
	state.resources.food = max(0, int(state.resources.food) - Balance.DAILY_CONSUME.food)
	state.resources.water = max(0, int(state.resources.water) - Balance.DAILY_CONSUME.water)
	state.lin.hunger = 0 if int(state.resources.food) > 0 else int(state.lin.hunger) + Balance.HUNGER_PER_DAY_NO_FOOD
	state.lin.thirst = 0 if int(state.resources.water) > 0 else int(state.lin.thirst) + Balance.THIRST_PER_DAY_NO_WATER
	state.lin.fatigue = max(0, int(state.lin.fatigue) - Balance.FATIGUE_RECOVER_PER_NIGHT)
	state.lin.stress = max(0, int(state.lin.stress) - int(state.lin.hope / Balance.HOPE_STRESS_DIVISOR))

static func _propagate_noise(state: _GameState, day: int) -> String:
	var shelter_noise: int = int(state.shelter.noise)
	var exploration_noise: int = int(state.exploration.noise)
	var total_noise: int = shelter_noise + exploration_noise
	if total_noise <= Balance.NOISE_ATTRACT_THRESHOLD:
		return ""
	var attracted := 0
	for location_id in ["convenience", "clinic", "bike_shop"]:
		if not state.locations.has(location_id):
			continue
		var loc: Dictionary = state.locations[location_id]
		if int(loc.range) > Balance.NOISE_ATTRACT_RANGE:
			continue
		var attract: int = clampi(total_noise - Balance.NOISE_ATTRACT_THRESHOLD, 0, Balance.NOISE_ATTRACT_MAX)
		state.locations[location_id].zombies = int(loc.zombies) + attract
		attracted += attract
	if attracted > 0:
		if total_noise >= 6:
			return "据点的噪音引来了近圈的尸群（+%d），明天探索风险增加。" % attracted
		return "夜里有些动静吸引了尸群注意（+%d）。" % attracted
	return ""

static func _resolve_infection_pressure(state: _GameState) -> String:
	if int(state.lin.infection_risk) < Balance.INFECTION_CRITICAL_THRESHOLD:
		return ""
	state.lin.health = max(0, int(state.lin.health) - Balance.INFECTION_HEALTH_PENALTY)
	state.lin.stress += Balance.INFECTION_STRESS_PENALTY
	return "感染风险恶化，林行发热、伤口发烫，生命和压力都受到影响。"

static func _resolve_blood_moon(state: _GameState, day: int) -> String:
	var support := _qimian_blood_moon_support(day)
	var pressure: int = Balance.BM_BASE_PRESSURE + int(day / Balance.BM_DAY_DIVISOR) * Balance.BM_DAY_MULT + int(state.shelter.noise) + int(state.shelter.scent) + int(state.shelter.light)
	pressure -= int(state.shelter.door) + int(state.shelter.defense) + support
	if not state.blood_moons_resolved.has(day):
		state.blood_moons_resolved.append(day)
	if pressure <= Balance.BM_LOW_THRESHOLD:
		state.lin.hope += Balance.BM_LOW_HOPE
		return "血月被稳稳撑过去，林行听见远处尸群被引开的声音。"
	if pressure <= Balance.BM_MID_THRESHOLD:
		state.shelter.door = max(1, int(state.shelter.door) - Balance.BM_MID_DOOR)
		state.resources.food = max(0, int(state.resources.food) - Balance.BM_MID_FOOD)
		return "血月擦着据点过去，门窗受损，食物也少了一些。"
	state.lin.health -= Balance.BM_HIGH_HEALTH
	state.shelter.door = max(0, int(state.shelter.door) - Balance.BM_HIGH_DOOR)
	return "血月冲破了外层防线，林行受伤，但仍撑到了天亮。"

static func _resolve_red_tide(state: _GameState, day: int) -> String:
	var intensity: int = day - Balance.RT_DAY_OFFSET
	var pressure: int = intensity + int(state.shelter.noise) + int(state.shelter.scent) + int(state.shelter.light)
	pressure -= int(state.shelter.door) + int(state.shelter.defense)
	if pressure <= Balance.RT_LOW_THRESHOLD:
		state.lin.stress += Balance.RT_LOW_STRESS
		return "红潮在窗外涌动了一夜，但没有突破防线。"
	if pressure <= Balance.RT_MID_THRESHOLD:
		state.lin.stress += Balance.RT_MID_STRESS
		state.resources.food = max(0, int(state.resources.food) - Balance.RT_MID_FOOD)
		state.shelter.door = max(1, int(state.shelter.door) - Balance.RT_MID_DOOR)
		return "红潮让尸群比预想中密集，据点门窗受损，食物也少了一些。"
	state.lin.health = max(0, int(state.lin.health) - Balance.RT_HIGH_HEALTH)
	state.lin.stress += Balance.RT_HIGH_STRESS
	state.shelter.door = max(0, int(state.shelter.door) - Balance.RT_HIGH_DOOR)
	state.resources.food = max(0, int(state.resources.food) - Balance.RT_HIGH_FOOD)
	return "红潮的密度压过了防御，林行被碎片划伤，据点出现缺口。"

static func _determine_ending_state(state: _GameState) -> String:
	if int(state.lin.health) <= Balance.ENDING_HEALTH_DEAD:
		return "collapsed"
	if int(state.lin.hunger) >= Balance.ENDING_HUNGER_CRITICAL and int(state.lin.thirst) >= Balance.ENDING_THIRST_CRITICAL:
		return "collapsed"
	if int(state.shelter.door) <= Balance.ENDING_HEALTH_DEAD and int(state.lin.health) <= 2:
		return "collapsed"
	if int(state.lin.health) <= Balance.ENDING_HEALTH_BARELY or int(state.shelter.door) <= Balance.ENDING_DOOR_BARELY:
		return "barely_reached_gate"
	if int(state.lin.hunger) >= Balance.ENDING_HUNGER_CRITICAL or int(state.lin.thirst) >= Balance.ENDING_THIRST_CRITICAL:
		return "barely_reached_gate"
	if bool(state.evacuation.safezone_confirmed) and bool(state.evacuation.address_known) and bool(state.evacuation.car_ready):
		return "reached_gate_quarantine"
	return "barely_reached_gate"

static func _ending_summary(state: _GameState, ending_state: String) -> String:
	var supply_phrase := ""
	if int(state.shelter.get("supply_preservation", 0)) > 0:
		supply_phrase = "他带着整理好的物资"
	match ending_state:
		"collapsed":
			return "林行没能稳定抵达保护区大门。最后的记忆是血月下翻倒的汽车、越来越近的低吼、以及一条再也走不完的路。\n\n祁眠日志揭示：那一夜尸群中藏着改变路线的人——不是为了林行，却间接护送了无数幸存者穿过东线。"
		"barely_reached_gate":
			var car_narrative := CarSys.evacuation_narrative(state)
			return "%s\n\n林行勉强到达保护区大门外，%s通过初筛。\n他被领到 3 号隔离棚，裹着薄毯坐在折叠床上。透过棚子的塑料窗能看见探照灯扫过铁丝网。\n\n筛查棚外有人低声说——「昨晚那股尸群像被人牵走了。」\n林行想起桥洞营地的老太太说的话、家门口的匿名药品、超市被精准拿走的食物。\n他没有开口问。他只是在日记最后一页写：\n\n「那个人是谁。那个骑摩托的。桥是空的。药放在门口。我不知道。但我欠他。」\n\n翻过一页，夹着童年避难计划的那张泛黄纸。三个人的笔迹还在上面。" % [car_narrative, supply_phrase]
		_:
			var car_narrative := CarSys.evacuation_narrative(state)
			return "%s\n\n林行抵达保护区大门外，%s通过初筛后被要求隔离观察 48 小时。\n他走进 3 号棚，裹着薄毯坐在折叠床上。\n\n玩家随后看到祁眠藏在尸群中改变路线的完整日志——\n这不是为了林行，却间接救下了他。\n\n祁眠的每一步行动被逐帧回放：\n醒来→取药→超市夜行→骑摩托清桥→红潮夜巡逻→尸群藏身。\n那些被拿走的药品、改道的尸群、留下的箭头——都是同一双手。\n\n最后一行祁眠的日志写着：\n「那个人是谁——往大门走的那个。他看起来走了很久。」" % [car_narrative, supply_phrase]

static func _qimian_blood_moon_support(day: int) -> int:
	var support := 0
	if not QimianPlan.PLAN.has(day):
		return support
	for action in QimianPlan.PLAN[day]:
		support += int(action.get("blood_moon_support", 0))
	return support
