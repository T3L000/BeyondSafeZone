# Task Plan: Planning Package Consolidation

## Goal

Create a consolidated planning package for 《保护区之外》 that matches the requested course-style deliverables:

1. 策划总纲
2. 策划概要案
3. 系统策划案 / GDD
4. 详细策划案

The package must reflect current project facts: 15-day demo, 14 locations, car evacuation, Qimian hidden AI, shared map, and limited second-run scope.

## Phases

| Phase | Status | Work |
|------|--------|------|
| 1 | complete | Read project handoff, cross-lane log, memory, current docs, and code data. |
| 2 | complete | Create centralized planning package files under `docs/planning_package/`. |
| 3 | complete | Update redundant/old entry files to point at the new package and fix stale 14-day public-facing statements. |
| 4 | complete | Verify created files and record cross-lane/project memory updates. |
| 5 | complete | Add first-run readable AI interaction system: anomaly investigation + indirect markers. |
| 6 | complete | Break the planning package into role-based development tasks for programming, art, UI/UX, design, audio, QA, and recruiting. |
| 7 | complete | Organize current documents: keep active entry files visible, move legacy/detail files into `docs/reference/` or `docs/archive/`, and update live references. |
| 8 | complete | Lock the near-term minimum demo scope: 10-15 minute vertical slice, 4 locations, clinic AI chain, and deferred full-scope systems. |

## Decisions

- Keep old long documents as detail/reference sources instead of deleting them.
- Make `docs/planning_package/README.md` the new design entry point.
- Treat second-run Qimian content as key playable chapters for contest demo, not a full second 15-day campaign.
- Keep stale historical notes in `docs/PROJECT_MEMORY.md` and `docs/CROSS_LANE_LOG.md` because they are chronological records; fix current-facing docs and package files.
- Add first-run AI gameplay as a concrete system: players discover anomalies, build an unknown-actor dossier, leave in-world markers/resources/routes, and Qimian AI reads those as perceivable input.
- Treat action points/dice and NPC cooperation as suggested mechanism additions until their detailed rules are approved; keep them separate from already implemented greybox features.
- Use `docs/reference/` for detail files that may still guide implementation, and `docs/archive/` for old entry points, reports, prototypes, and media that should not clutter the active docs folder.
- Treat `docs/MINIMUM_DEMO_SCOPE.md` as the authoritative near-term production scope. Full 14-location production, full second run, complex dice/action-point systems, long NPC cooperation, and replay animations are future scope until the minimum AI chain is playable.

## Errors Encountered

| Error | Attempt | Resolution |
|-------|---------|------------|
| Multi-file patch failed when matching `docs/策划案.md` header | 1 | Split into smaller patches and applied header notice successfully. |
