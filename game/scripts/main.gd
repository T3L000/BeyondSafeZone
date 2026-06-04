extends Control

const UILabels = preload("res://scripts/view/labels.gd")
const _GameState = preload("res://scripts/model/game_state.gd")
const NodeMapView = preload("res://scripts/view/node_map_view.gd")
const ExplorerView = preload("res://scripts/view/explorer_view.gd")
const ShelterPanel = preload("res://scripts/view/shelter_panel.gd")

var manager: Node

# Visual views
var node_map: NodeMapView
var explorer_view: ExplorerView
var shelter_panel: ShelterPanel

# Text widgets
var status_label: Label
var stats_label: Label
var resources_label: Label
var action_box: VBoxContainer
var event_log: RichTextLabel
var middle_section: Control  # container that swaps between map and explorer
var right_section: Control

func _ready() -> void:
	manager = Node.new()
	manager.set_script(preload("res://scripts/managers/game_manager.gd"))
	add_child(manager)
	manager.state_changed.connect(_on_state_changed)
	_build_ui()
	_refresh()

func _on_state_changed() -> void:
	_refresh()

func _build_ui() -> void:
	var root := VBoxContainer.new()
	root.set_anchors_preset(Control.PRESET_FULL_RECT)
	root.add_theme_constant_override("separation", 8)
	root.offset_left = 12
	root.offset_top = 10
	root.offset_right = -12
	root.offset_bottom = -10
	add_child(root)

	# Title
	var title := Label.new()
	title.text = "保护区之外 / Beyond Safe Zone — 灰盒演示"
	title.add_theme_font_size_override("font_size", 24)
	title.add_theme_color_override("font_color", Color.CRIMSON)
	root.add_child(title)

	# Status bar
	status_label = Label.new()
	status_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	status_label.custom_minimum_size = Vector2(0, 50)
	root.add_child(status_label)

	# Three-column layout
	var columns := HBoxContainer.new()
	columns.add_theme_constant_override("separation", 10)
	columns.size_flags_vertical = Control.SIZE_EXPAND_FILL
	root.add_child(columns)

	# LEFT: Stats + Resources + Shelter
	var left := VBoxContainer.new()
	left.custom_minimum_size = Vector2(280, 0)
	left.add_theme_constant_override("separation", 6)
	columns.add_child(left)

	stats_label = Label.new()
	stats_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	left.add_child(stats_label)

	resources_label = Label.new()
	resources_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	left.add_child(resources_label)

	shelter_panel = ShelterPanel.new()
	shelter_panel.size_flags_vertical = Control.SIZE_EXPAND_FILL
	shelter_panel.custom_minimum_size = Vector2(0, 280)
	left.add_child(shelter_panel)

	# MIDDLE: NodeMap or Explorer (swapped dynamically)
	middle_section = Control.new()
	middle_section.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	middle_section.size_flags_vertical = Control.SIZE_EXPAND_FILL
	middle_section.custom_minimum_size = Vector2(420, 300)
	columns.add_child(middle_section)

	# Node map view (default)
	node_map = NodeMapView.new()
	node_map.set_anchors_preset(Control.PRESET_FULL_RECT)
	node_map.setup(null, null, func(lid: String): manager.on_explore(lid))
	middle_section.add_child(node_map)

	# Explorer view (hidden initially)
	explorer_view = ExplorerView.new()
	explorer_view.set_anchors_preset(Control.PRESET_FULL_RECT)
	explorer_view.setup(null, func(rid, tac): manager.on_search_room(rid, tac), func(rid): manager.on_lure_room(rid), func(): manager.on_leave_exploration())
	explorer_view.visible = false
	middle_section.add_child(explorer_view)

	# RIGHT: Action buttons
	right_section = VBoxContainer.new()
	right_section.custom_minimum_size = Vector2(200, 0)
	right_section.add_theme_constant_override("separation", 4)
	columns.add_child(right_section)

	var action_label := Label.new()
	action_label.text = "操作"
	action_label.add_theme_font_size_override("font_size", 16)
	action_label.add_theme_color_override("font_color", Color.CRIMSON)
	right_section.add_child(action_label)

	action_box = VBoxContainer.new()
	action_box.add_theme_constant_override("separation", 3)
	action_box.size_flags_vertical = Control.SIZE_EXPAND_FILL
	right_section.add_child(action_box)

	# Event log at bottom
	event_log = RichTextLabel.new()
	event_log.custom_minimum_size = Vector2(0, 170)
	event_log.bbcode_enabled = true
	event_log.scroll_following = true
	root.add_child(event_log)

func _refresh() -> void:
	var state: _GameState = manager.get_state()
	var sim: RefCounted = manager.get_sim()

	# Status bar
	status_label.text = "%s\n%s  |  %s" % [
		state.last_event,
		"第 %d 天  %s  目标：%s" % [int(state.day), UILabels.phase_label(str(state.phase)), state.goal],
		_build_short_context(state),
	]

	# Left panel stats
	stats_label.text = "🧑 林行：%s\n撤离：📻%s 📍%s 🚗%s\n零件：🔋%d 🛢️%d 🛞%d | 修理：%s" % [
		sim.get_lin_condition_text(),
		UILabels.flag_label(state.evacuation.safezone_confirmed),
		UILabels.flag_label(state.evacuation.address_known),
		UILabels.flag_label(state.evacuation.car_ready),
		int(state.car_parts.battery), int(state.car_parts.gasoline), int(state.car_parts.tire),
		UILabels.car_step_label(state.car),
	]

	resources_label.text = "🍞%d 💧%d 💊%d  🧱%d 🔧%d ⛽%d" % [
		int(state.resources.food), int(state.resources.water), int(state.resources.meds),
		int(state.resources.materials), int(state.resources.parts), int(state.resources.fuel),
	]

	# Shelter panel
	shelter_panel.setup(state)

	# Swap middle section based on phase
	var is_searching := str(state.phase) == "searching"
	node_map.visible = not is_searching
	explorer_view.visible = is_searching

	if is_searching:
		explorer_view.setup(state,
			func(rid, tac): manager.on_search_room(rid, tac),
			func(rid): manager.on_lure_room(rid),
			func(): manager.on_leave_exploration()
		)
	else:
		node_map.setup(state, sim, func(lid: String): manager.on_explore(lid))

	# Action buttons
	_refresh_action_buttons()

	# Event log
	_refresh_log()

