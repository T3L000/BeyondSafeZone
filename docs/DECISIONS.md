# Decisions

This file records stable decisions for BeyondSafeZone. Add entries only when the decision should guide future sessions.

## 2026-06-04

### Unity Main Scene: OneRunMain Is The Formal First-Run Scene

- `Assets/Scenes/OneRunMain.unity` is the formal Unity scene for the first-run Lin Xing chapter.
- `Assets/Scenes/MainPrototype.unity` remains only as a temporary greybox/reference scene.
- New playable Unity work should continue through `OneRunGameController`, `GameSimulation`, `GameState`, and the existing controller/data architecture.
- The formal first-run scene should prioritize verified P0 interaction chains over expanding more locations or polish.
- Current verified chain: `OneRunMain` Play mode generates the walkable shelter and HUD, `去诊所` creates `ScavengeGreybox_clinic`, and `留下求助` writes visible help-mark feedback for the clinic.

### Delivery Discipline: Structured Scope, Single-Task Loop, Verifiable Interaction Chains

- Near-term implementation must be driven by structured docs instead of freeform feature discussion.
- `docs/MINIMUM_DEMO_SCOPE.md` answers whether a feature belongs in the minimum slice.
- `docs/开发任务拆解.md` is the task entry for selecting exactly one active task number at a time.
- `docs/UNITY_MIGRATION_STATUS.md` is the required place to record Unity-side verification, blockers, and refinement notes.
- P0 work must follow `Plan -> Build -> Test -> Refine`, with one explicit task number or one explicit interaction chain per work cycle.
- No P0 feature enters implementation unless its short structured spec is clear: trigger condition, player action, state change, visible feedback, and verification method.
- A task is complete only when all three are true: docs updated, implementation landed, verification recorded.
- Progress is judged primarily by verifiable interaction chains, not by feature count. The key minimum chains are Day 1 explore/night resolve, Day 5 anomaly awareness, Day 6 clinic marker, Day 7 feedback, and Day 15 log reveal.

## 2026-06-02

### Engine Direction: Unity Migration And PlayKit.ai SDK

- The main development direction is now Unity, not Godot.
- The Unity project target path is `E:\Download\working\BeyondSafeZoneUnity`.
- The existing Godot 4.6.2 project remains as a reference implementation for rules, data, text, tests, and greybox behavior.
- Future planning and implementation language should treat `docs/planning_package/` as the design source, but describe the implementation target as Unity.
- PlayKit.ai integration should use the Unity SDK. Godot SDK support is not treated as currently available for this project unless the user provides later evidence that it has shipped.
- PlayKit.ai should enhance narrative text: anomaly dossier entries, Qimian log text, NPC/dialogue, broadcast, and monologue variants.
- PlayKit.ai must not own core deterministic rules: resource math, damage, infection, car repair, blood moon resolution, endings, valid actions, or Qimian's local task selection.
- Developer Token or other secrets must not be committed, written into source constants, or shipped in a production build.

## 2026-06-03

### Minimum Demo Scope Lock

- The near-term production target is a 10-15 minute minimum playable vertical slice inside the 15-day narrative frame.
- `docs/MINIMUM_DEMO_SCOPE.md` is the authoritative near-term scope document.
- The minimum slice must prove one core experience: Lin Xing discovers an unknown-actor anomaly, leaves an in-world marker, Qimian AI reads that marker at night, the shared map/dossier changes the next day, and the ending log explains the decision chain.
- Near-term locations are limited to four core locations: Lin Xing home/shelter, community clinic, neighborhood supermarket, and repair shop/garage.
- The full 14-location map, 40+ rooms, full second-run Qimian campaign, Lin Xing AI takeover, Qimian 9 side-view missions, complex dice/action-point system, long NPC cooperation, and 5 replay animations are deferred to future scope.
- The teacher-suggested action-point/dice and NPC cooperation ideas remain useful design directions, but they should not enter P0 until a small rules spec exists and the minimum AI chain is playable.

### Document Layout: Active, Reference, Archive

- Active entry files stay visible at the repository root or top level of `docs/`.
- Current design work starts from `docs/planning_package/README.md`.
- Current Unity migration work starts from `docs/UNITY_MIGRATION_PLAN.md` and `docs/UNITY_MIGRATION_STATUS.md`.
- Role/task planning starts from `docs/开发任务拆解.md`.
- Detail files that still guide implementation live under `docs/reference/`.
- Old entry points, historical reports, prototypes, and media live under `docs/archive/`.
- External-facing intro copy lives at `marketing/介绍.md`, not the repository root.
- New sessions should avoid adding one-off docs to the root unless they are immediate onboarding files like `HANDOFF.md` or `README.md`.

