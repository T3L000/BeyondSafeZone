# 祁眠 AI 决策伪代码

> **用途**: 代码线直接翻译为 GDScript。
> **规则**: 确定性决策，同一天+同一世界状态 → 同一输出。不使用随机数。
> **视角**: 侧视横版。

---

## 一、输入数据结构

### 1.1 祁眠自身状态

```
qimian_state = {
    day: int,                     # 1-15
    awake: bool,                  # day >= 5
    moto_tier: 1 | 2 | 3,
    base_hideout: "villa",        # 当前藏身点
    exposure: 0..10,
    inventory: {
        food: int, water: int, medicine: int,
        materials: int, parts: int, fuel: int, battery: int
    },
    qijin_clues: 0..3,            # 追踪进度
    rescued_npc: []               # 已救 NPC ID 列表
}
```

### 1.2 世界可感知状态

```
perceivable_state = {
    weather: "clear" | "rain" | "fog" | "storm",
    moon_phase: "normal" | "blood_moon" | "red_tide",
    radio_signal_qijin: {
        active: bool,
        direction: "north" | "east" | "south" | "west" | null,
        strength: 1..3             # 1=模糊 3=清晰
    },
    zone_heat: { A: 0..3, B: 0..3, C: 0..3 },
    # 以下为祁眠"可感知"的——通过痕迹/观察推断，不是后台读取
    recently_visited_by_human: ["supermarket"],  # 看到有新鲜访问痕迹的地点
    zombie_group_positions: [                     # 自己侦查到的尸群位置
        {location: "bridge", count: 15},
        {location: "gas_station", count: 12}
    ],
    survivor_distress: [                         # 听到/看到的求救信号
        {location: "clinic", type: "trapped", urgency: "high"}
    ],
    player_marks: [                              # 林行留下的世界内标记；祁眠不知道是谁留下的
        {location: "clinic", type: "help", age_days: 0},
        {location: "bridge", type: "route", age_days: 1}
    ],
    reserved_resources: [                        # 现场被刻意保留的物资痕迹
        {location: "clinic", resources: {medicine: 1}}
    ],
    anomaly_traces: [                            # 林行/祁眠都可能看见的地点异常，不含后台身份
        {location: "supermarket", tag: "recently_opened"},
        {location: "bridge", tag: "zombie_diverted"}
    ],
    known_rebirth_locations: ["clinic", "quarantine", "radio_tower", "checkpoint"]
}
```

### 1.3 任务定义

