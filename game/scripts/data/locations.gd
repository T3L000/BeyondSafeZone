# 地点和房间数据 —— 纯数据，零逻辑
class_name LocationData

# 图标中文名映射
const ICON_LABELS := {
	"home": "🏠 林行家", "food": "🍞 食物", "water": "💧 饮水", "meds": "💊 药品",
	"parts": "🔧 零件", "bike": "🚲 自行车", "materials": "🧱 建材", "fuel": "⛽ 燃料",
	"gasoline": "🛢️ 汽油", "danger": "⚠️ 高风险", "clue": "📋 撤离线索",
	"route": "🗺️ 路线信息", "question": "❓ 异常痕迹", "safezone": "🏕️ 保护区",
	"qimian": "👤 祁眠痕迹", "npc": "🧑 幸存者"
}

const ROAD_NOTES := {
	"熟路": "无额外体力消耗", "碎玻璃": "无额外体力消耗",
	"雨后湿滑": "疲劳 +1", "积水": "疲劳 +1", "堵塞": "疲劳 +1", "路障": "疲劳 +1",
	"尸群迁移": "疲劳 +2", "封锁线": "疲劳 +2"
}

# 地点顶层数据
const LOCATION_DEFS := {
	"home": {"name": "林行家", "ring": "近圈", "range": 1, "zombies": 1, "resources": {"food": 1, "water": 2, "materials": 1}, "resource_tendency": "少量补给", "danger_level": "低", "route_time": 1, "road_condition": "熟路", "icons": ["home"]},
	"convenience": {"name": "小区便利店", "ring": "近圈", "range": 1, "zombies": 3, "resources": {"food": 4, "water": 4, "fuel": 1}, "resource_tendency": "食物/水", "danger_level": "中", "route_time": 1, "road_condition": "碎玻璃", "icons": ["food", "water"]},
	"clinic": {"name": "社区诊所", "ring": "近圈", "range": 1, "zombies": 2, "resources": {"meds": 2, "materials": 1}, "resource_tendency": "药品", "danger_level": "中", "route_time": 1, "road_condition": "雨后湿滑", "icons": ["meds", "question"]},
	"bike_shop": {"name": "自行车修理铺+车库", "ring": "近圈", "range": 1, "zombies": 2, "resources": {"parts": 3, "materials": 2, "tire": 1}, "resource_tendency": "零件/建材/轮胎", "danger_level": "中", "route_time": 1, "road_condition": "堵塞", "icons": ["parts", "bike", "question"]},
	"supermarket": {"name": "超市", "ring": "中圈", "range": 2, "zombies": 4, "resources": {"food": 8, "water": 4, "materials": 2}, "resource_tendency": "大量食物", "danger_level": "高", "route_time": 2, "road_condition": "尸群迁移", "icons": ["food", "danger"]},
	"school": {"name": "废弃学校", "ring": "中圈", "range": 2, "zombies": 3, "resources": {"materials": 4, "fuel": 1}, "resource_tendency": "建材/燃料", "danger_level": "中", "route_time": 2, "road_condition": "积水", "icons": ["materials"]},
	"police": {"name": "派出所", "ring": "中圈", "range": 2, "zombies": 5, "resources": {"fuel": 2, "materials": 1, "battery": 1}, "resource_tendency": "燃料/线索/电瓶", "danger_level": "高", "route_time": 2, "road_condition": "路障", "icons": ["fuel", "clue", "question"]},
	"subway": {"name": "地铁口", "ring": "中圈", "range": 2, "zombies": 5, "resources": {"materials": 2, "fuel": 1, "parts": 1, "gasoline": 1}, "resource_tendency": "路线线索/汽油", "danger_level": "高", "route_time": 2, "road_condition": "尸群迁移", "icons": ["route", "question"]},
	"bridge_camp": {"name": "桥洞营地", "ring": "中圈", "range": 2, "zombies": 1, "resources": {}, "resource_tendency": "幸存者据点", "danger_level": "低", "route_time": 2, "road_condition": "熟路", "icons": ["npc"]},
	"gas_station": {"name": "加油站", "ring": "中圈", "range": 2, "zombies": 4, "resources": {"fuel": 3, "gasoline": 2}, "resource_tendency": "大量燃料", "danger_level": "高", "route_time": 2, "road_condition": "尸群迁移", "icons": ["fuel", "danger"]},
	"hardware_store": {"name": "五金店", "ring": "中圈", "range": 2, "zombies": 3, "resources": {"materials": 6, "parts": 3}, "resource_tendency": "大量建材", "danger_level": "中", "route_time": 2, "road_condition": "堵塞", "icons": ["materials"]},
	"apartment": {"name": "废弃公寓", "ring": "中圈", "range": 2, "zombies": 3, "resources": {}, "resource_tendency": "混合物资/幸存者", "danger_level": "中", "route_time": 2, "road_condition": "积水", "icons": ["question", "npc"]},
	"safezone_edge": {"name": "城市边缘哨卡", "ring": "远圈", "range": 3, "zombies": 6, "resources": {"fuel": 2, "battery": 1, "gasoline": 1}, "resource_tendency": "燃料/电瓶/汽油", "danger_level": "极高", "route_time": 3, "road_condition": "封锁线", "icons": ["safezone", "danger", "bike"]},
	"quarantine": {"name": "防疫隔离站", "ring": "远圈", "range": 3, "zombies": 4, "resources": {"meds": 3, "fuel": 1, "gasoline": 1}, "resource_tendency": "药品/线索", "danger_level": "高", "route_time": 3, "road_condition": "封锁线", "icons": ["meds", "question"]}
}

