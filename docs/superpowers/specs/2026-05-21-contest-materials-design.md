# Contest Materials Design

Date: 2026-05-21

## Goal

Create a unified contest material package for `Beyond Safe Zone` so video scripts, AI usage explanations, pitch copy, PPT outlines, and screenshot planning all use the same truthful project positioning.

## Design

The contest lane will use `marketing/SUBMISSION_PLAN.md` as the master document. Derived files in `marketing/` will each handle one submission artifact:

- `DEMO_VIDEO_SCRIPT.md` for the 1-3 minute video.
- `AI_USAGE_STATEMENT.md` for the in-game AI feature and development tool usage.
- `PITCH_COPY.md` for short and long submission copy.
- `PPT_OUTLINE.md` for presentation structure.
- `SCREENSHOT_SHOTLIST.md` for capture planning.

## Cross-Conversation Memory

Future agents should discover the process from `AGENTS.md` and `HANDOFF.md`. Durable information goes into `docs/PROJECT_MEMORY.md`; stable decisions go into `docs/DECISIONS.md`.

## Truthfulness Rule

Contest materials must match the current demo. Implemented features can be stated directly; future features must be labeled as future scope.

## Review Notes

- No placeholder sections remain.
- The scope is limited to contest material planning and cross-session coordination.
- The design does not require changes to gameplay code.
