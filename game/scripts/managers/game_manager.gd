extends Node

const Simulation = preload("res://scripts/core/game_simulation.gd")
const _GameState = preload("res://scripts/model/game_state.gd")

signal state_changed()

var sim := Simulation.new()

func _ready() -> void:
	sim.new_game()

func get_state() -> _GameState:
	return sim.state

func get_sim() -> RefCounted:
	return sim

# --- Day cycle ---

func start_new_game() -> void:
	sim.new_game()
	state_changed.emit()

# --- Exploration ---

func on_explore(location_id: String) -> void:
	if sim.state.demo_complete or sim.state.phase not in ["morning", "day"]:
		return
	sim.enter_location(location_id)
	state_changed.emit()

func on_search_room(room_id: String, tactic: String = "careful") -> void:
	sim.search_room(room_id, tactic)
	state_changed.emit()

func on_lure_room(room_id: String) -> void:
	sim.lure_room(room_id)
	state_changed.emit()

func on_leave_exploration() -> void:
	sim.leave_exploration()
	state_changed.emit()

# --- Shelter ---

func on_shelter_action(action_id: String) -> void:
	if sim.state.demo_complete or sim.state.phase not in ["evening", "night"]:
		return
	sim.perform_shelter_action(action_id)
	state_changed.emit()

# --- Night ---

func on_sleep() -> void:
	if sim.state.demo_complete or sim.state.phase not in ["evening", "night"]:
		return
	sim.sleep_and_resolve_night()
	state_changed.emit()
