extends SceneTree

const Simulation = preload("res://scripts/core/game_simulation.gd")

var failures: Array[String] = []

func _initialize() -> void:
	_test_initial_goal_and_stats()
	_test_lin_condition_text_reports_infection_stage()
	_test_resources_and_evacuation_flags_match_latest_scope()
	_test_shelter_facilities_exist_with_core_roles()
	_test_day_event_table_and_morning_context()
	_test_blood_moon_days()
	_test_blood_moon_warnings_are_visible_before_night()
	_test_morning_pressure_applies_once()
	_test_exploration_marks_location_and_reports_risk()
	_test_locations_expose_node_map_metadata()
	_test_location_card_text_surfaces_node_map_metadata()
	_test_exploration_can_reveal_evacuation_address_from_map_nodes()
	_test_bad_road_conditions_increase_fatigue_deterministically()
	_test_qimian_actions_mark_affected_map_nodes()
	_test_locations_expose_indoor_search_rooms()
	_test_enter_location_starts_indoor_search_phase()
	_test_room_card_text_reports_dark_risk_and_lure_state()
	_test_search_room_collects_resources_and_leave_advances_to_evening()
	_test_dark_hidden_zombie_room_causes_deterministic_injury()
	_test_noise_lure_reduces_hidden_zombie_injury_but_spends_time()
	_test_overstaying_indoor_search_adds_fatigue_on_leave()
	_test_high_infection_risk_worsens_at_night()
	_test_treat_wound_spends_medicine_and_reduces_infection()
	_test_treat_wound_without_medicine_does_not_change_condition()
	_test_facility_actions_drive_recovery_and_evacuation_readiness()
	_test_storage_table_preserves_supplies_for_escape()
	_test_qimian_wakes_on_day_five_and_uses_default_card()
	_test_demo_reveal_contains_hidden_causality()
	_test_day_fifteen_assigns_ending_state()
	_test_damaged_low_resource_run_can_collapse()
	_report()

func _test_initial_goal_and_stats() -> void:
	var sim = Simulation.new()
	var state = sim.new_game()
	_expect_equal(state.day, 1, "new game starts on day 1")
	_expect_equal(state.goal, "撤离到保护区", "Lin Xing's main goal is evacuation")
	_expect_true(state.lin is Dictionary, "state uses Lin Xing as the playable character key")
	_expect_true(not ("chen" in state), "state no longer exposes the old Chen Xing key")
	_expect_true(state.resources.food > 0, "player starts with food")
	_expect_true(state.shelter.door > 0, "shelter starts with a usable door")
	_expect_equal(state.qimian.awake, false, "Qimian starts asleep")
	_expect_equal(state.ending_state, "in_progress", "new game starts without an ending")
	_expect_true(state.morning_context.text.length() > 0, "new game has a morning context")
	_expect_true(state.last_event.find("林行") >= 0, "opening text names Lin Xing")
	_expect_true(state.last_event.find("家") >= 0, "opening starts from home")

func _test_lin_condition_text_reports_infection_stage() -> void:
	var sim = Simulation.new()
	sim.new_game()
	var initial_text: String = sim.get_lin_condition_text()
	_expect_true(initial_text.find("感染风险：低") >= 0, "new game reports low infection risk")
	sim.state.lin.infection_risk = 3
	_expect_true(sim.get_lin_condition_text().find("发热风险") >= 0, "infection risk 3 reports fever risk")
	sim.state.lin.infection_risk = 5
	_expect_true(sim.get_lin_condition_text().find("危险感染") >= 0, "infection risk 5 reports dangerous infection")

