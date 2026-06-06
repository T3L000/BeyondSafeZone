# Submission Plan

This is the contest material plan for 《保护区之外》 / `Beyond Safe Zone`.

## Deadline And Track

- Active contest: Tencent Cloud Hackathon game development challenge 2026.
- Registration deadline: `2026-06-20`.
- Work submission deadline: `2026-06-20`.
- Primary track: `叙事类游戏：用 AI 重塑叙事体验`.

## Submission Positioning

`Beyond Safe Zone` is a 2D pixel-art survival management game about a visible survivor and a hidden AI-controlled protagonist sharing one collapsing city.

The player controls Lin Xing. The AI controls Qimian, a hidden infected survivor who wakes on day 5. Qimian reads only perceivable traces in the shared world, such as location changes and player marks, then modifies the same map through deterministic local rules.

## Core Judge Takeaway

- AI has a role: Qimian, the hidden protagonist.
- AI has constraints: fixed personality card, deterministic rules, perceivable state only.
- AI changes gameplay: the shared map, dossier, resources, and traces can change after Qimian acts.
- AI is explainable: the ending log shows input, options, ordering, final choice, and map impact.

## Current Unity Greybox Truth

- Current project: `BeyondSafeZoneUnity/`.
- Current formal scene: `Assets/Scenes/OneRunMain.unity`.
- Runtime greybox generates shelter, Lin Xing, HUD, and core location entries.
- Player can enter clinic, supermarket, and garage greybox locations.
- Player can leave a help marker in the current scavenging location.
- Day 5 onward, Qimian can read the clinic help marker.
- Clinic help marker can trigger anonymous medicine / shallow-arrow feedback.
- Unknown-actor dossier panel is available through HUD `档案`.
- Ending reveal text explains the clinic marker causal chain.
- Latest recorded Unity EditMode regression: `42/42 passed`.

Do not claim the following as implemented unless they are completed later:

- Formal pixel art.
- Complete second-run playable Qimian campaign.
- Full action-point/dice system.
- Long-term NPC cooperation system.
- Five animated replay scenes.
- Fully polished 15-day content with all locations.

## Material Package

- `marketing/DEMO_PITCH.md`: short demo pitch.
- `marketing/DEMO_VIDEO_SCRIPT.md`: 1-2 minute greybox video script.
- `marketing/AI_USAGE_STATEMENT.md`: AI feature and tool usage explanation.
- `marketing/PITCH_COPY.md`: submission copy.
- `marketing/PPT_OUTLINE.md`: presentation outline.
- `marketing/SCREENSHOT_SHOTLIST.md`: screenshot and recording checklist.

## Priority Order

1. Keep the project scope locked to the Unity greybox vertical slice.
2. Record the current clinic AI chain clearly.
3. Prepare screenshots of shelter, clinic, help marker, dossier, and ending explanation.
4. Prepare a short video focused on “AI as hidden protagonist”.
5. Finalize AI usage explanation.
6. Submit before `2026-06-20`.

## Evidence To Collect

- Unity project path and scene path.
- Footage of `OneRunMain` Play mode.
- Footage of clinic entry and help marker.
- Footage of night feedback with anonymous medicine / shallow arrow.
- Screenshot of the unknown-actor dossier panel.
- Screenshot or text capture of the ending causal explanation.
- Fresh Unity Test Runner result before final submission.
