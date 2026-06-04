# Submission Plan

This is the master contest material plan for 《保护区之外》 / `Beyond Safe Zone`.

## Deadline And Track

- Active contest: Tencent Cloud Hackathon game development challenge 2026.
- Registration deadline: `2026-06-20`.
- Work submission deadline: `2026-06-20`.
- CodeBuddy Credits release cadence: every Friday.
- Primary track: `叙事类游戏：用 AI 重塑叙事体验`.
- Backup tracks are not recommended for the current pitch unless the game gains a clear public-welfare or cultural-expression layer.

## Important Links

- Contest page: `https://tch.cloud.tencent.com/contest/40`
- CodeBuddy: `https://www.codebuddy.cn/ide/`
- Work submission form: `https://wj.qq.com/s2/26396867/8ef8/`
- Registration form: `https://wj.qq.com/s2/26331484/2e19/`
- Contest manual: `https://img-bss.csdnimg.cn/bss/TencentCodeBuddyWorkshop/Tencent_Cloud_Hackathon_ZH.pdf`

## Submission Positioning

`Beyond Safe Zone` is a 2D pixel-art survival management demo about a visible survivor and a hidden AI-controlled protagonist sharing one collapsing city.

The player controls Lin Xing, a normal survivor trying to reach the safe zone. The AI controls Qimian, a hidden infected survivor who wakes on day 5 and secretly changes the shared map by moving resources, redirecting zombies, and leaving supplies — all resolved through a shared map with sequential time-slot settlement (Lin Xing daytime → Qimian nighttime → next day's consequences).

## Core Judge Takeaway

The AI feature is part of the game rules and narrative structure:

- AI has a role: Qimian, the hidden protagonist.
- AI has constraints: fixed personality card, deterministic decision rules, perceivable state only.
- AI changes gameplay: resources, routes, zombie density, anonymous supplies, and blood moon pressure — visible as `qimian` anomaly icons on the node map.
- AI creates replay value: the demo-end action log reveals the hidden causes behind the player's first run.

## Current Demo Truth (As Implemented in Godot Greybox)

- Lin Xing line covers the full 15-day survival cycle.
- Day/night loop: morning → daytime exploration → indoor search → evening → night shelter management.
- Node-based overworld with 14 locations in near/mid/far rings, bicycle range limits.
- Indoor search with room cards, careful/quick search, noise lure, hidden zombie risk, overstay fatigue.
- 5 shelter facilities: bed, workbench, window barricade, radio, storage/organizing table.
- 6 core resources: food, water, medicine, materials, parts, fuel.
- Infection system with readable stages (low risk / fever risk / dangerous infection) and wound treatment.
- Evacuation conditions: safezone_confirmed → address_known → car_ready.
- Car evacuation chain: find old car → repair engine wiring → replace tire → install battery → add gasoline → day 15 breakdown and final walk.
- Day 7 blood moon (tutorial defense); days 11-14 red-tide nights; day 15 final blood moon (escape pressure).
- Qimian wakes on day 5 with a fixed default personality card.
- Qimian hidden actions: clinic medicine pickup (day 6), supermarket night raid (day 8), bridge clearing (day 10), subway zombie diversion (day 11), red-tide observation/medicine drop (day 14), blood-moon zombie group route change (day 15).
- Affected locations display `qimian` trace icons on the node map starting from midgame.
- 3 endings: `reached_gate_quarantine`, `barely_reached_gate`, `collapsed`.
- Demo end: Qimian action log revealed (AI replay + subjective fragments).

**Do not claim** full playable Qimian route, complete pixel art, or full multi-ending structure unless those are implemented later. These are [planned] future scope.

## Material Package

- `marketing/DEMO_PITCH.md`: short pitch currently available.
- `marketing/DEMO_VIDEO_SCRIPT.md`: 1-3 minute video script.
- `marketing/AI_USAGE_STATEMENT.md`: AI feature and AI tool usage explanation.
- `marketing/PITCH_COPY.md`: submission page copy in short and long forms.
- `marketing/PPT_OUTLINE.md`: slide-by-slide presentation outline.
- `marketing/SCREENSHOT_SHOTLIST.md`: screenshot and recording checklist.

## Priority Order

1. Register before `2026-06-20` using the official registration form.
2. Freeze the demo scope around the 15-day Lin Xing cycle and Qimian hidden AI log.
3. Prepare a playable build or accessible project package.
4. Record a 1-3 minute demo video.
5. Finalize the AI feature and CodeBuddy usage explanation.
6. Finalize submission page copy and PPT outline.
7. Submit work before `2026-06-20` using the official submission form.

## Evidence To Collect

- Demo build or playable project path (Godot 4.6.2).
- Video capture of the full 15-day loop: morning→explore→indoor search→shelter→sleep→next day.
- Footage or screenshots of blood moon warnings and resolution (day 7, day 15) plus red-tide pressure (days 11-14).
- Footage or screenshots of Qimian anomaly clues on the node map (`qimian` trace icons).
- Footage or screenshots of the demo-end Qimian hidden action log.
- Screenshots of indoor search phase (room cards, search/lure buttons, fatigue pressure).
- Screenshots of shelter facility panel and resource state.
- AI/tool usage notes, especially CodeBuddy-assisted planning, coding, testing, and documentation.
- Any asset generation or processing notes, including FrameRonin if used.
