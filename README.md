# BeyondSafeZone

《保护区之外》是一个 Unity 2D 像素末日生存经营游戏项目。

玩家一周目扮演林行，在 15 天叙事框架内白天外出搜刮、夜晚经营据点，并尝试撤离到保护区。第 5 天后，隐藏主角祁眠由本地确定性 AI 规则驱动，在同一张共享地图里夜间行动。玩家白天留下的痕迹会影响祁眠的任务排序，第二天通过异常档案、匿名药品、路线痕迹等反馈被玩家读到。

## Current Focus

- 当前主工程：`BeyondSafeZoneUnity/`
- 当前正式场景：`BeyondSafeZoneUnity/Assets/Scenes/OneRunMain.unity`
- 当前目标：先完成 10-15 分钟 Unity 灰盒纵切，而不是完整大体量版本。
- 当前核心链路：诊所异常 → 玩家留下求助标记 → 祁眠夜间读取 → 匿名药品回应 → 未知行动者档案 → 结尾日志解释。

## Project Structure

- `BeyondSafeZoneUnity/`：当前 Unity 主工程。
- `docs/`：策划、任务拆解、Unity 状态、项目记忆和决策记录。
- `docs/planning_package/`：当前统一策划包入口。
- `docs/reference/`：仍有实现价值的细节资料。
- `docs/archive/`：历史资料归档，不作为当前实现口径。
- `assets/`：源素材和导出素材。
- `marketing/`：参赛文案、视频脚本、截图说明。
- `builds/`：后续导出的可运行版本。

## Current Implementation

- Unity `OneRunMain` 运行时生成可走动据点、HUD、诊所/超市/车库搜刮灰盒。
- 已有 Day 1 基础循环、Day 5 后祁眠隐藏 AI 链路、诊所求助标记、匿名药品回应、未知行动者档案、结尾因果解释。
- 已实现 `U-007 未知行动者档案面板`：玩家可点击 HUD `档案` 按钮打开面板。
- 最新 Unity EditMode 回归：`BeyondSafeZone.Tests.TestGameSimulation` `42/42 passed`，jobId `09d9a3cb`。

## Workflow

- 开工先读 `AGENTS.md`、`HANDOFF.md`、`docs/CROSS_LANE_LOG.md`。
- 单次只推进一个明确任务编号。
- 没有“触发条件 / 玩家操作 / 状态变化 / 可见反馈 / 验证方法”的短规格，不进入实现。
- 完成口径必须同时满足：文档已更新、实现已落地、验证已记录。
