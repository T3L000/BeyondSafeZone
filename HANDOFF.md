# BeyondSafeZone Handoff

Last updated: 2026-05-21

## Start Here For Every Session

- Read `AGENTS.md` first if available.
- Read this `HANDOFF.md` before acting.
- Append durable session notes to `docs/PROJECT_MEMORY.md` before ending a conversation.
- Add stable project decisions to `docs/DECISIONS.md`.
- Keep this file concise: only update it with information a fresh session must know immediately.

## Project Snapshot

Project path:

- `E:\Download\working\BeyondSafeZone`

Game:

- Chinese title: 《保护区之外》
- English title: `Beyond Safe Zone`
- Engine: Godot 4.6.2
- Style: 2D pixel-art survival management game
- Current target: contest demo, not the full 30-day game

Core demo promise:

- Player controls Chen Xing.
- Chen Xing's visible goal is to evacuate to the safe zone.
- The demo covers the first 14 days.
- Day 7 and Day 14 are blood moon events.
- Qimian is asleep for days 1-10, wakes on day 11, then secretly affects the shared world.
- At demo end, Qimian's hidden action log is revealed.

## Read First

New sessions should read these files before acting:

- `AGENTS.md`
- `README.md`
- `docs/PROJECT_MEMORY.md`
- `docs/DECISIONS.md`
- `docs/策划案.md`
- `docs/DEMO_SCOPE.md`
- `docs/ASSET_PIPELINE.md`
- `marketing/DEMO_PITCH.md`
- `game/scripts/core/game_simulation.gd`
- `game/scripts/main.gd`

## Current Implementation

Implemented:

- Root collaboration guide in `AGENTS.md`
- Long-term memory file in `docs/PROJECT_MEMORY.md`
- Stable decision log in `docs/DECISIONS.md`
- Contest material package skeleton in `marketing/`
- Godot project skeleton in `game/`
- Greybox UI in `game/scripts/main.gd`
- Core simulation in `game/scripts/core/game_simulation.gd`
- Simulation test in `game/tests/test_game_simulation.gd`
- Design docs in `docs/`
- Pitch draft in `marketing/`

Current gameplay skeleton:

- Day/night loop
- Daytime location exploration
- Night shelter actions
- Resource consumption
- Bicycle range limit
- Blood moon resolution
- Qimian hidden action plan
- Shared map changes caused by Qimian
- Demo-end Qimian log reveal

## Commands

Run simulation tests:

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd
```

Load Godot project headlessly:

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --quit-after 1
```

Open project normally:

```powershell
Godot_v4.6.2-stable_mono_win64_console.exe --path "E:\Download\working\BeyondSafeZone\game"
```

Last known verification:

- Simulation tests passed with `All simulation tests passed.`
- Headless Godot project load exited successfully.

## Multi-Conversation Workflow

Use separate conversations by ownership. Each conversation should avoid editing files outside its lane unless explicitly asked.

### 1. Lore / Design Conversation

Purpose:

- Flesh out worldbuilding, character motivations, 14-day event table, dialogue, endings, and pitch framing.

Primary files:

- `docs/策划案.md`
- `docs/DEMO_SCOPE.md`
- `marketing/DEMO_PITCH.md`

Do not edit:

- `game/scripts/**` unless coordinating with the code conversation.

Good opening prompt:

```text
请接手 BeyondSafeZone 的设定线。先阅读 HANDOFF.md、docs/策划案.md、docs/DEMO_SCOPE.md、marketing/DEMO_PITCH.md。你的任务是只完善设定、14天事件表、角色动机和参赛表达，不要改 game/scripts 代码。
```

### 2. Code Conversation

Purpose:

- Implement Godot gameplay, tests, UI, data structures, and export-ready demo behavior.

Primary files:

- `game/scripts/core/game_simulation.gd`
- `game/scripts/main.gd`
- `game/tests/test_game_simulation.gd`
- `game/scenes/main.tscn`

Reference files:

- `docs/DEMO_SCOPE.md`
- `docs/策划案.md`

Do not edit:

- `docs/策划案.md` narrative sections unless the design conversation has agreed on the changes.

Good opening prompt:

