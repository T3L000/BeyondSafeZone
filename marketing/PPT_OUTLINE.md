# PPT Outline

> **对齐版本**: 基于 Godot 灰盒 Demo (`game/scripts/core/game_simulation.gd`) 实际实现状态撰写。
> 计划中的功能标为 `future scope`。

## Slide 1: Title

- 《保护区之外》 / `Beyond Safe Zone`
- 2D pixel-art survival management demo (Godot 4.6.2)
- Core line: survive outside the safe zone while a hidden AI protagonist changes the shared world

## Slide 2: Problem / Opportunity

- Many AI games place AI in dialogue or generation tools.
- This project asks: what if AI controls a hidden protagonist whose rule-based decisions become part of the player's survival story?

## Slide 3: Core Gameplay (已实现)

- **日夜循环**: 清晨 → 白天探索 → 黄昏 → 夜晚经营
- **节点式大地图**: 14 个地点（近圈/中圈/远圈），自行车范围限制
- **室内搜索**: 俯拍灰盒潜行，谨慎/快速搜索，噪音引尸，隐藏丧尸风险，超时疲劳
- **五大据点设施**: 床铺、工作台、封窗、收音机、储物/整理台
- **六大核心资源**: 食物、水、药品、建材、零件、燃料
- **感染系统**: 可读阶段（低风险 / 发热风险 / 危险感染），药品治疗

## Slide 4: AI Innovation (已实现)

- Qimian 由确定性规则人格卡控制（非概率摇点）。
- Qimian 在第 5 天醒来，在同一张共享地图上行动。
- Qimian 修改资源、门锁、尸群密度、路线状态，产生匿名物资。
- 玩家一周目只看到后果（被搬空的货架、异常减少的尸群、匿名药品）。
- Demo 结尾解锁祁眠完整行动日志。

## Slide 5: Demo Flow (当前实现)

- **第 1-4 天**: 学习生存循环，祁眠昏睡
- **第 5 天**: 祁眠醒来，确认自身变化，将寻找祁烬定为主任务；玩家感到异常
- **第 6 天**: 血月前兆预警
- **第 7 天**: 第一次血月（教学型防守）
- **第 8-10 天**: 中圈探索，异常痕迹持续出现
- **第 11-14 天**: 红潮夜逐步加密，完成汽车修理和撤离准备
- **第 15 天**: 终局血月——林行撤离，旧车故障后徒步抵达保护区门口；祁眠藏身尸群间接影响路线
- **结尾**: 林行通过初筛被隔离观察；玩家解锁祁眠行动日志

## Slide 6: Ending States (已实现)

- `reached_gate_quarantine`: 抵达保护区门口，通过初筛，隔离观察
- `barely_reached_gate`: 勉强抵达保护区门口
- `collapsed`: 崩溃边缘

祁眠日志揭示所有隐藏因果：AI 行动回放 + 祁眠主观残句。

## Slide 7: Why It Is Replayable

- 第一轮: 生存 + 谜团
- 结尾揭示: 隐藏因果链
- Future scope: 不同人格卡生成不同隐藏因果 / 祁眠可玩路线 / AI 继承林行行为

## Slide 8: Development Status

- **当前 Demo**: 15 天完整灰盒循环已实现并测试通过
- **引擎**: Godot 4.6.2
- **已实现系统**: 日夜循环、资源经营、五大设施、节点地图、室内搜索、感染/药品闭环、血月/红潮结算、汽车撤离、祁眠 AI 行动、共享地图、三层结局
- **素材状态**: 灰盒占位，像素素材待替换

## Slide 9: AI/Tool Usage

- **游戏内 AI**: 隐藏主角行为引擎（人格卡 → 确定性规则 → 共享世界改写）
- **开发中 AI**: CodeBuddy 用于策划规划、GDScript 实现、测试编写、文档生成、比赛材料起草
- 人类负责范围控制、叙事决策、实现审核和比赛材料真实性校验

## Slide 10: Closing

- AI 不是附加功能。
- AI 是那个你看不见、但已经在改变你世界的人。
- 你以为是随机末日，其实是另一个角色的规则驱动选择。
