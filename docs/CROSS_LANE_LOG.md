# Cross-Lane Change Log

> **用途**: 每条开发线在会话结束时，将本次认可的变动写入对应栏目。其他线开工时先读此文件，确认是否有需要关注的联动变更。
>
> **规则**: 只写 "做了什么 / 改了哪里 / 对其他线有没有影响"。不写详细原因（原因留在 `docs/DECISIONS.md` 和 `docs/PROJECT_MEMORY.md`）。

---

## 读取指引（每次开工必读）

| 如果你是.. | 必看栏目 |
|-----------|---------|
| 代码线 | Code、Design（看机制变更）、Art（看素材路径变更） |
| 设定/策划线 | Design、Code（看实现反馈） |
| 美术线 | Art、Design（看新素材需求）、Code（看新素材引用） |
| 比赛材料线 | Design、Code、Art（全部，需要对齐 Demo 实际情况） |

---

## Code Lane

<!-- 代码线每次会话结束后在此追加 -->

### [2026-06-04] OneRunMain HUD 布局热修
- **改了什么**:
  1. 修正 `OneRunMain` 运行时 HUD 生成布局：`Header` 顶部居中，`Status` 左上，`Log` 右上，`Prompt` 底部居中。
  2. 底部按钮从单行横排改成两行居中，上排 y=`84`、下排 y=`40`，避免当前 Game 视图宽度下向右溢出并减少压住据点底边。
  3. 世界空间设施/搜索点标签字号从 `2.8f` 调整到 `0.75f`，缓解设施名压住据点灰盒的问题，同时保持可读。
  4. `CanvasScaler` 增加参考分辨率 `1280x720` 与宽高折中缩放。
- **新增/修改的文件或资产**: Unity 脚本 `Assets/Scripts/UI/OneRunGameController.cs`；文档 `docs/UNITY_MIGRATION_STATUS.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ `OneRunGameController.cs` 编译反馈：`errorCount: 0`。
  - ✅ Play 模式读取运行时 RectTransform，确认 `Status` 左上、`Log` 右上、`Prompt` 底部、按钮两行居中。
  - ✅ EditMode 完整回归 `BeyondSafeZone.Tests.TestGameSimulation`：`41/41 passed`，jobId `cb51ad29`。
  - ✅ 最终 Unity Console：`warnings: 0`、`errors: 0`。
  - ⚠️ 过程中曾在 Play 模式误启动 Test Runner，jobId `68504ce3` 因 `This cannot be used during play mode` 超时失败；已退出 Play、清空 Console、在 EditMode 重跑通过。
- **对其他线的影响**:
  - **设定线**：无机制变化。
  - **美术线**：当前仍是灰盒 UI；后续正式 HUD 可沿用“状态左上、日志右上、操作底部”的基础分区。
  - **比赛材料线**：可以重新截取 `OneRunMain` 灰盒画面，当前 UI 可读性比截图中的重叠状态更适合展示，但仍应标注为 greybox。

### [2026-06-04] C-010 最小测试：诊所 AI 纵切集成回归
- **改了什么**:
  1. 新增 Unity EditMode 集成测试 `TestMinimumVerticalSliceCoversClinicAiChain`。
  2. 测试串联覆盖 Day 1 白天探索 / 搜索 / 返回据点 / 夜晚推进，Day 5 诊所异常线索、`clinic/help` 标记、祁眠读取和匿名药品回应，Day 15 终局日志解释。
  3. 本任务只做测试闭环整理，未修改生产玩法规则。
  4. 更新 `docs/UNITY_MIGRATION_STATUS.md`，将 C-010 按单任务闭环记录。
- **新增/修改的文件或资产**: Unity 测试 `Assets/Tests/TestGameSimulation.cs`；文档 `docs/UNITY_MIGRATION_STATUS.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ `TestGameSimulation.cs` 编译反馈：`errorCount: 0`。
  - ✅ Unity Console 初始检查：`warnings: 0`、`errors: 0`。
  - ✅ 新增测试 `TestMinimumVerticalSliceCoversClinicAiChain`：`1/1 passed`，jobId `1db8f604`。
  - ✅ Unity EditMode 完整回归 `BeyondSafeZone.Tests.TestGameSimulation`：`41/41 passed`，jobId `6103dd69`。
- **对其他线的影响**:
  - **设定线**：C-005 到 C-009 的 P0 诊所 AI 最小链路已有一条集成回归保护；后续机制变更若打断该链路，测试会暴露。
  - **美术线**：无新增素材路径；仍可按诊所异常、求助标记、匿名药包、浅箭头、祁眠日志面板补 P0 表现。
  - **比赛材料线**：可以如实描述“Unity greybox 已有自动化测试覆盖最小纵切链路”；注意当前是 EditMode 规则/UI 文本验证，不是最终美术录屏证据。

### [2026-06-04] C-009 结尾日志：祁眠结构化解释诊所标记因果链
- **改了什么**:
  1. 新增 Unity 结尾日志文本生成：展示祁眠人格卡、感知输入、候选行动、排序理由、最终选择和共享地图影响。
  2. Day 15 终局 `Reveal.Summary` 会追加 C-009 结构化祁眠日志，明确解释 `社区诊所求助标记 → 祁眠感知 → 匿名药品 / 浅箭头 → 诊所药品+1 / 异常档案`。
  3. 新增 Unity EditMode 测试 `TestEndingRevealExplainsClinicHelpMarkCausality`，覆盖通关后玩家能读懂“我留下的标记影响过祁眠”。
  4. 更新 `docs/UNITY_MIGRATION_STATUS.md`，按单任务闭环记录 C-009 短规格、实现文件和验证结果。
