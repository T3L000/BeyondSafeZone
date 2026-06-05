using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPrototypeController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text detailText;
    [SerializeField] private TMP_Text logText;

    [Header("Location Buttons")]
    [SerializeField] private Button shelterButton;
    [SerializeField] private Button clinicButton;
    [SerializeField] private Button supermarketButton;
    [SerializeField] private Button garageButton;

    [Header("Action Buttons")]
    [SerializeField] private Button carefulSearchButton;
    [SerializeField] private Button quickSearchButton;
    [SerializeField] private Button leaveHelpMarkButton;
    [SerializeField] private Button resolveNightButton;
    [SerializeField] private Button nextDayButton;

    private enum Phase { Daytime, Dusk, Night, Ended }

    private int day = 1;
    private Phase phase = Phase.Daytime;
    private string selectedLocation = "clinic";

    private int health = 5;
    private int fatigue = 0;
    private int stress = 0;
    private int infectionRisk = 0;

    private int food = 3;
    private int water = 3;
    private int medicine = 1;
    private int parts = 0;
    private int fuel = 0;

    private bool clinicAnomalyFound = false;
    private bool clinicHelpMark = false;
    private bool qimianReadHelpMark = false;
    private bool clinicAnonymousMedicine = false;

    private readonly List<string> logs = new List<string>();

    private void Start()
    {
        shelterButton.onClick.AddListener(() => SelectLocation("shelter"));
        clinicButton.onClick.AddListener(() => SelectLocation("clinic"));
        supermarketButton.onClick.AddListener(() => SelectLocation("supermarket"));
        garageButton.onClick.AddListener(() => SelectLocation("garage"));

        carefulSearchButton.onClick.AddListener(() => Search(true));
        quickSearchButton.onClick.AddListener(() => Search(false));
        leaveHelpMarkButton.onClick.AddListener(LeaveHelpMark);
        resolveNightButton.onClick.AddListener(ResolveNight);
        nextDayButton.onClick.AddListener(NextDay);

        AddLog("林行醒来，广播断断续续。");
        AddLog("当前目标：先搜刮物资，撑过今晚。");
        RefreshAll();
    }

    private void SelectLocation(string locationId)
    {
        selectedLocation = locationId;
        AddLog("选中地点：" + GetLocationName(locationId));
        RefreshAll();
    }

    private void Search(bool careful)
    {
        if (phase != Phase.Daytime)
        {
            AddLog("现在不能搜索。");
            return;
        }

        if (selectedLocation == "shelter")
        {
            AddLog("据点里没有新的可搜刮物资。");
            return;
        }

        if (selectedLocation == "clinic")
        {
            medicine += careful ? 1 : 2;
            infectionRisk += careful ? 0 : 1;
            AddLog(careful ? "你谨慎搜索诊所，找到 1 份药品。" : "你快速翻找诊所，找到 2 份药品，但划伤了手。");

            if (day >= 5 && !clinicAnomalyFound)
            {
                clinicAnomalyFound = true;
                AddLog("未知行动者档案 +1：药柜锁孔有新的撬动痕迹，但柜门没有被砸开。");
            }

            if (clinicAnonymousMedicine)
            {
                medicine += 1;
                clinicAnonymousMedicine = false;
                AddLog("你收起门口匿名留下的药包，药品 +1。");
            }
        }
        else if (selectedLocation == "supermarket")
        {
            food += careful ? 1 : 2;
            water += 1;
            fatigue += careful ? 1 : 2;
            AddLog(careful ? "你在超市找到食物和水。" : "你快速搜刮超市，拿到更多食物，但体力消耗更大。");
        }
        else if (selectedLocation == "garage")
        {
            parts += careful ? 1 : 2;
            fuel += careful ? 0 : 1;
            fatigue += 1;
            AddLog(careful ? "你在修理铺找到 1 个可用零件。" : "你翻开车库杂物，找到零件和少量燃料。");
        }

        phase = Phase.Dusk;
        RefreshAll();
    }

    private void LeaveHelpMark()
    {
        if (phase != Phase.Dusk || selectedLocation != "clinic" || day < 6 || !clinicAnomalyFound)
        {
            AddLog("现在还不能留下有效求助标记。");
            return;
        }

        clinicHelpMark = true;
        AddLog("你在诊所门边留下了一个很浅的求助标记：这里有人需要药。");
        RefreshAll();
    }

    private void ResolveNight()
    {
        if (phase != Phase.Dusk)
        {
            AddLog("需要先完成白天行动。");
            return;
        }

        phase = Phase.Night;

        food -= 1;
        water -= 1;

        if (food < 0)
        {
            food = 0;
            stress += 1;
            health -= 1;
            AddLog("夜里食物不足，林行状态变差。");
        }

        if (water < 0)
        {
            water = 0;
            stress += 1;
            health -= 1;
            AddLog("夜里饮水不足，林行状态变差。");
        }

        if (day >= 5)
        {
            AddLog("夜里，城市边缘传来很轻的摩托声。");
        }

        if (clinicHelpMark && !qimianReadHelpMark)
        {
            qimianReadHelpMark = true;
            clinicAnonymousMedicine = true;
            AddLog("祁眠 AI：感知到诊所求助标记，匿名留药权重上升。");
        }

        if (day >= 15)
        {
            phase = Phase.Ended;
            AddEndingLog();
        }

        RefreshAll();
    }

    private void NextDay()
    {
        if (phase != Phase.Night)
        {
            AddLog("还没有完成夜晚结算。");
            return;
        }

        day += 1;
        phase = Phase.Daytime;

        if (day == 5)
        {
            AddLog("未知行动者开始出现。部分地点状态可能被夜间改写。");
        }

        if (clinicAnonymousMedicine)
        {
            AddLog("次日，诊所门口多了一个用布包着的药品。");
            AddLog("未知行动者档案更新：对方似乎能理解求助标记，且不完全敌对。");
        }

        if (day == 15)
        {
            AddLog("最终血月临近，林行必须准备离开据点。");
        }

        RefreshAll();
    }

    private void AddEndingLog()
    {
        AddLog("=== 祁眠行动日志解锁 ===");
        AddLog("人格卡：寻找祁烬优先，谨慎避开暴露，会帮助近处的人。");
        AddLog("输入：诊所求助标记、药品剩余、低暴露风险。");
        AddLog("候选行动：搜刮、休整、匿名留药、追踪信号。");
        AddLog("最终选择：匿名留药。原因：近处有人需要药，且暴露风险可控。");
        AddLog("世界影响：诊所出现匿名药包，林行档案验证未知行动者存在。");
    }

    private void RefreshAll()
    {
        headerText.text = $"Day {day} / {GetPhaseText()} / {GetGoalText()}";

        statusText.text =
            $"生命：{health}\n疲劳：{fatigue}\n压力：{stress}\n感染风险：{infectionRisk}\n\n" +
            $"食物：{food}\n水：{water}\n药品：{medicine}\n零件：{parts}\n燃料：{fuel}";

        detailText.text = GetLocationDetail();

        int start = Mathf.Max(0, logs.Count - 12);
        logText.text = string.Join("\n", logs.GetRange(start, logs.Count - start));

        carefulSearchButton.interactable = phase == Phase.Daytime && selectedLocation != "shelter";
        quickSearchButton.interactable = phase == Phase.Daytime && selectedLocation != "shelter";
        leaveHelpMarkButton.interactable = phase == Phase.Dusk && selectedLocation == "clinic" && day >= 6 && clinicAnomalyFound && !clinicHelpMark;
        resolveNightButton.interactable = phase == Phase.Dusk;
        nextDayButton.interactable = phase == Phase.Night;
    }

    private string GetLocationDetail()
    {
        if (selectedLocation == "clinic")
        {
            string anomaly = clinicAnomalyFound ? "药柜有新撬锁痕" : "暂无";
            string mark = clinicHelpMark ? "已留下求助标记" : "无";
            string gift = clinicAnonymousMedicine ? "门口有匿名药包" : "无";
            return $"社区诊所\n危险：中\n资源倾向：药品\n异常：{anomaly}\n玩家标记：{mark}\n祁眠反馈：{gift}";
        }

        if (selectedLocation == "supermarket")
            return "小区超市\n危险：低\n资源倾向：食物 / 水\n异常：暂无";

        if (selectedLocation == "garage")
            return "修理铺/车库\n危险：中\n资源倾向：零件 / 燃料\n异常：暂无";

        return "林行家/据点\n危险：低\n用途：夜晚休整、整理物资、准备撤离";
    }

    private string GetLocationName(string id)
    {
        if (id == "clinic") return "社区诊所";
        if (id == "supermarket") return "小区超市";
        if (id == "garage") return "修理铺/车库";
        return "林行家/据点";
    }

    private string GetPhaseText()
    {
        if (phase == Phase.Daytime) return "白天";
        if (phase == Phase.Dusk) return "黄昏";
        if (phase == Phase.Night) return "夜晚";
        return "结尾";
    }

    private string GetGoalText()
    {
        if (day < 5) return "搜刮物资";
        if (day < 7) return "调查未知行动者";
        if (day < 15) return "准备撤离";
        return "最终血月";
    }

    private void AddLog(string text)
    {
        logs.Add("- " + text);
    }
}