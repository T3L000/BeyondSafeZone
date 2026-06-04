# BeyondSafeZone Handoff

Last updated: 2026-06-04

## Contest Deadline Alert

- Current hard submission sprint targets the Tencent Cloud Hackathon game challenge.
- Registration deadline: `2026-06-20`.
- Work submission deadline: `2026-06-20`.
- CodeBuddy Credits are released every Friday.
- Primary track for this project: `叙事类游戏：用 AI 重塑叙事体验`.
- Official contest page: `https://tch.cloud.tencent.com/contest/40`.
- Registration form: `https://wj.qq.com/s2/26331484/2e19/`.
- Work submission form: `https://wj.qq.com/s2/26396867/8ef8/`.
- Manual link provided by user: `https://img-bss.csdnimg.cn/bss/TencentCodeBuddyWorkshop/Tencent_Cloud_Hackathon_ZH.pdf`.

## Start Here For Every Session

- Read `AGENTS.md` first if available.
- Read this `HANDOFF.md` before acting.
- **Read `docs/CROSS_LANE_LOG.md`** to check what other lanes changed since last session.
- Append durable session notes to `docs/PROJECT_MEMORY.md` before ending a conversation.
- **Append a summary entry to `docs/CROSS_LANE_LOG.md`** before ending a conversation.
- Add stable project decisions to `docs/DECISIONS.md`.
- Keep this file concise: only update it with information a fresh session must know immediately.

## Project Snapshot

Project path:

- `E:\Download\working\BeyondSafeZone`
- Unity migration target: `E:\Download\working\BeyondSafeZoneUnity`

Game:

- Chinese title: 《保护区之外》
- English title: `Beyond Safe Zone`
- Engine direction: Unity
- Current reference implementation: Godot 4.6.2 greybox under `game/`
- Style: 2D pixel-art survival management game
- Current target: 10-15 minute minimum playable vertical slice inside the 15-day narrative frame

Current development direction:

- User decided on 2026-06-02 to fully migrate the main project to Unity.
- Keep the existing Godot project as a reference for rules, data, text, tests, and greybox behavior.
- Do not continue expanding Godot as the main implementation unless the user explicitly reverses this decision.
- PlayKit.ai integration should use the Unity SDK; Godot SDK is not treated as currently available unless the user provides later evidence.
- PlayKit.ai should enhance narrative text only. Core gameplay rules remain deterministic local Unity/C# logic.

Core demo promise:

- Player controls Lin Xing.
- Lin Xing's visible goal is to evacuate to the safe zone.
- The current near-term design target is the minimum slice in `docs/MINIMUM_DEMO_SCOPE.md`, not full 15-day all-content production.
- Priority proof: Lin Xing explores a few core locations, finds an unknown-actor anomaly, leaves a marker, Qimian AI reads it at night, the next day map/dossier feedback changes, and the ending log explains the decision chain.
- Near-term locations are limited to Lin Xing home/shelter, community clinic, neighborhood supermarket, and repair shop/garage.
- Day 7 and Day 15 are blood moon events.
- Qimian is asleep for days 1-4, wakes on day 5, then secretly affects the shared world.
- Day 15 is an escape-pressure blood moon: Lin Xing leaves the failing shelter, heads toward the safe zone, and unknowingly passes a zombie group with Qimian hidden inside it.
- At demo end, Qimian's hidden action log is revealed.
- Code has been synced to the Lin Xing naming, Qimian day-5 wake timing, fuel/clue flag model, and day-14 safe-zone screening reveal.

## Read First

New sessions should read these files before acting:

- `AGENTS.md`
- `docs/CROSS_LANE_LOG.md` ← check other lanes' latest changes first
- `README.md`
- `docs/planning_package/README.md`
- `docs/planning_package/03_系统策划案_GDD.md`
- `docs/planning_package/04_详细策划案.md`
- `docs/PROJECT_MEMORY.md`
- `docs/DECISIONS.md`
- `docs/reference/DEMO_SCOPE.md`
- `docs/archive/legacy_design/策划案.md`
- `docs/ASSET_PIPELINE.md`
- `marketing/DEMO_PITCH.md`
- `game/scripts/core/game_simulation.gd`
- `game/scripts/main.gd`

