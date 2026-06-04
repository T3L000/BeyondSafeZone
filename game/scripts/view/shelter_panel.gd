# 据点横截面视图 —— 设施可视化 + 状态条
class_name ShelterPanel extends Control

var _state = null

func setup(state) -> void:
	_state = state
	queue_redraw()

func _draw() -> void:
	if _state == null: return
	var w: float = size.x
	var h: float = size.y
	if w <= 0 or h <= 0: return

	draw_rect(Rect2(0, 0, w, h), Color(0.06, 0.06, 0.12, 1))
	var f: Font = ThemeDB.fallback_font

	var sx: float = w * 0.04
	var sy: float = h * 0.05
	var sw: float = w - sx * 2
	var sh: float = h * 0.62

	draw_rect(Rect2(sx, sy, sw, sh), Color(0.08, 0.08, 0.16, 1))
	draw_rect(Rect2(sx, sy, sw, sh), Color(0.22, 0.28, 0.38, 0.4), false, 1.5)
	draw_rect(Rect2(sx - 2, sy - 8, sw + 4, 10), Color(0.12, 0.12, 0.20))

	var fac_width: float = sw / 5.0
	var fac_names: Array[String] = ["bed", "workbench", "barricade", "radio", "storage"]
	var fac_labels: Array[String] = ["床铺", "工作台", "封窗", "收音机", "整理台"]
	var fac_emoji: Array[String] = ["🛏️", "🔧", "🪟", "📻", "📦"]
	var fac_role: Array[String] = ["恢复疲劳", "修理制造", "防线加固", "广播情报", "物资保存"]

	var i: int = 0
	while i < 5:
		var fx: float = sx + float(i) * fac_width + 4
		var fy: float = sy + 8
		var fw: float = fac_width - 8
		var fh: float = sh - 32

		var fid: String = fac_names[i]
		var used_today: bool = false
		if _state.shelter.facilities.has(fid):
			used_today = bool(_state.shelter.facilities[fid].get("used_today", false))
		var bg_c: Color = Color(0.15, 0.15, 0.28) if used_today else Color(0.10, 0.10, 0.20)
		draw_rect(Rect2(fx, fy, fw, fh), bg_c)
		draw_rect(Rect2(fx, fy, fw, fh), Color(0.25, 0.3, 0.4, 0.3), false, 1.0)

		draw_string(f, Vector2(fx + fw * 0.15, fy + fh * 0.25), fac_emoji[i], HORIZONTAL_ALIGNMENT_LEFT, -1, 20, Color.WHITE)
		draw_string(f, Vector2(fx + 2, fy + fh * 0.5), fac_labels[i], HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color(0.7, 0.7, 0.8))
		draw_string(f, Vector2(fx + 2, fy + fh * 0.68), fac_role[i], HORIZONTAL_ALIGNMENT_LEFT, -1, 8, Color(0.5, 0.5, 0.6))

		if used_today:
			draw_string(f, Vector2(fx + fw * 0.2, fy + fh * 0.82), "今日已用", HORIZONTAL_ALIGNMENT_LEFT, -1, 8, Color.GOLD)
		i += 1

	# Door indicator
	var door_x: float = sx - 12
	var door_y: float = sy + sh * 0.4
	var door_h: float = sh * 0.35
	var door_val: int = int(_state.shelter.door)
	var door_c: Color
	if door_val >= 3: door_c = Color(0.2, 0.6, 0.2)
	elif door_val >= 1: door_c = Color(0.7, 0.6, 0.2)
	else: door_c = Color(0.7, 0.2, 0.2)
	draw_rect(Rect2(door_x, door_y, 10, door_h), door_c)
	draw_string(f, Vector2(door_x - 22, door_y + door_h * 0.5 - 5), "🚪%d" % door_val, HORIZONTAL_ALIGNMENT_CENTER, -1, 9, Color.WHITE)

	# Status bars
	var bar_y: float = sy + sh + 14
	var bar_data: Array = [
		["🛡️防御", int(_state.shelter.defense), 4, Color(0.3, 0.5, 0.8)],
		["🔊噪音", int(_state.shelter.noise), 6, Color(0.7, 0.5, 0.3)],
		["👃气味", int(_state.shelter.scent), 6, Color(0.5, 0.5, 0.3)],
		["💡光源", int(_state.shelter.light), 6, Color(0.7, 0.7, 0.3)],
	]
	var bi: int = 0
	while bi < 4:
		var bar_item: Array = bar_data[bi]
		var bname: String = bar_item[0]
		var bval: int = bar_item[1]
		var bmax: int = bar_item[2]
		var bcol: Color = bar_item[3]
		var bx: float = sx + float(bi) * (sw / 4.0)
		draw_string(f, Vector2(bx + 2, bar_y - 2), "%s %d" % [bname, bval], HORIZONTAL_ALIGNMENT_LEFT, -1, 9, Color.WHITE)
		var bar_w: float = sw / 4.0 - 8
		var fill_w: float = bar_w * float(bval) / float(max(1, bmax))
		draw_rect(Rect2(bx + 2, bar_y + 8, bar_w, 7), Color(0.15, 0.15, 0.20))
		draw_rect(Rect2(bx + 2, bar_y + 8, fill_w, 7), bcol)
		bi += 1

	# Bike info
	var bik_y: float = bar_y + 24
	draw_string(f, Vector2(sx + 4, bik_y), "🚲 自行车: 耐久%d  范围%d  载重%d" % [int(_state.bike.durability), int(_state.bike.range), int(_state.bike.capacity)], HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color(0.7, 0.7, 0.6))
