# 共享地图状态 API

> **用途**: 代码线实现林行↔祁眠分时段顺序结算的数据结构定义。
> **规则**: 同一套地点节点 + 同一套地点状态 → 林行白天改写 → 祁眠夜晚读取再改写 → 次日林行看到后果。

---

## 一、地点状态数据结构

每个地点在共享世界中用以下结构表示：

```
location_state = {
    id: string,                    # 地点 ID（见下方地点 ID 表）

    # === 资源 ===
    remaining_food: int,           # 剩余可拾取食物份数
    remaining_water: int,          # 剩余可拾取水份数
    remaining_medicine: int,       # 剩余可拾取药品份数
    remaining_materials: int,      # 剩余可拾取建材份数
    remaining_parts: int,          # 剩余可拾取零件份数
    remaining_fuel: int,           # 剩余可拾取燃料份数

    # === 完整性 ===
    lock_state: "open" | "locked" | "broken" | "picked",
                                    # open=未锁 locked=需要撬棍/钥匙
                                    # broken=强行撬坏 picked=被祁眠安静撬开
    door_state: "intact" | "damaged" | "boarded",
    windows_state: "intact" | "broken" | "boarded",

    # === 威胁 ===
    zombie_count: int,             # 当前活丧尸数量
    zombie_density: "low" | "medium" | "high" | "extreme",
    danger_level: "low" | "medium" | "high",   # 给林行大地图展示

    # === 路线 ===
    route_blocked: bool,           # 通往此地点的路线是否可通行
    route_time: int,               # 从据点到地点的基础时间(min)
    route_extra_time: int,         # 因天气/路况/尸群额外耗时(min)

    # === 人员 ===
    survivor_present: bool,        # 有幸存者吗
    survivor_id: string | null,    # NPC ID

    # === 祁眠痕迹 ===
    qimian_trace: bool,            # 祁眠近期来过（林行大地图显示 ? 图标）
    qimian_last_day: int | null,   # 祁眠最后访问的天数
    qimian_mark: string | null,    # 祁眠留下的荧光标记: ◯/✕/↓→/△/⧖

    # === 一周目异常调查 / 隔空标记 ===
    anomaly_tags: [string],        # 林行可发现异常: picked_lock/missing_portable_food/moto_tire_trace/zombie_diverted/anonymous_medicine/fluorescent_mark
    anomaly_last_seen_day: int | null,
                                    # 林行最近一次把异常写入档案的天数
    player_mark: "danger" | "help" | "route" | "reserved_supply" | null,
                                    # 林行离开地点前留下的世界内标记
    player_mark_day: int | null,   # 标记留下的天数，默认 2 天过期
    player_reserved_resources: {   # 玩家故意保留的物资，供祁眠通过现场痕迹推断
        food: int, water: int, medicine: int,
        materials: int, parts: int, fuel: int
    },

    # === 组织痕迹 ===
    rebirth_trace: bool,           # 返生计划活动痕迹
    rebirth_poster: bool,          # 有无返生计划海报
    rebirth_guard_present: bool,   # 有无组织安保人员

    # === 一次性旗标 ===
    plan_found: bool,              # 林行找到童年避难计划
    safezone_hint_1: bool,         # 便利店纸条
    rebirth_clue_1: bool,          # 诊所隔离记录
    rebirth_clue_2: bool,          # 派出所联络名单
    address_known: bool,           # 林行知道保护区地址
    childhood_memory: bool,        # 学校童年笔记
    car_found: bool,               # 修理铺车库汽车发现
    qimian_file: bool,             # 隔离站零号感染者档案
    lab_location: bool,            # 哨塔地图
    apartment_letter: bool,        # 公寓 302 信件
    qijin_apartment: bool,         # 公寓 402 祁烬信封

    # === 祁眠事件专用状态 ===
    doctor_rescued: bool,          # 诊所救医生成功
    jammer_destroyed: bool,        # 广播塔干扰器被拆
    flood_gate_closed: bool,       # 防洪闸已关
    underground_cleared: bool,     # 地下通道已清
    basement_hideout_found: bool,  # 闸门控制室地下备用藏身点
}
```