- **新增/修改的文件或资产**: Unity 脚本 `Assets/Scripts/Core/TextRenderer.cs`、`Assets/Scripts/Core/GameSimulation.cs`、`Assets/Scripts/Controllers/NightController.cs`；Unity 测试 `Assets/Tests/TestGameSimulation.cs`；文档 `docs/UNITY_MIGRATION_STATUS.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ `TestEndingRevealExplainsClinicHelpMarkCausality` 红绿记录：先 `0/1 passed`，jobId `9b3f6d78`；后 `1/1 passed`，jobId `38843bb8`。
  - ✅ Unity EditMode 完整回归 `BeyondSafeZone.Tests.TestGameSimulation`：`40/40 passed`，jobId `061afa85`。
  - ✅ 相关脚本编译反馈：`TextRenderer.cs`、`GameSimulation.cs`、`NightController.cs`、`TestGameSimulation.cs` 均 `errorCount: 0`。
  - ✅ Play 运行态 Console：`warnings: 0`、`errors: 0`。
- **对其他线的影响**:
  - **设定线**：P0 诊所 AI 因果链已在终局文字日志闭环；后续若想增强体验，应优先写 `U-008` 日志面板规格，而不是扩大到回放动画。
  - **美术线**：本次未新增素材路径；后续可围绕“匿名药包”“浅箭头”“祁眠日志面板”补表现素材。
  - **比赛材料线**：可以如实描述“Unity greybox 已能在通关后用祁眠结构化日志解释诊所求助标记影响链”；需继续标注当前表现形式为文本日志，不是最终动画回放。

### [2026-06-04] C-008 诊所反馈：匿名药品 / 浅箭头回应
- **改了什么**:
  1. `QimianController` 新增诊所 `help` 标记的最小回应：夜晚读取后在 `clinic` 共享地图状态留下匿名药品。
  2. `clinic.Resources["meds"]` 增加 1，`clinic.QimianTrace` 变为 `true`，地点图标加入 `qimian`。
  3. `state.AnomalyDossier` 新增“匿名药品 / 浅箭头 / 理解标记”的诊所反馈记录。
  4. HUD 夜晚结算文本新增匿名药品反馈：求助标记旁多出浅箭头，表示有人读懂并回应。
  5. 更新 `docs/UNITY_MIGRATION_STATUS.md`，将 C-008 按单任务闭环记录；下一步转向 C-009 结尾日志解释链。
- **新增/修改的文件或资产**: Unity 脚本 `Assets/Scripts/Controllers/QimianController.cs`；Unity 测试 `Assets/Tests/TestGameSimulation.cs`；文档 `docs/UNITY_MIGRATION_STATUS.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ `TestClinicHelpMarkCreatesAnonymousMedicineFeedback` 红绿记录：先 `0/1 passed`，jobId `c3d25c58`；后 `1/1 passed`，jobId `39b33337`。
  - ✅ Unity EditMode 完整回归 `BeyondSafeZone.Tests.TestGameSimulation`：`39/39 passed`，jobId `880a4edd`。
  - ✅ `OneRunMain` Play 验证：Day 5 进入诊所并留下求助后，夜晚结算 HUD Log 显示 `社区诊所出现匿名药品：求助标记旁边多了一条浅箭头，像是有人读懂后留下的回应。`
  - ✅ Unity Console：`warnings: 0`、`errors: 0`。
- **对其他线的影响**:
  - **设定线**：P0 诊所链路已经从“读到标记”推进到“读到并回应”；下一步 C-009 应把这条因果链放进结尾日志解释。
  - **美术线**：后续可补 `匿名药包`、`浅箭头/回应痕迹` 图标或地面标记，占位文本已能支撑灰盒验证。
  - **比赛材料线**：可以如实描述“诊所求助标记会触发祁眠匿名药品/浅箭头反馈，并写入异常档案”；仍需标注当前是 Unity greybox / 文本反馈，不是最终美术表现。

### [2026-06-04] C-007 祁眠读取诊所 help 标记
- **改了什么**:
  1. `QimianController.ResolveForDay()` 在 Day 5 后读取 `state.PlayerMarks` 中的 `help` 标记，并将诊所求助标记写入祁眠日志 `AiReplay`。
  2. `QimianController` 同步写入 `state.Qimian.PublicClues`，让“求助标记被读到”成为玩家可见的夜晚线索。
  3. `GameSimulation.SleepAndResolveNight()` 会把本次夜晚新增、且尚未被夜晚结算文本显示过的公开线索追加到返回文本。
  4. 修正 Day 5 固定祁眠苏醒公开线索在 `昨夜` / `昨夜线索` 中重复显示的问题。
  5. 更新 `docs/UNITY_MIGRATION_STATUS.md`，将 C-007 按单任务闭环记录为“实现已落地 + 验证已记录”；未把 C-008 写成完成。
- **新增/修改的文件或资产**: Unity 脚本 `Assets/Scripts/Controllers/QimianController.cs`、`Assets/Scripts/Core/GameSimulation.cs`；Unity 测试 `Assets/Tests/TestGameSimulation.cs`；文档 `docs/UNITY_MIGRATION_STATUS.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ `TestQimianReadsClinicHelpMarkOnWakeNight` 红绿记录：先 `0/1 passed`，jobId `0f1fd5fb`；后 `1/1 passed`，jobId `2f745a1c`。
  - ✅ `TestNightResultShowsQimianReadClinicHelpMark` 红绿记录：先 `0/1 passed`，jobId `a7bbb122`；后 `1/1 passed`，jobId `793dfd6c`。
  - ✅ `TestNightResultDoesNotDuplicateExistingQimianPublicClue` 红绿记录：先 `0/1 passed`，jobId `5a2a2d8e`；后 `1/1 passed`，jobId `900cc0f8`。
  - ✅ Unity EditMode 完整回归 `BeyondSafeZone.Tests.TestGameSimulation`：`38/38 passed`，jobId `f6bef90c`。
  - ✅ `OneRunMain` Play 验证：Day 5 进入诊所并留下求助后，夜晚结算 HUD Log 显示 `昨夜线索：社区诊所附近的求助标记被人轻轻描深了一笔。`
  - ✅ Unity Console：`warnings: 0`、`errors: 0`。
- **对其他线的影响**:
  - **设定线**：C-007 已证明一周目玩家能看到隐藏 AI 感知玩家标记，不再只是结尾反转；下一步应聚焦 C-008 匿名药品 / 次日反馈的文本与档案表现。
  - **美术线**：后续需要“求助标记被描深”或“标记被回应”的占位/正式图标，但本次未新增素材路径。
  - **比赛材料线**：可以如实描述“祁眠 AI 已能在 Day 5 后读取玩家留在诊所的求助标记，并在次日 HUD 日志出现可见线索”；匿名药品和档案验证仍不能写成已完成。

### [2026-06-04] Unity OneRunMain 正式一周目灰盒 + 求助标记入口
- **改了什么**:
  1. 明确 Unity 正式主场景切到 `Assets/Scenes/OneRunMain.unity`；`Assets/Scenes/MainPrototype.unity` 只作为临时灰盒参考。
  2. `OneRunMain` 运行时生成可走动据点、`LinXing_Player`、六个据点设施、`OneRunHUD`，并可通过 HUD 进入诊所/超市/车库搜刮灰盒。
  3. 诊所 Play 验证链路跑通：点击 `去诊所` 后生成 `ScavengeGreybox_clinic`，包含 `SearchPoint_waiting`、`SearchPoint_exam_a`、`SearchPoint_pharmacy`，据点根对象隐藏。
  4. 在 `OneRunGameController` 增加 `LeaveHelpMarkAtActiveLocation()` 和 `OneRunHUD/LeaveHelpMark` 按钮；玩家在搜刮地点可留下 `help` 求助标记，写入 `GameSimulation.AddPlayerMark`。
  5. 更新 `docs/UNITY_MIGRATION_STATUS.md`、`HANDOFF.md`、`docs/PROJECT_MEMORY.md`、`docs/DECISIONS.md`，同步正式主场景和验证证据。
- **新增/修改的文件或资产**: Unity 脚本 `Assets/Scripts/UI/OneRunGameController.cs`；Unity 测试 `Assets/Tests/TestGameSimulation.cs`；文档 `docs/UNITY_MIGRATION_STATUS.md`、`HANDOFF.md`、`docs/PROJECT_MEMORY.md`、`docs/DECISIONS.md`、`docs/CROSS_LANE_LOG.md`。
- **测试状态**:
  - ✅ 新增测试 `TestOneRunControllerExposesHelpMarkAction` 按红绿流程验证：缺方法时失败，补实现后通过。
  - ✅ Unity EditMode 完整回归 `BeyondSafeZone.Tests.TestGameSimulation`：`35/35 passed`，jobId `2c7a6f63`。
  - ✅ Play 验证：`ExploreClinic` 和 `LeaveHelpMark` 按钮事件均可调用；HUD 日志出现 `林行在社区诊所留下求助标记。`
  - ✅ Unity Console：`warnings: 0`、`errors: 0`。
- **对其他线的影响**:
  - **设定线**：一周目“隔空标记”已经有正式场景入口，下一步应聚焦祁眠读取标记后的次日反馈文本和档案表现。
  - **美术线**：后续优先需要据点设施、诊所搜索点、求助标记/匿名药包的占位或正式图标。
  - **比赛材料线**：可以如实描述“Unity 正式灰盒已验证玩家可在诊所留下求助标记”，但祁眠次日反馈链路仍应标为进行中，不能写成完整已实现。

### [2026-06-04] Unity ChineseTMP 字体 atlas 可读性修复
- **改了什么**:
  1. 定位 Unity Console 缺字 warning 的根因：`Assets/Fonts/ChineseTMP.asset` 内嵌 `ChineseTMP Atlas` 的 `m_IsReadable` 为 `0`，导致 TMP 动态补字失败。
  2. 新增 `Assets/Editor/ChineseTmpAtlasReadableFixer.cs`，通过 Unity 编辑器序列化 API 将 `ChineseTMP Atlas` 设置为 readable，并保存字体资产。
  3. 确认 `Assets/Fonts/ChineseTMP.asset` 中 `ChineseTMP Atlas` 已变为 `m_IsReadable: 1`。
  4. 清空 Console、保存 `Assets/Scenes/MainPrototype.unity`，进入 Play 做快速验证。
- **新增/修改的文件或资产**: Unity 工具脚本 `Assets/Editor/ChineseTmpAtlasReadableFixer.cs`；Unity 字体资产 `Assets/Fonts/ChineseTMP.asset`；Unity 场景 `Assets/Scenes/MainPrototype.unity`；文档 `HANDOFF.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ `script_get_compile_feedback` 显示 `Assets/Editor/ChineseTmpAtlasReadableFixer.cs` 无编译错误。
  - ✅ Play 后 `console_get_stats` 显示 `warnings: 0`、`errors: 0`，没有新的 TMP 缺字 warning。
  - ⚠️ 退出 Play 后出现 1 条 `NativeFormatImporter generated inconsistent result for asset ... Assets/Fonts/ChineseTMP.asset` warning；当前无错误，不阻塞继续做灰盒玩法验证，但后续若反复出现需要再处理字体资产导入稳定性。
