# Unity 迁移状态报告

> 更新时间：2026-06-04
> 项目：BeyondSafeZone（Godot → Unity 迁移）
> 状态：**Unity Editor 已验证；正式一周目灰盒场景 `OneRunMain` 进行中**

---

## 2026-06-04 执行记录：OneRunMain HUD 布局热修

任务编号：UI 热修（非新增玩法任务）

功能名：
- 修正 `OneRunMain` 运行时 HUD 重叠、按钮溢出、世界标签过大的显示问题

触发条件：
- 进入 `Assets/Scenes/OneRunMain.unity` Play 模式
- `OneRunGameController.Start()` 运行时生成 `OneRunHUD`

玩家操作：
- 点击 Play 后查看 Game 视图

系统状态变化：
- `OneRunHUD` 的 `CanvasScaler` 增加固定参考分辨率 `1280x720` 与宽高折中缩放。
- HUD 文本分区：
  - `Header` 顶部居中
  - `Status` 左上
  - `Log` 右上
  - `Prompt` 底部居中
- 底部按钮从单行固定横排改成两行居中，避免在当前 Game 窗口宽度下向右溢出。
- 世界空间设施/搜索点标签字号从 `2.8f` 调整到 `0.75f`，避免设施名压住据点灰盒，同时保持可读。

玩家可见反馈：
- 截图中“状态文字压在据点中间、设施标签糊成一团、按钮右侧溢出屏幕”的问题应明显缓解。
- 状态、日志、提示、按钮现在分别占据屏幕不同区域。

