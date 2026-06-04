# 《保护区之外》 Godot → Unity 迁移计划

创建日期：2026-06-02

## 0. 迁移概述

- **源项目**: `E:\Download\working\BeyondSafeZone` (Godot 4.6.2)
- **目标项目**: `E:\Download\working\BeyondSafeZoneUnity` (Unity 2D)
- **迁移策略**: 全量迁移，Godot 项目作为参考保留，不删除
- **最终目标**: 可运行的 Unity 2D 灰盒 Demo，接入 PlayKit.ai Unity SDK

---

## 1. 文件对应映射

### 1.1 源文件 → 目标文件

| Godot 源文件 | Unity 目标文件 | 说明 |
|---|---|---|
| `game/scripts/model/game_state.gd` | `Assets/Scripts/Model/GameState.cs` | 游戏状态数据类 |
| `game/scripts/core/game_simulation.gd` | `Assets/Scripts/Core/GameSimulation.cs` | 流程协调器 |
| `game/scripts/controller/exploration_controller.gd` | `Assets/Scripts/Controllers/ExplorationController.cs` | 探索系统 |
| `game/scripts/controller/shelter_controller.gd` | `Assets/Scripts/Controllers/ShelterController.cs` | 据点设施 |
| `game/scripts/controller/night_controller.gd` | `Assets/Scripts/Controllers/NightController.cs` | 夜晚结算 |
| `game/scripts/controller/car_controller.gd` | `Assets/Scripts/Controllers/CarController.cs` | 汽车系统 |
| `game/scripts/controller/qimian_controller.gd` | `Assets/Scripts/Controllers/QimianController.cs` | 祁眠 AI |
| `game/scripts/data/constants.gd` | `Assets/Scripts/Data/Constants.cs` | 全局常量 |
| `game/scripts/data/balance.gd` | `Assets/Scripts/Data/BalanceData.cs` | 数值配置 |
| `game/scripts/data/events_15d.gd` | `Assets/Scripts/Data/EventsData.cs` | 15天事件 |
| `game/scripts/data/locations.gd` | `Assets/Scripts/Data/LocationData.cs` | 地点/房间 |
| `game/scripts/data/facilities.gd` | `Assets/Scripts/Data/FacilityData.cs` | 设施 |
| `game/scripts/data/qimian_plan.gd` | `Assets/Scripts/Data/QimianPlanData.cs` | 祁眠日程 |
| `game/scripts/data/safe_route.gd` | `Assets/Scripts/Data/SafeRouteData.cs` | 安全路线 |
| `game/scripts/view/text_renderer.gd` | `Assets/Scripts/UI/TextRenderer.cs` | 文本渲染 |
| `game/scripts/main.gd` | `Assets/Scripts/UI/MainUI.cs` | 主界面 |
| `game/scenes/main.tscn` | `Assets/Scenes/Main.unity` | 主场景 |
| `game/tests/test_game_simulation.gd` | `Assets/Tests/TestGameSimulation.cs` | 测试 |

---

## 2. 类型映射

### 2.1 Godot Dictionary → C# Class

**GameState 顶层字段：**

