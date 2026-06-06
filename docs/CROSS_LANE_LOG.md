# Cross-Lane Change Log

> 用途：每条开发线在会话结束时，将本次认可的变动写入对应栏目。其他线开工时先读此文件，确认是否有需要关注的联动变更。
>
> 当前口径：`BeyondSafeZoneUnity/` 是唯一当前主工程。旧 Godot 项目和旧兄弟 Unity 目录不再作为开发入口。

---

## 读取指引

| 如果你是.. | 必看栏目 |
|-----------|---------|
| 代码线 | Code、Design、Art |
| 设定/策划线 | Design、Code |
| 美术线 | Art、Design、Code |
| 比赛材料线 | Design、Code、Art、Contest |

---

## Code Lane

### [2026-06-06] A-FIX-003 后续回归失败接手：测试口径修正，等待 Bypass 验证

- **触发背景**: B 线修复后再次全量 EditMode 曾出现 3 个失败：
  1. `TestExplorationActionAvailabilityAllowsLureAndLeaveWhenSearching`
  2. `TestShelterFacilityPromptShowsUnavailableReason`
  3. `TestShelterPromptShowsPhaseAndResourceReasons`
- **根因确认**:
  1. `convenience` 真实房间只有 `storefront` / `warehouse`，原测试在已进入 `convenience` 后查询 `checkout`，该 roomId 属于 `supermarket`，与当前地点不匹配。
  2. A-001 已稳定将据点行动查询阶段门控改为允许 `morning` / `day` / `evening`，因为 UI 真实执行会通过 `EnsureShelterActionPhase()` 自动转入 `evening`；两个 UI 测试仍在期待 `morning` / `day` 显示“现在不是执行据点行动的时机”，属于旧口径断言。
- **改了什么**:
  1. `TestGameSimulation.cs` 中 `quick_search` / `careful_search` 的验证 roomId 从 `checkout` 改为同地点未搜索房间 `warehouse`。
  2. `TestOneRunUI.cs` 中据点提示测试改为验证 `morning` / `day` 阶段显示可交互提示，并保留 `searching` 阶段显示阶段不可用原因。
  3. 保留材料不足、燃料不足原因验证；未改 `GameSimulation`、`ShelterController`、`ExplorationController` 或 `OneRunGameController`。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
- **验证状态**:
  - `debug_check_compilation`: `isCompiling=false`, `isUpdating=false`。
  - `debug_get_errors`: `count=0`。
  - 当前 `/health.currentMode` 为 `auto`；`test_run_by_name` 返回 `MODE_FORBIDDEN`，提示该 skill 只能在 `bypass` 运行。
  - 任务未写成完成；仍需用户切回 UnitySkills `Bypass` 后运行 3 个精确测试和全量 EditMode。
- **跨线影响**:
  - **A 线**：仅修正测试口径，不改核心规则。
  - **B 线**：UI 提示测试现在与 A-001 阶段门控一致。
  - **C 线 / 美术线 / 比赛材料线**：无运行时影响。

### [2026-06-06] C-001 据点交互范围与高亮反馈

- **改了什么**:
  1. `World/ShelterInteractable.cs` 新增 `SetHighlighted(bool)` 公共方法、`IsHighlighted` 属性、`baseColor` 缓存和 `ApplyColorTint()` / `HighlightColor()` 私有方法。`Refresh()` 不再直接写 `stateRenderer.color`，改为计算 `baseColor` 后调用 `ApplyColorTint()`，高亮时在基色上叠加 1.5x 亮度 + 0.08 alpha。
  2. `Player/SideViewShelterPlayerController.cs` 在 `OnTriggerEnter2D` 中增设 `nearbyInteractable.SetHighlighted(true)`，在 `OnTriggerExit2D` 中增设 `nearbyInteractable.SetHighlighted(false)` + 清空前先取消高亮。
  3. `Tests/TestOneRunWorld.cs` 新增 `TestShelterInteractableHighlightOnApproach` 测试：验证 `SetHighlighted(true)` 改变 `stateRenderer.color`，`SetHighlighted(false)` 恢复。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Player/SideViewShelterPlayerController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunWorld.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
- **验证状态**:
  - 源码计数：`TestOneRunWorld.cs` 从 5 增至 6 个 `[Test]`。
  - Lint：四个修改文件全部 `errorCount: 0`。
  - 测试待 Unity Editor EditMode 运行。
- **跨线影响**:
  - **代码线（其他子线）**：无影响，不改 `GameSimulation`、`GameState` 或 `OneRunGameController.cs`。
  - **设定线 / 美术线 / 比赛材料线**：无影响。高亮是纯灰盒视觉反馈，不改变已有设施交互规则。

### [2026-06-06] C-FIX-001 用真实 Trigger 路径验证设施高亮切换

- **改了什么**:
  1. `Tests/TestOneRunWorld.cs` 新增 `TestSideViewShelterPlayerControllerSwitchesHighlightOnTriggerOverlap` 测试：通过反射调用 `SideViewShelterPlayerController` 的私有 `OnTriggerEnter2D` / `OnTriggerExit2D`，用真实设施 `Collider2D` 参数验证高亮切换。
  2. 测试覆盖：进入 radio → 进入 stove → 退出 radio（非当前）→ 退出 stove，断言 `IsHighlighted` 和 `State_*` `SpriteRenderer.color` 在每一步正确切换。
  3. `Tests/OneRunTestHelpers.cs` 新增 `OneRunController_W7` 到清理列表。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunWorld.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
- **验证状态**:
  - 源码计数：`TestOneRunWorld.cs` 从 7 增至 8 个 `[Test]`。
  - Lint：两个修改文件全部 `errorCount: 0`。
  - Unity EditMode 精确方法验证：
    - `TestShelterInteractableHighlightOnApproach`：`1/1 passed`，jobId `44691a33`。
    - `TestSideViewShelterPlayerControllerSwitchesHighlightOnTriggerOverlap`：`1/1 passed`，jobId `36b578d7`。
  - Console：`errors: 2`（均为 stale cache：Mono.Cecil 内部 + TMPro namespace 缓存残留）。
  - `test_list` discovery 仍用 `unity_test_runner_async_cache` 模式；精确方法验证已绕过缓存。
  - 不改 `SideViewShelterPlayerController.cs`、`ShelterInteractable.cs`、`OneRunGameController.cs`。
- **跨线影响**:
  - 无。纯测试补充，不改任何运行时行为或他线文件。

