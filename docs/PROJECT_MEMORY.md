# Project Memory

This file is the long-term memory for BeyondSafeZone conversations. Append important information here before ending a session. Keep `HANDOFF.md` short; put durable details here.

## 2026-06-04

### Code Lane: C-008 Clinic Anonymous Medicine Feedback

- Continued the single-task loop after C-007 and selected `C-008 诊所反馈` only.
- Short spec used for C-008:
  - Trigger: `Day >= 5`, night resolution, `clinic` has a `help` player mark.
  - Player action: enter clinic, click `留下求助`, then click `夜晚结算`.
  - State change: Qimian responds to the clinic help mark by leaving anonymous medicine in the shared clinic state, marking the clinic as changed by Qimian, and writing an anomaly dossier entry.
  - Visible feedback: next morning/night HUD log shows anonymous medicine and a shallow arrow beside the help mark; location card/dossier text can expose the changed state.
  - Verification: Unity EditMode red-green test, full regression, and `OneRunMain` Play path.
- Implementation details:
  - `Assets/Scripts/Controllers/QimianController.cs` now has a minimal clinic help-mark response path.
  - The response adds `+1` to `state.Locations["clinic"].Resources["meds"]`.
  - The clinic location gets `QimianTrace = true` and a `qimian` icon.
  - `state.AnomalyDossier` receives a clinic entry describing anonymous medicine and a shallow arrow next to Lin Xing's help mark.
  - `state.Qimian.PublicClues` and `state.Qimian.Log` receive the public/reveal-facing response text.
- Verification evidence:
  - `TestClinicHelpMarkCreatesAnonymousMedicineFeedback` failed first with `0/1 passed`, jobId `c3d25c58`.
  - After implementation, the same test passed with `1/1 passed`, jobId `39b33337`.
  - Final Unity EditMode full regression passed: `BeyondSafeZone.Tests.TestGameSimulation` `39/39`, jobId `880a4edd`.
  - `OneRunMain` Play verification: clicked `下一天` to Day 5, `去诊所`, `留下求助`, `夜晚结算`; HUD Log showed `社区诊所出现匿名药品：求助标记旁边多了一条浅箭头，像是有人读懂后留下的回应。`; Header advanced to Day 6; Console had `warnings: 0`, `errors: 0`.
- Updated docs:
  - `docs/UNITY_MIGRATION_STATUS.md` has a dedicated C-008 execution record.
  - `docs/CROSS_LANE_LOG.md` has a Code Lane entry for C-008.
- Boundary:
  - C-008 is complete as a minimal greybox text/shared-state feedback loop.
  - No formal pixel medicine-pack asset or separate dossier UI panel was created.
  - Next code task should be `C-009`: ending log explaining Qimian personality card, inputs, action ordering, final choice, world impact, and the clinic mark causal chain.
  - No new stable decision was made; `docs/DECISIONS.md` was not updated.

### Code Lane: C-007 Qimian Reads Clinic Help Mark

- Continued the code-lane single-task loop for `C-007 祁眠读取诊所 help 标记`.
- Startup protocol was followed in this lane: read `AGENTS.md`, `HANDOFF.md`, `docs/CROSS_LANE_LOG.md`, `README.md`, `docs/UNITY_MIGRATION_PLAN.md`, `docs/UNITY_MIGRATION_STATUS.md`, `docs/MINIMUM_DEMO_SCOPE.md`, `docs/开发任务拆解.md`, and relevant Unity files under `E:\Download\working\BeyondSafeZoneUnity`.
- Short spec used for C-007:
  - Trigger: `Day >= 5`, night resolution, `clinic` has a `help` player mark.
  - Player action: enter clinic, click `留下求助`, then click `夜晚结算`.
  - State change: `QimianController` reads the clinic `help` mark and records it in Qimian AI replay/log.
  - Visible feedback: night HUD log shows the mark was noticed.
  - Verification: Unity EditMode tests plus `OneRunMain` Play path.
- Implementation details:
  - `Assets/Scripts/Controllers/QimianController.cs` now records perceived `help` marks in `state.Qimian.Log` and also appends the public clue to `state.Qimian.PublicClues`.
  - `Assets/Scripts/Core/GameSimulation.cs` now records the public clue count before night resolution and appends only newly added public clues that were not already displayed by `NightController.Resolve()`.
  - This also fixes a Day 5 duplicate-visible-clue issue where the fixed Qimian wake clue could appear both under `昨夜` and `昨夜线索`.
  - `Assets/Tests/TestGameSimulation.cs` now includes tests for Qimian reading the clinic help mark, HUD-visible night result, and non-duplicated public clues.
- Verification evidence:
  - Earlier C-007 log-read TDD: `TestQimianReadsClinicHelpMarkOnWakeNight` failed first with `0/1 passed`, jobId `0f1fd5fb`, then passed with `1/1 passed`, jobId `2f745a1c`.
  - Earlier full regression after first C-007 slice: `36/36 passed`, jobId `bb9999ab`.
  - HUD visible feedback TDD: `TestNightResultShowsQimianReadClinicHelpMark` failed first with `0/1 passed`, jobId `a7bbb122`, then passed with `1/1 passed`, jobId `793dfd6c`.
  - Duplicate clue TDD: `TestNightResultDoesNotDuplicateExistingQimianPublicClue` failed first with `0/1 passed`, jobId `5a2a2d8e`, then passed with `1/1 passed`, jobId `900cc0f8`.
  - Final Unity EditMode full regression: `BeyondSafeZone.Tests.TestGameSimulation` passed `38/38`, jobId `f6bef90c`.
  - Final `OneRunMain` Play verification: clicked `下一天` to Day 5, `去诊所`, `留下求助`, `夜晚结算`; HUD Log showed `昨夜线索：社区诊所附近的求助标记被人轻轻描深了一笔。`; Console had `warnings: 0`, `errors: 0`.
- Updated docs:
  - `docs/UNITY_MIGRATION_STATUS.md` has a dedicated C-007 execution record.
  - `docs/CROSS_LANE_LOG.md` has a Code Lane entry for C-007.
- Boundary:
  - C-007 is complete as a read-and-visible-feedback loop.
  - `C-008` is still open: anonymous medicine / next-day map or dossier feedback has not been implemented.
  - No new stable decision was made; `docs/DECISIONS.md` was not updated.

### Code Lane: Unity OneRunMain Formal First-Run Scene

- Continued the full first-run Unity implementation plan after the user asked to continue previous work.
- Startup protocol followed: read `AGENTS.md`, `HANDOFF.md`, `docs/CROSS_LANE_LOG.md`, `docs/PROJECT_MEMORY.md`, `docs/DECISIONS.md`, and relevant UnitySkills guidance.
- Unity environment verified through UnitySkills:
  - Project: `E:\Download\working\BeyondSafeZoneUnity`
  - Unity: `2022.3.62f3c1`
  - UnitySkills: `2.0.1`
  - Mode: `bypass`
  - Formal scene: `Assets/Scenes/OneRunMain.unity`
- Stable decision added: `Assets/Scenes/OneRunMain.unity` is now the formal first-run Lin Xing scene. `Assets/Scenes/MainPrototype.unity` remains reference-only and should not be expanded as the main scene.
- Current Unity scene behavior:
  - `OneRunMain` has a single `OneRunBootstrap` object with `OneRunGameController`.
  - At runtime it generates `WalkableShelterGreybox`, `LinXing_Player`, six `Facility_*` interactables, and `OneRunHUD`.
  - The HUD can enter clinic, supermarket, and garage scavenging greyboxes.
  - Entering clinic creates `ScavengeGreybox_clinic` with `SearchPoint_waiting`, `SearchPoint_exam_a`, and `SearchPoint_pharmacy`, and hides the shelter root.
- Added first-run AI-readable player action entry:
  - `OneRunGameController` now creates `OneRunHUD/LeaveHelpMark`.
  - New public method `LeaveHelpMarkAtActiveLocation()` writes a `help` mark through `GameSimulation.AddPlayerMark(State, locationId, "help", ...)`.
  - Play verification showed HUD log feedback: `林行在社区诊所留下求助标记。`
- Test work:
  - Added `TestOneRunControllerExposesHelpMarkAction` to `Assets/Tests/TestGameSimulation.cs`.
  - Red-green record: after asset refresh, the test first failed because `LeaveHelpMarkAtActiveLocation` was missing; after implementation it passed.
  - Full Unity EditMode regression passed: `BeyondSafeZone.Tests.TestGameSimulation` `35/35 passed`, jobId `2c7a6f63`.
- Play verification:
  - `ExploreClinic` `Button.onClick` invoked successfully.
  - `LeaveHelpMark` `Button.onClick` invoked successfully.
  - Runtime hierarchy contained `ScavengeGreybox_clinic` and the three clinic search points.
  - Unity Console stats after verification: `warnings: 0`, `errors: 0`.
- Updated docs:
  - `docs/UNITY_MIGRATION_STATUS.md` now records the formal scene, help-mark chain, verification evidence, and next blocker.
  - `HANDOFF.md` now points future code work to `OneRunMain` instead of `MainPrototype`.
  - `docs/DECISIONS.md` now records `OneRunMain` as the formal Unity main scene.
- Remaining gap:
  - Search-point `E` key proximity interaction has not yet been manually/automated verified end-to-end.
  - Next P0 task is the AI feedback chain: Day 5+ Qimian reads the clinic `help` mark, then the player sees anonymous medicine / response traces the next day, and the ending log explains the input and decision.

### Design Lane: Delivery Discipline Docs Landed

- User asked to turn three abstract principles into enforceable project rules: structured docs drive implementation, `Plan -> Build -> Test -> Refine` runs in small loops, and verifiable interaction chains matter more than feature sprawl.
- This session read existing memory first, then treated the task as a design/process documentation pass.
- Wrote the execution rules directly into active docs instead of creating a separate floating process memo:
  - `docs/MINIMUM_DEMO_SCOPE.md` now includes one-task-at-a-time discipline, required 6-field feature spec, three-part completion definition, and a fixed five-question pre-implementation check.
  - `docs/开发任务拆解.md` now includes the single-task execution rule, a reusable per-task template, explicit mapping from P0 interaction chains to `T-xxx` regressions, and a fixed five-step task rhythm.
  - `docs/UNITY_MIGRATION_STATUS.md` now includes Unity execution rhythm requirements, completion criteria, and a reusable task execution record template for future verification notes.
- Stable process rule locked: near-term P0 work should be measured by verified interaction chains rather than feature count; each work cycle should cover one task number or one explicit chain only.
- Cross-lane impact:
  - Code lane should stop batching multiple P0 tasks together without separate verification notes.
  - Design lane should define features in short structured form before asking for implementation.
  - Contest lane can report progress in terms of verified playable chains instead of vague system totals.
- No gameplay code, Unity scene content, or contest copy changed in this pass; the work was documentation and durable process alignment only.

### Unity MainPrototype ChineseTMP Fix