涉及文件：
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\UI\OneRunGameController.cs`

验证方法：
- 编译反馈检查 `OneRunGameController.cs`。
- Play 模式重新生成运行时 HUD。
- 读取运行时 `RectTransform` 验证关键 UI 分区。
- 检查 Unity Console。
- 退出 Play 后在 EditMode 跑完整 `TestGameSimulation` 回归。

验证结果：
- `OneRunGameController.cs` 编译反馈：`errorCount: 0`。
- Play 模式运行时 RectTransform：
  - `OneRunHUD/Header`：顶部居中，`sizeDelta 900x44`
  - `OneRunHUD/Status`：左上，`anchoredPosition (18, -72)`，`sizeDelta 360x132`
  - `OneRunHUD/Log`：右上，`anchoredPosition (-18, -72)`，`sizeDelta 390x150`
  - `OneRunHUD/Prompt`：底部居中，`anchoredPosition (0, 4)`，`sizeDelta 780x24`
  - `ExploreClinic` / `LeaveHelpMark` 在上排，按钮 y 为 `84`
  - `ResolveNight` / `NextDay` 在下排，按钮 y 为 `40`
- Play 验证时 Console：`warnings: 0`、`errors: 0`。
- 误操作记录：曾在 Play 模式启动 Unity Test Runner，jobId `68504ce3`，失败原因为 `This cannot be used during play mode`，不是玩法脚本断言失败；随后清空 Console 并在 EditMode 重跑。
- EditMode 完整回归：`BeyondSafeZone.Tests.TestGameSimulation` 为 `41/41 passed`，jobId `cb51ad29`。
- 最终 Console：`warnings: 0`、`errors: 0`。

阻塞/偏差：
- 本次没有做正式 UI 视觉设计，只修运行时灰盒布局可读性。
- 仍建议用户在 Game 视图重新 Play 看一次实际画面；若目标窗口更窄，按钮还可以继续改为三行或改成左侧竖排。

下一步：
- 若画面已可读，可以继续推进玩法；若还觉得乱，下一步应做 `U-001/U-002/U-008` 的正式 HUD/日志面板布局，而不是继续临时堆按钮。

---

## 2026-06-04 执行记录：C-010 最小测试

任务编号：C-010

功能名：
- 最小纵切集成测试
- 覆盖 Day 1 循环、诊所异常、玩家标记、祁眠读取/回应、异常档案、Day 15 结尾日志链路

触发条件：
- Unity EditMode 运行 `BeyondSafeZone.Tests.TestGameSimulation`

玩家操作：
- 测试模拟 Day 1 白天进入地点、搜索、返回据点、夜晚结算。
- 测试模拟 Day 5 进入诊所、搜索异常线索、留下求助标记、夜晚结算。
- 测试模拟 Day 15 终局结算并查看结尾日志文本。

系统状态变化：
- 本任务不新增生产规则，只新增回归测试 `TestMinimumVerticalSliceCoversClinicAiChain()`。
- 测试串联验证：
  - Day 1 探索能进入地点、获得资源、返回据点并推进到 Day 2。
  - 诊所 `exam_a` 搜索会写入异常档案。
  - `clinic/help` 玩家标记会写入 `state.PlayerMarks`。
  - Day 5 夜晚祁眠会读取求助标记，并产生匿名药品回应。
  - 地点卡和异常档案能显示祁眠异常、匿名药品。
  - Day 15 `Reveal.Summary` 能解释人格卡、感知输入、最终选择和地图影响。

玩家可见反馈：
- 搜索返回文本包含 `带回`。
- 夜晚返回文本包含 `求助标记`、`匿名药品`。
- 地点卡包含 `祁眠异常`。
- 异常档案文本包含 `诊所隔离记录`、`匿名药品`。
- 结尾日志包含 `人格卡`、`感知输入`、`最终选择`、`地图影响`、`社区诊所`、`求助标记`、`匿名药品`。

涉及文件：
- Unity 测试：`E:\Download\working\BeyondSafeZoneUnity\Assets\Tests\TestGameSimulation.cs`

验证方法：
- 编译反馈检查测试文件。
- 运行新增 Unity EditMode 单测。
- 运行完整 Unity EditMode 回归。
- 检查 Unity Console 统计。

验证结果：
- `TestGameSimulation.cs` 编译反馈：`errorCount: 0`。
- Unity Console 初始检查：`warnings: 0`、`errors: 0`。
- 新增测试 `TestMinimumVerticalSliceCoversClinicAiChain` 通过：`1/1 passed`，jobId `1db8f604`。
- 完整 Unity EditMode 回归：`BeyondSafeZone.Tests.TestGameSimulation` 为 `41/41 passed`，jobId `6103dd69`。

阻塞/偏差：
- 本任务是测试闭环整理，新增测试直接通过，说明 C-005 到 C-009 的既有实现已能串成最小纵切链路。
- 因为没有生产代码缺口，本任务未修改玩法脚本，也没有制造失败测试。

下一步：
- P0 程序任务 `C-005` 到 `C-010` 的诊所 AI 最小链路已有回归保护。后续可转向 UI 线 `U-008` 祁眠日志面板可读性，或开始做下一轮最小可玩体验补强。

---

## 2026-06-04 执行记录：C-009 结尾日志

任务编号：C-009

功能名：
- 结尾祁眠日志解释诊所标记因果链
- 展示祁眠人格卡、感知输入、候选行动、排序理由、最终选择、共享地图影响

触发条件：
- Day 15 或等价终局结算后
- `state.Reveal.Unlocked == true`
- 一周目中 `clinic` 存在 `help` 标记，并已触发 C-008 匿名药品反馈

玩家操作：
- 玩家完成终局夜晚结算。
- 通关后查看结尾日志文本。

系统状态变化：
- `TextRenderer.GetQimianEndingRevealText()` 根据当前 `GameState` 生成祁眠结构化终局解释文本。
- `GameSimulation.GetQimianEndingRevealText()` 暴露纯文本委托，供终局和 UI 读取。
- `NightController.Resolve()` 在 Day 15 终局时把祁眠结构化解释追加进 `state.Reveal.Summary`。
- 规则本身未改动：祁眠仍按既有 C-007 / C-008 感知与回应链路执行。

玩家可见反馈：
- 结尾日志显示：
  - `人格卡`
  - `感知输入`
  - `候选行动`
  - `排序理由`
  - `最终选择`
  - `地图影响`
- 文本明确显示 `社区诊所`、`求助标记`、`匿名药品`，解释“林行留下的求助标记影响过祁眠”。

涉及文件：
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Core\TextRenderer.cs`
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Core\GameSimulation.cs`
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Controllers\NightController.cs`
- Unity 测试：`E:\Download\working\BeyondSafeZoneUnity\Assets\Tests\TestGameSimulation.cs`