func _test_resources_and_evacuation_flags_match_latest_scope() -> void:
	var sim = Simulation.new()
	var state = sim.new_game()
	_expect_true(state.resources.has("fuel"), "fuel is a core resource")
	_expect_true(not state.resources.has("batteries"), "batteries are no longer a core resource")
	_expect_true(not state.resources.has("intel"), "intel is no longer a stackable resource")
	_expect_true("evacuation" in state, "evacuation readiness flags exist")
	_expect_equal(state.evacuation.safezone_confirmed, false, "safe zone starts unconfirmed")
	_expect_equal(state.evacuation.address_known, false, "safe zone address starts unknown")
	_expect_equal(state.evacuation.bike_ready, false, "bike starts not ready for the gate run")

func _test_shelter_facilities_exist_with_core_roles() -> void:
	var sim = Simulation.new()
	var state = sim.new_game()
	_expect_true(state.shelter.facilities.has("bed"), "bed facility exists")
	_expect_true(state.shelter.facilities.has("workbench"), "workbench facility exists")
	_expect_true(state.shelter.facilities.has("barricade"), "window barricade facility exists")
	_expect_true(state.shelter.facilities.has("radio"), "radio facility exists")
	_expect_true(state.shelter.facilities.has("storage"), "storage table facility exists")
	_expect_equal(state.shelter.facilities.bed.role, "recover", "bed recovers fatigue and stress")
	_expect_equal(state.shelter.facilities.workbench.role, "craft_repair", "workbench handles bike and tools")
	_expect_equal(state.shelter.facilities.barricade.role, "blood_moon_defense", "barricade handles blood moon defense")
	_expect_equal(state.shelter.facilities.radio.role, "broadcast_clues", "radio handles broadcast clues")
	_expect_equal(state.shelter.facilities.storage.role, "preserve_carry", "storage preserves supplies")

func _test_day_event_table_and_morning_context() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for day in range(1, 16):
		var event: Dictionary = sim.get_day_event(day)
		_expect_equal(event.day, day, "day event table includes day %d" % day)
		_expect_true(event.morning_text.length() > 0, "day %d has morning text" % day)
		_expect_true(event.pressure_type.length() > 0, "day %d has pressure type" % day)
		_expect_true(event.clue.length() > 0, "day %d has clue text" % day)

func _test_blood_moon_days() -> void:
	var sim = Simulation.new()
	sim.new_game()
	_expect_equal(sim.is_blood_moon_day(7), true, "day 7 is a blood moon")
	_expect_equal(sim.is_blood_moon_day(15), true, "day 15 is a blood moon")
	_expect_equal(sim.is_blood_moon_day(14), false, "day 14 is a red-tide night, not a blood moon")
	_expect_equal(sim.is_blood_moon_day(10), false, "day 10 is not a blood moon")

func _test_blood_moon_warnings_are_visible_before_night() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.start_day(7)
	_expect_true(sim.state.morning_context.blood_moon_warning.length() > 0, "day 7 exposes blood moon warning")
	sim.start_day(15)
	_expect_true(sim.state.morning_context.blood_moon_warning.length() > 0, "day 15 exposes blood moon warning")

func _test_morning_pressure_applies_once() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.start_day(5)
	var first_stress: int = sim.state.lin.stress
	var first_water: int = sim.state.resources.water
	sim.start_day(5)
	_expect_equal(sim.state.lin.stress, first_stress, "day pressure does not stack when morning refreshes")
	_expect_equal(sim.state.resources.water, first_water, "day resource pressure does not stack when morning refreshes")

func _test_exploration_marks_location_and_reports_risk() -> void:
	var sim = Simulation.new()
	sim.new_game()
	var label_before: String = sim.get_location_label("convenience")
	_expect_true(label_before.find("未搜") >= 0, "unvisited locations are labelled")
	var result: String = sim.explore("convenience")
	var label_after: String = sim.get_location_label("convenience")
	_expect_equal(sim.state.locations.convenience.visited, true, "exploring marks location visited")
	_expect_equal(sim.state.phase, "evening", "exploring advances to evening")
	_expect_true(result.find("风险") >= 0, "exploration reports deterministic risk text")
	_expect_true(label_after.find("已搜") >= 0, "visited locations are labelled")

