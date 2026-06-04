# 汽车系统 —— 规则层，只接收 state 参数，返回文本
class_name CarSystem extends RefCounted

const _GameState = preload("res://scripts/model/game_state.gd")
const Balance = preload("res://scripts/data/balance.gd")

static func repair(state: _GameState) -> String:
	if not bool(state.car.found):
		state.last_event = "林行还没找到能用的汽车。修理铺后院的车库或许有线索。"
		return state.last_event
	if bool(state.car.ready):
		state.last_event = "汽车已经修好。油箱加满，引擎能发动。"
		return state.last_event
	# Step 1: engine wiring
	if not bool(state.car.step_engine):
		if _spend(state, "materials", Balance.CAR_REPAIR_ENGINE_MATERIALS) and _spend(state, "parts", Balance.CAR_REPAIR_ENGINE_PARTS):
			state.car.step_engine = true
			_mark_workbench(state)
			state.last_event = "林行接好引擎线路，仪表盘亮了——电路通了。还需要换轮胎、装电瓶、加汽油。"
		else:
			state.last_event = "引擎线路需要建材×%d和零件×%d，目前的材料不够。" % [Balance.CAR_REPAIR_ENGINE_MATERIALS, Balance.CAR_REPAIR_ENGINE_PARTS]
		return state.last_event
	# Step 2: tire
	if not bool(state.car.step_tire):
		if int(state.car_parts.tire) >= Balance.CAR_REPAIR_TIRE_COUNT and _spend(state, "parts", Balance.CAR_REPAIR_TIRE_PARTS):
			state.car_parts.tire -= Balance.CAR_REPAIR_TIRE_COUNT
			state.car.step_tire = true
			_mark_workbench(state)
			state.last_event = "林行卸下瘪轮胎换上新的，车身终于不再倾斜。还剩电瓶和汽油。"
		else:
			state.last_event = "需要轮胎×%d和零件×%d来换胎。" % [Balance.CAR_REPAIR_TIRE_COUNT, Balance.CAR_REPAIR_TIRE_PARTS]
		return state.last_event
	# Step 3: battery
	if not bool(state.car.step_battery):
		if int(state.car_parts.battery) >= Balance.CAR_REPAIR_BATTERY_COUNT:
			if _spend(state, "fuel", Balance.CAR_REPAIR_BATTERY_FUEL):
				state.car_parts.battery -= Balance.CAR_REPAIR_BATTERY_COUNT
				state.car.step_battery = true
				_mark_workbench(state)
				state.last_event = "林行装上电瓶、调试引擎——发动机咳嗽两声后平稳运转。最后一步：加油。"
			else:
				state.last_event = "调试引擎需要燃料×%d来测试电路。" % Balance.CAR_REPAIR_BATTERY_FUEL
		else:
			state.last_event = "需要电瓶×%d（派出所停车场有废弃警车可卸）和燃料×%d来调试。" % [Balance.CAR_REPAIR_BATTERY_COUNT, Balance.CAR_REPAIR_BATTERY_FUEL]
		return state.last_event
	# Step 4: gasoline
	if not bool(state.car.step_fueled):
		if int(state.car_parts.gasoline) >= Balance.CAR_REPAIR_GASOLINE_COUNT:
			state.car_parts.gasoline -= Balance.CAR_REPAIR_GASOLINE_COUNT
			state.car.step_fueled = true
			state.car.ready = true
			state.evacuation.car_ready = true
			_mark_workbench(state)
			state.lin.hope += 1
			state.last_event = "林行把两桶汽油倒进油箱，拧紧盖子。\n\n他坐进驾驶座，转了一下钥匙。引擎发出一声低沉的轰鸣——像一只野兽醒过来。\n\n汽车就绪。可以去保护区了。"
		else:
			state.last_event = "需要汽油×%d来加满油箱。去哨卡、加油站或地铁口找。" % Balance.CAR_REPAIR_GASOLINE_COUNT
		return state.last_event
	return state.last_event

static func evacuation_narrative(state: _GameState) -> String:
	var lines := []
	lines.append("天刚亮。远处的低吼不再是零星叫声——像瀑布一样，持续不断。")
	if bool(state.car.ready):
		lines.append("林行把最后一口背包扔进后备箱。引擎第一下没着，第二下咳嗽着启动了。排气管吐出黑烟。")
		lines.append("西线的路很通畅——桥被清理过了。后视镜里，据点的窗户像一只闭着的眼睛。他没有回头。")
		lines.append("开了四十分钟。远郊的路遍地废弃车辆，得绕。仪表盘上温度指针开始抖。")
		lines.append("一声刺耳的金属摩擦——左前轮爆了，或者引擎过热熄了火。汽车滑进路边沟里。不动了。")
		lines.append("林行转动钥匙。没反应。再转——发动机呻吟了一声，像叹息。")
		lines.append("「……操。」他下车。后备箱里能带走的只有一个背包。他塞进食物和水，背上撬棍。")
	else:
		lines.append("没有汽车。林行只能靠自行车——但自行车到不了远圈。")
		lines.append("他把能带的全塞进背包，推着自行车走了最后一段能骑的路，然后弃车徒步。")
	lines.append("保护区大门在正南方约八公里。步行要三个小时。但愿能在天黑前到。但愿尸潮比他慢。")
	lines.append("柏油路上全是裂缝。路边一辆翻倒的救护车，车门开着，里面是空的——车身上有返生计划的标志。")
	lines.append("穿过一个无名小镇。商店卷帘门都拉下来。太安静了——只听见自己的脚步和远处不变的尸潮低吼。")
	lines.append("天快黑了。月亮变成红色。背后能听到它们——不是一只两只，是像风暴一样的声音。不能回头。")
	return "\n".join(lines)

static func _spend(state: _GameState, resource_name: String, amount: int) -> bool:
	if int(state.resources.get(resource_name, 0)) < amount:
		return false
	state.resources[resource_name] = int(state.resources[resource_name]) - amount
	return true

static func _mark_workbench(state: _GameState) -> void:
	if state.shelter.facilities.has("workbench"):
		state.shelter.facilities["workbench"].used_today = true
