# Project Memory

This file is the long-term memory for BeyondSafeZone conversations. Keep `HANDOFF.md` short; put durable details here.

## Current Snapshot

- Project: 《保护区之外》 / `Beyond Safe Zone`.
- Current engine: Unity.
- Current repository root: `E:\Download\working\BeyondSafeZone`.
- Current Unity project: `E:\Download\working\BeyondSafeZone\BeyondSafeZoneUnity`.
- Current formal scene: `BeyondSafeZoneUnity/Assets/Scenes/OneRunMain.unity`.
- Current target: a 10-15 minute Unity greybox vertical slice inside the 15-day Lin Xing first-run frame.
- Current proof chain: clinic anomaly → Lin Xing leaves help marker → Qimian AI reads it at night → anonymous medicine / shallow-arrow feedback → unknown-actor dossier → ending log explains the hidden decision chain.

## Stable Decisions

- `BeyondSafeZoneUnity/` inside the repository is the only current Unity project entry.
- The sibling folder `E:\Download\working\BeyondSafeZoneUnity` is deprecated and must not be used.
- The old Godot `game/` project is removed from the current working tree and should not guide current implementation claims.
- `docs/UNITY_STATUS.md` is the current Unity verification and blocker record.
- `docs/UNITY_MIGRATION_PLAN.md` and `docs/UNITY_MIGRATION_STATUS.md` are obsolete current-entry documents.
- `Assets/Scenes/OneRunMain.unity` is the formal first-run scene.
- `Assets/Scenes/MainPrototype.unity` is reference-only.
- Contest materials must describe current Unity greybox truth and mark future scope clearly.

## User Preferences And Workflow

- Respond in Chinese unless the user asks otherwise.
- Be structured, careful, and do not guess exact identifiers, paths, fields, or API names.
- Read source files, docs, configs, or tests before making coding or path claims.
- Use single-task small loops.
- Do not enter implementation without a short spec containing:
  - trigger condition
  - player action
  - state change
  - visible feedback
  - verification method
- A task is complete only when:
  - docs updated
  - implementation landed
  - verification recorded
- Before ending substantial work, update:
  - `docs/CROSS_LANE_LOG.md`
  - `docs/PROJECT_MEMORY.md`
  - `docs/DECISIONS.md` if a stable decision changed

## Current Implementation Facts

- `OneRunMain` runtime generates:
  - side-view cutaway shelter greybox
  - ground and upper floor platforms
  - stair trigger for moving between floors
  - Lin Xing player object
  - Lin Xing prototype sprite from `Assets/Resources/Sprites/Characters/lin_xing_player.png`
  - shelter facilities
  - shelter facility state visuals
  - HUD
  - readability HUD panels: `ReadabilitySafeFrame`, `StatusPanel`, `LogPanel`, `PromptPanel`
  - clinic / supermarket / garage greybox entries
- SHELTER-002 is implemented:
  - Shelter facilities now expose `Blueprint_*`, `Built_*`, `UsedMarker_*`, `DamageMarker_barricade`, and `Feedback_*` runtime children.
  - `ShelterInteractable.Refresh` switches those layers from existing `GameState` facility data without changing shelter economy rules.
  - Pressing `E` on a shelter facility writes the action result into the nearby `Feedback_*` world text and still reports the same result to the HUD log.
- SHELTER-001 is implemented:
  - `WalkableShelterGreybox` now contains `CutawayShelterFrame`, `ShelterFloor_Ground`, `ShelterFloor_Upper`, and `Stairs_GroundToUpper`.
  - Lin Xing uses `SideViewShelterPlayerController` while inside the shelter.
  - Shelter movement reads direct `A/D` and `LeftArrow/RightArrow` input as a fallback beyond Unity's `Horizontal` axis. Stair switching reads direct `W/S` and `UpArrow/DownArrow` input when near the stair trigger.
  - Entering scavenging switches Lin Xing back to `TopDownPlayerController`; returning to shelter switches him back to side-view shelter movement.
  - Shelter facilities expose `State_*` visual blocks for unbuilt, used-today, and damaged-wall feedback.