# 房间数据（简化 key: room_name, visibility, search_time, hidden_zombies, res_dict, flags, locked）
const ROOM_DEFS := {
	"home": [
		["living_room", "客厅", "窗光", 1, 0, {"food": 1, "water": 1, "materials": 1}, ["plan_found"], false],
		["bedroom", "卧室", "窗光", 1, 0, {}, [], false],
		["kitchen", "厨房", "窗光", 1, 0, {"food": 1, "water": 1}, [], false],
		["storage", "储物间", "黑暗", 2, 1, {"materials": 3, "parts": 2}, [], false],
	],
	"convenience": [
		["storefront", "店面", "窗光", 1, 0, {"food": 3, "water": 2}, [], false],
		["warehouse", "仓库夹层", "黑暗", 2, 1, {"food": 2, "materials": 1}, ["safezone_hint_1"], false],
	],
	"clinic": [
		["waiting", "候诊室", "窗光", 1, 0, {"water": 2, "materials": 1}, [], false],
		["exam_a", "诊室A", "窗光", 1, 1, {"meds": 2}, ["rebirth_clue_1"], false],
		["pharmacy", "药房", "黑暗", 2, 2, {"meds": 4}, [], false],
		["back_alley", "后门垃圾区", "窗光", 1, 0, {"materials": 1, "food": 1}, [], false],
	],
	"bike_shop": [
		["storefront", "店面", "窗光", 1, 0, {"parts": 3, "materials": 1, "tire": 1}, [], false],
		["workshop", "工作间", "窗光", 1, 1, {"parts": 2, "fuel": 1}, [], false],
		["backyard", "后院", "窗光", 1, 0, {"materials": 2}, [], false],
		["garage", "车库", "黑暗", 2, 0, {}, ["car_found"], false],
	],
	"supermarket": [
		["checkout", "入口/收银区", "窗光", 1, 1, {"food": 2, "water": 1}, [], false],
		["food_aisle", "食品区", "窗光", 2, 1, {"food": 5}, [], false],
		["household", "日用品区", "窗光", 1, 0, {"materials": 2, "water": 2}, [], false],
		["storage", "仓储区", "黑暗", 2, 2, {"food": 3, "fuel": 1, "meds": 1}, [], false],
	],
	"school": [
		["class_a", "教室A", "窗光", 1, 1, {"materials": 2, "food": 1}, [], false],
		["class_b", "教室B", "窗光", 1, 2, {"water": 2, "meds": 1}, [], false],
		["library", "图书馆", "黑暗", 2, 1, {"fuel": 1}, ["childhood_memory"], false],
		["gym", "体育馆", "昏暗", 2, 3, {"food": 2, "materials": 3}, [], false],
	],
	"police": [
		["lobby", "大厅", "窗光", 1, 1, {"materials": 1}, ["address_known"], false],
		["office", "办公室", "窗光", 2, 0, {"parts": 1, "fuel": 1}, ["rebirth_clue_2"], false],
		["cell", "拘留室", "黑暗", 1, 0, {"food": 1}, ["crowbar_found"], false],
		["parking", "停车场", "窗光", 1, 1, {"battery": 1, "fuel": 1}, [], false],
	],
	"subway": [
		["entrance", "入口大厅", "昏暗", 1, 0, {"materials": 2, "water": 1}, [], false],
		["platform", "月台", "黑暗", 3, 4, {"fuel": 2, "parts": 1, "gasoline": 1}, ["rebirth_poster"], false],
		["duty_room", "值班室", "窗光", 1, 1, {"food": 1}, [], false],
	],
	"bridge_camp": [
		["camp_medical", "医疗站", "窗光", 1, 0, {}, [], false],
		["camp_fire", "篝火旁", "窗光", 1, 0, {}, [], false],
		["camp_guard", "岗哨", "窗光", 1, 0, {}, [], false],
	],
	"gas_station": [
		["store", "便利店(站内)", "窗光", 1, 1, {"food": 2, "water": 2}, [], false],
		["pump", "加油泵区", "窗光", 1, 0, {"fuel": 2, "gasoline": 2}, [], false],
		["underground", "地下油库", "黑暗", 2, 2, {"fuel": 3, "parts": 1}, [], false],
	],
	"hardware_store": [
		["storefront", "店面", "窗光", 1, 1, {"materials": 3, "parts": 1}, [], false],
		["shelves", "货架区", "窗光", 1, 1, {"materials": 3}, [], false],
		["upstairs", "二楼仓库", "昏暗", 2, 0, {"materials": 4, "parts": 2}, [], true],
	],
	"apartment": [
		["lobby", "一楼大厅", "昏暗", 1, 2, {"materials": 1}, [], false],
		["room_201", "201室", "窗光", 1, 0, {"food": 1, "water": 1}, [], false],
		["room_202", "202室", "窗光", 1, 1, {"meds": 1, "water": 1}, [], false],
		["room_301", "301室", "窗光", 1, 1, {"food": 2, "materials": 1}, [], false],
		["room_302", "302室", "黑暗", 2, 2, {"food": 1, "meds": 1}, ["apartment_letter"], false],
		["room_401", "401室", "窗光", 1, 0, {"food": 1, "water": 1, "materials": 1}, [], false],
		["room_402", "402室", "窗光", 1, 1, {"meds": 1}, ["qijin_apartment"], false],
		["room_501", "501室(有人!)", "窗光", 1, 0, {}, ["rebirth_insider"], true],
		["rooftop", "天台", "窗光", 1, 0, {}, [], false],
	],
	"safezone_edge": [
		["tower", "哨塔", "窗光", 1, 0, {}, ["lab_location"], false],
		["checkpoint", "检查站", "窗光", 1, 2, {"fuel": 1, "battery": 1}, [], false],
		["armory", "军械库(锁死)", "黑暗", 0, 0, {}, [], true],
	],
	"quarantine": [
		["isolation", "隔离大厅", "窗光", 1, 1, {"meds": 3, "materials": 1}, [], false],
		["specimen", "标本室", "黑暗", 2, 0, {}, ["qimian_file"], false],
		["lab", "化验室", "窗光", 1, 1, {"fuel": 1, "gasoline": 1}, [], false],
	],
}