## 2026-05-30

### First-Run Readable AI Interaction

- The first run must not rely only on ending revelation to make Qimian's AI feel meaningful.
- Add two concrete first-run systems:
  - **异常调查**: Lin Xing discovers AI-caused world anomalies and records them in an unknown-actor dossier.
  - **隔空标记**: Lin Xing can leave danger/help/route/reserved-supply marks in the world; Qimian AI reads them only as perceivable traces.
- These marks are not player commands. They modify Qimian's deterministic task ranking while Qimian's own personality card, exposure risk, zone heat, and Qijin goal still govern final decisions.
- Required shared-map fields include `anomaly_tags`, `player_mark`, `player_mark_day`, and `player_reserved_resources`.
- Required AI log field includes `world_trace_input` so the post-run reveal can show which player-visible traces affected Qimian's choice.
- Minimum demo chain: clinic anomaly → player leaves help marker → Qimian reads it at night → anonymous medicine / shallow arrow appears → unknown-actor dossier verifies the inference.

### Planning Package As Canonical Design Entry

- `docs/planning_package/` is now the canonical current planning entry for development, course review, and presentation preparation.
- The package contains:
  - `01_策划总纲.md`
  - `02_策划概要案.md`
  - `03_系统策划案_GDD.md`
  - `04_详细策划案.md`
- Legacy long documents such as `docs/archive/legacy_design/策划案.md`, `docs/reference/DEMO_SCOPE.md`, and `docs/archive/legacy_design/ONE_PAGE_GDD.md` remain available as historical/detail references, but new work should start from `docs/planning_package/README.md`.
- Current external-facing and development-facing wording should use the 15-day/car-evacuation/14-location/Qimian-hidden-AI canon in the planning package.
- Contest-demo second-run wording should describe Qimian **key playable action chapters**, not a complete second 15-day campaign. A full second campaign remains full-version scope.

## 2026-05-27

### Contest Lane Role Clarification

- The contest lane does NOT plan development tasks for other lanes.
- The contest lane has three responsibilities:
  1. **完成度审查 (Completion Review)**: Check project completeness against official contest requirements; flag drift or missing deliverables.
  2. **合规建议 (Compliance Advice)**: Read the official contest manual and give packaging/submission guidance.
  3. **提交辅助 (Submission Assistance)**: When the project is ready, help package materials and submit.
- Contest lane is not a project manager; it is a contest readiness checkpoint.

### Design Line: Evacuation, Qijin, and Two-Week Structure (2026-05-27)

- Day count is now uniformly 15 days across all docs. The two blood moons are Day 7 and Day 15; late abnormal nights (Days 11-14) are `红潮夜`.
- Qijin does **not** physically appear in the Demo. His presence is felt through radio broadcasts mentioning Rebirth Project activity, and a distant convoy seen at Day 15 dawn. The `白昼协议` plotline is removed from Demo scope.
- Car procurement replaces "fix the bike" as the final evacuation condition. Flow: discover car in repair-shop garage (Days 5-7) → collect battery + gasoline + tire (Days 8-11) → repair at workbench (Days 12-13) → car ready (Day 14) → drive out but car breaks down in outskirts (Day 15), forcing Lin Xing to walk to the gate.
- Evacuation trigger on Day 15 is dual: super horde broadcast warning + shelter too damaged to hold.
- Bicycle remains Lin Xing's near/mid-range transport; it is not enough to reach the safe zone.
- The Day 15 ending now spans into the early hours of Day 16: Lin Xing walks on foot after the car fails and reaches the gate at dawn.
- Week 1 ending is a **two-layer reveal**: (1) Lin Xing hears hushed talk at the screening shed about "the horde being steered" and "a motorcycle crossing the east line"; (2) the player unlocks Qimian's diary + animated scene replays showing 5 key Qimian actions (waking, riding motorcycle, clearing the bridge, leaving medicine, hiding in the horde on blood-moon night).
- Week 2 Qimian is a **playable character**, not a cutscene log. Loop: hide by day in hillside villa → plan → ride motorcycle on night missions (scout, lure zombies, move survivors, track Qijin). Lin Xing is AI-controlled in Week 2, inheriting the player's Week 1 tendencies.
- Car breaks down on Day 15 (engine overheat / tire blowout). Lin Xing abandons most supplies and walks. This creates the most tense stretch: car dead → on foot → dawn arrival.
- Qimian's Day 15 motorcycle is at the safe-zone outer line processing horde routes; he does not recognize Lin Xing walking past.