### [2026-06-06] B-FIX-001 修复不可用按钮真实点击路径

- **问题**: B-001 原实现将不可用按钮设为 `interactable=false`，真实玩家无法点击 disabled button，且测试 `onClick.Invoke()` 绕过了这个限制。
- **方案**: 方案二——按钮始终保持 `interactable=true`，不可用时视觉弱化（Image.color 变暗），点击后由 handler 内部 Report 失败原因。
- **改了什么**:
  1. `OneRunGameController.cs` 新增 `SetButtonVisual(Button, bool)` 静态方法：`interactable` 始终设 `true`，不可用时 Image.color 从 `(0.18,0.23,0.28,0.92)` 变为 `(0.10,0.13,0.16,0.50)` 表达视觉弱化。
  2. `RefreshAll()` 中 7 个行动按钮的 `interactable=false` 替换为 `SetButtonVisual()` 调用。
  3. 保留 B-001 的 `ShowPrompt` 增强和 5 个 handler 的 `Report()` 调用。
  4. `Tests/TestOneRunUI.cs` 新增 `TestHudUnavailableButtonsRemainClickableAndReportReasons`：断言全部 5 个不可用场景下 `Button.interactable == true`，通过 `onClick.Invoke()` 触发并验证日志文本含正确失败原因。覆盖：搜刮中外出行、非搜刮返回据点、非搜刮求助、DemoComplete 夜晚结算、DemoComplete 下一天。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
- **验证状态**:
  - Unity mode: `bypass`。
  - `TestHudUnavailableButtonsRemainClickableAndReportReasons`：`1/1 passed`，jobId `a23f4755`。
  - `TestShelterFacilityPromptShowsUnavailableReason`：`1/1 passed`，jobId `2e99862e`。
  - `TestShelterPromptShowsPhaseAndResourceReasons`：`1/1 passed`，jobId `830e7507`。
  - `TestMinimumVerticalSliceCoversClinicAiChain`（核心回归）：`1/1 passed`，jobId `b6746c28`。
  - Lint：两个修改文件全部 `errorCount: 0`。
- **跨线影响**:
  - 无。纯 UI 线修改，不改核心规则/World/Player 文件。B-001 的 `ShowPrompt` 增强和 handler Report 保留不变。

### [2026-06-06] B-001 行动按钮不可用/原因提示 UI

- **短规格**:
  - 触发条件：OneRunMain Play 模式，玩家处于据点或搜刮相关阶段，点击不可用的 HUD 按钮或靠近不可用的设施。
  - 玩家操作：点击 HUD 行动按钮（外出、返回、结算、下一天、求助），或靠近据点设施查看底部提示。
  - 状态变化：不改核心规则；只读取 `GameSimulation.CheckShelterActionAvailability(State, actionId)` 或判断 State 阶段。
  - 可见反馈：按钮不可用时点击后在日志显示原因；靠近设施时底部提示在行动不可用时直接显示失败原因（如资源不足、阶段不允许）。
  - 验证方法：EditMode 测试写在 `TestOneRunUI.cs`。
- **改了什么**:
  1. `OneRunGameController.ShowPrompt(facilityId)` 增强：靠近据点设施时调用 `GameSimulation.CheckShelterActionAvailability` 查询行动可用性，不可用时在底部提示直接显示失败原因文本（替代原来的行动描述）。
  2. `EnterScavengeLocation` / `ReturnToShelter` / `LeaveHelpMarkAtActiveLocation` / `ResolveNight` / `NextDay` 五个按钮方法在原本静默 return 的分支增加 `Report()` 调用，向日志输出不可用原因。
  3. `Tests/TestOneRunUI.cs` 新增 3 个 `[Test]`：`TestShelterFacilityPromptShowsUnavailableReason`（阶段门控 + 材料不足）、`TestButtonClickReportsReasonWhenUnavailable`（搜刮中外出、DemoComplete 下一天、不在搜刮中返回据点）、`TestShelterPromptShowsPhaseAndResourceReasons`（day 阶段 + 建造材料不够 + 燃料不足）。
  4. `Tests/OneRunTestHelpers.cs` 新增 `OneRunController_B1/B2/B3` 到清理列表。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
- **验证状态**:
  - 源码计数：`TestOneRunUI.cs` 从 6 增至 9 个 `[Test]`。
  - Lint：三个修改文件全部 `errorCount: 0`。
  - 测试待 Unity Editor EditMode 运行。
- **跨线影响**:
  - **代码线（A 线）**：B-001 直接消费 A-001 提供的 `CheckShelterActionAvailability` API，不改规则层。
  - **代码线（C 线 / 世界线）**：不改 `ShelterInteractable.cs` 或玩家控制器；`ShowPrompt` 增强对世界线透明。
  - **设定线 / 美术线 / 比赛材料线**：无影响。纯 UI 可读性改进。

### [2026-06-06] A-FIX-003 修正夜晚/次日可用性查询与真实 handler 对齐

- **问题**: A-003 的 `CheckDayPhaseActionAvailability` 给 `resolve_night` / `next_day` 增加了 `night`、`reveal`、`searching` 等阶段门控，但 `OneRunGameController.ResolveNight` / `NextDay` 当前真实行为只在 `DemoComplete` 时阻止；`ResolveNight` 在 `searching` 会先返回据点再结算，`NextDay` 其他阶段直接调用 `StartDay`。
- **修复**:
  1. `GameSimulation.CheckDayPhaseActionAvailability(state, actionId)` 改为严格对齐当前 handler：`resolve_night` 和 `next_day` 均只在 `DemoComplete` 时返回不可用；未知行动仍返回 `未知行动。`。
  2. `Tests/TestGameSimulation.cs` 将目标测试改为 `TestDayPhaseActionAvailabilityMatchesCurrentResolveNightHandler` 和 `TestDayPhaseActionAvailabilityMatchesCurrentNextDayHandler`，覆盖 `morning/day/evening/searching/night/reveal`。
  3. 只读测试已覆盖 `Day`、`Phase`、`DemoComplete`、`EndingState`、`LastEvent`、`Resources` 六字段、`Lin` 关键字段、`Exploration` 关键字段和 `Qimian` 日志/线索计数。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
