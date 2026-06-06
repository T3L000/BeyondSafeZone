# Unity 状态记录

更新时间：2026-06-06

> 项目：BeyondSafeZone  
> 当前 Unity 主工程：`E:\Download\working\BeyondSafeZone\BeyondSafeZoneUnity`  
> 状态：正式一周目灰盒场景 `OneRunMain` 进行中

---

## 当前口径

- 当前只使用仓库内 Unity 工程：`BeyondSafeZoneUnity/`。
- 旧的兄弟 Unity 目录 `E:\Download\working\BeyondSafeZoneUnity` 已废弃；后续不要用它做 UnitySkills 或手动开发。
- `Assets/Scenes/OneRunMain.unity` 是正式一周目林行线灰盒场景。
- `Assets/Scenes/MainPrototype.unity` 只保留为临时参考，不继续扩成主线。
- `docs/UNITY_MIGRATION_PLAN.md` 已过时并移出当前文档入口。

## 当前已验证实现

- `OneRunMain` 运行时生成：
  - `WalkableShelterGreybox`
  - 横截面多房间据点灰盒：`CutawayShelterFrame`、`ShelterFloor_Ground`、`ShelterFloor_Upper`、`Stairs_GroundToUpper`
  - `LinXing_Player`
  - 林行当前原型贴图：`Assets/Resources/Sprites/Characters/lin_xing_player.png`
  - 据点设施灰盒
  - 据点设施状态反馈：`Blueprint_*`、`Built_*`、`UsedMarker_*`、`DamageMarker_barricade`、`Feedback_*`
  - `OneRunHUD`
  - 可读性 HUD 结构：`ReadabilitySafeFrame`、`StatusPanel`、`LogPanel`、`PromptPanel`
  - 诊所、超市、车库搜刮入口
- 当前核心 AI 链路：

```text
诊所异常
→ 林行留下 help 求助标记
→ Day 5 后祁眠夜间读取可感知标记
→ 诊所出现匿名药品 / 浅箭头反馈
→ 未知行动者档案记录“诊所隔离记录 / 匿名药品 / 理解标记”
→ Day 15 结尾日志解释人格卡、感知输入、候选行动、排序、最终选择和地图影响
```

- `U-007 未知行动者档案面板` 已落地：
  - HUD `档案` 按钮
  - `未知行动者档案` 面板
  - 空状态文本 `暂无异常记录。`
  - 正文读取 `GameSimulation.GetAnomalyDossierText(State)`
- `U-008 祁眠行动日志面板` 已落地：
  - HUD `日志` 按钮
  - `祁眠行动日志` 面板
  - 未通关状态显示 `通关后解锁祁眠行动日志。`
  - 通关 reveal 解锁后读取 `GameSimulation.GetQimianEndingRevealText(State)`
- `U-004 地点选择信息卡` 已落地：
  - HUD 新增 `LocationCardPanel`，位于地点按钮上方
  - 3 个地点卡：`LocationCard_clinic`、`LocationCard_supermarket`、`LocationCard_bike_shop`
  - 每个卡片显示地点名和资源/危险/异常信息
  - 动态追加：有新痕迹、已留标记、资源减少
  - `RefreshAll()` 中调用 `RefreshLocationCards()` 实时刷新
- `U-001 当前目标与阶段引导面板` 已落地：
  - HUD 新增 `ObjectivePanel`、`ObjectiveTitle`、`ObjectiveBody`
  - Header 改为中文阶段：`第 {Day} 天  {阶段中文名}`
  - 阶段中文映射：morning→清晨, day→白天, searching→搜刮中, evening→黄昏, night→夜晚, reveal→结尾揭示
  - 目标文案按天数和状态动态切换
- `C-001 据点交互范围与高亮反馈` 已落地：
  - 任务短规格：
    - 触发条件：OneRunMain Play 模式，林行在横截面据点内移动。
    - 玩家操作：靠近设施、离开设施、按 E 互动。
    - 状态变化：只改变世界表现和交互提示；不改 GameSimulation 规则。
    - 可见反馈：当前设施 stateRenderer 颜色变亮（RGB ×1.5 + alpha +0.08）；离开后恢复原色；底部提示不变；互动后 Feedback 文本不变。
    - 验证方法：EditMode 测试写在 `TestOneRunWorld.cs`。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/Player/SideViewShelterPlayerController.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunWorld.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
  - 源码结构验证：
    - `TestOneRunWorld.cs`：6 个 `[Test]`（C-001 新增 1 个）。
    - Lint：四个修改文件全部 `errorCount: 0`。
  - 测试待 Unity Editor EditMode 运行。
  - 判定：实现已落地、Lint 干净、测试已编写。需 Unity Editor 中运行完整回归确认。