- U-007 is implemented:
  - HUD `档案` button
  - `未知行动者档案` panel
  - `暂无异常记录。` empty state
  - dossier text reads `GameSimulation.GetAnomalyDossierText(State)`
- U-008 is implemented:
  - HUD `日志` button
  - `祁眠行动日志` panel
  - locked text `通关后解锁祁眠行动日志。`
  - reveal text reads `GameSimulation.GetQimianEndingRevealText(State)` after `State.Reveal.Unlocked`
- U-004 is implemented:
  - HUD `LocationCardPanel` added above the location buttons, containing 3 location cards.
  - Each card (`LocationCard_clinic`, `LocationCard_supermarket`, `LocationCard_bike_shop`) shows location name and info text.
  - Clinic: `社区诊所` / `资源：药品 / 危险：中 / 异常：待调查`
  - Supermarket: `小区超市` / `资源：食物、水 / 危险：低 / 异常：暂无`
  - Garage: `修理铺车库` / `资源：零件、燃料 / 危险：中 / 异常：暂无`
  - Dynamic append: `有新痕迹` (QimianTrace), `已留标记` (PlayerMarks), `资源减少` (visited + low resources).
  - New private methods: `BuildLocationCards()`, `RefreshLocationCards()`, `GetLocationCardInfo()`.
  - `BuildHud()` creates cards; `RefreshAll()` calls `RefreshLocationCards()`.
- ARCH-001 is implemented at the test source level:
  - `Assets/Tests/TestGameSimulation.cs` now keeps core rules and deterministic simulation tests only.
  - `Assets/Tests/TestOneRunUI.cs` owns HUD / dossier / Qimian log / objective / location-card UI tests.
  - `Assets/Tests/TestOneRunWorld.cs` owns shelter, movement, interactable, and runtime-world scaffolding tests.
  - `Assets/Tests/OneRunTestHelpers.cs` provides shared UI/world test helpers.
  - `OneRunGameController.cs` is intentionally not split yet; future feature work should prefer small helper methods and avoid expanding core methods.
- U-001 is implemented:
  - HUD `ObjectivePanel` below Header, with `ObjectiveTitle` (固定 `当前目标`) and `ObjectiveBody` (动态 1-2 行目标说明)
  - Header shows Chinese phase: `第 {Day} 天  {阶段中文名}` instead of English `Phase`
  - Phase label mapping: `morning→清晨, day→白天, searching→搜刮中, evening→黄昏, night→夜晚, reveal→结尾揭示`
  - Objective text changes by state: Day 1-4 scavenge prompt, Day 5+ unknown actor + clinic hints, post-mark push toward night, post-reveal directs to log panel
- C-007 to C-010 are implemented around the clinic AI chain:
  - Qimian reads the clinic `help` mark after Day 5.
  - Night feedback tells the player the mark was noticed.
  - Clinic receives anonymous medicine / shallow-arrow feedback.
  - Ending reveal explains personality card, perceivable input, options, ordering, final choice, and map impact.

## Latest Verification Evidence

- ARCH-001 verified on 2026-06-06:
  - Source count: `TestGameSimulation.cs` has 40 `[Test]`; `TestOneRunUI.cs` has 6 `[Test]`; `TestOneRunWorld.cs` has 5 `[Test]`; total project tests in source = 51.
  - UnitySkills `/health`: `status: ok`, Unity `2022.3.62f3c1`, UnitySkills `2.0.1`, `currentMode: bypass`, not compiling.
  - `BeyondSafeZone.Tests.TestOneRunUI`: `1/1 passed`, jobId `d1ccae55`.
  - `BeyondSafeZone.Tests.TestOneRunWorld`: `1/1 passed`, jobId `b427ad52`.
  - Exact-method checks passed: `TestOneRunHudShowsLocationCards` (`cd712083`), `TestShelterInteractionShowsVisibleFeedbackText` (`d58335f9`), `TestMinimumVerticalSliceCoversClinicAiChain` (`d2cecf9c`).
  - Current Unity Test Runner caveat: class-level `BeyondSafeZone.Tests.TestGameSimulation` returns `43/51 passed, 8 failed`, jobId `375e5e88`, because `test_list` still uses `unity_test_runner_async_cache` and reports moved UI/world tests under old `TestGameSimulation.*` names.
  - `debug_force_recompile` was run and Unity finished compiling, but discovery remained stale. Next verification should restart Unity Editor or clear Test Runner discovery cache before rerunning the three classes.