- **对其他线的影响**:
  - **设定线**：无机制变化。
  - **美术线**：中文 UI 字体当前可继续用于灰盒；后续正式美术字体仍需单独确认授权和导入方案。
  - **比赛材料线**：可继续截 Unity 灰盒图，但仍应标注为 Prototype / Greybox。

### [2026-06-04] Unity MainPrototype UI 灰盒整理
- **改了什么**:
  1. 通过 UnitySkills REST 验证 Unity 项目 `E:\Download\working\BeyondSafeZoneUnity` 正在运行，Unity `2022.3.62f3c1`，UnitySkills `2.0.1`，当前场景为 `Assets/Scenes/MainPrototype.unity`。
  2. 检查 `GameController` 上的 `MainPrototypeController` 序列化引用，确认 4 个文本和 9 个按钮引用均已连接。
  3. 重排 `MainPrototype` 的 Canvas 灰盒 UI：顶部标题、左侧状态与地点、中央行动按钮、右侧地点详情、底部日志；按钮尺寸从拥挤的小尺寸改为稳定可点的灰盒尺寸。
  4. 设置编辑态中文文案、按钮文案、TMP 字号、对齐、面板颜色和文本颜色，避免不点 Play 时仍显示 `New Text` / `Button`。
  5. 将 `Assets/Fonts/ChineseTMP.asset` 设置为 `Dynamic` 并开启 multi-atlas，降低后续中文缺字变方块风险。
- **新增/修改的文件或资产**: Unity 场景对象 `Assets/Scenes/MainPrototype.unity`、Unity 字体资产 `Assets/Fonts/ChineseTMP.asset`；文档 `HANDOFF.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`。
- **测试状态**:
  - ✅ UnitySkills `/health` 成功。
  - ✅ `console_get_stats` 显示 Unity Console `errors: 0`。
  - ⚠️ `scene_save` 在 UnitySkills `auto` 模式下返回 `MODE_FORBIDDEN`，需用户在 Unity 中手动 `Ctrl+S` 保存，或切到 Bypass 后再调用保存。
  - ⚠️ Console 仍有 TMP 缺字 warning 历史记录；字体资产已改为动态，需清 Console 并重新 Play 后再确认 warning 是否消失。
- **对其他线的影响**:
  - **设定线**：Unity 灰盒已对齐 4 地点最小 Demo 和一周目 AI 互动链路的展示结构。
  - **美术线**：当前仍是 UGUI 灰盒，不依赖正式像素素材；后续可按该 UI 信息结构替换正式视觉。
  - **比赛材料线**：可以开始截取 Unity 灰盒进度图，但对外仍应标注为 Prototype / Greybox，不能写成最终美术效果。

### [2026-05-31] 第8轮：MVC 架构重构 + 对齐 planning_package
- **改了什么**:
  1. **MVC 目录重组**：
     - `core/` → 保留 `game_simulation.gd`（协调器），其他拆入 `model/` `controller/` `view/`
     - `model/game_state.gd` — 纯数据类，新增 `anomaly_dossier`、`player_marks` 字段（对齐一周目 AI 互动系统）
     - `controller/` — `exploration_controller.gd` `shelter_controller.gd` `night_controller.gd` `car_controller.gd` `qimian_controller.gd`
     - `view/` — `main.gd` `node_map_view.gd` `explorer_view.gd` `shelter_panel.gd` `labels.gd` + 新增 `text_renderer.gd`
     - `data/` — 不变（constants/events/locations/facilities/qimian_plan）
  2. **解耦 game_simulation.gd**（388→196行）：所有文本格式化方法（`get_lin_condition_text`、`get_location_card_text`、`_daily_monologue` 等）抽出到 `view/text_renderer.gd`；game_simulation 只做流程调度 + Controller委托 + View委托
  3. **新增 Model 字段**：`anomaly_dossier: Array`（未知行动者档案）、`player_marks: Dictionary`（隔空标记），对齐 `planning_package/03_系统策划案_GDD.md` 一周目 AI 互动系统
  4. 所有 preload 路径从旧 `core/` `ui/` 更新为新目录
