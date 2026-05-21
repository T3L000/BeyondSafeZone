extends Control

const Simulation = preload("res://scripts/core/game_simulation.gd")

var sim = Simulation.new()
var status_label: Label
var day_context_label: RichTextLabel
var stats_label: Label
var resources_label: Label
var shelter_label: Label
var location_box: VBoxContainer
var action_box: VBoxContainer
var event_log: RichTextLabel

func _ready() -> void:
	sim.new_game()
	_build_ui()
	_refresh()

func _build_ui() -> void:
	var root := VBoxContainer.new()
	root.set_anchors_preset(Control.PRESET_FULL_RECT)
	root.add_theme_constant_override("separation", 10)
	root.offset_left = 18
	root.offset_top = 18
	root.offset_right = -18
	root.offset_bottom = -18
	add_child(root)

	var title := Label.new()
	title.text = "保护区之外 / Beyond Safe Zone - Greybox Demo"
	title.add_theme_font_size_override("font_size", 28)
	root.add_child(title)

	status_label = Label.new()
	status_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	root.add_child(status_label)

	day_context_label = RichTextLabel.new()
	day_context_label.fit_content = true
	day_context_label.custom_minimum_size = Vector2(0, 92)
	day_context_label.bbcode_enabled = true
	root.add_child(day_context_label)

	var columns := HBoxContainer.new()
	columns.add_theme_constant_override("separation", 16)
	root.add_child(columns)

	var left := VBoxContainer.new()
	left.custom_minimum_size = Vector2(360, 0)
	columns.add_child(left)

	stats_label = Label.new()
	stats_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	left.add_child(stats_label)

	resources_label = Label.new()
	resources_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	left.add_child(resources_label)

	shelter_label = Label.new()
	shelter_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	left.add_child(shelter_label)

	var middle := VBoxContainer.new()
	middle.custom_minimum_size = Vector2(380, 0)
	columns.add_child(middle)

	var location_title := Label.new()
	location_title.text = "节点式大地图 / 白天探索"
	location_title.add_theme_font_size_override("font_size", 20)
	middle.add_child(location_title)

	location_box = VBoxContainer.new()
	middle.add_child(location_box)

	var right := VBoxContainer.new()
	right.custom_minimum_size = Vector2(380, 0)
	columns.add_child(right)

	var action_title := Label.new()
	action_title.text = "夜晚经营"
	action_title.add_theme_font_size_override("font_size", 20)
	right.add_child(action_title)

	action_box = VBoxContainer.new()
	right.add_child(action_box)

	event_log = RichTextLabel.new()
	event_log.fit_content = true
	event_log.custom_minimum_size = Vector2(0, 220)
	event_log.bbcode_enabled = true
	root.add_child(event_log)

func _refresh() -> void:
	var state: Dictionary = sim.state
	status_label.text = "第 %d 天 / 阶段：%s / 目标：%s\n%s" % [
		state.day,
		_phase_label(state.phase),
		state.goal,
		state.last_event
	]
	day_context_label.text = _build_day_context_text(state)
	stats_label.text = "林行：%s\n撤离：广播 %s  地址 %s  自行车 %s" % [
		sim.get_lin_condition_text(),
		_flag_label(state.evacuation.safezone_confirmed),
		_flag_label(state.evacuation.address_known),
		_flag_label(state.evacuation.bike_ready)
	]
	resources_label.text = "资源：食物 %d  水 %d  药 %d  建材 %d  零件 %d  燃料 %d" % [
		state.resources.food,
		state.resources.water,
		state.resources.meds,
		state.resources.materials,
		state.resources.parts,
		state.resources.fuel
	]
	shelter_label.text = "据点：门窗 %d  噪音 %d  气味 %d  光源 %d  防御 %d  整理 %d\n自行车：耐久 %d  范围 %d  载重 %d\n设施：%s" % [
		state.shelter.door,
		state.shelter.noise,
		state.shelter.scent,
		state.shelter.light,
		state.shelter.defense,
		state.shelter.supply_preservation,
		state.bike.durability,
		state.bike.range,
		state.bike.capacity,
		_facility_summary(state.shelter.facilities)
	]
	_rebuild_location_buttons()
	_rebuild_action_buttons()
	_refresh_log()