- U-004 verified on 2026-06-06 (v2 —审查修复):
  - Fix: `GetLocationCardInfo("clinic")` anomaly changed from `暂无` to `待调查` (clinic-specific initial state).
  - Test updated: `TestOneRunHudShowsLocationCards` now asserts `待调查` for clinic.
  - New test run: `TestOneRunHudShowsLocationCards`, `1/1 passed`, jobId `ba5fb6e0`.
  - Regression: `TestOneRunHudShowsCurrentObjectiveAndChinesePhase` (`242c27d2`), `TestDossierButtonOpensEmptyDossierPanel` (`e3f627e8`), `TestQimianLogButtonOpensLockedLogPanel` (`3f157c57`), `TestMinimumVerticalSliceCoversClinicAiChain` (`7e4db0c3`) all `1/1 passed`.
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, 52 `[Test]` methods all passed, jobId `6cd8b217`.
  - Play screenshot: `BeyondSafeZoneUnity/Assets/Screenshots/u004_onerunmain_play.png` (1280×720, 3 location cards visible).
  - Console: `logs: 0`, `warnings: 0`, `errors: 1` (stale CS0103 cache, not a real compile error — method `BuildObjectivePanel` exists at line 489, all 52 tests pass).
- U-004 (initial implementation) verified on 2026-06-06:
  - New test: `TestOneRunHudShowsLocationCards`, `1/1 passed`, jobId `fa4d436d`.
  - Regression tests all passed: `TestOneRunHudShowsCurrentObjectiveAndChinesePhase` (`17cfbff9`), `TestDossierButtonOpensEmptyDossierPanel` (`2daf972b`), `TestQimianLogButtonOpensLockedLogPanel` (`701a9d34`), `TestMinimumVerticalSliceCoversClinicAiChain` (`0edeea54`).
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, jobId `626bbf16`.
  - Compile error fix: `BuildObjectivePanel` was accidentally overwritten during initial edit; restored and recompiled.
- U-001 verified on 2026-06-05:
  - New test: `TestOneRunHudShowsCurrentObjectiveAndChinesePhase`, `1/1 passed`, jobId `fea7db19`.
  - Regression tests all passed: `TestOneRunVisualReadabilityScaffold` (`74a5e147`), `TestShelterInteractionShowsVisibleFeedbackText` (`c54a2bb6`), `TestQimianLogButtonOpensLockedLogPanel` (`11e76358`), `TestShelterFacilityVisualsExposeBuildUseAndDamageState` (`832503fc`), `TestDossierButtonOpensEmptyDossierPanel` (`723fcbd7`).
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `46/46 passed`, jobId `3bd30e20`.
  - Verification caveat: 1 new `[Test]` added, but class-level regression still reports `46/46`; the new test was run by exact method name and passed.
  - Play hierarchy verified via screenshot: `BeyondSafeZoneUnity/Assets/Screenshots/u001_onerunmain_play.png`.
  - Console during Play: `logs: 7`, `warnings: 0`, `errors: 0`.
- SHELTER-002 verified on 2026-06-05:
  - Red tests before implementation: `TestShelterFacilityVisualsExposeBuildUseAndDamageState`, `0/1 passed`, jobId `2a35082b`; `TestShelterInteractionShowsVisibleFeedbackText`, `0/1 passed`, jobId `46609ee0`.
  - Compile feedback: `OneRunGameController.cs`, `ShelterInteractable.cs`, and `TestGameSimulation.cs` all returned `errorCount: 0`.
  - Target tests after implementation: `TestShelterFacilityVisualsExposeBuildUseAndDamageState`, `1/1 passed`, jobId `5d7fbb73`; `TestShelterInteractionShowsVisibleFeedbackText`, `1/1 passed`, jobId `468e7f9b`.
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `46/46 passed`, jobId `3a2b6a84`.
  - Verification caveat: this source file now has 2 new `[Test]` methods, but Unity Test Runner class-level regression still reported `46/46`; both new tests were run by exact method name and passed.
  - Play hierarchy verified `WalkableShelterGreybox/Facility_bed/Blueprint_bed`, `WalkableShelterGreybox/Facility_workbench/Built_workbench`, and `WalkableShelterGreybox/Facility_radio/Feedback_radio`.
  - Play screenshot evidence: `BeyondSafeZoneUnity/Assets/Screenshots/shelter002_onerunmain_play.png`.
  - Clean Play Console after separating test and Play verification: `warnings: 1`, `errors: 0`. A prior 2-error Console state came from starting Unity Test Runner during Play mode, not from project gameplay scripts.
