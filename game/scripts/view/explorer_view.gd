# 侧视横版探索视图 —— 建筑剖面，多层房间格子+尸群+林行
class_name ExplorerView extends Control

var _state = null
var _on_search: Callable
var _on_lure: Callable
var _on_leave: Callable
var _room_zones: Array = []

func setup(state, on_search: Callable, on_lure: Callable, on_leave: Callable) -> void:
	_state = state
	_on_search = on_search
	_on_lure = on_lure
	_on_leave = on_leave
	queue_redraw()

func _draw() -> void:
	if _state == null: return
	var w: float = size.x
	var h: float = size.y
	if w <= 0 or h <= 0: return

	draw_rect(Rect2(0, 0, w, h), Color(0.06, 0.06, 0.14, 1))

	var loc_id: String = str(_state.exploration.active_location)
	if loc_id == "" or not _state.locations.has(loc_id): return

	var loc: Dictionary = _state.locations[loc_id]
	var rooms: Dictionary = loc.rooms
	var room_count: int = max(1, rooms.size())

	var bx: float = w * 0.06
	var by: float = h * 0.08
	var bw: float = w * 0.88
	var bh: float = h * 0.68
	var rw: float = bw / float(room_count)
	var margin: float = 6.0

	draw_rect(Rect2(bx, by, bw, bh), Color(0.08, 0.08, 0.16, 1))
	draw_rect(Rect2(bx, by, bw, bh), Color(0.2, 0.25, 0.35, 0.5), false, 2.0)
	draw_rect(Rect2(bx - 4, by - 10, bw + 8, 12), Color(0.12, 0.12, 0.20, 1))

	var f: Font = ThemeDB.fallback_font
	_room_zones.clear()

	var idx: int = 0
	for room_id in rooms.keys():
		var room: Dictionary = rooms[room_id]
		var rx: float = bx + float(idx) * rw + margin
		var ry: float = by + margin
		var rr: float = rw - margin * 2
		var rh: float = bh - margin * 2

		var vis_str: String = str(room.get("visibility", ""))
		var bg: Color
		if bool(room.get("searched", false)): bg = Color(0.06, 0.14, 0.06)
		elif vis_str == "黑暗": bg = Color(0.03, 0.03, 0.06)
		elif vis_str == "昏暗": bg = Color(0.08, 0.08, 0.14)
		else: bg = Color(0.10, 0.10, 0.18)
		draw_rect(Rect2(rx, ry, rr, rh), bg)
		draw_rect(Rect2(rx, ry, rr, rh), Color(0.2, 0.2, 0.3, 0.4), false, 1.0)

		if vis_str != "黑暗":
			draw_rect(Rect2(rx + rr * 0.2, ry + 6, rr * 0.6, rh * 0.12), Color(0.2, 0.25, 0.4) if vis_str != "昏暗" else Color(0.15, 0.15, 0.3))
		else:
			draw_rect(Rect2(rx + rr * 0.2, ry + 6, rr * 0.6, rh * 0.12), Color(0.02, 0.02, 0.04))

		var door_h: float = 16.0
		draw_rect(Rect2(rx + rr * 0.35, ry + rh - door_h - 4, rr * 0.3, door_h), Color(0.25, 0.2, 0.1))

		var name_color: Color = Color(0.5, 0.9, 0.5) if bool(room.searched) else Color.WHITE
		if bool(room.get("locked", false)):
			draw_string(f, Vector2(rx + 4, ry - 8), "🔒 %s" % room.name, HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color.CRIMSON)
		else:
			draw_string(f, Vector2(rx + 4, ry - 8), str(room.name), HORIZONTAL_ALIGNMENT_LEFT, -1, 10, name_color)

		var zn: int = int(room.get("hidden_zombies", 0))
		if zn > 0 and not bool(room.searched):
			var lured: bool = _state.exploration.lured_rooms.has(room_id)
			var zx: float = rx + rr * 0.5
			var zy: float = ry + rh * 0.5
			var z: int = 0
			while z < mini(zn, 3):
				var zox: float = zx + float(z - 1) * 16
				var zoy: float = zy + float(z % 2) * 10
				draw_rect(Rect2(zox - 5, zoy - 7, 10, 13), Color(0.35, 0.55, 0.2) if not lured else Color(0.3, 0.3, 0.3))
				draw_rect(Rect2(zox - 3, zoy - 9, 6, 5), Color(0.2, 0.3, 0.15))
				draw_rect(Rect2(zox - 3, zoy - 8, 2, 2), Color.CRIMSON)
				draw_rect(Rect2(zox + 1, zoy - 8, 2, 2), Color.CRIMSON)
				z += 1
			if lured:
				draw_string(f, Vector2(zx - 18, zy - 16), "→ 已引开 →", HORIZONTAL_ALIGNMENT_LEFT, -1, 8, Color.GREEN)

		if bool(room.searched):
			draw_string(f, Vector2(rx + rr * 0.25, ry + rh * 0.38), "搜过", HORIZONTAL_ALIGNMENT_CENTER, -1, 16, Color(0.1, 0.5, 0.1))

		_room_zones.append({
			"id": room_id,
			"rect": Rect2(rx, ry, rr, rh),
			"locked": bool(room.get("locked", false)),
			"searched": bool(room.get("searched", false)),
			"has_zombie": zn > 0
		})
		idx += 1

	# Player at entrance
	var px: float = bx - 28
	var py: float = by + bh * 0.85
	draw_rect(Rect2(px - 5, py - 10, 10, 17), Color(0.3, 0.5, 0.8))
	draw_rect(Rect2(px - 3, py - 13, 6, 6), Color(0.6, 0.7, 1.0))
	draw_string(f, Vector2(px - 14, py - 18), "林行", HORIZONTAL_ALIGNMENT_CENTER, -1, 9, Color.WHITE)

	# Info bar
	var info_y: float = by + bh + 14
	draw_rect(Rect2(bx, info_y, bw, 28), Color(0, 0, 0, 0.55))
	var tused: int = int(_state.exploration.get("time_used", 0))
	var tlim: int = int(_state.exploration.get("time_limit", 0))
	var pnoise: int = int(_state.exploration.get("noise", 0))
	var unsearched: int = 0
	for rid in rooms.keys():
		if not bool(rooms[rid].searched): unsearched += 1
	draw_string(f, Vector2(bx + 8, info_y + 6), "⏱ %d/%dh  | 🔊 %d  | 🧟 %d  | 可搜: %d间" % [tused, tlim, pnoise, int(loc.zombies), unsearched], HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color.WHITE)

	var leave_x: float = bx + bw * 0.65
	draw_rect(Rect2(leave_x, info_y + 4, w - leave_x - bx, 20), Color(0.2, 0.3, 0.2))
	draw_string(f, Vector2(leave_x + 8, info_y + 5), "🚶 离开 → (点击)", HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color.GREEN)

func _gui_input(event: InputEvent) -> void:
	if not (event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT): return
	var pos: Vector2 = event.position

	# Check leave button area first
	var w: float = size.x
	var h: float = size.y
	var bw: float = w * 0.88
	var bh: float = h * 0.68
	var by: float = h * 0.08
	var bx: float = w * 0.06
	var info_y: float = by + bh + 14
	var leave_x: float = bx + bw * 0.65
	if pos.y > info_y - 2 and pos.y < info_y + 26 and pos.x > leave_x:
		_on_leave.call()
		return

	for zone in _room_zones:
		var zr: Rect2 = zone["rect"]
		if zr.has_point(pos):
			if zone["searched"] or zone["locked"]: return
			if zone["has_zombie"] and not _state.exploration.lured_rooms.has(str(zone["id"])):
				_on_lure.call(str(zone["id"]))
				return
			_on_search.call(str(zone["id"]), "careful")
			return
