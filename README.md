# BeyondSafeZone

《保护区之外》是一个 Godot 4 单人开发项目：2D 像素末日经营游戏，首版目标是比赛 Demo。

## 当前目标

- 做出陈醒线前 14 天灰盒 Demo
- 包含第 7 天和第 14 天两次血月
- 让玩家以“撤离到保护区”为表层目标推进经营
- 在 Demo 结尾解锁祁眠隐藏行动日志，解释一周目中的异常资源和尸群变化

## 目录

- `docs/`：策划案、比赛分析、Demo 设计资料
- `game/`：Godot 4 工程
- `assets/`：源素材和导出的 Sprite Sheet
- `builds/`：后续导出的可运行版本
- `marketing/`：参赛文案、视频脚本、截图说明

## 运行

本机 Godot 控制台命令：

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --path game
```

运行核心模拟测试：

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --headless --path game --script res://tests/test_game_simulation.gd
```

## 当前实现状态

- 已有 Godot 工程骨架
- 已有灰盒 UI
- 已有 C 线核心资源、据点、自行车、血月和祁眠隐藏日志模拟
- 美术仍是占位阶段，后续用 FrameRonin 和手工修正统一像素素材

