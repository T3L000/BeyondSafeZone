# 设施定义 —— 纯数据，零逻辑
class_name FacilityData

static func defaults() -> Dictionary:
	return {
		"bed": {"name": "床铺", "role": "recover", "level": 1, "used_today": false},
		"workbench": {"name": "工作台", "role": "craft_repair", "level": 1, "used_today": false},
		"barricade": {"name": "封窗", "role": "blood_moon_defense", "level": 1, "used_today": false},
		"radio": {"name": "收音机", "role": "broadcast_clues", "level": 1, "used_today": false},
		"storage": {"name": "储物/整理台", "role": "preserve_carry", "level": 1, "used_today": false}
	}
