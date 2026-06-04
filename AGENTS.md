# Agent Coordination Rules

This project uses multiple focused conversations for code, design, art, and contest submission work.

## Cross-Lane Sync (MANDATORY)

This project uses the **cross-lane-sync skill** to coordinate parallel conversations.

**On startup** every session MUST:
1. Read `docs/CROSS_LANE_LOG.md` to check what other lanes changed since last session.
2. Check the "跨线阻塞/待同步项" table for unresolved cross-lane dependencies.

**On shutdown** (user-approved work complete) every session MUST:
1. Append a summary entry to `docs/CROSS_LANE_LOG.md` under the correct lane section.
2. Update `docs/PROJECT_MEMORY.md` with detailed session notes.
3. Update `docs/DECISIONS.md` if any stable decision was locked.

## Start Here

Every new agent session must read `HANDOFF.md` before editing or planning. If the session belongs to a specific lane, also read that lane's primary files listed in `HANDOFF.md`.

## Persistent Memory

Important information must be written to disk before the session ends:

- Update `docs/CROSS_LANE_LOG.md` with a brief summary of changes and cross-lane impact.
- Update `docs/PROJECT_MEMORY.md` with dated discoveries, decisions, completed work, risks, and next steps.
- Update `HANDOFF.md` only with information a fresh session needs immediately.
- Update `docs/DECISIONS.md` when a stable project or submission decision is made.
- Update the lane-owned plan or material file, such as `marketing/SUBMISSION_PLAN.md` for contest work.

## Lane Ownership

- Code lane owns `game/scripts/**`, `game/tests/**`, and `game/scenes/**`.
- Design lane owns narrative and scope docs in `docs/`.
- Art lane owns `assets/` and `docs/ASSET_PIPELINE.md`.
- Contest lane owns `marketing/`, contest submission copy, AI usage explanation, video scripts, and submission checklists.

Avoid editing outside the active lane unless the user explicitly asks or the change is needed to keep shared docs consistent.

## Contest Truthfulness

Contest-facing text must match the current demo. Do not claim implemented features unless they exist in the Godot project or are clearly labeled as planned.