- VIS-001 verified on 2026-06-05:
  - `OneRunMain` readability pass added HUD safe-frame and panel roots: `ReadabilitySafeFrame`, `StatusPanel`, `LogPanel`, and `PromptPanel`.
  - Camera now uses a low-contrast solid-color background and `orthographicSize = 4.9`.
  - Facility world labels stay short; detailed facility prompts remain in the bottom prompt panel.
  - Lin Xing sprite sorting order is higher than greybox facilities so the player remains visible.
  - Verification caveat: early VIS-001 red-test runs reported stale results that did not match current source; after `asset_refresh` and compile feedback, verification continued with the synchronized test assembly.
  - Compile feedback: `OneRunGameController.cs`, `ShelterInteractable.cs`, and `TestGameSimulation.cs` all returned `errorCount: 0`.
  - Target test: `TestOneRunVisualReadabilityScaffold`, `1/1 passed`, jobId `c23b9140`.
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `46/46 passed`, jobId `ea92bf08`.
  - Play hierarchy verified `OneRunHUD/ReadabilitySafeFrame`, `OneRunHUD/StatusPanel`, and `OneRunHUD/PromptPanel`.
  - Unity Console after Play verification: `warnings: 1`, `errors: 0`.
  - Screenshot evidence: `BeyondSafeZoneUnity/Assets/Screenshots/vis001_onerunmain_play.png`.
- FIX-PLAYER-001 verified on 2026-06-05:
  - User-provided Lin Xing image was copied into the Unity project at `BeyondSafeZoneUnity/Assets/Resources/Sprites/Characters/lin_xing_player.png`.
  - Source image dimensions checked locally: `76x116`.
  - Import settings verified through UnitySkills: `textureType: Sprite`, `filterMode: Point`, `mipmapEnabled: false`, `spritePixelsPerUnit: 64`.
  - Red test before implementation: `TestOneRunShelterUsesSideViewCutawayController`, `0/1 passed`, jobId `307318d1`.
  - Compile feedback: `SideViewShelterPlayerController.cs` and `OneRunGameController.cs` both returned `errorCount: 0`.
  - Target test after implementation: `TestOneRunShelterUsesSideViewCutawayController`, `1/1 passed`, jobId `2e42660a`.
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `46/46 passed`, jobId `909ee05b`.
  - Play hierarchy verified `WalkableShelterGreybox/LinXing_Player` and `BeyondSafeZone.Player.SideViewShelterPlayerController`.
  - Unity Console after Play verification: `warnings: 1`, `errors: 0`.
- SHELTER-001 verified on 2026-06-05:
  - Red test before implementation: `TestDossierButtonOpensEmptyDossierPanel` with cutaway shelter assertions, `0/1 passed`, jobId `504c0b9a`.
  - Compile feedback: `OneRunGameController.cs`, `SideViewShelterPlayerController.cs`, `ShelterStairZone.cs`, and `TestGameSimulation.cs` all returned `errorCount: 0`.
  - Target tests passed: `TestDossierButtonOpensEmptyDossierPanel`, jobId `99496be6`; `TestOneRunShelterUsesSideViewCutawayController`, jobId `144087ba`.
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `46/46 passed`, jobId `c63a3b85`.
  - Play hierarchy verified `CutawayShelterFrame`, `ShelterFloor_Ground`, `ShelterFloor_Upper`, `Stairs_GroundToUpper`, six `State_*` facility visuals, and `LinXing_Player` with `SideViewShelterPlayerController`.
  - Unity Console after Play verification: `warnings: 1`, `errors: 0`.