```
TASKS = {
    # === 事件任务（有触发窗口） ===
    "gas_diversion": {
        type: "event",
        day_range: 5..7,
        difficulty: 2,              # 对应 moto_tier 最低要求
        req_zone: "B",
        req_perception: {location: "gas_station", condition: "oil_spill_visible"},
        actions: ["lure_zombies", "ignite_fuel", "mark_route"],
        world_effect: {gas_station: {route_south_clear: true, duration_days: 2}},
        qimian_reward: {parts: +1}
    },
    "rescue_doctor": {
        type: "event", day_range: 6..9, difficulty: 1, req_zone: "A",
        req_perception: {location: "clinic", condition: "human_inside"},
        actions: ["lure_zombies", "unlock_door", "escort_npc"],
        world_effect: {bridge_camp: {npc_add: "doctor"}},
        qimian_reward: {exposure: -1, rescued_npc: +1}
    },
    "destroy_jammer": {
        type: "event", day_range: 8..11, difficulty: 2, req_zone: "C",
        req_perception: {location: "radio_tower", condition: "jammer_active"},
        actions: ["infiltrate", "sabotage", "erase_trace"],
        world_effect: {radio: {civilian_channel_restored: true}},
        qimian_reward: {battery: +1}
    },
    "close_flood_gate": {
        type: "event", day_range: 7..10, difficulty: 1, req_zone: "B",
        req_perception: {location: "flood_gate", condition: "water_rising"},
        actions: ["rotate_wheel", "defend_position", "escape"],
        world_effect: {bridge_camp: {flood_prevented: true}},
        qimian_reward: {exposure: -2, hideout_backup: "flood_gate_basement"}
    },
    "clear_underground": {
        type: "event", day_range: 9..12, difficulty: 1, req_zone: "B",
        req_perception: {location: "subway", condition: "tunnel_has_zombies"},
        actions: ["lure_from_A", "lure_from_B", "mark_entrances"],
        world_effect: {route: {AB_stealth_unlocked: true}},
        qimian_reward: {materials: +2}
    },
    "track_qijin_1": {
        type: "event", day_range: 8..10, difficulty: 2, req_zone: "C",
        req_perception: {radio_signal_qijin: {active: true, strength: ">=2"}},
        actions: ["ride_to_zone", "deploy_antenna", "infiltrate_location"],
        world_effect: {qijin_clues: +1},
        qimian_reward: {qijin_clues: +1}
    },
    "track_qijin_2": {
        type: "event", day_range: 11..13, difficulty: 2, req_zone: "C",
        req_perception: {radio_signal_qijin: {active: true}},
        actions: ["ride_to_zone", "deploy_antenna", "infiltrate_location"],
        world_effect: {qijin_clues: +1},
        qimian_reward: {qijin_clues: +1}
    },

    # === 常规任务（无触发窗口，每晚可用） ===
    "patrol": {
        type: "routine", difficulty: 1, req_zone: "any",
        actions: ["ride_through_zone", "mark_safe_routes"],
        world_effect: {zone: {heat: +1}, flagged_routes: +1},
        qimian_reward: {exposure_if_clear_weather: +0}
    },
    "scavenge": {
        type: "routine", difficulty: 1, req_zone: "any",
        actions: ["infiltrate_location", "loot_quietly"],
        world_effect: {location: {resources: reduced}},
        qimian_reward: {materials_or_parts: +1, food_or_water: +1}
    },
    "supply_drop": {
        type: "routine", difficulty: 1, req_zone: "any",
        req_perception: {any_survivor_camp: "needs_supplies"},
        actions: ["leave_package", "erase_own_trace"],
        world_effect: {camp: {supplies: +1}},
        qimian_reward: {exposure: -1}
    },
    "rescue_nearby": {
        type: "routine", difficulty: 1, req_zone: "any",
        req_perception: {player_mark_or_distress: ["help", "trapped", "injured"]},
        actions: ["observe_from_cover", "leave_supplies_or_open_route", "erase_own_trace"],
        world_effect: {location: {anomaly_tags: ["anonymous_medicine"]}},
        qimian_reward: {exposure: -1}
    },
    "clear_route": {
        type: "routine", difficulty: 1, req_zone: "any",
        req_perception: {player_mark_or_trace: ["danger", "route", "zombie_group"]},
        actions: ["scout_route", "lure_zombies", "mark_route"],
        world_effect: {location: {danger_level: "down", anomaly_tags: ["zombie_diverted"]}},
        qimian_reward: {parts: +0}
    },
    "rest": {
        type: "routine", difficulty: 0, req_zone: "hideout",
        actions: ["rest"],
        world_effect: {},
        qimian_reward: {exposure_if_zone_cold: -1}
    },

    # === 固定日程任务 ===
    "supermarket_raid": {
        type: "scheduled", day: 6, difficulty: 1, req_zone: "B",
        actions: ["infiltrate_supermarket", "take_essentials"],
        world_effect: {supermarket: {storage_food: -2}},
        qimian_reward: {food: +2}
    },
    "clear_bridge": {
        type: "scheduled", day: 10, difficulty: 2, req_zone: "B",
        actions: ["climb_bridge_pillar", "trigger_alarm", "ride_to_lure"],
        world_effect: {bridge: {zombie_count: -12, route_west_open: true}},
        qimian_reward: {parts: +1}
    },
    "leave_medicine": {
        type: "scheduled", day: 12, difficulty: 1, req_zone: "A",
        actions: ["ride_to_home_area", "leave_package_by_door", "leave_silently"],
        world_effect: {linxing_home: {anonymous_medicine: true}},
        qimian_reward: {exposure: -1}
    }
}
```

---

## 二、决策主流程