If starting the Unity migration, first create `docs/UNITY_MIGRATION_PLAN.md` from the Godot reference files before creating or editing the Unity project.

Unity project note:

- `E:\Download\working\BeyondSafeZoneUnity` now exists and is being used for the main Unity greybox.
- UnitySkills REST server was verified on `http://127.0.0.1:8090/health` with Unity `2022.3.62f3c1`, UnitySkills `2.0.1`; latest verified mode was `bypass`.
- Current formal Unity scene: `Assets/Scenes/OneRunMain.unity`.
- `Assets/Scenes/MainPrototype.unity` is now a temporary greybox/reference scene only; do not keep expanding it as the main game scene.
- UnitySkills `scene_save` is blocked in `auto` mode as `MODE_FORBIDDEN`; latest verified UnitySkills mode was `bypass`.
- `Assets/Fonts/ChineseTMP.asset` is dynamic + multi-atlas, and its embedded `ChineseTMP Atlas` texture has been fixed to `m_IsReadable: 1` through `Assets/Editor/ChineseTmpAtlasReadableFixer.cs`.

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
- Unity project at `E:\Download\working\BeyondSafeZoneUnity`
- Unity `MainPrototype` scene with `GameController`, `MainPrototypeController`, Canvas UI, 4 location buttons, 5 action buttons, and TextMeshPro Chinese font asset
- Unity `OneRunMain` formal scene with `OneRunBootstrap` and runtime-generated walkable shelter / HUD / scavenging greybox

Current status:

- Godot implementation is the verified reference implementation.
- Unity implementation has a working reference greybox `MainPrototype` scene for the 4-location minimum slice.
- Unity implementation now has a formal `OneRunMain` scene for the first-run Lin Xing chapter. It generates a walkable shelter, HUD, clinic/supermarket/garage scavenging entries, and a player-facing `留下求助` help-mark button at runtime.
- `OneRunMain` Play verification confirmed `ScavengeGreybox_clinic`, `SearchPoint_waiting`, `SearchPoint_exam_a`, `SearchPoint_pharmacy`, and visible HUD feedback after leaving a clinic help mark.
- `Assets/Fonts/ChineseTMP.asset` has been set to dynamic atlas population with multi-atlas enabled, and the atlas readability issue that caused missing Chinese glyph warnings has been fixed.
- Latest Unity EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation` passed `35/35`, jobId `2c7a6f63`.
- Latest `OneRunMain` Play verification showed Console `warnings: 0`, `errors: 0`.

Current gameplay skeleton:

- Formal Unity `OneRunMain` scene runtime bootstrap
- Walkable shelter greybox with interactable bed, workbench, stove, barricade, radio, storage
- Top-down scavenging greybox entry for clinic, supermarket, and repair shop/garage
- Player-facing help-mark button in scavenging, currently verified in clinic
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
E:\Godot_v4.6.2\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd
```

Load Godot project headlessly:

```powershell
E:\Godot_v4.6.2\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --quit-after 1
```

Open project normally:

