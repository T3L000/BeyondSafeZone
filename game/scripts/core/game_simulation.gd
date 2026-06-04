# 游戏模拟协调器 —— Controller 层薄调度器
# 职责：流程编排 + Controller 委托 + View 文本委托
# 不改 UI，不存状态（状态在 Model/game_state.gd）
extends RefCounted

# ---- Model ----
const _GameState = preload("res://scripts/model/game_state.gd")

# ---- Data (Config) ----
const GameConsts = preload("res://scripts/data/constants.gd")
const Balance = preload("res://scripts/data/balance.gd")
const DayEvents = preload("res://scripts/data/events_15d.gd")
const LocData = preload("res://scripts/data/locations.gd")
const SafeRoute = preload("res://scripts/data/safe_route.gd")

# ---- Controllers ----
const CarSys = preload("res://scripts/controller/car_controller.gd")
const ShelterSys = preload("res://scripts/controller/shelter_controller.gd")
const NightRes = preload("res://scripts/controller/night_controller.gd")
const QimianAI = preload("res://scripts/controller/qimian_controller.gd")
const ExplorationSystem = preload("res://scripts/controller/exploration_controller.gd")

# ---- View ----
const TextRenderer = preload("res://scripts/view/text_renderer.gd")

const MAX_DEMO_DAY = GameConsts.MAX_DEMO_DAY
const FULL_DAY_LIMIT = GameConsts.FULL_DAY_LIMIT

var state: _GameState

# ============ 初始化 ============

func new_game() -> _GameState:
	state = _GameState.new()
	state.day = 1
	state.phase = "morning"
	state.goal = "撤离到保护区"
	state.demo_complete = false
	state.ending_state = "in_progress"
	state.last_event = "林行在家中醒来。收音机里反复出现保护区断续广播。"
	state.morning_context = {}
	state.applied_day_events = []
	state.resources = Balance.INIT_RESOURCES.duplicate(true)
	state.lin = Balance.INIT_LIN.duplicate(true)
	state.shelter = Balance.INIT_SHELTER.duplicate(true)
	state.shelter["facilities"] = ShelterSys.default_facilities()
	state.bike = Balance.INIT_BIKE.duplicate(true)
	state.car = Balance.INIT_CAR.duplicate(true)
	state.car_parts = Balance.INIT_CAR_PARTS.duplicate(true)
	state.qimian = {
		"awake": false, "log": [], "public_clues": [],
		"personality_card": {"main_goal": "寻找祁烬", "exposure": "谨慎，避免暴露", "moral_rule": "会救近处的人，但不承担大规模救援", "resource_rule": "只拿任务需要的资源", "safezone_attitude": "靠近观察，但不信任筛查"},
		"ai_state": Balance.INIT_QIMIAN_AI_STATE.duplicate(true)
	}
	state.evacuation = Balance.INIT_EVACUATION.duplicate(true)
	state.exploration = {"active_location": "", "time_used": 0, "time_limit": 0, "noise": 0, "searched_rooms": [], "lured_rooms": []}
	state.blood_moons_resolved = []
	state.reveal = {"unlocked": false, "summary": ""}
	state.anomaly_dossier = []
	state.player_marks = {}
	_build_locations()
	start_day(1)
	return state

# ============ 日循环 ============

func is_blood_moon_day(day: int) -> bool:
	return GameConsts.BLOOD_MOON_DAYS.has(day)

func get_day_event(day: int) -> Dictionary:
	return DayEvents.get_event(day)

func start_day(day: int) -> String:
	state.day = day
	if state.phase != "reveal":
		state.phase = "morning"
	_reset_facility_use()
	var event := get_day_event(day)
	state.morning_context = {"day": day, "text": event.morning_text, "pressure_type": event.pressure_type, "clue": event.clue, "blood_moon_warning": event.blood_moon_warning}
	if not state.applied_day_events.has(day):
		_apply_day_pressure(event)
		state.applied_day_events.append(day)
	state.last_event = "第 %d 天清晨。%s %s" % [day, event.morning_text, event.clue]
	state.last_event += TextRenderer.daily_monologue(state, day)
	return state.last_event