- Continued the Unity `MainPrototype` setup after the user asked for the next step.
- Followed the project handoff protocol by reading `HANDOFF.md`, `docs/CROSS_LANE_LOG.md`, and `docs/PROJECT_MEMORY.md` before mutating Unity state.
- Root cause of the visible Chinese glyph issue was verified in Unity Console and asset YAML: `Assets/Fonts/ChineseTMP.asset` contained embedded texture `ChineseTMP Atlas` with `m_IsReadable: 0`, so TMP could not dynamically add missing characters.
- Confirmed TextMeshPro's own editor package code uses `SerializedObject(texture).FindProperty("m_IsReadable").boolValue = true` for atlas readability.
- Created Unity editor utility `Assets/Editor/ChineseTmpAtlasReadableFixer.cs` with menu item `Tools/Beyond Safe Zone/Fix ChineseTMP Atlas Readable`, targeting only `Assets/Fonts/ChineseTMP.asset`.
- Ran the menu through UnitySkills `editor_execute_menu`; verified `Assets/Fonts/ChineseTMP.asset` now has `m_Name: ChineseTMP Atlas` followed by `m_IsReadable: 1`.
- Verified the utility script compiles through `script_get_compile_feedback` with `hasErrors: false` and `errorCount: 0`.
- Cleared Console, saved `Assets/Scenes/MainPrototype.unity` through `scene_save`, entered Play through `editor_play`, and checked Console after initialization: `warnings: 0`, `errors: 0`, with no new TMP missing-glyph warnings.
- After `editor_stop`, Unity reported one non-blocking warning: `NativeFormatImporter generated inconsistent result for asset ... Assets/Fonts/ChineseTMP.asset`. Current status is not blocked, but if it repeats after future font edits, inspect font asset import stability.
- Next Unity prototype step is gameplay flow testing: from Day 1 explore core locations, advance to Day 5+ Qimian wake/anomaly behavior, then Day 6 marker/night feedback chain.

## 2026-05-21

### Contest Line Setup

- The contest lane will prepare a unified submission material package before drafting isolated assets.
- Primary contest files live in `marketing/`.
- Contest wording must stay aligned with the current demo: 14-day Lin Xing greybox demo, day 7 and day 14 blood moons, Qimian hidden AI action log revealed at demo end.
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
- Current code implementation is a greybox 14-day Lin Xing demo skeleton with day/night loop, location exploration, shelter actions, resource consumption, bicycle range limits, day 7/day 14 blood moon resolution, Qimian hidden actions, shared-map changes, and demo-end Qimian log reveal.
- Useful next code tasks remain: deterministic 14-day event table, clearer greybox UI for phase/blood moon/Qimian clues, and a simple day-14 survival/failure ending state.

### Design Line Intake

- User explicitly instructed future work to read `AGENTS.md` and `HANDOFF.md` first and obey the cross-conversation memory rules inside them.
- Current active context is the design/lore handoff unless the user redirects lanes.
- For design/lore work, avoid editing `game/scripts/**`; mirror mechanics-affecting narrative changes into `docs/DEMO_SCOPE.md` before code implements them.
- User renamed the first playable protagonist from `陈醒` / `Chen Xing` to `林行` / `Lin Xing`. Design docs, marketing copy, asset docs, memory, and decisions were updated. Code still contains old display strings and should be synchronized in a code-lane pass.
- User chose Lin Xing's opening as waking at home rather than in a hospital. Design docs now treat home as the opening/shelter location, with hospital/clinic content retained as early exploration and medical clue material.
- User chose option 3 for Qijin's organization: the Rebirth Project is a hybrid organization with a scientific outer shell and religious/internal doctrine. This should guide Qijin's guilt, the organization's language, and the White Day Protocol framing.
- User requested batching setting changes and writing them after 5 accumulated decisions. This batch was written: Lin Xing, Qimian, and Qijin are all male; Lin Xing and Qimian are old friends who were close in childhood but drifted apart as adults; the three boys made a childhood "apocalypse shelter plan"; Qimian does not immediately think of Lin Xing after waking because the old-friend relationship has faded; Qimian first recalls his infection experience, confirms his changed body, then chooses finding Qijin as his main task before gradually discovering many people need help.
- User approved the next 5-decision batch: the current game structure is a complete 14-day survival cycle rather than a 30-day structure; Qimian wakes on day 5; day 14 is an escape-pressure blood moon caused by a failing shelter plus a short safe-zone opening, with Rebirth Project cleanup pressure hinted in the distance; Lin Xing and Qimian can pass very close without recognizing each other as Lin Xing moves toward the safe zone and Qimian hides inside a zombie group; Qimian changes the zombie group's route for his own goals, not for Lin Xing, but this indirectly saves Lin Xing.
- Code may still need synchronization with the latest design: older implementation notes and Godot code may still use day-11 Qimian timing and a simpler day-14 blood moon resolution.
- User approved another 5-decision batch: day-14 departure is both forced by a failing shelter and enabled by a short safe-zone window; the window publicly means temporary intake for outer-ring survivors but secretly exists because Rebirth Project cleanup pressure is forcing the safe zone to contract; safe-zone intake is centered on infection screening; Lin Xing reaches evacuation readiness by confirming the safe zone still exists through radio, finding its address through map/checkpoint clues, and repairing the bicycle enough to travel; the day-14 endpoint is Lin Xing reaching the safe-zone gate and waiting for infection screening, not already entering true safety.
- User approved the next ending/log batch: Lin Xing passes initial screening but is placed under quarantine observation; the ending uses a double reveal where Lin Xing only hears a small anomaly clue while the player unlocks the full Qimian log; the day-14 scene tone is Lin Xing reaching the screening shed while Qimian hides inside a zombie group and changes its route, with neither recognizing the other; the Qimian log should show AI integration through "AI action replay + Qimian subjective fragments"; Qimian's opening personality card should deterministically define his AI decision rules rather than acting as probability weights or random daily choices.
- User approved the next Qimian AI boundary batch: Qimian AI input is limited to Qimian-perceivable state, not Lin Xing or other survivors' hidden backend state; Qimian can make limited in-world inferences from traces but cannot identify Lin Xing directly; the personality card is hidden during the first run and revealed in the post-demo log; long-term design can generate personality cards per run, but the current Demo uses one fixed default card for stability; the default card is finding Qijin as the main goal, cautious/avoids exposure, helps nearby people without taking on mass rescue, takes only resources needed for the task, and wants to observe the safe zone while distrusting screening.
- User approved the first Lin Xing management-experience batch: Lin Xing's management loop should reference This War of Mine's pressure structure without copying its systems; the Demo lightly includes resource scarcity/base maintenance, survivor moral choices, and dangerous scavenging; shelter facilities are locked to five core facilities (bed, workbench, window barricade, radio, storage/organizing table); these facilities respectively handle fatigue/stress, bike/tools/material work, blood moon defense and day-14 escape losses, safe-zone/blood-moon/Rebirth broadcasts, and preservation/carrying for day-14; core resources are food, water, medicine, materials, parts, and fuel, where fuel can serve vehicle/generator needs but generator use creates noise risk.
- User approved the first exploration-gameplay batch: exploration locations should become small top-down stealth levels rather than result-only menus; room reading can reference Hotline Miami's top-down layout style; unlit rooms have no pre-entry vision, windowed rooms with no rain reveal partial information, and rain weakens/blocks window vision; rooms can hide zombies; Lin Xing can counterattack only in limited, costly ways, with recommended play being hiding, routing around, closing doors, or making noise to lure zombies; main exploration punishments are injury/infection plus time/fatigue, where staying too long leads to nightfall and fatigue; location goals are "take enough and leave" rather than mandatory objective completion.
- User approved the map/search visual reference batch: the overworld should use a node-based pixel map with location nodes, route limits, resource tendencies, danger levels, and small status icons; indoor search can borrow dark pixel-room lighting, furniture density, search-point mood, and darkness pressure from the second reference image, but final exploration remains top-down rather than 45-degree/isometric; location states show resource tendency plus danger level, with Qimian traces and safe-zone clues as small icons/question marks; routes have road conditions and travel time, affected by rain, blockage, and zombie migration, but there is no multi-route choice per destination for now.

### Art Line Handoff

- User asked this session to read `AGENTS.md` and `HANDOFF.md` first and obey cross-conversation memory rules.
- Active lane for this conversation is the art/asset lane unless the user redirects.
- Art lane primary ownership is `assets/` and `docs/ASSET_PIPELINE.md`; avoid editing `game/scripts/**` unless adding already-agreed asset paths.
- Current asset folders exist as `assets/source/` and `assets/sprites/`, but no actual art files are present yet.
- Current visual direction is 2D pixel art with a small, unified first pass rather than a large polished asset set.
- Earlier intake noted character size as open between `32x32` and `48x48`, and tile size as open between `16x16` and `32x32`; this is superseded by the later Art Line Spec Lock below.
- First-pass asset candidates are Lin Xing, Qimian, normal zombie, blood moon zombie, and core shelter/location art.
- FrameRonin is intended for video-to-frames, background removal, sprite sheet organization, GIF previews, and temporary pixel asset generation.
- Before external or AI-generated art is used in the contest build, the project needs an asset license/source log.
- Minecraft and Plants vs. Zombies assets may only be private placeholders and must not ship in the public contest submission.
- This workspace currently has no `.git` repository, so session handoff depends on explicit file notes rather than git commits.

### Art Line Next Steps

- Lock one character sprite size for the contest demo.
- Create a minimal placeholder sprite plan covering Lin Xing, Qimian, normal zombie, blood moon zombie, shelter, and the most visible locations.
- Extend `docs/ASSET_PIPELINE.md` with concrete naming, folder, sprite sheet, and license-log rules before producing or importing art.

### Art Line Spec Lock

- User asked to lock pixel specs, first asset batch, FrameRonin workflow, and license tracking, and requested the browser visual companion.
- Browser visual companion server started at `http://localhost:53911`, but the in-app browser blocked both localhost and file preview URLs by policy, so the visual preview could not be shown in-browser.
- A standalone visual preview file was created at `.superpowers/brainstorm/art-assets-20260521/art-spec-lock-standalone.html` for reference, but it was not opened in the in-app browser because file URLs are blocked.
- The art pipeline now locks the contest demo to `32x32` character sprites, `16x16` base tiles, and larger props as `32x32`, `32x48`, or tile combinations.
- `docs/ASSET_LICENSE_LOG.md` was created as the required license/source log for external, AI-generated, and modified assets.
- Next art task is to produce or source the first Must Have assets listed in `docs/ASSET_PIPELINE.md`, beginning with Lin Xing, Qimian, normal zombie, and blood moon zombie.

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

### Art Line Intake: Current Session

- This art-lane session read `AGENTS.md`, `HANDOFF.md`, `README.md`, `docs/ASSET_PIPELINE.md`, `docs/ASSET_LICENSE_LOG.md`, `docs/PROJECT_MEMORY.md`, `docs/DECISIONS.md`, `docs/DEMO_SCOPE.md`, and `docs/策划案.md` before planning or editing.
- Active ownership for this session is `assets/` and `docs/ASSET_PIPELINE.md`; gameplay code under `game/scripts/**` remains out of scope unless the user explicitly coordinates asset-path wiring.
- Confirmed current locked art specs: `32x32` character sprites, `16x16` base tiles, larger props as `32x32`, `32x48`, `48x32`, or tile combinations.
- Confirmed `assets/source/` and `assets/sprites/` exist, but no actual asset files are present yet.
- Confirmed every external or AI-generated asset entering `assets/sprites/` must first be recorded in `docs/ASSET_LICENSE_LOG.md` with source/tool, license or rights status, prompt summary if AI-generated, modification status, and contest status.
- No new stable art decision was made in this intake pass, so `docs/DECISIONS.md` was left unchanged.
- Next art-lane work should begin with the Must Have batch: Lin Xing, Qimian, normal zombie, blood moon zombie, core demo locations, and resource/status UI icons.

### Art Line: Lin Xing Prompt Direction

