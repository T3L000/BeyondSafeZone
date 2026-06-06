# AI Usage Statement

## Game AI Feature

`Beyond Safe Zone` 的核心 AI 设计是祁眠：一个由本地确定性规则驱动的隐藏主角。

祁眠不是聊天 NPC，也不是任务发布器。他在林行看不见的夜晚读取同一张共享地图上的可感知痕迹，并改写地点状态。玩家第二天看到的是结果：异常档案、匿名药品、浅箭头、地点状态变化和结尾日志解释。

## How It Works

1. **固定人格卡**
   当前 Demo 使用固定默认人格：寻找祁烬优先、谨慎避开暴露、会帮助近处的人、只拿任务所需资源、不信任保护区筛查。

2. **确定性规则**
   祁眠的行动选择由本地规则排序，不是概率抽卡。相同世界状态和相同人格卡会产生同类行动。

3. **可感知输入限制**
   祁眠只读取他能看见、听见或推断到的信息，例如地点痕迹、玩家留下的世界内标记、广播线索和尸群变化。它不能读取林行后台血量、背包或玩家计划。

4. **分时段共享地图**
   林行白天探索并改写地点状态；祁眠夜晚读取可感知痕迹并再次改写；次日林行看到后果。当前实现不是实时双角色同屏，而是一套共享地点状态的顺序结算。

5. **一周目可读互动**
   当前 Unity 灰盒已实现诊所链路：诊所异常、求助标记、祁眠读取、匿名药品/浅箭头回应、未知行动者档案、结尾日志解释。

6. **通关后解释**
   结尾日志展示人格卡、感知输入、候选行动、排序理由、最终选择和地图影响，让玩家理解“看似随机”的异常来自另一个角色的规则选择。

## Current Unity Greybox Truth

- 当前主工程：`BeyondSafeZoneUnity/`
- 当前正式场景：`Assets/Scenes/OneRunMain.unity`
- 当前可展示重点：
  - 可走动据点灰盒
  - 诊所、超市、车库入口
  - HUD `留下求助`
  - HUD `档案` 按钮与未知行动者档案面板
  - 诊所 AI 因果链
  - Unity EditMode 回归测试覆盖该链路
- 当前仍属于灰盒阶段：正式像素美术、完整二周目、复杂行动点/骰子和长期 NPC 合作不作为已实现功能宣传。

## Development AI Usage

CodeBuddy / Codex 用于辅助：

| Area | Usage |
|------|-------|
| Planning | 整理策划案、One Page、GDD、任务拆解和范围控制 |
| Implementation | 编写 Unity C# 灰盒逻辑、UI 原型和测试 |
| Testing | 编写 Unity EditMode 测试并记录验证证据 |
| Documentation | 维护 `AGENTS.md`、`HANDOFF.md`、`docs/CROSS_LANE_LOG.md`、`docs/PROJECT_MEMORY.md`、`docs/DECISIONS.md` |
| Contest Materials | 起草并修订介绍、PPT、视频脚本、AI 使用说明 |

Human developer keeps final control over scope, narrative decisions, implementation review, and contest truthfulness.

## Important Notes

- Game AI uses local deterministic rules, not a large language model at runtime.
- PlayKit.ai is planned as a narrative text enhancement layer only. It must not decide resources, damage, legal actions, endings, or Qimian's local task choice.
- No API token or secret should be committed to the repository.