```powershell
E:\Godot_v4.6.2\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64_console.exe --path "E:\Download\working\BeyondSafeZone\game"
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

- `docs/planning_package/README.md`
- `docs/planning_package/01_策划总纲.md`
- `docs/planning_package/02_策划概要案.md`
- `docs/planning_package/03_系统策划案_GDD.md`
- `docs/planning_package/04_详细策划案.md`
- `docs/reference/DEMO_SCOPE.md`
- `docs/archive/legacy_design/策划案.md`
- `marketing/DEMO_PITCH.md`

Do not edit:

- `game/scripts/**` unless coordinating with the code conversation.

Good opening prompt:

```text
请接手 BeyondSafeZone 的设定/策划线。

开工协议：
1. 先读 docs/CROSS_LANE_LOG.md，检查其他线（代码/美术/比赛）上次会话后有无新变更。
2. 再读 HANDOFF.md、docs/planning_package/README.md、docs/reference/DEMO_SCOPE.md、marketing/DEMO_PITCH.md。

收工协议（在我认可改动后执行）：
1. 在 docs/CROSS_LANE_LOG.md 的 Design Lane 栏目下追加本次摘要。
2. 如有稳定决策，更新 docs/DECISIONS.md。
3. 更新 docs/PROJECT_MEMORY.md 记录详细上下文。

边界：不编辑 game/scripts/**。任务限定为完善设定、15天事件表、角色动机和参赛表达。
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

- `docs/UNITY_MIGRATION_PLAN.md`
- `docs/UNITY_MIGRATION_STATUS.md`
- `docs/reference/DEMO_SCOPE.md`
- `docs/archive/legacy_design/策划案.md`

Do not edit:

- `docs/archive/legacy_design/策划案.md` narrative sections unless the design conversation has agreed on the changes.

Good opening prompt:

```text
请接手 BeyondSafeZone 的代码线。

开工协议：
1. 先读 docs/CROSS_LANE_LOG.md，检查设定/美术/比赛线上次会话后有无新变更。
2. 再读 HANDOFF.md、README.md、docs/UNITY_MIGRATION_PLAN.md、docs/UNITY_MIGRATION_STATUS.md、game/scripts/core/game_simulation.gd、game/scripts/main.gd、game/tests/test_game_simulation.gd。

收工协议（在我认可改动后执行）：
1. 运行 Godot 模拟测试确认通过。
2. 在 docs/CROSS_LANE_LOG.md 的 Code Lane 栏目下追加本次摘要（含测试状态）。
3. 如有稳定决策，更新 docs/DECISIONS.md。
4. 更新 docs/PROJECT_MEMORY.md 记录详细上下文。

边界：不编辑 docs/archive/legacy_design/策划案.md 的叙事章节。优先推进 Unity 迁移；Godot 灰盒只作为参考和回归对照。
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
请接手 BeyondSafeZone 的美术素材线。

开工协议：
1. 先读 docs/CROSS_LANE_LOG.md，检查代码/设定/比赛线上次会话后有无新变更（特别是素材路径变更和新素材需求）。
2. 再读 HANDOFF.md、docs/ASSET_PIPELINE.md、docs/planning_package/03_系统策划案_GDD.md。

收工协议（在我认可改动后执行）：
1. 在 docs/CROSS_LANE_LOG.md 的 Art Lane 栏目下追加本次摘要（含素材规格/命名变更）。
2. 如有新增外部或AI生成素材，更新 docs/ASSET_LICENSE_LOG.md。
3. 更新 docs/PROJECT_MEMORY.md 记录详细上下文。

边界：不编辑 game/scripts/**。任务限定为像素素材清单、FrameRonin 流程和版权记录。
```

### 4. Contest / Submission Conversation

Purpose:

- **完成度审查**：对照官方赛事要求，检查项目是否跑偏、是否缺材料。
- **合规建议**：基于官方手册（大赛手册 PDF）给出封装与提交的指导意见。
- **提交辅助**：项目完成时协助准备比赛包、AI 使用说明、截图、视频脚本、CodeBuddy 使用记录和提交文案。
- **Not 任务规划**：比赛线不负责给其他线排开发计划。

Primary files:

- `marketing/`
- `docs/AI_CAN_DO_IT_游戏开发挑战赛分析.md`
- `docs/PROJECT_MEMORY.md`
- `docs/DECISIONS.md`
- `README.md`

Reference files:

- `docs/reference/DEMO_SCOPE.md`
- `docs/archive/legacy_design/策划案.md`
- `marketing/SUBMISSION_PLAN.md`

Good opening prompt:

```text
请接手 BeyondSafeZone 的参赛材料线。

你的职责不是规划任务，而是：
1. 完成度审查：对照大赛官方要求，检查项目是否跑偏、缺什么材料。
2. 合规建议：基于官方手册给出封装与提交指导意见。
3. 提交辅助：项目完成时协助封装与提交。

开工协议：
1. 先读 docs/CROSS_LANE_LOG.md，检查代码/设定/美术线上次会话后有无新变更（比赛材料必须对齐当前 Demo 实际状态）。
2. 再读 AGENTS.md、HANDOFF.md、docs/PROJECT_MEMORY.md、docs/DECISIONS.md、marketing/SUBMISSION_PLAN.md、docs/AI_CAN_DO_IT_游戏开发挑战赛分析.md、docs/reference/DEMO_SCOPE.md。

收工协议（在我认可改动后执行）：
1. 在 docs/CROSS_LANE_LOG.md 的 Contest Lane 栏目下追加本次摘要。
2. 更新 docs/PROJECT_MEMORY.md 记录详细上下文。

边界：不编辑 game/scripts/**。真实原则——比赛材料必须对齐当前 Demo 实际状态，计划中的功能必须标注为 future scope。不负责给其他线排开发计划。
```

## Coordination Rules

- `AGENTS.md` is the root-level collaboration rule file for future agents.
- Treat `HANDOFF.md` as the first file every new session reads.
- Treat `docs/CROSS_LANE_LOG.md` as the cross-lane changelog: read on startup, write on shutdown.
- Treat `docs/PROJECT_MEMORY.md` as the dated long-term project memory.
- Treat `docs/DECISIONS.md` as the stable decision log.
- Before ending any substantial session, update the active lane's files and append important notes to `docs/PROJECT_MEMORY.md` and `docs/CROSS_LANE_LOG.md`.
- Before editing, each conversation should state which files it will own.
- Code changes should run the Godot simulation test before claiming success.
- Design changes that affect mechanics should be mirrored into `docs/planning_package/03_系统策划案_GDD.md` or `docs/planning_package/04_详细策划案.md`; keep `docs/reference/DEMO_SCOPE.md` as reference unless the user asks to refresh it.
- Asset changes should update `docs/ASSET_PIPELINE.md` with size, source, and license notes.
- Contest wording should not invent implemented features; it should match the current demo.
- Avoid parallel edits to the same file from different conversations.

## Suggested Next Tasks

Code line:

- Continue from Unity `Assets/Scenes/OneRunMain.unity`, not `MainPrototype`.
- Implement the next P0 AI feedback chain in `OneRunMain`: Day 5+ Qimian reads the clinic `help` mark, then the player sees anonymous medicine / response traces on the next day.
- Verify the full chain and record exact results in `docs/UNITY_MIGRATION_STATUS.md`: enter clinic → leave help mark → resolve night after Qimian wake → next-day feedback → ending/Qimian log explanation.
- Add focused Unity tests before changing rule behavior. Current full regression baseline: `TestGameSimulation` `35/35 passed`, jobId `2c7a6f63`.
- Use the Godot project under `game/` as reference for rules/data/text/tests, but do not expand Godot as the main implementation unless the user reverses the Unity decision.

Design line:

- Keep `docs/MINIMUM_DEMO_SCOPE.md`, `docs/planning_package/`, and `docs/开发任务拆解.md` aligned around the 4-location minimum slice.
- Detail only the P0 clinic AI chain first: Day 6 anomaly, help marker, Qimian response, Day 7 feedback, ending log explanation.
- Leave full second run, 14-location production, complex dice/action-point systems, long NPC cooperation, and replay animations as future scope.

Art line:

- Use the locked art spec in `docs/ASSET_PIPELINE.md`: `32x32` characters, `16x16` base tiles, larger props as `32x32`, `32x48`, or tile combinations.
- Produce the Must Have art batch: Lin Xing, Qimian, normal zombie, blood moon zombie, core demo locations, and resource/status UI icons.
- Record every external or AI-generated asset in `docs/ASSET_LICENSE_LOG.md` before it enters `assets/sprites/`.

Contest line:

- Treat `2026-06-20` as the hard deadline for both registration and work submission.
- Prepare materials for the `叙事类游戏：用 AI 重塑叙事体验` track unless the user changes direction.
- Expand `marketing/SUBMISSION_PLAN.md` into final submission-ready copy.
- Refine `marketing/DEMO_VIDEO_SCRIPT.md` into a timed narration script.
- Refine `marketing/AI_USAGE_STATEMENT.md` with actual CodeBuddy/Codex usage evidence.
- Use `marketing/SCREENSHOT_SHOTLIST.md` while capturing demo footage.