- User asked to use FrameRonin `nanobanana预设像素角色生成器V2-V3` with V3, generating one target at a time, starting with Lin Xing.
- Lin Xing's requested visual reference is "兵长"; for contest-safe originality, the prompt should translate that into compact build, short black hair, sharp eyes, stern expression, disciplined survivor posture, and clean silhouette, while explicitly avoiding direct anime-character replication, uniforms, insignia, cloaks, weapons, and copyrighted details.
- The first Lin Xing generation prompt should target a single `32x32` pixel-art survivor sprite sheet suitable for later FrameRonin/Sprite Sheet cleanup.
- User rejected the first Lin Xing prompt result as ugly and not close enough to the "兵长" reference. Next prompt should push the recognizable archetype cues harder: short black undercut-like layered hair, center-part curtain bangs, half-lidded narrow grey eyes, pale tired face, compact agile build, deadpan expression, white scarf/cravat-like neck cloth adapted into a survivor outfit, and a cleaner disciplined silhouette, while still avoiding exact copyrighted uniform, insignia, cloak, swords, or direct replica wording.
- User reviewed the next Lin Xing image and said it is only average. The face is now too close to the "兵长" reference, while the body reads too chibi/uniform-like. Next iteration should first lock Lin Xing's original silhouette: a wrapped robe/poncho-like blue-gray survivor cloak, less anime-identical face, smaller head-to-body ratio, side-swept or messy short hair instead of center-part bangs, sharper but original eyes, and a more post-apocalyptic ordinary-survivor identity before expanding to actions.
- User rejected the robe iteration because the head is still too large, the face looks ugly and corpse-like, and the character reads as sick/dead rather than capable. Next Lin Xing prompt must strongly specify adult non-chibi proportions, a much smaller head, warmer living skin tone, alert eyes without heavy eye bags, cleaner robe silhouette, and "alive, capable survivor" energy.
- User clarified that the current main Lin Xing problem remains the oversized head and face. The robe silhouette is closer, but next prompts should suppress facial detail instead of trying to beautify it: use adult 6-head-tall/non-chibi proportions, head no more than 15-18% of full body height for the concept pass, tiny simplified pixel face with only minimal eye/mouth marks, no portrait-level facial rendering, and ideally restart from text-only if image-to-image keeps anchoring the large head.
- User said the latest Lin Xing pass is acceptable in proportions, but it still feels too AI-generated and the clothing looks too modern. Keep the smaller-head adult proportions, but next prompt should reduce polished AI styling: handmade low-detail pixel art, slightly imperfect asymmetry, flatter limited palette, less smooth shading, fewer random straps/pouches, and less tactical/modern clothing. Clothing direction should shift toward old canvas, patched rain cloth, cotton wrap/robe, and civilian improvised survivor layers rather than a modern jacket or tactical outfit.
- User asked for multiple fresh-start Lin Xing prompts instead of continuing from image references. Future prompt attempts should be text-only, one target at a time, and avoid reusing previous generated images that anchor oversized heads, corpse-like faces, or polished modern tactical clothing.
- User accepted the latest Lin Xing generated sprite sheet as "temporarily this one". The useful locked direction is a small-head adult pixel survivor with short black hair, muted blue-gray wrapped robe/poncho, dark clothes, boots, and small backpack/pack silhouette. The sheet includes extra weapon/shield/fantasy or kneeling frames that should not be treated as Lin Xing's required contest actions; cleanup should keep/derive only idle, four-direction walk, search, injured, and bicycle-related frames.
- User asked for several character references because the current Lin Xing generations still look too ugly. Next art step should stop prompt-grinding and define a reference board first, separating face, body proportion, clothing silhouette, and mood references so the generator does not collapse everything into an overdesigned large-head survivor.
- User showed the second Lin Xing result and judged it only average. Visual issue: the result captured some face/hair cues but became a large clean chibi anime figure with oversized head, smooth illustration shading, too-polished coat, and fake checkerboard/watermark-like presentation. Next prompt should force a true tiny RPG sprite: actual `32x32` frame, no enlarged portrait, 2.5-head-tall compact adult proportions, head around one-third of body height, simpler survival jacket, dirt/wear, stronger pixel block readability, transparent or plain background, and no watermark.

### Contest Schedule Update

- User provided updated Tencent Cloud Hackathon schedule and links for the active submission sprint.
- Treat `2026-06-20` as the hard deadline for both registration and work submission.
- CodeBuddy Credits are released every Friday.
- Available challenge directions are: 小红花游戏, 文化表达类游戏, and 叙事类游戏.
- Recommended/default direction for BeyondSafeZone is `叙事类游戏：用 AI 重塑叙事体验`, because the project's strongest mechanic is Qimian as an AI-controlled hidden protagonist shaping a dynamic narrative.
- Important links: contest page `https://tch.cloud.tencent.com/contest/40`, CodeBuddy `https://www.codebuddy.cn/ide/`, submission form `https://wj.qq.com/s2/26396867/8ef8/`, registration form `https://wj.qq.com/s2/26331484/2e19/`, manual `https://img-bss.csdnimg.cn/bss/TencentCodeBuddyWorkshop/Tencent_Cloud_Hackathon_ZH.pdf`.
- The contest page was checked and identifies the event as `腾讯云黑客松 · 游戏开发挑战赛 2026`, organizer Tencent Cloud, status `报名中`, prize `¥100万元`; the manual PDF link could not be fetched in the current browser tool and should be opened manually if detailed rules are needed.

### Code Line Status Read: Next Implementation Entry

- A code status read confirmed the Godot project still passes current simulation tests and headless load, but code is behind the latest design decisions.
- Current code still uses old `陈醒` strings, old `chen` state keys, `batteries`/`intel` resources, hospital opening text, and Qimian waking on day 11.
- Latest docs/decisions now require Lin Xing (`林行`), home opening, Qimian waking on day 5, a 14-day complete survival cycle, day-14 escape-pressure blood moon, safe-zone gate/quarantine ending, fuel as a core resource, key clues as flags instead of stackable intel, and five shelter facilities.
- Recommended next code slice: synchronize the core simulation and tests to the latest design before adding new systems. Start with renaming display text/state labels to Lin Xing, updating Qimian day 5-14 hidden plan, replacing hospital opening with home/shelter opening, and rewriting day-14 reveal to safe-zone gate + quarantine + Qimian-in-zombie-group double reveal.
- After that sync, implement the management loop upgrades: fuel resource, key clue flags for radio/map/bike readiness, five shelter facilities, and day-14 evacuation readiness checks.

### Code Line: Lin Xing / Day-5 Qimian Sync

- Synchronized `game/scripts/core/game_simulation.gd`, `game/scripts/main.gd`, and `game/tests/test_game_simulation.gd` with the latest design direction.
- The playable state key is now `lin`, display text names Lin Xing/林行, and the opening is Lin Xing waking at home.
- Core resources now use `fuel`; old `batteries` and stackable `intel` were removed from the simulation model.
- Added `evacuation` flags for `safezone_confirmed`, `address_known`, and `bike_ready`; radio and bike repair now feed the day-14 gate outcome.
- Qimian now wakes on day 5 with a fixed default personality card, and Qimian log entries include `ai_replay` plus `subjective_fragment`.
- Day-14 endings now use `reached_gate_quarantine`, `barely_reached_gate`, and `collapsed`; the safe route reaches the safe-zone screening gate and enters quarantine observation while the player sees Qimian's hidden zombie-group route change.
- Current verification after this sync: simulation tests pass and headless Godot project load exits successfully.

### Code Line: Five Shelter Facilities Slice

- Implemented the five core shelter facilities in the Godot simulation: bed, workbench, window barricade, radio, and storage/organizing table.
- Added facility state under `state.shelter.facilities`, daily used markers, and `state.shelter.supply_preservation`.
- Added shelter actions: `rest_bed`, `workbench_repair`, `barricade_windows`, `radio_broadcast`, and `organize_storage`.
- Facility effects now connect to fatigue/stress recovery, bike readiness, blood moon defense, safe-zone broadcast/address flags, fuel spending/noise risk, and day-14 organized-supplies ending text.
- Updated the greybox UI to show facility status and expose the five facility actions.
- Current verification after this slice: simulation tests pass and headless Godot project load exits successfully.

### Code Line: Node Map and Evacuation Clue Slice

- Implemented the approved "节点式大地图 + 撤离线索可视化" code slice.
- `game/scripts/core/game_simulation.gd` now treats each location as a map node with `resource_tendency`, `danger_level`, `route_time`, `road_condition`, `icons`, and `qimian_trace`.
- Added `get_location_card_text(location_id)` so UI/tests can read one compact node-card string containing resources, danger, route time, road condition, icons, risk text, and Qimian traces.
- Exploration now deterministically applies road-condition fatigue pressure and can reveal evacuation flags from map nodes: police/subway/safe-zone-edge style nodes can set `evacuation.address_known`, and the safe-zone edge can confirm active intake.
- Qimian actions that affect a location now mark that node with `qimian_trace` and a `qimian` icon, making hidden-world changes visible on the overworld before the final log reveal.
- `game/scripts/main.gd` now labels the exploration area as a node map and uses location-card text for greybox buttons, including clearer too-far range text.
- Added simulation tests for node metadata, card text, evacuation clue discovery, deterministic road fatigue, and Qimian trace icons.
- Verification run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd` printed `All simulation tests passed.`
- Headless project load run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --quit-after 1` exited successfully.

### Code Line: Indoor Search Greybox Slice

- Implemented the first deterministic indoor search / stealth greybox layer.
- `game/scripts/core/game_simulation.gd` now gives locations room data with room names, visibility, search time, hidden zombie count, deterministic resources, and searched state.
- Added `state.exploration` with active location, time used, time limit, local noise, searched rooms, and lured rooms.
- Added public simulation methods: `enter_location(location_id)`, `get_room_card_text(room_id)`, `search_room(room_id, tactic)`, `lure_room(room_id)`, and `leave_exploration()`.
- Entering a node now starts a `searching` phase for the greybox UI. Searching a room collects deterministic resources; leaving advances to evening and marks the map node visited.
- Dark or rushed hidden-zombie rooms deterministically hurt Lin Xing and increase infection risk. Using `lure_room` first spends time and increases local noise but can avoid direct injury.
- Staying past the location time limit adds fatigue and reports time pressure when leaving.
- `game/scripts/main.gd` now uses `enter_location` from map buttons and shows room-search controls during the `searching` phase.
- Added simulation tests for room metadata, entering search phase, deterministic room pickup, hidden-zombie injury/infection, noise lure mitigation, and overstay fatigue.
- Verification run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd` printed `All simulation tests passed.`
- Headless project load run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --quit-after 1` exited successfully.

### Code Line: Infection and Medicine Loop Slice

- Implemented the "室内搜索调优 + 感染/药品闭环" code slice.
- Added `get_lin_condition_text()` so the simulation and UI can show Lin Xing's health, fatigue, stress, infection stage, and hope in one readable string.
- Infection risk now has readable stages: low risk, fever risk at `infection_risk >= 3`, and dangerous infection at `infection_risk >= 5`.
- Dangerous infection now creates deterministic night pressure: health loss, stress gain, and an infection warning in the night event text.
- Added the night shelter action `treat_wound`, spending one medicine to restore one health and reduce infection risk by one. Without medicine it only reports failure and does not change condition.
- Room cards now show dark-room risk and hidden-zombie lure state (`未引开` / `已引开`), making the search choices clearer.
- `game/scripts/main.gd` now shows Lin Xing's condition text in the main stats and indoor-search header, exposes both careful and quick search buttons, and adds the wound-treatment night action.
- Added simulation tests for condition text, infection stage thresholds, dangerous infection night consequences, wound treatment success/failure, and room-card lure/risk wording.
- Verification run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --script res://tests/test_game_simulation.gd` printed `All simulation tests passed.`
- Headless project load run: `Godot_v4.6.2-stable_mono_win64_console.exe --headless --path "E:\Download\working\BeyondSafeZone\game" --quit-after 1` exited successfully.