- **验证状态**:
  - Unity project path: `E:/Download/working/BeyondSafeZone/BeyondSafeZoneUnity/Assets`。
  - Unity mode: `bypass`。
  - `TestDayPhaseActionAvailabilityMatchesCurrentResolveNightHandler`：`1/1 passed`，jobId `59aa6aa3`。
  - `TestDayPhaseActionAvailabilityMatchesCurrentNextDayHandler`：`1/1 passed`，jobId `496030ea`。
  - `test_run` EditMode 全量：jobId `016cf322`，**130/130 passed, 0 failed, 0 skipped**。
- **跨线影响**:
  - **B 线 / UI 子线**：若后续接入 `CheckDayPhaseActionAvailability`，它现在表达的是当前按钮 handler 行为，不额外创造新阶段规则。
  - **设定线 / 美术线 / 比赛材料线**：无影响。

### [2026-06-06] A-003 夜晚/次日流程可用性与失败原因规则接口

- **短规格**:
  - 触发条件：UI 层准备执行夜晚结算/下一天前查询。
  - 玩家操作：无直接操作，规则查询。
  - 状态变化：查询不改变 GameState。
  - 可见反馈：不做 UI，返回 FailureReason 给 B 线使用。
  - 验证方法：EditMode 测试写在 TestGameSimulation.cs。
- **改了什么**:
  1. `Model/GameState.cs` 新增 `DayPhaseActionAvailability` 结构体。
  2. `Core/GameSimulation.cs` 新增 `CheckDayPhaseActionAvailability(state, actionId)`：覆盖 `resolve_night`（DemoComplete 阻止、night/reveal 阻止、searching 允许（UI 层先 ReturnToShelter））和 `next_day`（DemoComplete 阻止、night/reveal 阻止、searching 阻止）。
  3. `Tests/TestGameSimulation.cs` 新增 5 个 `[Test]`：shelter phases 允许、searching 行为报告、DemoComplete 阻止、未知行动、只读不改变状态。
  4. 后续已由 `A-FIX-003` 修正：查询接口不再额外增加 `night/reveal/searching` 阶段门控，改为对齐当前 `OneRunGameController` handler。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/Model/GameState.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
- **验证状态**:
  - `debug_force_recompile` → 编译完成。
  - `test_run` EditMode，jobId `e8a23846`，Unity mode: bypass。
  - 结果：**130/130 passed, 0 failed, 0 skipped**。
  - Lint：三个修改文件全部 `errorCount: 0`。
  - 缓存状态：发现列表 130（含跨类 stale cache），全量通过。
- **跨线影响**:
  - **代码线（B 线 / UI 子线）**：可通过 `GameSimulation.CheckDayPhaseActionAvailability(state, actionId)` 获取夜晚/次日不可用原因。
  - **设定线 / 美术线 / 比赛材料线**：无影响。

- **问题**: `TestExplorationActionAvailabilityBlocksSearchedOrLockedRoom` 中先 `EnterLocation("convenience")` → `SearchRoom("storefront")`，phase 仍为 "searching"。随后 `EnterLocation("clinic")` 被 `ExplorationController.EnterLocation` 的 phase 检查拒绝（要求 morning/day），ActiveLocation 实际未切换。后续 pharmacy Locked 测试实为"roomId 在 convenience 下不存在"，而非"Locked 房间"。
- **修复**: 拆分为两个独立 GameState：① 已搜房间用当前 `_state` 测试 convenience/storefront；② 锁房间新建 `state2 = GameSimulation.NewGame()`，EnterLocation("clinic") 后设置 pharmacy.Locked=true，并断言 Phase=="searching"、ActiveLocation=="clinic"。
- **修改的文件**: `TestGameSimulation.cs`（仅测试修复，零 lint，不改规则层）。
- **验证结果**: `debug_force_recompile` → 编译完成。`test_run` EditMode，jobId `481edb84`，**130/130 passed, 0 failed, 0 skipped**。Unity mode: bypass。缓存状态: 发现列表 130，全量通过。
- **跨线影响**: 无。

### [2026-06-06] A-002 搜刮房间行动可用性与失败原因规则接口

- **短规格**:
  - 触发条件：玩家已进入或未进入室内搜刮状态。
  - 玩家操作：规则层查询 `search_room`、`lure_room`、`leave_exploration` 是否可用。
  - 状态变化：查询不改变 `GameState`、不消耗时间、不拿资源、不改变房间状态。
  - 可见反馈：不做 UI，返回的 `FailureReason` 可直接给 B 线显示。
  - 验证方法：EditMode 测试写在 `TestGameSimulation.cs`。
- **改了什么**:
  1. `Model/GameState.cs` 新增 `ExplorationActionAvailability` 结构体（`Available`/`ActionId`/`FailureReason` + `Ok()`/`Fail()`）。
  2. `Controllers/ExplorationController.cs` 新增 `CheckActionAvailability(GameState, actionId, roomId)`：覆盖 `search_room`/`quick_search`/`careful_search`/`lure_room`/`leave_exploration`，对齐 `SearchRoom`/`LureRoom`/`LeaveExploration` 真实前置条件（阶段、ActiveLocation、roomId 存在、Searched、Locked）。
  3. `Core/GameSimulation.cs` 新增 `CheckExplorationActionAvailability` 委托方法。
  4. `Tests/TestGameSimulation.cs` 新增 5 个 `[Test]`：不在 searching 阶段阻止、未知/无效 roomId 阻止、已搜/上锁房间阻止、lure 和 leave 在 searching 中可用、只读不改变状态。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/Model/GameState.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Controllers/ExplorationController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
- **验证状态**:
  - `debug_force_recompile` → 编译完成。
  - `test_run` EditMode，jobId `4fbbe193`，结果 **130/130 passed, 0 failed, 0 skipped**。
  - Lint：四个修改文件全部 `errorCount: 0`。
  - 缓存状态：发现列表 130（含跨类 stale cache），全量通过。
- **跨线影响**:
  - **代码线（B 线 / UI 子线）**：可通过 `GameSimulation.CheckExplorationActionAvailability(state, actionId, roomId)` 获取搜刮行动不可用原因，在 UI 底部提示或按钮上直接显示。
  - **设定线 / 美术线 / 比赛材料线**：无影响。

### [2026-06-06] A-FIX-001 修复 A-001 测试断言与验证缓存