验证方法：
- TDD：先补 Unity EditMode 测试，确认终局日志缺少结构化解释时失败。
- Unity EditMode 单测验证终局 `Reveal.Summary` 包含人格卡、感知输入、候选行动、排序、最终选择、地图影响、社区诊所、求助标记、匿名药品。
- Unity EditMode 完整回归。
- Play / Console 轻量验证 Unity 运行态无 warning / error。

验证结果：
- `TestEndingRevealExplainsClinicHelpMarkCausality` 先失败：`0/1 passed`，jobId `9b3f6d78`。
- 补实现后同一测试通过：`1/1 passed`，jobId `38843bb8`。
- 完整 Unity EditMode 回归：`BeyondSafeZone.Tests.TestGameSimulation` 为 `40/40 passed`，jobId `061afa85`。
- 相关脚本编译反馈：
  - `TextRenderer.cs`：`errorCount: 0`
  - `GameSimulation.cs`：`errorCount: 0`
  - `NightController.cs`：`errorCount: 0`
  - `TestGameSimulation.cs`：`errorCount: 0`
- Play 运行态 Console：`warnings: 0`、`errors: 0`。

阻塞/偏差：
- 本任务只完成文字型结尾日志解释，不制作回放动画、独立档案 UI 面板或二周目内容。
- 当前日志为确定性文本生成；后续若接入 PlayKit.ai，只能作为文本润色层，不能改动本地规则结算。

下一步：
- C-009 已形成最小闭环。后续可转向 `C-010` 最小测试整理，或从 UI 线补 `U-008` 祁眠日志面板的显示体验。

---

## 2026-06-04 执行记录：C-008 诊所反馈

任务编号：C-008

功能名：
- 诊所 `help` 标记触发祁眠匿名药品回应
- 次日 HUD / 共享地图 / 异常档案显示回应痕迹

触发条件：
- `Day >= 5`
- `clinic` 已存在 `help` 玩家标记
- 玩家触发夜晚结算

玩家操作：
- 在 `OneRunMain` Play 模式中点击 `下一天` 到 Day 5。
- 点击 `去诊所`。
- 点击 `留下求助`。
- 点击 `夜晚结算`。

系统状态变化：
- `QimianController.RespondToClinicHelpMark()` 读取 `clinic/help` 标记。
- `clinic.Resources["meds"]` 增加 1，表示匿名药品留在诊所共享地图状态里。
- `clinic.QimianTrace` 变为 `true`，`clinic.Icons` 增加 `qimian`。
- `state.AnomalyDossier` 新增诊所匿名药品 / 浅箭头回应记录。
- `state.Qimian.PublicClues` 和 `state.Qimian.Log` 新增“响应玩家标记：社区诊所”记录。

玩家可见反馈：
- 夜晚结算后 HUD Log 显示：`社区诊所出现匿名药品：求助标记旁边多了一条浅箭头，像是有人读懂后留下的回应。`
- 后续地点卡可通过 `GameSimulation.GetLocationCardText(state, "clinic")` 看到 `祁眠异常`。
- 异常档案可通过 `GameSimulation.GetAnomalyDossierText(state)` 看到匿名药品 / 标记回应记录。