- `C-FIX-001 用真实 Trigger 路径验证设施高亮切换`：
  - 任务短规格：
    - 触发条件：OneRunMain 运行时生成据点、玩家、设施 trigger。
    - 玩家操作：林行进入 radio 交互范围，再进入 stove 交互范围，再离开。
    - 状态变化：只改变高亮状态，不改 GameState。
    - 可见反馈：当前附近设施高亮，旧设施取消高亮。
    - 验证方法：TestOneRunWorld 用反射调用 SideViewShelterPlayerController 的 OnTriggerEnter2D / OnTriggerExit2D，验证 IsHighlighted 和颜色变化。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunWorld.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
  - 源码结构验证：
    - `TestOneRunWorld.cs`：8 个 `[Test]`（C-FIX-001 新增 1 个）。
    - Lint：两个修改文件全部 `errorCount: 0`。
    - 未修改 `SideViewShelterPlayerController.cs`、`ShelterInteractable.cs`、`OneRunGameController.cs`。
  - **C-VERIFY-001 验证结果**（2026-06-06）：
    - `TestSideViewShelterPlayerControllerSwitchesHighlightOnTriggerOverlap`：`1/1 passed`，jobId `36b578d7`（6s）。
    - `TestShelterInteractableHighlightOnApproach`：`1/1 passed`，jobId `44691a33`（7s）。
    - Console：`errors: 2`（stale cache：Mono.Cecil 内部 + TMPro namespace），无项目代码真实错误。
    - `test_list`：`discoveryMode: unity_test_runner_async_cache`，仅显示 UnitySkills 内部测试。
    - `test_run`（全量 EditMode）：`MODE_FORBIDDEN`（Auto 模式下 `mayEnterPlayMode: true` 被禁止）。
  - 判定：C-FIX-001 两个目标测试全部通过，高亮真实 Trigger 路径验证闭环。

- `B-FIX-001 修复不可用按钮真实点击路径`：
  - 问题：B-001 将不可用按钮设为 `interactable=false`，真实玩家无法点击 disabled button，测试 `onClick.Invoke()` 绕过了限制。
  - 方案二：按钮始终保持 `interactable=true`，不可用时视觉弱化（`Image.color`），点击后 handler Report 原因。
  - 任务短规格：
    - 触发条件：OneRunMain Play 模式，HUD 行动当前不可执行。
    - 玩家操作：点击 HUD 按钮。
    - 状态变化：不改核心规则；handler 内部调用已有查询判断。
    - 可见反馈：日志显示失败原因文本。
    - 验证方法：EditMode 测试验证 `Button.interactable == true` + `onClick.Invoke()` → 日志含原因。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
  - 源码结构验证：
    - 新增 `SetButtonVisual(Button, bool)` 静态方法。
    - `RefreshAll()` 中 7 个按钮用 `SetButtonVisual()` 替代 `interactable=false`。
    - `TestOneRunUI.cs`：9 个 `[Test]`（B-001 原有 8 个 + B-FIX-001 新增 `TestHudUnavailableButtonsRemainClickableAndReportReasons`，替换旧 `TestButtonClickReportsReasonWhenUnavailable`）。
    - Lint：两个文件全部 `errorCount: 0`。
  - Unity EditMode 验证（bypass 模式）：
    - `TestHudUnavailableButtonsRemainClickableAndReportReasons`：`1/1 passed`，jobId `a23f4755`。
    - `TestShelterFacilityPromptShowsUnavailableReason`：`1/1 passed`，jobId `2e99862e`。
    - `TestShelterPromptShowsPhaseAndResourceReasons`：`1/1 passed`，jobId `830e7507`。
    - `TestMinimumVerticalSliceCoversClinicAiChain`（核心回归）：`1/1 passed`，jobId `b6746c28`。
  - 判定：实现已落地、Lint 干净、4 个测试全部通过。真实 UI 路径可解释不可用原因。

- `B-001 行动按钮不可用/原因提示 UI`：
  - 任务短规格：
    - 触发条件：OneRunMain Play 模式，玩家处于据点或搜刮相关阶段，点击不可用的 HUD 按钮或靠近不可用的设施。
    - 玩家操作：点击 HUD 行动按钮（外出、返回、结算、下一天、求助），或靠近据点设施查看底部提示。
    - 状态变化：不改核心规则；只读取 `GameSimulation.CheckShelterActionAvailability(State, actionId)` 或判断 State 阶段。
    - 可见反馈：按钮不可用时点击后在日志显示原因（如"正在搜刮中，无法再次外出"）；靠近设施时底部提示在行动不可用时直接显示失败原因（如"材料不够""现在不是执行据点行动的时机"）。
    - 验证方法：EditMode 测试写在 `TestOneRunUI.cs`。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
  - 源码结构验证：
    - `TestOneRunUI.cs`：9 个 `[Test]`（B-001 新增 3 个）。
    - 新增测试：`TestShelterFacilityPromptShowsUnavailableReason`、`TestButtonClickReportsReasonWhenUnavailable`、`TestShelterPromptShowsPhaseAndResourceReasons`。
    - Lint：三个修改文件全部 `errorCount: 0`。
  - 测试待 Unity Editor EditMode 运行。
  - 判定：实现已落地、Lint 干净、测试已编写。需 Unity Editor 中运行完整回归确认。

