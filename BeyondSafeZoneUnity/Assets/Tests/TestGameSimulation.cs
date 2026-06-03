using NUnit.Framework;
using System.Linq;
using BeyondSafeZone.Controllers;
using BeyondSafeZone.Core;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Tests
{
    /// <summary>28 个测试方法 —— 对应 Godot test_game_simulation.gd</summary>
    [TestFixture]
    public class TestGameSimulation
    {
        private GameState _state;

        [SetUp]
        public void SetUp()
        {
            _state = GameSimulation.NewGame();
        }

        #region State & Init Tests

        [Test]
        public void TestInitialGoalAndStats()
        {
            Assert.AreEqual(1, _state.Day, "new game starts on day 1");
            Assert.AreEqual("撤离到保护区", _state.Goal, "Lin Xing's main goal is evacuation");
            Assert.IsNotNull(_state.Lin, "state uses Lin Xing as the playable character key");
            Assert.Greater(_state.Resources.Food, 0, "player starts with food");
            Assert.Greater(_state.Shelter.Door, 0, "shelter starts with a usable door");
            Assert.IsFalse(_state.Qimian.Awake, "Qimian starts asleep");
            Assert.AreEqual("in_progress", _state.EndingState, "new game starts without an ending");
            Assert.Greater(_state.MorningContext.Text.Length, 0, "new game has a morning context");
            Assert.IsTrue(_state.LastEvent.Contains("林行"), "opening text names Lin Xing");
            Assert.IsTrue(_state.LastEvent.Contains("家"), "opening starts from home");
        }

        [Test]
        public void TestLinConditionText()
        {
            string initialText = GameSimulation.GetLinConditionText(_state);
            Assert.IsTrue(initialText.Contains("感染风险：低"), "new game reports low infection risk");

            _state.Lin.InfectionRisk = 3;
            Assert.IsTrue(GameSimulation.GetLinConditionText(_state).Contains("发热风险"),
                "infection risk 3 reports fever risk");

            _state.Lin.InfectionRisk = 5;
            Assert.IsTrue(GameSimulation.GetLinConditionText(_state).Contains("危险感染"),
                "infection risk 5 reports dangerous infection");
        }

        [Test]
        public void TestResourcesAndEvacuation()
        {
            Assert.Greater(_state.Resources.Fuel, 0, "fuel is a core resource");
            Assert.AreEqual(0, _state.CarParts.Battery, "batteries are part of car_parts not core resources");
            Assert.IsFalse(_state.Evacuation.SafezoneConfirmed, "safe zone starts unconfirmed");
            Assert.IsFalse(_state.Evacuation.AddressKnown, "safe zone address starts unknown");
            Assert.IsFalse(_state.Evacuation.BikeReady, "bike starts not ready for the gate run");
        }

        [Test]
        public void TestShelterFacilities()
        {
            var f = _state.Shelter.Facilities;
            Assert.IsTrue(f.ContainsKey("bed"), "bed facility exists");
            Assert.IsTrue(f.ContainsKey("workbench"), "workbench facility exists");
            Assert.IsTrue(f.ContainsKey("barricade"), "window barricade facility exists");
            Assert.IsTrue(f.ContainsKey("radio"), "radio facility exists");
            Assert.IsTrue(f.ContainsKey("storage"), "storage table facility exists");

            Assert.AreEqual("recover", f["bed"].Role, "bed recovers fatigue and stress");
            Assert.AreEqual("craft_repair", f["workbench"].Role, "workbench handles bike and tools");
            Assert.AreEqual("blood_moon_defense", f["barricade"].Role, "barricade handles blood moon defense");
            Assert.AreEqual("broadcast_clues", f["radio"].Role, "radio handles broadcast clues");
            Assert.AreEqual("preserve_carry", f["storage"].Role, "storage preserves supplies");
        }

        #endregion

        #region Day Event & Blood Moon Tests

        [Test]
        public void TestDayEventTable()
        {
            for (int day = 1; day <= 15; day++)
            {
                var evt = GameSimulation.GetDayEvent(day);
                Assert.AreEqual(day, evt.Day, $"day event table includes day {day}");
                Assert.Greater(evt.MorningText.Length, 0, $"day {day} has morning text");
                Assert.Greater(evt.PressureType.Length, 0, $"day {day} has pressure type");
                Assert.Greater(evt.Clue.Length, 0, $"day {day} has clue text");
            }
        }

        [Test]
        public void TestBloodMoonDays()
        {
            Assert.IsTrue(GameSimulation.IsBloodMoonDay(7), "day 7 is a blood moon");
            Assert.IsTrue(GameSimulation.IsBloodMoonDay(15), "day 15 is a blood moon");
            Assert.IsFalse(GameSimulation.IsBloodMoonDay(14), "day 14 is a red-tide night, not a blood moon");
            Assert.IsFalse(GameSimulation.IsBloodMoonDay(10), "day 10 is not a blood moon");
        }

        [Test]
        public void TestBloodMoonWarnings()
        {
            GameSimulation.StartDay(_state, 7);
            Assert.Greater(_state.MorningContext.BloodMoonWarning.Length, 0, "day 7 exposes blood moon warning");

            GameSimulation.StartDay(_state, 15);
            Assert.Greater(_state.MorningContext.BloodMoonWarning.Length, 0, "day 15 exposes blood moon warning");
        }

        [Test]
        public void TestMorningPressureAppliesOnce()
        {
            GameSimulation.NewGame();  // reset
            GameSimulation.StartDay(_state, 5);
            int firstStress = _state.Lin.Stress;
            int firstWater = _state.Resources.Water;

            GameSimulation.StartDay(_state, 5);
            Assert.AreEqual(firstStress, _state.Lin.Stress, "day pressure does not stack when morning refreshes");
            Assert.AreEqual(firstWater, _state.Resources.Water, "day resource pressure does not stack when morning refreshes");
        }

        #endregion

        #region Exploration Tests

        [Test]
        public void TestExplorationMarksLocationAndReportsRisk()
        {
            string labelBefore = GameSimulation.GetLocationLabel(_state, "convenience");
            Assert.IsTrue(labelBefore.Contains("未搜"), "unvisited locations are labelled");

            string result = GameSimulation.Explore(_state, "convenience");
            string labelAfter = GameSimulation.GetLocationLabel(_state, "convenience");

            Assert.IsTrue(_state.Locations["convenience"].Visited, "exploring marks location visited");
            Assert.AreEqual("evening", _state.Phase, "exploring advances to evening");
            Assert.IsTrue(result.Contains("风险"), "exploration reports deterministic risk text");
            Assert.IsTrue(labelAfter.Contains("已搜"), "visited locations are labelled");
        }

        [Test]
        public void TestLocationsExposeNodeMapMetadata()
        {
            foreach (string locationId in GameSimulation.GetLocationIds(_state))
            {
                var location = _state.Locations[locationId];
                Assert.IsNotNull(location.ResourceTendency, $"{locationId} has resource tendency metadata");
                Assert.Greater(location.ResourceTendency.Length, 0, $"{locationId} resource tendency is readable");
                Assert.IsNotNull(location.DangerLevel, $"{locationId} has danger level metadata");
                Assert.Greater(location.DangerLevel.Length, 0, $"{locationId} danger level is readable");
                Assert.Greater(location.RouteTime, 0, $"{locationId} route time is positive");
                Assert.IsNotNull(location.RoadCondition, $"{locationId} has road condition metadata");
                Assert.Greater(location.RoadCondition.Length, 0, $"{locationId} road condition is readable");
                Assert.Greater(location.Icons.Count, 0, $"{locationId} exposes at least one map icon");
            }
        }

        [Test]
        public void TestLocationCardText()
        {
            string cardText = GameSimulation.GetLocationCardText(_state, "police");
            Assert.IsTrue(cardText.Contains("资源倾向"), "location card shows resource tendency");
            Assert.IsTrue(cardText.Contains("危险等级"), "location card shows danger level");
            Assert.IsTrue(cardText.Contains("路况"), "location card shows road condition");
            Assert.IsTrue(cardText.Contains("小时"), "location card shows route time in hours");
            Assert.IsTrue(cardText.Contains("地点特征"), "location card shows map icon descriptions");
            Assert.IsTrue(cardText.Contains("燃料"), "police card shows its resource tendency");
            Assert.IsTrue(cardText.Contains("路障"), "police card shows blocked road condition");
        }

        [Test]
        public void TestExplorationRevealsEvacuationAddress()
        {
            _state.Bike.Range = 2;
            Assert.IsFalse(_state.Evacuation.AddressKnown, "safe-zone address starts unknown");

            string result = GameSimulation.Explore(_state, "police");
            Assert.IsTrue(_state.Evacuation.AddressKnown, "police map node can reveal the safe-zone address");
            Assert.IsTrue(result.Contains("地址"), "exploration result calls out the address clue");
        }

        [Test]
        public void TestBadRoadConditionsIncreaseFatigue()
        {
            _state.Bike.Range = 2;
            int startingFatigue = _state.Lin.Fatigue;

            string result = GameSimulation.Explore(_state, "bike_shop");
            Assert.GreaterOrEqual(_state.Lin.Fatigue, startingFatigue + 2,
                "blocked road adds deterministic fatigue beyond route distance");
            Assert.IsTrue(result.Contains("路况"), "exploration result reports road-condition pressure");
        }

        [Test]
        public void TestQimianActionsMarkAffectedNodes()
        {
            GameSimulation.NewGame();
            // Advance to day 6 and manually resolve Qimian
            _state.Day = 6;
            QimianController.ResolveForDay(_state, 6);

            Assert.IsTrue(_state.Locations["clinic"].QimianTrace, "Qimian's clinic action marks the clinic node");
            Assert.IsTrue(_state.Locations["clinic"].Icons.Contains("qimian"), "Qimian trace adds a map icon");
            Assert.IsTrue(GameSimulation.GetLocationCardText(_state, "clinic").Contains("祁眠异常"),
                "location card surfaces Qimian trace");
        }

        [Test]
        public void TestLocationsExposeIndoorSearchRooms()
        {
            foreach (string locationId in new[] { "convenience", "clinic", "supermarket" })
            {
                var location = _state.Locations[locationId];
                Assert.GreaterOrEqual(location.Rooms.Count, 2, $"{locationId} has at least two room choices");
                foreach (var kv in location.Rooms)
                {
                    Assert.Greater(kv.Value.Name.Length, 0, $"{locationId}/{kv.Key} has a readable name");
                    Assert.Greater(kv.Value.Visibility.Length, 0, $"{locationId}/{kv.Key} has visibility metadata");
                    Assert.Greater(kv.Value.SearchTime, 0, $"{locationId}/{kv.Key} has deterministic search time");
                    Assert.IsNotNull(kv.Value.Resources, $"{locationId}/{kv.Key} has room resources");
                }
            }
        }

        [Test]
        public void TestEnterLocationStartsIndoorSearch()
        {
            string result = GameSimulation.EnterLocation(_state, "convenience");
            Assert.AreEqual("searching", _state.Phase, "entering a location starts indoor searching");
            Assert.AreEqual("convenience", _state.Exploration.ActiveLocation, "active exploration records the selected node");
            Assert.Greater(_state.Exploration.TimeLimit, 0, "indoor search has a time limit");
            Assert.IsTrue(result.Contains("进入"), "enter location reports entry text");
            Assert.IsTrue(GameSimulation.GetRoomCardText(_state, "storefront").Contains("能见度"),
                "room card surfaces visibility");
        }

        [Test]
        public void TestRoomCardReportsDarkRiskAndLureState()
        {
            GameSimulation.EnterLocation(_state, "clinic");

            string beforeLure = GameSimulation.GetRoomCardText(_state, "pharmacy");
            Assert.IsTrue(beforeLure.Contains("黑暗"), "dark room card reports shadow risk");
            Assert.IsTrue(beforeLure.Contains("未排除"), "room card reports hidden threat not yet cleared");

            GameSimulation.LureRoom(_state, "pharmacy");
            string afterLure = GameSimulation.GetRoomCardText(_state, "pharmacy");
            Assert.IsTrue(afterLure.Contains("已引开"), "room card reports lured hidden threat");
        }

        [Test]
        public void TestSearchRoomCollectsResources()
        {
            GameSimulation.EnterLocation(_state, "convenience");
            int startingFood = _state.Resources.Food;

            string result = GameSimulation.SearchRoom(_state, "storefront", "careful");
            Assert.Greater(_state.Resources.Food, startingFood, "searching a stocked room collects deterministic resources");
            Assert.IsTrue(_state.Locations["convenience"].Rooms["storefront"].Searched, "searched room is marked");
            Assert.IsTrue(result.Contains("带回") || result.Contains("搜到"), "room search reports resource pickup");

            GameSimulation.LeaveExploration(_state);
            Assert.AreEqual("evening", _state.Phase, "leaving indoor search advances to evening");
            Assert.IsTrue(_state.Locations["convenience"].Visited, "leaving marks the location visited");
            Assert.AreEqual("", _state.Exploration.ActiveLocation, "leaving clears active exploration");
        }

        [Test]
        public void TestDarkHiddenZombieRoomCausesInjury()
        {
            GameSimulation.EnterLocation(_state, "clinic");
            int startingHealth = _state.Lin.Health;
            int startingInfection = _state.Lin.InfectionRisk;

            string result = GameSimulation.SearchRoom(_state, "pharmacy", "quick");
            Assert.Less(_state.Lin.Health, startingHealth, "rushing a dark hidden-zombie room hurts Lin Xing");
            Assert.Greater(_state.Lin.InfectionRisk, startingInfection, "hidden-zombie contact raises infection risk");
            Assert.IsTrue(result.Contains("隐藏尸群"), "search result reports hidden-zombie contact");
        }

        [Test]
        public void TestNoiseLureReducesInjury()
        {
            GameSimulation.EnterLocation(_state, "clinic");
            int startingHealth = _state.Lin.Health;

            string lureResult = GameSimulation.LureRoom(_state, "pharmacy");
            string searchResult = GameSimulation.SearchRoom(_state, "pharmacy", "careful");

            Assert.AreEqual(startingHealth, _state.Lin.Health, "luring before search avoids direct injury");
            Assert.GreaterOrEqual(_state.Exploration.TimeUsed, 2, "luring and searching spend time");
            Assert.Greater(_state.Exploration.Noise, 0, "luring raises local noise");
            Assert.IsTrue(lureResult.Contains("制造噪音"), "lure action reports noise");
            Assert.IsTrue(searchResult.Contains("已被引开"), "search reports the threat was lured");
        }

        [Test]
        public void TestOverstayingIndoorSearchAddsFatigue()
        {
            _state.Bike.Range = 2;
            GameSimulation.EnterLocation(_state, "supermarket");
            int startingFatigue = _state.Lin.Fatigue;

            GameSimulation.SearchRoom(_state, "checkout", "careful");
            GameSimulation.LureRoom(_state, "storage");
            GameSimulation.SearchRoom(_state, "storage", "careful");
            GameSimulation.LureRoom(_state, "food_aisle");
            string result = GameSimulation.LeaveExploration(_state);

            Assert.Greater(_state.Lin.Fatigue, startingFatigue, "overstaying indoor search adds fatigue");
            Assert.IsTrue(result.Contains("天色"), "leaving after overstaying reports time pressure");
        }

        #endregion

        #region Night & Shelter Tests

        [Test]
        public void TestHighInfectionRiskWorsensAtNight()
        {
            _state.Phase = "evening";
            _state.Lin.InfectionRisk = 5;
            int startingHealth = _state.Lin.Health;
            int startingStress = _state.Lin.Stress;

            string result = GameSimulation.SleepAndResolveNight(_state);

            Assert.Less(_state.Lin.Health, startingHealth, "dangerous infection costs health during night resolution");
            Assert.Greater(_state.Lin.Stress, startingStress, "dangerous infection raises stress during night resolution");
            Assert.IsTrue(result.Contains("感染"), "night result reports infection pressure");
        }

        [Test]
        public void TestTreatWoundSpendsMedicineAndReducesInfection()
        {
            _state.Phase = "evening";
            _state.Lin.Health = 7;
            _state.Lin.InfectionRisk = 4;
            _state.Resources.Meds = 2;

            string result = GameSimulation.PerformShelterAction(_state, "treat_wound");
            Assert.AreEqual(1, _state.Resources.Meds, "treat wound spends one medicine");
            Assert.AreEqual(8, _state.Lin.Health, "treat wound restores one health");
            Assert.AreEqual(3, _state.Lin.InfectionRisk, "treat wound reduces infection risk");
            Assert.IsTrue(result.Contains("处理伤口"), "treat wound reports wound treatment");
        }

        [Test]
        public void TestTreatWoundWithoutMedicine()
        {
            _state.Phase = "evening";
            _state.Lin.Health = 6;
            _state.Lin.InfectionRisk = 3;
            _state.Resources.Meds = 0;

            string result = GameSimulation.PerformShelterAction(_state, "treat_wound");
            Assert.AreEqual(6, _state.Lin.Health, "failed wound treatment does not change health");
            Assert.AreEqual(3, _state.Lin.InfectionRisk, "failed wound treatment does not change infection risk");
            Assert.IsTrue(result.Contains("没有药品"), "failed wound treatment reports missing medicine");
        }

        [Test]
        public void TestFacilityActionsDriveRecoveryAndEvacuation()
        {
            // Bed
            _state.Phase = "evening";
            _state.Lin.Fatigue = 5;
            _state.Lin.Stress = 5;
            GameSimulation.PerformShelterAction(_state, "rest_bed");
            Assert.Less(_state.Lin.Fatigue, 5, "bed reduces fatigue");
            Assert.Less(_state.Lin.Stress, 5, "bed reduces stress");
            Assert.IsTrue(_state.Shelter.Facilities["bed"].UsedToday, "bed records daily use");

            // Workbench
            _state.Phase = "evening";
            int startingParts = _state.Resources.Parts;
            GameSimulation.PerformShelterAction(_state, "workbench_repair");
            Assert.Less(_state.Resources.Parts, startingParts, "workbench repair spends parts");
            Assert.GreaterOrEqual(_state.Bike.Range, 2, "workbench repair improves bike range");

            // Barricade
            _state.Phase = "evening";
            _state.Resources.Materials = 4;
            int startingDefense = _state.Shelter.Defense;
            GameSimulation.PerformShelterAction(_state, "barricade_windows");
            Assert.Greater(_state.Shelter.Defense, startingDefense, "barricade improves defense");
            Assert.GreaterOrEqual(_state.Shelter.Facilities["barricade"].Level, 2, "barricade facility level improves");

            // Radio
            _state.Phase = "evening";
            _state.Day = 9;
            _state.Resources.Fuel = 2;
            GameSimulation.PerformShelterAction(_state, "radio_broadcast");
            Assert.IsTrue(_state.Evacuation.SafezoneConfirmed, "radio confirms safe zone");
            Assert.IsTrue(_state.Evacuation.AddressKnown, "radio can reveal screening gate address");
        }

        [Test]
        public void TestStoragePreservesSuppliesForEscape()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 3;
            GameSimulation.PerformShelterAction(_state, "organize_storage");
            Assert.IsTrue(_state.Shelter.Facilities["storage"].UsedToday, "storage table records daily use");
            Assert.AreEqual(1, _state.Shelter.SupplyPreservation, "storage improves supply preservation");

            // Fast-forward to day 15 and resolve
            _state.Day = 15;
            _state.Resources.Food = 1;
            _state.Resources.Water = 1;
            _state.Evacuation.SafezoneConfirmed = true;
            _state.Evacuation.AddressKnown = true;
            _state.Evacuation.BikeReady = true;
            GameSimulation.SleepAndResolveNight(_state);
            Assert.IsTrue(_state.Reveal.Summary.Contains("带着整理好的物资"),
                "ending mentions organized supplies");
        }

        #endregion

        #region Qimian & Reveal Tests

        [Test]
        public void TestQimianWakesOnDayFive()
        {
            QimianController.ResolveForDay(_state, 4);
            Assert.IsFalse(_state.Qimian.Awake, "Qimian is still asleep on day 4");
            Assert.AreEqual(0, _state.Qimian.Log.Count, "Qimian has no action log before waking");

            QimianController.ResolveForDay(_state, 5);
            Assert.IsTrue(_state.Qimian.Awake, "Qimian wakes on day 5");
            Assert.AreEqual("寻找祁烬", _state.Qimian.PersonalityCard.MainGoal, "demo uses fixed Qimian goal card");
            Assert.GreaterOrEqual(_state.Qimian.Log.Count, 1, "Qimian logs an action on day 5");
            if (_state.Qimian.Log.Count > 0)
            {
                Assert.IsNotNull(_state.Qimian.Log[0].AiReplay, "Qimian log includes AI action replay");
                Assert.IsNotNull(_state.Qimian.Log[0].SubjectiveFragment, "Qimian log includes subjective fragment");
            }
        }

        [Test]
        public void TestDemoRevealContainsHiddenCausality()
        {
            for (int day = 1; day <= 15; day++)
                GameSimulation.PlaySafeDemoDay(_state, day);

            Assert.IsTrue(_state.DemoComplete, "demo completes after day 15 resolves");
            Assert.IsTrue(_state.Reveal.Unlocked, "Qimian log reveal unlocks at demo end");
            Assert.GreaterOrEqual(_state.Qimian.Log.Count, 5, "reveal has at least five hidden-causality log entries");
            Assert.IsTrue(_state.BloodMoonsResolved.Contains(7), "first blood moon is resolved");
            Assert.IsTrue(_state.BloodMoonsResolved.Contains(15), "second blood moon is resolved");
            Assert.IsTrue(new[] { "reached_gate_quarantine", "barely_reached_gate", "collapsed" }
                .Contains(_state.EndingState), "demo assigns a valid ending state");
            Assert.IsTrue(_state.Reveal.Summary.Contains("保护区"), "reveal mentions the safe zone gate");
            Assert.IsTrue(_state.Reveal.Summary.Contains("隔离"), "reveal mentions quarantine or isolation");
            Assert.IsTrue(_state.Reveal.Summary.Contains("尸群"), "reveal mentions the zombie group near miss");
        }

        [Test]
        public void TestDayFifteenAssignsEndingState()
        {
            for (int day = 1; day <= 15; day++)
                GameSimulation.PlaySafeDemoDay(_state, day);

            Assert.AreEqual("reveal", _state.Phase, "day 15 ends in reveal phase");
            Assert.IsTrue(new[] { "reached_gate_quarantine", "barely_reached_gate" }
                .Contains(_state.EndingState),
                "safe demo route reaches or barely reaches the screening gate");
            Assert.IsTrue(_state.Reveal.Summary.Contains("初筛") || _state.Reveal.Summary.Contains("隔离观察"),
                "ending summary reflects screening or quarantine");
        }

        [Test]
        public void TestDamagedLowResourceRunCanCollapse()
        {
            GameSimulation.StartDay(_state, 15);
            _state.Resources.Food = 0;
            _state.Resources.Water = 0;
            _state.Lin.Health = 1;
            _state.Lin.Hunger = 3;
            _state.Lin.Thirst = 3;
            _state.Lin.Stress = 9;
            _state.Shelter.Door = 0;
            _state.Shelter.Defense = 0;
            _state.Shelter.Noise = 4;
            _state.Shelter.Scent = 4;
            _state.Shelter.Light = 4;

            GameSimulation.SleepAndResolveNight(_state);

            Assert.IsTrue(_state.DemoComplete, "collapsed run still completes the demo frame");
            Assert.AreEqual("collapsed", _state.EndingState, "damaged low-resource state collapses");
        }

        #endregion
    }
}