- U-008 verified on 2026-06-05:
  - Red test before implementation: `TestOneRunControllerExposesQimianLogPanelActions`, `0/1 passed`, jobId `e6df5b15`.
  - Compile feedback: `OneRunGameController.cs` `errorCount: 0`; `TestGameSimulation.cs` `errorCount: 0`.
  - New tests passed: `TestQimianLogButtonOpensLockedLogPanel`, jobId `7ec15e49`; `TestOneRunControllerExposesQimianLogPanelActions`, jobId `6413a695`.
  - Full EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `45/45 passed`, jobId `356bd9ec`.
  - Play hierarchy verified `OneRunHUD/QimianLogButton`, `QimianLogPanel`, `QimianLogTitle`, `QimianLogBody`, and `CloseQimianLog`; panel defaults hidden.
  - Unity Console after Play verification: `warnings: 1`, `errors: 0`.
- ENV-001 environment recovery verified on 2026-06-05:
  - UnitySkills actual REST endpoint is `http://127.0.0.1:8090`, not `42610`.
  - `/health` returned UnitySkills `2.0.1`, Unity `2022.3.62f3c1`, `currentMode: bypass`, and `serverRunning: true`.
  - `project_get_info` returned `projectPath: E:/Download/working/BeyondSafeZone/BeyondSafeZoneUnity/Assets`.
  - Active scene verified as `Assets/Scenes/OneRunMain.unity`.
  - Play hierarchy contained `OneRunBootstrap`, `Main Camera`, `WalkableShelterGreybox`, `WalkableShelterGreybox/LinXing_Player`, `EventSystem`, and `OneRunHUD`.
  - Play screenshot evidence: `BeyondSafeZoneUnity/Assets/Screenshots/env001_onerunmain_play.png`.
  - Unity Console: `warnings: 1`, `errors: 0`.
  - EditMode regression: `BeyondSafeZone.Tests.TestGameSimulation`, `43/43 passed`, jobId `9aa14f2b`.
- Latest recorded Unity EditMode full regression:
  - Test assembly: `BeyondSafeZone.Tests.TestGameSimulation`
  - Latest result: `46/46 passed`
  - jobId: `3bd30e20`
- Latest known Unity Console caveat:
  - `warnings: 0`
  - `errors: 0`

## Current Risks And Gotchas

- After ARCH-001, Unity Test Runner discovery may keep stale `TestGameSimulation.*` test names even though the source has moved them to `TestOneRunUI` and `TestOneRunWorld`. If class-level `TestGameSimulation` shows 8 failures for moved UI/world tests, do not edit gameplay code first; restart Unity Editor or clear test discovery cache, then rerun the three split classes.
- The sibling folder `E:\Download\working\BeyondSafeZoneUnity` was partially deleted but still has locked Unity generated folders (`Library`, `Logs`, `Temp`). Close any Unity Editor instance using that folder before retrying deletion.
- UnitySkills or Unity Editor may still point to the deprecated sibling project if the user opened the wrong directory. Always verify project path before Unity automation.
- UnitySkills port is not fixed on this machine. On 2026-06-05 it was `8090`; if another documented port fails, read `Editor.log` or the UnitySkills panel before diagnosing the plugin as missing.
- Current UI is greybox. Do not describe it as polished or final art.
- Current `OneRunMain` greybox is more readable after VIS-001, but it is still a functional blockout rather than a final UI/art pass.
- Current Lin Xing sprite is a prototype single-image character. It is not a final 32x32 animated character sheet, and its public contest license still needs review.
- Formal pixel art, complete second run, complex dice/action points, long NPC cooperation, and replay animations are future scope unless explicitly selected.
- `PlayKitNarrativeService` is a narrative enhancement layer only; core rules remain local deterministic code.
- Never commit tokens, local env files, Unity generated folders, builds, logs, or temporary files.

## Current Docs To Read First

- `AGENTS.md`
- `HANDOFF.md`
- `docs/CROSS_LANE_LOG.md`
- `docs/MINIMUM_DEMO_SCOPE.md`
- `docs/UNITY_STATUS.md`
- `docs/开发任务拆解.md`
- `docs/planning_package/README.md`

