using System.Collections.Generic;

namespace BeyondSafeZone.Data
{
    /// <summary>地点和房间数据 —— 对应 Godot locations.gd</summary>
    public static class LocationData
    {
        public static readonly Dictionary<string, string> ICON_LABELS = new()
        {
            {"home", "🏠 林行家"}, {"food", "🍞 食物"}, {"water", "💧 饮水"}, {"meds", "💊 药品"},
            {"parts", "🔧 零件"}, {"bike", "🚲 自行车"}, {"materials", "🧱 建材"}, {"fuel", "⛽ 燃料"},
            {"gasoline", "🛢️ 汽油"}, {"danger", "⚠️ 高风险"}, {"clue", "📋 撤离线索"},
            {"route", "🗺️ 路线信息"}, {"question", "❓ 异常痕迹"}, {"safezone", "🏕️ 保护区"},
            {"qimian", "👤 祁眠痕迹"}, {"npc", "🧑 幸存者"}
        };

        public static readonly Dictionary<string, string> ROAD_NOTES = new()
        {
            {"熟路", "无额外体力消耗"}, {"碎玻璃", "无额外体力消耗"},
            {"雨后湿滑", "疲劳+1"}, {"积水", "疲劳+1"}, {"堵塞", "疲劳+1"}, {"路障", "疲劳+1"},
            {"尸群迁移", "疲劳+2"}, {"封锁线", "疲劳+2"}
        };

        /// <summary>地点顶层定义</summary>
        public static readonly Dictionary<string, LocationDef> LOCATION_DEFS = new()
        {
            ["home"] = new() { Name="林行家", Ring="近圈", Range=1, Zombies=1, Resources=new(){{"food",1},{"water",2},{"materials",1}}, ResourceTendency="少量补给", DangerLevel="低", RouteTime=1, RoadCondition="熟路", Icons=new(){"home"} },
            ["convenience"] = new() { Name="小区便利店", Ring="近圈", Range=1, Zombies=3, Resources=new(){{"food",4},{"water",4},{"fuel",1}}, ResourceTendency="食物/水", DangerLevel="中", RouteTime=1, RoadCondition="碎玻璃", Icons=new(){"food","water"} },
            ["clinic"] = new() { Name="社区诊所", Ring="近圈", Range=1, Zombies=2, Resources=new(){{"meds",2},{"materials",1}}, ResourceTendency="药品", DangerLevel="中", RouteTime=1, RoadCondition="雨后湿滑", Icons=new(){"meds","question"} },
            ["bike_shop"] = new() { Name="自行车修理铺+车库", Ring="近圈", Range=1, Zombies=2, Resources=new(){{"parts",3},{"materials",2},{"tire",1}}, ResourceTendency="零件/建材/轮胎", DangerLevel="中", RouteTime=1, RoadCondition="堵塞", Icons=new(){"parts","bike","question"} },
            ["supermarket"] = new() { Name="超市", Ring="中圈", Range=2, Zombies=4, Resources=new(){{"food",8},{"water",4},{"materials",2}}, ResourceTendency="大量食物", DangerLevel="高", RouteTime=2, RoadCondition="尸群迁移", Icons=new(){"food","danger"} },
            ["school"] = new() { Name="废弃学校", Ring="中圈", Range=2, Zombies=3, Resources=new(){{"materials",4},{"fuel",1}}, ResourceTendency="建材/燃料", DangerLevel="中", RouteTime=2, RoadCondition="积水", Icons=new(){"materials"} },
            ["police"] = new() { Name="派出所", Ring="中圈", Range=2, Zombies=5, Resources=new(){{"fuel",2},{"materials",1},{"battery",1}}, ResourceTendency="燃料/线索/电瓶", DangerLevel="高", RouteTime=2, RoadCondition="路障", Icons=new(){"fuel","clue","question"} },
            ["subway"] = new() { Name="地铁口", Ring="中圈", Range=2, Zombies=5, Resources=new(){{"materials",2},{"fuel",1},{"parts",1},{"gasoline",1}}, ResourceTendency="路线线索/汽油", DangerLevel="高", RouteTime=2, RoadCondition="尸群迁移", Icons=new(){"route","question"} },
            ["bridge_camp"] = new() { Name="桥洞营地", Ring="中圈", Range=2, Zombies=1, Resources=new(), ResourceTendency="幸存者据点", DangerLevel="低", RouteTime=2, RoadCondition="熟路", Icons=new(){"npc"} },
            ["gas_station"] = new() { Name="加油站", Ring="中圈", Range=2, Zombies=4, Resources=new(){{"fuel",3},{"gasoline",2}}, ResourceTendency="大量燃料", DangerLevel="高", RouteTime=2, RoadCondition="尸群迁移", Icons=new(){"fuel","danger"} },
            ["hardware_store"] = new() { Name="五金店", Ring="中圈", Range=2, Zombies=3, Resources=new(){{"materials",6},{"parts",3}}, ResourceTendency="大量建材", DangerLevel="中", RouteTime=2, RoadCondition="堵塞", Icons=new(){"materials"} },
            ["apartment"] = new() { Name="废弃公寓", Ring="中圈", Range=2, Zombies=3, Resources=new(), ResourceTendency="混合物资/幸存者", DangerLevel="中", RouteTime=2, RoadCondition="积水", Icons=new(){"question","npc"} },
            ["safezone_edge"] = new() { Name="城市边缘哨卡", Ring="远圈", Range=3, Zombies=6, Resources=new(){{"fuel",2},{"battery",1},{"gasoline",1}}, ResourceTendency="燃料/电瓶/汽油", DangerLevel="极高", RouteTime=3, RoadCondition="封锁线", Icons=new(){"safezone","danger","bike"} },
            ["quarantine"] = new() { Name="防疫隔离站", Ring="远圈", Range=3, Zombies=4, Resources=new(){{"meds",3},{"fuel",1},{"gasoline",1}}, ResourceTendency="药品/线索", DangerLevel="高", RouteTime=3, RoadCondition="封锁线", Icons=new(){"meds","question"} }
        };