涉及文件：
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Controllers\QimianController.cs`
- Unity 测试：`E:\Download\working\BeyondSafeZoneUnity\Assets\Tests\TestGameSimulation.cs`

验证方法：
- TDD：先补 Unity EditMode 测试，确认匿名药品 / 档案反馈缺失时失败。
- Unity EditMode 单测验证诊所药品 +1、祁眠痕迹、异常档案、夜晚返回文本。
- Unity EditMode 完整回归。
- `OneRunMain` Play 路径验证：Day 5 → 诊所 → 留下求助 → 夜晚结算 → 读取 HUD Log。

验证结果：
- `TestClinicHelpMarkCreatesAnonymousMedicineFeedback` 先失败：`0/1 passed`，jobId `c3d25c58`。
- 补实现后同一测试通过：`1/1 passed`，jobId `39b33337`。
- 完整 Unity EditMode 回归：`BeyondSafeZone.Tests.TestGameSimulation` 为 `39/39 passed`，jobId `880a4edd`。
- Play 验证：
  - Header 到达 `Day 5  Phase morning`。
  - `ExploreClinic`、`LeaveHelpMark`、`ResolveNight` 按钮事件均调用成功。
  - HUD Log 显示 `林行在社区诊所留下求助标记。`
  - 夜晚结算后 HUD Log 显示 `昨夜线索：社区诊所附近的求助标记被人轻轻描深了一笔。 社区诊所出现匿名药品：求助标记旁边多了一条浅箭头，像是有人读懂后留下的回应。`
  - Header 推进到 `Day 6  Phase morning`。
  - Unity Console：`warnings: 0`、`errors: 0`。

阻塞/偏差：
- 本任务完成最小 C-008：匿名药品 / 浅箭头回应已进入共享地图状态、HUD 日志和异常档案。
- 本任务未制作正式像素药包素材，也未做独立档案 UI 面板；当前仍通过 HUD 文本 / TextRenderer 暴露。

下一步：
- 推进 `C-009`：结尾日志展示祁眠人格卡、输入、候选行动、排序、最终选择、地图影响，并明确解释“林行留下的求助标记影响过祁眠”。

---

## 2026-06-04 执行记录：C-007 祁眠读取诊所 help 标记

任务编号：C-007

功能名：
- 祁眠读取诊所 `help` 标记
- 夜晚 HUD 显示祁眠已感知求助标记的公开线索

触发条件：
- `Day >= 5`
- `clinic` 已存在 `help` 玩家标记
- 玩家触发夜晚结算

玩家操作：
- 在 `OneRunMain` Play 模式中点击 `下一天` 到 Day 5。
- 点击 `去诊所`。
- 点击 `留下求助`。
- 点击 `夜晚结算`。

系统状态变化：
- `QimianController.ResolveForDay()` 在 Day 5 后读取 `state.PlayerMarks` 中可感知的 `help` 标记。
- `QimianController` 将诊所求助标记写入 `state.Qimian.Log` 的 `AiReplay`。
- 同一条感知结果写入 `state.Qimian.PublicClues`，供夜晚 HUD 反馈读取。
- `GameSimulation.SleepAndResolveNight()` 会把本次夜晚新增、且尚未被夜晚结算文本显示过的公开线索追加到返回文本。

玩家可见反馈：
- HUD 日志在夜晚结算后显示：`昨夜线索：社区诊所附近的求助标记被人轻轻描深了一笔。`
- HUD 日志同时保留 Day 5 固定祁眠苏醒线索：`远处旧楼有一扇门从里面被打开，又被人小心合上。`
- 固定祁眠苏醒线索不再在 `昨夜` 和 `昨夜线索` 中重复显示。

涉及文件：
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Controllers\QimianController.cs`
- Unity 脚本：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Core\GameSimulation.cs`
- Unity 测试：`E:\Download\working\BeyondSafeZoneUnity\Assets\Tests\TestGameSimulation.cs`

验证方法：
- TDD：先补 Unity EditMode 测试，确认缺少 HUD 可见反馈时失败。
- Unity EditMode 单测验证祁眠日志记录诊所求助标记。
- Unity EditMode 单测验证夜晚返回文本包含诊所求助标记线索。
- Unity EditMode 单测验证固定公开线索不会重复显示。
- Unity EditMode 完整回归。
- `OneRunMain` Play 路径验证：Day 5 → 诊所 → 留下求助 → 夜晚结算 → 读取 HUD Log。

验证结果：
- 先前 C-007 日志读取测试红绿记录：
  - `TestQimianReadsClinicHelpMarkOnWakeNight` 先失败：`0/1 passed`，jobId `0f1fd5fb`。
  - 补实现后通过：`1/1 passed`，jobId `2f745a1c`。
  - 当时完整回归：`36/36 passed`，jobId `bb9999ab`。
- 本次 HUD 可见反馈测试：
  - `TestNightResultShowsQimianReadClinicHelpMark` 先失败：`0/1 passed`，jobId `a7bbb122`。
  - 补实现后通过：`1/1 passed`，jobId `793dfd6c`。
- 本次重复线索修正测试：
  - `TestNightResultDoesNotDuplicateExistingQimianPublicClue` 先失败：`0/1 passed`，jobId `5a2a2d8e`。
  - 补过滤后通过：`1/1 passed`，jobId `900cc0f8`。
- 完整 Unity EditMode 回归：`BeyondSafeZone.Tests.TestGameSimulation` 为 `38/38 passed`，jobId `f6bef90c`。
- Play 验证：
  - Header 到达 `Day 5  Phase morning`。
  - `ExploreClinic`、`LeaveHelpMark`、`ResolveNight` 按钮事件均调用成功。
  - HUD Log 显示 `林行在社区诊所留下求助标记。`
  - 夜晚结算后 HUD Log 显示 `昨夜线索：社区诊所附近的求助标记被人轻轻描深了一笔。`
  - Header 推进到 `Day 6  Phase morning`。
  - Unity Console：`warnings: 0`、`errors: 0`。

阻塞/偏差：
- 本任务只完成 C-007 的读取与可见反馈，不包含匿名药品、资源增减、档案验证或次日诊所物件变化。
- `C-008 匿名药品 / 次日反馈` 仍未完成。

下一步：
- 推进 `C-008`：在 C-007 已验证的基础上，让祁眠对诊所 `help` 标记产生匿名药品或明确回应痕迹，并把次日反馈接入地图 / HUD / 档案。

---

## 2026-06-04 最新执行记录：正式一周目主场景

任务编号：Unity 正式一周目灰盒接入 / P0 交互链路前置

功能名：
- 正式场景 `Assets/Scenes/OneRunMain.unity`
- 可走动据点运行时生成
- 诊所/超市/车库顶视搜刮灰盒入口
- 一周目 AI 可读玩法入口：在当前搜刮地点留下 `help` 求助标记

触发条件：
- 打开 Unity 项目 `E:\Download\working\BeyondSafeZoneUnity`
- 当前场景为 `Assets/Scenes/OneRunMain.unity`
- 进入 Play

玩家操作：
- 在 HUD 点击 `去诊所`
- 进入 `ScavengeGreybox_clinic`
- 点击 `留下求助`

系统状态变化：
- `OneRunMain` 由 `OneRunBootstrap` 上的 `OneRunGameController` 运行时生成正式灰盒对象。
- Play 后生成 `WalkableShelterGreybox`、`LinXing_Player`、六个 `Facility_*` 和 `OneRunHUD`。
- 点击 `去诊所` 后生成 `ScavengeGreybox_clinic`，并隐藏 `WalkableShelterGreybox`。
- 诊所灰盒生成三个搜索点：`SearchPoint_waiting`、`SearchPoint_exam_a`、`SearchPoint_pharmacy`。
- 点击 `留下求助` 后调用 `GameSimulation.AddPlayerMark(State, "clinic", "help", ...)`，给祁眠夜间 AI 读取玩家痕迹提供入口。

玩家可见反馈：
- HUD 日志显示进入诊所文本。
- HUD 日志显示：`林行在社区诊所留下求助标记。`

涉及文件：
- Unity 场景：`E:\Download\working\BeyondSafeZoneUnity\Assets\Scenes\OneRunMain.unity`
- Unity 脚本：`Assets/Scripts/UI/OneRunGameController.cs`
- Unity 脚本：`Assets/Scripts/World/ExplorationSiteCatalog.cs`
- Unity 脚本：`Assets/Scripts/World/ScavengeSearchPoint.cs`
- Unity 脚本：`Assets/Scripts/World/ShelterInteractionCatalog.cs`
- Unity 脚本：`Assets/Scripts/World/ShelterInteractable.cs`
- Unity 脚本：`Assets/Scripts/Player/TopDownPlayerController.cs`
- Unity 测试：`Assets/Tests/TestGameSimulation.cs`

验证方法：
- UnitySkills `/health`。
- `script_get_compile_feedback` 检查 `OneRunGameController.cs` 和 `TestGameSimulation.cs`。
- Unity EditMode 测试 `BeyondSafeZone.Tests.TestGameSimulation`。
- Play 模式调用 `ExploreClinic` 和 `LeaveHelpMark` 的 `Button.onClick`。
- 运行时读取 Hierarchy、HUD 日志文本和 Console 统计。

验证结果：
- UnitySkills health：Unity `2022.3.62f3c1`，UnitySkills `2.0.1`，当前模式 `bypass`。
- 编译反馈：`OneRunGameController.cs` 无编译错误；`TestGameSimulation.cs` 无编译错误。
- 新增测试红绿记录：`TestOneRunControllerExposesHelpMarkAction` 先失败，补实现后通过。
- 完整 Unity EditMode 回归：`TestGameSimulation` 为 `35/35 passed`，jobId `2c7a6f63`。
- Play 验证：`ScavengeGreybox_clinic` 已生成，3 个诊所搜索点已生成，`WalkableShelterGreybox` 被隐藏。
- Play 验证：`OneRunHUD/LeaveHelpMark` 存在，点击后 HUD 日志出现 `林行在社区诊所留下求助标记。`
- Console 统计：`warnings: 0`、`errors: 0`。

阻塞/偏差：
- 搜索点 `E` 键近身交互尚未做完整手动验证；目前验证覆盖了 HUD 按钮进入诊所、生成搜刮灰盒、留下求助标记。
- 祁眠读取标记已在 C-007 中完成并验证；次日匿名药品 / 档案反馈链路尚未完成。

下一步：
- 做 `C-008` P0 AI 反馈链路：次日让玩家在诊所或日志中看到匿名药品 / 回应痕迹。
- 再验证一条完整链：诊所异常 → 留下求助标记 → 夜晚结算 → 次日反馈 → 结尾日志解释。

---

## 一、已创建的 Unity 文件

### 项目结构

```
E:\Download\working\BeyondSafeZoneUnity\
├── Assets/
│   ├── Scripts/
│   │   ├── BeyondSafeZone.Scripts.asmdef
│   │   ├── Core/
│   │   │   ├── GameSimulation.cs          # 流程协调器（NewGame/StartDay/SleepAndResolveNight/PlaySafeDemoDay）
│   │   │   └── TextRenderer.cs            # 文本渲染器（状态/独白/地点卡片/房间卡片/档案/标记链）
│   │   ├── Model/
│   │   │   └── GameState.cs               # 所有 Model 类（GameState/LinState/ResourceState/... 共 20+ 类）
│   │   ├── Data/
│   │   │   ├── BalanceData.cs             # 数值常量（80+ const）
│   │   │   ├── Constants.cs               # 全局常量（MAX_DEMO_DAY/BLOOD_MOON_DAYS/RED_TIDE_DAYS）
│   │   │   ├── Events15dData.cs           # 15 天逐日事件表
│   │   │   ├── FacilityData.cs            # 5 设施默认定义
│   │   │   ├── LocationData.cs            # 14 地点 + 40+ 房间定义
│   │   │   ├── QimianPlanData.cs          # 祁眠 7 天固定日程
│   │   │   └── SafeRouteData.cs           # 15 天安全演示路线
│   │   ├── Controllers/
│   │   │   ├── CarController.cs           # 汽车修理（4 步流程）
│   │   │   ├── ExplorationController.cs   # 探索系统（进地点/搜房间/引开/离开/异常档案写入）
│   │   │   ├── NightController.cs         # 夜晚结算（消耗/噪音/感染/血月/红潮/结局）
│   │   │   ├── QimianController.cs        # 祁眠 AI 引擎（固定日程 + 动态 AI + 玩家标记感知）
│   │   │   └── ShelterController.cs       # 据点设施（14 种行动）
│   │   ├── Services/
│   │   │   └── PlayKitNarrativeService.cs # PlayKit.ai 本地兜底占位（待 SDK 接入）
│   │   └── UI/
│   │       └── MainUI.cs                  # Unity MonoBehaviour UI 入口（UGUI 刷新）
│   └── Tests/
│       ├── BeyondSafeZone.Tests.asmdef
│       └── TestGameSimulation.cs          # 28 个 NUnit 测试方法
├── Packages/
│   └── manifest.json                      # Unity 包清单（含 PlayKit.ai Git URL）
└── ProjectSettings/
    ├── ProjectVersion.txt                 # Unity 2022.3.40f1
    └── ProjectSettings.asset              # 最小项目设置