## 2026-05-21

### Demo Scope

- The current game structure is a complete 15-day survival cycle, not a 30-day structure.
- The first playable protagonist's Chinese name is `林行`; use `Lin Xing` in English materials. This replaces the older `陈醒` / `Chen Xing` name in design, art, and submission copy.
- Lin Xing wakes at home, not in a hospital. The home doubles as the emotional opening and early shelter anchor.
- Day 7 and day 15 night are the two true blood moon events in the current demo structure.
- Lin Xing is the first playable perspective.
- Qimian sleeps during days 1-4, wakes on day 5, then secretly affects the shared world.
- The demo-end reveal is Qimian's hidden action log.
- Day 15 is the escape-pressure blood moon: Lin Xing leaves in daylight because (1) radio warns of a super horde approaching within 24 hours, and (2) the shelter is too damaged to survive another night.
- Lin Xing's day-15 departure is both forced and chosen: the shelter is failing, and the short safe-zone window is the only viable chance.
- The day-15 endpoint is Lin Xing reaching the safe-zone gate and waiting for infection screening, not fully entering the safe zone.
- Lin Xing passes initial screening but is placed under quarantine observation; he reaches a stage gate, not true safety.
- The safe-zone window publicly reads as temporary intake for outer-ring survivors, but the hidden cause is Rebirth Project cleanup pressure forcing the safe zone to contract its defensive line.
- Safe-zone intake centers on infection screening.
- Lin Xing's pre-day-15 evacuation readiness comes from three conditions: confirming the safe zone still exists by radio, finding its address through maps/checkpoints, and finding and repairing a car (bicycle only covers near/mid-range exploration; a car is required to actually reach the safe zone).

### Lin Xing Management Loop

- Lin Xing's management experience references This War of Mine's pressure structure: scavenging, shelter maintenance, scarcity, injury/fatigue, unsafe nights, and survival/moral tradeoffs. It should not copy This War of Mine's exact systems.
- The 15-day Demo lightly includes three layers: resource scarcity and shelter building, survivor moral choices, and dangerous location exploration.
- Shelter facilities are limited to five core facilities: bed, workbench, window barricade, radio, and storage/organizing table.
- Facility roles: bed lowers fatigue/stress; workbench repairs the bike and creates simple tools; window barricade affects day-7 blood moon defense and day-15 pre-escape losses; radio provides safe-zone clues, blood-moon warnings, and Rebirth Project abnormal broadcasts; storage/organizing table improves preservation/carrying and affects day-15 supplies.
- Core resources are food, water, medicine, materials, parts, and fuel.
- Fuel can support vehicle or generator needs. Generator use can provide benefits such as lighting or radio operation, but creates noise risk.
- Intel/clues should be key discoveries or narrative flags rather than ordinary stackable resources.
- Exploration locations are small top-down stealth levels, not result-only menus.
- Top-down room readability can reference Hotline Miami-style spatial presentation, but the gameplay goal is survival stealth rather than combat clearing.
- Unlit rooms provide no pre-entry vision; windowed rooms provide partial vision when it is not raining; rain weakens or blocks window vision.
- Rooms may hide zombies. Lin Xing can counterattack only in limited, costly ways, while recommended play is hiding, routing around, closing doors, or making noise to lure zombies.
- Main exploration punishments are injury/infection and time/fatigue; staying too long leads to nightfall and fatigue gain. Noise is a local attraction risk, not the main long-term punishment.
- Location goals are "take enough and leave"; players decide how greedy to be.
- The overworld uses a node-based pixel map: location nodes, route limits, resource tendencies, danger levels, and small status icons.
- Location status on the overworld should show resource tendency and danger level; Qimian traces, safe-zone clues, and abnormal states can appear as small icons or question marks, not full detailed values.
- Routes have road condition and travel time. Rain, blockage, and zombie migration can increase travel time and fatigue; do not add multi-route choices per destination in the current scope.
- Indoor search remains top-down. It can borrow dark pixel-room lighting, furniture density, search-point mood, and darkness pressure from the provided room reference, but should not switch to 45-degree/isometric view.

