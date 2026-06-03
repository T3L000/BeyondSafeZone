using System;
using System.Collections.Generic;
using System.Linq;
using BeyondSafeZone.Controllers;
using BeyondSafeZone.Data;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Core
{
    /// <summary>流程协调器 —— 对应 Godot game_simulation.gd</summary>
    public static class GameSimulation
    {
        // ============ 初始化 ============

        public static GameState NewGame()
        {
            var state = new GameState
            {
                Day = 1,
                Phase = "morning",
                Goal = "撤离到保护区",
                DemoComplete = false,
                EndingState = "in_progress",
                LastEvent = "林行在家中醒来。收音机里反复出现保护区断续广播。",
                MorningContext = new MorningContext(),
                AppliedDayEvents = new List<int>(),
                Lin = new LinState { Health = 10, Hunger = 0, Thirst = 0, Fatigue = 1, Stress = 2, InfectionRisk = 0, Hope = 4 },
                Resources = new ResourceState { Food = 5, Water = 5, Meds = 2, Materials = 4, Parts = 1, Fuel = 3 },
                Shelter = new ShelterState { Door = 4, Noise = 2, Scent = 2, Light = 2, Defense = 1, Escape = 0, SupplyPreservation = 0 },
                Bike = new BikeState { Durability = 6, Capacity = 6, Range = 1, Noise = 1 },
                Car = new CarState(),
                CarParts = new CarPartsState { Battery = 0, Gasoline = 0, Tire = 0 },
                Evacuation = new EvacuationState(),
                Exploration = new ExplorationState(),
                Qimian = new QimianState
                {
                    Awake = false,
                    Log = new List<QimianLogEntry>(),
                    PublicClues = new List<string>(),
                    PersonalityCard = new PersonalityCardState(),
                    AiState = new QimianAiState()
                },
                Locations = new Dictionary<string, LocationState>(),
                BloodMoonsResolved = new List<int>(),
                Reveal = new RevealState(),
                AnomalyDossier = new List<AnomalyDossierEntry>(),
                PlayerMarks = new Dictionary<string, PlayerMark>()
            };

            state.Shelter.Facilities = ShelterController.DefaultFacilities();
            BuildLocations(state);
            StartDay(state, 1);
            return state;
        }

        // ============ 日循环 ============

        public static bool IsBloodMoonDay(int day) => Constants.BLOOD_MOON_DAYS.Contains(day);

        public static DayEvent GetDayEvent(int day) => Events15dData.GetEvent(day);

        /// <summary>开始新的一天，返回当天事件文本</summary>
        public static string StartDay(GameState state, int day)
        {
            state.Day = day;
            if (state.Phase != "reveal")
                state.Phase = "morning";

            ShelterController.ResetFacilityUse(state);

            var evt = GetDayEvent(day);
            state.MorningContext = new MorningContext
            {
                Day = day,
                Text = evt.MorningText,
                PressureType = evt.PressureType,
                Clue = evt.Clue,
                BloodMoonWarning = evt.BloodMoonWarning
            };

            if (!state.AppliedDayEvents.Contains(day))
            {
                ApplyDayPressure(state, evt);
                state.AppliedDayEvents.Add(day);
            }

            state.LastEvent = $"第 {day} 天清晨。{evt.MorningText} {evt.Clue}";
            state.LastEvent += TextRenderer.DailyMonologue(state, day);
            return state.LastEvent;
        }

        // ============ View 文本委托 ============

        public static string GetLinConditionText(GameState state) =>
            TextRenderer.GetLinConditionText(state);

        public static string GetLocationLabel(GameState state, string locationId) =>
            TextRenderer.GetLocationLabel(state, locationId);

        public static string GetLocationCardText(GameState state, string locationId) =>
            TextRenderer.GetLocationCardText(state, locationId);

        public static string GetRoomCardText(GameState state, string roomId) =>
            TextRenderer.GetRoomCardText(state, roomId);

        public static string GetLocationRiskText(GameState state, string locationId) =>
            TextRenderer.GetLocationRiskText(state, locationId);

        public static string GetAnomalyDossierText(GameState state) =>
            TextRenderer.GetAnomalyDossierText(state);

        // ============ 玩家标记 (player_marks) ============

        /// <summary>林行在地图上留下标记</summary>
        public static void AddPlayerMark(GameState state, string locationId, string markType, string note)
        {
            // markType: danger / help / route / reserve
            var validTypes = new[] { "danger", "help", "route", "reserve" };
            if (!validTypes.Contains(markType)) markType = "help";

            state.PlayerMarks[locationId] = new PlayerMark
            {
                Type = markType,
                Day = state.Day,
                Note = note
            };
            state.LastEvent += $"\n林行在 {locationId} 留下了一个{MarkTypeLabel(markType)}标记：{note}";
        }

        /// <summary>移除某个地点的玩家标记</summary>
        public static void RemovePlayerMark(GameState state, string locationId)
        {
            state.PlayerMarks.Remove(locationId);
        }

        /// <summary>获取玩家标记链文本（结尾展示用）</summary>
        public static string GetPlayerMarkPerceptionChain(GameState state) =>
            TextRenderer.GetPlayerMarkPerceptionChain(state);

        private static string MarkTypeLabel(string type) => type switch
        {
            "danger" => "危险",
            "help" => "求助",
            "route" => "路线",
            "reserve" => "储备",
            _ => "?"
        };

        // ============ 探索委托 ============

        public static string EnterLocation(GameState state, string locationId) =>
            ExplorationController.EnterLocation(state, locationId);

        public static string SearchRoom(GameState state, string roomId, string tactic = "careful") =>
            ExplorationController.SearchRoom(state, roomId, tactic);

        public static string LureRoom(GameState state, string roomId) =>
            ExplorationController.LureRoom(state, roomId);

        public static string LeaveExploration(GameState state) =>
            ExplorationController.LeaveExploration(state);

        public static string Explore(GameState state, string locationId) =>
            ExplorationController.Explore(state, locationId);

        public static List<string> GetLocationIds(GameState state) =>
            state.Locations.Keys.ToList();

        // ============ 据点委托 ============

        public static string PerformShelterAction(GameState state, string actionId) =>
            ShelterController.PerformAction(state, actionId);

        // ============ 夜晚结算 ============

        public static string SleepAndResolveNight(GameState state)
        {
            int day = state.Day;
            QimianController.ResolveForDay(state, day);
            return NightController.Resolve(state);
        }

        // ============ 安全演示 ============

        public static void PlaySafeDemoDay(GameState state, int day)
        {
            StartDay(state, day);
            string locationId = SafeRouteData.GetLocationForDay(day);
            AutoSearchLocation(state, locationId);
            PerformShelterAction(state, SafeRouteData.GetActionForDay(day));
            SleepAndResolveNight(state);
        }

        // ============ 内部辅助 ============

        private static void BuildLocations(GameState state)
        {
            foreach (var locId in LocationData.LOCATION_DEFS.Keys)
                state.Locations[locId] = LocationData.BuildLocation(locId);
        }

        private static void ApplyDayPressure(GameState state, DayEvent evt)
        {
            if (evt.Modifiers == null) return;
            foreach (var kv in evt.Modifiers)
            {
                int amount = kv.Value;
                switch (kv.Key)
                {
                    case "food": state.Resources.Food = Math.Max(0, state.Resources.Food + amount); break;
                    case "water": state.Resources.Water = Math.Max(0, state.Resources.Water + amount); break;
                    case "stress": state.Lin.Stress = Math.Max(0, state.Lin.Stress + amount); break;
                    case "hope": state.Lin.Hope = Math.Max(0, state.Lin.Hope + amount); break;
                    case "door": state.Shelter.Door = Math.Max(0, state.Shelter.Door + amount); break;
                    case "noise": state.Shelter.Noise = Math.Max(0, state.Shelter.Noise + amount); break;
                    case "scent": state.Shelter.Scent = Math.Max(0, state.Shelter.Scent + amount); break;
                    case "bike_durability": state.Bike.Durability = Math.Max(0, state.Bike.Durability + amount); break;
                }
            }
        }

        private static void AutoSearchLocation(GameState state, string locationId)
        {
            EnterLocation(state, locationId);
            if (state.Phase != "searching") return;

            if (!state.Locations.TryGetValue(locationId, out var location)) return;

            int searchedCount = 0;
            foreach (var kv in location.Rooms)
            {
                if (searchedCount >= SafeRouteData.MAX_ROOMS_PER_LOCATION) break;
                var room = kv.Value;
                if (room.Searched) continue;
                if (room.Locked) continue;
                string roomId = kv.Key;
                if (room.HiddenZombies > 0) LureRoom(state, roomId);
                SearchRoom(state, roomId, SafeRouteData.SEARCH_TACTIC);
                searchedCount++;
            }
            LeaveExploration(state);
        }
    }
}