- **问题**: `TestCheckAvailabilityReturnsCorrectActionId` 用 `Parts = 0` 判断 `radio` 不可用是错误的。`radio_broadcast` 的资源门槛是 `Fuel`（`BalanceData.SHELTER_RADIO_FUEL = 1`），初始 Fuel=3，导致 `Parts = 0` 时 `radio` 仍报告 `Available = true`，断言 `Assert.IsFalse(aliasRadio.Available)` 会失败。
- **修复**: `_state.Resources.Parts = 0` → `_state.Resources.Fuel = 0`。`repair_bike` 不可用由 workbench 未建造保证。注释完善。
- **修改的文件**: `TestGameSimulation.cs`（仅 A 线）。
- **强制重编译**: `debug_force_recompile` → `isCompiling: false`。
- **测试运行**: `test_run` EditMode，jobId `f313f1ac`，结果 **130/130 passed, 0 failed, 0 skipped**。
- **缓存状态**: 发现列表 130（含跨类缓存）。`test_run_by_name` / `test_get_result` 本机有 JSON 解析 bug（`Invalid property identifier character: \\.`），但 `test_run` + `test_get_last_result` 全量通过。
- **跨线影响**: 无。

### [2026-06-06] A-001 据点行动可用性与失败原因规则接口

- **改了什么**:
  1. `Model/GameState.cs` 新增 `ShelterActionAvailability` 结构体：`Available`、`ActionId`、`FailureReason` 三个只读字段，带 `Ok()` / `Fail()` 工厂方法。
  2. `Controllers/ShelterController.cs` 新增 `CheckActionAvailability(GameState, string actionId)` 方法：覆盖所有 15 个行动 ID（含别名），逐条复制 `PerformAction` 的前置条件但不修改状态。
  3. `Core/GameSimulation.cs` 新增 `CheckShelterActionAvailability` 委托方法。
  4. `Tests/TestGameSimulation.cs` 新增 13 个 `[Test]`（初始 9 个 + 审查修复 4 个）。
- **审查修复（2026-06-06）**:
  - 阶段门控：从 `only evening` 改为允许 `morning`/`day`/`evening`，匹配 `EnsureShelterActionPhase()` 的自动阶段转换行为。`searching`/`night`/`reveal` 仍然阻止。
  - `workbench_car` 前置条件完善：新增 `Car.Found`/`Car.Ready` 检查 + 按当前修理步骤（StepEngine→StepTire→StepBattery→StepFueled）逐级验证材料和 CarParts 需求，与 `CarController.Repair` 完全对齐。
  - 别名 ActionId 保留：`repair_bike` 和 `radio` 查询返回原始的请求 actionId，不再被覆盖为正则表示 `workbench_repair`/`radio_broadcast`。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/Model/GameState.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Controllers/ShelterController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
- **验证状态**:
  - 源码计数：`TestGameSimulation.cs` 共 53 个 `[Test]`（40 原有 + 9 A-001 初始 + 4 审查修复）。
  - Lint：所有修改文件 `errorCount: 0`。
  - 测试待 Unity Editor 中运行（UnitySkills API 不直接暴露 Test Runner 端点）。
- **跨线影响**:
  - **代码线（UI 子线）**：`ShowPrompt` 在 morning/day 阶段查询时不会再显示"现在不是执行据点行动的时机。"，因为 UI 层已有 `EnsureShelterActionPhase` 自动转换。`repair_bike`/`radio` 查询返回的 `ActionId` 现在是原始请求 ID，UI 可据此区分玩家意图。
  - **设定线 / 美术线 / 比赛材料线**：无影响。

### [2026-06-06] ARCH-001 并行开发解耦

- **改了什么**:
  1. `TestGameSimulation.cs` 收窄为核心规则和确定性模拟测试，共 40 个 `[Test]`。
  2. 新增 `TestOneRunUI.cs`，承接 HUD、档案、日志、目标、地点卡等 U 类 UI 测试，共 6 个 `[Test]`。
  3. 新增 `TestOneRunWorld.cs`，承接据点、移动、互动、可读性等世界运行时测试，共 5 个 `[Test]`。
  4. 新增 `OneRunTestHelpers.cs`，收纳 UI / World 测试共享的 TMP 文本读取、字号读取和运行时对象清理辅助。
  5. 暂未拆分 `OneRunGameController.cs`；后续新增功能优先收进小 helper 方法，避免继续扩大核心方法。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunWorld.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/DECISIONS.md`
- **验证状态**:
  - 源码计数：`TestGameSimulation.cs` 40、`TestOneRunUI.cs` 6、`TestOneRunWorld.cs` 5，总计 51 个项目 `[Test]`。
  - `BeyondSafeZone.Tests.TestOneRunUI`：`1/1 passed`，jobId `d1ccae55`。
  - `BeyondSafeZone.Tests.TestOneRunWorld`：`1/1 passed`，jobId `b427ad52`。
  - 精确方法验证通过：
    - `TestOneRunHudShowsLocationCards`：jobId `cd712083`
    - `TestShelterInteractionShowsVisibleFeedbackText`：jobId `d58335f9`
    - `TestMinimumVerticalSliceCoversClinicAiChain`：jobId `d2cecf9c`
  - 当前 Unity Test Runner 类级发现仍有缓存问题：`BeyondSafeZone.Tests.TestGameSimulation` 返回 `43/51 passed, 8 failed`，jobId `375e5e88`；8 个失败名均为旧的 `TestGameSimulation.*` UI / World 测试名，源码中已经不存在。
  - 已执行 `debug_force_recompile`，Unity 编译结束后 `test_list` 仍显示 `unity_test_runner_async_cache` 旧发现结果。
- **跨线影响**:
  - **代码线**：后续可并行拆为 A/B/C 三类代码对话：核心规则改 `TestGameSimulation.cs`，UI 改 `TestOneRunUI.cs`，据点/世界改 `TestOneRunWorld.cs`。三条线仍需避免同时修改 `OneRunGameController.cs`。
  - **设定线 / 美术线 / 比赛材料线**：无玩法口径变化；这是协作和测试维护结构调整。
- **未完成/风险**:
  - 需要重启 Unity Editor 或清理 Unity Test Runner 发现缓存后，重新跑 `TestGameSimulation`、`TestOneRunUI`、`TestOneRunWorld` 三类完整回归。

### [2026-06-06] U-004 地点选择信息卡（修订 v2 — 审查修复）

- **修复内容**:
  1. `GetLocationCardInfo("clinic")` 诊所无 QimianTrace 时异常显示从 `暂无` 改为 `待调查`。超市和车库无 QimianTrace 时保持 `暂无`。
  2. 测试 `TestOneRunHudShowsLocationCards` 新增断言验证诊所初始 info 包含 `待调查`。
- **进一步验证**:
  - Clear Console 后重跑全部测试。
  - Play 截图：`BeyondSafeZoneUnity/Assets/Screenshots/u004_onerunmain_play.png`。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Screenshots/u004_onerunmain_play.png`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