func _refresh_action_buttons() -> void:
	for child in action_box.get_children():
		child.queue_free()
	var state: _GameState = manager.get_state()

	if state.demo_complete:
		var label := Label.new()
		label.text = "Demo 结束\n祁眠日志已解锁"
		label.add_theme_color_override("font_color", Color.CRIMSON)
		action_box.add_child(label)
		_add_restart_button()
		return

	if str(state.phase) == "searching":
		# During exploration: show a tip, the explorer view handles clicks
		var tip := Label.new()
		tip.text = "👆 点击房间进行搜索\n暗房 → 先引开尸群\n搜完 → 点击离开"
		tip.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
		action_box.add_child(tip)
		return

	# During evening/night: show shelter actions
	var phase_ok := str(state.phase) in ["evening", "night"]
	var act_data := [
		["rest_bed", "🛏️ 休息"],
		["workbench_repair", "🔧 修自行车"],
		["barricade_windows", "🪟 封窗(建材-2)"],
		["radio_broadcast", "📻 听广播(燃料-1)"],
		["organize_storage", "📦 整理物资"],
		["treat_wound", "💊 处理伤口"],
		["workbench_car", "🚗 修理汽车"],
		["fortify", "🛡️ 加固(建材-2)"],
		["quiet", "🤫 降低噪音"],
		["mask_scent", "🫙 遮蔽气味"],
	]

	for pair in act_data:
		var action_id: String = pair[0]
		var btn_label: String = pair[1]
		var btn := Button.new()
		btn.text = btn_label
		if not phase_ok:
			btn.disabled = true
			btn.text += " (等待夜晚)"
		if action_id == "workbench_car" and not (bool(state.car.found) and not bool(state.car.ready)):
			continue
		btn.pressed.connect(func(): manager.on_shelter_action(action_id))
		action_box.add_child(btn)

	# Sleep button
	var sleep_btn := Button.new()
	sleep_btn.text = "😴 睡觉结算夜晚" if phase_ok else "😴 (等待夜晚)"
	sleep_btn.disabled = not phase_ok
	sleep_btn.pressed.connect(func(): manager.on_sleep())
	action_box.add_child(sleep_btn)

	_add_restart_button()

func _add_restart_button() -> void:
	var btn := Button.new()
	btn.text = "🔄 重新开始"
	btn.pressed.connect(func(): manager.start_new_game())
	action_box.add_child(btn)

func _build_short_context(state: _GameState) -> String:
	var ctx: Dictionary = state.morning_context
	var parts := []
	parts.append("态势：%s" % UILabels.pressure_label(str(ctx.get("pressure_type", ""))))
	var warn := str(ctx.get("blood_moon_warning", ""))
	if warn != "":
		parts.append("[color=red]%s[/color]" % warn)
	if state.demo_complete:
		parts.append("[b]结局：%s[/b]" % UILabels.ending_label(str(state.ending_state)))
	return " | ".join(parts)

func _refresh_log() -> void:
	var state: _GameState = manager.get_state()
	var lines := []

	lines.append("[b]普通事件[/b]")
	lines.append(state.last_event)
	lines.append("")

	lines.append("[b]祁眠异常线索[/b]")
	if state.qimian.public_clues.is_empty():
		lines.append("暂时没有。")
	else:
		for clue in state.qimian.public_clues:
			lines.append("- %s" % clue)

	if state.reveal.unlocked:
		lines.append("")
		lines.append("[b]═══ 祁眠行动日志 · 一周目回放 ═══[/b]")
		lines.append("结局：%s" % UILabels.ending_label(state.ending_state))
		lines.append(state.reveal.summary)
		lines.append("")
		lines.append("[b]▸ 祁眠人格卡[/b]")
		lines.append("主目标：%s | 暴露：%s | 道德：%s" % [
			state.qimian.personality_card.main_goal,
			state.qimian.personality_card.exposure,
			state.qimian.personality_card.moral_rule,
		])
		lines.append("")
		lines.append("[b]▸ AI 运行状态[/b]")
		lines.append("暴露值：%d/10 | 摩托：Lv.%d | 祁烬线索：%d/3" % [
			int(state.qimian.ai_state.exposure),
			int(state.qimian.ai_state.moto_tier),
			int(state.qimian.ai_state.qijin_clues),
		])
		lines.append("")
		lines.append("[b]▸ 逐日行动回放[/b]")
		for entry in state.qimian.log:
			lines.append("── 第 %d 天 ──" % entry.day)
			lines.append("   行动：%s" % entry.title)
			lines.append("   真相：%s" % entry.truth)
			if entry.has("ai_replay") and str(entry.ai_replay) != "":
				lines.append("   AI 决策：%s" % entry.ai_replay)
			if entry.has("subjective_fragment") and str(entry.subjective_fragment) != "":
				lines.append("   祁眠记录：%s" % entry.subjective_fragment)
	else:
		lines.append("")
		lines.append("祁眠日志仍被隐藏。通关 Demo 后解锁。")

	event_log.text = "\n".join(lines)