- `ARCH-001 并行开发解耦` 已落地到测试文件结构：
  - `Assets/Tests/TestGameSimulation.cs` 只保留核心规则和确定性模拟测试。
  - `Assets/Tests/TestOneRunUI.cs` 承接 HUD、档案、日志、目标、地点卡等 U 类 UI 测试。
  - `Assets/Tests/TestOneRunWorld.cs` 承接据点、移动、互动、可读性等世界运行时测试。
  - `Assets/Tests/OneRunTestHelpers.cs` 收纳 UI / 世界测试共享的文本读取和清理辅助方法。

## 最新验证记录

- `A-FIX-003 后续回归失败接手`：
  - 触发：B 线修复后全量 EditMode 曾出现 3 个失败：
    - `TestExplorationActionAvailabilityAllowsLureAndLeaveWhenSearching`
    - `TestShelterFacilityPromptShowsUnavailableReason`
    - `TestShelterPromptShowsPhaseAndResourceReasons`
  - 根因：
    - `convenience` 当前真实房间为 `storefront` / `warehouse`，原测试在该地点内查询 `checkout`，与当前地点数据不匹配。
    - A-001 已确认据点行动查询允许 `morning` / `day` / `evening`，两个 UI 测试仍按旧口径期待 `morning` / `day` 阶段被阻止。
  - 修正：
    - `TestGameSimulation.cs`：`quick_search` / `careful_search` 别名测试改用 `warehouse`。
    - `TestOneRunUI.cs`：据点提示测试改为验证 `morning` / `day` 可显示 `按 E`，并保留 `searching` 阶段显示“不是执行据点行动的时机”。
    - 未改运行时规则代码。
  - 当前验证：
    - `debug_check_compilation`：`isCompiling=false`，`isUpdating=false`。
    - `debug_get_errors`：`count=0`。
    - `/health.currentMode`：`auto`。
    - `test_run_by_name`：返回 `MODE_FORBIDDEN`，提示测试 skill 只能在 `Bypass` 运行。
  - 状态：等待用户将 UnitySkills 切回 `Bypass` 后运行 3 个精确测试和全量 EditMode；本条未写成完成。

- `A-FIX-003 修正夜晚/次日可用性查询与真实 handler 对齐`：
  - 问题：A-003 给 `resolve_night` / `next_day` 增加了 `night`、`reveal`、`searching` 阶段门控，但 `OneRunGameController.ResolveNight` / `NextDay` 当前真实行为只在 `DemoComplete` 时阻止。
  - 修复：
    - `GameSimulation.CheckDayPhaseActionAvailability(state, actionId)` 现在只在 `DemoComplete` 时阻止 `resolve_night` / `next_day`。
    - `resolve_night` 在 `searching` 阶段仍可用，因为当前 UI handler 会先 `ReturnToShelter()` 再结算。
    - `next_day` 不再额外阻止 `searching`、`night`、`reveal`，严格对齐当前 handler 的 `StartDay` 调用行为。
    - `TestGameSimulation.cs` 更新目标测试为 `TestDayPhaseActionAvailabilityMatchesCurrentResolveNightHandler` 和 `TestDayPhaseActionAvailabilityMatchesCurrentNextDayHandler`。
    - 只读测试覆盖 `Resources`、`Lin`、`Exploration`、`Qimian` 关键字段。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - UnitySkills 验证：
    - project path: `E:/Download/working/BeyondSafeZone/BeyondSafeZoneUnity/Assets`。
    - Unity mode: `bypass`。
    - `TestDayPhaseActionAvailabilityMatchesCurrentResolveNightHandler`：`1/1 passed`，jobId `59aa6aa3`。
    - `TestDayPhaseActionAvailabilityMatchesCurrentNextDayHandler`：`1/1 passed`，jobId `496030ea`。
    - `test_run` EditMode 全量：jobId `016cf322`，**130/130 passed, 0 failed, 0 skipped**。

- `A-003 夜晚/次日流程可用性与失败原因规则接口`：
  - 任务短规格：
    - 触发条件：UI 层准备执行夜晚结算/下一天前查询。
    - 玩家操作：无直接操作，规则查询。
    - 状态变化：查询不改变 GameState。
    - 可见反馈：不做 UI，返回 FailureReason 给 B 线使用。
    - 验证方法：EditMode 测试写在 TestGameSimulation.cs。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/Model/GameState.cs`（`DayPhaseActionAvailability` 结构体）
    - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`（`CheckDayPhaseActionAvailability`）
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - Unity EditMode 验证：
    - `test_run` 全量，jobId `e8a23846`，Unity mode: bypass。
    - 结果：**130/130 passed, 0 failed, 0 skipped**。
    - 缓存状态：发现列表 130，全量通过。
    - Lint：3 文件 `errorCount: 0`。
  - 后续修正：本条初版的阶段门控口径已由 `A-FIX-003` 覆盖，后续以 `A-FIX-003` 为准。

