# AI Usage Statement

## AI Feature In The Game

The main AI design in `Beyond Safe Zone` is Qimian, a hidden protagonist controlled by a deterministic AI decision layer driven by a personality card.

### How It Works

1. **Personality Card**: At game start, Qimian receives a fixed personality card defining his goals, behavioral tendencies, moral rules, resource habits, and safe-zone attitude. The default demo card: main goal = find Qijin; cautious/avoid exposure; help nearby people without mass rescue; take only needed resources; observe the safe zone but distrust screening.

2. **Decision Engine**: The personality card compiles into deterministic decision rules — not probability rolls. The same card in the same world state produces the same class of action.

3. **Perceptible State Only**: Qimian's AI input is strictly limited to what Qimian can see, hear, or infer from traces. It cannot read Lin Xing's hidden state, other survivors' backend state, or whether Lin Xing explored a location.

4. **Sequential Shared Map**: Lin Xing's daytime actions modify shared location state → Qimian's nighttime AI reads and further modifies it → next day Lin Xing sees the consequences. This is not full real-time co-presence; it is one set of nodes with sequential time-slot settlement — sufficient for the contest scope.

5. **Visible Anomalies**: From days 5-6 onward, the player encounters recurring world anomalies (precisely taken supplies, unlocked doors, diverted zombies, anonymous medicine). Affected locations show `qimian` trace icons on the node map, teaching the player that a second actor exists before the final reveal.

6. **[planned] First-Run Readable AI Interaction**: The design adds anomaly investigation and indirect markers. Lin Xing can record anomalies in an unknown-actor dossier and leave danger/help/route/reserved-supply marks. Qimian can perceive those marks as world traces, but they do not become direct player commands.

7. **Demo-End Reveal**: After the 15-day demo ends (Lin Xing reaches the safe-zone gate and enters quarantine), Qimian's full action log is unlocked. Each entry contains: AI replay (input state, rule filtering, decision path), shared-world impact, and subjective fragment (1-2 lines of Qimian's perspective).

### Why This Matters

The AI is not a decorative chatbot. It is part of the simulation:

- It has constrained action choices based on personality rules.
- It modifies shared world state through a traceable settlement pipeline.
- It creates mystery during the first run (midgame anomalies consistently appear).
- It can support first-run player inference and indirect influence through world markers. `[planned]`
- It becomes explainable through a post-demo log (AI replay + subjective fragments).
- It supports replay by turning "random events" into traceable hidden intent.

## AI Tool Usage During Development

### CodeBuddy Usage Summary

Development used CodeBuddy across multiple lanes under the cross-lane-sync coordination protocol:

| Area | What CodeBuddy Did |
|------|-------------------|
| **Planning / Design** | Analyzed full project documentation; generated master planning report identifying inconsistencies (day count drift, missing daily event table, Qijin Demo decision); created structured development requirements per lane. |
| **Code Implementation** | Implemented Godot 4.6.2 GDScript simulation: day/night loop, 15-day event table, node-based overworld (14 locations), indoor search greybox (room cards, search/lure, hidden zombie risk, overstay fatigue), infection/medicine loop (readable stages, dangerous infection pressure, wound treatment), 5 shelter facilities, 6 core resources, car evacuation condition checks, shared map with Qimian trace icons, 3 endings, and full Qimian AI action log with AI replay + subjective fragments. |
| **Testing** | Wrote and maintained `test_game_simulation.gd` covering all simulation behaviors; ran headless Godot verification after each code slice; all tests currently pass. |
| **Documentation** | Generated and maintained cross-lane coordination files: `AGENTS.md`, `HANDOFF.md`, `CROSS_LANE_LOG.md`, `DECISIONS.md`, `PROJECT_MEMORY.md`; maintained lane-specific docs under `docs/` and `marketing/`. |
| **Contest Materials** | Drafted and refined all marketing files: `SUBMISSION_PLAN.md`, `DEMO_PITCH.md`, `PITCH_COPY.md`, `PPT_OUTLINE.md`, `DEMO_VIDEO_SCRIPT.md`, `SCREENSHOT_SHOTLIST.md`, `AI_USAGE_STATEMENT.md`. |
| **Cross-Lane Sync** | Implemented `cross-lane-sync` skill enforcing startup/shutdown protocols across Code/Design/Art/Contest lanes to prevent inter-lane drift. |

### Development Workflow

1. Human defines scope, narrative direction, and design decisions.
2. CodeBuddy assists with GDScript implementation, test writing, documentation, and contest materials.
3. Human reviews all AI-assisted output for correctness and alignment with demo truth.
4. All simulation tests are verified with headless Godot before claiming code completion.

### Tools Used

- **CodeBuddy** (IDE-integrated AI): Primary development assistant for planning, code, tests, docs, and contest materials.
- **Godot 4.6.2**: Game engine.
- **FrameRonin** (planned for pixel art production): Video-to-frames, Sprite Sheet organization, GIF previews. Not yet used for production assets.

### Important Notes

- All AI-generated or AI-assisted content was reviewed by the human developer before inclusion.
- Contest materials strictly align with the current Godot greybox demo state. Planned features are explicitly labeled as `[planned]` or `future scope`.
- Game AI (Qimian's personality engine) is designed as a deterministic rule system, not a large language model or generative AI.