---

## 二、地点 ID 表

| ID | 中文名 | 圈层 |
|----|--------|------|
| `home` | 林行家 | 近 |
| `convenience_store` | 便利店 | 近 |
| `clinic` | 社区诊所 | 近 |
| `bike_shop` | 自行车修理铺+车库 | 近 |
| `supermarket` | 超市 | 中 |
| `school` | 废弃学校 | 中 |
| `police_station` | 派出所 | 中 |
| `subway` | 地铁口 | 中 |
| `bridge_camp` | 桥洞营地 | 中(NPC) |
| `gas_station` | 加油站 | 中 |
| `hardware_store` | 五金店 | 中 |
| `apartment` | 废弃公寓 | 中 |
| `checkpoint` | 城市边缘哨卡 | 远 |
| `quarantine` | 防疫隔离站 | 远 |

---

## 三、全局共享状态

```
global_shared_state = {
    day: 1..15,
    weather: "clear" | "rain" | "fog" | "storm",
    moon_phase: "normal" | "blood_moon" | "red_tide",
    blood_moon_warning: bool,      # 血月倒计时已触发
    civilian_radio_restored: bool, # 祁眠拆干扰器后 = true

    # === 林行专属(不在共享状态中，但祁眠可感知部分暴露在 perceivable_state) ===
    # lin_health, lin_hunger... 等不在共享状态中。祁眠不可读。

    # === 祁眠专属 ===
    qimian_exposure: 0..10,
    qimian_moto_tier: 1..3,
    qijin_clue_progress: 0..3,
    zone_heat_A: 0..3,
    zone_heat_B: 0..3,
    zone_heat_C: 0..3,

    # === 全局旗标 ===
    super_horde_warning: bool,     # Day 14 超大型尸潮广播
    last_call_36h: bool,           # Day 13 最后通告
    final_broadcast: bool,         # Day 14 不再广播

    # === 一周目未知行动者档案 ===
    anomaly_dossier: {
        entries: [
            {day: int, location_id: string, anomaly_tag: string,
             evidence_text: string, inference_tags: [string], verified: bool}
        ],
        known_patterns: [string],
        player_hypotheses: [string]
    },
}
```

---

## 四、结算顺序（分时段）

```
每日结算管道:

清晨(05:00)
├─ 林行清晨结算(饥饿-1, 口渴-1, 疲劳变化, 精神变化)
├─ 天气/月相更新
├─ 广播事件触发(收音机)
└─ 地点状态同步到林行大地图

白天(06:00-16:00) — 林行操作
├─ 林行选择地点 → 探索
├─ 拾取资源: location_state.remaining_xxx -= n
├─ 击杀丧尸: location_state.zombie_count -= n
├─ 解锁旗标: location_state.xxx_flag = true
├─ 发现异常: location_state.anomaly_tags → global_shared_state.anomaly_dossier.entries
└─ 离开: 标记 location 已访问过, 可写入 player_mark / player_reserved_resources

黄昏(16:00-18:00)
├─ 林行整理背包
├─ 分配资源
├─ 据点建设/修理
└─ 保存林行结束状态

夜晚(18:00-05:00) — 祁眠操作(AI 或玩家)
├─ 构建 perceivable_state:
│   ├─ 最新 location_state
│   ├─ 未过期 player_mark
│   ├─ player_reserved_resources 的现场痕迹
│   └─ anomaly_tags / qimian_mark 等世界痕迹
├─ 祁眠 AI 决策(读取最新 location_state + perceivable_state)
├─ 执行任务:
│   ├─ 拿资源: location_state.remaining_xxx -= n
│   ├─ 引偏丧尸: location_state.zombie_count -= n(另一处 += n)
│   ├─ 撬锁: location_state.lock_state = "picked"
│   ├─ 留痕迹: location_state.qimian_trace = true
│   ├─ 写异常: location_state.anomaly_tags += [...]
│   ├─ 喷漆: location_state.qimian_mark = "◯" 等
│   └─ 解旗标: location_state.xxx_flag = true
├─ 区域热度更新
├─ 暴露值更新
└─ 保存祁眠结束状态

次日清晨 — 林行再次看到改写后的地点状态
```

