using System;
using System.Collections.Generic;

namespace BeyondSafeZone.Model
{
    /// <summary>林行状态</summary>
    [Serializable]
    public class LinState
    {
        public int Health = 10;
        public int Hunger = 0;
        public int Thirst = 0;
        public int Fatigue = 1;
        public int Stress = 2;
        public int InfectionRisk = 0;
        public int Hope = 4;
    }

    /// <summary>六类基础资源</summary>
    [Serializable]
    public class ResourceState
    {
        public int Food = 5;
        public int Water = 5;
        public int Meds = 2;
        public int Materials = 4;
        public int Parts = 1;
        public int Fuel = 3;

        public int Get(string key) => key switch
        {
            "food" => Food, "water" => Water, "meds" => Meds,
            "materials" => Materials, "parts" => Parts, "fuel" => Fuel,
            "medicine" => Meds, _ => 0
        };

        public void Set(string key, int value)
        {
            switch (key)
            {
                case "food": Food = value; break;
                case "water": Water = value; break;
                case "meds": case "medicine": Meds = value; break;
                case "materials": Materials = value; break;
                case "parts": Parts = value; break;
                case "fuel": Fuel = value; break;
            }
        }
    }

    /// <summary>据点状态</summary>
    [Serializable]
    public class ShelterState
    {
        public int Door = 4;
        public int Noise = 2;
        public int Scent = 2;
        public int Light = 2;
        public int Defense = 1;
        public int Escape = 0;
        public int SupplyPreservation = 0;
        public Dictionary<string, FacilityState> Facilities = new();
    }

    /// <summary>设施状态</summary>
    [Serializable]
    public class FacilityState
    {
        public string Name = "";
        public string Role = "";
        public int Level = 1;
        public bool Built = true;
        public bool UsedToday = false;
    }

    /// <summary>自行车状态</summary>
    [Serializable]
    public class BikeState
    {
        public int Durability = 6;
        public int Capacity = 6;
        public int Range = 1;
        public int Noise = 1;
    }

    /// <summary>汽车修理状态</summary>
    [Serializable]
    public class CarState
    {
        public bool Found = false;
        public bool Ready = false;
        public bool StepEngine = false;
        public bool StepTire = false;
        public bool StepBattery = false;
        public bool StepFueled = false;
        public string Breakdown = "";
    }

    /// <summary>汽车零件</summary>
    [Serializable]
    public class CarPartsState
    {
        public int Battery = 0;
        public int Gasoline = 0;
        public int Tire = 0;

        public int Get(string key) => key switch
        {
            "battery" => Battery, "gasoline" => Gasoline, "tire" => Tire, _ => 0
        };

        public void Set(string key, int value)
        {
            switch (key)
            {
                case "battery": Battery = value; break;
                case "gasoline": Gasoline = value; break;
                case "tire": Tire = value; break;
            }
        }
    }

    /// <summary>撤离旗标</summary>
    [Serializable]
    public class EvacuationState
    {
        public bool SafezoneConfirmed = false;
        public bool AddressKnown = false;
        public bool CarReady = false;
        public bool BikeReady = false;
    }

    /// <summary>探索状态</summary>
    [Serializable]
    public class ExplorationState
    {
        public string ActiveLocation = "";
        public int TimeUsed = 0;
        public int TimeLimit = 0;
        public int Noise = 0;
        public List<string> SearchedRooms = new();
        public List<string> LuredRooms = new();
    }

    /// <summary>人格卡</summary>
    [Serializable]
    public class PersonalityCardState
    {
        public string MainGoal = "寻找祁烬";
        public string Exposure = "谨慎，避免暴露";
        public string MoralRule = "会救近处的人，但不承担大规模救援";
        public string ResourceRule = "只拿任务需要的资源";
        public string SafezoneAttitude = "靠近观察，但不信任筛查";
    }

    /// <summary>祁眠 AI 状态</summary>
    [Serializable]
    public class QimianAiState
    {
        public int Exposure = 0;
        public int MotoTier = 1;
        public Dictionary<string, int> ZoneHeat = new() { {"A", 0}, {"B", 0}, {"C", 0} };
        public int QijinClues = 0;
        public List<string> RescuedNpc = new();
        public ResourceState Inventory = new() { Food = 0, Water = 0, Meds = 1, Materials = 0, Parts = 1, Fuel = 1 };
    }

    /// <summary>祁眠状态</summary>
    [Serializable]
    public class QimianState
    {
        public bool Awake = false;
        public List<QimianLogEntry> Log = new();
        public List<string> PublicClues = new();
        public PersonalityCardState PersonalityCard = new();
        public QimianAiState AiState = new();
    }

