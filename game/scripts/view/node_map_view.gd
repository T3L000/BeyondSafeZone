# 节点大地图视图 —— 俯视14个地点节点+连线+范围圈
class_name NodeMapView extends Control

var _state = null
var _sim = null
var _on_location_clicked: Callable

func setup(state, sim: RefCounted, on_click: Callable) -> void:
	_state = state
	_sim = sim
	_on_location_clicked = on_click
	queue_redraw()

func _draw() -> void:
	if _state == null: return
	var w: float = size.x
	var h: float = size.y
	if w <= 0 or h <= 0: return

	draw_rect(Rect2(0, 0, w, h), Color(0.06, 0.06, 0.12, 1))
	var grid_color: Color = Color(0.12, 0.12, 0.18, 0.5)
	var gi: int = 0
	while gi < int(w):
		draw_line(Vector2(gi, 0), Vector2(gi, h), grid_color, 0.5)
		gi += 30
	gi = 0
	while gi < int(h):
		draw_line(Vector2(0, gi), Vector2(w, gi), grid_color, 0.5)
		gi += 30

	var home_x: float = w * 0.72
	var home_y: float = h * 0.72
	var home_r: float = 50.0

	var near_rad: float = h * 0.22
	var mid_rad: float = h * 0.38
	draw_arc(Vector2(home_x, home_y), near_rad, 0, TAU, 64, Color(0.2, 0.25, 0.35, 0.4), 1.5, true)
	draw_arc(Vector2(home_x, home_y), mid_rad, 0, TAU, 64, Color(0.2, 0.25, 0.35, 0.3), 1.0, true)

	var f: Font = ThemeDB.fallback_font
	var fsize: int = 10
	draw_string(f, Vector2(home_x + near_rad - 36, home_y - near_rad + 14), "近圈", HORIZONTAL_ALIGNMENT_LEFT, -1, fsize, Color(0.3, 0.4, 0.6))
	draw_string(f, Vector2(home_x + mid_rad - 36, home_y - mid_rad + 14), "中圈", HORIZONTAL_ALIGNMENT_LEFT, -1, fsize, Color(0.25, 0.35, 0.5))

	var bike_range: int = int(_state.bike.range)

	for location_id in _state.locations.keys():
		var loc: Dictionary = _state.locations[location_id]
		var raw_pos: Vector2 = _get_location_pos(str(location_id))
		var x: float = raw_pos.x * w
		var y: float = raw_pos.y * h
		var reachable: bool = int(loc.range) <= bike_range

		var line_c: Color = Color(0.25, 0.25, 0.18, 0.3) if bool(loc.visited) else Color(0.2, 0.2, 0.15, 0.2)
		if reachable: line_c = Color(0.3, 0.35, 0.25, 0.4)
		draw_line(Vector2(x, y), Vector2(home_x, home_y), line_c, 1.0)

		var radius: float = 11.0 + float(int(loc.zombies)) * 1.2
		var base_c: Color
		if not reachable: base_c = Color(0.3, 0.12, 0.12)
		elif bool(loc.visited): base_c = Color(0.12, 0.28, 0.12)
		elif bool(loc.get("qimian_trace", false)): base_c = Color(0.22, 0.22, 0.12)
		else: base_c = Color(0.15, 0.25, 0.35)

		draw_circle(Vector2(x, y), radius, base_c)
		draw_arc(Vector2(x, y), radius, 0, TAU, 32, Color(0.4, 0.5, 0.6, 0.6) if reachable else Color(0.5, 0.2, 0.2, 0.6), 1.5)

		var name_color: Color = Color.WHITE if reachable else Color(0.5, 0.5, 0.5)
		draw_string(f, Vector2(x - 18, y - radius - 13), str(loc.name), HORIZONTAL_ALIGNMENT_LEFT, -1, 9, name_color)

		var z_count: int = mini(int(loc.zombies), 5)
		var zi: int = 0
		while zi < z_count:
			var dot_c: Color
			if zi < 3: dot_c = Color.GREEN_YELLOW
			elif zi < 4: dot_c = Color.GOLD
			else: dot_c = Color.CRIMSON
			draw_circle(Vector2(x - 8 + zi * 4, y + radius + 6), 2.0, dot_c)
			zi += 1

		if bool(loc.get("qimian_trace", false)):
			draw_circle(Vector2(x + radius + 2, y - radius * 0.5), 4.0, Color.CORNFLOWER_BLUE)

	draw_circle(Vector2(home_x, home_y), home_r, Color(0.12, 0.12, 0.22, 0.9))
	draw_arc(Vector2(home_x, home_y), home_r, 0, TAU, 32, Color.CRIMSON, 2.5)
	draw_string(f, Vector2(home_x - 40, home_y - 20), "🏠 林行据点", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, Color.WHITE)

	draw_circle(Vector2(w * 0.18, h * 0.2), 35.0, Color(0.12, 0.3, 0.12, 0.5))
	draw_arc(Vector2(w * 0.18, h * 0.2), 35.0, 0, TAU, 32, Color.GREEN, 2.0)
	draw_string(f, Vector2(w * 0.09, h * 0.12), "🏕️ 保护区", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, Color.GREEN)

	var phase_name: String = "?"
	match str(_state.phase):
		"morning": phase_name = "清晨"
		"day": phase_name = "白天"
		"evening": phase_name = "黄昏"
		"night": phase_name = "夜晚"
		"reveal": phase_name = "结局"
	draw_rect(Rect2(12, 8, 180, 36), Color(0, 0, 0, 0.6))
	draw_string(f, Vector2(18, 18), "第 %d 天 · %s" % [int(_state.day), phase_name], HORIZONTAL_ALIGNMENT_LEFT, -1, 13, Color.WHITE)

	if str(_state.morning_context.get("bm_warn", "")) != "":
		draw_string(f, Vector2(18, 34), "⚠ %s" % _state.morning_context.bm_warn, HORIZONTAL_ALIGNMENT_LEFT, -1, 9, Color.CRIMSON)

func _get_location_pos(id: String) -> Vector2:
	match id:
		"home": return Vector2(0.82, 0.78)
		"convenience": return Vector2(0.68, 0.70)
		"clinic": return Vector2(0.58, 0.80)
		"bike_shop": return Vector2(0.87, 0.84)
		"supermarket": return Vector2(0.42, 0.58)
		"school": return Vector2(0.35, 0.75)
		"police": return Vector2(0.52, 0.44)
		"subway": return Vector2(0.62, 0.38)
		"bridge_camp": return Vector2(0.28, 0.46)
		"gas_station": return Vector2(0.72, 0.30)
		"hardware_store": return Vector2(0.46, 0.68)
		"apartment": return Vector2(0.38, 0.62)
		"safezone_edge": return Vector2(0.18, 0.22)
		"quarantine": return Vector2(0.25, 0.14)
	return Vector2(0.5, 0.5)

func _gui_input(event: InputEvent) -> void:
	if not (event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT): return
	if _state == null or _state.demo_complete or not str(_state.phase) in ["morning", "day"]: return

	var pos: Vector2 = event.position
	var w: float = size.x
	var h: float = size.y

	for location_id in _state.locations.keys():
		var np: Vector2 = _get_location_pos(str(location_id)) * Vector2(w, h)
		if pos.distance_to(np) < 28.0:
			_on_location_clicked.call(location_id)
			return