static func default_rooms_for(location_id: String) -> Array:
	return [
		["front_store", "入口", "昏暗", 1, 0, {"materials": 1}, [], false],
		["back_room", "深处", "黑暗", 2, 1, {}, [], false],
	]

static func build_location(location_id: String) -> Dictionary:
	var def: Dictionary = LOCATION_DEFS.get(location_id, {"name": "未知", "ring": "?", "range": 1, "zombies": 0, "resources": {}, "resource_tendency": "?", "danger_level": "?", "route_time": 1, "road_condition": "?", "icons": []})
	var rooms := {}
	var room_list: Array = ROOM_DEFS.get(location_id, default_rooms_for(location_id))
	for r in room_list:
		rooms[r[0]] = {
			"name": r[1], "visibility": r[2], "search_time": r[3],
			"hidden_zombies": r[4], "resources": r[5].duplicate(true), "flags": r[6].duplicate(true),
			"locked": r[7], "searched": false
		}
	return {
		"name": def.name, "ring": def.ring, "range": def.range,
		"zombies": def.zombies, "resources": def.resources.duplicate(true),
		"resource_tendency": def.resource_tendency, "danger_level": def.danger_level,
		"route_time": def.route_time, "road_condition": def.road_condition,
		"icons": def.icons.duplicate(true),
		"qimian_trace": false, "rooms": rooms, "visited": false
	}
