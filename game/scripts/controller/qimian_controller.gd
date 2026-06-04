# 祁眠AI决策引擎 —— 规则层：日程执行/感知/收集任务/排序/执行
class_name QimianAI extends RefCounted

const GameConsts = preload("res://scripts/data/constants.gd")
const Balance = preload("res://scripts/data/balance.gd")
const QimianPlan = preload("res://scripts/data/qimian_plan.gd")
const _GameState = preload("res://scripts/model/game_state.gd")

static func resolve_for_day(state: _GameState, day: int) -> void:
	if day < Balance.QIMIAN_AWAKE_DAY:
		return
	state.qimian.awake = true
	var qs: Dictionary = state.qimian.ai_state

	# Update moto_tier: upgrade per QIMIAN_MOTO_UPGRADE_DAYS
	if Balance.QIMIAN_MOTO_UPGRADE_DAYS.has(day):
		qs.moto_tier = Balance.QIMIAN_MOTO_UPGRADE_DAYS[day]
		if day == 8:
			state.qimian.log.append({"day": day, "title": "摩托升级", "truth": "祁眠在别墅找到了备用零件和工具，把摩托改装到二级——可以跑更远的路了。", "public_clue": "", "ai_replay": "摩托升级：范围扩大至中远圈。", "subjective_fragment": "引擎声音更稳了。今晚可以骑远一点。"})

	# Step 1: Scheduled tasks always run first
	if QimianPlan.PLAN.has(day):
		for action in QimianPlan.PLAN[day]:
			_apply_action(state, day, action)
			_update_ai_state(state, day, action)
		return

	# Step 2: AI decision for non-scheduled days (day 7, 9, 11, 13)
	var perceivable := _perceive(state, day)
	var candidates := _collect_tasks(state, day, perceivable)
	if candidates.is_empty():
		return
	var chosen := _rank_and_select(candidates)
	_execute(state, day, chosen, perceivable)

# --- Private helpers ---

static func _apply_action(state: _GameState, day: int, action: Dictionary) -> void:
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

static func _perceive(state: _GameState, day: int) -> Dictionary:
	var qs: Dictionary = state.qimian.ai_state
	var p := {
		"day": day,
		"moon": "normal",
		"weather": "clear",
		"available_zones": [],
		"zombie_hotspots": [],
		"survivor_in_need": false,
		"qijin_signal_active": qs.qijin_clues >= 1,
		"supply_shortage": qs.inventory.food <= 0 or qs.inventory.medicine <= 0
	}
	if GameConsts.BLOOD_MOON_DAYS.has(day):
		p.moon = "blood_moon"
	elif day >= 11:
		p.moon = "red_tide"
	# Available zones based on moto_tier and heat
	var tier: int = int(qs.moto_tier)
	if int(qs.zone_heat.A) < Balance.QIMIAN_ZONE_HEAT_MAX and qs.exposure < Balance.QIMIAN_ZONE_A_EXPOSURE_MAX:
		p.available_zones.append("A")
	if tier >= 2 and int(qs.zone_heat.B) < Balance.QIMIAN_ZONE_HEAT_MAX and qs.exposure < Balance.QIMIAN_ZONE_B_EXPOSURE_MAX:
		p.available_zones.append("B")
	if tier >= 3 and int(qs.zone_heat.C) < Balance.QIMIAN_ZONE_C_HEAT_MAX and qs.exposure < Balance.QIMIAN_ZONE_C_EXPOSURE_MAX:
		p.available_zones.append("C")
	# Detect zombie hotspots from location data
	for loc_id in state.locations.keys():
		var loc: Dictionary = state.locations[loc_id]
		if int(loc.zombies) >= 4:
			p.zombie_hotspots.append(loc_id)
	# Survivor needs (simplified: if day >= 6, assume bridge_camp needs help)
	if day >= 6:
		p.survivor_in_need = true
	return p

static func _collect_tasks(state: _GameState, day: int, p: Dictionary) -> Array:
	var tasks := []
	var qs: Dictionary = state.qimian.ai_state
	# Patrol: always available
	if not p.available_zones.is_empty():
		tasks.append({"id": "patrol", "type": "routine", "zone": p.available_zones[0], "priority": 10})
	# Scavenge: if supply shortage
	if p.supply_shortage and not p.available_zones.is_empty():
		tasks.append({"id": "scavenge", "type": "routine", "zone": p.available_zones[0], "location": "supermarket", "priority": 50})
	# Supply drop: if survivor in need
	if p.survivor_in_need and not p.available_zones.is_empty():
		tasks.append({"id": "supply_drop", "type": "routine", "zone": "A", "priority": 80, "location": "bridge_camp"})
	# Track Qijin: if signal active and has moto_tier >= 2
	if p.qijin_signal_active and "B" in p.available_zones and qs.qijin_clues < 3:
		tasks.append({"id": "track_qijin", "type": "event", "zone": "B", "priority": 100})
	# Rest: if exposure high or no good tasks
	if qs.exposure >= 5 or tasks.is_empty():
		tasks.append({"id": "rest", "type": "routine", "zone": "hideout", "priority": 0})
	return tasks

