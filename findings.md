# Findings: Planning Package Consolidation

## Current Canon

- Game title: 《保护区之外》 / Beyond Safe Zone.
- Engine direction: Unity main development target; Godot 4.6.2 greybox remains the rule/data/text/test reference.
- Near-term demo target: 10-15 minute minimum playable vertical slice inside the 15-day narrative frame.
- Blood moon days: Day 7 and Day 15.
- Red tide nights: Day 11-14 remain background/future pressure content for the current minimum slice.
- Current P0 locations: Lin Xing home/shelter, community clinic, neighborhood supermarket, repair shop/garage.
- Reference data exists for 14 top-level locations with 40+ room entries, but full production of those locations is future scope.
- Evacuation target: old car, not bicycle. Bicycle is near/mid-range exploration transport.
- Qimian wakes on Day 5 and acts as hidden AI in first run.
- AI flow: perceive, collect tasks, rank/select, execute, update shared world, log replay.
- Full second-run Qimian gameplay is future scope.

## Existing Document Situation

- `marketing/介绍.md` is useful as external pitch copy, not GDD.
- `docs/archive/legacy_design/ONE_PAGE_GDD.md` is closer to a development one-page but should become part of a package or redirect to it.
- `docs/archive/legacy_design/策划案.md` contains valuable detail but is too large and historically mixed.
- `docs/reference/DEMO_SCOPE.md`, `docs/reference/共享地图状态API.md`, `docs/reference/祁眠AI决策伪代码.md`, `docs/reference/15天逐日事件表.md`, `docs/reference/地点结构化数据.md`, and `docs/reference/祁眠事件关卡布局.md` should be referenced as detailed sources.
- README and multiple marketing files still contain stale 14-day/bike-ready/9-location statements.

## First-Run AI Interaction Design

- Problem: If Qimian AI only creates "random-looking" results that are explained at the ending, the first run feels passive and relies too much on art/atmosphere.
- Proposed fix: make first-run AI readable and lightly interactive through two systems:
  - Anomaly investigation: anomalies become explicit location states and update an "unknown actor dossier".
  - Indirect markers: Lin Xing can leave danger/help/route/supply markers that Qimian AI can perceive at night.
- Design principle: Player does not directly control Qimian, but can alter Qimian's perceivable input through in-world traces.
- Minimum demo chain: Lin Xing discovers clinic anomaly, leaves a help marker, Qimian reads it, leaves medicine or changes route, and next morning the dossier updates.

## Development Task Breakdown

- `docs/开发任务拆解.md` is the current role-based task list for recruiting and sprint planning.
- The programming P0 list should start with the playable first-run AI chain: anomaly dossier, indirect markers, Qimian reading marks, clinic feedback, node-map hints, ending-log explanation, and tests.
- Action points/dice and NPC cooperation are useful responses to teacher feedback, but they need detailed rules before implementation. They are listed as P1 mechanism additions, not current greybox features.
- Art P0 should prioritize readable silhouettes and UI icons for the AI chain: anomaly dossier, four marker icons, anonymous medicine, clinic/supermarket/repair shop/bridge, Lin Xing, Qimian, and zombies.

## Document Organization

- Active project entry files should stay at the root or top of `docs/`: `HANDOFF.md`, `README.md`, `docs/planning_package/README.md`, `docs/UNITY_MIGRATION_PLAN.md`, `docs/UNITY_MIGRATION_STATUS.md`, `docs/开发任务拆解.md`, `docs/ASSET_PIPELINE.md`, `docs/ASSET_LICENSE_LOG.md`, `docs/CROSS_LANE_LOG.md`, `docs/PROJECT_MEMORY.md`, and `docs/DECISIONS.md`.
- `docs/reference/` is for detail files that still inform implementation: day table, location data, shared map API, Qimian AI pseudocode, Qimian level layouts, and demo scope.
- `docs/archive/` is for old entry points, historical analysis, technical reports, prototypes, media, and temporary images.
- `marketing/介绍.md` holds the external pitch introduction; it is not the GDD or implementation source of truth.

## Minimum Demo Scope Lock

- `docs/MINIMUM_DEMO_SCOPE.md` is the authoritative near-term scope document.
- The minimum slice proves one chain: Lin Xing discovers an unknown-actor anomaly, leaves a marker, Qimian AI reads it at night, the next day map/dossier feedback changes, and the ending log explains the decision.
- Active planning docs should describe full 14-location content, 40+ rooms, full second run, Qimian 9 side-view missions, complex dice/action-point systems, long NPC cooperation, and replay animations as future scope.
- The next implementation work should not start by expanding content volume. It should first verify Unity compilation and implement the clinic AI chain.