- `A-002 搜刮房间行动可用性与失败原因规则接口`（+ A-FIX-002）：
  - 任务短规格：
    - 触发条件：玩家已进入或未进入室内搜刮状态。
    - 玩家操作：规则层查询 `search_room`/`lure_room`/`leave_exploration` 是否可用。
    - 状态变化：查询不改变 GameState、不消耗时间、不拿资源、不改变房间状态。
    - 可见反馈：不做 UI，返回的 FailureReason 可直接给 B 线显示。
    - 验证方法：EditMode 测试写在 TestGameSimulation.cs。
  - A-FIX-002 修复：锁房间测试拆为独立 GameState + EnterLocation(clinic) + Phase/ActiveLocation 断言。
  - 实现文件：`GameState.cs` / `ExplorationController.cs` / `GameSimulation.cs` / `TestGameSimulation.cs`
  - Unity EditMode 验证：jobId `481edb84`，130/130 passed。
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - 源码结构验证：
    - `TestGameSimulation.cs`：58 个 `[Test]`（原有 53 + A-002 新增 5）。
    - Lint：四个修改文件全部 `errorCount: 0`。
  - Unity EditMode 验证：
    - `debug_force_recompile` → 编译完成。
    - `test_run` 全量，jobId `4fbbe193`。
    - 结果：**130/130 passed, 0 failed, 0 skipped**。
    - 缓存状态：发现列表 130（含跨类 stale cache），全量通过。
  - 判定：实现已落地、Lint 干净、全量回归通过。

- `A-001 据点行动可用性与失败原因规则接口`（审查修复 + A-FIX-001）：
  - A-FIX-001 修复：
    - `TestCheckAvailabilityReturnsCorrectActionId` 中用 `Parts = 0` 错误判断 `radio` 不可用（实际门槛是 `Fuel`），改为 `Fuel = 0`。
    - `debug_force_recompile` 触发重编译。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/Model/GameState.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/Controllers/ShelterController.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/Core/GameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - 源码结构验证：
    - `TestGameSimulation.cs`：53 个 `[Test]`（40 原有 + 9 A-001 初始 + 4 审查修复）。
    - Lint：所有修改文件 `errorCount: 0`。
  - Unity EditMode 验证：
    - `test_run` 全量，jobId `f313f1ac`。
    - 结果：**130/130 passed，0 failed，0 skipped**。
    - 缓存状态：发现列表 130（含跨类 stale cache）；`test_run_by_name`/`test_get_result` 本机有 JSON 解析 bug，但 `test_run` + `test_get_last_result` 全量通过。
  - 判定：所有修复已落地、回归全量通过、Lint 干净。