---

## 五、关键共享案例的实现方式

### 案例 1：超市二次进入

```
Day 5 白天: 林行搜超市仓储区, 拿走食物×3 → location_state["supermarket"].remaining_food -= 3
Day 6 夜晚: 祁眠夜行到超市, 拿走仓储区剩余食物×2 → location_state["supermarket"].remaining_food -= 2
            祁眠留痕迹 → location_state["supermarket"].qimian_trace = true
Day 7 白天: 林行第二次来超市 → 大地图显示 qimian_trace ? 图标
            进入后仓储区食物比预期少, 且有被精确挑拣的痕迹(UI文本)
```

### 案例 2：清桥

```
Day 10 夜晚: 祁眠清桥 → location_state["bridge"].zombie_count = 0(从 15)
             location_state["bridge"].qimian_mark = "◯"
Day 11 白天: 林行大地图 → 桥的 danger_level 从 "high" 变为 "medium"
             路线耗时减少 10min
             幸存者营地新增传闻:"桥通了"
```

### 案例 3：匿名药品

```
Day 12 夜晚: 祁眠留药 → location_state["home"].qimian_trace = true(门阶上)
             location_state["home"].remaining_medicine += 2(临时加入, 无副作用)
Day 13 清晨: 林行开门 → 触发发现药品事件 → 精神变化 → 日记追加
             location_state["home"].remaining_medicine -= 2(林行捡走)
```

### 案例 4：区域热度对林行的影响

```
热度不直接让林行看到。但影响体现在:
- B 区热度 2+ → B 区地点 danger_level 可能有 1 级的临时上升
- C 区热度 3 → 祁眠今晚不在 C 区活动 → C 区地点没有 qimian_trace 变化
```

### 案例 5：诊所求助标记 → 匿名药品反馈

```
Day 6 白天: 林行进入诊所
            发现药柜被重新锁过:
            location_state["clinic"].anomaly_tags += ["picked_lock"]
            global_shared_state.anomaly_dossier.entries += {
                day: 6,
                location_id: "clinic",
                anomaly_tag: "picked_lock",
                evidence_text: "药柜锁孔旁有细小刮痕，但门没有被砸坏。",
                inference_tags: ["cautious"],
                verified: false
            }

            林行离开前留下求助标记:
            location_state["clinic"].player_mark = "help"
            location_state["clinic"].player_mark_day = 6
            location_state["clinic"].player_reserved_resources.medicine = 1

Day 6 夜晚: 构建祁眠 perceivable_state:
            player_marks += [{location: "clinic", type: "help", age_days: 0}]
            reserved_resources += [{location: "clinic", medicine: 1}]

            祁眠 AI 排序:
            help 标记提高 rescue_nearby / supply_drop / leave_medicine 权重
            最终选择 leave_medicine 或 rescue_nearby

            共享地图变化:
            location_state["clinic"].anomaly_tags += ["anonymous_medicine", "fluorescent_mark"]
            location_state["clinic"].remaining_medicine += 1
            location_state["clinic"].qimian_trace = true

Day 7 白天: 林行返回诊所或查看节点:
            发现未拆封抗生素和浅箭头标记
            anomaly_dossier 对 Day 6 条目 verified = true
            known_patterns += ["对方能理解幸存者标记", "对方不完全敌对"]
```

---

## 六、数据持久化

```
save_game = {
    timestamp: string,
    day: int,
    lin_state: { ... },           # 林行全部状态
    locations: {                  # 所有地点状态
        "home": { ... },
        "convenience_store": { ... },
        ...
    },
    global_state: { ... },        # 全局共享状态
    qimian_state: { ... },        # 祁眠状态(含日志历史)
    log_history: [                # 祁眠每日日志
        {day: 5, ...},
        {day: 6, ...},
        ...
    ]
}
```

一周目通关后 `qimian_state` 和 `log_history` 全部解锁展示。二周目祁眠可操作时从 `qimian_state` 读当前数据。