```text
请接手 BeyondSafeZone 的代码线。先阅读 HANDOFF.md、README.md、docs/DEMO_SCOPE.md、game/scripts/core/game_simulation.gd、game/scripts/main.gd、game/tests/test_game_simulation.gd。继续实现 Godot 灰盒 Demo，并保持测试通过。
```

### 3. Art / Asset Conversation

Purpose:

- Define pixel-art specs, FrameRonin workflow, sprite list, placeholder rules, and copyright tracking.

Primary files:

- `docs/ASSET_PIPELINE.md`
- `assets/`

Possible later files:

- `assets/source/`
- `assets/sprites/`
- `marketing/`

Do not edit:

- `game/scripts/**` unless adding already-agreed asset paths.

Good opening prompt:

```text
请接手 BeyondSafeZone 的美术素材线。先阅读 HANDOFF.md、docs/ASSET_PIPELINE.md、docs/策划案.md。你的任务是规划并产出统一像素素材清单、FrameRonin 使用流程和版权记录，不要改玩法代码。
```

### 4. Contest / Submission Conversation

Purpose:

- Prepare contest package, AI usage explanation, screenshots, video outline, CodeBuddy usage record, and submission copy.

Primary files:

- `marketing/`
- `docs/AI_CAN_DO_IT_游戏开发挑战赛分析.md`
- `docs/PROJECT_MEMORY.md`
- `docs/DECISIONS.md`
- `README.md`

Reference files:

- `docs/DEMO_SCOPE.md`
- `docs/策划案.md`
- `marketing/SUBMISSION_PLAN.md`

Good opening prompt:

```text
请接手 BeyondSafeZone 的参赛材料线。先阅读 AGENTS.md、HANDOFF.md、docs/PROJECT_MEMORY.md、docs/DECISIONS.md、marketing/SUBMISSION_PLAN.md、marketing/DEMO_PITCH.md、docs/AI_CAN_DO_IT_游戏开发挑战赛分析.md、docs/DEMO_SCOPE.md。你的任务是准备参赛说明、AI 使用说明、视频脚本和提交材料清单，不要改 game/scripts 代码。
```

## Coordination Rules

- `AGENTS.md` is the root-level collaboration rule file for future agents.
- Treat `HANDOFF.md` as the first file every new session reads.
- Treat `docs/PROJECT_MEMORY.md` as the dated long-term project memory.
- Treat `docs/DECISIONS.md` as the stable decision log.
- Before ending any substantial session, update the active lane's files and append important notes to `docs/PROJECT_MEMORY.md`.
- Before editing, each conversation should state which files it will own.
- Code changes should run the Godot simulation test before claiming success.
- Design changes that affect mechanics should be mirrored into `docs/DEMO_SCOPE.md` before code implements them.
- Asset changes should update `docs/ASSET_PIPELINE.md` with size, source, and license notes.
- Contest wording should not invent implemented features; it should match the current demo.
- Avoid parallel edits to the same file from different conversations.

## Suggested Next Tasks

Code line:

- Add a deterministic 14-day event table instead of only procedural safe demo actions.
- Improve the greybox UI so the player clearly sees phase, blood moon warnings, and Qimian clues.
- Add a simple failure/survival ending state for day 14.

Design line:

- Write the first 14 days as a table using the locked home wake-up opening: morning clue, exploration pressure, night event, Qimian hidden truth.
- Clarify Qijin's role in the demo without over-expanding the scope.

Art line:

- Use the locked art spec in `docs/ASSET_PIPELINE.md`: `32x32` characters, `16x16` base tiles, larger props as `32x32`, `32x48`, or tile combinations.
- Produce the Must Have art batch: Chen Xing, Qimian, normal zombie, blood moon zombie, core demo locations, and resource/status UI icons.
- Record every external or AI-generated asset in `docs/ASSET_LICENSE_LOG.md` before it enters `assets/sprites/`.

Contest line:

- Expand `marketing/SUBMISSION_PLAN.md` into final submission-ready copy.
- Refine `marketing/DEMO_VIDEO_SCRIPT.md` into a timed narration script.
- Refine `marketing/AI_USAGE_STATEMENT.md` with actual CodeBuddy/Codex usage evidence.
- Use `marketing/SCREENSHOT_SHOTLIST.md` while capturing demo footage.