| Godot 字段 | GDScript 类型 | C# 类型 | C# 字段名 |
|---|---|---|---|
| `day` | `var day: int = 1` | `int` | `Day` |
| `phase` | `var phase: String = "morning"` | `string` | `Phase` |
| `goal` | `var goal: String` | `string` | `Goal` |
| `demo_complete` | `var demo_complete: bool` | `bool` | `DemoComplete` |
| `ending_state` | `var ending_state: String` | `string` | `EndingState` |
| `last_event` | `var last_event: String` | `string` | `LastEvent` |
| `morning_context` | `var morning_context: Dictionary` | `MorningContext` | `MorningContext` |
| `applied_day_events` | `var applied_day_events: Array` | `List<int>` | `AppliedDayEvents` |
| `state.lin` | `Dictionary` (health/hunger/...) | `LinState` | `Lin` |
| `state.resources` | `Dictionary` (food/water/...) | `ResourceState` | `Resources` |
| `state.shelter` | `Dictionary` (door/noise/...) | `ShelterState` | `Shelter` |
| `state.bike` | `Dictionary` (durability/...) | `BikeState` | `Bike` |
| `state.car` | `Dictionary` (found/ready/...) | `CarState` | `Car` |
| `state.car_parts` | `Dictionary` (battery/...) | `CarPartsState` | `CarParts` |
| `state.evacuation` | `Dictionary` (4 flags) | `EvacuationState` | `Evacuation` |
| `state.locations` | `Dictionary<string, Dict>` | `Dictionary<string, LocationState>` | `Locations` |
| `state.exploration` | `Dictionary` (6 fields) | `ExplorationState` | `Exploration` |
| `state.qimian` | `Dictionary` (awake/log/...) | `QimianState` | `Qimian` |
| `state.anomaly_dossier` | `Array` | `List<AnomalyDossierEntry>` | `AnomalyDossier` |
| `state.player_marks` | `Dictionary` | `Dictionary<string, PlayerMark>` | `PlayerMarks` |

### 2.2 子状态类型

**LinState（林行状态）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `health` | `Health` | `int` | 10 |
| `hunger` | `Hunger` | `int` | 0 |
| `thirst` | `Thirst` | `int` | 0 |
| `fatigue` | `Fatigue` | `int` | 1 |
| `stress` | `Stress` | `int` | 2 |
| `infection_risk` | `InfectionRisk` | `int` | 0 |
| `hope` | `Hope` | `int` | 4 |

**ResourceState（六类资源 + 汽车零件）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `food` | `Food` | `int` | 5 |
| `water` | `Water` | `int` | 5 |
| `meds` | `Meds` | `int` | 2 |
| `materials` | `Materials` | `int` | 4 |
| `parts` | `Parts` | `int` | 1 |
| `fuel` | `Fuel` | `int` | 3 |

**CarPartsState（汽车零件）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `battery` | `Battery` | `int` | 0 |
| `gasoline` | `Gasoline` | `int` | 0 |
| `tire` | `Tire` | `int` | 0 |

**CarState（汽车修理状态）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `found` | `Found` | `bool` | false |
| `ready` | `Ready` | `bool` | false |
| `step_engine` | `StepEngine` | `bool` | false |
| `step_tire` | `StepTire` | `bool` | false |
| `step_battery` | `StepBattery` | `bool` | false |
| `step_fueled` | `StepFueled` | `bool` | false |
| `breakdown` | `Breakdown` | `string` | "" |

**ShelterState（据点状态）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `door` | `Door` | `int` | 4 |
| `noise` | `Noise` | `int` | 2 |
| `scent` | `Scent` | `int` | 2 |
| `light` | `Light` | `int` | 2 |
| `defense` | `Defense` | `int` | 1 |
| `escape` | `Escape` | `int` | 0 |
| `supply_preservation` | `SupplyPreservation` | `int` | 0 |
| `facilities` | `Facilities` | `Dictionary<string, FacilityState>` | 5设施 |

**BikeState（自行车状态）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `durability` | `Durability` | `int` | 6 |
| `capacity` | `Capacity` | `int` | 6 |
| `range` | `Range` | `int` | 1 |
| `noise` | `Noise` | `int` | 1 |

**EvacuationState（撤离旗标）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `safezone_confirmed` | `SafezoneConfirmed` | `bool` | false |
| `address_known` | `AddressKnown` | `bool` | false |
| `car_ready` | `CarReady` | `bool` | false |
| `bike_ready` | `BikeReady` | `bool` | false |

**ExplorationState（探索状态）：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `active_location` | `ActiveLocation` | `string` | "" |
| `time_used` | `TimeUsed` | `int` | 0 |
| `time_limit` | `TimeLimit` | `int` | 0 |
| `noise` | `Noise` | `int` | 0 |
| `searched_rooms` | `SearchedRooms` | `List<string>` | [] |
| `lured_rooms` | `LuredRooms` | `List<string>` | [] |