- **新增/修改的文件**: 新建 `model/game_state.gd` `view/text_renderer.gd`；移动并重命名 `controller/*` `view/*`；改写 `core/game_simulation.gd` `main.gd` `managers/game_manager.gd`；更新所有 controller 内 preload
- **测试状态**: ✅ All simulation tests passed
- **对其他线的影响**:
  - **设定线**：Model 已预埋 `anomaly_dossier` 和 `player_marks` 字段，后续实现异常调查/隔空标记时可直接使用
  - **比赛材料线**：无影响
  - **美术线**：无影响

### [2026-05-28] 第7轮：safe demo 室内搜索流程改造
- **改了什么**:
  1. `play_safe_demo_day` 从 `explore()`（跳过房间）改为完整室内搜索流程：`enter_location` → 自动搜索房间 → `leave_exploration`
  2. `enter_location` 补上路况疲劳惩罚、尸群压力、自行车耐久消耗（之前只在旧 `explore()` 中有）
  3. `search_room` 新增汽车零件路由：`battery/gasoline/tire` → `state.car_parts`
  4. bike_shop 车库解锁（`locked: false`），通过房间旗标 `car_found` 自然触发汽车发现
  5. bike_shop 店面新增 `tire: 1` 资源，确保房间级搜索可获取轮胎
  6. `play_safe_demo_day` 地点选择覆盖 13/14 个地点（原仅 5 个），每天搜索 ≤3 个房间
- **新增/修改的文件**: `game/scripts/core/game_simulation.gd`, `game/scripts/core/exploration.gd`, `game/scripts/data/locations.gd`
- **测试状态**: ✅ All simulation tests passed
- **对其他线的影响**: 无。纯代码层改进，不改变叙事或素材需求。
- **关键发现**: `explore()` 函数现在仅在室内搜索流程不可用时作为 fallback 保留，safe demo 路径已全面使用新流程。

### [2026-05-27] session summary
- **改了什么**:
  1. 统一天数为 15 天：`MAX_DEMO_DAY` 14→15，血月从 Day7+Day14 改为 Day7+Day15
  2. 新增 Day14 红潮夜事件 + Day15 终局血月事件，新增 Day14 祁眠红潮夜观察行动
  3. 祁眠 Day14 尸群藏身+双层揭示迁移到 Day15
  4. 改进节点地图可读性：图标中文化（🏠/🍞/💊等）、路线上限明确提示、房间能见度描述增强
  5. 改进结局揭示：祁眠日志回放含人格卡展示+逐日 AI 决策回放+祁眠主观残句
  6. 安全路线 Day14 改为便利店补充食物避免缺水
- **新增/修改的文件**: `game/scripts/core/game_simulation.gd`, `game/scripts/main.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ All simulation tests passed
- **对其他线的影响**:
  - **比赛材料线**：需将 Demo 描述从 14 天更新为 15 天，血月 Day14→Day15
  - **设定线**：天数已统一为 15 天，Day14 红潮夜事件已落地，祁眠 Day14 新增红潮夜观察行动
  - **美术线**：无新素材路径变更，但地图图标已中文化（emoji），后续素材可对齐

### [2026-05-27] 第4轮：14地点房间数据升级
- **改了什么**:
  1. 地点从 9 个扩展到 **14 个**：新增桥洞营地(NPC)、加油站、五金店、废弃公寓(5F/9房+幸存者)、防疫隔离站
  2. 房间数据从每地点 2 房升级为设计文档定义的全部 **40+ 房间**（含楼层、窗/暗、丧尸数量、精确资源、叙事旗标）
  3. 新增 `_room_data()` 函数（含 `flags` 和 `locked` 字段）+ `_apply_room_flags()` 旗标系统
  4. 搜索上锁房间提示需要撬棍；`get_room_card_text` 显示 🔒 状态
  5. 叙事旗标系统：`plan_found`/`safezone_hint_1`/`rebirth_clue_1+2`/`childhood_memory`/`crowbar_found`/`lab_location`/`qimian_file`/`apartment_letter`/`qijin_apartment`/`rebirth_insider` 等 15+ 旗标在搜索时自动触发
- **新增/修改的文件**: `game/scripts/core/game_simulation.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ All simulation tests passed

### [2026-05-27] 第5轮：噪音模型+情境独白+完整撤离叙事
- **改了什么**:
  1. **噪音传播模型**：`_propagate_noise()` — 夜晚据点和探索噪音吸引近圈尸群，高噪音（≥6）显著增加周边丧尸密度
  2. **情境独白系统**：`_daily_monologue()` — 根据生命/感染/饥饿/口渴/疲劳/压力/希望值/汽车状态/天数/祁眠线索，动态生成林行每日独白段落
  3. **Day 15 完整撤离叙事弧**：`_car_evacuation_narrative()` — 开车→引擎启动→穿越城市→远郊路况恶化→爆胎/过热→弃车→徒步→无名小镇→血月下最后一公里→抵达大门
  4. **结局分层**：三层结局各有完整的叙事闭环（collapsed/barely_reached_gate/reached_gate_quarantine），含祁眠日记最后一行
  5. Day 15 弃车事件写入夜晚结算（engine_overheat / not_ready / no_car）