## 2026-05-24

### External Session History Intake

- User asked this session to read historical Codex session logs under `E:\a临时使用\codex保留\sessions` and inherit BeyondSafeZone setting-line context.
- This session re-read `AGENTS.md`, `HANDOFF.md`, `docs/策划案.md`, `docs/DEMO_SCOPE.md`, `marketing/DEMO_PITCH.md`, `docs/PROJECT_MEMORY.md`, and `docs/DECISIONS.md` before extracting session history.
- Relevant historical matches were found mainly in these external session files:
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T10-14-25-019e4850-09c4-76f2-a497-09840bf0be7d.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T11-28-39-019e4894-0186-7ce1-b2f5-67930d011de4.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T11-36-06-019e489a-d0f8-72a3-bc0e-fb33714f2eb0.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T11-36-31-019e489b-3580-7930-9629-023d3c0a9338.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T11-37-11-019e489b-d0a9-7583-aa8f-dbb00bdd2b61.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T11-55-55-019e48ac-e20b-74b3-a190-e7d88fe82a7c.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T15-30-23-019e4971-40ba-7ae0-95c9-824e2631ff8c.jsonl`
  - `E:\a临时使用\codex保留\sessions\2026 (2)\05\21\rollout-2026-05-21T16-07-02-019e4992-d854-7201-8488-3e25c830c260.jsonl`
- The external history did not reveal any setting decision that conflicts with the current local design docs. The current local canon remains the 14-day Lin Xing first-playable structure already recorded in `docs/DECISIONS.md`.
- The useful inheritance result for future design-lane sessions is that the current setting line is already consolidated locally: Lin Xing home opening, Qimian day-5 wake-up, Rebirth Project hybrid doctrine, day-14 safe-zone gate plus quarantine ending, and the post-run Qimian AI replay/log structure should all be treated as locked unless the user changes them.

### Design Line: Dual-Loop Revision

- User proposed pushing Lin Xing's exploration and pressure closer to `This War of Mine`, including room-scavenging tension, costly zombie combat, and stronger blood moon home defense.
- User also pointed out that Qimian does not fit a base-management loop well. This was accepted and formalized: Qimian should use a different gameplay identity, focused on hiding by day and performing covert night missions.
- Locked design direction: Lin Xing remains the resource-and-defense protagonist; Qimian becomes the covert intervention protagonist.
- Qimian's pressure should center on exposure risk, human suspicion, hiding spots, and infection-state instability rather than ordinary food/water/base upkeep.
- Locked fiction rule: Qimian can use zombie proximity as cover, but cannot openly reveal in front of human witnesses that zombies do not attack him.
- User suggested a "bridge occupied by zombies" scenario. This was kept as a strong representative Qimian mission, but revised so Qimian resolves it by covert luring and environmental manipulation rather than obvious frontal clearing.

### Design Line: AI Reveal Timing And Shared Map Clarification

- User questioned whether waiting until after the first run to reveal the AI layer would feel too slow. The resulting decision is yes: the player should feel a second actor in the world during the first run.
- Locked presentation rule: one run should reveal repeated anomalies and hidden-world traces from midgame onward, while the post-run log reveals identity, motives, and decision rules.
- Shared-map clarification for future code/design sessions: the intended contest-scope implementation is not full real-time co-presence. It is one shared set of nodes and world states, updated sequentially by Lin Xing's daytime actions and Qimian's night actions.
- This scope is considered sufficient to deliver the core promise that both protagonists act in the same world and indirectly reshape each other's routes, resources, and outcomes.
- User approved a 5-point contest-scope shared-map package:
  - same-location revisits are the main showcase rule
  - Qimian mostly affects Lin Xing through location-state changes rather than frequent direct encounters
  - the first stable shared state set is resources, lock/entry state, zombie density, route access, survivor presence, and anomaly traces
  - the first run should teach the player that a second actor changes the map before revealing identity
  - the first featured examples are supermarket revisit, clinic cabinet theft, rerouted bridge/road zombies, and anonymous medicine/tool drops

### Design Line: Combat, Infection, And Night Pressure

- User approved the combat direction of melee as the normal survival tool and firearms as rare, noisy, high-cost late tools.
- User accepted the infection direction as staged deterioration pressure rather than instant death. Design intent is that wounds can be treated, while infection can be suppressed or slowed but not lightly hand-waved away.
- User raised a possible redesign where late nights become more frequent and more oppressive over time instead of staying at a fixed blood-moon interval.
- That discussion is now resolved into a concrete structure: total cycle is `15` days, with the first true blood moon on day `7`, increasingly frequent `红潮夜` on days `11-14`, and the final true blood moon on day `15` night after Lin Xing has already begun daylight evacuation.
- Important wording note now locked: not every late oppressive night is a blood moon. The project uses two major blood moons and more frequent late `红潮夜`.

### Design Line: Qimian Vehicle And Hideout

- User changed the protagonist vehicle split: Lin Xing uses a bicycle; Qimian uses a motorcycle for his nightly route work.
- User locked Qimian's daytime hideout as a villa/cabin in the hills on the suburban edge.
- The accepted explanation for why Qimian does not return home combines practical and emotional reasons:
  - he does not want to expose his anomaly in a familiar human neighborhood
  - he does not feel able to bring his changed self back into the old life-space he once belonged to
- User also clarified Qimian's endgame behavior: he is not waiting all game for the final rescue moment. He follows a regular night-patrol/task rhythm and only shifts route toward the safe-zone outer line on the final night because he learns that the blood moon/red-tide pressure will be unusually severe.

## 2026-05-30

### 总体规划线 (Master Planning): 快速同步 + 游戏介绍输出

- **做了什么**: 读取全量 CROSS_LANE_LOG 和 PROJECT_MEMORY，汇总四条线自上次总体规划以来的全部变更；输出了以 AI 玩法为核心的分享式游戏介绍文案。
- **跨线同步汇总（5/27 → 5/30）**:
  - **代码线 7 轮**: 15天统一、汽车撤离系统、14地点40+房间数据、噪音模型、情境独白体系、Day15 完整撤离叙事弧、架构重构(1464→373行)、safe demo 室内搜索改造、祁眠 AI 决策引擎
  - **策划线 1 轮**: 五大核心决策（15天统一/祁烬广播呈现/汽车撤离/一二周目结构/祁眠第5天叙事保持）、15天逐日事件表、数值平衡表、4 个结构化数据文件（地点数据/祁眠AI伪代码/关卡布局/共享地图API）
  - **美术线 1 轮**: 38 个占位精灵、美术方向锁定（Levi/Keanu/PvZMC/TWoM）、素材路径就绪
  - **比赛线 1 轮**: 全部 7 个 marketing 文件对齐 Demo 真实状态、角色定位澄清（合规审查+提交辅助，非任务规划）
- **阻塞项变化**: 原 4 条全部 ✅ 已解决；余 3 条：比赛 15 天对齐、一周目 5 段分镜动画、二周目祁眠可操作——后两者属于完整版 scope
- **当前 Demo 核心实现状态**:
  - ✅ 15 天完整循环（15天逐日事件表落码）
  - ✅ 14 地点 40+ 房间（全套旗标系统：plan_found / safezone_hint / rebirth_clue / childhood_memory / car_found / qimian_file 等 15+）
  - ✅ 祁眠 AI 决策引擎（感知→收集→排序→执行 / 混合模式固定+动态 / 暴露值管理）
  - ✅ 汽车撤离系统（4 步修理流程：引擎→轮胎→电瓶→加油 / Day15 弃车徒步叙事弧）
  - ✅ 噪音传播模型 / 情境独白 / 三层结局 / 祁眠日志回放
  - ⏳ 侧视横版 Godot 实际场景未迁移（当前灰盒为文本+模拟）
  - ⏳ 一周目 5 段分镜动画未制作（需要 Godot 场景级工作）
  - ⏳ 二周目祁眠可操作未实现（完整版 scope）

## 2026-05-27

### 总体规划线 (Master Planning): 全文档梳理与需求分析

- **做了什么**: 完整阅读 docs/ 下 10 个文件 + marketing/ 下 7 个文件 + 3 个根目录文件的全部内容，生成《总体规划分析报告》,包含策划案核心内容总结、完整度/一致性问题分析、结构化开发需求概要、三条 Lane 启动提示词。
- **关键发现**:
  1. 天数不统一: "14 天"与"15 天"在多处混用,建议统一为 15 天
  2. 祁眠第 5 天醒来叙事负担过重,建议将身体变化认知分散到第 5-7 天
  3. 童年"末日避难计划"钩子缺少具体的非家触发地点
  4. 林行第 15 天撤离动机不足,缺少事件驱动
  5. 祁烬在 Demo 中的具体呈现方式未决策
  6. 策划案缺失: 逐日事件表、数值平衡表、对话大纲、结局条件(均为 P0-P1 紧迫)
- **输出文件**: `docs/总体规划分析报告.md`
- **对 cross-lane-sync skill 的确认**: Skill 已存在且设计完善,无需修改
- **下一步建议**:
  - 策划线优先: 15 天逐日事件表 + 数值平衡表 + 祁烬 Demo 方案
  - 代码线优先级: 完整 15 天循环 + 祁眠 AI 引擎 + 共享地图结算
  - 美术线优先级: 林行/祁眠/丧尸 Must Have 素材
  - 比赛线: 可先继续完善营销材料,但需对齐实际 Demo 状态

### 比赛线 (Contest Lane): 全材料对齐 Demo 真实状态

- **做了什么**: 完整阅读 CROSS_LANE_LOG（全部四条线 + 阻塞项）、所有 marketing/ 文件、Godot 代码实现（`game_simulation.gd` + `main.gd`），逐一校验比赛材料中的每一条声称是否与当前灰盒 Demo 实际实现一致。
- **修改的文件**:
  - `marketing/PPT_OUTLINE.md`：修复关键错误（Slide 5 祁眠苏醒日 Day 11→Day 5）；重构为 10 页带实现状态标注的幻灯片
  - `marketing/DEMO_PITCH.md`：重写为「当前 Demo 实现」结构，列出全部已实现系统
  - `marketing/PITCH_COPY.md`：三版文案对齐实际资源名、结局、AI 机制
  - `marketing/SUBMISSION_PLAN.md`：Current Demo Truth 扩展为 21 项具体功能清单
  - `marketing/AI_USAGE_STATEMENT.md`：新增 AI 特性六步技术说明 + CodeBuddy 实际使用表格 + 工具链
  - `marketing/DEMO_VIDEO_SCRIPT.md`：细化 6 段时间线分镜 + 录制清单表格
  - `marketing/SCREENSHOT_SHOTLIST.md`：重构为 12 必截 + 8 选截表格
  - `README.md`：当前实现状态扩展为 14 项系统列表
- **关键对齐发现**:
  1. PPT_OUTLINE.md 存在严重事实错误：Slide 5 写「Day 11: Qimian wakes」，但代码中祁眠第 5 天醒来（`resolve_qimian_for_day` 从 day>=5 开始执行）。已在修改中修正。
  2. 天数不一致问题：代码使用 `MAX_DEMO_DAY=14`（days 1-14），设计文档多处写 15 天。比赛材料已按代码实现统一为 14 天口径，但最终需要代码线+设计线协商确认。
  3. 旧资源名残留：PITCH_COPY.md 中曾提及「batteries/电池」——代码中已改为 fuel/燃料。
  4. 「转移幸存者」声明：旧文案声称祁眠转移幸存者，但当前灰盒中 Qimian 行动仅包括资源搬运、尸群偏移和血月掩护，需要设计线确认 Demo 中是否需要加入该功能。
  5. 所有修改严格遵守「比赛真实性原则」：已实现功能用陈述句，计划功能标注 `[planned]` 或 `future scope`。
- **对其他线的影响**:
  - 代码线无直接影响（比赛线未修改 game/ 下的任何文件）
  - 设计线：需要关注比赛材料已统一为 14 天口径，14 vs 15 天需要设计线+代码线最终确认
  - 美术线：SCREENSHOT_SHOTLIST 提供了可操作的截图清单，标注了当前 Demo 实现状态
- **添加到 CROSS_LANE_LOG 的阻塞项**: 14 vs 15 天最终确认（来源线：比赛线，需要代码线+设计线响应）

### 比赛线 (Contest Lane): 角色定位确立

- **用户明确**: 比赛线不是用来规划任务的。三个核心职责：
  1. **完成度审查**: 对照官方赛事要求，检查项目是否跑偏、缺什么材料
  2. **合规建议**: 基于官方手册给出封装与提交指导意见
  3. **提交辅助**: 项目完成时协助准备比赛包和提交
- **写入文件**: `HANDOFF.md`（比赛线 Purpose 和启动提示词重写）、`DECISIONS.md`（Contest Lane Role Clarification）

### 代码线 (Code Lane): 红潮夜系统 + 桥梁清障 + 双层揭示 (2026-05-27 第2轮)

- **触发**: 用户要求代码线对标已锁定的 DECISIONS.md 策划决策，将已策划完成但未实现的功能落地。
- **实现内容**:
  1. 红潮夜统一：Days 11-13 的压力类型从 `qimian` 改为 `red_tide`，与 Day 14 形成统一的「红潮夜」区块。新增 `_resolve_red_tide(day)` 函数，红潮夜强度按 `day - 10` 递增，结算受噪音/气味/光源/门窗/防御影响。
  2. Day 14 超大型尸潮广播：在 Day 14 事件文案中加入了「收音机紧急广播：超大型尸潮将在24小时内抵达本区」作为撤离触发点。更新 `_radio_message_for_day` 的 Day 14+ 分支。
  3. 祁眠 Day 10 桥梁清障行动：新增 `夜晚骑摩托清桥` 行动，祁眠骑摩托夜间抵达学校方向的桥梁，利用喇叭+燃烧物引离尸群。效果是对 school 地点 zombie_delta=-2，恢复通行路线。公共线索为「桥面散落未燃尽的照明棒和轮胎刹车痕」。
  4. Day 15 双层揭示加强：筛查棚外线索从「尸群被牵走了」扩展为「尸群被牵走了 + 摩托穿过了东线封锁」，将祁眠的摩托车足迹引入林行可耳闻的线索。
- **决策对齐**: 对标 `DECISIONS.md` 中 2026-05-27 「Design Line: Evacuation, Qijin, and Two-Week Structure」决策：
  - 红潮夜 (Days 11-14) ✅
  - 超大型尸潮广播 ✅
  - 祁眠清桥行动 (作为5大核心行动之一) ✅
  - 双层揭示含摩托线索 ✅
- **尚未实现的决策**: 汽车系统（修理铺发现旧车→收集电瓶/汽油/轮胎→工作台修理→Day 15 故障弃车徒步）——这是 DECISIONS 中锁定的最大功能，但体量较大，建议下次会话单独处理。
- **修改文件**: `game/scripts/core/game_simulation.gd`
- **测试状态**: ✅ `All simulation tests passed.`（全部 20 项测试通过）
- **CROSS_LANE_LOG 更新**: Code Lane 追加第2轮摘要，比赛线 14/15 天阻塞项更新为「代码线已统一为15天，比赛材料需重新对齐」

### 设定线 (Design Lane): 五天决策 + 15天细线事件表 + 数值平衡 (2026-05-27)

- **触发**: 用户接手设定/策划线，要求：①读 CROSS_LANE_LOG 检查跨线变更 ②不自行决策，呈现问题+选项让用户定 ③凑齐 5 个后写入 ④收工前执行 cross-lane sync。

- **五个锁定决策**（均由用户逐条确认后写入）：

  1. **天数统一为 15 天**：全文档（HANDOFF/DEMO_SCOPE/策划案/DEMO_PITCH/DECISIONS）统一修改。血月 Day 7 + Day 15，红潮夜 Day 11-14。

  2. **祁烬 Demo 呈现**：广播感知（收音机收到返生计划公告）+ Day 15 擦肩暗示（林行凌晨徒步时远望到返生计划车队灯光），不移除白昼协议（留给完整版）。祁烬不出场、不需要新素材。

  3. **撤离条件重构**：自行车只限制近中圈探索范围。真正前往保护区需要汽车——修理铺渐进式流程：Day 5-7 修理铺车库发现旧车 → Day 8-11 收集电瓶+汽油×2+轮胎 → Day 12-13 工作台分次修理 → Day 14 汽车就绪 → Day 15 白天出发，远郊引擎过热/爆胎，弃车徒步，凌晨抵达保护区大门。撤离触发原因从白昼协议改为「广播超大型尸潮 24h 逼近 + 据点受损无法继续坚守」。

  4. **一周目结尾 + 二周目结构**：
     - 一周目结尾双层揭示：①林行视角——筛查棚外听到"尸群被牵走了""摩托穿过了东线"议论 ②祁眠日记+片段回放（5 段分镜动画：醒来/骑行/清桥/留药/血月擦肩）
     - 二周目：回放结束后解锁祁眠为可操作角色，林行由 AI 继承一周目玩家倾向

  5. **祁眠第 5 天醒来**：保持现状，不拆分叙事负担

- **新增文件**:
  - `docs/15天逐日事件表.md`：15 天细线逐日表（This War of Mine 级细度）。包含每天——清晨结算数值、林行可探索地点+房间详情+资源数量+丧尸风险、祁眠隐藏行动+后果+日志残句+AI 决策回放、黄昏决策、夜晚广播原文、旗标变化表。Day 15 延伸至次日凌晨抵达保护区，含完整祁眠日记回放分镜。

- **修改文件**:
  - `docs/策划案.md`：新增汽车获取流程表、祁烬呈现方式、叙事第四幕重写（移除白昼协议、替换为超大型尸潮+据点受损触发）、一周目回放+二周目解锁结构、数值平衡表（林行 8 项状态初始值+每日消耗、6 类资源产出曲线表、15 天压力曲线、设施建造/升级消耗、丧尸数值基准、汽车修理 4 步数值、疲劳-探索时间惩罚表）
  - `docs/DEMO_SCOPE.md`：撤离条件替换为汽车流程、新增一周目回放+二周目可操作章节、更新日期和关联文档引用
  - `docs/DECISIONS.md`：旧撤离条件修正、新增 2026-05-27「Design Line: Evacuation, Qijin, and Two-Week Structure」七项决策
  - `HANDOFF.md`：14-day → 15-day
  - `marketing/DEMO_PITCH.md`：14 → 15 天、血月 Day 14 → Day 15

- **跨线同步状态**:
  - 代码线已在我会话期间推进两轮（红潮夜/桥梁清障/双层揭示均已落地），和我的设计决策高度对齐
  - 代码线尚未实现：汽车系统、一周目回放演出、二周目祁眠可操作。这三个作为新阻塞项写入 CROSS_LANE_LOG
  - 比赛材料线：14 天 → 15 天需重新对齐
  - 美术线：片段回放需要 5 个关键帧/场景、汽车需简单像素素材。已在 CROSS_LANE_LOG 中标注

- **CROSS_LANE_LOG 更新**: 追加 Design Lane 摘要、阻塞项表全部更新（4 条旧项 ✅ 已解决、3 条新项 ⏳ 待代码线响应）

### 代码线 (Code Lane): 汽车撤离系统 (2026-05-27 第3轮)

- **触发**: 设计线产出大量结构化文档（15天逐日事件表、地点结构化数据、祁眠AI决策伪代码、共享地图状态API、祁眠事件关卡布局），用户要求对齐 DECISIONS 中锁定的汽车撤离系统。
- **实现内容**:
  1. **汽车状态系统**: 新增 `state.car`（found/ready/step_engine/step_tire/step_battery/step_fueled/breakdown）和 `state.car_parts`（battery/gasoline/tire）
  2. **四步修理流程**: `_repair_car()` —— ① 引擎线路（建材×2+零件×1）→ ② 换轮胎（轮胎×1+零件×1）→ ③ 装电瓶+调试（电瓶×1+燃料×1）→ ④ 加油（汽油×2）；每步需上一步完成，全部完成 → `car_ready=true` + `evacuation.car_ready=true`
  3. **零件分布**: 轮胎（修理铺店面）、电瓶（派出所停车场/哨卡）、汽油（地铁口维修车/哨卡残油）
  4. **撤离条件更新**: `_determine_ending_state` 中 `bike_ready` → `car_ready`；`evacuation` dict 新增 `car_ready` 字段
  5. **安全路线更新**: Day 5 发现汽车 → Day 12 地铁口取汽油 → Day 13 哨卡取电瓶+汽油 → Day 10/12/13/14 四晚逐步修车 → Day 15 撤离
  6. **UI 更新**: 主界面显示汽车零件库存（电瓶/汽油/轮胎）和修理进度（1/4引擎→2/4轮胎→3/4电瓶→4/4加油→已完成）；夜晚操作新增「修理汽车」按钮
  7. **`explore()` 更新**: 自动识别 `battery/gasoline/tire` 资源键 → 路由到 `state.car_parts` 而非普通资源
- **设计对齐**: 对标 `15天逐日事件表.md` 的车库发现（Day 5-7）+ 零件收集（Day 8-11）+ 工作台修理（Day 12-13）+ 汽车就绪（Day 14）流程
- **尚未实现的决策**: 
  - Day 15 弃车徒步叙事（引擎过热/爆胎的具体文本和状态变化）
  - 14 地点数据结构升级（当前仍为 9 地点简化版，`地点结构化数据.md` 定义了 14 地点 40+ 房间详细数据）
  - 祁眠 AI 决策引擎（`祁眠AI决策伪代码.md` 可直译为 GDScript，但当前仍使用固定 `_qimian_plan` 表）
  - 二周目祁眠可操作角色
- **修改文件**: `game/scripts/core/game_simulation.gd`, `game/scripts/main.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ `All simulation tests passed.`
- **CROSS_LANE_LOG 更新**: Code Lane 追加第3轮摘要，汽车系统阻塞项标记为 ✅