func _test_locations_expose_node_map_metadata() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for location_id in sim.get_location_ids():
		var location: Dictionary = sim.state.locations[location_id]
		_expect_true(location.has("resource_tendency"), "%s has resource tendency metadata" % location_id)
		_expect_true(location.has("danger_level"), "%s has danger level metadata" % location_id)
		_expect_true(location.has("route_time"), "%s has route time metadata" % location_id)
		_expect_true(location.has("road_condition"), "%s has road condition metadata" % location_id)
		_expect_true(location.has("icons"), "%s has map icon metadata" % location_id)
		_expect_true(str(location.get("resource_tendency", "")).length() > 0, "%s resource tendency is readable" % location_id)
		_expect_true(str(location.get("danger_level", "")).length() > 0, "%s danger level is readable" % location_id)
		_expect_true(int(location.get("route_time", 0)) > 0, "%s route time is positive" % location_id)
		_expect_true(str(location.get("road_condition", "")).length() > 0, "%s road condition is readable" % location_id)
		_expect_true(location.get("icons", []).size() > 0, "%s exposes at least one map icon" % location_id)

func _test_location_card_text_surfaces_node_map_metadata() -> void:
	var sim = Simulation.new()
	sim.new_game()
	var card_text: String = sim.get_location_card_text("police")
	_expect_true(card_text.find("资源倾向") >= 0, "location card shows resource tendency")
	_expect_true(card_text.find("危险等级") >= 0, "location card shows danger level")
	_expect_true(card_text.find("路况") >= 0, "location card shows road condition")
	_expect_true(card_text.find("小时") >= 0, "location card shows route time in hours")
	_expect_true(card_text.find("地点特征") >= 0, "location card shows map icon descriptions")
	_expect_true(card_text.find("燃料") >= 0, "police card shows its resource tendency")
	_expect_true(card_text.find("路障") >= 0, "police card shows blocked road condition")

func _test_exploration_can_reveal_evacuation_address_from_map_nodes() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.bike.range = 2
	_expect_equal(sim.state.evacuation.address_known, false, "safe-zone address starts unknown")
	var result: String = sim.explore("police")
	_expect_equal(sim.state.evacuation.address_known, true, "police map node can reveal the safe-zone address")
	_expect_true(result.find("地址") >= 0, "exploration result calls out the address clue")

func _test_bad_road_conditions_increase_fatigue_deterministically() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.bike.range = 2
	var starting_fatigue: int = sim.state.lin.fatigue
	var result: String = sim.explore("bike_shop")
	_expect_true(sim.state.lin.fatigue >= starting_fatigue + 2, "blocked road adds deterministic fatigue beyond route distance")
	_expect_true(result.find("路况") >= 0, "exploration result reports road-condition pressure")

func _test_qimian_actions_mark_affected_map_nodes() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.resolve_qimian_for_day(6)
	_expect_equal(sim.state.locations.clinic.qimian_trace, true, "Qimian's clinic action marks the clinic node")
	_expect_true(sim.state.locations.clinic.icons.has("qimian"), "Qimian trace adds a map icon")
	_expect_true(sim.get_location_card_text("clinic").find("祁眠异常") >= 0, "location card surfaces Qimian trace")

func _test_locations_expose_indoor_search_rooms() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for location_id in ["convenience", "clinic", "supermarket"]:
		var location: Dictionary = sim.state.locations[location_id]
		_expect_true(location.has("rooms"), "%s exposes indoor search rooms" % location_id)
		_expect_true(location.rooms.size() >= 2, "%s has at least two room choices" % location_id)
		for room_id in location.rooms.keys():
			var room: Dictionary = location.rooms[room_id]
			_expect_true(room.has("name"), "%s/%s has a readable name" % [location_id, room_id])
			_expect_true(room.has("visibility"), "%s/%s has visibility metadata" % [location_id, room_id])
			_expect_true(room.has("search_time"), "%s/%s has deterministic search time" % [location_id, room_id])
			_expect_true(room.has("resources"), "%s/%s has deterministic room resources" % [location_id, room_id])

