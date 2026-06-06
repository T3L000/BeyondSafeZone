# BeyondSafeZone Handoff

Last updated: 2026-06-05

## Start Here

- Read `AGENTS.md`.
- Read this `HANDOFF.md`.
- Read `docs/CROSS_LANE_LOG.md` and check unresolved cross-lane items.
- For Unity/code work, read:
  - `docs/MINIMUM_DEMO_SCOPE.md`
  - `docs/UNITY_STATUS.md`
  - `docs/开发任务拆解.md`
  - `BeyondSafeZoneUnity/Assets/Scripts/UI/OneRunGameController.cs`
  - `BeyondSafeZoneUnity/Assets/Tests/TestGameSimulation.cs`
- Before ending a substantial session, update:
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/DECISIONS.md` only if a stable decision changed

## Current Canon

- Repository root: `E:\Download\working\BeyondSafeZone`
- Active Unity project: `E:\Download\working\BeyondSafeZone\BeyondSafeZoneUnity`
- The sibling folder `E:\Download\working\BeyondSafeZoneUnity` is deprecated and should not be used.
- Engine target: Unity 2022.3 LTS.
- Active scene: `Assets/Scenes/OneRunMain.unity`.
- `Assets/Scenes/MainPrototype.unity` is only a temporary reference scene.
- Current target: a 10-15 minute Unity greybox vertical slice inside the 15-day Lin Xing first-run frame.
- Core proof: Lin Xing discovers an anomaly, leaves a help marker, Qimian AI reads it at night, the shared map/dossier changes, and the ending log explains the hidden decision chain.

## Current Verified Unity State

- `OneRunMain` generates a walkable shelter, HUD, clinic/supermarket/garage scavenging entries, and a help-mark button at runtime.
- P0 clinic AI chain is implemented and tested:
  - clinic anomaly
  - `clinic/help` player mark
  - Qimian reads the mark after waking on day 5
  - anonymous medicine / shallow-arrow response
  - anomaly dossier records the chain
  - ending reveal explains the causality
- U-007 is implemented:
  - HUD `档案` button
  - `未知行动者档案` panel
  - empty text `暂无异常记录。`
  - panel text reads `GameSimulation.GetAnomalyDossierText(State)`
- Latest Unity EditMode regression:
  - `BeyondSafeZone.Tests.TestGameSimulation`
  - `42/42 passed`
  - jobId `09d9a3cb`
- Latest known Unity Console caveat:
  - `warnings: 1`, `errors: 0`
  - warning is from `com.unity.ide.visualstudio@2.0.22` UDP messaging, not project code.

## Development Rules

- Work in single-task loops.
- Do not implement a task unless its short spec has:
  - trigger condition
  - player action
  - state change
  - visible feedback
  - verification method
- A task is complete only when:
  - docs updated
  - implementation landed
  - verification recorded
- Do not expand into second-run Qimian gameplay, action points, dice, NPC cooperation, or formal art unless the user explicitly selects that task.
- Code lane owns `BeyondSafeZoneUnity/Assets/Scripts/**`, `BeyondSafeZoneUnity/Assets/Tests/**`, and `BeyondSafeZoneUnity/Assets/Scenes/**`.

## Suggested Next Code Tasks

- `U-008`: Qimian ending log panel, so the ending reveal is readable in a dedicated UI instead of only through text output.
- Or improve the playable shelter/scavenging feel inside `OneRunMain`, but only after writing a short spec.

## Contest Reminder

- Active contest: Tencent Cloud Hackathon game development challenge 2026.
- Registration deadline: `2026-06-20`.
- Work submission deadline: `2026-06-20`.
- Track: `叙事类游戏：用 AI 重塑叙事体验`.
- Contest-facing text must describe the current Unity greybox truth and mark future scope clearly.
