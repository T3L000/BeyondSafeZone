using System.Collections.Generic;

namespace BeyondSafeZone.Data
{
    /// <summary>每日事件定义</summary>
    public class DayEvent
    {
        public int Day;
        public string MorningText = "";
        public string PressureType = "";
        public string Clue = "";
        public string BloodMoonWarning = "";
        public Dictionary<string, int> Modifiers = new();
    }

    /// <summary>15 天逐日事件表 —— 对应 Godot events_15d.gd</summary>
    public static class Events15dData
    {
        public static readonly Dictionary<int, DayEvent> EVENTS = new()
        {
            [1] = new() { Day=1, MorningText="林行在家中的旧沙发上醒来，桌上还压着童年画过的末日避难路线。", PressureType="tutorial", Clue="收音机里反复出现保护区断续广播。", BloodMoonWarning="" },
            [2] = new() { Day=2, MorningText="楼下有人翻过垃圾桶，瓶装水比昨天更难找。", PressureType="scarcity", Clue="便利店门口的玻璃碎得很整齐。", BloodMoonWarning="", Modifiers = new() {{"water", -1}} },
            [3] = new() { Day=3, MorningText="清晨有短促敲门声，门外只剩一串拖痕。", PressureType="stress", Clue="墙上多了一句保护区方向的粉笔字。", BloodMoonWarning="", Modifiers = new() {{"stress", 1}} },
            [4] = new() { Day=4, MorningText="自行车链条卡住了，远处广播却催促幸存者尽快转移。", PressureType="mobility", Clue="修理铺附近的尸群被什么声音吸引过。", BloodMoonWarning="", Modifiers = new() {{"bike_durability", -1}} },
            [5] = new() { Day=5, MorningText="雨停后气味闷在楼道里，据点开始暴露生活痕迹；城市另一端有人从感染昏睡中醒来。", PressureType="qimian", Clue="楼梯口能闻到潮湿血腥味。", BloodMoonWarning="", Modifiers = new() {{"scent", 1}, {"stress", 1}} },
            [6] = new() { Day=6, MorningText="月色比平时更红，收音机要求外围幸存者提前熄灯。", PressureType="warning", Clue="保护区广播第一次提到血月。", BloodMoonWarning="明晚血月：门窗、防御、噪音和气味会决定据点能不能撑住。", Modifiers = new() {{"noise", 1}} },
            [7] = new() { Day=7, MorningText="血月当天，街上几乎没有普通尸群的游荡声，像是在等夜晚。", PressureType="blood_moon", Clue="窗外的月亮还没升起，玻璃已经开始轻轻震动。", BloodMoonWarning="今晚血月：这是第一次防守考试。", Modifiers = new() {{"stress", 1}} },
            [8] = new() { Day=8, MorningText="血月过后，附近街区被翻得乱七八糟。", PressureType="aftermath", Clue="保护区广播说中圈仍有通行可能。", BloodMoonWarning="", Modifiers = new() {{"door", -1}} },
            [9] = new() { Day=9, MorningText="自行车还能撑一段路，但每一次远行都会留下更响的动静。", PressureType="mobility", Clue="废弃学校方向飘来断续铃声。", BloodMoonWarning="", Modifiers = new() {{"bike_durability", -1}} },
            [10] = new() { Day=10, MorningText="社区诊所的门被风吹开，里面安静得不正常。", PressureType="foreshadow", Clue="有个药柜像是被人从里面重新锁上。", BloodMoonWarning="", Modifiers = new() {{"stress", 1}} },
            [11] = new() { Day=11, MorningText="红潮的暗光从东边漫过来，整个街区异常安静。林行发现诊所门口多了一支没用完的消毒液——不像是被丢弃的。", PressureType="red_tide", Clue="窗外的暗红色比昨晚更浓，尸群的低吼渐渐靠近。", BloodMoonWarning="红潮夜：夜晚压力开始升级，据点需要更安静、更坚固。", Modifiers = new() {{"hope", 1}, {"stress", 1}} },
            [12] = new() { Day=12, MorningText="超市方向没有争抢声，只有货架被拖动后的空响。红潮密度再加一层，收音机警告避难所窗户不要透光。", PressureType="red_tide", Clue="最容易保存的食物像是被人有计划地拿走，但留下了更安全的路。", BloodMoonWarning="红潮夜：噪音和光源会成为尸群的信号。", Modifiers = new() {{"stress", 1}, {"noise", 1}} },
            [13] = new() { Day=13, MorningText="红潮夜连续第四天，地铁口的尸群突然稀了。墙上有一道像箭头的划痕，方向直指保护区。", PressureType="red_tide", Clue="那道箭头像是在避开探照灯——有人在红潮中移动得比丧尸还安静。", BloodMoonWarning="明晚终局血月：据点撑不住时，只能抓住保护区短暂开放窗口。", Modifiers = new() {{"hope", 1}, {"scent", 1}} },
            [14] = new() { Day=14, MorningText="收音机紧急广播：超大型尸潮将在24小时内抵达本区，所有幸存者立即向保护区转移。", PressureType="red_tide", Clue="据点墙壁不停震动——撑不过今晚了。东边地平线上浮现一道移动的黑线。", BloodMoonWarning="明晚终局血月：据点撑不过下一夜，必须在血月降临前赶到保护区大门外。", Modifiers = new() {{"stress", 2}, {"scent", 1}, {"door", -1}} },
            [15] = new() { Day=15, MorningText="收音机最后一次响了——然后陷入永久的沉默。窗外地平线上，一道黑线正在扩大。那是超大型尸潮，像潮水一样漫过来。", PressureType="blood_moon", Clue="林行把最后的背包扔进后备箱，发动了引擎。", BloodMoonWarning="终局血月：撤离、故障、徒步、筛查、祁眠日志——全部在今夜收束。", Modifiers = new() {{"stress", 3}} }
        };

        public static DayEvent GetEvent(int day)
        {
            if (EVENTS.TryGetValue(day, out var evt))
                return evt;
            return new DayEvent { Day = day, MorningText = "这一天还没有写入 Demo。", PressureType = "unknown", Clue = "没有新的线索。" };
        }
    }
}