- **新增/修改的文件**: `game/scripts/core/game_simulation.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ All simulation tests passed

### [2026-05-28] 阶段B+C+D完成：架构重构完毕（移动端执行）

- **改了什么**:
  1. 阶段B 系统拆分：探索→ExplorationSystem, 据点→ShelterSystem, 夜晚→NightResolver, AI→QimianAI, 汽车→CarSystem
  2. 阶段C 引入 GameManager 信号路由
  3. 阶段D GameState class_name 类型化
  4. game_simulation.gd 从 1464 行减至 373 行(-75%)，只做流程调度+UI文本生成
  5. 所有核心函数委托至独立系统文件
- **文件结构**: scripts/core/{game_state,exploration,shelter,night_resolver,qimian_ai,car_system}.gd + scripts/managers/game_manager.gd + scripts/ui/labels.gd
- **测试状态**: ✅ All simulation tests passed
- **中断清理确认**: exploration.gd(258行) 完整，全部 preload 有效，零残留旧函数
- **改了什么**:
  1. 创建 `scripts/data/` 目录，4 个纯数据文件：
     - `constants.gd` — 全局常量（MAX_DEMO_DAY/BLOOD_MOON_DAYS/RED_TIDE_DAYS）
     - `events_15d.gd` — 15 天逐日事件表（原 _day_events）
     - `qimian_plan.gd` — 祁眠固定日程表（原 _qimian_plan）
     - `locations.gd` — 14 地点 + 40 房间 + 图标描述 + 路况说明
     - `facilities.gd` — 5 核心设施定义
  2. `game_simulation.gd` 内联数据 → `preload` 引用，删除 ~200 行硬编码数据
  3. 移除 `_day_event()`, `_room_data()`, `_rooms_for_location()`, `_facility()` 等旧工厂函数
  4. 修复 const 字典深拷贝问题（.duplicate(true)）
- **新增的文件**: `scripts/data/constants.gd`, `events_15d.gd`, `qimian_plan.gd`, `locations.gd`, `facilities.gd`
- **修改的文件**: `scripts/core/game_simulation.gd`
- **测试状态**: ✅ All simulation tests passed
- **文件行数变化**: game_simulation.gd 1464→~1200 行（-18%）

### [2026-05-27] 第6轮：祁眠 AI 决策引擎
- **改了什么**:
  1. **AI 状态系统**：`state.qimian.ai_state` — 暴露值(0-10)、摩托等级(1-3)、区域热度(A/B/C 0-3)、祁烬线索进度(0-3)、AI 背包库存
  2. **决策引擎核心**：
     - `_qimian_perceive()` — 构建感知状态（天气/月相/可用区域/尸群热点/幸存者需求/信号追踪）
     - `_qimian_collect_tasks()` — 收集可用任务（巡逻/搜刮/匿名补给/追踪祁烬/休整）
     - `_qimian_rank_and_select()` — 按人格优先级排序选择
     - `_qimian_execute()` — 执行任务+更新世界状态+写日志
  3. **混合模式**：固定日程任务（Day 5/6/8/10/11/14/15）保持叙事保证；非固定日（Day 7/9/11/13）由 AI 动态决策
  4. **摩托升级**：Day 8 自动升至 Lv.2（解锁中圈），Day 12 升至 Lv.3（解锁远圈）
  5. **暴露管理**：高暴露(≥8)限制可用区域，≥10 触发坏结局；红潮/雨夜提供掩护
  6. **UI 显示**：通关回放显示 AI 运行状态（暴露值/摩托等级/祁烬线索/区域热度）
- **新增/修改的文件**: `game/scripts/core/game_simulation.gd`, `game/scripts/main.gd`
- **测试状态**: ✅ All simulation tests passed

### [2026-05-27] 第3轮：汽车撤离系统 + 设计线文档对齐
- **改了什么**:
  2. 撤离条件从 `bike_ready` 改为 **`car_ready`**（自行车仅限近中圈探索，汽车才够远圈）
  3. 汽车零件分布：轮胎（修理铺店面）、电瓶（派出所/哨卡）、汽油（地铁口/哨卡）
  4. 安全路线 Day 5 发现汽车 → Day 10/12/13/14 四晚分步修理 → Day 15 撤离
  5. UI 新增汽车状态行和修理进度标签
  6. 对齐设计线新产出：`15天逐日事件表`、`地点结构化数据`、`祁眠AI决策伪代码`、`共享地图状态API`
- **新增/修改的文件**: `game/scripts/core/game_simulation.gd`, `game/scripts/main.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ All simulation tests passed
- **对其他线的影响**:
  - **设定线**：汽车系统已落地（DECISIONS 锁定的4步修理流程），安全路线已验证可完成
  - **比赛材料线**：撤离从「修好自行车」更新为「修好汽车」，需对齐比赛文案
  - **美术线**：新需要的素材——旧轿车（可复用占位）、电瓶/汽油桶/轮胎图标
- **更新阻塞项**: `汽车系统` + `Days 11-14 红潮夜` 已标记为 ✅

---

## Master Planning (总体规划)

<!-- 总体规划对话在此追加 -->

### [2026-06-03] 文档目录整理：active / reference / archive
- **改了什么**:
  1. 保留根目录和 `docs/` 顶层为当前入口：`HANDOFF.md`、`README.md`、`docs/planning_package/`、`docs/UNITY_MIGRATION_PLAN.md`、`docs/UNITY_MIGRATION_STATUS.md`、`docs/开发任务拆解.md`、记忆/决策/素材规范等。
  2. 新增 `docs/reference/`，集中仍有实现价值的细节文档：`DEMO_SCOPE.md`、`15天逐日事件表.md`、`地点结构化数据.md`、`共享地图状态API.md`、`祁眠AI决策伪代码.md`、`祁眠事件关卡布局.md`。
  3. 新增 `docs/archive/`，归档旧入口、历史报告、技术报告、灰盒 HTML 原型、临时媒体和 `temp_img/`。
  4. 将根目录 `介绍.md` 移到 `marketing/介绍.md`。
  5. 更新 `HANDOFF.md`、`README.md`、策划包 README/GDD、`docs/DECISIONS.md`、`docs/开发任务拆解.md` 中的活引用。
- **新增/修改的文件**: `docs/reference/**`, `docs/archive/**`, `marketing/介绍.md`, `HANDOFF.md`, `README.md`, `docs/planning_package/README.md`, `docs/planning_package/03_系统策划案_GDD.md`, `docs/DECISIONS.md`, `docs/开发任务拆解.md`, `task_plan.md`, `progress.md`, `findings.md`
- **对其他线的影响**:
  - **代码线**：查细节数据改用 `docs/reference/`；当前主线仍按 Unity 迁移资料走。
  - **设定线**：新文档优先写入 `docs/planning_package/` 或 `docs/开发任务拆解.md`，不要再恢复旧根部入口。
  - **美术线**：临时图移入 `docs/archive/media/temp_img/`；正式素材仍在 `assets/` 和素材日志。
  - **比赛材料线**：对外介绍稿位置改为 `marketing/介绍.md`。

### [2026-06-02] 主开发方向切换为 Unity + PlayKit.ai Unity SDK
- **改了什么**:
  1. 用户明确决定全量转向 Unity，目标新项目路径为 `E:\Download\working\BeyondSafeZoneUnity`。
  2. Godot 4.6.2 当前灰盒保留为规则、数据、文本、测试和行为参考，不再作为主开发线继续扩展。
  3. PlayKit.ai 当前接入方向锁定为 Unity SDK；Godot SDK 不作为当前可用依据。
  4. 策划入口 `docs/planning_package/` 保持不变，但引擎/实现口径同步为“Unity 主开发目标；Godot 灰盒为迁移参考”。
- **新增/修改的文件**: `docs/PROJECT_MEMORY.md`, `docs/DECISIONS.md`, `docs/CROSS_LANE_LOG.md`, `HANDOFF.md`, `docs/planning_package/README.md`, `docs/planning_package/03_系统策划案_GDD.md`, `docs/planning_package/04_详细策划案.md`
- **对其他线的影响**:
  - **代码线**：后续优先执行 Unity 迁移；迁移前先写 `docs/UNITY_MIGRATION_PLAN.md`，再创建 Unity 项目。
  - **设定线**：机制口径不变，策划文档中的实现目标改按 Unity 表达。
  - **美术线**：素材规格仍可沿用，但导入目标从 Godot 转为 Unity。
  - **比赛材料线**：后续材料需避免继续写“Godot 当前主开发引擎”，应说明 Unity 迁移和 PlayKit.ai Unity SDK 接入方向。

### [2026-05-30] 快速同步 + 游戏介绍（分享版）
- **改了什么**: 快速读取全量跨线同步日志，汇总四条线最新进展；输出了以AI玩法为核心的分享式游戏介绍
- **新增/修改的文件**: 无新增文件；更新了 CROSS_LANE_LOG 阻塞项状态
- **同步发现**: 代码线7轮+策划线1轮+美术线1轮+比赛线1轮均已完成；4条阻塞项全部✅，余3条（比赛15天对齐、一周目回放动画、二周目）为后续范围