### Contest Positioning

- The contest pitch should emphasize that AI is not a chat UI; AI is the hidden protagonist action engine.
- The strongest submission angle is "AI-controlled hidden protagonist changes the same world the player is trying to survive in."
- Contest materials should not invent implemented features. Planned ideas must be labeled as planned or future scope.
- For the active Tencent Cloud Hackathon submission sprint, treat `2026-06-20` as the hard deadline for both registration and work submission.
- The default challenge direction is `叙事类游戏：用 AI 重塑叙事体验`, unless the user later redirects to another track.

### Worldbuilding

- Qijin's organization is the Rebirth Project: a hybrid group with a scientific outer shell and religious/internal doctrine.
- The Rebirth Project publicly frames itself through research, samples, cleansing, and life-extension language; internally it treats infection as "rebirth" and city collapse as a necessary selection process.

### Core Character Setup

- Lin Xing, Qimian, and Qijin are all male.
- Lin Xing and Qimian are old friends who were close in childhood, disconnected as adults, and have a faded relationship by the time the demo begins.
- Lin Xing, Qimian, and Qijin made a childhood "apocalypse shelter plan"; the home opening can use old maps, notes, and routes from that plan.
- Qimian does not immediately think of Lin Xing after waking. His first arc is recalling his infection experience, confirming his changed body, becoming confused, then choosing to find Qijin as his main task.
- Qimian's desire to help strangers develops later from what he sees during night actions, not from an instant heroic mission.
- Qimian's regular vehicle is a motorcycle, while Lin Xing's regular vehicle remains a bicycle.
- Qimian's fixed daytime hideout is a villa/cabin in the hills on the suburban edge.
- Qimian does not return to his old home after waking. The practical reason is exposure risk in familiar human neighborhoods; the emotional reason is that he no longer feels able to bring his changed self back into his former life.
- Lin Xing and Qimian can pass very close at the day-14 climax without recognizing each other; Lin Xing later learns he received many indirect benefits from Qimian, and Qimian later learns some of the people he accidentally helped included Lin Xing.
- Qimian may actively change a zombie group's route for his own goals, such as avoiding safe-zone searchlights, moving survivors, or pursuing clues. If this saves Lin Xing, it is an unintended consequence, not targeted protection.
- The ending uses a double reveal: Lin Xing only hears a small anomaly clue, while the player unlocks the full Qimian action log. Lin Xing and Qimian still do not know they passed each other.
- Qimian's log should be "AI action replay + Qimian subjective fragments": personality tendency, feasible actions, rule ordering, final action, shared-world impact, and a short human note.
- Qimian's opening personality card deterministically defines his AI decision rules. It should not be treated as probability weights or random daily selection; the same personality card in the same world state should produce the same class of action.
- Qimian AI input is limited to Qimian-perceivable state: what he can see, hear, or infer. It should not read Lin Xing's hidden state, other survivors' backend state, or whether Lin Xing explored a location.
- The system can use global state to resolve indirect consequences, but Qimian's decisions must remain grounded in his own perception.
- Qimian can make limited in-world inferences from traces, such as fresh bicycle tracks or familiar marks, but cannot directly identify Lin Xing from backend knowledge.
- Qimian's personality card is hidden during the first run and fully revealed in the post-demo log with its rules and consequences.
- Long-term design can generate Qimian personality cards per run; the current Demo uses a fixed default personality card for stable narrative, testing, and balance.
- Demo default Qimian card: main goal is finding Qijin; tendency is cautious and avoids exposure; moral rule is helping nearby people without taking on mass rescue; resource habit is taking only resources needed for the task; safe-zone attitude is observing it while distrusting screening.

### Collaboration

- Future sessions should use `AGENTS.md` and `HANDOFF.md` as the first coordination layer.
- When both files exist, read `AGENTS.md` first, then `HANDOFF.md`, before planning or editing.
- Important cross-session information should be appended to `docs/PROJECT_MEMORY.md`.
- Before ending a substantial session, update `docs/PROJECT_MEMORY.md`; add stable decisions to `docs/DECISIONS.md`.
- `HANDOFF.md` should stay concise and only contain immediate onboarding context.

### Art Lane Boundaries

- Art lane owns `assets/` and `docs/ASSET_PIPELINE.md`.
- Art lane should not edit `game/scripts/**` unless the change only wires already-agreed asset paths or the user explicitly coordinates with the code lane.
- Contest art must be original, AI-generated with commercial-use rights, CC0, or clearly licensed.
- Minecraft and Plants vs. Zombies assets are private placeholders only and must not be included in public contest submissions.