**QimianState（祁眠状态）：**

| Godot 字段 | C# 字段 | 类型 |
|---|---|---|
| `awake` | `Awake` | `bool` |
| `log` | `Log` | `List<QimianLogEntry>` |
| `public_clues` | `PublicClues` | `List<string>` |
| `personality_card` | `PersonalityCard` | `PersonalityCardState` |
| `ai_state` | `AiState` | `QimianAiState` |

**QimianAiState：**

| Godot 字段 | C# 字段 | 类型 | 初始值 |
|---|---|---|---|
| `exposure` | `Exposure` | `int` | 0 |
| `moto_tier` | `MotoTier` | `int` | 1 |
| `zone_heat` | `ZoneHeat` | `Dictionary<string, int>` | {A:0, B:0, C:0} |
| `qijin_clues` | `QijinClues` | `int` | 0 |
| `rescued_npc` | `RescuedNpc` | `List<string>` | [] |
| `inventory` | `Inventory` | `ResourceState` | {food:0, water:0, medicine:1, materials:0, parts:1, fuel:1} |

**LocationState（地点状态）：**

| Godot 字段 | C# 字段 | 类型 |
|---|---|---|
| `name` | `Name` | `string` |
| `ring` | `Ring` | `string` |
| `range` | `Range` | `int` |
| `zombies` | `Zombies` | `int` |
| `resources` | `Resources` | `Dictionary<string, int>` |
| `resource_tendency` | `ResourceTendency` | `string` |
| `danger_level` | `DangerLevel` | `string` |
| `route_time` | `RouteTime` | `int` |
| `road_condition` | `RoadCondition` | `string` |
| `icons` | `Icons` | `List<string>` |
| `qimian_trace` | `QimianTrace` | `bool` |
| `rooms` | `Rooms` | `Dictionary<string, RoomState>` |
| `visited` | `Visited` | `bool` |

**RoomState（房间状态）：**

| Godot 字段 | C# 字段 | 类型 |
|---|---|---|
| `name` | `Name` | `string` |
| `visibility` | `Visibility` | `string` |
| `search_time` | `SearchTime` | `int` |
| `hidden_zombies` | `HiddenZombies` | `int` |
| `resources` | `Resources` | `Dictionary<string, int>` |
| `flags` | `Flags` | `List<string>` |
| `locked` | `Locked` | `bool` |
| `searched` | `Searched` | `bool` |

### 2.3 集合类型映射

| Godot | C# |
|---|---|
| `Dictionary` | `System.Collections.Generic.Dictionary<TKey, TValue>` |
| `Array` | `System.Collections.Generic.List<T>` |
| `String` | `string` |
| `int` | `int` |
| `bool` | `bool` |
| `float` | `float` |
| `Callable` | `System.Action` / `Func<...>` |
| `static func` | `public static <return> Method(...)` |
| `extends RefCounted` | `public class ClassName` |
| `class_name` | 命名空间 + public class |

---

## 3. 模块迁移顺序表

| 阶段 | 模块 | 源 | 目标 | 优先级 |
|---|---|---|---|---|
| 2 | Model 层 | `model/game_state.gd` | `Model/GameState.cs` | P0 |
| 2 | Data - Constants | `data/constants.gd` | `Data/Constants.cs` | P0 |
| 2 | Data - Balance | `data/balance.gd` | `Data/BalanceData.cs` | P0 |
| 2 | Data - Events | `data/events_15d.gd` | `Data/Events15dData.cs` | P0 |
| 2 | Data - Locations | `data/locations.gd` | `Data/LocationData.cs` | P0 |
| 2 | Data - Facilities | `data/facilities.gd` | `Data/FacilityData.cs` | P0 |
| 2 | Data - Qimian Plan | `data/qimian_plan.gd` | `Data/QimianPlanData.cs` | P0 |
| 2 | Data - Safe Route | `data/safe_route.gd` | `Data/SafeRouteData.cs` | P0 |
| 3 | Controller - Exploration | `controller/exploration_controller.gd` | `Controllers/ExplorationController.cs` | P0 |
| 3 | Controller - Shelter | `controller/shelter_controller.gd` | `Controllers/ShelterController.cs` | P0 |
| 3 | Controller - Night | `controller/night_controller.gd` | `Controllers/NightController.cs` | P0 |
| 3 | Controller - Car | `controller/car_controller.gd` | `Controllers/CarController.cs` | P0 |
| 3 | Controller - Qimian | `controller/qimian_controller.gd` | `Controllers/QimianController.cs` | P0 |
| 3 | Core - GameSimulation | `core/game_simulation.gd` | `Core/GameSimulation.cs` | P0 |

