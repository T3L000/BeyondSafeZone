extends SceneTree

const Simulation = preload("res://scripts/core/game_simulation.gd")

var failures: Array[String] = []

func _initialize() -> void:
	_test_initial_goal_and_stats()
	_test_day_event_table_and_morning_context()
	_test_blood_moon_days()
	_test_blood_moon_warnings_are_visible_before_night()
	_test_morning_pressure_applies_once()
	_test_exploration_marks_location_and_reports_risk()
	_test_qimian_starts_after_day_ten_and_changes_world()
	_test_demo_reveal_contains_hidden_causality()
	_test_day_fourteen_assigns_ending_state()
	_test_damaged_low_resource_run_can_collapse()
	_report()

func _test_initial_goal_and_stats() -> void:
	var sim = Simulation.new()
	var state = sim.new_game()
	_expect_equal(state.day, 1, "new game starts on day 1")
	_expect_equal(state.goal, "撤离到保护区", "Chen Xing's main goal is evacuation")
	_expect_true(state.resources.food > 0, "player starts with food")
	_expect_true(state.shelter.door > 0, "shelter starts with a usable door")
	_expect_equal(state.qimian.awake, false, "Qimian starts asleep")
	_expect_equal(state.ending_state, "in_progress", "new game starts without an ending")
	_expect_true(state.morning_context.text.length() > 0, "new game has a morning context")

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
	var first_stress: int = sim.state.chen.stress
	var first_water: int = sim.state.resources.water
	sim.start_day(5)
	_expect_equal(sim.state.chen.stress, first_stress, "day pressure does not stack when morning refreshes")
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

func _test_qimian_starts_after_day_ten_and_changes_world() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.resolve_qimian_for_day(10)
	_expect_equal(sim.state.qimian.awake, false, "Qimian is still asleep on day 10")
	_expect_equal(sim.state.qimian.log.size(), 0, "Qimian has no action log before waking")
	sim.resolve_qimian_for_day(11)
	_expect_equal(sim.state.qimian.awake, true, "Qimian wakes on day 11")
	_expect_true(sim.state.qimian.log.size() >= 1, "Qimian logs an action on day 11")
	_expect_true(sim.state.locations.supermarket.resources.food < 8, "Qimian can change shared supermarket resources")

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
	_expect_true(["survived_demo", "barely_survived", "collapsed"].has(sim.state.ending_state), "demo assigns a valid ending state")

func _test_day_fourteen_assigns_ending_state() -> void:
	var sim = Simulation.new()
	sim.new_game()
	for day in range(1, 15):
		sim.play_safe_demo_day(day)
	_expect_equal(sim.state.phase, "reveal", "day 14 ends in reveal phase")
	_expect_equal(sim.state.ending_state, "survived_demo", "safe demo route survives the demo")
	_expect_true(sim.state.reveal.summary.find("撑过") >= 0, "ending summary reflects survival result")

func _test_damaged_low_resource_run_can_collapse() -> void:
	var sim = Simulation.new()
	sim.new_game()
	sim.start_day(14)
	sim.state.resources.food = 0
	sim.state.resources.water = 0
	sim.state.chen.health = 1
	sim.state.chen.hunger = 3
	sim.state.chen.thirst = 3
	sim.state.chen.stress = 9
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
