# Decisions

This file records stable decisions for BeyondSafeZone. Add entries only when the decision should guide future sessions.

## 2026-05-21

### Demo Scope

- The contest demo target is the first 14 days, not the full 30-day game.
- Chen Xing wakes at home, not in a hospital. The home doubles as the emotional opening and early shelter anchor.
- Day 7 and day 14 are blood moon events.
- Chen Xing is the first playable perspective.
- Qimian sleeps during days 1-10, wakes on day 11, then secretly affects the shared world.
- The demo-end reveal is Qimian's hidden action log.

### Contest Positioning

- The contest pitch should emphasize that AI is not a chat UI; AI is the hidden protagonist action engine.
- The strongest submission angle is "AI-controlled hidden protagonist changes the same world the player is trying to survive in."
- Contest materials should not invent implemented features. Planned ideas must be labeled as planned or future scope.

### Worldbuilding

- Qijin's organization is the Rebirth Project: a hybrid group with a scientific outer shell and religious/internal doctrine.
- The Rebirth Project publicly frames itself through research, samples, cleansing, and life-extension language; internally it treats infection as "rebirth" and city collapse as a necessary selection process.

### Core Character Setup

- Chen Xing, Qimian, and Qijin are all male.
- Chen Xing and Qimian are old friends who were close in childhood, disconnected as adults, and have a faded relationship by the time the demo begins.
- Chen Xing, Qimian, and Qijin made a childhood "apocalypse shelter plan"; the home opening can use old maps, notes, and routes from that plan.
- Qimian does not immediately think of Chen Xing after waking. His first arc is recalling his infection experience, confirming his changed body, becoming confused, then choosing to find Qijin as his main task.
- Qimian's desire to help strangers develops later from what he sees during night actions, not from an instant heroic mission.

### Collaboration

- Future sessions should use `AGENTS.md` and `HANDOFF.md` as the first coordination layer.
- When both files exist, read `AGENTS.md` first, then `HANDOFF.md`, before planning or editing.
- Important cross-session information should be appended to `docs/PROJECT_MEMORY.md`.
- Before ending a substantial session, update `docs/PROJECT_MEMORY.md`; add stable decisions to `docs/DECISIONS.md`.
- `HANDOFF.md` should stay concise and only contain immediate onboarding context.

### Art Lane Boundaries

- Art lane owns `assets/` and `docs/ASSET_PIPELINE.md`.
- Art lane should not edit `game/scripts/**` unless the change only wires already-agreed asset paths or the user explicitly coordinates with the code lane.
- Contest art must be original, AI-generated with commercial-use rights, CC0, or clearly licensed.
- Minecraft and Plants vs. Zombies assets are private placeholders only and must not be included in public contest submissions.

### Art Specs

- The contest demo art target is `32x32` character sprites.
- Base tiles use `16x16`; larger props may use `32x32`, `32x48`, or tile combinations.
- The first art batch prioritizes Chen Xing, Qimian, normal zombie, blood moon zombie, core demo locations, and basic resource/status UI icons.
- Every external or AI-generated asset that enters `assets/sprites/` needs a row in `docs/ASSET_LICENSE_LOG.md`.

### Code Lane Boundary

- Code-lane conversations own Godot implementation files under `game/scripts/**`, `game/tests/**`, and `game/scenes/**`.
- Code-lane work may read design and contest docs for context, but should not alter narrative/design sections unless the user explicitly asks or cross-lane coordination requires it.
- Before claiming code work is complete, run the Godot simulation test listed in `HANDOFF.md`.

### Repository

- Use `https://github.com/T3L000/BeyondSafeZone.git` as the canonical GitHub repository for this project.
- The local working branch should be `main` unless the user requests another branch.

### Demo Pressure Mechanics

- The contest demo should use a deterministic 14-day pressure table rather than procedural randomness.
- This War of Mine-like influence means scarcity, day/night tradeoffs, scavenging pressure, shelter survival, and hidden-world clues; it does not mean copying combat, art, UI, or exact systems.
- Day-start pressure applies once per day and should not stack from UI refreshes or repeated calls.
- Ending states for the 14-day demo are `survived_demo`, `barely_survived`, and `collapsed`.

### Repository Hygiene

- Do not commit `.superpowers/`, `.godot/`, generated builds, temporary files, or logs.
