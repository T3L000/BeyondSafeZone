# PPT Outline

> 对齐版本：当前 Unity 灰盒 `BeyondSafeZoneUnity/`。

## Slide 1: Title

- 《保护区之外》 / `Beyond Safe Zone`
- 2D pixel-art survival management greybox
- Core line: survive as Lin Xing while a hidden AI protagonist changes the shared world

## Slide 2: Design Question

- Most AI game features appear as dialogue, generation, or assistant tools.
- This project asks: what if AI controls another protagonist whose rule-based decisions become part of the player's survival story?

## Slide 3: Player Loop

- Lin Xing daytime: choose a location, scavenge, find clues.
- Lin Xing night: return to shelter, manage pressure and resources.
- Qimian night: hidden AI reads perceivable traces and modifies shared location state.
- Next day: player sees consequences and records anomalies.

## Slide 4: AI Mechanic

- Qimian has a fixed personality card.
- Qimian reads only perceivable state.
- Decisions are deterministic and explainable.
- Shared map changes are visible through anomalies, resources, traces, and dossier entries.

## Slide 5: Current Unity Greybox

- Formal scene: `Assets/Scenes/OneRunMain.unity`
- Runtime shelter greybox and HUD.
- Clinic, supermarket, garage entry buttons.
- Help marker action.
- Unknown-actor dossier panel.
- Ending text can explain the clinic AI causal chain.

## Slide 6: Implemented Vertical Slice

```text
Clinic anomaly
→ Lin Xing leaves help marker
→ Qimian reads marker after Day 5
→ Anonymous medicine / shallow arrow appears
→ Dossier records the inference
→ Ending log reveals AI decision chain
```

## Slide 7: Why It Is Not Just A Twist

- First run already lets the player investigate and test the hidden system.
- The player is not watching AI perform; the player is living with the consequences.
- The ending log makes the rules traceable instead of magical.

## Slide 8: Current Status

- Engine: Unity 2022.3 LTS.
- Project path: `BeyondSafeZoneUnity/`.
- Latest recorded Unity EditMode regression: `42/42 passed`.
- Current UI is greybox; formal art and polished UX are future work.

## Slide 9: Next Production Needs

- Make shelter/scavenge actions feel more playable.
- Add a readable Qimian ending log panel (`U-008`).
- Improve UI clarity for state, resources, location cards, and action costs.
- Replace greybox visuals with consistent pixel assets.

## Slide 10: Closing

- AI is not an add-on feature.
- AI is the person you never meet, but whose choices reshape your route.
- You thought it was the apocalypse being random. It was another protagonist making decisions.