### [2026-05-27] session summary
- **改了什么**: 完整梳理全部文档,生成总体规划分析报告,包含策划案总结、完整度/一致性问题、结构化需求概要、三条 Lane 启动提示词
- **新增/修改的文件**: `docs/总体规划分析报告.md`
- **对其他线的影响**: 需要代码/美术/设定线关注以下不一致——天数混用(14/15)需统一、林行撤离动机需增强事件驱动、祁烬 Demo 方案待决策

---

## Design Lane

<!-- 设定/策划线每次会话结束后在此追加 -->

### [2026-06-04] 执行纪律文档化
- **改了什么**:
  1. 将“结构化文档驱动实现、单任务小步闭环、交互闭环优先于功能堆砌”正式写入 `docs/MINIMUM_DEMO_SCOPE.md`、`docs/开发任务拆解.md`、`docs/UNITY_MIGRATION_STATUS.md`。
  2. 新增统一执行约束：一次只推进一个明确任务编号；没有“触发条件/玩家操作/状态变化/可见反馈/验证方法”的短规格，不进入实现。
  3. 新增统一完成口径：只有“文档已更新 + 实现已落地 + 验证已记录”同时满足，任务才算完成。
  4. 把 P0 交互闭环与 `T-001` 到 `T-010` 回归项绑定，减少“做了功能但没有证据证明玩家能感知”的情况。
- **新增/修改的文件**: `docs/MINIMUM_DEMO_SCOPE.md`, `docs/开发任务拆解.md`, `docs/UNITY_MIGRATION_STATUS.md`, `docs/PROJECT_MEMORY.md`, `docs/DECISIONS.md`, `docs/CROSS_LANE_LOG.md`
- **对其他线的影响**:
  - **代码线**：后续 Unity / Godot P0 工作应按单任务节奏记录验证，避免把多个功能合并成一个“已完成”描述。
  - **美术线**：无直接素材规格变化，但后续应优先服务已锁定的 P0 闭环，而不是先扩素材面。
  - **比赛材料线**：可以按“已验证的交互闭环”描述进度，避免用模糊的系统数量包装完成度。

### [2026-06-03] 最小 Demo 范围锁定
- **改了什么**:
  1. 新增并确立 `docs/MINIMUM_DEMO_SCOPE.md` 为近期制作范围依据。
  2. 将近期目标收束为 10-15 分钟最小可玩纵切：4 个核心地点、诊所 AI 因果链、异常调查、隔空标记、祁眠读取标记、次日反馈、结尾日志解释。
  3. 更新 `HANDOFF.md`、`docs/planning_package/01_策划总纲.md`、`02_策划概要案.md`、`04_详细策划案.md`、`docs/开发任务拆解.md`，避免把完整 14 地点、完整二周目、复杂骰子/NPC 系统写成近期必做。
  4. 修正 `docs/开发任务拆解.md` 程序任务编号重复问题。
- **对其他线的影响**:
  - **代码线**：优先验证 Unity 编译，再实现 4 地点 + 诊所 AI 链路；不要继续按 14 地点或完整二周目扩张。
  - **美术线**：P0 素材聚焦据点、诊所、超市、修理铺/车库、异常档案、四类标记、匿名药包。
  - **比赛材料线**：对外材料应说明“Demo 聚焦最小纵切”，完整二周目、复杂骰子和长期 NPC 合作只能写作后续规划。

### [2026-06-02] 开发任务拆解 + 招队友分工清单
- **改了什么**:
  1. 新增 `docs/开发任务拆解.md`，把当前策划包拆成可分配任务。
  2. 按程序、美术、UI/UX、策划/关卡、音频、测试、比赛/招队友材料拆分 P0/P1/P2。
  3. 明确当前已实现基础、已预埋字段、待实现闭环和待定稿机制。
  4. 将“异常调查 + 隔空标记 + 诊所最小链路”列为 P0 AI 玩法核心。
  5. 将行动点/骰子与 NPC 合作标为老师建议下的机制增强，需先定详细规则再进代码。
- **新增/修改的文件**: `docs/开发任务拆解.md`, `task_plan.md`, `progress.md`, `findings.md`
- **对其他线的影响**:
  - **代码线**：优先看 `docs/开发任务拆解.md` 的 `C-001` 到 `C-007`，先做一周目 AI 可读互动闭环。
  - **美术线**：优先看 `A-001` 到 `A-016`，尤其是异常档案图标、四类标记、匿名药包、诊所/超市/修理铺/桥梁。
  - **比赛材料线**：招队友和 PPT 可使用第 7-9 节，但需要把待定机制表述为“计划/招募任务”，不要写成已实现。

### [2026-05-31] 一周目 AI 可读互动系统
- **改了什么**:
  1. 将一周目 AI 玩法从“结尾揭示随机后果”强化为“异常调查 + 隔空标记”
  2. `docs/planning_package/03_系统策划案_GDD.md` 新增异常调查系统、隔空标记系统、未知行动者档案、标记 UI 和诊所反馈链路
  3. `docs/planning_package/04_详细策划案.md` 新增程序模块、状态字段、机制流程、UI、测试检查表
  4. `docs/ONE_PAGE_GDD.md`、`docs/DEMO_SCOPE.md` 同步加入一周目 AI 互动说明
  5. `docs/共享地图状态API.md` 新增 `anomaly_tags`、`player_mark`、`player_mark_day`、`player_reserved_resources` 和诊所求助标记案例
  6. `docs/祁眠AI决策伪代码.md` 新增 `player_marks`、`reserved_resources`、`anomaly_traces`、`world_trace_input` 和标记加权规则
- **对其他线的影响**:
  - **代码线**：后续实现一周目 AI 互动时，优先做诊所最小链路：药柜异常 → 求助标记 → 祁眠夜晚读取 → 匿名药品/档案验证
  - **比赛材料线**：介绍 AI 玩法时可强调玩家一周目能读懂并间接影响隐藏 AI，不只是结尾反转
  - **美术线**：后续需要异常档案图标和四类标记图标（危险/求助/路线/物资保留）

### [2026-05-30] 策划包集中整理
- **改了什么**:
  1. 新增 `docs/planning_package/` 作为当前统一策划入口
  2. 新增 `01_策划总纲.md`、`02_策划概要案.md`、`03_系统策划案_GDD.md`、`04_详细策划案.md`
  3. `README.md`、`marketing/` 主要材料同步到 15 天、14 地点、汽车撤离、Day 15 终局血月口径
  4. `docs/ONE_PAGE_GDD.md`、`docs/策划案.md`、`docs/DEMO_SCOPE.md` 顶部增加当前维护入口提示
  5. 修正 `docs/策划案.md` 中最容易误导的旧 14 天/自行车撤离句子