### 代码线 (Code Lane): 14地点房间数据升级 (2026-05-27 第4轮)

- **触发**: 用户要求继续执行策划线已锁定的内容。`docs/地点结构化数据.md` 定义了 14 地点 40+ 房间的精确数据。
- **实现内容**:
  1. **地点扩展 9→14**: 新增 `bridge_camp`（桥洞营地NPC）、`gas_station`（加油站）、`hardware_store`（五金店）、`apartment`（废弃公寓5F/9房含幸存者）、`quarantine`（防疫隔离站）
  2. **房间数据升级**:
     - 旧系统: 每地点 2 间通用模板房
     - 新系统: 每地点 2-9 间精确设计房（含楼层、窗/暗、丧尸数量、精确资源、叙事旗标）
     - 新函数 `_room_data(name, visibility, search_time, hidden_zombies, resources, flags, locked=false)` 替代旧的 `_room()`
  3. **旗标系统**: `_apply_room_flags(room)` — 搜索房间时自动触发 15+ 叙事旗标:
     - `plan_found`, `safezone_hint_1`, `rebirth_clue_1+2`, `address_known`, `childhood_memory`, `rebirth_poster`, `car_found`, `crowbar_found`, `lab_location`, `qimian_file`, `apartment_letter`, `qijin_apartment`, `rebirth_insider`
  4. **锁门机制**: 部分房间 `locked=true`（车库、军械库、五金店二楼、公寓501），搜索时提示"需要撬棍"
  5. **UI 更新**: `get_room_card_text` 显示 🔒 上锁状态
  6. **图标扩展**: `_describe_icons` 新增 `gasoline`(🛢️) 和 `npc`(🧑)