## Next Suggested Tasks

- Continue UI clarity work from `U-002` to `U-006`, especially location costs, disabled action reasons, and marker entry.
- Improve playable shelter/scavenging loop inside `OneRunMain`, but only after writing a short spec.
- Prepare fresh screenshots/video from current Unity greybox if moving toward contest submission.

## 2026-06-05 Notes

- Current cleanup removed redundant/outdated current-facing Godot and migration wording from main docs and marketing materials.
- `docs/CROSS_LANE_LOG.md` and this file were compacted so future sessions do not inherit obsolete Godot implementation history as current truth.
- Marketing files now align to current Unity greybox: they emphasize the clinic AI causal chain, dossier panel, and hidden protagonist AI, while marking broader systems as future scope.
- ENV-001 found that the project was not deleted and `OneRunMain` was not empty; the earlier blank Play concern is consistent with opening the wrong scene or checking the wrong REST port. Verified Play objects, screenshot, Console, and EditMode tests through UnitySkills.
- U-008 added a runtime Qimian log panel to `OneRunMain`. It does not change AI rules; it only surfaces the existing reveal text through HUD UI.
- SHELTER-001 replaced the one-run shelter presentation with a side-view cutaway greybox inspired by This War of Mine shelter readability. This is now the near-term visual direction for the home base; scavenging remains top-down for now.
- FIX-PLAYER-001 replaced the one-pixel Lin Xing runtime block with the imported prototype sprite and made shelter movement more forgiving by reading direct `A/D`, arrow-key, `W/S`, and stair input keys. In the shelter, `W/S` only changes floors while near the stair trigger; normal movement remains horizontal.
- VIS-001 made `OneRunMain` more readable without changing gameplay rules: HUD text now sits on panels, camera/background are less noisy, world labels are short, and the player sprite renders in front of facilities. Next visual step should be facility shape/state clarity rather than more HUD rearrangement.
- SHELTER-002 made shelter facilities easier to read and use without changing economy rules: blueprints indicate unbuilt objects, built layers indicate completed facilities, used markers show today's interaction, the barricade has a low-door damage marker, and each facility can show its latest action result as nearby world text.
- U-001 added a runtime objective panel to `OneRunMain`. The HUD now shows a Chinese phase label (清晨/白天/搜刮中/黄昏/夜晚/结尾揭示), a fixed `当前目标` title, and 1-2 lines of context-aware objective text that changes by day number, anomaly state, and mark existence. No gameplay rules were changed; the panel reads existing state only.

## 2026-06-06 Notes

