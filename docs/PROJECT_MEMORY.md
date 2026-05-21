# Project Memory

This file is the long-term memory for BeyondSafeZone conversations. Append important information here before ending a session. Keep `HANDOFF.md` short; put durable details here.

## 2026-05-21

### Contest Line Setup

- The contest lane will prepare a unified submission material package before drafting isolated assets.
- Primary contest files live in `marketing/`.
- Contest wording must stay aligned with the current demo: 14-day Chen Xing greybox demo, day 7 and day 14 blood moons, Qimian hidden AI action log revealed at demo end.
- The project is currently not a Git repository in `E:\Download\working\BeyondSafeZone`, so sessions should use explicit file notes instead of relying on git history.

### Cross-Conversation Memory Rule

- `AGENTS.md` is the root-level instruction file for future agents.
- `HANDOFF.md` remains the immediate entry point for new sessions.
- `docs/PROJECT_MEMORY.md` stores dated detailed memory.
- `docs/DECISIONS.md` stores stable decisions.
- Lane-owned files store active work products, such as `marketing/SUBMISSION_PLAN.md`.

### Recommended Session Ending Checklist

- Append important discoveries and completed work to this file.
- Update `HANDOFF.md` if the next session needs to know something immediately.
- Update `docs/DECISIONS.md` for stable decisions.
- Update the active lane's material or plan file.

### Code Line Handoff Intake

- A code-lane session read `AGENTS.md`, `HANDOFF.md`, `README.md`, `docs/DEMO_SCOPE.md`, `docs/策划案.md`, `docs/ASSET_PIPELINE.md`, `marketing/DEMO_PITCH.md`, `game/scripts/core/game_simulation.gd`, `game/scripts/main.gd`, `game/tests/test_game_simulation.gd`, and `game/scenes/main.tscn`.
- Active code-lane ownership remains `game/scripts/**`, `game/tests/**`, and `game/scenes/**`; design narrative files should stay reference-only unless explicitly coordinated.
- Baseline simulation verification was run with `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd` and printed `All simulation tests passed.`
- Current code implementation is a greybox 14-day Chen Xing demo skeleton with day/night loop, location exploration, shelter actions, resource consumption, bicycle range limits, day 7/day 14 blood moon resolution, Qimian hidden actions, shared-map changes, and demo-end Qimian log reveal.
- Useful next code tasks remain: deterministic 14-day event table, clearer greybox UI for phase/blood moon/Qimian clues, and a simple day-14 survival/failure ending state.

### Design Line Intake

- User explicitly instructed future work to read `AGENTS.md` and `HANDOFF.md` first and obey the cross-conversation memory rules inside them.
- Current active context is the design/lore handoff unless the user redirects lanes.
- For design/lore work, avoid editing `game/scripts/**`; mirror mechanics-affecting narrative changes into `docs/DEMO_SCOPE.md` before code implements them.
- User chose Chen Xing's opening as waking at home rather than in a hospital. Design docs now treat home as the opening/shelter location, with hospital/clinic content retained as early exploration and medical clue material.
- User chose option 3 for Qijin's organization: the Rebirth Project is a hybrid organization with a scientific outer shell and religious/internal doctrine. This should guide Qijin's guilt, the organization's language, and the White Day Protocol framing.
- User requested batching setting changes and writing them after 5 accumulated decisions. This batch was written: Chen Xing, Qimian, and Qijin are all male; Chen Xing and Qimian are old friends who were close in childhood but drifted apart as adults; the three boys made a childhood "apocalypse shelter plan"; Qimian does not immediately think of Chen Xing after waking because the old-friend relationship has faded; Qimian first recalls his infection experience, confirms his changed body, then chooses finding Qijin as his main task before gradually discovering many people need help.

### Art Line Handoff