- **设计对齐**: 对标 `docs/地点结构化数据.md` 全部 14 个地点的房间表
- **尚未实现的决策**:
  - Day 15 弃车徒步叙事（引擎过热/爆胎）
  - 祁眠 AI 决策引擎（`祁眠AI决策伪代码.md`）
  - 二周目祁眠可操作角色
- **修改文件**: `game/scripts/core/game_simulation.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ `All simulation tests passed.`

### 代码线 (Code Lane): 噪音模型+情境独白+Day 15 撤离叙事 (2026-05-27 第5轮)

- **触发**: 用户提供优先级路线图，要求"玩家能玩到完整循环"优先。第1优先级（侧视横版）需Godot场景工作，本会话聚焦可落地项。
- **实现内容**:
  1. **噪音传播模型**: `_propagate_noise(day)` — 夜晚结算时据点和探索噪音吸引近圈尸群。噪音来源=据点噪音+白天探索噪音，吸引范围=近圈(≤range 1)，高噪音(≥6)显著增加周边丧尸密度。
  2. **情境独白系统**: `_daily_monologue(day)` — 每天清晨根据生命/感染/饥饿/口渴/疲劳/压力/希望值/汽车状态/天数/祁眠线索动态生成林行独白段落。
  3. **Day 15 完整撤离叙事弧**: `_car_evacuation_narrative()` — 12行叙事：天亮/引擎启动→西线→后视镜据点→仪表盘抖动→爆胎→「操」→弃车徒步→废弃救护车→无名小镇→血月→尸潮→抵达大门。三层结局各有独立叙事闭环。
  4. **Day 15 弃车事件**: `sleep_and_resolve_night` 中写入 `state.car.breakdown`
- **尚未实现的决策**: 祁眠 AI 决策引擎、二周目祁眠可操作角色。侧视横版 Godot 场景迁移是下一步最大工程。
- **修改文件**: `game/scripts/core/game_simulation.gd`, `game/tests/test_game_simulation.gd`
- **测试状态**: ✅ `All simulation tests passed.`

## 2026-05-28

### 代码线 (Code Lane): safe demo 室内搜索流程改造 (第7轮)

- **触发**: HANDOFF 建议「tune the greybox into a clearer playtest flow」「Polish the greybox node map and indoor search」。当前 `play_safe_demo_day` 用 `explore()` 跳过了室内搜索系统，无法在完整 Demo 路径中演练房间级玩法。
- **实现内容**:
  1. **`play_safe_demo_day` 重写**: 从 `explore()` 改为 `enter_location()` → `_auto_search_location()` → `leave_exploration()`。新辅助函数 `_select_safe_location(day)` 覆盖 13/14 个地点（原仅 5 个重复地点），`_auto_search_location(location_id)` 对每个地点自动搜索 ≤3 个房间（优先引诱→谨慎搜索）。
  2. **`enter_location` 补全探索压力**: 新增长途疲劳（route_time）、路况惩罚（`_road_condition_fatigue_penalty`）、尸群压力（`_apply_exploration_risk`）、自行车耐久消耗。这些效果之前在 `explore()` 中有但室内搜索入口没有。
  3. **`search_room` 汽车零件路由**: 识别 `battery/gasoline/tire` 资源键 → 路由到 `state.car_parts`（而非普通资源），与旧 `explore()` 行为一致。
  4. **地点数据微调**: bike_shop 车库解锁（`locked: true → false`），汽车通过房间旗标 `car_found` 自然发现；店面新增 `tire: 1` 确保房间级搜索可获取轮胎。
- **关键洞察**: `explore()` 函数现在仅在室内搜索流程不可用时保留，safe demo 路径已全面使用新流程。这暴露了一个架构问题——`explore()` 和室内搜索是两套并行但部分重复的探索路径，长期应合并。
- **修改文件**: `game/scripts/core/game_simulation.gd`, `game/scripts/core/exploration.gd`, `game/scripts/data/locations.gd`
- **测试状态**: ✅ `All simulation tests passed.`
- **对其他线的影响**: 无。纯代码层改进。
- **下一步建议**:
  - 代码线：合并 `explore()` 和室内搜索为统一路径；考虑加入撬棍物品追踪以支持上锁房间
  - 比赛线：15 天口径已统一（阻塞项表中的比赛线条目可标记 ✅）

## 2026-05-31

### 代码线 (Code Lane): MVC 架构重构 + 对齐 planning_package (第8轮)

- **触发**: 用户要求「查看所有信息更新，尤其是doc，依然按照mvc来设计代码」。Design Lane 在 5/30-5/31 产出了 `planning_package/`（含 GDD、详细策划案、一周目 AI 互动系统），旧目录混乱（core/ 混合 model+controller+view）。
- **实现内容**:
  1. **MVC 目录重组**：
     - `scripts/model/game_state.gd` — Model 层，纯数据。新增 `anomaly_dossier: Array`（未知行动者档案，每项含 day/location_id/clue_text/conclusion）和 `player_marks: Dictionary`（隔空标记，key=location_id，value 含 type/day/note），对齐 planning_package 的"一周目 AI 可读互动系统"。
     - `scripts/controller/` — 5 个静态方法 Controller，只改 Model 不画 UI：`exploration_controller.gd` `shelter_controller.gd` `night_controller.gd` `car_controller.gd` `qimian_controller.gd`
     - `scripts/view/` — 6 个 View，只读 Model 不改状态：`main.gd` `node_map_view.gd` `explorer_view.gd` `shelter_panel.gd` `labels.gd` + 新增 `text_renderer.gd`
     - `scripts/data/` — 不变（constants/events_15d/locations/facilities/qimian_plan）
     - `scripts/core/game_simulation.gd` — 保留为协调器（流程编排 + Controller 委托 + View 文本委托）
  2. **解耦 game_simulation.gd**（388→196行，-50%）：12 个文本格式化方法全部移入 `view/text_renderer.gd` 作为 static func。game_simulation 保留薄层委托方法（向后兼容，测试不中断）。
  3. **数据流明确化**：View(点击) → GameManager(信号) → Controller(改 Model) → View(重绘读 Model)。View 永远只读 `state`；Controller 只写 `state`；`game_simulation` 只做调度。
  4. **全部 preload 路径更新**：10 个文件的引用路径从 `core/*` `ui/*` 更新为 `model/*` `controller/*` `view/*`。清理 `.godot/uid_cache.bin` 解决 class_name 重复注册。
- **CROSS_LANE_LOG 同步发现**: Design Lane 5/31 新增了一周目 AI 互动系统（异常调查+隔空标记），Model 已预埋对应字段待 Controller 实现。阻塞项表无变化（比赛 15 天对齐、一周目回放、二周目仍为后续范围）。
- **文件变化**: 移动 10 个文件，新建 2 个，修改 preload 引用 10 个文件。清理 Godot 缓存。
- **测试状态**: ✅ `All simulation tests passed.`
- **对其他线的影响**: 设定线的 AI 互动系统字段已预埋在 Model，可直接对接。
- **下一步建议**: 代码线优先实现诊所最小 AI 互动链路（药柜异常→求助标记→祁眠夜晚读取→匿名药品反馈）；考虑实现 `player_mark_action` Controller 方法。
- **下一步建议**:
  - 代码线：合并 `explore()` 和室内搜索为统一路径；考虑加入撬棍物品追踪以支持上锁房间
  - 比赛线：15 天口径已统一（阻塞项表中的比赛线条目可标记 ✅）

## 2026-05-30

### 比赛/介绍材料：根目录介绍文案

- 用户要求将游戏介绍保存到本地，文件名为 `介绍`。
- 已整理并保存 `介绍.md`，内容覆盖：2D 像素末日生存经营定位、林行 15 天一周目、祁眠 AI 隐藏行动、人格卡确定性决策、共享地图改写、通关日志揭示、第二周目祁眠可操作、林行由 AI 接管。
- 本次介绍采用最新设计口径：15 天 Demo、Day 7/Day 15 双血月、旧车撤离、祁眠第 5 天醒来、AI 不是聊天框而是隐藏主角行为引擎。
- 后续比赛材料可从 `介绍.md` 提取短版简介、PPT 开场、视频旁白和 AI 玩法说明。

### 设定/开发入口：One Page GDD

- 用户转述老师反馈：`介绍.md` 更像宣传文案，不是能指导开发的 GDD；需要 one-page，写清机制循环和系统构成，避免只写“玩家可以……”。
- 已新增 `docs/ONE_PAGE_GDD.md`，作为开发/答辩入口文件。
- 文件内容覆盖：游戏定位、核心循环、林行玩家系统、15 天结构、撤离条件、祁眠 AI 输入和决策流程、共享地图状态、通关揭示、第二周目边界、Demo 必做/不承诺范围。
- 第二周目口径调整为比赛 Demo 的“祁眠关键行动章节”，不承诺完整第二条 15 天战役；完整版可扩展为完整二周目。

### 策划包集中整理

- 用户要求按照课程截图中的结构，将策划总纲、策划概要案、系统策划案/GDD、详细策划案集中制作出来，结合当前项目和已有文档信息，冗余旧文档可删改。
- 已新增 `docs/planning_package/` 作为当前统一策划入口：
  - `README.md`：说明使用方式、当前统一口径和旧文档细节来源
  - `01_策划总纲.md`：游戏名称、系统、目标玩家、故事概要、独特性、卖点、发行计划、可行性
  - `02_策划概要案.md`：基础规则、大致流程、系统列表、异常问题处理、创新点、当前实现对齐
  - `03_系统策划案_GDD.md`：封面、系统概述、核心循环、功能规则、UI/UX、剧情、美术、音效、附录索引
  - `04_详细策划案.md`：程序模块、状态字段、机制流程、UI、场景、剧情节点、美术、音效、测试检查表、范围控制
- 已将 `README.md` 和 `marketing/` 主要材料从旧 14 天/9 地点/bike_ready 口径同步到当前 15 天/14 地点/car_ready/Day 15 终局血月口径。
- `docs/ONE_PAGE_GDD.md`、`docs/策划案.md`、`docs/DEMO_SCOPE.md` 顶部已标注当前维护入口，旧长文档保留为历史/细节来源。
- `docs/DECISIONS.md` 新增稳定决策：`docs/planning_package/` 是当前开发、课程评审和展示准备的 canonical design entry。

## 2026-05-31

### 设定线：一周目 AI 可读互动系统

- 用户指出一周目如果只让玩家接受“看似随机”的 AI 后果，会太依赖美术打磨；需要让玩家在一周目就能玩懂并轻度影响 AI。
- 已将稳定机制锁定为 **“异常调查 + 隔空标记”**：
  - **异常调查**：林行发现新鲜撬锁痕、便携食物减少、摩托胎痕、尸群被引偏、匿名药品、荧光标记等异常，写入“未知行动者档案”。
  - **隔空标记**：林行离开地点前可留下危险、求助、路线、物资保留标记；这些是祁眠夜晚 AI 的可感知世界痕迹，不是玩家命令。
- 已更新策划包和接口文档：
  - `docs/planning_package/03_系统策划案_GDD.md`
  - `docs/planning_package/04_详细策划案.md`
  - `docs/ONE_PAGE_GDD.md`
  - `docs/DEMO_SCOPE.md`
  - `docs/共享地图状态API.md`
  - `docs/祁眠AI决策伪代码.md`
- 新状态/接口口径：
  - `state.anomaly_dossier`
  - `location.anomaly_tags`
  - `location.player_mark`
  - `location.player_mark_day`
  - `location.player_reserved_resources`
  - `perceivable_state.player_marks`
  - `world_trace_input`
- 最小 Demo 链路：Day 6 诊所发现药柜异常 → 玩家留下求助标记 → 祁眠 AI 读取标记、药品剩余、低暴露风险 → 次日匿名抗生素/浅箭头出现 → 档案验证“对方能理解幸存者标记，且不完全敌对”。
- 设计边界：一周目标记不会让玩家直接控制祁眠；祁眠仍受人格卡、寻找祁烬、暴露值、区域热度和可感知信息限制。

## 2026-06-02

### 总体方向：Unity 全量迁移 + PlayKit.ai Unity SDK

- 用户已明确决定将《保护区之外》主开发方向从 Godot 转为 Unity；后续 CodeBuddy 执行迁移时，应把 Godot 项目作为规则、数据、文本、测试和灰盒参考保留，而不是继续作为主开发线扩展。
- 新 Unity 项目目标路径锁定为 `E:\Download\working\BeyondSafeZoneUnity`。当前读取时该目录尚不存在，迁移执行者需要新建 Unity 2D 项目。
- Godot 现有实现仍然是重要参考：15 天循环、14 地点、室内搜索、五大设施、汽车撤离、血月/红潮、祁眠本地 AI、`state.anomaly_dossier` 和 `state.player_marks` 预埋字段，都应迁移到 Unity 灰盒中。
- 策划口径同步调整：`docs/planning_package/` 仍是统一策划入口，但引擎/实现描述应按“Unity 为主开发目标；Godot 4.6.2 灰盒为迁移参考”理解。
- PlayKit.ai 接入方向锁定为 Unity SDK。用户提供的 Dashboard 截图显示 Godot SDK 为“即将推出”，因此当前不以 Godot SDK 作为主接入依据。
- PlayKit.ai 在本项目中的职责是叙事文本增强：异常档案文本、祁眠日志文本、NPC/广播/独白文本等。核心规则仍由本地 Unity/C# 规则控制，包括资源、伤害、结局、行动合法性、祁眠行动选择。
- CodeBuddy 执行提示词已给出：先读现有文档和 Godot 源码，先写 `docs/UNITY_MIGRATION_PLAN.md`，再创建 `E:\Download\working\BeyondSafeZoneUnity`，最后写 `docs/UNITY_MIGRATION_STATUS.md`。
- 安全要求：不要把 Developer Token 写进代码、配置文件或仓库；PlayKit Unity SDK 具体 C# API 名称必须从 SDK 文档或示例确认，不能猜测。

### 设定线：开发任务拆解与招队友分工

- 用户要求把策划案设计拆解成可执行任务，尤其说明程序需要做哪些模块、美术需要哪些素材。
- 已新增 `docs/开发任务拆解.md`，用途是给新队友快速理解当前已有基础、待做任务、岗位分工和下一步排期。
- 文档结构包括：当前项目状态、程序任务 P0/P1/P2、美术任务 P0/P1/P2、UI/UX、策划/关卡、音频、测试/QA、比赛/招队友材料、当前最适合招的队友、下一步推荐排期。
- 已核对当前代码和文档字段：`game/scripts/model/game_state.gd` 中存在 `state.anomaly_dossier` 和 `state.player_marks`；`docs/共享地图状态API.md` 和 `docs/祁眠AI决策伪代码.md` 中记录了 `anomaly_tags`、`player_mark`、`player_mark_day`、`player_reserved_resources`、`perceivable_state.player_marks`、`world_trace_input`。
- 文档表达口径：P0 聚焦“异常调查 + 隔空标记 + 诊所最小链路”，这是最能让一周目玩家玩懂 AI 的核心闭环；行动点/骰子和 NPC 合作被标为老师建议下的机制增强，需要先写详细规则再进代码。
- 未更新 `docs/DECISIONS.md`，因为本次没有锁定新的稳定机制；行动点/骰子与 NPC 合作仍是待定稿方向。
- 本次没有修改 Godot 代码，也没有运行 Godot 测试；验证方式是文档读取、字段检索和任务编号检索。

### 美术线：FHL Image Studio CLI 魔改包只读审计

- 用户目标：不是使用官方 ChatGPT Plus 生图，而是让 Codex 自动读取项目、拆分素材需求、优化提示词，并通过本地 CLI 批量生成游戏素材、分镜和概念图。
- 已放入根目录的文件：`FHL-Image-Studio方汤圆CLI魔改版1.0.7.zip`。
- 只读审计结果：
  - ZIP SHA256：`67FCEEB3EC296B5033D5E0395FE22824EAC8CE7E4CA616C06652A636F538FDD9`。
  - 包内包含 `image-cli.cmd`、`start-ui.cmd`、Go CLI 源码、Image Studio 前后端源码、Cloudflare Worker、Android shell、便携 `runtime/cli/gptcodex-image.exe` 与 `runtime/node/node.exe`。
  - `image-cli.cmd` 默认调用包内 `runtime/cli/gptcodex-image.exe`，默认 base URL 为 `https://www.fhl.mom`，API mode 为 `responses`，文本模型 `gpt-5.5`，图像模型 `gpt-image-2`，输出目录为包内 `output/`，raw 日志为 `output/log/`。
  - `config/cli.env.example` 要求用户本地复制为 `config/cli.env.local` 并填写 `IMAGE_STUDIO_API_KEY`；不得把 key 发到聊天窗口或写入仓库。
  - 源码里看到 base URL HTTPS 校验、Authorization Bearer 请求、keyring/本地 env 配置、raw SSE 响应保存逻辑。未发现启动脚本中明显的额外后台安装动作，但没有运行预编译 exe，第三方二进制仍按不可信处理。
- 安全边界：
  - 本次未解压、未运行 exe、未启动 UI、未配置 API Key、未生成图片。
  - 后续如启用，建议先解压到项目外临时目录或加入 `.gitignore` 的工具目录，配置只放本地私有文件，生成图进入 `assets/source/ai_generated/` 或独立 staging 目录，筛选后再进 `assets/sprites/`。
  - 任何由该工具生成并进入公开参赛包的素材，都必须更新 `docs/ASSET_LICENSE_LOG.md`，记录工具、日期、prompt 摘要、人工修改情况、商业/参赛可用状态。
- 推荐用途：先用于概念图、关键剧情分镜、宣传图、场景气氛图和 UI 图标参考，不要直接期待生成最终 `32x32` 透明像素动画帧；正式 sprite 仍需人工清理、像素化和统一调色。

## 2026-06-03

### 总体规划：文档目录整理

- 用户要求整理整个文件夹，把很多用不上的 doc 移到一起。
- 本次整理原则：不删除内容；只把当前入口、参考资料、历史归档分开；活文档更新引用，历史记录中的旧路径不强行重写。
- 当前活入口：
  - `HANDOFF.md`
  - `README.md`
  - `docs/planning_package/README.md`
  - `docs/UNITY_MIGRATION_PLAN.md`
  - `docs/UNITY_MIGRATION_STATUS.md`
  - `docs/开发任务拆解.md`
  - `docs/ASSET_PIPELINE.md`
  - `docs/ASSET_LICENSE_LOG.md`
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/DECISIONS.md`
- 新增/整理 `docs/reference/`，用于仍有实现价值的细节文件：`DEMO_SCOPE.md`、`15天逐日事件表.md`、`地点结构化数据.md`、`共享地图状态API.md`、`祁眠AI决策伪代码.md`、`祁眠事件关卡布局.md`。
- 新增/整理 `docs/archive/`，用于旧入口、历史分析、旧技术报告、灰盒 HTML 原型、音视频图片和临时图片：`legacy_design/`、`analysis/`、`technical/`、`prototypes/`、`media/`、`superpowers/`。
- 根目录 `介绍.md` 已移动到 `marketing/介绍.md`；根目录 `temp_img/` 已移动到 `docs/archive/media/temp_img/`。
- 已补 `docs/reference/README.md` 和 `docs/archive/README.md`，说明两个目录用途。
- 已同步修改 `HANDOFF.md`、`README.md`、`docs/planning_package/README.md`、`docs/planning_package/03_系统策划案_GDD.md`、`docs/DECISIONS.md`、`docs/开发任务拆解.md`、`task_plan.md`、`progress.md`、`findings.md`。
- 稳定决策已写入 `docs/DECISIONS.md`：active / reference / archive 文档布局。
- 注意：尝试删除空目录 `docs/superpowers/specs` 时 PowerShell 返回 Access denied，因此空目录暂时保留；其中原规格文件已归档到 `docs/archive/superpowers/`。

### 设定线：最小 Demo 范围锁定

- 用户明确认为当前内容需要删减才能做完，并批准先按最小可玩纵切推进。
- 已新增并确立 `docs/MINIMUM_DEMO_SCOPE.md` 为近期制作范围依据；`docs/DECISIONS.md` 已记录 `Minimum Demo Scope Lock`。
- 近期目标：15 天叙事框架下的 10-15 分钟可试玩 Demo，证明“隐藏 AI 主角改写玩家生存路线”成立。
- P0 只保留 4 个核心地点：林行家/据点、社区诊所、小区超市、修理铺/车库。
- P0 AI 链路：林行发现诊所异常 → 留下求助标记 → 祁眠 AI 夜里读取标记 → 次日出现匿名药品/地图或档案反馈 → 结尾日志展示祁眠人格卡、输入、候选行动、排序理由、最终选择和地图影响。
- 已同步收窄 `HANDOFF.md`、`docs/planning_package/01_策划总纲.md`、`02_策划概要案.md`、`04_详细策划案.md`、`docs/开发任务拆解.md`。
- 已修正 `docs/开发任务拆解.md` 中程序任务编号重复：P0 为 `C-001` 到 `C-010`，P1 从 `C-011` 开始，P2 从 `C-019` 开始。
- 明确延期：完整二周目、林行 AI 接管、祁眠 9 个侧视关卡、桥洞营地完整 NPC 群、14 地点全量、40+ 房间全量、复杂行动点/骰子、长期 NPC 合作、5 段回放动画和完整远圈/保护区关卡。
- 老师建议的行动点/骰子与 NPC 合作仍有价值，但在当前项目中作为 P1/P2 增强方向；必须先写小规则规格，不应压过诊所 AI 最小链路。

## 2026-06-04

### 代码线：OneRunMain HUD 布局热修

- 用户发来 Game 视图截图，画面问题包括：
  - 状态文本压在据点灰盒中间。
  - 设施/搜索点世界标签字号过大，多个标签重叠。
  - 底部 7 个按钮固定单行横排，右侧按钮溢出画面。
  - 日志/状态/提示没有清晰分区。
- 根因定位到 `E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\UI\OneRunGameController.cs` 的运行时 UI 生成参数：
  - `Status` 文字使用顶部中心锚点但左对齐，导致出现在画面中上区域而不是左上 HUD 区。
  - `Log` 使用右下逻辑，和当前信息架构不匹配。
  - 按钮 x 坐标从 `-320` 排到 `500`，在当前 Game 视图宽度下必然溢出。
  - `CreateWorldLabel()` 使用 `TextMeshPro` 世界空间字号 `2.8f`，对于 2D 据点尺度过大。
- 本次只做 UI 热修，不新增玩法规则：
  - `CanvasScaler` 增加 `referenceResolution = 1280x720`，`matchWidthOrHeight = 0.5`。
  - `Header` 顶部居中。
  - `Status` 改为左上，`anchoredPosition (18, -72)`，`sizeDelta 360x132`。
  - `Log` 改为右上，`anchoredPosition (-18, -72)`，`sizeDelta 390x150`。
  - `Prompt` 改为底部居中，`anchoredPosition (0, 12)`，`sizeDelta 780x30`。
  - 按钮分两行：上排 `ExploreClinic / ExploreSupermarket / ExploreGarage / ReturnShelter / LeaveHelpMark`，y=`84`；下排 `ResolveNight / NextDay`，y=`40`。
  - 世界空间标签字号从 `2.8f` 调整为 `0.75f`，标签框从 `4x1` 改为 `3.2x0.7`。
- 验证结果：
  - `OneRunGameController.cs` 编译反馈：`errorCount: 0`。
  - Play 模式运行时读取 `OneRunHUD/Header`、`Status`、`Log`、`Prompt`、`ExploreClinic`、`LeaveHelpMark`、`ResolveNight`、`NextDay` 的 RectTransform，确认已经分区；第二轮微调后 `ExploreClinic` y=`84`，`ResolveNight` y=`40`，`Prompt` y=`4`。
  - Play 验证时 Console：`warnings: 0`、`errors: 0`。
  - 过程中误在 Play 模式启动 Unity Test Runner，jobId `68504ce3` 失败，报错为 `This cannot be used during play mode`；这是操作时机错误，不是玩法脚本断言失败。
  - 已退出 Play、清空 Console、在 EditMode 重跑完整回归；第二轮微调后的最终回归为 `BeyondSafeZone.Tests.TestGameSimulation` `41/41 passed`，jobId `cb51ad29`。
  - 最终 Console：`warnings: 0`、`errors: 0`。
- 后续建议：如果用户仍觉得画面乱，应进入正式 `U-001/U-002/U-008` HUD/日志面板任务，做真正 UI 信息架构；不要继续只靠临时位置参数堆按钮。

### 代码线：C-010 最小纵切集成测试

- 本次只推进任务编号 `C-010`，没有新增玩法系统，也没有扩大到 UI 面板、美术表现或二周目内容。
- 本次短规格：
  - 触发条件：Unity EditMode 运行 `BeyondSafeZone.Tests.TestGameSimulation`。
  - 玩家操作：测试模拟 Day 1 白天探索、搜索、返回据点、夜晚结算；Day 5 诊所搜索异常线索、留下求助标记、夜晚结算；Day 15 查看结尾日志。
  - 状态变化：验证探索、玩家标记、祁眠读取/回应、异常档案、终局日志串成同一条链。
  - 可见反馈：返回文本、地点卡、档案文本、终局日志都能读到关键反馈。
  - 验证方法：新增一条 Unity EditMode 集成测试，跑单测和完整回归。
- Unity 实现落点：
  - `E:\Download\working\BeyondSafeZoneUnity\Assets\Tests\TestGameSimulation.cs` 新增 `TestMinimumVerticalSliceCoversClinicAiChain()`。
  - 没有修改生产脚本；测试直接通过，说明 C-005 到 C-009 的既有实现已能组成最小纵切链路。
- 测试覆盖内容：
  - Day 1 `convenience` 进入、搜索、返回据点、夜晚推进到 Day 2。
  - Day 5 `clinic/exam_a` 搜索触发 `诊所隔离记录` 异常档案。
  - `clinic/help` 标记写入 `state.PlayerMarks`。
  - Day 5 夜晚返回文本包含 `求助标记` 和 `匿名药品`。
  - 诊所地点卡包含 `祁眠异常`。
  - 异常档案包含 `诊所隔离记录` 和 `匿名药品`。
  - Day 15 `Reveal.Summary` 包含 `人格卡`、`感知输入`、`最终选择`、`地图影响`、`社区诊所`、`求助标记`、`匿名药品`。
- 验证结果：
  - `TestGameSimulation.cs` 编译反馈：`errorCount: 0`。
  - Unity Console 初始检查：`warnings: 0`、`errors: 0`。
  - 新增单测 `TestMinimumVerticalSliceCoversClinicAiChain`：`1/1 passed`，jobId `1db8f604`。
  - 完整 Unity EditMode 回归 `BeyondSafeZone.Tests.TestGameSimulation`：`41/41 passed`，jobId `6103dd69`。
- 设计影响：P0 程序任务 `C-005` 到 `C-010` 的诊所 AI 最小链路已有集成回归保护。后续如果调整行动点、NPC 合作、日志 UI 或 PlayKit 文本润色，需要保持这条链路不被打断。
- `docs/DECISIONS.md` 本次未更新，因为没有新增稳定项目决策。

### 代码线：C-009 结尾日志解释诊所 AI 因果链

- 本次只推进任务编号 `C-009`，没有扩大到回放动画、独立档案 UI、二周目或新 AI 规则。
- 开工已按协议读取 `AGENTS.md`、`HANDOFF.md`、`docs/CROSS_LANE_LOG.md`，并补读 `docs/开发任务拆解.md`、`docs/MINIMUM_DEMO_SCOPE.md`、`docs/UNITY_MIGRATION_STATUS.md`。
- 本次短规格：
  - 触发条件：Day 15 或等价终局结算后，`state.Reveal.Unlocked == true`，且一周目中 `clinic` 存在 `help` 标记并触发过匿名药品反馈。
  - 玩家操作：完成终局夜晚结算，查看结尾日志文本。
  - 状态变化：终局 `Reveal.Summary` 追加祁眠结构化解释日志。
  - 可见反馈：结尾文本显示人格卡、感知输入、候选行动、排序理由、最终选择、地图影响，并点明社区诊所求助标记、匿名药品。
  - 验证方法：Unity EditMode 红绿测试、完整回归、Play 运行态 Console 检查。
- Unity 实现落点：
  - `E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Core\TextRenderer.cs` 新增 `GetQimianEndingRevealText(GameState state)`，只读状态生成结尾日志文本。
  - `E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Core\GameSimulation.cs` 新增同名委托，供终局和后续 UI 读取。
  - `E:\Download\working\BeyondSafeZoneUnity\Assets\Scripts\Controllers\NightController.cs` 在 Day 15 终局时把结构化日志追加进 `state.Reveal.Summary`。
  - `E:\Download\working\BeyondSafeZoneUnity\Assets\Tests\TestGameSimulation.cs` 新增 `TestEndingRevealExplainsClinicHelpMarkCausality`。
- TDD 记录：
  - 红测：`TestEndingRevealExplainsClinicHelpMarkCausality` 为 `0/1 passed`，jobId `9b3f6d78`。
  - 绿测：同一测试为 `1/1 passed`，jobId `38843bb8`。
  - 完整回归：`BeyondSafeZone.Tests.TestGameSimulation` 为 `40/40 passed`，jobId `061afa85`。
- 编译/运行验证：
  - `TextRenderer.cs`、`GameSimulation.cs`、`NightController.cs`、`TestGameSimulation.cs` 编译反馈均为 `errorCount: 0`。
  - Play 运行态 Console 统计：`warnings: 0`、`errors: 0`。
- 设计影响：P0 诊所 AI 链路现在具备“玩家一周目留下标记 → 祁眠夜晚读取并回应 → 通关后日志解释规则”的最小闭环。后续体验提升优先考虑 `U-008` 日志面板，而不是先做 5 段回放动画。
- `docs/DECISIONS.md` 本次未更新，因为没有新增稳定项目决策；只是落实既有 C-009 范围。

### 代码线：Unity MainPrototype UI 灰盒整理

- 用户已安装并运行 UnitySkills 配套 Unity 插件；REST 健康检查地址为 `http://127.0.0.1:8090/health`。
- 本次验证到的 Unity 环境：
  - Unity project: `E:\Download\working\BeyondSafeZoneUnity`
  - Unity version: `2022.3.62f3c1`
  - UnitySkills version: `2.0.1`
  - UnitySkills mode: `auto`
  - 当前场景：`Assets/Scenes/MainPrototype.unity`
- 场景结构：
  - 根对象：`Main Camera`、`Directional Light`、`GameController`、`Canvas`、`EventSystem`
  - `GameController` 上有 `MainPrototypeController`
  - `Canvas` 下有 `HeaderText`、`StatusText`、`DetailText`、`LogText`、`LocationPanel`、`ActionPanel`
  - `LocationPanel` 下有 `ShelterButton`、`ClinicButton`、`SupermarketButton`、`GarageButton`
  - `ActionPanel` 下有 `CarefulSearchButton`、`QuickSearchButton`、`LeaveHelpMarkButton`、`ResolveNightButton`、`NextDayButton`
- 已确认 `MainPrototypeController` 的序列化字段均已连上，包括 `headerText`、`statusText`、`detailText`、`logText` 和 9 个按钮引用。
- 已通过 UnitySkills 整理 `MainPrototype` UI：
  - 顶部标题：`Canvas/HeaderText`
  - 左侧状态：`Canvas/StatusText`
  - 左下地点按钮：`Canvas/LocationPanel`
  - 中央行动按钮：`Canvas/ActionPanel`
  - 右侧地点详情：`Canvas/DetailText`
  - 底部日志：`Canvas/LogText`
  - 按钮文案改为 `据点`、`诊所`、`超市`、`车库`、`谨慎搜索`、`快速搜索`、`留下求助标记`、`夜晚结算`、`下一天`
  - 编辑态文本改为与原型流程相关的中文文案，避免显示 `New Text` / `Button`
  - 正文、按钮字号和对齐已统一，按钮从过小尺寸调整为稳定可点尺寸
- 字体处理：
  - 字体资产路径：`Assets/Fonts/ChineseTMP.asset`
  - `ChineseTMP` 的 `sourceFontFile` 已读回为 `SIMHEI`
  - 已将 `atlasPopulationMode` 设置为 `Dynamic`
  - 已将 `isMultiAtlasTexturesEnabled` 设置为 `True`
  - 目的：减少后续新增中文时出现方块或 TMP 缺字 warning 的概率
- 验证结果：
  - UnitySkills `/health` 成功
  - `console_get_stats` 显示 `errors: 0`
  - `scene_get_info` 显示场景仍为 dirty，因为 `scene_save` 在当前 `auto` 模式下返回 `MODE_FORBIDDEN`
- 重要 gotcha：
  - UnitySkills `scene_save` 在 `auto` 模式下被判定为 high-risk / never-in-semi，不能保存场景；用户需要在 Unity 里手动 `Ctrl+S` 保存，或把 UnitySkills 切到 Bypass 后再让 Codex 调用 `scene_save`。
  - Console 里仍有 TMP 缺字 warning 历史记录；动态字体设置已写入，但需要清空 Console 并重新 Play 后再判断 warning 是否消失。
