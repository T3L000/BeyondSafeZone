# Progress: Planning Package Consolidation

## 2026-05-30

- Read startup coordination files: `AGENTS.md`, `HANDOFF.md`, `docs/CROSS_LANE_LOG.md`, `docs/PROJECT_MEMORY.md`.
- Reviewed relevant current docs and code data.
- Confirmed stale statements via `rg`: README and marketing files still mention 14-day scope, 9 locations, bike-ready evacuation, and Day 14 blood moon in places.
- Created plan tracking files for this consolidation task.
- Created `docs/planning_package/` with README, planning overview, design brief, GDD, and detailed design files.
- Updated `README.md` current status from stale 14-day/bike-ready/9-location wording to 15-day/car-ready/14-location wording.
- Added maintenance-entry notices to `docs/archive/legacy_design/ONE_PAGE_GDD.md`, `docs/archive/legacy_design/策划案.md`, and `docs/reference/DEMO_SCOPE.md`.
- Encountered one patch context mismatch on `docs/archive/legacy_design/策划案.md`; resolved by splitting the patch.
- Updated marketing files to current 15-day/14-location/car-evacuation wording.
- Corrected the most misleading current-facing stale lines inside `docs/archive/legacy_design/策划案.md` while keeping it as a historical detail source.
- Added stable decision that `docs/planning_package/` is the canonical current planning entry.
- Updated cross-lane log and project memory with the planning package consolidation summary.
- Updated `HANDOFF.md` so future sessions read `docs/planning_package/` as the current design entry.
- Started adding first-run readable AI interaction system after user approved the direction: anomaly investigation + indirect markers.

## 2026-05-31

- Completed Phase 5: added first-run readable AI interaction as a concrete design system.
- Updated `docs/planning_package/03_系统策划案_GDD.md` with the first-run loop, `3.10 异常调查系统`, `3.11 隔空标记系统`, UI surfaces, story beats, and asset needs.
- Updated `docs/planning_package/04_详细策划案.md` with modules, state fields (`state.anomaly_dossier`, `location.anomaly_tags`, `location.player_mark`, `location.player_mark_day`, `location.player_reserved_resources`), mechanism flows, UI requirements, scene priorities, log requirements, and test checks.
- Updated `docs/archive/legacy_design/ONE_PAGE_GDD.md` and `docs/reference/DEMO_SCOPE.md` so the quick GDD and scope doc explicitly include "异常调查 + 隔空标记".
- Updated `docs/reference/共享地图状态API.md` with shared-map fields, settlement order, and the clinic help-marker-to-anonymous-medicine example.
- Updated `docs/reference/祁眠AI决策伪代码.md` so `player_marks`, `reserved_resources`, and `anomaly_traces` enter `perceivable_state`, affect task scoring, and appear in `world_trace_input` logs.

## 2026-06-02

- Completed Phase 6: created `docs/开发任务拆解.md` as a role-based task breakdown for recruiting and production handoff.
- Covered programming modules, art assets, UI/UX, design/level work, audio, QA, contest/recruiting materials, target teammates, and next recommended schedule.
- Checked exact current implementation fields with `rg`: `state.anomaly_dossier` and `state.player_marks` exist in `game/scripts/model/game_state.gd`; shared-map fields such as `anomaly_tags`, `player_mark`, `player_mark_day`, `player_reserved_resources`, and `world_trace_input` are documented in planning/API files.
- Adjusted the breakdown so "异常调查 + 隔空标记" remains the P0 AI gameplay chain, while action points/dice and NPC cooperation are marked as suggested mechanism additions pending detailed rules.

## 2026-06-03

- Started Phase 7: organized project documents after the Unity direction change.
- Moved detail/reference docs into `docs/reference/`: `DEMO_SCOPE.md`, `15天逐日事件表.md`, `地点结构化数据.md`, `共享地图状态API.md`, `祁眠AI决策伪代码.md`, and `祁眠事件关卡布局.md`.
- Moved legacy or temporary materials into `docs/archive/`: old design entries, historical analysis reports, technical report, greybox HTML prototype, media files, and temporary images.
- Moved external pitch copy from root `介绍.md` to `marketing/介绍.md`.
- Updated live references in `HANDOFF.md`, `README.md`, `docs/planning_package/README.md`, `docs/planning_package/03_系统策划案_GDD.md`, `docs/DECISIONS.md`, and `docs/开发任务拆解.md`.
- Added `docs/reference/README.md` and `docs/archive/README.md` to explain the new document layout.
- Attempted to remove empty `docs/superpowers/specs`; PowerShell reported access denied, so the empty folder was left in place.
- Completed Phase 8: locked the near-term minimum demo scope in `docs/MINIMUM_DEMO_SCOPE.md`.
- Updated `docs/DECISIONS.md` with `Minimum Demo Scope Lock`.
- Updated `HANDOFF.md` so new sessions prioritize Unity compilation and the 4-location clinic AI chain.
- Updated `docs/planning_package/01_策划总纲.md`, `02_策划概要案.md`, and `04_详细策划案.md` so full 14-location production, full second run, complex dice/action-point systems, long NPC cooperation, and replay animations are marked as future scope.
- Fixed duplicate programming task IDs in `docs/开发任务拆解.md`: P0 is `C-001` to `C-010`, P1 is `C-011` to `C-018`, and P2 is `C-019` to `C-022`.
- Updated `docs/CROSS_LANE_LOG.md`, `docs/PROJECT_MEMORY.md`, `task_plan.md`, and `findings.md` with the minimum-scope decision.