### Art Specs

- The contest demo art target is `32x32` character sprites.
- Base tiles use `16x16`; larger props may use `32x32`, `32x48`, or tile combinations.
- The first art batch prioritizes Lin Xing, Qimian, normal zombie, blood moon zombie, core demo locations, and basic resource/status UI icons.
- Every external or AI-generated asset that enters `assets/sprites/` needs a row in `docs/ASSET_LICENSE_LOG.md`.
- Lin Xing's first character prompt should use an original design with a compact, sharp-eyed, short black-haired, stern survivalist silhouette inspired by the "captain/兵长" archetype, while avoiding direct replication of any anime uniform, insignia, cloak, weapon, or recognizable copyrighted character details.
- Lin Xing's provisional character look is a small-head adult pixel survivor with short black hair, a muted blue-gray wrapped robe/poncho silhouette, dark inner clothes, boots, and a small pack. Keep this look as the base for cleanup and animation, but remove unrelated weapon/shield/fantasy frames from the contest demo set.

### Code Lane Boundary

- Code-lane conversations own Godot implementation files under `game/scripts/**`, `game/tests/**`, and `game/scenes/**`.
- Code-lane work may read design and contest docs for context, but should not alter narrative/design sections unless the user explicitly asks or cross-lane coordination requires it.
- Before claiming code work is complete, run the Godot simulation test listed in `HANDOFF.md`.

### Repository

- Use `https://github.com/T3L000/BeyondSafeZone.git` as the canonical GitHub repository for this project.
- The local working branch should be `main` unless the user requests another branch.

### Demo Pressure Mechanics

- The contest demo should use a deterministic 15-day pressure table rather than procedural randomness.
- This War of Mine-like influence means scarcity, day/night tradeoffs, scavenging pressure, shelter survival, and hidden-world clues; it does not mean copying combat, art, UI, or exact systems.
- Day-start pressure applies once per day and should not stack from UI refreshes or repeated calls.
- Ending states for the 15-day demo are `reached_gate_quarantine`, `barely_reached_gate`, and `collapsed`.

### Node Map Mechanics

- The current demo overworld uses deterministic node metadata instead of procedural discovery: each location exposes resource tendency, danger level, route time, road condition, and compact icon labels.
- Exploration clues that matter for evacuation should set explicit flags such as `safezone_confirmed` and `address_known`; these clues are not stackable resources.
- Road conditions such as blockage, rain/standing water, zombie migration, and hard blockade add deterministic fatigue pressure during exploration.
- When Qimian changes a location, that node should carry a visible `qimian_trace`/`qimian` marker so the player sees an abnormal map trace before the final hidden-log reveal.

### Indoor Search Mechanics

- The current demo indoor exploration is a deterministic greybox state machine, not a freeform combat system.
- Entering a location starts a `searching` phase with an active location, time limit, searched rooms, local noise, and lured-room state.
- Room data should expose visibility, search time, hidden zombie pressure, and deterministic resource pickup.
- Hidden zombies punish rushed or dark-room searching with health and infection-risk costs.
- Noise lure is a preparation action: it spends time and raises local noise, but can prevent direct hidden-zombie injury in that room.
- Leaving a location advances to evening; overstaying the room-search time limit converts into fatigue pressure.

### Infection and Medicine Mechanics

- Infection risk is a deterministic pressure value for the demo, not a random death roll.
- Infection risk stages are readable to the player: low risk by default, fever risk at `infection_risk >= 3`, and dangerous infection at `infection_risk >= 5`.
- Dangerous infection creates night pressure by reducing health and raising stress, while keeping the 15-day demo flow intact.
- Medicine is currently scoped to Lin Xing's own wound care: `treat_wound` spends one medicine, restores one health, and lowers infection risk by one.
- Wound treatment without medicine should fail with event text and no hidden state changes.

### Dual-Protagonist Loop Refinement