- **新增/修改的文件**: `docs/planning_package/**`, `README.md`, `marketing/*.md`, `docs/ONE_PAGE_GDD.md`, `docs/策划案.md`, `docs/DEMO_SCOPE.md`, `docs/DECISIONS.md`
- **对其他线的影响**:
  - **代码线**：后续开发入口优先读 `docs/planning_package/03_系统策划案_GDD.md` 和 `04_详细策划案.md`
  - **比赛材料线**：当前材料已同步 15 天/汽车撤离/14 地点口径，可继续从策划包提取答辩内容
  - **美术线**：无新增素材需求；美术需求集中在 `03_系统策划案_GDD.md` 和 `04_详细策划案.md`

### [2026-05-30] One Page GDD 创建
- **改了什么**: 新增 `docs/ONE_PAGE_GDD.md`，将现有宣传介绍和长策划内容压缩为开发/答辩用单页 GDD，明确核心循环、玩家系统、15 天结构、撤离条件、祁眠 AI 输入/决策流程、共享地图状态、通关揭示、第二周目 Demo 边界。
- **影响的机制/数值**: 未新增机制；整理并固定当前表达口径。第二周目明确为比赛 Demo 的关键行动章节，不承诺完整 15 天第二战役。
- **对其他线的影响**:
  - **代码线**：可将 `docs/ONE_PAGE_GDD.md` 作为开发入口，长文档作为细节来源。
  - **比赛材料线**：可用该文件回应“机制循环如何运作、有几个系统”的评审/老师问题。
  - **美术线**：无新增素材需求。

### [2026-05-27] session summary
- **改了什么**:
  1. 天数统一为 15 天：HANDOFF、DEMO_SCOPE、策划案、DEMO_PITCH、DECISIONS 全部更新
  2. 祁烬 Demo 呈现决策：广播感知 + 不直接出场，移除白昼协议
  3. 撤离条件重构：自行车限近中圈 → 汽车（修理铺渐进式：发现→攒件→组装→Day15故障弃车徒步）
  4. 撤离触发：Day 14 广播超大型尸潮 + 据点受损，Day 15 白天出发→凌晨抵达
  5. 一周目结尾：祁眠日记+片段回放（5 段分镜：醒来/骑行/清桥/留药/血月擦肩）
  6. 二周目：祁眠成为可操作角色，林行由 AI 接管
  7. 新增 `docs/15天逐日事件表.md`：15 天细线逐日表（含房间级资源、广播原文、双线叙事、数值倾向）
  8. 数值平衡表写入策划案：林行 8 项状态初始值、6 类资源产出曲线、设施消耗、丧尸数值、汽车修理子系统、疲劳影响表
- **影响的机制/数值**: 汽车获取流程（4 步修理）、撤离触发条件、一周目回放结构、疲劳-探索时间曲线
- **对其他线的影响**:
  - **代码线**：汽车系统（新机制，4 步修理需新 state/key）、Day 15 弃车徒步事件、一周目片段回放演出系统、二周目祁眠可操作角色切换。15 天逐日事件表可直接对照实现。
  - **比赛材料线**：撤离触发改为超大型尸潮广播+据点受损，不再使用白昼协议；Demo 明确展示一周目回放+二周目解锁。DEMO_PITCH 已更新为 15 天。
  - **美术线**：片段回放需要 5 个关键帧/场景（废弃诊所、摩托夜行、桥面侧视、家门口留药、血月擦肩）。祁烬不需要新素材（不直接出场）。汽车需简单像素素材。
  - **补充(代码开工级交付)**：新增 3 个结构化数据文件——`docs/地点结构化数据.md`（14 地点房间级数据）、`docs/祁眠AI决策伪代码.md`（完整 GDScript 翻译就绪伪代码）、`docs/祁眠事件关卡布局.md`（9 个侧视横版关卡精确布局）、`docs/共享地图状态API.md`（位置状态结构+结算管道+持久化格式）。代码线可直接用这 4 个文件开工。

---

## Art Lane

<!-- 美术线每次会话结束后在此追加 -->

### [2026-06-02] FHL Image Studio CLI 包只读审计
- **产出/修改的素材**: 无。本次未生成图片、未运行第三方 exe、未配置或读取 API Key。
- **新增的素材规格/命名**: 无。确认该包若后续启用，应优先用于概念图、关键帧、宣传图和提示词批量产出；进入 `assets/sprites/` 前仍需按 `ASSET_PIPELINE.md` 做像素化、授权记录和人工筛选。
- **审计结论**:
  1. 根目录存在 `FHL-Image-Studio方汤圆CLI魔改版1.0.7.zip`，SHA256 为 `67FCEEB3EC296B5033D5E0395FE22824EAC8CE7E4CA616C06652A636F538FDD9`。
  2. 包内包含 Go CLI、Image Studio 前端/后端、Cloudflare Worker、Android shell、便携 `runtime/cli/gptcodex-image.exe` 和 `runtime/node/node.exe`。
  3. `image-cli.cmd` 默认调用 `https://www.fhl.mom`、`gpt-5.5`、`gpt-image-2`，读取 `config/cli.env.local` 或样例配置，输出到包内 `output/` 和 `output/log/`。
  4. 源码侧看到 base URL 校验、HTTPS 限制、keyring/本地 env 配置逻辑；未做动态运行验证，预编译 exe 仍应视为第三方不可信二进制。
- **对其他线的影响**:
  - **美术线**：可作为候选 AI 概念图流水线，但正式参赛素材仍需记录工具、日期、prompt 摘要、人工修改情况和比赛可用状态。
  - **比赛材料线**：若使用该工具生成展示素材，需在 AI 使用说明和素材授权日志中披露第三方中转/API 工具链。
  - **代码线**：无影响，未接入 Godot 工程。

### [2026-05-27] 占位素材批量生成 + 美术方向锁定
- **产出/修改的素材**:
  - 林行 8 动作占位（idle/walk/search/hurt/bike/gun/jump/climb）32×32
  - 祁眠 8 动作占位（idle/walk/search/rescue/hidden/gun/jump/climb）32×32
  - 普通丧尸 3 动作占位（idle/walk/attack）32×32
  - 血月丧尸 3 动作占位（idle/run/attack）32×32
  - UI 图标 9 个占位（food/water/medicine/materials/bike_parts/fuel/danger/qimian_trace/blood_moon_warn）16×16
  - 场景 Tile 6 个占位（shelter/hospital/supermarket/pharmacy/bike_shop/blood_moon_exterior）16×16 + 32×32
  - 共 38 个占位精灵文件，全部为 Python 生成的纯色块（Prototype Only）
- **新增的素材规格/命名**:
  - 按 ASSET_PIPELINE 命名规范：`char_linxing_{action}_32x32.png` 等
  - 目录结构：`assets/sprites/{characters,enemies,ui,tilesets}/`
- **对其他线的影响**:
  - **代码线**：素材路径已就绪，可直接在 Godot 中引用占位精灵
  - **比赛材料线**：当前所有素材为 Prototype Only，录屏前需替换为正式素材
  - **设计线**：美术方向已锁定——林行（兵长 Levi 风格）、祁眠（基努里维斯风格）、丧尸（PvZ/MC 卡通化）、大地图（This War of Mine 俯视）、据点（TWoM 横截面侧视图）