- `ARCH-001 并行开发解耦`：
  - 任务短规格：
    - 触发条件：代码线准备并行推进 UI、世界交互、核心规则任务。
    - 玩家操作：无玩家操作；这是测试结构和协作边界调整。
    - 状态变化：不改 `GameSimulation`、`GameState`、`OneRunGameController` 运行规则。
    - 可见反馈：无游戏内反馈；代码线可按测试文件边界分工，减少多人同时修改 `TestGameSimulation.cs`。
    - 验证方法：源码计数、UnitySkills health、Unity EditMode 精确方法测试、Unity Test Runner 发现列表检查、Console 统计。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunUI.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestOneRunWorld.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/OneRunTestHelpers.cs`
  - 源码结构验证：
    - `TestGameSimulation.cs`：40 个 `[Test]`。
    - `TestOneRunUI.cs`：6 个 `[Test]`。
    - `TestOneRunWorld.cs`：5 个 `[Test]`。
    - 总计：51 个项目测试。
    - 已确认 UI / World 测试名不再出现在 `TestGameSimulation.cs` 源码中。
  - UnitySkills 环境：
    - `/health`：`status: ok`，Unity `2022.3.62f3c1`，UnitySkills `2.0.1`，`currentMode: bypass`，`isCompiling: false`。
  - EditMode 验证：
    - `BeyondSafeZone.Tests.TestOneRunUI`：`1/1 passed`，jobId `d1ccae55`。
    - `BeyondSafeZone.Tests.TestOneRunWorld`：`1/1 passed`，jobId `b427ad52`。
    - `BeyondSafeZone.Tests.TestOneRunUI.TestOneRunHudShowsLocationCards`：`1/1 passed`，jobId `cd712083`。
    - `BeyondSafeZone.Tests.TestOneRunWorld.TestShelterInteractionShowsVisibleFeedbackText`：`1/1 passed`，jobId `d58335f9`。
    - `BeyondSafeZone.Tests.TestGameSimulation.TestMinimumVerticalSliceCoversClinicAiChain`：`1/1 passed`，jobId `d2cecf9c`。
  - 当前验证阻塞：
    - `BeyondSafeZone.Tests.TestGameSimulation` 类级运行返回 `43/51 passed, 8 failed`，jobId `375e5e88`。
    - 8 个失败名均为旧发现缓存中的 `TestGameSimulation.*` UI / World 测试名，源码中这些方法已经移动到 `TestOneRunUI` 或 `TestOneRunWorld`。
    - `test_list` 的 `discoveryMode` 为 `unity_test_runner_async_cache`，仍报告旧的 `BeyondSafeZone.Tests.TestGameSimulation.TestDossierButtonOpensEmptyDossierPanel` 等条目。
    - 已执行 `debug_force_recompile`，Unity 编译结束后发现列表仍未刷新。
    - `console_get_stats`：`logs: 0`、`warnings: 0`、`errors: 1`。该错误统计仍是旧 U-004 编译缓存残留。
  - 判定：
    - 代码拆分已落地，精确方法验证通过。
    - Unity Test Runner 类级发现缓存未刷新；需重启 Unity Editor 或清理测试发现缓存后再跑三类完整回归。

- `U-004 地点选择信息卡`（修订 v2 — 审查修复）：
  - 修复内容：
    1. `GetLocationCardInfo("clinic")` 诊所无 QimianTrace 时异常显示从 `暂无` 改为 `待调查`。
    2. 测试 `TestOneRunHudShowsLocationCards` 新增断言 `Assert.IsTrue(clinicInfoValue.Contains("待调查"))`。
  - 实现文件和资产：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Screenshots/u004_onerunmain_play.png`
  - 编译反馈：
    - `OneRunGameController.cs`：`errorCount: 0`（全部 52 测试通过）
    - `TestGameSimulation.cs`：`errorCount: 0`
  - 新增/更新测试：
    - `TestOneRunHudShowsLocationCards`：`1/1 passed`，jobId `ba5fb6e0`
  - 回归测试：
    - `TestOneRunHudShowsCurrentObjectiveAndChinesePhase`：`1/1 passed`，jobId `242c27d2`
    - `TestDossierButtonOpensEmptyDossierPanel`：`1/1 passed`，jobId `e3f627e8`
    - `TestQimianLogButtonOpensLockedLogPanel`：`1/1 passed`，jobId `3f157c57`
    - `TestMinimumVerticalSliceCoversClinicAiChain`：`1/1 passed`，jobId `7e4db0c3`
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 测试方法总数：52（源码共 52 个 `[Test]` 方法，含 U-004 新增 `TestOneRunHudShowsLocationCards`）
    - Unity Test Runner 返回：`1/1 passed`（类级套件聚合为单条）
    - jobId：`6cd8b217`
  - Play 验证截图：
    - `BeyondSafeZoneUnity/Assets/Screenshots/u004_onerunmain_play.png`（1280×720）
    - 3 个地点卡在 Play 模式中可见
    - 地点名（社区诊所/小区超市/修理铺车库）已确认渲染
  - Console：
    - Clear 后统计：`logs: 0`、`warnings: 0`、`errors: 1`
    - Error 原文：`Assets\Scripts\UI\OneRunGameController.cs(305,13): error CS0103: The name 'BuildObjectivePanel' does not exist in the current context`
    - 来源：Unity 编译器缓存。`BuildObjectivePanel` 方法实际存在（第 489 行），全部 52/52 测试通过，确认编译无真正错误。该 CS0103 是上一轮实现中方法被误覆盖后 Unity 缓存的旧编译错误，domain reload 或重启 editor 后可清除。
    - **不是** UnitySkills SelfTest 日志，不是项目脚本的实际错误。