```
function decide_qimian_action(qimian_state, perceivable_state, shared_world_state):
    # --- Step 0: 边界检查 ---
    if qimian_state.day < 5 or qimian_state.exposure >= 10:
        return {action: "none", reason: "asleep_or_exposed"}

    # --- Step 1: 排除不可用区域 ---
    available_zones = ["A", "B", "C", "hideout"]
    for zone in ["A", "B", "C"]:
        if perceivable_state.zone_heat[zone] >= 3:
            available_zones.remove(zone)        # 热区——巡逻太密
        if qimian_state.moto_tier == 1 and zone == "C":
            available_zones.remove(zone)        # Lv.1 去不了远圈

    # --- Step 2: 收集可用任务 ---
    candidate_tasks = []

    for task_id, task in TASKS:
        # 2a. 固定日程先筛——今天的 scheduled 任务排第一
        if task.type == "scheduled" and task.day == qimian_state.day:
            if can_do(task, available_zones, perceivable_state, qimian_state):
                candidate_tasks.append(task_id)
                continue

        # 2b. 事件任务——在触发窗口内且感知条件满足
        if task.type == "event":
            if qimian_state.day not in task.day_range:
                continue
            if not can_do(task, available_zones, perceivable_state, qimian_state):
                continue
            candidate_tasks.append(task_id)

        # 2c. 常规任务——总是加入
        if task.type == "routine":
            if can_do(task, available_zones, perceivable_state, qimian_state):
                candidate_tasks.append(task_id)

    # --- Step 3: 按人格规则排序 ---
    PERSONALITY_RULES = [
        {rule: "find_qijin",     match: ["track_qijin"],          priority: 100},
        {rule: "rescue_nearby",  match: ["rescue"],               priority: 90},
        {rule: "improve_routes", match: ["clear", "diversion"],   priority: 70},
        {rule: "scavenge",       match: ["scavenge", "raid"],     priority: 50},
        {rule: "observe_safezone", match: ["checkpoint"],          priority: 30},
        {rule: "avoid_exposure",  fallback: true,                 priority: 0}
    ]

    # 给每个 candidate 打分
    scored = []
    for task_id in candidate_tasks:
        task = TASKS[task_id]
        score = 0
        matched_rule = "avoid_exposure"
        for rule in PERSONALITY_RULES:
            if "match" in rule and any(kw in task_id for kw in rule.match):
                score = max(score, rule.priority)
                matched_rule = rule.rule
                break
        # 调度优先: scheduled > event > routine
        type_bonus = {"scheduled": 200, "event": 150, "routine": 0}
        score += type_bonus[task.type]

        # 一周目隔空标记只作为可感知痕迹加权，不是玩家命令
        mark_bonus = score_player_mark_input(task_id, task, perceivable_state, qimian_state)
        score += mark_bonus

        # 高暴露时，标记再强也不能压过生存/隐蔽边界
        if qimian_state.exposure >= 8 and task_id != "rest":
            score -= 80
        scored.append({id: task_id, score: score, dominant_rule: matched_rule})

    # 降序排列
    scored.sort(key: -score)

    # --- Step 4: 如果有调度任务，直走（不等排序） ---
    for s in scored:
        if TASKS[s.id].type == "scheduled":
            selected = s.id
            break
    else:
        # 否则取最高分
        selected = scored[0].id
    dominant_rule = first(s for s in scored if s.id == selected).dominant_rule

    # --- Step 5: 执行 + 结算 ---
    result = execute_task(selected, qimian_state, shared_world_state)

    # --- Step 6: 记录行动日志 ---
    log_entry = {
        day: qimian_state.day,
        task: selected,
        personality: dominant_rule,
        perceivable_input: summarize(perceivable_state),
        world_trace_input: summarize_world_traces(perceivable_state),
        candidates_sorted: [s.id for s in scored],
        decision_reason: dominant_rule,
        world_impact: result.world_changes,
        subjective_fragment: get_fragment(selected, qimian_state)
    }
    return log_entry


function can_do(task, available_zones, perception, qimian):
    # 区域检查
    if task.req_zone != "any" and task.req_zone not in available_zones:
        return false
    # 摩托等级
    if qimian.moto_tier < task.difficulty:
        return false
    # 感知条件
    if task.req_perception:
        for key, val in task.req_perception:
            if not check_perception(key, val, perception):
                return false
    return true


function score_player_mark_input(task_id, task, perception, qimian):
    bonus = 0
    for mark in perception.player_marks:
        if mark.age_days > 2:
            continue

        if mark.type == "help":
            if task_id in ["rescue_nearby", "supply_drop", "leave_medicine", "rescue_doctor"]:
                bonus += 45
            if task_id == "scavenge":
                bonus += 10    # 可顺路取任务所需资源，但不清空地点

        if mark.type == "danger":
            if task_id in ["clear_route", "patrol", "clear_bridge"]:
                bonus += 35
            if task.req_zone != "hideout" and qimian.exposure >= 6:
                bonus -= 30    # 暴露较高时更可能避开危险标记

        if mark.type == "route":
            if task_id in ["patrol", "clear_route", "clear_bridge", "gas_diversion"]:
                bonus += 30

        if mark.type == "reserved_supply":
            if task_id in ["supply_drop", "rescue_nearby", "leave_medicine"]:
                bonus += 25
            if task_id == "scavenge":
                bonus -= 20    # 人格卡“只拿任务所需”，避免把保留物资扫空

    return bonus
```

---

## 三、暴露值结算