- User asked this session to read `AGENTS.md` and `HANDOFF.md` first and obey cross-conversation memory rules.
- Active lane for this conversation is the art/asset lane unless the user redirects.
- Art lane primary ownership is `assets/` and `docs/ASSET_PIPELINE.md`; avoid editing `game/scripts/**` unless adding already-agreed asset paths.
- Current asset folders exist as `assets/source/` and `assets/sprites/`, but no actual art files are present yet.
- Current visual direction is 2D pixel art with a small, unified first pass rather than a large polished asset set.
- Earlier intake noted character size as open between `32x32` and `48x48`, and tile size as open between `16x16` and `32x32`; this is superseded by the later Art Line Spec Lock below.
- First-pass asset candidates are Chen Xing, Qimian, normal zombie, blood moon zombie, and core shelter/location art.
- FrameRonin is intended for video-to-frames, background removal, sprite sheet organization, GIF previews, and temporary pixel asset generation.
- Before external or AI-generated art is used in the contest build, the project needs an asset license/source log.
- Minecraft and Plants vs. Zombies assets may only be private placeholders and must not ship in the public contest submission.
- This workspace currently has no `.git` repository, so session handoff depends on explicit file notes rather than git commits.

### Art Line Next Steps

- Lock one character sprite size for the contest demo.
- Create a minimal placeholder sprite plan covering Chen Xing, Qimian, normal zombie, blood moon zombie, shelter, and the most visible locations.
- Extend `docs/ASSET_PIPELINE.md` with concrete naming, folder, sprite sheet, and license-log rules before producing or importing art.

### Art Line Spec Lock

- User asked to lock pixel specs, first asset batch, FrameRonin workflow, and license tracking, and requested the browser visual companion.
- Browser visual companion server started at `http://localhost:53911`, but the in-app browser blocked both localhost and file preview URLs by policy, so the visual preview could not be shown in-browser.
- A standalone visual preview file was created at `.superpowers/brainstorm/art-assets-20260521/art-spec-lock-standalone.html` for reference, but it was not opened in the in-app browser because file URLs are blocked.
- The art pipeline now locks the contest demo to `32x32` character sprites, `16x16` base tiles, and larger props as `32x32`, `32x48`, or tile combinations.
- `docs/ASSET_LICENSE_LOG.md` was created as the required license/source log for external, AI-generated, and modified assets.
- Next art task is to produce or source the first Must Have assets listed in `docs/ASSET_PIPELINE.md`, beginning with Chen Xing, Qimian, normal zombie, and blood moon zombie.

### Repository Setup

- The local workspace `E:\Download\working\BeyondSafeZone` was initialized as a git repository on branch `main`.
- Git remote `origin` is set to `https://github.com/T3L000/BeyondSafeZone.git` for both fetch and push.
- `git ls-remote https://github.com/T3L000/BeyondSafeZone.git` returned no refs during setup, so the remote appears empty or has no published branches yet.
- Current files are untracked after initialization; no initial commit or push has been made yet.

### Code Line: This War of Mine-Like Pressure Slice

- Implemented the planned "日夜压力" code slice in the Godot greybox demo.
- `game/scripts/core/game_simulation.gd` now has a deterministic 14-day morning event table with pressure type, clue text, blood moon warnings, one-time day-start modifiers, location visit/depletion labels, deterministic exploration risk text, and day-14 ending states.
- `game/scripts/main.gd` now surfaces a "今日态势" panel, translated phase labels, blood moon warnings, clearer disabled-button reasons, Qimian clue separation, and final ending labels.
- `game/tests/test_game_simulation.gd` now covers the event table, morning context, blood moon warnings, one-time pressure application, exploration visit/risk behavior, Qimian reveal requirements, day-14 ending assignment, and collapse edge case.
- Verification run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd` printed `All simulation tests passed.`
- Headless project load run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --quit-after 1` exited successfully.

### Repository Hygiene

- Added `.gitignore` to keep local Godot caches, local Superpowers/browser-assistant state, generated builds, temporary files, and logs out of source control.
- `.superpowers/` is a local planning/browser artifact and should not be committed.
- Created the root initial commit `e2c811e` with message `feat: add Beyond Safe Zone demo foundation`.