func _test_enter_location_starts_indoor_search_phase() -> void:
	var sim = Simulation.new()
	sim.new_game()
	var result: String = sim.enter_location("convenience")
	_expect_equal(sim.state.phase, "searching", "entering a location starts indoor searching")
	_expect_equal(sim.state.exploration.active_location, "convenience", "active exploration records the selected node")
	_expect_true(sim.state.exploration.time_limit > 0, "indoor search has a time limit")
	_expect_true(result.find("进入") >= 0, "enter location reports entry text")
	_expect_true(sim.get_room_card_text("storefront").find("能见度") >= 0, "room card surfaces visibility")

func _test_room_card_text_reports_dark_risk_and_lure_state() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.enter_location("clinic")
	var before_lure: String = sim.get_room_card_text("pharmacy")
	_expect_true(before_lure.find("黑暗") >= 0, "dark room card reports shadow risk")
	_expect_true(before_lure.find("未排除") >= 0, "room card reports hidden threat not yet cleared")
	sim.lure_room("pharmacy")
	var after_lure: String = sim.get_room_card_text("pharmacy")
	_expect_true(after_lure.find("已引开") >= 0, "room card reports lured hidden threat")

func _test_search_room_collects_resources_and_leave_advances_to_evening() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.enter_location("convenience")
	var starting_food: int = sim.state.resources.food
	var result: String = sim.search_room("storefront", "careful")
	_expect_true(sim.state.resources.food > starting_food, "searching a stocked room collects deterministic resources")
	_expect_equal(sim.state.locations.convenience.rooms.storefront.searched, true, "searched room is marked")
	_expect_true(result.find("带回") >= 0, "room search reports resource pickup")
	sim.leave_exploration()
	_expect_equal(sim.state.phase, "evening", "leaving indoor search advances to evening")
	_expect_equal(sim.state.locations.convenience.visited, true, "leaving marks the location visited")
	_expect_equal(sim.state.exploration.active_location, "", "leaving clears active exploration")

func _test_dark_hidden_zombie_room_causes_deterministic_injury() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.enter_location("clinic")
	var starting_health: int = sim.state.lin.health
	var starting_infection: int = sim.state.lin.infection_risk
	var result: String = sim.search_room("pharmacy", "quick")
	_expect_true(sim.state.lin.health < starting_health, "rushing a dark hidden-zombie room hurts Lin Xing")
	_expect_true(sim.state.lin.infection_risk > starting_infection, "hidden-zombie contact raises infection risk")
	_expect_true(result.find("隐藏尸群") >= 0, "search result reports hidden-zombie contact")

func _test_noise_lure_reduces_hidden_zombie_injury_but_spends_time() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.enter_location("clinic")
	var starting_health: int = sim.state.lin.health
	var lure_result: String = sim.lure_room("pharmacy")
	var search_result: String = sim.search_room("pharmacy", "careful")
	_expect_equal(sim.state.lin.health, starting_health, "luring before search avoids direct injury")
	_expect_true(sim.state.exploration.time_used >= 2, "luring and searching spend time")
	_expect_true(sim.state.exploration.noise > 0, "luring raises local noise")
	_expect_true(lure_result.find("制造噪音") >= 0, "lure action reports noise")
	_expect_true(search_result.find("已被引开") >= 0, "search reports the threat was lured")

func _test_overstaying_indoor_search_adds_fatigue_on_leave() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.bike.range = 2
	sim.enter_location("supermarket")
	var starting_fatigue: int = sim.state.lin.fatigue
	sim.search_room("checkout", "careful")
	sim.lure_room("storage")
	sim.search_room("storage", "careful")
	sim.lure_room("food_aisle")
	var result: String = sim.leave_exploration()
	_expect_true(sim.state.lin.fatigue > starting_fatigue, "overstaying indoor search adds fatigue")
	_expect_true(result.find("天色") >= 0, "leaving after overstaying reports time pressure")

