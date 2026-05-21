extends SceneTree

const Simulation = preload("res://scripts/core/game_simulation.gd")

var failures: Array[String] = []

func _initialize() -> void:
	_test_initial_goal_and_stats()
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
	_test_facility_actions_drive_recovery_and_evacuation_readiness()
	_test_storage_table_preserves_supplies_for_escape()
	_test_qimian_wakes_on_day_five_and_uses_default_card()
	_test_demo_reveal_contains_hidden_causality()
	_test_day_fourteen_assigns_ending_state()
	_test_damaged_low_resource_run_can_collapse()
	_report()

func _test_initial_goal_and_stats() -> void:
	var sim = Simulation.new()
	var state = sim.new_game()
	_expect_equal(state.day, 1, "new game starts on day 1")
	_expect_equal(state.goal, "撤离到保护区", "Lin Xing's main goal is evacuation")
	_expect_true(state.has("lin"), "state uses Lin Xing as the playable character key")
	_expect_true(not state.has("chen"), "state no longer exposes the old Chen Xing key")
	_expect_true(state.resources.food > 0, "player starts with food")
	_expect_true(state.shelter.door > 0, "shelter starts with a usable door")
	_expect_equal(state.qimian.awake, false, "Qimian starts asleep")
	_expect_equal(state.ending_state, "in_progress", "new game starts without an ending")
	_expect_true(state.morning_context.text.length() > 0, "new game has a morning context")
	_expect_true(state.last_event.find("林行") >= 0, "opening text names Lin Xing")
	_expect_true(state.last_event.find("家") >= 0, "opening starts from home")

func _test_resources_and_evacuation_flags_match_latest_scope() -> void:
	var sim = Simulation.new()
	var state = sim.new_game()
	_expect_true(state.resources.has("fuel"), "fuel is a core resource")
	_expect_true(not state.resources.has("batteries"), "batteries are no longer a core resource")
	_expect_true(not state.resources.has("intel"), "intel is no longer a stackable resource")
	_expect_true(state.has("evacuation"), "evacuation readiness flags exist")
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
	for day in range(1, 15):
		var event: Dictionary = sim.get_day_event(day)
		_expect_equal(event.day, day, "day event table includes day %d" % day)
		_expect_true(event.morning_text.length() > 0, "day %d has morning text" % day)
		_expect_true(event.pressure_type.length() > 0, "day %d has pressure type" % day)
		_expect_true(event.clue.length() > 0, "day %d has clue text" % day)

func _test_blood_moon_days() -> void:
	var sim = Simulation.new()
	sim.new_game()
	_expect_equal(sim.is_blood_moon_day(7), true, "day 7 is a blood moon")
	_expect_equal(sim.is_blood_moon_day(14), true, "day 14 is a blood moon")
	_expect_equal(sim.is_blood_moon_day(10), false, "day 10 is not a blood moon")

func _test_blood_moon_warnings_are_visible_before_night() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.start_day(7)
	_expect_true(sim.state.morning_context.blood_moon_warning.length() > 0, "day 7 exposes blood moon warning")
	sim.start_day(14)
	_expect_true(sim.state.morning_context.blood_moon_warning.length() > 0, "day 14 exposes blood moon warning")

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
	_expect_true(card_text.find("资源") >= 0, "location card shows resource tendency")
	_expect_true(card_text.find("危险") >= 0, "location card shows danger level")
	_expect_true(card_text.find("路况") >= 0, "location card shows road condition")
	_expect_true(card_text.find("路程") >= 0, "location card shows route time")
	_expect_true(card_text.find("图标") >= 0, "location card shows map icons")
	_expect_true(card_text.find("燃料/地图线索") >= 0, "police card shows its resource tendency")
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
	sim.state.day = 14
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
	for day in range(1, 15):
		sim.play_safe_demo_day(day)
	_expect_equal(sim.state.demo_complete, true, "demo completes after day 14 resolves")
	_expect_true(sim.state.reveal.unlocked, "Qimian log reveal unlocks at demo end")
	_expect_true(sim.state.qimian.log.size() >= 5, "reveal has at least five hidden-causality log entries")
	_expect_true(sim.state.blood_moons_resolved.has(7), "first blood moon is resolved")
	_expect_true(sim.state.blood_moons_resolved.has(14), "second blood moon is resolved")
	_expect_true(["reached_gate_quarantine", "barely_reached_gate", "collapsed"].has(sim.state.ending_state), "demo assigns a valid ending state")
	_expect_true(sim.state.reveal.summary.find("保护区") >= 0, "reveal mentions the safe zone gate")
	_expect_true(sim.state.reveal.summary.find("隔离观察") >= 0, "reveal mentions quarantine observation")
	_expect_true(sim.state.reveal.summary.find("尸群") >= 0, "reveal mentions the zombie group near miss")

func _test_day_fourteen_assigns_ending_state() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for day in range(1, 15):
		sim.play_safe_demo_day(day)
	_expect_equal(sim.state.phase, "reveal", "day 14 ends in reveal phase")
	_expect_equal(sim.state.ending_state, "reached_gate_quarantine", "safe demo route reaches the safe-zone screening gate")
	_expect_true(sim.state.reveal.summary.find("初筛") >= 0, "ending summary reflects screening")

func _test_damaged_low_resource_run_can_collapse() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.start_day(14)
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