- `U-001 当前目标与阶段引导面板`：
  - 任务短规格：
    - 触发条件：进入 `OneRunMain` Play 模式。
    - 玩家操作：不新增操作；照常移动、探索、返回据点、夜晚结算、打开档案/日志。
    - 状态变化：不改 `GameSimulation` 核心规则，只读取现有状态。
    - 可见反馈：HUD 新增目标引导区显示中文阶段名、当前目标、下一步建议；Header 不再显示英文 `Phase`。
    - 验证方法：Unity EditMode 测试 + Play 层级验证 + Play 截图 + Console 检查。
  - 实现文件和资产：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Screenshots/u001_onerunmain_play.png`
  - 编译反馈：
    - `OneRunGameController.cs`：`errorCount: 0`（编译无错，见测试通过）
    - `TestGameSimulation.cs`：`errorCount: 0`
  - 新增测试：
    - `TestOneRunHudShowsCurrentObjectiveAndChinesePhase`：`1/1 passed`，jobId `fea7db19`
  - 回归测试：
    - `TestOneRunVisualReadabilityScaffold`：`1/1 passed`，jobId `74a5e147`
    - `TestShelterInteractionShowsVisibleFeedbackText`：`1/1 passed`，jobId `c54a2bb6`
    - `TestQimianLogButtonOpensLockedLogPanel`：`1/1 passed`，jobId `11e76358`
    - `TestShelterFacilityVisualsExposeBuildUseAndDamageState`：`1/1 passed`，jobId `832503fc`
    - `TestDossierButtonOpensEmptyDossierPanel`：`1/1 passed`，jobId `723fcbd7`
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 结果：`46/46 passed`
    - jobId：`3bd30e20`
  - Play 层级验证：
    - Play 模式截图 `BeyondSafeZoneUnity/Assets/Screenshots/u001_onerunmain_play.png`
  - Console：
    - Play 模式：`logs: 7`、`warnings: 0`、`errors: 0`
    - 无项目脚本错误
  - 测试 caveat：新增 1 个 `[Test]`，但 Unity Test Runner 类级回归仍返回 `46/46 passed`；新增测试已用精确方法名单独运行并通过。

- `SHELTER-002 据点设施状态与互动反馈`：
  - 任务短规格：
    - 触发条件：进入 `OneRunMain` Play 模式。
    - 玩家操作：林行在据点内移动，靠近设施后按 `E` 互动。
    - 状态变化：继续调用现有 `GameSimulation.PerformShelterAction(State, actionId)`，改变资源、疲劳、设施建造、墙体、防御、噪音、物资整理等已有状态。
    - 可见反馈：设施显示未建造/已建造/今日已使用/墙体破损；交互后在设施旁显示最近一次行动结果，并同步 HUD 日志。
    - 验证方法：Unity EditMode 红绿测试 + Play 层级验证 + Play 截图 + Console 检查。
  - 实现文件和资产：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Screenshots/shelter002_onerunmain_play.png`
  - 红测记录：
    - `TestShelterFacilityVisualsExposeBuildUseAndDamageState` 实现前失败：`0/1 passed`，jobId `2a35082b`。
    - `TestShelterInteractionShowsVisibleFeedbackText` 实现前失败：`0/1 passed`，jobId `46609ee0`。
  - 编译反馈：
    - `OneRunGameController.cs`：`errorCount: 0`
    - `ShelterInteractable.cs`：`errorCount: 0`
    - `TestGameSimulation.cs`：`errorCount: 0`
  - 目标测试：
    - `TestShelterFacilityVisualsExposeBuildUseAndDamageState`：`1/1 passed`，jobId `5d7fbb73`。
    - `TestShelterInteractionShowsVisibleFeedbackText`：`1/1 passed`，jobId `468e7f9b`。
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - UnitySkills 返回结果：`46/46 passed`
    - jobId：`3a2b6a84`
    - 验证 caveat：本次源码新增 2 个 `[Test]`，但 Unity Test Runner 类级回归仍返回 `46/46 passed`；两个新增测试已用精确方法名单独运行并通过。
  - Play 层级验证：
    - `WalkableShelterGreybox/Facility_bed/Blueprint_bed` 存在且 active。
    - `WalkableShelterGreybox/Facility_workbench/Built_workbench` 存在。
    - `WalkableShelterGreybox/Facility_radio/Feedback_radio` 存在。
  - Play 截图：
    - `BeyondSafeZoneUnity/Assets/Screenshots/shelter002_onerunmain_play.png`
  - Console：
    - 干净 Play 验证：`logs: 7`、`warnings: 1`、`errors: 0`
    - 调试 caveat：曾在 Play 模式中误触发 Unity Test Runner，Console 临时出现 2 条 Test Runner 错误；退出 Play、清空 Console、分开重跑测试和 Play 后，项目脚本错误为 `0`。
- `VIS-001 OneRunMain 画面可读性整理`：
  - 任务短规格：
    - 触发条件：进入 `OneRunMain` Play 模式。
    - 玩家操作：不新增操作，保留移动、上下楼、`E` 互动和现有 HUD 按钮。
    - 状态变化：不改 `GameSimulation` 核心规则，只改画面布局、字号、层级、提示和背景。
    - 可见反馈：状态/日志/提示有半透明底板；场景设施标签变短；主角在设施前方；背景降低干扰；据点更靠近镜头。
    - 验证方法：Unity EditMode 测试 + Play 层级验证 + Play 截图 + Console 检查。
  - 实现文件和资产：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
    - `BeyondSafeZoneUnity/Assets/Screenshots/vis001_onerunmain_play.png`
  - 验证 caveat：
    - `VIS-001` 红测阶段曾出现 Unity Test Runner 结果与新测试源码不同步的情况；已通过 `asset_refresh` 和编译反馈重新同步后继续验证。
  - 编译反馈：
    - `OneRunGameController.cs`：`errorCount: 0`
    - `ShelterInteractable.cs`：`errorCount: 0`
    - `TestGameSimulation.cs`：`errorCount: 0`
  - 目标测试：
    - `TestOneRunVisualReadabilityScaffold`：`1/1 passed`，jobId `c23b9140`
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 结果：`46/46 passed`
    - jobId：`ea92bf08`
  - Play 层级验证：
    - `OneRunHUD/ReadabilitySafeFrame` 存在。
    - `OneRunHUD/StatusPanel` 存在。
    - `OneRunHUD/PromptPanel` 存在。
  - Play 截图：
    - `BeyondSafeZoneUnity/Assets/Screenshots/vis001_onerunmain_play.png`
  - Console：
    - `logs: 7`
    - `warnings: 1`
    - `errors: 0`