### [YYYY-MM-DD] session summary
- **产出/修改的素材**:
- **新增的素材规格/命名**:
- **对其他线的影响**:

---

## Contest Lane

<!-- 比赛材料线每次会话结束后在此追加 -->

### [2026-05-30] 本地介绍文案保存
- **准备/更新的材料**: 新增/整理根目录 `介绍.md`，作为对外介绍稿，重点说明 15 天生存经营、祁眠 AI 隐藏行动、共享地图改写、通关日志揭示、第二周目祁眠视角与林行 AI 接管。
- **对齐的 Demo/设计口径**: 采用最新 15 天、Day 7/Day 15 双血月、旧车撤离、祁眠第 5 天醒来、AI 人格卡确定性决策、第二周目祁眠可操作的介绍口径。
- **对其他线的影响**: 比赛材料线后续可从 `介绍.md` 提取 PPT/视频旁白/报名简介；README 和部分旧营销材料仍有 14 天旧口径，后续需继续同步。

### [2026-05-27] 全材料对齐 Demo 真实状态
- **准备/更新的材料**: 全面修订 `marketing/` 全部 7 个文件 + `README.md`，对齐 Godot 灰盒 Demo（`game_simulation.gd` + `main.gd`）的当前实现状态
  - `PPT_OUTLINE.md`：修复关键错误（Slide 5 祁眠苏醒日 Day 11→Day 5）；重构为 10 页幻灯片，每页标注已实现/[planned]；新增已实现系统明细、三层结局说明、开发状态页
  - `DEMO_PITCH.md`：全面重写为"当前 Demo 实现"结构，列出所有已实现系统（日夜循环、节点地图、室内搜索、五大设施、六大资源、感染系统、撤离条件、血月、三层结局、AI 日志）；明确标注 [planned] future scope
  - `PITCH_COPY.md`：一句话/短/长三版文案对齐实际资源名（fuel 而非 batteries）、实际结局、实际 AI 机制（共享地图分时段结算、qimian 标记、确定性人格卡规则）
  - `SUBMISSION_PLAN.md`：Current Demo Truth 扩充为具体已实现功能清单（21 项）；Evidence To Collect 增加室内搜索、设施面板、感染系统截图
  - `AI_USAGE_STATEMENT.md`：大幅扩展——新增 AI 特性六步说明（人格卡→决策引擎→感知限制→共享地图→异常标记→日志揭示）；新增 CodeBuddy 实际使用表格（规划/代码/测试/文档/比赛材料/跨线同步六大领域）；补充工具链说明和重要声明
  - `DEMO_VIDEO_SCRIPT.md`：细化分镜（6 个时间段，中文字幕+旁白）；新增录制清单表格（标注每段的 Demo 实现状态）；增加 [effect] 标记区分后期合成内容
  - `SCREENSHOT_SHOTLIST.md`：重构为表格形式（12 必截 + 8 选截），每项标注对应 Demo 功能和实现状态
  - `README.md`："当前实现状态"从 4 行扩为 14 项已实现系统列表
- **对齐的 Demo 实际情况**: 所有文案基于 `game_simulation.gd` 中 `MAX_DEMO_DAY=14`、`_day_events` (1-14)、`_qimian_plan` (5/6/8/11/14)、`_default_locations` (9 节点)、`_default_facilities` (5 设施)、六大资源、感染/撤离/结局系统逐一校验
- **对其他线的影响**: 无阻塞项。14 vs 15 天不一致需待设计线+代码线协商确认

### [2026-05-27] 比赛线角色定位确立
- **改了什么**: 用户明确比赛线职责——不是规划任务，而是：①完成度审查（对照官方要求检查是否跑偏）、②合规建议（基于赛事手册给封装/提交建议）、③提交辅助（项目完成时协助封装与提交）
- **新增/修改的文件**: `HANDOFF.md`（重写比赛线 Purpose 和启动提示词）、`DECISIONS.md`（新增 Contest Lane Role Clarification）
- **对其他线的影响**: 无。该定位不改变其他线的所有权或工作方式
  - `DEMO_VIDEO_SCRIPT.md`：细化分镜（6 个时间段，中文字幕+旁白）；新增录制清单表格（标注每段的 Demo 实现状态）；增加 [effect] 标记区分后期合成内容
  - `SCREENSHOT_SHOTLIST.md`：重构为表格形式（12 必截 + 8 选截），每项标注对应 Demo 功能和实现状态；新增截图规则
  - `README.md`："当前实现状态"从 4 行扩为 14 项已实现系统列表
- **对齐的 Demo 实际情况**: 所有文案基于 `game_simulation.gd` 中 `MAX_DEMO_DAY=14`、`_day_events` (1-14)、`_qimian_plan` (5/6/8/11/14)、`_default_locations` (9 节点)、`_default_facilities` (5 设施)、六大资源、感染/撤离/结局系统逐一校验；确认当前 Demo 用 14 天（days 1-14），与设计文档中的 15 天计数存在不一致
- **对其他线的影响**: 无阻塞项。设计线需注意：比赛材料已统一为 14 天口径（对齐代码实现），CROSS_LANE_LOG 中 15 天统一决议待设计线和代码线协商决定

---

## 跨线阻塞/待同步项

<!-- 当某条线的变更需要其他线联动，但尚未完成时，在此记录 -->

| 日期 | 来源线 | 阻塞项 | 需要哪条线响应 | 状态 |
|------|--------|--------|---------------|------|
| 2026-05-27 | 总体规划 | 天数不统一(14天/15天混用),需统一为15天 | 设定线、代码线 | ✅ 已解决（双方已完成） |
| 2026-05-27 | 总体规划 | 祁烬Demo呈现方式未决策(广播/录音/偶遇/不出现) | 设定线 | ✅ 已解决（广播+擦肩） |
| 2026-05-27 | 总体规划 | 缺少15天逐日事件表,阻塞代码线实现 | 设定线 → 代码线 | ✅ 已解决（15天逐日事件表.md） |
| 2026-05-27 | 总体规划 | 缺少数值平衡表(资源消耗曲线/丧尸数值) | 设定线 → 代码线 | ✅ 已解决（策划案第7节） |
| 2026-05-27 | 比赛线 | 比赛材料已按14天口径，需重新对齐15天 | 比赛线 | ⏳待响应 |
| 2026-05-27 | 设定线 | 汽车系统为新机制(发现/攒件/组装/故障) | 代码线 | ✅ 已解决（第3轮实现） |
| 2026-05-27 | 设定线 | 一周目回放演出系统(5段分镜动画) | 代码线 | ⏳ 待后续（当前灰盒已有祁眠日志回放，5段分镜动画是Godot场景级工作） |
| 2026-05-27 | 设定线 | 二周目祁眠可操作角色切换 | 代码线 | ⏳ 待后续（二周目属于完整版scope） |