- **测试状态 (v2)**:
  - 新增/更新测试：`TestOneRunHudShowsLocationCards`：`1/1 passed`，jobId `ba5fb6e0`。
  - 回归：`TestOneRunHudShowsCurrentObjectiveAndChinesePhase` (`242c27d2`)、`TestDossierButtonOpensEmptyDossierPanel` (`e3f627e8`)、`TestQimianLogButtonOpensLockedLogPanel` (`3f157c57`)、`TestMinimumVerticalSliceCoversClinicAiChain` (`7e4db0c3`) 全部通过。
  - Unity EditMode 完整回归：`BeyondSafeZone.Tests.TestGameSimulation`：52 个 `[Test]` 方法全部通过，jobId `6cd8b217`。
- **跨线影响**: 低。纯 UI 文本修正，不影响核心规则。
- **Console**: `logs: 0`, `warnings: 0`, `errors: 1`。
  - Error 原文：`CS0103: BuildObjectivePanel does not exist` at `OneRunGameController.cs:305`
  - 来源：Unity 编译器缓存残留。方法实际存在，全部 52/52 测试通过确认无真正编译错误。Domain reload 后可清除。
- **未完成/风险**: 无。

### [2026-06-05] U-001 当前目标与阶段引导面板

- **改了什么**:
  1. HUD 新增 `ObjectivePanel`，位于 Header 下方，显示 `当前目标` 标题和动态 1-2 行目标说明。
  2. Header 改为中文阶段：`第 {Day} 天  {阶段中文名}`，不再显示英文 `Phase`。
  3. 阶段中文映射：`morning→清晨, day→白天, searching→搜刮中, evening→黄昏, night→夜晚, reveal→结尾揭示`。
  4. 目标文案按天数和状态动态切换：Day 1-4 提示外出搜刮，Day 5+ 提示未知行动者和诊所异常，已有标记后提示推进夜晚，通关后提示打开日志。
  5. `RefreshAll()` 内调用 `RefreshObjectivePanel()`，目标面板随所有状态刷新同步更新。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Screenshots/u001_onerunmain_play.png`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
- **测试状态**:
  - 新增测试：`TestOneRunHudShowsCurrentObjectiveAndChinesePhase`：`1/1 passed`，jobId `fea7db19`。
  - 回归测试全部通过：`TestOneRunVisualReadabilityScaffold` (`74a5e147`)、`TestShelterInteractionShowsVisibleFeedbackText` (`c54a2bb6`)、`TestQimianLogButtonOpensLockedLogPanel` (`11e76358`)、`TestShelterFacilityVisualsExposeBuildUseAndDamageState` (`832503fc`)、`TestDossierButtonOpensEmptyDossierPanel` (`723fcbd7`)。
  - Unity EditMode 完整回归：`46/46 passed`，jobId `3bd30e20`。
  - 测试 caveat：本次源码新增 1 个 `[Test]`，但 Unity Test Runner 类级回归仍返回 `46/46 passed`；新增测试已用精确方法名单独运行并通过。
  - Play 层级验证：截图 `BeyondSafeZoneUnity/Assets/Screenshots/u001_onerunmain_play.png`。
  - Console：`logs: 7`、`warnings: 0`、`errors: 0`。
- **对其他线的影响**:
  - **设定线**：玩家进入游戏后能立即从 HUD 看到当前目标引导，不再需要翻文档。
  - **美术线**：后续可为 `ObjectivePanel` 和 `ObjectiveTitle` 设计正式底图和图标样式。
  - **比赛材料线**：可如实描述"Unity greybox 已有目标引导面板和中文阶段显示"。

### [2026-06-05] SHELTER-002 据点设施状态与互动反馈

- **改了什么**:
  1. `OneRunMain` 运行时设施新增更明确的状态子对象：`Blueprint_*`、`Built_*`、`UsedMarker_*`、`DamageMarker_barricade`、`Feedback_*`。
  2. `ShelterInteractable` 现在按设施建造、今日使用和墙体破损状态切换对应视觉层。
  3. 玩家靠近设施按 `E` 后，设施旁 `Feedback_*` 会显示本次据点行动结果，并继续同步 HUD 日志。
  4. 保留旧 `State_*` 色块，避免破坏 `SHELTER-001` 的横截面据点结构和已有测试。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Screenshots/shelter002_onerunmain_play.png`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
- **测试状态**:
  - 红测：`TestShelterFacilityVisualsExposeBuildUseAndDamageState` 实现前 `0/1 passed`，jobId `2a35082b`。
  - 红测：`TestShelterInteractionShowsVisibleFeedbackText` 实现前 `0/1 passed`，jobId `46609ee0`。
  - 编译反馈：`OneRunGameController.cs`、`ShelterInteractable.cs`、`TestGameSimulation.cs` 均为 `errorCount: 0`。
  - 目标测试：`TestShelterFacilityVisualsExposeBuildUseAndDamageState`：`1/1 passed`，jobId `5d7fbb73`。
  - 目标测试：`TestShelterInteractionShowsVisibleFeedbackText`：`1/1 passed`，jobId `468e7f9b`。
  - Unity EditMode 完整回归：`46/46 passed`，jobId `3a2b6a84`。
  - 测试 caveat：本次源码新增 2 个 `[Test]`，但 Unity Test Runner 类级回归仍返回 `46/46 passed`；两个新增测试已用精确方法名单独运行并通过。
  - Play 层级验证：`Blueprint_bed`、`Built_workbench`、`Feedback_radio` 均存在；截图为 `BeyondSafeZoneUnity/Assets/Screenshots/shelter002_onerunmain_play.png`。
  - Console：干净 Play 验证 `warnings: 1`、`errors: 0`；此前 Play 中误触发 Test Runner 造成的 2 条工具层错误已排除。
- **对其他线的影响**:
  - **设定线**：据点设施现在能更清楚表达“未建造、已建造、今日已用、墙体破损、行动结果”。
  - **美术线**：后续正式设施素材可按这 5 类视觉状态替换灰盒层，不需要重新定义状态结构。
  - **比赛材料线**：可如实描述“据点设施已有状态反馈和互动结果反馈”，仍需标注为灰盒 UI/美术。