static func _rank_and_select(candidates: Array) -> Dictionary:
	if candidates.is_empty():
		return {}
	candidates.sort_custom(func(a, b): return a.priority > b.priority)
	return candidates[0]

static func _execute(state: _GameState, day: int, task: Dictionary, p: Dictionary) -> void:
	var qs: Dictionary = state.qimian.ai_state
	match task.id:
		"patrol":
			qs.exposure = min(Balance.QIMIAN_EXPOSURE_MAX, int(qs.exposure) + Balance.QIMIAN_PATROL_EXPOSURE)
			state.qimian.log.append({
				"day": day, "title": "夜间巡逻", "truth": "祁眠骑摩托在%s区巡逻，标记安全路线。" % task.zone,
				"public_clue": "", "ai_replay": "任务：巡逻。区域：%s。暴露+1。" % task.zone,
				"subjective_fragment": "安静的一夜。至少这个方向还安全。"
			})
		"scavenge":
			qs.inventory.food += Balance.QIMIAN_SCAVENGE_FOOD
			qs.inventory.medicine += Balance.QIMIAN_SCAVENGE_MEDICINE
			qs.exposure = min(Balance.QIMIAN_EXPOSURE_MAX, int(qs.exposure) + Balance.QIMIAN_SCAVENGE_EXPOSURE)
			state.qimian.log.append({
				"day": day, "title": "夜间搜刮", "truth": "祁眠在%s区搜刮补给，拿了食物和药品。" % task.zone,
				"public_clue": "超市后门被人从里面用铁丝重新别上——上次来的时候不是这样的。",
				"ai_replay": "任务：搜刮。目标：超市。获得食物+1药品+1。暴露+1。",
				"subjective_fragment": "只拿够用的。剩下的——有人比我更需要。"
			})
		"supply_drop":
			qs.exposure = max(0, int(qs.exposure) + Balance.QIMIAN_DROP_EXPOSURE)
			state.qimian.public_clues.append("桥洞营地外多了一包绷带和水——放在不会被雨淋到的位置。")
			state.qimian.log.append({
				"day": day, "title": "匿名补给", "truth": "祁眠骑车经过桥洞营地，看到老太太和发烧的小女孩，把绷带和水放在营地外围。",
				"public_clue": "桥洞营地外多了一包绷带和水。", "ai_replay": "任务：匿名补给。目标：桥洞营地。暴露-1（善意行为未暴露身份）。",
				"subjective_fragment": "那个小女孩烧退了。我只是放了东西在那里——不是我治好的。"
			})
		"track_qijin":
			qs.qijin_clues += 1
			qs.exposure = min(Balance.QIMIAN_EXPOSURE_MAX, int(qs.exposure) + Balance.QIMIAN_TRACK_EXPOSURE)
			state.qimian.log.append({
				"day": day, "title": "追踪祁烬信号", "truth": "祁眠追踪返生计划加密频段，找到了祁烬最近活动过的地点——桌上水还是热的。",
				"public_clue": "",
				"ai_replay": "任务：追踪祁烬。区域：%s。祁烬线索+%d。暴露+2。" % [task.zone, qs.qijin_clues],
				"subjective_fragment": "水还是热的。他十分钟前还在这里。"
			})
		"rest":
			qs.exposure = max(0, int(qs.exposure) + Balance.QIMIAN_REST_EXPOSURE)
			state.qimian.log.append({
				"day": day, "title": "在别墅休整", "truth": "祁眠今晚没有外出，在别墅补睡和整理线索。",
				"public_clue": "", "ai_replay": "任务：休整。暴露-2。", "subjective_fragment": "今晚没有任务。睡了一觉——很久没有睡这么死了。"
			})
	# Update zone heat after action
	var zone_key: String = task.get("zone", "")
	if zone_key in ["A", "B", "C"]:
		qs.zone_heat[zone_key] = min(Balance.QIMIAN_ZONE_HEAT_MAX, int(qs.zone_heat[zone_key]) + 1)

static func _update_ai_state(state: _GameState, day: int, action: Dictionary) -> void:
	var qs: Dictionary = state.qimian.ai_state
	var title: String = action.get("title", "")
	if "诊" in title or "取药" in title or "超市" in title or "尸群" in title:
		qs.exposure = min(Balance.QIMIAN_EXPOSURE_MAX, int(qs.exposure) + 1)
	if "清桥" in title:
		qs.exposure = min(Balance.QIMIAN_EXPOSURE_MAX, int(qs.exposure) + 2)
		qs.zone_heat.B = min(Balance.QIMIAN_ZONE_HEAT_MAX, int(qs.zone_heat.B) + 1)
	if "观察" in title:
		qs.exposure = max(0, int(qs.exposure) - 1)
	if "藏身" in title:
		qs.exposure = min(Balance.QIMIAN_EXPOSURE_MAX, int(qs.exposure) + 2)
		qs.zone_heat.C = min(Balance.QIMIAN_ZONE_HEAT_MAX, int(qs.zone_heat.C) + 1)