---

## 4. 关键逻辑对应

### 4.1 new_game() → NewGame()
- GDScript: `func new_game() -> _GameState`
- C#: `public static GameState NewGame()`
- 行为：创建所有子状态对象，初始化值为 BalanceData 常量值，调用 `_build_locations()`，调用 `start_day(1)`

### 4.2 start_day(day) → StartDay(int day)
- GDScript: `func start_day(day: int) -> String`
- C#: `public static string StartDay(GameState state, int day)`
- 行为：设置 day、phase="morning"，获取当日 event 并填充 morning_context，应用 day pressure modifiers

### 4.3 sleep_and_resolve_night() → SleepAndResolveNight()
- GDScript: `func sleep_and_resolve_night() -> String`
- C#: `public static string SleepAndResolveNight(GameState state)`
- 行为：调用 QimianController.ResolveForDay → 调用 NightController.Resolve
- NightController.Resolve 内部：消耗资源 → 噪音传播 → 感染恶化 → 血月/红潮结算 → day>=15 结局判定 → 或 start_next_day(day+1)

---

## 5. 数值常量全部保留

所有 `balance.gd` 中的数值常量原样迁移为 C# `public const` 或 `public static readonly`：

| 类别 | 常量数 | 示例 |
|---|---|---|
| 初始资源 | 6 | `INIT_RESOURCES` |
| 林行初始状态 | 7 | `INIT_LIN` |
| 据点初始状态 | 7 | `INIT_SHELTER` |
| 自行车初始状态 | 4 | `INIT_BIKE` |
| 汽车初始状态 | 7 | `INIT_CAR` |
| 汽车零件初始 | 3 | `INIT_CAR_PARTS` |
| 每日消耗 | 2 | `DAILY_CONSUME` |
| 饥饿/口渴惩罚 | 2 | `HUNGER_PER_DAY_NO_FOOD` |
| 感染公式 | 3 | `INFECTION_CRITICAL_THRESHOLD` |
| 血月公式 | 8 | `BM_BASE_PRESSURE` 等 |
| 红潮公式 | 8 | `RT_DAY_OFFSET` 等 |
| 结局阈值 | 6 | `ENDING_HEALTH_DEAD` 等 |
| 汽车修理成本 | 7 | `CAR_REPAIR_ENGINE_MATERIALS` 等 |
| 设施行动成本 | 22 | `SHELTER_REST_FATIGUE` 等 |
| 探索数值 | 7 | `SEARCH_MAX_PER_RESOURCE` 等 |
| 祁眠 AI 数值 | 11 | `QIMIAN_AWAKE_DAY` 等 |

---

## 6. 15天事件表（全量迁移）

`events_15d.gd` 中的 EVENTS Dictionary 保持结构不变，逐条迁移为 C# `Dictionary<int, DayEvent>`：

每项 DayEvent 包含：
- `Day` (int)
- `MorningText` (string)
- `PressureType` (string)
- `Clue` (string)
- `BloodMoonWarning` (string)
- `Modifiers` (Dictionary<string, int>)

---

## 7. 14 地点 + 40+ 房间（全量迁移）

`locations.gd` 中：
- `LOCATION_DEFS` (14 条顶层数据) → `Dictionary<string, LocationDef>`
- `ROOM_DEFS` (14×N 条房间数据) → `Dictionary<string, List<RoomDef>>`
- `ICON_LABELS` (chi → emoji) → `Dictionary<string, string>`
- `ROAD_NOTES` (路况 → 描述) → `Dictionary<string, string>`

