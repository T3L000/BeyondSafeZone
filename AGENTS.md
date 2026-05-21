# Agent Coordination Rules

This project uses multiple focused conversations for code, design, art, and contest submission work.

## Start Here

Every new agent session must read `HANDOFF.md` before editing or planning. If the session belongs to a specific lane, also read that lane's primary files listed in `HANDOFF.md`.

## Persistent Memory

Important information must be written to disk before the session ends:

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
