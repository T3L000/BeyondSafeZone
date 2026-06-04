# 游戏状态定义 —— Model 层，纯数据，零逻辑
# 对应 docs/planning_package/04_详细策划案.md 程序模块 - 关键状态字段
class_name GameState extends RefCounted

# ---- 全局进度 ----
var day: int = 1
var phase: String = "morning"
var goal: String = "撤离到保护区"
var demo_complete: bool = false
var ending_state: String = "in_progress"
var last_event: String = ""
var morning_context: Dictionary = {}
var applied_day_events: Array = []

# ---- 林行状态 ----
var lin: Dictionary = {}          # health/hunger/thirst/fatigue/stress/infection_risk/hope

# ---- 资源 ----
var resources: Dictionary = {}    # food/water/meds/materials/parts/fuel

# ---- 据点 ----
var shelter: Dictionary = {}      # door/noise/scent/light/defense/escape/supply_preservation/facilities

# ---- 载具 ----
var bike: Dictionary = {}         # durability/capacity/range/noise
var car: Dictionary = {}          # found/ready/step_engine/step_tire/step_battery/step_fueled/breakdown
var car_parts: Dictionary = {}    # battery/gasoline/tire

# ---- 撤离 ----
var evacuation: Dictionary = {}   # safezone_confirmed/address_known/car_ready/bike_ready

# ---- 世界地点 ----
var locations: Dictionary = {}    # 14 地点，含 rooms 子字典

# ---- 探索 ----
var exploration: Dictionary = {}  # active_location/time_used/time_limit/noise/searched_rooms/lured_rooms

# ---- 祁眠 AI ----
var qimian: Dictionary = {}       # awake/log/public_clues/personality_card/ai_state

# ---- 血月/结局 ----
var blood_moons_resolved: Array = []
var reveal: Dictionary = {}       # unlocked/summary

# ---- 一周目 AI 可读互动系统 (planning_package 03/04) ----
var anomaly_dossier: Array = []   # [{day, location_id, clue_text, conclusion}]
var player_marks: Dictionary = {} # {location_id: {type, day, note}}  type: danger/help/route/reserve