- A-FIX-003 follow-up on 2026-06-06: After B-line work, a later full EditMode run reported 3 failures: `TestExplorationActionAvailabilityAllowsLureAndLeaveWhenSearching`, `TestShelterFacilityPromptShowsUnavailableReason`, and `TestShelterPromptShowsPhaseAndResourceReasons`. Root cause was stale test expectations, not new production rule changes. `convenience` only has `storefront` / `warehouse`, so the exploration availability alias check was changed from nonexistent-in-location `checkout` to `warehouse`. UI shelter prompt tests now match the A-001 phase policy: `morning` / `day` are allowed for shelter action availability because `OneRunGameController` uses `EnsureShelterActionPhase()` before executing; `searching` remains blocked and still shows the phase failure reason. Modified only `TestGameSimulation.cs` and `TestOneRunUI.cs`. Verification so far: `debug_check_compilation` reports `isCompiling=false`, `isUpdating=false`; `debug_get_errors` reports `count=0`. UnitySkills is currently in `auto`, so `test_run_by_name` returns `MODE_FORBIDDEN`; target/full EditMode verification still needs Bypass mode before this can be marked complete.
- C-001 added shelter facility highlight feedback on player approach. `ShelterInteractable.SetHighlighted(bool)` toggles a 1.5x brightness + 0.08 alpha tint on the facility's `stateRenderer` without changing GameState. `SideViewShelterPlayerController` calls highlight on `OnTriggerEnter2D` (near facility) and `OnTriggerExit2D` (leave). New test `TestShelterInteractableHighlightOnApproach` verifies color change and restoration. No changes to `GameSimulation`, `GameState`, or `OneRunGameController.cs`.
- C-FIX-001 added real trigger-path test `TestSideViewShelterPlayerControllerSwitchesHighlightOnTriggerOverlap` to `TestOneRunWorld.cs`. The test invokes `OnTriggerEnter2D` / `OnTriggerExit2D` on `SideViewShelterPlayerController` via reflection using real `Facility_radio` / `Facility_stove` `Collider2D` objects, verifying highlight state and `State_*` `SpriteRenderer.color` through the full enter-A → enter-B → exit-A → exit-B sequence. No source changes to production code — purely testing what C-001 already implements. `TestOneRunWorld.cs` now has 8 `[Test]` methods.
- C-VERIFY-001 Unity EditMode regression: both C-FIX-001 target tests passed via `test_run_by_name`: `TestShelterInteractableHighlightOnApproach` (jobId `44691a33`, 1/1) and `TestSideViewShelterPlayerControllerSwitchesHighlightOnTriggerOverlap` (jobId `36b578d7`, 1/1). Console `errors: 2` are stale cache (Mono.Cecil + TMPro), not project code. `test_list` discovery still uses `unity_test_runner_async_cache` mode. Full EditMode `test_run` blocked in Auto mode (`MODE_FORBIDDEN`).
- A-FIX-003 on 2026-06-06 corrected A-003's day phase availability query to match current `OneRunGameController` handlers. `GameSimulation.CheckDayPhaseActionAvailability(state, actionId)` now only blocks `resolve_night` / `next_day` when `DemoComplete` is true; it no longer invents extra `night` / `reveal` / `searching` phase gates. `resolve_night` remains available in `searching` because the UI handler returns to shelter before resolving; `next_day` mirrors the current handler, which calls `StartDay` from any non-DemoComplete phase. Tests were renamed/updated to `TestDayPhaseActionAvailabilityMatchesCurrentResolveNightHandler` and `TestDayPhaseActionAvailabilityMatchesCurrentNextDayHandler`, covering `morning/day/evening/searching/night/reveal`. Read-only coverage now includes Resources, Lin, Exploration, and Qimian key fields. Verified project path `E:/Download/working/BeyondSafeZone/BeyondSafeZoneUnity/Assets`; target tests passed (jobIds `59aa6aa3`, `496030ea`); full EditMode regression jobId `016cf322`: 130/130 passed, 0 failed.
- A-003 on 2026-06-06: Added read-only day phase action availability query. `GameSimulation.CheckDayPhaseActionAvailability(state, actionId)` returns `DayPhaseActionAvailability` covering `resolve_night` (phase-allowed except night/reveal; searching allowed with auto-return note; DemoComplete blocked) and `next_day` (morning/day/evening only; searching/night/reveal blocked; DemoComplete blocked). Five new tests. Full regression jobId `e8a23846`: 130/130 passed, 0 failed. Lint clean.
- A-FIX-002 on 2026-06-06: Fixed `TestExplorationActionAvailabilityBlocksSearchedOrLockedRoom` — split into two independent GameState instances because `EnterLocation` requires morning/day phase and can't be called mid-searching. Searched test uses existing `_state` with convenience; Locked test creates fresh `state2` via `NewGame()`, enters clinic, sets pharmacy.Locked=true, and asserts Phase/ActiveLocation correct. Full regression jobId `481edb84`: 130/130 passed, 0 failed.
- A-002 on 2026-06-06: Added read-only exploration action availability layer. `ExplorationController.CheckActionAvailability(state, actionId, roomId)` returns `ExplorationActionAvailability` covering `search_room`/`quick_search`/`careful_search`/`lure_room`/`leave_exploration`. Preconditions mirror `SearchRoom` (phase=searching, ActiveLocation set, roomId exists, not Searched, not Locked), `LureRoom` (phase+location+roomId), and `LeaveExploration` (phase+location). Five new tests added. Full EditMode regression jobId `4fbbe193`: 130/130 passed, 0 failed. Lint clean.
- A-FIX-001 on 2026-06-06: Fixed `TestCheckAvailabilityReturnsCorrectActionId` where `Parts = 0` incorrectly tried to gate `radio` availability. `radio_broadcast` resource gate is `Fuel` (`SHELTER_RADIO_FUEL = 1`), not Parts. Changed to `Fuel = 0`. Full EditMode regression via `test_run`, jobId `f313f1ac`: 130/130 passed, 0 failed. `test_run_by_name`/`test_get_result` have JSON parsing bug on this machine (`Invalid property identifier character: \\.`), but `test_run` + `test_get_last_result` works.
- A-001 review fix on 2026-06-06: (1) Phase gate relaxed from `only evening` to allow `morning`/`day`/`evening`, matching the existing `EnsureShelterActionPhase()` auto-transition in `OneRunGameController`. Only `searching`/`night`/`reveal` still block. (2) `workbench_car` availability now checks `Car.Found`, `Car.Ready`, and current repair step resource requirements (engine materials/parts, tire/parts, battery/fuel, gasoline) directly mirroring `CarController.Repair`. (3) Alias actions `repair_bike` and `radio` now preserve the original requested `ActionId` in the returned struct instead of overwriting it with the canonical name. Added 4 new tests for workbench_car and extended alias/phase tests. Total 53 `[Test]` methods in `TestGameSimulation.cs`.
- U-004 added location info cards to `OneRunMain` HUD. Three cards appear above the location buttons showing resource tendency, danger level, anomaly status, and dynamic player mark / Qimian trace updates. During implementation, a `replace_in_file` edit accidentally overwrote `BuildObjectivePanel` (removed method body while inserting `BuildLocationCards`); this was caught by the compile error `CS0103` and fixed by restoring the method definition before `EnterScavengeLocation`.
- U-004 v2 review fix: `GetLocationCardInfo("clinic")` was originally showing `异常：暂无` for the clinic's initial state, but the review required `异常：待调查` (clinic has narrative reason for investigation). Fixed by making anomaly text location-dependent: clinic → `待调查`, others → `暂无` when no QimianTrace. Test `TestOneRunHudShowsLocationCards` updated to assert `待调查`.
- ARCH-001 split Unity tests for parallel code work: core rule tests remain in `TestGameSimulation.cs`, UI tests moved to `TestOneRunUI.cs`, world/shelter tests moved to `TestOneRunWorld.cs`, and shared helpers moved to `OneRunTestHelpers.cs`. Exact-method verification passed for representative tests, but Unity Test Runner discovery cache still reports old `TestGameSimulation.*` names; restart Unity before treating class-level regression as authoritative.
- B-001 made HUD action buttons and shelter facility prompts show unavailable reasons. `ShowPrompt(facilityId)` now calls `GameSimulation.CheckShelterActionAvailability` and replaces the facility prompt with the failure reason when the action is unavailable. Five button handlers (`EnterScavengeLocation`, `ReturnToShelter`, `LeaveHelpMarkAtActiveLocation`, `ResolveNight`, `NextDay`) now `Report()` the reason instead of silently returning. Three new tests added to `TestOneRunUI.cs` for a total of 9 `[Test]` methods: prompt phase/reason feedback, button-click log output, and resource-specific prompt reasons. Lint clean on all three modified files. No changes to core rules, World scripts, or other non-UI files.
- B-FIX-001 on 2026-06-06 corrected B-001's button approach: B-001 used `interactable=false` for unavailable buttons, but real users can't click disabled buttons. B-FIX-001 switched to 方案二 — all action buttons stay `interactable=true`, unavailable ones get visual dimming via `SetButtonVisual()` (Image.color alpha lowered 0.92→0.50, RGB darkened). Refactored `RefreshAll()` to use `SetButtonVisual()` instead of `interactable=false`. New test `TestHudUnavailableButtonsRemainClickableAndReportReasons` asserts `Button.interactable==true` in all 5 unavailable scenarios, uses real `onClick.Invoke()`, and verifies log text. TestOneRunUI: 9 `[Test]`. Verified: bypass mode, 4 tests all 1/1 passed (jobIds: a23f4755, 2e99862e, 830e7507, b6746c28). Lint clean.
