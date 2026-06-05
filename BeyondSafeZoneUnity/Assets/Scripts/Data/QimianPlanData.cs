using System.Collections.Generic;

namespace BeyondSafeZone.Data
{
    /// <summary>祁眠行动定义</summary>
    public class QimianAction
    {
        public string Title = "";
        public string Location = "";
        public string Resource = "";
        public int Amount = 0;
        public int ZombieDelta = 0;
        public Dictionary<string, int> ResourceGain = new();
        public string PublicClue = "";
        public string Truth = "";
        public string AiReplay = "";
        public string SubjectiveFragment = "";
        public int BloodMoonSupport = 0;
    }

    /// <summary>祁眠固定日程计划 —— 对应 Godot qimian_plan.gd</summary>
    public static class QimianPlanData
    {
        public static readonly Dictionary<int, List<QimianAction>> PLAN = new()
        {
            [5] = new()
            {
                new() { Title="祁眠醒来", PublicClue="远处旧楼有一扇门从里面被打开，又被人小心合上。",
                    Truth="祁眠从感染昏睡中醒来，确认普通丧尸不会主动攻击自己，把寻找祁烬定为主任务。",
                    AiReplay="输入：自身感染异常、附近尸群无攻击反应。规则：先确认身体，再寻找祁烬，避免暴露。",
                    SubjectiveFragment="他们没有扑上来。我还活着，或者说，变成了另一种东西。" },
            },
            [6] = new()
            {
                new() { Title="诊所取药", Location="clinic", Resource="meds", Amount=-1,
                    PublicClue="社区诊所药柜被重新锁过，锁孔旁有新鲜刮痕。",
                    Truth="祁眠只拿走任务途中需要的药品，留下大部分绷带和消毒物。",
                    AiReplay="输入：可感知药品、低暴露风险。规则：只拿任务所需，不制造明显抢掠痕迹。",
                    SubjectiveFragment="够路上用就行。拿太多，只会让后来的人死得更快。" },
            },
            [8] = new()
            {
                new() { Title="超市夜行", Location="supermarket", Resource="food", Amount=-2,
                    PublicClue="超市货架像被人有计划地清过，最容易保存的食物少了一批。",
                    Truth="祁眠拿走一部分便携食物，同时避开会暴露身份的人类巡逻。",
                    AiReplay="输入：中圈食物、巡逻痕迹、寻找祁烬需要补给。规则：补给优先，避免接触。",
                    SubjectiveFragment="货架空一点，总比我被他们拖去筛查要好。" },
            },
            [10] = new()
            {
                new() { Title="夜晚骑摩托清桥", Location="school", ZombieDelta=-2,
                    PublicClue="去往学校方向的河道桥上，尸群在一夜之间稀疏了大半。桥面散落着未燃尽的照明棒和轮胎刹车痕。",
                    Truth="祁眠骑摩托在深夜抵达桥头，利用喇叭和燃烧物把尸群缓慢引离桥面，恢复了一条关键通行路线。",
                    AiReplay="输入：桥梁被尸群占据、阻断幸存者转移路线、夜间视野允许隐蔽操作。规则：不能正面清怪，用噪音和光源缓慢引导，避免目击。",
                    SubjectiveFragment="桥通了一条路。他们不知道是谁做的，这样最好。" },
            },
            [11] = new()
            {
                new() { Title="尸群偏移", Location="subway", ZombieDelta=-2,
                    PublicClue="地铁口的尸群少了，墙上有一条像是刻意留下的箭头。",
                    Truth="祁眠为了避开保护区探照灯，把尸群引到另一条街，也让陌生幸存者有了空隙。",
                    AiReplay="输入：探照灯、尸群、地铁口可穿行。规则：避免暴露，顺路降低近处幸存者风险。",
                    SubjectiveFragment="灯扫过来之前，尸群得先动。" },
            },
            [14] = new()
            {
                new() { Title="红潮夜观察", ResourceGain=new(){{"meds",1}},
                    PublicClue="诊所门口多了一支未拆封的抗生素，上面还贴着撕了一半的旧处方。",
                    Truth="祁眠在红潮夜前最后巡查近圈，确认返生计划巡逻队今夜不会进入林行所在街区，留下药品应急。",
                    AiReplay="输入：尸群迁移路线、巡逻队时间、近圈幸存者活动。规则：避开暴露，最后确认安全窗口。",
                    SubjectiveFragment="趁他们还缩在营地里，把这里最后一扇窗也关好。" },
            },
            [15] = new()
            {
                new() { Title="尸群藏身", BloodMoonSupport=2,
                    PublicClue="保护区外的尸群像被某个看不见的人牵走，给筛查棚前让出一条窄路。",
                    Truth="祁眠藏在尸群里改变路线，目标是避开探照灯和追查线索，间接让林行抵达筛查棚。",
                    AiReplay="输入：保护区探照灯、尸群密度、可借尸群掩护。规则：避开筛查，继续追查祁烬，不主动接触林行。",
                    SubjectiveFragment="那个人擦肩而过。我没有看清他的脸，也不能停。" },
                new() { Title="双层揭示",
                    PublicClue="筛查棚外有人低声议论：昨晚那股尸群像被人牵走了，还有人看到一辆摩托穿过了东线的封锁。",
                    Truth="林行只听见「尸群被牵走」「摩托穿东线」的异常线索，玩家随后看到祁眠完整行动回放。",
                    AiReplay="输入：林行不可见；系统结算共享世界影响。规则：玩家获得上帝视角，角色仍保持误认。",
                    SubjectiveFragment="如果那个在筛查棚排队的人是他，我也已经错过。但我不能停下来确认。" },
            },
        };
    }
}
