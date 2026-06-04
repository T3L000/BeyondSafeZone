# BeyondSafeZone

《保护区之外》是一个 2D 像素末日经营游戏项目，当前主开发方向已切换为 Unity，Godot 4 灰盒保留为规则和行为参考。

## 当前目标

- 做出林行线 15 天完整生存周期 Unity 灰盒 Demo
- 包含第 7 天和第 15 天两次血月
- 祁眠第 5 天醒来并开始隐藏影响共享地图
- 让玩家以“撤离到保护区”为表层目标推进经营
- 在 Demo 结尾解锁祁眠隐藏行动日志，解释一周目中的异常资源和尸群变化

## 目录

- `docs/`：当前策划入口、Unity 迁移资料、素材规范、项目记忆
- `docs/planning_package/`：当前统一策划包入口（总纲、概要案、GDD、详细策划案）
- `docs/reference/`：仍有实现价值的细节资料（逐日表、地点数据、共享地图 API、祁眠 AI 伪代码等）
- `docs/archive/`：旧入口、历史报告、原型和媒体归档
- `game/`：Godot 4 灰盒参考工程
- `assets/`：源素材和导出的 Sprite Sheet
- `builds/`：后续导出的可运行版本
- `marketing/`：参赛文案、视频脚本、截图说明

## 运行 Godot 参考灰盒

本机 Godot 控制台命令：

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --path game
```

运行核心模拟测试：

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --headless --path game --script res://tests/test_game_simulation.gd
```

## 当前实现状态

- Godot 参考灰盒已有完整 15 天循环，模拟测试曾通过
- Unity 迁移计划见 `docs/UNITY_MIGRATION_PLAN.md`
- Unity 迁移状态见 `docs/UNITY_MIGRATION_STATUS.md`
- 日夜循环（清晨→白天→室内搜索→黄昏→夜晚经营→睡觉结算）
- 节点式大地图（14 个地点，近/中/远三圈，自行车范围限制，qimian 异常标记）
- 室内搜索灰盒（房间卡片、谨慎/快速搜索、噪音引尸、隐藏丧尸风险、超时疲劳）
- 五大据点设施（床铺、工作台、封窗、收音机、储物/整理台）
- 六大核心资源（食物、水、药品、建材、零件、燃料）
- 感染系统（可读阶段：低风险/发热风险/危险感染）+ 药品治疗
- 撤离条件（safezone_confirmed / address_known / car_ready）
- 汽车撤离系统（发现旧车→引擎→轮胎→电瓶→加油→Day 15 出发后故障徒步）
- 第 7 天血月（教学防守）+ 第 11-14 天红潮夜 + 第 15 天终局血月
- 祁眠第 5 天醒来，确定性人格卡控制，隐藏影响共享地图
- Demo 结尾三层结局 + 祁眠行动日志（AI 回放 + 主观残句）
- 美术仍是灰盒占位阶段，后续用 FrameRonin 和手工修正统一像素素材