func _test_high_infection_risk_worsens_at_night() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.phase = "evening"
	sim.state.lin.infection_risk = 5
	var starting_health: int = sim.state.lin.health
	var starting_stress: int = sim.state.lin.stress
	var result: String = sim.sleep_and_resolve_night()
	_expect_true(sim.state.lin.health < starting_health, "dangerous infection costs health during night resolution")
	_expect_true(sim.state.lin.stress > starting_stress, "dangerous infection raises stress during night resolution")
	_expect_true(result.find("感染") >= 0, "night result reports infection pressure")

func _test_treat_wound_spends_medicine_and_reduces_infection() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.phase = "evening"
	sim.state.lin.health = 7
	sim.state.lin.infection_risk = 4
	sim.state.resources.meds = 2
	var result: String = sim.perform_shelter_action("treat_wound")
	_expect_equal(sim.state.resources.meds, 1, "treat wound spends one medicine")
	_expect_equal(sim.state.lin.health, 8, "treat wound restores one health")
	_expect_equal(sim.state.lin.infection_risk, 3, "treat wound reduces infection risk")
	_expect_true(result.find("处理伤口") >= 0, "treat wound reports wound treatment")

func _test_treat_wound_without_medicine_does_not_change_condition() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.phase = "evening"
	sim.state.lin.health = 6
	sim.state.lin.infection_risk = 3
	sim.state.resources.meds = 0
	var result: String = sim.perform_shelter_action("treat_wound")
	_expect_equal(sim.state.lin.health, 6, "failed wound treatment does not change health")
	_expect_equal(sim.state.lin.infection_risk, 3, "failed wound treatment does not change infection risk")
	_expect_true(result.find("没有药品") >= 0, "failed wound treatment reports missing medicine")

func _test_facility_actions_drive_recovery_and_evacuation_readiness() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.phase = "evening"
	sim.state.lin.fatigue = 5
	sim.state.lin.stress = 5
	sim.perform_shelter_action("rest_bed")
	_expect_true(sim.state.lin.fatigue < 5, "bed reduces fatigue")
	_expect_true(sim.state.lin.stress < 5, "bed reduces stress")
	_expect_equal(sim.state.shelter.facilities.bed.used_today, true, "bed records daily use")

	sim.state.phase = "evening"
	var starting_parts: int = sim.state.resources.parts
	sim.perform_shelter_action("workbench_repair")
	_expect_true(sim.state.resources.parts < starting_parts, "workbench repair spends parts")
	_expect_true(sim.state.bike.range >= 2, "workbench repair improves bike range")

	sim.state.phase = "evening"
	sim.state.resources.materials = 4
	var starting_defense: int = sim.state.shelter.defense
	sim.perform_shelter_action("barricade_windows")
	_expect_true(sim.state.shelter.defense > starting_defense, "barricade improves defense")
	_expect_true(sim.state.shelter.facilities.barricade.level >= 2, "barricade facility level improves")

	sim.state.phase = "evening"
	sim.state.day = 9
	sim.state.resources.fuel = 2
	sim.perform_shelter_action("radio_broadcast")
	_expect_equal(sim.state.evacuation.safezone_confirmed, true, "radio confirms safe zone")
	_expect_equal(sim.state.evacuation.address_known, true, "radio can reveal screening gate address")

func _test_storage_table_preserves_supplies_for_escape() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.state.phase = "evening"
	sim.state.resources.materials = 3
	sim.perform_shelter_action("organize_storage")
	_expect_equal(sim.state.shelter.facilities.storage.used_today, true, "storage table records daily use")
	_expect_equal(sim.state.shelter.supply_preservation, 1, "storage improves supply preservation")
	sim.state.day = 15
	sim.state.resources.food = 1
	sim.state.resources.water = 1
	sim.state.evacuation.safezone_confirmed = true
	sim.state.evacuation.address_known = true
	sim.state.evacuation.bike_ready = true
	sim.sleep_and_resolve_night()
	_expect_true(sim.state.reveal.summary.find("带着整理好的物资") >= 0, "ending mentions organized supplies")