```

### 文件统计

| 类别 | 文件数 | 总行数（约） |
|------|--------|-------------|
| Core | 2 | ~460 |
| Model | 1 | ~292 |
| Data | 7 | ~480 |
| Controllers | 5 | ~1150 |
| Services | 1 | ~110 |
| UI | 1 | ~340 |
| Tests | 1 | ~505 |
| 项目元数据 | 4 | ~50 |
| **合计** | **22** | **~3400** |

---

## 二、已完成的阶段

| 阶段 | 内容 | 状态 |
|------|------|------|
| 阶段 0 | 迁移前盘点 + 映射表 | ✅ 完成 |
| 阶段 1 | Unity 项目结构创建 | ✅ 完成（源文件结构，待 Unity Editor 确认） |
| 阶段 2 | Model + Data 层迁移 | ✅ 完成 |
| 阶段 3 | Controllers + Core 迁移 | ✅ 完成 |
| 阶段 4 | UI 灰盒（MainUI） | ✅ 完成（代码层面，Unity 场景未创建） |
| 阶段 5 | 一周目 AI 可读互动 | ✅ 完成（anomaly_dossier / player_marks / 祁眠感知） |
| 阶段 6 | PlayKit.ai SDK 接入 | ⏳ 占位完成，待 SDK 拉包 |
| 阶段 7 | 验证 | ⬜ 待 Unity Editor 编译 + 测试运行 |
| 阶段 8 | 同步文档 | ✅ 本文档即为同步产物 |

---

## 三、阶段 5 实现详情

### 3.1 anomaly_dossier 写入

`ExplorationController.ApplyRoomFlags()` 在搜到以下旗标时自动写入档案：

| 旗标 | 档案内容 | 结论 |
|------|----------|------|
| `plan_found` | 童年避难计划图纸 | 推测祁眠也持有副本 |
| `rebirth_clue_1` | 诊所隔离记录：零号病人「眠」 | 零号感染者 = 祁眠 |
| `rebirth_clue_2` | 派出所联络名单：「烬」 | 祁烬在返生计划内部 |
| `childhood_memory` | 学校笔记：三人避难计划 | 祁眠可能看到过林行的计划副本 |
| `lab_location` | 哨塔地图：第三实验室地址 | 祁眠在追踪同一实验室 |
| `qimian_file` | 隔离站标本室：实验日志 | 确认祁眠是返生计划零号实验体 |
| `apartment_letter` | 公寓信件：警告远离实验室 | 可能是祁烬在警告幸存者 |
| `qijin_apartment` | 祁烬留下的信封 | 祁烬曾在公寓活动 |

### 3.2 player_marks 系统

- **类型**：`danger`（危险）、`help`（求助）、`route`（路线）、`reserve`（储备）
- **入口**：`GameSimulation.AddPlayerMark(state, locationId, type, note)`
- **展示**：地点标签/卡片中显示标记图标和内容
- **独白**：林行在 DailyMonologue 中会根据标记类型产生对应独白

### 3.3 祁眠感知链

```
诊所异常线索 → 林行留下 help 标记 → 祁眠 Perceive() 读取 
→ CollectTasks 中 supply_drop 优先响应 help 标记地点
→ Execute 中记录 player_mark_response 
→ 结尾日志展示完整链路
```

### 3.4 结尾日志展示

`MainUI.RefreshEventLog()` 在 Reveal.Unlocked 时展示：
1. **玩家标记 → 祁眠感知链**（新增）
2. 祁眠行动日志 · 一周目回放
3. 逐日行动回放

---

## 四、静态审查结果

### 审查通过项

| 检查项 | 结果 |
|--------|------|
| Namespace 一致性 | ✅ 通过（修复 TextRenderer 位置后一致） |
| Class/Method/Field 交叉引用 | ✅ 通过（所有引用对应存在的方法/字段） |
| asmdef 引用链 | ✅ 通过（Tests → Scripts 引用正确） |
| 测试文件引用完整性 | ✅ 通过（28 个测试方法均可追溯到实现） |

### 已修复问题

| # | 问题 | 修复 |
|---|------|------|
| 1 | `TextRenderer.cs` 在 `UI/` 目录但 namespace 为 `Core` | 移动到 `Core/` 目录，删除 `UI/` 副本 |

---

## 五、待 Unity 安装完成后的验证清单

### 执行节奏要求

后续 Unity 迁移和灰盒实现统一按下面节奏推进：

1. `Plan`：只选一个明确任务编号，例如 `C-001` 或 `C-006`。
2. `Build`：只修改与该任务直接相关的文件或场景对象。
3. `Test`：至少完成一次与该任务直接相关的验证。
4. `Refine`：把失败点、阻塞点或偏差写回本文件。

如果一次改动跨越多个任务编号，必须在文档里拆成多个记录，不允许只写成“顺手一起做完了”。

### 完成口径

一个 Unity 任务只有在以下 3 条同时满足时，才能写成完成：

- 对应任务的闭环说明已经明确。
- 对应代码或场景改动已经落地。
- 对应验证结果已经记录在本文件或测试记录中。

如果只完成了其中 1-2 条，状态只能写为“进行中”或“待验证”。

### 必须验证

- [ ] **打开 Unity 项目**：用 Unity 2022.3 LTS 打开 `E:\Download\working\BeyondSafeZoneUnity`
- [ ] **编译通过**：确认所有 C# 文件无编译错误
- [ ] **PlayKit.ai SDK 拉包**：Unity Package Manager 通过 Git URL 拉取 SDK，确认 API 签名
- [ ] **测试运行**：Unity Test Runner 中运行全部 28 个测试，确认全部通过
- [ ] **创建 Unity 场景**：
  - [ ] 创建 `Assets/Scenes/Main.unity`
  - [ ] 添加 Canvas + MainUI 组件
  - [ ] 绑定所有 `[Header("UI References")]` 公共字段
  - [ ] 绑定按钮事件（OnSleep/OnRestart 等）
- [ ] **15 天完整跑通**：Play 模式下执行安全演示路线，确认：
  - [ ] 15 天逐日事件正常触发
  - [ ] 探索搜索正常（室内搜索/资源收集/旗标发现）
  - [ ] 据点设施正常（休息/修车/封窗/广播等）
  - [ ] 夜晚结算正常（血月/红潮/感染/结局）
  - [ ] 祁眠日志正常（Day 5 唤醒/固定日程/动态 AI）
  - [ ] 结局正确（reached_gate_quarantine / barely_reached_gate）
- [ ] **PlayKit SDK 集成**（需先拉包确认 API）：
  - [ ] 读取 SDK 示例代码
  - [ ] 将 PlayKitNarrativeService 中的 Fallback 方法替换为真实 SDK 调用
  - [ ] 确认 Developer Token 不在代码中（通过环境变量或 Editor 配置注入）

### 可选验证

- [ ] UI 交互测试（按钮点击/面板切换/刷新）
- [ ] 边界条件测试（零资源/极端高感染/崩溃结局）
- [ ] 中文字体渲染测试

---

## 六、PlayKit SDK 状态

| 项目 | 状态 |
|------|------|
| Git URL 已在 manifest.json | ✅ `https://gitlab.com/playkit-ai/playkit-unitysdk.git?path=Packages/ai.playkit.sdk` |
| SDK 已拉包 | ⬜ 待 Unity Editor 打开后 Package Manager 自动拉取 |
| C# API 已确认 | ⬜ 待读取 SDK 文档/示例 |
| 本地兜底接口 | ✅ `PlayKitNarrativeService.cs` 已创建 |
| Token 安全 | ✅ 无 Token 写入 |

---

## 七、任务执行记录模板

后续每完成或卡住一个 Unity 任务，建议按下面模板往本文件追加简短记录：

```text
日期：
任务编号：
功能名：
触发条件：
玩家操作：
系统状态变化：
玩家可见反馈：
涉及文件：
验证方法：
验证结果：
阻塞/偏差：
下一步：
```

建议重点记录：

- 是否真的只推进了一个任务编号。
- 当前验证覆盖了哪条交互闭环。
- 失败是出在编译、运行时逻辑、UI 可读性，还是范围失控。

---

## 八、注意事项

1. **未经 Unity Editor 编译验证**：所有 C# 文件仅通过静态审查，可能在 Unity 编译时出现语法/引用错误。
2. **Unity 场景未创建**：MainUI.cs 依赖 Unity Editor 中绑定 UI 引用（Text/Button/Transform），需手动创建 Canvas 层级。
3. **Godot 原项目未修改**：所有修改仅限于 `E:\Download\working\BeyondSafeZoneUnity\`，Godot 项目保持原样。
4. **PlayKit API 请勿猜测**：等 SDK 可用后务必读取官方文档/示例代码，不要根据经验猜测方法签名。