### [2026-06-05] VIS-001 OneRunMain 画面可读性整理

- **改了什么**:
  1. `OneRunMain` 运行时 HUD 新增 `ReadabilitySafeFrame`，并为状态、日志、底部提示分别增加 `StatusPanel`、`LogPanel`、`PromptPanel` 半透明底板。
  2. 调整 HUD 字号和位置：标题、状态、提示和日志在默认 Game 视图下更容易读。
  3. 相机改为低对比纯色背景，`orthographicSize` 调近到 `4.9`，让横截面据点和主角占据更主要画面。
  4. 世界设施标签改为短名，不再在房间里显示长行动提示；详细行动说明保留到底部提示栏。
  5. 为灰盒对象设置 `SpriteRenderer.sortingOrder`，林行主角渲染在设施前方。
  6. 生成 Play 验证截图 `BeyondSafeZoneUnity/Assets/Screenshots/vis001_onerunmain_play.png`。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `BeyondSafeZoneUnity/Assets/Screenshots/vis001_onerunmain_play.png`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
- **测试状态**:
  - 测试 caveat：`VIS-001` 红测阶段曾出现 Unity Test Runner 结果与新测试源码不同步的情况；已通过 `asset_refresh` 和编译反馈重新同步后继续验证。
  - 编译反馈：`OneRunGameController.cs`、`ShelterInteractable.cs`、`TestGameSimulation.cs` 均为 `errorCount: 0`。
  - 目标测试：`TestOneRunVisualReadabilityScaffold`：`1/1 passed`，jobId `c23b9140`。
  - Unity EditMode 完整回归：`46/46 passed`，jobId `ea92bf08`。
  - Play 层级验证：`OneRunHUD/ReadabilitySafeFrame`、`StatusPanel`、`PromptPanel` 存在；Console `warnings: 1`、`errors: 0`。
  - Play 截图验证：`BeyondSafeZoneUnity/Assets/Screenshots/vis001_onerunmain_play.png`。
- **对其他线的影响**:
  - **美术线**：当前据点仍是灰盒，但画面布局更明确；后续正式美术应继续服务横截面房屋、短设施标签、底部交互提示这套结构。
  - **比赛材料线**：可使用 `vis001_onerunmain_play.png` 作为当前 Unity 灰盒进度截图，但仍需标注不是最终美术。

### [2026-06-05] FIX-PLAYER-001 据点移动输入与林行主角贴图

- **改了什么**:
  1. 修复据点侧视移动的输入读取方式：`SideViewShelterPlayerController` 现在直接读取 `A/D`、`←/→` 作为左右移动兜底，不再只依赖 Unity Input Manager 的 `Horizontal` 轴。
  2. 楼梯上下楼输入也增加直接读取 `W/S`、`↑/↓` 兜底；仍然只有靠近 `Stairs_GroundToUpper` 时才会上下楼。
  3. 将用户提供的林行像素图导入当前 Unity 工程，作为 `LinXing_Player` 的运行时 Sprite。
  4. `OneRunGameController` 运行时优先从 `Resources.Load<Sprite>("Sprites/Characters/lin_xing_player")` 加载林行贴图，失败时才退回一像素灰盒块。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/Player/SideViewShelterPlayerController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Resources/Sprites/Characters/lin_xing_player.png`
  - `BeyondSafeZoneUnity/Assets/Resources/Sprites/Characters/lin_xing_player.png.meta`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/ASSET_LICENSE_LOG.md`
  - `docs/DECISIONS.md`
- **测试状态**:
  - 红测：`TestOneRunShelterUsesSideViewCutawayController` 在实现前失败：`0/1 passed`，jobId `307318d1`。
  - 导入设置验证：`lin_xing_player.png` 为 `Sprite`，`filterMode: Point`，`mipmapEnabled: false`，`spritePixelsPerUnit: 64`。
  - 编译反馈：`SideViewShelterPlayerController.cs`、`OneRunGameController.cs` 均为 `errorCount: 0`。
  - 目标测试：`TestOneRunShelterUsesSideViewCutawayController`：`1/1 passed`，jobId `2e42660a`。
  - Unity EditMode 完整回归：`46/46 passed`，jobId `909ee05b`。
  - Play 层级验证：`WalkableShelterGreybox/LinXing_Player` 存在；挂载 `BeyondSafeZone.Player.SideViewShelterPlayerController`；Console `warnings: 1`、`errors: 0`。
- **对其他线的影响**:
  - **美术线**：当前林行图为本地生成图，已进入 Unity 原型；公开参赛授权仍需确认，暂记为 `Needs Review`。
  - **设定线 / 比赛材料线**：可说当前 Unity 灰盒已有可见林行角色贴图，但不能称为最终角色动画或最终美术。

### [2026-06-05] SHELTER-001 横截面可走动据点灰盒

- **改了什么**:
  1. `OneRunMain` 的 `WalkableShelterGreybox` 从顶视单层房间改为横截面多房间灰盒。
  2. 运行时新增 `CutawayShelterFrame`、`ShelterFloor_Ground`、`ShelterFloor_Upper`、`Stairs_GroundToUpper`。
  3. 新增 `SideViewShelterPlayerController`：据点内左右移动，靠近楼梯后按上下切换楼层，靠近设施按 `E` 互动。
  4. 新增 `ShelterStairZone`，负责楼梯上下楼目标点。
  5. 外出搜刮仍切回 `TopDownPlayerController`，保持诊所/超市/车库顶视搜索链路不变。
  6. 据点设施新增 `State_*` 状态色块，用于表现未建造、已使用、墙体破损等灰盒反馈。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/Player/SideViewShelterPlayerController.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterStairZone.cs`
  - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/DECISIONS.md`
- **测试状态**:
  - 红测：`TestDossierButtonOpensEmptyDossierPanel` 加入横截面据点断言后，实现在前失败：`0/1 passed`，jobId `504c0b9a`。
  - 编译反馈：`OneRunGameController.cs`、`SideViewShelterPlayerController.cs`、`ShelterStairZone.cs`、`TestGameSimulation.cs` 均为 `errorCount: 0`。
  - `TestDossierButtonOpensEmptyDossierPanel`：`1/1 passed`，jobId `99496be6`。
  - `TestOneRunShelterUsesSideViewCutawayController`：`1/1 passed`，jobId `144087ba`。
  - Unity EditMode 完整回归：`46/46 passed`，jobId `c63a3b85`。
  - Play 层级验证：`CutawayShelterFrame`、`ShelterFloor_Ground`、`ShelterFloor_Upper`、`Stairs_GroundToUpper` 存在；`LinXing_Player` 挂载 `SideViewShelterPlayerController`；6 个 `State_*` 设施状态对象存在。
  - Unity Console：`warnings: 1`、`errors: 0`；warning 仍为已知 VS/Unity UDP 提示。