- Lin Xing should move closer to a `This War of Mine`-like pressure loop in feel: scavenging pressure, night defense pressure, and costly combat, while still avoiding direct system copying.
- Lin Xing may fight zombies in the demo, but combat is a costly survival tool for escape or route control, not a room-clearing power fantasy.
- Lin Xing's standard combat path is near-melee survival: melee weapons are common, while guns are rare late tools with scarce ammo, very high noise, and strong attraction risk.
- Injury and infection should remain separate systems: injuries can be treated and recovered from, while infection is a staged deterioration pressure that can be suppressed or slowed rather than easily erased.
- Blood moon nights should read as real defense / survival events, not only background settlement math.
- Qimian should not inherit Lin Xing's base-management loop. Qimian's playable identity is a task-driven night operator: hide by day, investigate and intervene by night.
- Qimian's main gameplay pressure is exposure risk rather than ordinary food-water-shelter scarcity.
- A hard fiction rule is locked: Qimian can move near or through zombie groups, but cannot openly reveal in front of human witnesses that ordinary zombies do not attack him.
- Midgame Qimian missions can include route rescue tasks such as restoring a bridge blocked by zombies, but the solution should be covert redirection, luring, or environmental manipulation rather than obvious frontal clearing.
- Late-game night pressure should intensify over time through increasingly frequent `红潮夜` before the final blood moon.
- Worldbuilding should preserve a distinction between the two major blood moons and more frequent late abnormal nights branded as `红潮夜`, instead of turning every late night into the same red-moon event.

### AI Transparency And Shared Map

- The first run should not wait until the ending to let the player feel the AI system exists.
- From days `5-6` onward, the player should begin noticing recurring anomalies caused by a second actor in the world: precisely taken supplies, quietly opened locks, rerouted zombie groups, anonymous drops, or altered route danger.
- The post-run Qimian log should reveal identity, motivation, and rule-based decision logic, rather than serving as the first proof that a hidden actor existed.
- Contest-scope shared-map implementation is defined as one set of location nodes and shared location states resolved in sequence across time periods, not two protagonists freely roaming a fully real-time shared overworld at once.
- Shared location state should at minimum cover resources, lock state, zombie density, route blockage, survivor presence, Qimian traces, and organization traces.

### Design Line: Qimian Gameplay Systems and Exploration (2026-05-27)

- **Perspective**: All exploration levels (Lin Xing and Qimian) now use **side-view horizontal perspective** (多层建筑剖面、上下攀爬、声音/影子判断楼层), replacing earlier 俯拍 references.
- **Qimian Exposure Value (暴露值)**: Qimian's failure condition is exposure (0-10), not death. Gaining exposure: searchlight zones +2, survivor witness +1, organization facility breach +3, witnessed zombie slaughter +3. Reducing: change hideout -3, destroy evidence -1, rain/red-tide cover = no exposure penalty for that night.
- **Qimian Motorcycle Tier (摩托状态)**: Lv.1 starter → Lv.2 (parts×3+materials×2) → Lv.3 (parts×5+fuel×2+battery). AI only checks "tier ≥ mission requirement?".
- **Zone Heat (区域热度)**: A/B/C three zones. Heat 0-3, rising when Qimian acts in a zone, cooling otherwise. Heat 3 = AI refuses primary missions in that zone.
- **Signal Tracking (信号追踪)**: Radio direction-finding to locate Qijin. Three tracking missions (Day 8/11/14). Incomplete info still advances progress.
- **Mark Language (标记语言)**: Fluorescent spray symbols for routes, personal notes, and anonymous survivor messages. Shared map integration.
- **Qimian Event Pool**: 9 side-view stealth levels: supermarket night raid, gas station diversion, clinic doctor rescue, bridge clearing, radio tower jammer destruction, flood gate closing, underground passage clearing, Qijin signal tracking, anonymous medicine drop. AI auto-completes; player-operated with fail consequences.
- **Qimian Musou Combat**: Qimian CAN slaughter zombies freely (zombies don't attack him), but any witness → exposure +3. Core tension: power vs. visibility.
- **Lin Xing Situational Monologue**: 15-day morning monologue table with state variants (hunger/fatigue/infection/stress triggers).
- **Complete Version Vision (30-day)**: Archived for future reference. Day 15 choice to stay or evacuate; Qimian only joins party post-evacuation in protection zone; 白昼协议 deferred to full version. Not in Demo scope.
- **NPC rule**: Bridge camp is primary NPC hub. Other NPCs can be met but do not join.
- **Single canonical ending**: Demo follows one main narrative line. Alternate endings (barely_reached/collapsed) exist as system fallbacks without separate narrative design.

### Repository Hygiene

- Do not commit `.superpowers/`, `.godot/`, generated builds, temporary files, or logs.