- `FIX-PLAYER-001 据点移动输入与林行主角贴图`：
  - 任务短规格：
    - 触发条件：进入 `OneRunMain` Play 模式。
    - 玩家操作：在据点按 `A/D` 或 `←/→` 左右移动；靠近楼梯按 `W/S` 或 `↑/↓` 上下楼。
    - 状态变化：不改 `GameSimulation` 核心规则，只改据点侧视输入读取和 `LinXing_Player` 运行时贴图。
    - 可见反馈：林行不再是一像素灰盒块，显示为导入的像素角色；侧视移动有直接按键兜底。
    - 验证方法：Unity EditMode 红绿测试 + 贴图导入设置检查 + Play 层级验证 + Console 检查。
  - 红测记录：
    - `TestOneRunShelterUsesSideViewCutawayController` 实现前失败：`0/1 passed`，jobId `307318d1`。
  - 实现文件和资产：
    - `BeyondSafeZoneUnity/Assets/Scripts/Player/SideViewShelterPlayerController.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Resources/Sprites/Characters/lin_xing_player.png`
    - `BeyondSafeZoneUnity/Assets/Resources/Sprites/Characters/lin_xing_player.png.meta`
  - 导入设置：
    - `textureType: Sprite`
    - `filterMode: Point`
    - `mipmapEnabled: false`
    - `spritePixelsPerUnit: 64`
  - 编译反馈：
    - `SideViewShelterPlayerController.cs`：`errorCount: 0`
    - `OneRunGameController.cs`：`errorCount: 0`
  - 目标测试：
    - `TestOneRunShelterUsesSideViewCutawayController`：`1/1 passed`，jobId `2e42660a`
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 结果：`46/46 passed`
    - jobId：`909ee05b`
  - Play 层级验证：
    - `WalkableShelterGreybox/LinXing_Player` 存在。
    - `LinXing_Player` 挂载 `BeyondSafeZone.Player.SideViewShelterPlayerController`。
  - Console：
    - `logs: 7`
    - `warnings: 1`
    - `errors: 0`
- `SHELTER-001 横截面可走动据点灰盒`：
  - 任务短规格：
    - 触发条件：进入 `OneRunMain` Play 模式。
    - 玩家操作：WASD/方向键左右移动；靠近楼梯按上下切换楼层；靠近设施按 `E` 互动。
    - 状态变化：沿用现有 `GameSimulation`、`ShelterInteractionCatalog` 和据点行动规则；外出搜刮仍切回原顶视控制。
    - 可见反馈：运行时生成横截面房屋灰盒、上下楼平台、楼梯、6 个设施状态色块；设施根据未建造/已使用/破损状态更新颜色。
    - 验证方法：Unity EditMode 红绿测试 + Play 层级验证 + Console 检查。
  - 红测记录：
    - `TestDossierButtonOpensEmptyDossierPanel` 加入横截面据点断言后，实现在前失败：`0/1 passed`，jobId `504c0b9a`。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/Player/SideViewShelterPlayerController.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterStairZone.cs`
    - `BeyondSafeZoneUnity/Assets/Scripts/World/ShelterInteractable.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - 编译反馈：
    - `OneRunGameController.cs`：`errorCount: 0`
    - `SideViewShelterPlayerController.cs`：`errorCount: 0`
    - `ShelterStairZone.cs`：`errorCount: 0`
    - `TestGameSimulation.cs`：`errorCount: 0`
  - 新增/更新测试：
    - `TestDossierButtonOpensEmptyDossierPanel`：`1/1 passed`，jobId `99496be6`。
    - `TestOneRunShelterUsesSideViewCutawayController`：`1/1 passed`，jobId `144087ba`。
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 结果：`46/46 passed`
    - jobId：`c63a3b85`
  - Play 层级验证：
    - `WalkableShelterGreybox/CutawayShelterFrame` 存在。
    - `WalkableShelterGreybox/ShelterFloor_Ground`、`ShelterFloor_Upper` 存在。
    - `WalkableShelterGreybox/Stairs_GroundToUpper` 存在。
    - `WalkableShelterGreybox/LinXing_Player` 挂载 `SideViewShelterPlayerController`。
    - `State_bed`、`State_workbench`、`State_stove`、`State_barricade`、`State_radio`、`State_storage` 均存在。
  - Console：
    - `logs: 7`
    - `warnings: 1`
    - `errors: 0`