- **对其他线的影响**:
  - **设定线**：据点表达口径改为横截面可走动家，不再只是顶视据点房间。
  - **美术线**：后续据点美术应围绕横截面多房间、上下楼平台、楼梯、设施状态块制作。
  - **比赛材料线**：可如实描述“Unity greybox 已有横截面可走动据点”；仍需标注当前为灰盒，不是正式像素美术。

### [2026-06-05] U-008 祁眠行动日志面板

- **改了什么**:
  1. `OneRunMain` 运行时 HUD 新增 `日志` 按钮。
  2. 新增 `QimianLogPanel`、`QimianLogTitle`、`QimianLogBody`、`CloseQimianLog` 运行时 UI。
  3. 面板默认隐藏；未通关时显示 `通关后解锁祁眠行动日志。`。
  4. 通关 reveal 解锁后，面板读取 `GameSimulation.GetQimianEndingRevealText(State)`，展示人格卡、感知输入、候选行动、排序、最终选择和地图影响。
  5. 打开 `日志` 面板时会关闭 `档案` 面板，打开 `档案` 面板时会关闭 `日志` 面板，避免两个大面板叠在一起。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
- **测试状态**:
  - 红测：`TestOneRunControllerExposesQimianLogPanelActions` 实现前 `0/1 passed`，jobId `e6df5b15`。
  - 编译反馈：`OneRunGameController.cs` `errorCount: 0`；`TestGameSimulation.cs` `errorCount: 0`。
  - `TestQimianLogButtonOpensLockedLogPanel`：`1/1 passed`，jobId `7ec15e49`。
  - `TestOneRunControllerExposesQimianLogPanelActions`：`1/1 passed`，jobId `6413a695`。
  - Unity EditMode 完整回归：`45/45 passed`，jobId `356bd9ec`。
  - Play 层级验证：`OneRunHUD/QimianLogButton` active；`QimianLogPanel` 默认 hidden；`DossierPanel` 仍存在。
  - Unity Console：`warnings: 1`、`errors: 0`；warning 仍为已知 VS/Unity UDP 提示。
- **对其他线的影响**:
  - **设定线**：Day 15 结尾解释现在可通过玩家主动打开的日志面板阅读。
  - **美术线**：后续可补 `祁眠行动日志` 面板底图、日志条目图标、人格卡/输入/选择分区样式。
  - **比赛材料线**：可如实描述“Unity greybox 已有祁眠行动日志面板”；仍需标注当前为灰盒 UI。

### [2026-06-05] ENV-001 Unity 环境恢复与 Play 验证

- **改了什么**:
  1. 未改玩法代码，只做 Unity 环境取证和验证。
  2. 确认当前 Unity 主工程为 `E:\Download\working\BeyondSafeZone\BeyondSafeZoneUnity`。
  3. 确认当前 active scene 为 `Assets/Scenes/OneRunMain.unity`。
  4. 确认 UnitySkills 包已启动，但实际 REST 地址为 `http://localhost:8090/`，不是计划里写的 `42610`。
  5. 生成 Play 验证截图 `BeyondSafeZoneUnity/Assets/Screenshots/env001_onerunmain_play.png`，仅作为环境验证证据。
- **新增/修改的文件或资产**:
  - `docs/UNITY_STATUS.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `BeyondSafeZoneUnity/Assets/Screenshots/env001_onerunmain_play.png`
- **验证状态**:
  - `http://127.0.0.1:8090/health`：`status: ok`，Unity `2022.3.62f3c1`，UnitySkills `2.0.1`，`currentMode: bypass`。
  - `project_get_info`：`projectPath: E:/Download/working/BeyondSafeZone/BeyondSafeZoneUnity/Assets`。
  - `scene_get_info` / `scene_get_loaded`：`Assets/Scenes/OneRunMain.unity`。
  - Play 层级存在：`OneRunBootstrap`、`Main Camera`、`WalkableShelterGreybox`、`LinXing_Player`、`EventSystem`、`OneRunHUD`。
  - Console：`warnings: 1`、`errors: 0`。
  - EditMode 回归：`BeyondSafeZone.Tests.TestGameSimulation`，`43/43 passed`，jobId `9aa14f2b`。
- **对其他线的影响**:
  - **设定线 / 比赛材料线**：后续可继续用 `OneRunMain` 作为当前真实 Demo 入口。
  - **美术线**：截图是灰盒验证图，不是正式美术交付。
  - **代码线**：UnitySkills 端口需以面板或 `Editor.log` 为准；本机当前验证端口为 `8090`。

### [2026-06-05] DOC-UNITY-CANONICAL 当前 Unity 工程口径清理

- **改了什么**:
  1. 当前工程入口统一为 `E:\Download\working\BeyondSafeZone\BeyondSafeZoneUnity`。
  2. 旧的兄弟 Unity 目录 `E:\Download\working\BeyondSafeZoneUnity` 标记为废弃；此前删除该目录时被 Unity 锁文件阻塞，已删除核心内容但仍残留 `Library`、`Logs`、`Temp`。
  3. 仓库内旧 Godot 项目 `game/` 已从当前工作树移除。
  4. 旧迁移文件 `docs/UNITY_MIGRATION_PLAN.md`、`docs/UNITY_MIGRATION_STATUS.md` 移出当前入口，新增 `docs/UNITY_STATUS.md` 作为 Unity 验证和阻塞记录。
  5. 更新 README、HANDOFF、AGENTS、策划包、素材规范、比赛材料，使当前对外口径对齐 Unity 灰盒。
  6. 压缩跨线日志和项目记忆，删除会误导当前开发的旧实现流水账。
- **新增/修改的文件或资产**:
  - `docs/UNITY_STATUS.md`
  - `AGENTS.md`
  - `HANDOFF.md`
  - `README.md`
  - `docs/DECISIONS.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/planning_package/*`
  - `docs/ASSET_PIPELINE.md`
  - `docs/ASSET_LICENSE_LOG.md`
  - `marketing/*`