    /// <summary>祁眠日志条目</summary>
    [Serializable]
    public class QimianLogEntry
    {
        public int Day;
        public string Title = "";
        public string Truth = "";
        public string PublicClue = "";
        public string AiReplay = "";
        public string SubjectiveFragment = "";
    }

    /// <summary>房间状态</summary>
    [Serializable]
    public class RoomState
    {
        public string Name = "";
        public string Visibility = "";
        public int SearchTime = 0;
        public int HiddenZombies = 0;
        public Dictionary<string, int> Resources = new();
        public List<string> Flags = new();
        public bool Locked = false;
        public bool Searched = false;
    }

    /// <summary>地点状态</summary>
    [Serializable]
    public class LocationState
    {
        public string Name = "";
        public string Ring = "";
        public int Range = 0;
        public int Zombies = 0;
        public Dictionary<string, int> Resources = new();
        public string ResourceTendency = "";
        public string DangerLevel = "";
        public int RouteTime = 0;
        public string RoadCondition = "";
        public List<string> Icons = new();
        public bool QimianTrace = false;
        public Dictionary<string, RoomState> Rooms = new();
        public bool Visited = false;
    }

    /// <summary>清晨事件上下文</summary>
    [Serializable]
    public class MorningContext
    {
        public int Day;
        public string Text = "";
        public string PressureType = "";
        public string Clue = "";
        public string BloodMoonWarning = "";
    }

    /// <summary>异常档案条目</summary>
    [Serializable]
    public class AnomalyDossierEntry
    {
        public int Day;
        public string LocationId = "";
        public string ClueText = "";
        public string Conclusion = "";
    }

    /// <summary>玩家标记</summary>
    [Serializable]
    public class PlayerMark
    {
        public string Type = "";  // danger/help/route/reserve
        public int Day;
        public string Note = "";
    }

    /// <summary>通关揭示</summary>
    [Serializable]
    public class RevealState
    {
        public bool Unlocked = false;
        public string Summary = "";
    }

    /// <summary>据点行动可用性查询结果（只读，不修改 GameState）</summary>
    [Serializable]
    public struct ShelterActionAvailability
    {
        public bool Available;
        public string ActionId;
        public string FailureReason; // 可用时为空字符串

        public static ShelterActionAvailability Ok(string actionId) => new()
        {
            Available = true, ActionId = actionId, FailureReason = ""
        };

        public static ShelterActionAvailability Fail(string actionId, string reason) => new()
        {
            Available = false, ActionId = actionId, FailureReason = reason
        };
    }

    /// <summary>搜刮行动可用性查询结果（只读，不修改 GameState）</summary>
    [Serializable]
    public struct ExplorationActionAvailability
    {
        public bool Available;
        public string ActionId;
        public string FailureReason; // 可用时为空字符串

        public static ExplorationActionAvailability Ok(string actionId) => new()
        {
            Available = true, ActionId = actionId, FailureReason = ""
        };

        public static ExplorationActionAvailability Fail(string actionId, string reason) => new()
        {
            Available = false, ActionId = actionId, FailureReason = reason
        };
    }

    /// <summary>日阶段行动可用性查询结果（resolve_night / next_day，只读）</summary>
    [Serializable]
    public struct DayPhaseActionAvailability
    {
        public bool Available;
        public string ActionId;
        public string FailureReason; // 可用时为空字符串

        public static DayPhaseActionAvailability Ok(string actionId) => new()
        {
            Available = true, ActionId = actionId, FailureReason = ""
        };

        public static DayPhaseActionAvailability Fail(string actionId, string reason) => new()
        {
            Available = false, ActionId = actionId, FailureReason = reason
        };
    }

    /// <summary>游戏状态（Model 层根对象）</summary>
    [Serializable]
    public class GameState
    {
        public int Day = 1;
        public string Phase = "morning";
        public string Goal = "撤离到保护区";
        public bool DemoComplete = false;
        public string EndingState = "in_progress";
        public string LastEvent = "";
        public MorningContext MorningContext = new();
        public List<int> AppliedDayEvents = new();

        public LinState Lin = new();
        public ResourceState Resources = new();
        public ShelterState Shelter = new();
        public BikeState Bike = new();
        public CarState Car = new();
        public CarPartsState CarParts = new();
        public EvacuationState Evacuation = new();
        public Dictionary<string, LocationState> Locations = new();
        public ExplorationState Exploration = new();
        public QimianState Qimian = new();

        public List<int> BloodMoonsResolved = new();
        public RevealState Reveal = new();

        public List<AnomalyDossierEntry> AnomalyDossier = new();
        public Dictionary<string, PlayerMark> PlayerMarks = new();
    }
}