func _rebuild_location_buttons() -> void:
	for child in location_box.get_children():
		child.queue_free()
	for location_id in sim.get_location_ids():
		var button := Button.new()
		var location: Dictionary = sim.state.locations[location_id]
		var disabled_reason := ""
		if sim.state.demo_complete:
			disabled_reason = "Demo 已结束"
		elif sim.state.phase not in ["morning", "day"]:
			disabled_reason = "只能白天探索"
		elif int(location.range) > int(sim.state.bike.range):
			disabled_reason = "太远：自行车范围 %d/%d" % [int(sim.state.bike.range), int(location.range)]
		button.text = sim.get_location_card_text(location_id)
		if disabled_reason != "":
			button.text = "%s\n（%s）" % [button.text, disabled_reason]
		button.disabled = disabled_reason != ""
		button.pressed.connect(func() -> void:
			sim.enter_location(location_id)
			_refresh()
		)
		location_box.add_child(button)

func _rebuild_action_buttons() -> void:
	for child in action_box.get_children():
		child.queue_free()
	if sim.state.phase == "searching":
		_rebuild_room_search_buttons()
		return
	var actions := {
		"rest_bed": "床铺：休息降疲劳/压力",
		"workbench_repair": "工作台：修车（零件-1）",
		"barricade_windows": "封窗：加固防线（建材-2）",
		"radio_broadcast": "收音机：听广播（燃料-1）",
		"organize_storage": "整理台：打包物资",
		"treat_wound": "处理伤口（药品-1）",
		"quiet": "降低噪音",
		"mask_scent": "遮蔽气味（建材-1）"
	}
	for action_id in actions.keys():
		var button := Button.new()
		button.text = actions[action_id]
		var disabled_reason := ""
		if sim.state.demo_complete:
			disabled_reason = "Demo 已结束"
		elif sim.state.phase not in ["evening", "night"]:
			disabled_reason = "等待白天探索结束"
		if disabled_reason != "":
			button.text = "%s\n（%s）" % [button.text, disabled_reason]
		button.disabled = disabled_reason != ""
		button.pressed.connect(func() -> void:
			sim.perform_shelter_action(action_id)
			_refresh()
		)
		action_box.add_child(button)

	var sleep_button := Button.new()
	sleep_button.text = "睡觉并结算夜晚"
	var sleep_disabled_reason := ""
	if sim.state.demo_complete:
		sleep_disabled_reason = "Demo 已结束"
	elif sim.state.phase not in ["evening", "night"]:
		sleep_disabled_reason = "需要先完成白天探索"
	if sleep_disabled_reason != "":
		sleep_button.text = "%s\n（%s）" % [sleep_button.text, sleep_disabled_reason]
	sleep_button.disabled = sleep_disabled_reason != ""
	sleep_button.pressed.connect(func() -> void:
		sim.sleep_and_resolve_night()
		_refresh()
	)
	action_box.add_child(sleep_button)

	var restart_button := Button.new()
	restart_button.text = "重新开始"
	restart_button.pressed.connect(func() -> void:
		sim.new_game()
		_refresh()
	)
	action_box.add_child(restart_button)

func _rebuild_room_search_buttons() -> void:
	var location_id := str(sim.state.exploration.active_location)
	var location: Dictionary = sim.state.locations[location_id]
	var header := Label.new()
	header.text = "室内搜索：%s  时间 %d/%d  噪音 %d\n%s" % [
		location.name,
		int(sim.state.exploration.time_used),
		int(sim.state.exploration.time_limit),
		int(sim.state.exploration.noise),
		sim.get_lin_condition_text()
	]
	header.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	action_box.add_child(header)

	for room_id in location.rooms.keys():
		var room_label := Label.new()
		room_label.text = sim.get_room_card_text(room_id)
		room_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
		action_box.add_child(room_label)

		var search_button := Button.new()
		search_button.text = "谨慎搜索：%s" % location.rooms[room_id].name
		search_button.disabled = bool(location.rooms[room_id].searched)
		search_button.pressed.connect(func() -> void:
			sim.search_room(room_id, "careful")
			_refresh()
		)
		action_box.add_child(search_button)

		var quick_button := Button.new()
		quick_button.text = "快速搜索：%s" % location.rooms[room_id].name
		quick_button.disabled = bool(location.rooms[room_id].searched)
		quick_button.pressed.connect(func() -> void:
			sim.search_room(room_id, "quick")
			_refresh()
		)
		action_box.add_child(quick_button)

		var lure_button := Button.new()
		lure_button.text = "制造噪音：%s" % location.rooms[room_id].name
		lure_button.disabled = bool(location.rooms[room_id].searched)
		lure_button.pressed.connect(func() -> void:
			sim.lure_room(room_id)
			_refresh()
		)
		action_box.add_child(lure_button)

	var leave_button := Button.new()
	leave_button.text = "离开地点，回到据点"
	leave_button.pressed.connect(func() -> void:
		sim.leave_exploration()
		_refresh()
	)
	action_box.add_child(leave_button)