- **验证状态**:
  - 本条为文档/仓库口径清理；最终格式和引用检查见本次会话收尾记录。
- **对其他线的影响**:
  - **设定线**：当前策划入口仍是 `docs/planning_package/README.md`，但实现口径改为 Unity-only。
  - **美术线**：素材仍放 `assets/source/` 和 `assets/sprites/`，导入目标改为 Unity。
  - **比赛材料线**：只可宣传当前 Unity 灰盒已落地内容，尤其是 `OneRunMain` 和诊所 AI 因果链。

### [2026-06-05] U-007 未知行动者档案面板

- **改了什么**:
  1. `OneRunMain` 运行时 HUD 新增 `档案` 按钮。
  2. 新增 `DossierPanel`、`DossierTitle`、`DossierBody`、`CloseDossier` 运行时 UI。
  3. 面板默认隐藏，打开时读取 `GameSimulation.GetAnomalyDossierText(State)`。
  4. 空档案状态显示 `暂无异常记录。`。
- **新增/修改的文件或资产**:
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - `docs/UNITY_STATUS.md`
- **测试状态**:
  - `OneRunGameController.cs` 编译反馈：`errorCount: 0`。
  - `TestGameSimulation.cs` 编译反馈：`errorCount: 0`。
  - `TestOneRunControllerExposesDossierPanelActions`：`1/1 passed`，jobId `74bb7d83`。
  - `TestDossierButtonOpensEmptyDossierPanel`：`1/1 passed`，jobId `867c80a0`。
  - Unity EditMode 完整回归：`42/42 passed`，jobId `09d9a3cb`。
  - Unity Console：`warnings: 1`、`errors: 0`；warning 为 VS/Unity UDP 端口提示，不来自项目脚本。
- **对其他线的影响**:
  - **设定线**：一周目玩家可主动打开未知行动者档案。
  - **美术线**：后续可补档案面板底图、异常条目图标、匿名药包和理解标记图标。
  - **比赛材料线**：可如实描述“Unity greybox 已有未知行动者档案面板”。

### [2026-06-05] 仓库同步：合并远端 Unity 项目并推送 main

- **改了什么**:
  1. 远端仓库确认为 `https://github.com/T3L000/BeyondSafeZone.git`。
  2. 合并远端 Unity 项目历史，并将 Unity 工程内容同步进仓库内 `BeyondSafeZoneUnity/`。
  3. 推送到远端 `origin/main`。
- **验证状态**:
  - `git push origin main` 成功。
  - 推送后 `git status --short --branch` 曾显示工作区干净。
- **对其他线的影响**:
  - 后续所有线都应围绕仓库内 `BeyondSafeZoneUnity/` 协作。

### [2026-06-04] C-007 到 C-010 诊所 AI 最小链路

- **改了什么**:
  1. C-007：祁眠读取诊所 `help` 标记并给玩家夜晚可见反馈。
  2. C-008：诊所 `help` 标记触发匿名药品 / 浅箭头回应。
  3. C-009：Day 15 结尾日志解释诊所标记因果链。
  4. C-010：新增最小纵切集成回归，串联 Day 1、诊所异常、标记、祁眠回应和结尾日志。
- **测试状态**:
  - C-007 完整回归：`38/38 passed`，jobId `f6bef90c`。
  - C-008 完整回归：`39/39 passed`，jobId `880a4edd`。
  - C-009 完整回归：`40/40 passed`，jobId `061afa85`。
  - C-010 完整回归：`41/41 passed`，jobId `6103dd69`。
- **对其他线的影响**:
  - 当前最小 AI 玩法链路已具备测试保护；后续不要扩大范围前先保住这条链路。

### [2026-06-04] OneRunMain 正式一周目主场景

- **改了什么**:
  1. `Assets/Scenes/OneRunMain.unity` 确认为正式一周目林行线灰盒场景。
  2. `MainPrototype.unity` 降为参考场景。
  3. 运行时生成可走动据点、HUD、诊所/超市/车库搜刮灰盒和 `留下求助` 入口。
- **测试状态**:
  - 当时完整 Unity EditMode 回归：`35/35 passed`，jobId `2c7a6f63`。
- **对其他线的影响**:
  - 当前录屏和截图应从 `OneRunMain` 获取。

---

## Design Lane

### 当前稳定口径

- 当前目标是 10-15 分钟 Unity 灰盒纵切，不是完整大体量版本。
- 近期核心地点：林行家/据点、社区诊所、小区超市、修理铺/车库。
- 当前 AI 核心链路：诊所异常 → 求助标记 → 祁眠读取 → 匿名药品/浅箭头 → 档案验证 → 结尾日志解释。
- 完整二周目、14 地点全量、复杂行动点/骰子、长期 NPC 合作和 5 段回放动画均为后续范围。

---

## Art Lane

### 当前稳定口径

- 美术源文件：`assets/source/`。
- Unity 可导入导出图：`assets/sprites/`。
- 角色规格：`32x32`。
- 基础瓦片：`16x16`。
- 当前优先素材：
  - 林行
  - 祁眠
  - 普通丧尸 / 血月丧尸
  - 据点、诊所、超市、修理铺/车库
  - 资源图标、异常档案图标、求助标记、匿名药包、浅箭头
- 所有外部或 AI 生成素材必须记录到 `docs/ASSET_LICENSE_LOG.md`。

---

## Contest Lane

### 当前稳定口径

- 当前比赛材料只描述 Unity 灰盒真实状态。
- 可展示内容：
  - `OneRunMain`
  - 可走动据点灰盒
  - 诊所/超市/车库入口
  - 求助标记
  - 匿名药品/浅箭头反馈
  - 未知行动者档案面板
  - 结尾结构化日志文本
  - Unity EditMode 回归测试记录
- 不宣传为已实现：
  - 正式像素美术
  - 完整二周目
  - 完整行动点/骰子
  - 长期 NPC 合作
  - 5 段动画回放
  - 全部地点完整可玩

---

## 跨线阻塞/待同步项

| 日期 | 来源线 | 阻塞项 | 需要哪条线响应 | 状态 |
|------|--------|--------|---------------|------|
| 2026-06-05 | Code | 旧兄弟 Unity 目录仍残留锁定的 `Library/Logs/Temp` | 用户 / Code | 待用户关闭占用该目录的 Unity 后再删除 |
| 2026-06-05 | Art | 当前画面仍是灰盒，占位素材不适合最终提交 | Art / Contest | 待制作 |