        /// <summary>房间定义（name, visibility, search_time, hidden_zombies, resources, flags, locked）</summary>
        public static readonly Dictionary<string, List<RoomDef>> ROOM_DEFS = new()
        {
            ["home"] = new()
            {
                new("living_room", "客厅", "窗光", 1, 0, new(){{"food",1},{"water",1},{"materials",1}}, new(){"plan_found"}, false),
                new("bedroom", "卧室", "窗光", 1, 0, new(), new(), false),
                new("kitchen", "厨房", "窗光", 1, 0, new(){{"food",1},{"water",1}}, new(), false),
                new("storage", "储物间", "黑暗", 2, 1, new(){{"materials",3},{"parts",2}}, new(), false),
            },
            ["convenience"] = new()
            {
                new("storefront", "店面", "窗光", 1, 0, new(){{"food",3},{"water",2}}, new(), false),
                new("warehouse", "仓库夹层", "黑暗", 2, 1, new(){{"food",2},{"materials",1}}, new(){"safezone_hint_1"}, false),
            },
            ["clinic"] = new()
            {
                new("waiting", "候诊室", "窗光", 1, 0, new(){{"water",2},{"materials",1}}, new(), false),
                new("exam_a", "诊室A", "窗光", 1, 1, new(){{"meds",2}}, new(){"rebirth_clue_1"}, false),
                new("pharmacy", "药房", "黑暗", 2, 2, new(){{"meds",4}}, new(), false),
                new("back_alley", "后门垃圾区", "窗光", 1, 0, new(){{"materials",1},{"food",1}}, new(), false),
            },
            ["bike_shop"] = new()
            {
                new("storefront", "店面", "窗光", 1, 0, new(){{"parts",3},{"materials",1},{"tire",1}}, new(), false),
                new("workshop", "工作间", "窗光", 1, 1, new(){{"parts",2},{"fuel",1}}, new(), false),
                new("backyard", "后院", "窗光", 1, 0, new(){{"materials",2}}, new(), false),
                new("garage", "车库", "黑暗", 2, 0, new(), new(){"car_found"}, false),
            },
            ["supermarket"] = new()
            {
                new("checkout", "入口/收银区", "窗光", 1, 1, new(){{"food",2},{"water",1}}, new(), false),
                new("food_aisle", "食品区", "窗光", 2, 1, new(){{"food",5}}, new(), false),
                new("household", "日用品区", "窗光", 1, 0, new(){{"materials",2},{"water",2}}, new(), false),
                new("storage", "仓储区", "黑暗", 2, 2, new(){{"food",3},{"fuel",1},{"meds",1}}, new(), false),
            },
            ["school"] = new()
            {
                new("class_a", "教室A", "窗光", 1, 1, new(){{"materials",2},{"food",1}}, new(), false),
                new("class_b", "教室B", "窗光", 1, 2, new(){{"water",2},{"meds",1}}, new(), false),
                new("library", "图书馆", "黑暗", 2, 1, new(){{"fuel",1}}, new(){"childhood_memory"}, false),
                new("gym", "体育馆", "昏暗", 2, 3, new(){{"food",2},{"materials",3}}, new(), false),
            },
            ["police"] = new()
            {
                new("lobby", "大厅", "窗光", 1, 1, new(){{"materials",1}}, new(){"address_known"}, false),
                new("office", "办公室", "窗光", 2, 0, new(){{"parts",1},{"fuel",1}}, new(){"rebirth_clue_2"}, false),
                new("cell", "拘留室", "黑暗", 1, 0, new(){{"food",1}}, new(){"crowbar_found"}, false),
                new("parking", "停车场", "窗光", 1, 1, new(){{"battery",1},{"fuel",1}}, new(), false),
            },
            ["subway"] = new()
            {
                new("entrance", "入口大厅", "昏暗", 1, 0, new(){{"materials",2},{"water",1}}, new(), false),
                new("platform", "月台", "黑暗", 3, 4, new(){{"fuel",2},{"parts",1},{"gasoline",1}}, new(){"rebirth_poster"}, false),
                new("duty_room", "值班室", "窗光", 1, 1, new(){{"food",1}}, new(), false),
            },
            ["bridge_camp"] = new()
            {
                new("camp_medical", "医疗站", "窗光", 1, 0, new(), new(), false),
                new("camp_fire", "篝火旁", "窗光", 1, 0, new(), new(), false),
                new("camp_guard", "岗哨", "窗光", 1, 0, new(), new(), false),
            },
            ["gas_station"] = new()
            {
                new("store", "便利店(站内)", "窗光", 1, 1, new(){{"food",2},{"water",2}}, new(), false),
                new("pump", "加油泵区", "窗光", 1, 0, new(){{"fuel",2},{"gasoline",2}}, new(), false),
                new("underground", "地下油库", "黑暗", 2, 2, new(){{"fuel",3},{"parts",1}}, new(), false),
            },
            ["hardware_store"] = new()
            {
                new("storefront", "店面", "窗光", 1, 1, new(){{"materials",3},{"parts",1}}, new(), false),
                new("shelves", "货架区", "窗光", 1, 1, new(){{"materials",3}}, new(), false),
                new("upstairs", "二楼仓库", "昏暗", 2, 0, new(){{"materials",4},{"parts",2}}, new(), true),
            },
            ["apartment"] = new()
            {
                new("lobby", "一楼大厅", "昏暗", 1, 2, new(){{"materials",1}}, new(), false),
                new("room_201", "201室", "窗光", 1, 0, new(){{"food",1},{"water",1}}, new(), false),
                new("room_202", "202室", "窗光", 1, 1, new(){{"meds",1},{"water",1}}, new(), false),
                new("room_301", "301室", "窗光", 1, 1, new(){{"food",2},{"materials",1}}, new(), false),
                new("room_302", "302室", "黑暗", 2, 2, new(){{"food",1},{"meds",1}}, new(){"apartment_letter"}, false),
                new("room_401", "401室", "窗光", 1, 0, new(){{"food",1},{"water",1},{"materials",1}}, new(), false),
                new("room_402", "402室", "窗光", 1, 1, new(){{"meds",1}}, new(){"qijin_apartment"}, false),
                new("room_501", "501室(有人!)", "窗光", 1, 0, new(), new(){"rebirth_insider"}, true),
                new("rooftop", "天台", "窗光", 1, 0, new(), new(), false),
            },
            ["safezone_edge"] = new()
            {
                new("tower", "哨塔", "窗光", 1, 0, new(), new(){"lab_location"}, false),
                new("checkpoint", "检查站", "窗光", 1, 2, new(){{"fuel",1},{"battery",1}}, new(), false),
                new("armory", "军械库(锁死)", "黑暗", 0, 0, new(), new(), true),
            },
            ["quarantine"] = new()
            {
                new("isolation", "隔离大厅", "窗光", 1, 1, new(){{"meds",3},{"materials",1}}, new(), false),
                new("specimen", "标本室", "黑暗", 2, 0, new(), new(){"qimian_file"}, false),
                new("lab", "化验室", "窗光", 1, 1, new(){{"fuel",1},{"gasoline",1}}, new(), false),
            },
        };

