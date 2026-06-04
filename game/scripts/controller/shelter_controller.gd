# 据点设施系统 —— 规则层：设施使用/建造/修理/广播
class_name ShelterSystem extends RefCounted

const FacData = preload("res://scripts/data/facilities.gd")
const Balance = preload("res://scripts/data/balance.gd")
const CarSys = preload("res://scripts/controller/car_controller.gd")
const _GameState = preload("res://scripts/model/game_state.gd")

static func perform_action(state: _GameState, action_id: String) -> String:
	match action_id:
		"rest_bed":
			state.lin.fatigue = max(0, int(state.lin.fatigue) - Balance.SHELTER_REST_FATIGUE)
			state.lin.stress = max(0, int(state.lin.stress) - Balance.SHELTER_REST_STRESS)
			_mark_facility_used(state, "bed")
			state.last_event = "林行在床铺上断续睡了一会儿，疲劳和压力都降下来一点。"
		"workbench_repair":
			if _spend(state, "parts", Balance.SHELTER_REPAIR_BIKE_PARTS):
				state.bike.durability += Balance.SHELTER_REPAIR_BIKE_DURABILITY
				state.bike.range = min(Balance.SHELTER_REPAIR_BIKE_MAX_RANGE, int(state.bike.range) + Balance.SHELTER_REPAIR_BIKE_RANGE)
				state.bike.noise = max(0, int(state.bike.noise) - Balance.SHELTER_REPAIR_BIKE_NOISE)
				_mark_facility_used(state, "workbench")
				if int(state.bike.range) >= Balance.SHELTER_REPAIR_BIKE_MAX_RANGE:
					state.evacuation.bike_ready = true
				state.last_event = "林行在工作台修好车链和刹车，自行车更适合远行。"
			else:
				state.last_event = "没有足够零件，工作台只能摆着拆开的工具。"
		"barricade_windows":
			if _spend(state, "materials", Balance.SHELTER_BARRICADE_MATERIALS):
				state.shelter.door += Balance.SHELTER_BARRICADE_DOOR
				state.shelter.defense += Balance.SHELTER_BARRICADE_DEFENSE
				state.shelter.facilities.barricade.level += 1
				_mark_facility_used(state, "barricade")
				state.last_event = "林行把窗框和门缝重新钉死，血月前的防线厚了一层。"
			else:
				state.last_event = "建材不足，封窗只能停在一半。"
		"radio_broadcast":
			if _spend(state, "fuel", Balance.SHELTER_RADIO_FUEL):
				state.lin.hope += Balance.SHELTER_RADIO_HOPE
				state.shelter.noise += Balance.SHELTER_RADIO_NOISE
				_mark_facility_used(state, "radio")
				if int(state.day) >= 3:
					state.evacuation.safezone_confirmed = true
				if int(state.day) >= 9:
					state.evacuation.address_known = true
				state.last_event = _radio_message_for_day(int(state.day))
			else:
				state.last_event = "发电机没有燃料，收音机只剩沙沙声。"
		"organize_storage":
			state.shelter.supply_preservation = min(Balance.SHELTER_STORAGE_MAX_PRESERVATION, int(state.shelter.supply_preservation) + Balance.SHELTER_STORAGE_PRESERVATION)
			state.bike.capacity += Balance.SHELTER_STORAGE_CAPACITY
			_mark_facility_used(state, "storage")
			state.last_event = "林行把食物、水和路上要带的东西重新打包，撤离时能少丢一些。"
		"treat_wound":
			if _spend(state, "meds", Balance.SHELTER_TREAT_MEDS):
				state.lin.health = min(10, int(state.lin.health) + Balance.SHELTER_TREAT_HEALTH)
				state.lin.infection_risk = max(0, int(state.lin.infection_risk) - Balance.SHELTER_TREAT_INFECTION)
				state.last_event = "林行用药品处理伤口，体温稍微压下去，感染风险降低。"
			else:
				state.last_event = "没有药品，林行只能用清水压住伤口。"
		"workbench_car":
			return CarSys.repair(state)
		"fortify":
			if _spend(state, "materials", Balance.SHELTER_FORTIFY_MATERIALS):
				state.shelter.door += Balance.SHELTER_FORTIFY_DOOR
				state.shelter.defense += Balance.SHELTER_FORTIFY_DEFENSE
				state.last_event = "林行用木板和铁丝加固门窗。"
			else:
				state.last_event = "建材不足，无法加固。"
		"quiet":
			state.shelter.noise = max(0, int(state.shelter.noise) - Balance.SHELTER_QUIET_NOISE)
			state.lin.stress += Balance.SHELTER_QUIET_STRESS
			state.last_event = "林行拆掉会响的杂物，据点安静了一些。"
		"mask_scent":
			if _spend(state, "materials", Balance.SHELTER_MASK_MATERIALS):
				state.shelter.scent = max(0, int(state.shelter.scent) - Balance.SHELTER_MASK_SCENT)
				state.last_event = "林行封住垃圾和血腥味，降低尸群注意。"
			else:
				state.last_event = "缺少布料和胶带，气味遮蔽失败。"
		"repair_bike":
			return perform_action(state, "workbench_repair")
		"radio":
			return perform_action(state, "radio_broadcast")
		_:
			state.last_event = "林行什么也没来得及做。"
	state.phase = "night"
	return state.last_event

static func default_facilities() -> Dictionary:
	return FacData.defaults()

static func mark_facility_used(state: _GameState, facility_id: String) -> void:
	_mark_facility_used(state, facility_id)

static func reset_facility_use(state: _GameState) -> void:
	for facility_id in state.shelter.facilities.keys():
		state.shelter.facilities[facility_id].used_today = false

# --- Private helpers ---

static func _mark_facility_used(state: _GameState, facility_id: String) -> void:
	if state.shelter.facilities.has(facility_id):
		state.shelter.facilities[facility_id].used_today = true

static func _spend(state: _GameState, resource_name: String, amount: int) -> bool:
	if int(state.resources.get(resource_name, 0)) < amount:
		return false
	state.resources[resource_name] = int(state.resources[resource_name]) - amount
	return true

static func _radio_message_for_day(day: int) -> String:
	if day >= 14:
		return "紧急广播：超大型尸潮逼近，保护区临时开放外圈接收窗口。所有外围幸存者，这是最后撤离机会。"
	if day >= 11:
		return "收音机警告：红潮区域扩大，保护区外围筛查站已加固。"
	if day >= 9:
		return "广播短暂说清保护区外圈筛查棚地址，但提醒所有人必须接受感染初筛。"
	if day >= 5:
		return "广播夹杂着陌生敲击声，有人正在保护区外转移幸存者。"
	return "断续广播提到保护区仍在接收幸存者，但外围路线已经封锁。"