```
function update_exposure(qimian, action, weather, zone):
    delta = 0

    # 行动本身风险
    if action_zone is in_searchlight_area:
        delta += 2
    if action involves "infiltrate_rebirth_facility":
        delta += 2
    if action involves被幸存者目击:
        delta += 1

    # 区域热度惩罚
    delta += zone_heat[zone] if zone_heat[zone] >= 2 else 0

    # 天气掩护
    if weather == "rain" or moon_phase == "red_tide":
        delta = 0        # 完全掩护

    # 无双杀丧尸
    if action involves "slaughter_zombies" and witnessed_by_human:
        delta += 3

    qimian.exposure += delta
    clamp(qimian.exposure, 0, 10)

    # 暴光后果
    if qimian.exposure >= 5:
        zone_heat[zone] += 1    # 巡逻队注意你的区域
    if qimian.exposure >= 8:
        chance_of_ambush_next_visit = true
    if qimian.exposure >= 10:
        trigger_bad_ending()
```

---

## 四、日志结构（通关后展示）

```
qimian_log_format = {
    day: int,
    task_name: string,
    dominant_rule: "find_qijin" | "rescue_nearby" | "improve_routes" | "scavenge" | "avoid_exposure",
    ai_replay: {
        perceivable_state: { ... },     # 祁眠当时看到/听到的
        world_trace_input: {            # 玩家一周目可理解的世界内输入
            player_marks: [
                {location: string, type: "danger" | "help" | "route" | "reserved_supply", age_days: int}
            ],
            anomaly_traces: [{location: string, tag: string}],
            reserved_resources: [{location: string, resources: {...}}]
        },
        all_candidates: [               # 所有可行任务
            {name: string, rule_score: int, type_bonus: int, total: int}
        ],
        selected: string,
        why: "人格规则 'find_qijin' (score 100) 压过 'scavenge' (score 50)"
    },
    world_impact: {                    # 共享世界改变
        locations_changed: [{id: string, field: string, before: any, after: any}],
        flags_set: [string],
        lin_xing_visible_tomorrow: string   # 林行次日会看到什么
    },
    subjective_fragment: string         # 祁眠主观残句
}
```

---

## 五、完整一天 AI 流程示例

**假设**: Day 8, 天气晴, 摩托 Lv.2, 区域热度 A=1 B=2 C=0, 信号追踪 1 可触发；诊所有 Day 7 遗留的 help 标记

```
Step 0: awake=true, exposure=3 (OK)

Step 1: 可用区域 = [A, B, C, hideout]
        C 区热度=0 ✓, B 区热度=2 ⚠️, A 区热度=1 ✓

Step 2: 收集任务:
        patrol (A/B/C)
        scavenge (A/B/C)
        supply_drop (A, 因为 bridge_camp 有幸存者)
        rescue_nearby (A, 因为 clinic 有 help 标记)
        destroy_jammer (C, 在窗口内, moto_tier=2 ≥ difficulty=2)
        track_qijin_1 (C, 信号 active)
        supply_drop (B)  # B 也可以但要加热度惩罚
        → candidates = [patrol, scavenge(C), supply_drop(A), rescue_nearby(A), destroy_jammer, track_qijin_1]

Step 3: 人格排序:
        track_qijin_1 → 100 + 150(event) = 250
        rescue_nearby(A) → 90 + 0(routine) + 45(help 标记) = 135
        supply_drop(A) → 90  + 0(routine) + 45(help 标记) = 135
        destroy_jammer → 70  + 150(event) = 220  # improve_routes 类
        scavenge(C)    → 50  + 0(routine)   = 50
        patrol         → 0   + 0(routine)   = 0

Step 4: 无 scheduled → 选 track_qijin_1

Step 5: 执行追踪 → qijin_clues +1, C 区热度 +1

Step 6: 日志:
        "截获加密信号方向 = 南偏西。骑到隔离站附近——信号最强。潜入标本室。
         找到了: 实验日志，祁烬签名。他建议保留我。
         对不起……他在保护我。但为什么不让我找到他。"
```

---

## 六、AI 约束清单（代码强制）

| 约束 | 实现方式 |
|------|---------|
| 不能读取林行后台状态 | `perceivable_state` 中不包含 Lin Xing 的 health/hunger/fatigue 等 |
| 不能知道林行探索了哪个地点 | 只能通过"看到新鲜指印/胎印/翻过的柜子"来推断被访问过 |
| 可以读取玩家标记 | 只能读取 `player_mark` 这类世界内痕迹，不能知道标记者身份 |
| 不能知道幸存者精确位置 | 只能"听到求救声"或"看到火光"方向，不能读坐标 |
| 不能读出 NPC 对话内容 | 幸存者说的话林行才知道，祁眠只能"看到有人在交流" |
| 人格卡同输入→同输出 | 无 random() 调用 |
| 同一天 AI 可被多次调用 | 幂等——每次返回相同结果（无副作用状态变更在外部处理） |