func _refresh_log() -> void:
	var lines := []
	lines.append("[b]普通事件[/b]")
	lines.append(sim.state.last_event)
	lines.append("")
	lines.append("[b]祁眠异常线索[/b]")
	if sim.state.qimian.public_clues.is_empty():
		lines.append("暂时没有。")
	else:
		for clue in sim.state.qimian.public_clues:
			lines.append("- %s" % clue)

	if sim.state.reveal.unlocked:
		lines.append("")
		lines.append("[b]祁眠行动日志[/b]")
		lines.append("结局：%s" % _ending_label(sim.state.ending_state))
		lines.append(sim.state.reveal.summary)
		for entry in sim.state.qimian.log:
			lines.append("第 %d 天：%s - %s" % [entry.day, entry.title, entry.truth])
	else:
		lines.append("")
		lines.append("祁眠日志仍被隐藏。通关 Demo 后解锁。")
	event_log.text = "\n".join(lines)

func _build_day_context_text(state: Dictionary) -> String:
	var context: Dictionary = state.morning_context
	var lines := []
	lines.append("[b]今日态势[/b]")
	lines.append("压力：%s" % _pressure_label(str(context.get("pressure_type", "unknown"))))
	lines.append("清晨：%s" % str(context.get("text", "")))
	lines.append("线索：%s" % str(context.get("clue", "")))
	var warning := str(context.get("blood_moon_warning", ""))
	if warning != "":
		lines.append("[color=red]血月预警：%s[/color]" % warning)
	if state.demo_complete:
		lines.append("[b]结局：%s[/b]" % _ending_label(state.ending_state))
	return "\n".join(lines)

func _phase_label(phase: String) -> String:
	match phase:
		"morning":
			return "清晨"
		"day":
			return "白天"
		"evening":
			return "黄昏"
		"night":
			return "夜晚"
		"searching":
			return "室内搜索"
		"reveal":
			return "日志揭示"
		_:
			return phase

func _pressure_label(pressure_type: String) -> String:
	match pressure_type:
		"tutorial":
			return "熟悉环境"
		"scarcity":
			return "资源紧张"
		"stress":
			return "精神压力"
		"mobility":
			return "移动受限"
		"shelter":
			return "据点暴露"
		"warning":
			return "血月前兆"
		"blood_moon":
			return "血月"
		"aftermath":
			return "灾后清点"
		"foreshadow":
			return "异常伏笔"
		"qimian":
			return "异常痕迹"
		_:
			return pressure_type

func _ending_label(ending_state: String) -> String:
	match ending_state:
		"reached_gate_quarantine":
			return "抵达保护区门口，隔离观察"
		"barely_reached_gate":
			return "勉强抵达保护区门口"
		"collapsed":
			return "崩溃边缘"
		_:
			return "进行中"

func _flag_label(value: bool) -> String:
	return "已确认" if value else "未确认"

func _facility_summary(facilities: Dictionary) -> String:
	var labels := []
	for facility_id in ["bed", "workbench", "barricade", "radio", "storage"]:
		var facility: Dictionary = facilities[facility_id]
		var used_label := "*" if bool(facility.used_today) else ""
		labels.append("%s%d%s" % [facility.name, int(facility.level), used_label])
	return "  ".join(labels)
