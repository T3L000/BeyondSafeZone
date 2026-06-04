# 15 天逐日事件表 —— 纯数据，零逻辑
class_name DayEventData

const EVENTS := {
	1: {"day": 1, "morning_text": "林行在家中的旧沙发上醒来，桌上还压着童年画过的末日避难路线。", "pressure_type": "tutorial", "clue": "收音机里反复出现保护区断续广播。", "blood_moon_warning": "", "modifiers": {}},
	2: {"day": 2, "morning_text": "楼下有人翻过垃圾桶，瓶装水比昨天更难找。", "pressure_type": "scarcity", "clue": "便利店门口的玻璃碎得很整齐。", "blood_moon_warning": "", "modifiers": {"water": -1}},
	3: {"day": 3, "morning_text": "清晨有短促敲门声，门外只剩一串拖痕。", "pressure_type": "stress", "clue": "墙上多了一句保护区方向的粉笔字。", "blood_moon_warning": "", "modifiers": {"stress": 1}},
	4: {"day": 4, "morning_text": "自行车链条卡住了，远处广播却催促幸存者尽快转移。", "pressure_type": "mobility", "clue": "修理铺附近的尸群被什么声音吸引过。", "blood_moon_warning": "", "modifiers": {"bike_durability": -1}},
	5: {"day": 5, "morning_text": "雨停后气味闷在楼道里，据点开始暴露生活痕迹；城市另一端有人从感染昏睡中醒来。", "pressure_type": "qimian", "clue": "楼梯口能闻到潮湿血腥味。", "blood_moon_warning": "", "modifiers": {"scent": 1, "stress": 1}},
	6: {"day": 6, "morning_text": "月色比平时更红，收音机要求外围幸存者提前熄灯。", "pressure_type": "warning", "clue": "保护区广播第一次提到血月。", "blood_moon_warning": "明晚血月：门窗、防御、噪音和气味会决定据点能不能撑住。", "modifiers": {"noise": 1}},
	7: {"day": 7, "morning_text": "血月当天，街上几乎没有普通尸群的游荡声，像是在等夜晚。", "pressure_type": "blood_moon", "clue": "窗外的月亮还没升起，玻璃已经开始轻轻震动。", "blood_moon_warning": "今晚血月：这是第一次防守考试。", "modifiers": {"stress": 1}},
	8: {"day": 8, "morning_text": "血月过后，附近街区被翻得乱七八糟。", "pressure_type": "aftermath", "clue": "保护区广播说中圈仍有通行可能。", "blood_moon_warning": "", "modifiers": {"door": -1}},
	9: {"day": 9, "morning_text": "自行车还能撑一段路，但每一次远行都会留下更响的动静。", "pressure_type": "mobility", "clue": "废弃学校方向飘来断续铃声。", "blood_moon_warning": "", "modifiers": {"bike_durability": -1}},
	10: {"day": 10, "morning_text": "社区诊所的门被风吹开，里面安静得不正常。", "pressure_type": "foreshadow", "clue": "有个药柜像是被人从里面重新锁上。", "blood_moon_warning": "", "modifiers": {"stress": 1}},
	11: {"day": 11, "morning_text": "红潮的暗光从东边漫过来，整个街区异常安静。林行发现诊所门口多了一支没用完的消毒液——不像是被丢弃的。", "pressure_type": "red_tide", "clue": "窗外的暗红色比昨晚更浓，尸群的低吼渐渐靠近。", "blood_moon_warning": "红潮夜：夜晚压力开始升级，据点需要更安静、更坚固。", "modifiers": {"hope": 1, "stress": 1}},
	12: {"day": 12, "morning_text": "超市方向没有争抢声，只有货架被拖动后的空响。红潮密度再加一层，收音机警告避难所窗户不要透光。", "pressure_type": "red_tide", "clue": "最容易保存的食物像是被人有计划地拿走，但留下了更安全的路。", "blood_moon_warning": "红潮夜：噪音和光源会成为尸群的信号。", "modifiers": {"stress": 1, "noise": 1}},
	13: {"day": 13, "morning_text": "红潮夜连续第四天，地铁口的尸群突然稀了。墙上有一道像箭头的划痕，方向直指保护区。", "pressure_type": "red_tide", "clue": "那道箭头像是在避开探照灯——有人在红潮中移动得比丧尸还安静。", "blood_moon_warning": "明晚终局血月：据点撑不住时，只能抓住保护区短暂开放窗口。", "modifiers": {"hope": 1, "scent": 1}},
	14: {"day": 14, "morning_text": "收音机紧急广播：超大型尸潮将在24小时内抵达本区，所有幸存者立即向保护区转移。", "pressure_type": "red_tide", "clue": "据点墙壁不停震动——撑不过今晚了。东边地平线上浮现一道移动的黑线。", "blood_moon_warning": "明晚终局血月：据点撑不过下一夜，必须在血月降临前赶到保护区大门外。", "modifiers": {"stress": 2, "scent": 1, "door": -1}},
	15: {"day": 15, "morning_text": "收音机最后一次响了——然后陷入永久的沉默。窗外地平线上，一道黑线正在扩大。那是超大型尸潮，像潮水一样漫过来。", "pressure_type": "blood_moon", "clue": "林行把最后的背包扔进后备箱，发动了引擎。", "blood_moon_warning": "终局血月：撤离、故障、徒步、筛查、祁眠日志——全部在今夜收束。", "modifiers": {"stress": 3}}
}

static func get_event(day: int) -> Dictionary:
	if EVENTS.has(day):
		return EVENTS[day].duplicate(true)
	return {"day": day, "morning_text": "这一天还没有写入 Demo。", "pressure_type": "unknown", "clue": "没有新的线索。", "blood_moon_warning": "", "modifiers": {}}