        public static List<RoomDef> DefaultRoomsFor(string locationId) => new()
        {
            new("front_store", "入口", "昏暗", 1, 0, new(){{"materials",1}}, new(), false),
            new("back_room", "深处", "黑暗", 2, 1, new(), new(), false),
        };

        /// <summary>构建地点状态</summary>
        public static Model.LocationState BuildLocation(string locationId)
        {
            var def = LOCATION_DEFS.GetValueOrDefault(locationId, new LocationDef());
            var rooms = new Dictionary<string, Model.RoomState>();
            var roomList = ROOM_DEFS.GetValueOrDefault(locationId, DefaultRoomsFor(locationId));

            foreach (var rd in roomList)
            {
                rooms[rd.Id] = new Model.RoomState
                {
                    Name = rd.Name,
                    Visibility = rd.Visibility,
                    SearchTime = rd.SearchTime,
                    HiddenZombies = rd.HiddenZombies,
                    Resources = new Dictionary<string, int>(rd.Resources),
                    Flags = new List<string>(rd.Flags),
                    Locked = rd.Locked,
                    Searched = false
                };
            }

            return new Model.LocationState
            {
                Name = def.Name,
                Ring = def.Ring,
                Range = def.Range,
                Zombies = def.Zombies,
                Resources = new Dictionary<string, int>(def.Resources),
                ResourceTendency = def.ResourceTendency,
                DangerLevel = def.DangerLevel,
                RouteTime = def.RouteTime,
                RoadCondition = def.RoadCondition,
                Icons = new List<string>(def.Icons),
                QimianTrace = false,
                Rooms = rooms,
                Visited = false
            };
        }
    }

    /// <summary>地点顶层定义</summary>
    public class LocationDef
    {
        public string Name = "未知";
        public string Ring = "?";
        public int Range = 1;
        public int Zombies = 0;
        public Dictionary<string, int> Resources = new();
        public string ResourceTendency = "?";
        public string DangerLevel = "?";
        public int RouteTime = 1;
        public string RoadCondition = "?";
        public List<string> Icons = new();
    }

    /// <summary>房间定义</summary>
    public class RoomDef
    {
        public string Id;
        public string Name;
        public string Visibility;
        public int SearchTime;
        public int HiddenZombies;
        public Dictionary<string, int> Resources;
        public List<string> Flags;
        public bool Locked;

        public RoomDef(string id, string name, string visibility, int searchTime,
            int hiddenZombies, Dictionary<string, int> resources, List<string> flags, bool locked)
        {
            Id = id; Name = name; Visibility = visibility; SearchTime = searchTime;
            HiddenZombies = hiddenZombies; Resources = resources; Flags = flags; Locked = locked;
        }
    }
}