---

## 8. 祁眠固定日程（全量迁移）

`qimian_plan.gd` 中 PLAN Dictionary 不变，逐条迁移：

每天多条 action，每条含：
- `title` / `location` / `resource` / `amount` / `zombie_delta` / `resource_gain` / `public_clue` / `truth` / `ai_replay` / `subjective_fragment` / `blood_moon_support`

---

## 9. 15天安全演示路线（全量迁移）

`safe_route.gd` 中：
- `DAY_LOCATION`: 15 天每天对应地点
- `DAY_SHELTER_ACTION`: 固定修车日 (10,12,13,14 → workbench_car)
- `CONDITIONAL_ACTIONS`: 条件规则 (血月日加固/3倍日广播/2倍日修车/fallback 静默)

---

## 10. PlayKit.ai 接入计划（阶段 6）

### 10.1 SDK 安装
- Git URL: `https://gitlab.com/playkit-ai/playkit-unitysdk.git?path=Packages/ai.playkit.sdk`
- 通过 Unity Package Manager → Add package from git URL

### 10.2 使用边界
PlayKit.ai **只能**用于：
1. 异常档案文本生成（基于 `anomaly_tag` 和 `evidence_text` 扩写叙事文本）
2. 祁眠日志文本增强（基于 `ai_replay` 生成更丰富的 `subjective_fragment`）
3. NPC/广播/独白文本增强

PlayKit.ai **不能**用于：
- 资源结算
- 伤害计算
- 结局判定
- 行动合法性检查
- 任何核心规则逻辑

### 10.3 容错设计
- 每次 PlayKit 调用都需要 try-catch + timeout
- 失败时使用本地兜底文本（即当前 Godot 中已有的固定文本）
- PlayKit 不可用时 Demo 必须仍能完整跑完 Day 1-15

### 10.4 安全规则
- Developer Token 不得写入代码或仓库
- 使用环境变量或 Unity Editor 的 ScriptableObject 配置存储（.gitignore）

---

## 11. 测试对照

`test_game_simulation.gd` 中 28 个测试方法 → Unity Test Runner `[Test]` 方法：

| Godot 测试 | Unity 测试 | 验证内容 |
|---|---|---|
| `_test_initial_goal_and_stats` | `TestInitialGoalAndStats` | Day 1 初始化 |
| `_test_lin_condition_text_reports_infection_stage` | `TestLinConditionText` | 感染阶段 |
| `_test_resources_and_evacuation_flags` | `TestResourcesAndEvacuation` | 资源和撤离旗标 |
| `_test_shelter_facilities_exist` | `TestShelterFacilities` | 五大设施 |
| `_test_day_event_table_and_morning_context` | `TestDayEventTable` | 15天事件 |
| `_test_blood_moon_days` | `TestBloodMoonDays` | 血月日 |
| ...其余 22 个测试... | ...对应 Unity Tests... | ... |

---

## 12. 已知风险和阻塞项

| 风险 | 说明 | 对策 |
|---|---|---|
| PlayKit.ai SDK API 不确定 | 文档名/方法名需要在接入时确认 | 阶段6前暂停，记录需补充的信息 |
| 测试框架差异 | Godot 用 `extends SceneTree`，Unity 用 Test Runner | 需要安装 Unity Test Framework package |
| UI 框架差异 | Godot 用 Control nodes，Unity 用 Canvas/UGUI | 阶段1搭建基础 UGUI 结构 |

---

## 13. 不迁移的内容

以下 Godot 特有内容不迁移，仅在 Unity 中用等效方式重建：
- `.tscn` 场景文件（重新创建 Main.unity）
- `extends SceneTree` 测试基类（改用 `[TestFixture]`）
- Godot Signal 系统（改用 C# event/Action）
- Godot preload 资源路径（改用 C# 直接引用或 Resources）
- `.uid` 缓存文件
