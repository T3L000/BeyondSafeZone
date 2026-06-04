# Pitch Copy

> **对齐版本**: 基于 Godot 灰盒 Demo 实际实现状态。标注 `[planned]` 为计划功能。

## One-Sentence Pitch

`Beyond Safe Zone` is a 2D pixel-art survival management game where the player survives as Lin Xing while an AI-controlled hidden protagonist, Qimian, secretly changes the same shared map.

## Short Pitch

在《保护区之外》, the player controls Lin Xing, a normal survivor trying to reach the safe zone through daytime exploration and nighttime shelter management. But the world is not static. Qimian, another protagonist controlled by a deterministic AI personality card, wakes on day 5 and acts in the same city outside the player's view. The player first sees only the consequences: missing supplies, shifted zombie groups, anonymous medicine, and strange map traces. At the end of the 15-day demo, Qimian's action log reveals the hidden cause behind those events — and Lin Xing reaches the safe-zone gate only to be placed under quarantine observation.

## Long Pitch

《保护区之外》 is a 2D pixel-art survival management demo built around a visible survival story and a hidden AI-driven story.

The player plays Lin Xing, a normal survivor outside the safe zone. Each day, Lin Xing explores by bicycle across a node-based overworld of 14 locations (near/mid/far rings), gathers food, water, medicine, materials, parts, and fuel, then returns at night to manage five shelter facilities — bed, workbench, window barricade, radio, and storage table. The final evacuation requires finding and repairing an old car. Day 7 brings the first blood moon (tutorial defense), days 11-14 escalate through red-tide nights, and day 15 brings the final blood moon (escape pressure).

The innovation is that another protagonist is also alive in the same world. Qimian is infected but conscious, wakes on day 5, and is controlled by a deterministic AI rule engine driven by a personality card. Qimian can move resources, redirect zombies, leave anonymous supplies, and change route danger — all resolved through a shared map with sequential time-slot settlement. The player does not see Qimian's actions directly during the first run, but the consequences are real and traceable: precise medicine thefts at the clinic, emptied supermarket shelves, reduced zombie density near the subway, and `qimian` anomaly icons appearing on the node map.

When the 15-day demo ends, Lin Xing reaches the safe-zone gate, passes initial infection screening, and is placed under quarantine observation. Qimian's hidden action log is then revealed. Events that seemed random become traceable decisions made by another character. The AI is therefore not a chat window or content generator; it is a hidden protagonist whose rule-driven choices reshape the player's survival experience.

**Current demo endings**: `reached_gate_quarantine` (arrived at gate, quarantined), `barely_reached_gate`, `collapsed`.

**[planned]** Future scope includes first-run anomaly investigation and indirect marker input, playable Qimian key action chapters, procedurally generated personality cards per run, and AI-inherited Lin Xing behavior in subsequent playthroughs. A complete second 15-day Qimian campaign is full-version scope, not the contest demo promise.