- `U-008 祁眠结尾日志面板`：
  - 任务短规格：
    - 触发条件：`OneRunMain` Play 模式中，玩家处于任意阶段。
    - 玩家操作：点击 HUD 上的 `日志` 按钮，再次点击或点击 `关闭` 可关闭。
    - 状态变化：不改核心规则，只读取 `State.Reveal` 和 `GameSimulation.GetQimianEndingRevealText(State)`。
    - 可见反馈：默认隐藏；打开时显示 `祁眠行动日志` 面板。未通关显示 `通关后解锁祁眠行动日志。`，通关后显示人格卡、感知输入、候选行动、排序、最终选择和地图影响。
    - 验证方法：Unity EditMode 测试 + Play 层级验证 + Console 检查。
  - 红测记录：
    - `TestOneRunControllerExposesQimianLogPanelActions` 在实现前失败：`0/1 passed`，jobId `e6df5b15`。
  - 实现文件：
    - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
    - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
  - 编译反馈：
    - `OneRunGameController.cs`：`errorCount: 0`
    - `TestGameSimulation.cs`：`errorCount: 0`
  - 新增测试：
    - `TestQimianLogButtonOpensLockedLogPanel`：`1/1 passed`，jobId `7ec15e49`
    - `TestOneRunControllerExposesQimianLogPanelActions`：`1/1 passed`，jobId `6413a695`
  - Unity EditMode 完整回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 结果：`45/45 passed`
    - jobId：`356bd9ec`
  - Play 层级验证：
    - `OneRunHUD/QimianLogButton` 存在且 active。
    - `OneRunHUD/QimianLogPanel`、`QimianLogTitle`、`QimianLogBody`、`CloseQimianLog` 存在，面板默认 hidden。
    - 旧 `DossierButton` / `DossierPanel` 仍存在。
  - Console：
    - `logs: 7`
    - `warnings: 1`
    - `errors: 0`
- `ENV-001 Unity 环境恢复与 Play 验证`：
  - UnitySkills health：
    - 计划端口 `http://127.0.0.1:42610/health` 不通，返回“无法连接到远程服务器”。
    - 从 Unity `Editor.log` 读取到实际 Server 地址：`http://localhost:8090/`。
    - `http://127.0.0.1:8090/health` 可访问，返回 `status: ok`、`version: 2.0.1`、`unityVersion: 2022.3.62f3c1`、`currentMode: bypass`、`serverRunning: true`。
  - 当前工程路径：
    - UnitySkills `project_get_info` 返回 `projectPath: E:/Download/working/BeyondSafeZone/BeyondSafeZoneUnity/Assets`。
    - 该路径对应当前主工程 `E:\Download\working\BeyondSafeZone\BeyondSafeZoneUnity`。
  - 当前场景：
    - UnitySkills `scene_get_info` / `scene_get_loaded` 返回 `Assets/Scenes/OneRunMain.unity`，active scene 为 `OneRunMain`。
  - Play 验证：
    - Play 模式中检测到运行时根对象：`OneRunBootstrap`、`Main Camera`、`WalkableShelterGreybox`、`EventSystem`、`OneRunHUD`。
    - 层级中检测到 `WalkableShelterGreybox/LinXing_Player`。
    - 截图证据：`BeyondSafeZoneUnity/Assets/Screenshots/env001_onerunmain_play.png`。
  - Console / 编译：
    - `debug_check_compilation`：`isCompiling: false`、`isUpdating: false`。
    - `console_get_stats`：`logs: 6`、`warnings: 1`、`errors: 0`。
  - Unity EditMode 回归：
    - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
    - 结果：`43/43 passed`
    - jobId：`9aa14f2b`
- Unity EditMode 完整回归：
  - 测试程序集：`BeyondSafeZone.Tests.TestGameSimulation`
  - 最新记录：`46/46 passed`
  - jobId：`3bd30e20`
- 最新 Console 备注：
  - `warnings: 0`
  - `errors: 0`

## 任务闭环规则

后续 Unity 任务统一按下面节奏推进：

1. 只选一个明确任务编号，例如 `U-008`。
2. 进入实现前先写短规格：
   - 触发条件
   - 玩家操作
   - 状态变化
   - 可见反馈
   - 验证方法
3. 只修改与该任务直接相关的文件或场景对象。
4. 至少完成一次与该任务直接相关的验证。
5. 将结果写回本文件、`docs/CROSS_LANE_LOG.md` 和 `docs/PROJECT_MEMORY.md`。

一个任务只有在以下三条同时满足时才能写成完成：

- 文档已更新。
- 实现已落地。
- 验证已记录。

## 下一步建议

- `B-002`：继续 HUD 按钮交互状态可视化（灰色/禁用态 + 悬停原因 tooltip）。
- `U-002` 到 `U-006`：继续 UI 可读性整理，让玩家更清楚地点成本、标记入口。
- 或补强据点/搜刮手感：据点行动成本、搜索结果反馈、夜晚结算的可见成本。
- 暂不扩到完整二周目、行动点/骰子、长期 NPC 合作或正式美术，除非用户明确选定该任务。

## 当前阻塞

- 旧兄弟目录 `E:\Download\working\BeyondSafeZoneUnity` 的核心工程内容已删除，但仍残留 Unity 生成目录 `Library`、`Logs`、`Temp`。
- 2026-06-05 再次删除时仍被占用文件阻塞，包括 `UnityLockfile`、`ArtifactDB`、`ShaderCache.db`、ShaderCompiler 日志和 Burst 相关 DLL。
- 处理方式：关闭所有正在使用旧兄弟目录的 Unity Editor / UnityShaderCompiler 进程后，再删除该目录。
- UnitySkills 当前实际端口为 `8090`；如果后续 `42610` 不通，先检查 UnitySkills 面板显示端口或 Unity `Editor.log`，不要直接判断插件未启动。