func _test_qimian_wakes_on_day_five_and_uses_default_card() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.resolve_qimian_for_day(4)
	_expect_equal(sim.state.qimian.awake, false, "Qimian is still asleep on day 4")
	_expect_equal(sim.state.qimian.log.size(), 0, "Qimian has no action log before waking")
	sim.resolve_qimian_for_day(5)
	_expect_equal(sim.state.qimian.awake, true, "Qimian wakes on day 5")
	_expect_equal(sim.state.qimian.personality_card.main_goal, "寻找祁烬", "demo uses fixed Qimian goal card")
	_expect_true(sim.state.qimian.log.size() >= 1, "Qimian logs an action on day 5")
	_expect_true(sim.state.qimian.log[0].has("ai_replay"), "Qimian log includes AI action replay")
	_expect_true(sim.state.qimian.log[0].has("subjective_fragment"), "Qimian log includes subjective fragment")

func _test_demo_reveal_contains_hidden_causality() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for day in range(1, 16):
		sim.play_safe_demo_day(day)
	_expect_equal(sim.state.demo_complete, true, "demo completes after day 15 resolves")
	_expect_true(sim.state.reveal.unlocked, "Qimian log reveal unlocks at demo end")
	_expect_true(sim.state.qimian.log.size() >= 5, "reveal has at least five hidden-causality log entries")
	_expect_true(sim.state.blood_moons_resolved.has(7), "first blood moon is resolved")
	_expect_true(sim.state.blood_moons_resolved.has(15), "second blood moon is resolved")
	_expect_true(["reached_gate_quarantine", "barely_reached_gate", "collapsed"].has(sim.state.ending_state), "demo assigns a valid ending state")
	_expect_true(sim.state.reveal.summary.find("保护区") >= 0, "reveal mentions the safe zone gate")
	_expect_true(sim.state.reveal.summary.find("隔离") >= 0, "reveal mentions quarantine or isolation")
	_expect_true(sim.state.reveal.summary.find("尸群") >= 0, "reveal mentions the zombie group near miss")

func _test_day_fifteen_assigns_ending_state() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for day in range(1, 16):
		sim.play_safe_demo_day(day)
	_expect_equal(sim.state.phase, "reveal", "day 15 ends in reveal phase")
	_expect_true(["reached_gate_quarantine", "barely_reached_gate"].has(sim.state.ending_state), "safe demo route reaches or barely reaches the screening gate")
	_expect_true(sim.state.reveal.summary.find("初筛") >= 0 or sim.state.reveal.summary.find("隔离观察") >= 0, "ending summary reflects screening or quarantine")

func _test_damaged_low_resource_run_can_collapse() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.start_day(15)
	sim.state.resources.food = 0
	sim.state.resources.water = 0
	sim.state.lin.health = 1
	sim.state.lin.hunger = 3
	sim.state.lin.thirst = 3
	sim.state.lin.stress = 9
	sim.state.shelter.door = 0
	sim.state.shelter.defense = 0
	sim.state.shelter.noise = 4
	sim.state.shelter.scent = 4
	sim.state.shelter.light = 4
	sim.sleep_and_resolve_night()
	_expect_equal(sim.state.demo_complete, true, "collapsed run still completes the demo frame")
	_expect_equal(sim.state.ending_state, "collapsed", "damaged low-resource state collapses")

func _expect_equal(actual, expected, message: String) -> void:
	if actual != expected:
		failures.append("%s: expected %s, got %s" % [message, str(expected), str(actual)])

func _expect_true(value: bool, message: String) -> void:
	if not value:
		failures.append("%s: expected true" % message)

func _report() -> void:
	if failures.is_empty():
		print("All simulation tests passed.")
		quit(0)
		return
	for failure in failures:
		printerr(failure)
	quit(1)