# ============ 查询（只读） ============

func get_location_ids() -> Array:
	return state.locations.keys()

# ============ View 文本委托 ============

func get_lin_condition_text() -> String:
	return TextRenderer.get_lin_condition_text(state)

func get_location_label(location_id: String) -> String:
	return TextRenderer.get_location_label(state, location_id)

func get_location_card_text(location_id: String) -> String:
	return TextRenderer.get_location_card_text(state, location_id)

func get_room_card_text(room_id: String) -> String:
	return TextRenderer.get_room_card_text(state, room_id)

func get_location_risk_text(location_id: String) -> String:
	return TextRenderer.get_location_risk_text(state, location_id)

# ============ 探索委托 ============

func enter_location(location_id: String) -> String:
	return ExplorationSystem.enter_location(state, location_id)

func search_room(room_id: String, tactic: String = "careful") -> String:
	return ExplorationSystem.search_room(state, room_id, tactic)

func lure_room(room_id: String) -> String:
	return ExplorationSystem.lure_room(state, room_id)

func leave_exploration() -> String:
	return ExplorationSystem.leave_exploration(state)

func explore(location_id: String) -> String:
	return ExplorationSystem.explore(state, location_id)

# ============ 据点委托 ============

func perform_shelter_action(action_id: String) -> String:
	return ShelterSys.perform_action(state, action_id)

# ============ 夜晚委托 ============

func sleep_and_resolve_night() -> String:
	resolve_qimian_for_day(int(state.day))
	return NightRes.resolve(state, start_day)

# ============ 祁眠 AI 委托 ============

func resolve_qimian_for_day(day: int) -> void:
	QimianAI.resolve_for_day(state, day)

# ============ 安全演示 ============

func play_safe_demo_day(day: int) -> void:
	start_day(day)
	var location_id := SafeRoute.get_location_for_day(day)
	_auto_search_location(location_id)
	perform_shelter_action(SafeRoute.get_action_for_day(day))
	sleep_and_resolve_night()

func _select_safe_location(day: int) -> String:
	return SafeRoute.get_location_for_day(day)

func _auto_search_location(location_id: String) -> void:
	enter_location(location_id)
	if state.phase != "searching": return
	var location: Dictionary = state.locations[location_id]
	var searched_count := 0
	for room_id in location.rooms.keys():
		if searched_count >= SafeRoute.MAX_ROOMS_PER_LOCATION: break
		var room: Dictionary = location.rooms[room_id]
		if bool(room.searched): continue
		if bool(room.get("locked", false)): continue
		if int(room.hidden_zombies) > 0: lure_room(room_id)
		search_room(room_id, SafeRoute.SEARCH_TACTIC)
		searched_count += 1
	leave_exploration()

# ============ 内部辅助 ============

func _build_locations() -> void:
	for loc_id in LocData.LOCATION_DEFS.keys():
		state.locations[loc_id] = LocData.build_location(loc_id)

func _default_facilities() -> Dictionary:
	return ShelterSys.default_facilities()

func _mark_facility_used(facility_id: String) -> void:
	ShelterSys.mark_facility_used(state, facility_id)

func _reset_facility_use() -> void:
	ShelterSys.reset_facility_use(state)

func _apply_day_pressure(event: Dictionary) -> void:
	var modifiers: Dictionary = event.modifiers
	for key in modifiers.keys():
		var amount: int = int(modifiers[key])
		match String(key):
			"food": state.resources.food = max(0, int(state.resources.food) + amount)
			"water": state.resources.water = max(0, int(state.resources.water) + amount)
			"stress": state.lin.stress = max(0, int(state.lin.stress) + amount)
			"hope": state.lin.hope = max(0, int(state.lin.hope) + amount)
			"door": state.shelter.door = max(0, int(state.shelter.door) + amount)
			"noise": state.shelter.noise = max(0, int(state.shelter.noise) + amount)
			"scent": state.shelter.scent = max(0, int(state.shelter.scent) + amount)
			"bike_durability": state.bike.durability = max(0, int(state.bike.durability) + amount)
