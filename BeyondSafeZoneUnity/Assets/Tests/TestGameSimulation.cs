using NUnit.Framework;

using System;
using System.Linq;
using System.Reflection;
using BeyondSafeZone.Controllers;
using BeyondSafeZone.Core;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Tests
{
    /// <summary>
    /// Core rules and deterministic simulation tests only.
    /// UI and world/runtime rigging tests live in TestOneRunUI and TestOneRunWorld so parallel lanes avoid this file.
    /// </summary>
    [TestFixture]
    public class TestGameSimulation
    {
        private GameState _state;

        [SetUp]
        public void SetUp()
        {
            _state = GameSimulation.NewGame();
        }

        #region ======== CORE: State & Init (lane: Code) ========

        [Test]
        public void TestInitialGoalAndStats()
        {
            Assert.AreEqual(1, _state.Day);
            Assert.AreEqual("撤离到保护区", _state.Goal);
            Assert.IsNotNull(_state.Lin);
            Assert.Greater(_state.Resources.Food, 0);
            Assert.Greater(_state.Shelter.Door, 0);
            Assert.IsFalse(_state.Qimian.Awake);
            Assert.AreEqual("in_progress", _state.EndingState);
            Assert.Greater(_state.MorningContext.Text.Length, 0);
            Assert.IsTrue(_state.LastEvent.Contains("林行"));
            Assert.IsTrue(_state.LastEvent.Contains("家"));
        }

        [Test]
        public void TestLinConditionText()
        {
            string initialText = GameSimulation.GetLinConditionText(_state);
            Assert.IsTrue(initialText.Contains("感染风险：低"));

            _state.Lin.InfectionRisk = 3;
            Assert.IsTrue(GameSimulation.GetLinConditionText(_state).Contains("发热风险"));

            _state.Lin.InfectionRisk = 5;
            Assert.IsTrue(GameSimulation.GetLinConditionText(_state).Contains("危险感染"));
        }

        [Test]
        public void TestResourcesAndEvacuation()
        {
            Assert.Greater(_state.Resources.Fuel, 0);
            Assert.AreEqual(0, _state.CarParts.Battery);
            Assert.IsFalse(_state.Evacuation.SafezoneConfirmed);
            Assert.IsFalse(_state.Evacuation.AddressKnown);
            Assert.IsFalse(_state.Evacuation.BikeReady);
        }

        [Test]
        public void TestShelterFacilities()
        {
            var f = _state.Shelter.Facilities;
            Assert.IsTrue(f.ContainsKey("bed"));
            Assert.IsTrue(f.ContainsKey("workbench"));
            Assert.IsTrue(f.ContainsKey("stove"));
            Assert.IsTrue(f.ContainsKey("barricade"));
            Assert.IsTrue(f.ContainsKey("radio"));
            Assert.IsTrue(f.ContainsKey("storage"));

            Assert.AreEqual("recover", f["bed"].Role);
            Assert.AreEqual("craft_repair", f["workbench"].Role);
            Assert.AreEqual("warmth", f["stove"].Role);
            Assert.AreEqual("blood_moon_defense", f["barricade"].Role);
            Assert.AreEqual("broadcast_clues", f["radio"].Role);
            Assert.AreEqual("preserve_carry", f["storage"].Role);

            Assert.IsFalse(f["bed"].Built);
            Assert.IsFalse(f["workbench"].Built);
            Assert.IsFalse(f["stove"].Built);
            Assert.IsTrue(f["barricade"].Built);
            Assert.IsTrue(f["radio"].Built);
            Assert.IsTrue(f["storage"].Built);
        }

        #endregion

        #region ======== CORE: Day Event & Blood Moon (lane: Code) ========

        [Test]
        public void TestDayEventTable()
        {
            for (int day = 1; day <= 15; day++)
            {
                var evt = GameSimulation.GetDayEvent(day);
                Assert.AreEqual(day, evt.Day);
                Assert.Greater(evt.MorningText.Length, 0);
                Assert.Greater(evt.PressureType.Length, 0);
                Assert.Greater(evt.Clue.Length, 0);
            }
        }

        [Test]
        public void TestBloodMoonDays()
        {
            Assert.IsTrue(GameSimulation.IsBloodMoonDay(7));
            Assert.IsTrue(GameSimulation.IsBloodMoonDay(15));
            Assert.IsFalse(GameSimulation.IsBloodMoonDay(14));
            Assert.IsFalse(GameSimulation.IsBloodMoonDay(10));
        }

        [Test]
        public void TestBloodMoonWarnings()
        {
            GameSimulation.StartDay(_state, 7);
            Assert.Greater(_state.MorningContext.BloodMoonWarning.Length, 0);

            GameSimulation.StartDay(_state, 15);
            Assert.Greater(_state.MorningContext.BloodMoonWarning.Length, 0);
        }

        [Test]
        public void TestMorningPressureAppliesOnce()
        {
            GameSimulation.NewGame();
            GameSimulation.StartDay(_state, 5);
            int firstStress = _state.Lin.Stress;
            int firstWater = _state.Resources.Water;

            GameSimulation.StartDay(_state, 5);
            Assert.AreEqual(firstStress, _state.Lin.Stress);
            Assert.AreEqual(firstWater, _state.Resources.Water);
        }

        #endregion

        #region ======== CORE: Exploration (lane: Code) ========

        [Test]
        public void TestExplorationMarksLocationAndReportsRisk()
        {
            string labelBefore = GameSimulation.GetLocationLabel(_state, "convenience");
            Assert.IsTrue(labelBefore.Contains("未搜"));

            string result = GameSimulation.Explore(_state, "convenience");
            string labelAfter = GameSimulation.GetLocationLabel(_state, "convenience");

            Assert.IsTrue(_state.Locations["convenience"].Visited);
            Assert.AreEqual("evening", _state.Phase);
            Assert.IsTrue(result.Contains("风险"));
            Assert.IsTrue(labelAfter.Contains("已搜"));
        }

        [Test]
        public void TestLocationsExposeNodeMapMetadata()
        {
            foreach (string locationId in GameSimulation.GetLocationIds(_state))
            {
                var location = _state.Locations[locationId];
                Assert.IsNotNull(location.ResourceTendency);
                Assert.Greater(location.ResourceTendency.Length, 0);
                Assert.IsNotNull(location.DangerLevel);
                Assert.Greater(location.DangerLevel.Length, 0);
                Assert.Greater(location.RouteTime, 0);
                Assert.IsNotNull(location.RoadCondition);
                Assert.Greater(location.RoadCondition.Length, 0);
                Assert.Greater(location.Icons.Count, 0);
            }
        }

        [Test]
        public void TestLocationCardText()
        {
            string cardText = GameSimulation.GetLocationCardText(_state, "police");
            Assert.IsTrue(cardText.Contains("资源倾向"));
            Assert.IsTrue(cardText.Contains("危险等级"));
            Assert.IsTrue(cardText.Contains("路况"));
            Assert.IsTrue(cardText.Contains("小时"));
            Assert.IsTrue(cardText.Contains("地点特征"));
            Assert.IsTrue(cardText.Contains("燃料"));
            Assert.IsTrue(cardText.Contains("路障"));
        }

        [Test]
        public void TestExplorationRevealsEvacuationAddress()
        {
            _state.Bike.Range = 2;
            Assert.IsFalse(_state.Evacuation.AddressKnown);

            string result = GameSimulation.Explore(_state, "police");
            Assert.IsTrue(_state.Evacuation.AddressKnown);
            Assert.IsTrue(result.Contains("地址"));
        }

        [Test]
        public void TestBadRoadConditionsIncreaseFatigue()
        {
            _state.Bike.Range = 2;
            int startingFatigue = _state.Lin.Fatigue;

            string result = GameSimulation.Explore(_state, "bike_shop");
            Assert.GreaterOrEqual(_state.Lin.Fatigue, startingFatigue + 2);
            Assert.IsTrue(result.Contains("路况"));
        }

        [Test]
        public void TestQimianActionsMarkAffectedNodes()
        {
            GameSimulation.NewGame();
            _state.Day = 6;
            QimianController.ResolveForDay(_state, 6);

            Assert.IsTrue(_state.Locations["clinic"].QimianTrace);
            Assert.IsTrue(_state.Locations["clinic"].Icons.Contains("qimian"));
            Assert.IsTrue(GameSimulation.GetLocationCardText(_state, "clinic").Contains("祁眠异常"));
        }

        [Test]
        public void TestLocationsExposeIndoorSearchRooms()
        {
            foreach (string locationId in new[] { "convenience", "clinic", "supermarket" })
            {
                var location = _state.Locations[locationId];
                Assert.GreaterOrEqual(location.Rooms.Count, 2);
                foreach (var kv in location.Rooms)
                {
                    Assert.Greater(kv.Value.Name.Length, 0);
                    Assert.Greater(kv.Value.Visibility.Length, 0);
                    Assert.Greater(kv.Value.SearchTime, 0);
                    Assert.IsNotNull(kv.Value.Resources);
                }
            }
        }

        [Test]
        public void TestEnterLocationStartsIndoorSearch()
        {
            string result = GameSimulation.EnterLocation(_state, "convenience");
            Assert.AreEqual("searching", _state.Phase);
            Assert.AreEqual("convenience", _state.Exploration.ActiveLocation);
            Assert.Greater(_state.Exploration.TimeLimit, 0);
            Assert.IsTrue(result.Contains("进入"));
            Assert.IsTrue(GameSimulation.GetRoomCardText(_state, "storefront").Contains("能见度"));
        }

        [Test]
        public void TestRoomCardReportsDarkRiskAndLureState()
        {
            GameSimulation.EnterLocation(_state, "clinic");

            string beforeLure = GameSimulation.GetRoomCardText(_state, "pharmacy");
            Assert.IsTrue(beforeLure.Contains("黑暗"));
            Assert.IsTrue(beforeLure.Contains("未排除"));

            GameSimulation.LureRoom(_state, "pharmacy");
            string afterLure = GameSimulation.GetRoomCardText(_state, "pharmacy");
            Assert.IsTrue(afterLure.Contains("已引开"));
        }

        [Test]
        public void TestSearchRoomCollectsResources()
        {
            GameSimulation.EnterLocation(_state, "convenience");
            int startingFood = _state.Resources.Food;

            string result = GameSimulation.SearchRoom(_state, "storefront", "careful");
            Assert.Greater(_state.Resources.Food, startingFood);
            Assert.IsTrue(_state.Locations["convenience"].Rooms["storefront"].Searched);
            Assert.IsTrue(result.Contains("带回") || result.Contains("搜到"));

            GameSimulation.LeaveExploration(_state);
            Assert.AreEqual("evening", _state.Phase);
            Assert.IsTrue(_state.Locations["convenience"].Visited);
            Assert.AreEqual("", _state.Exploration.ActiveLocation);
        }

        [Test]
        public void TestDarkHiddenZombieRoomCausesInjury()
        {
            GameSimulation.EnterLocation(_state, "clinic");
            int startingHealth = _state.Lin.Health;
            int startingInfection = _state.Lin.InfectionRisk;

            string result = GameSimulation.SearchRoom(_state, "pharmacy", "quick");
            Assert.Less(_state.Lin.Health, startingHealth);
            Assert.Greater(_state.Lin.InfectionRisk, startingInfection);
            Assert.IsTrue(result.Contains("隐藏尸群"));
        }

        [Test]
        public void TestNoiseLureReducesInjury()
        {
            GameSimulation.EnterLocation(_state, "clinic");
            int startingHealth = _state.Lin.Health;

            string lureResult = GameSimulation.LureRoom(_state, "pharmacy");
            string searchResult = GameSimulation.SearchRoom(_state, "pharmacy", "careful");

            Assert.AreEqual(startingHealth, _state.Lin.Health);
            Assert.GreaterOrEqual(_state.Exploration.TimeUsed, 2);
            Assert.Greater(_state.Exploration.Noise, 0);
            Assert.IsTrue(lureResult.Contains("制造噪音"));
            Assert.IsTrue(searchResult.Contains("已被引开"));
        }

        [Test]
        public void TestExplorationSiteCatalogExposesCoreScavengeSites()
        {
            Type catalogType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("BeyondSafeZone.World.ExplorationSiteCatalog"))
                .FirstOrDefault(type => type != null);
            Assert.IsNotNull(catalogType);

            var getCoreSites = catalogType.GetMethod("GetCoreSites");
            var getRoomsForLocation = catalogType.GetMethod("GetRoomsForLocation");
            Assert.IsNotNull(getCoreSites);
            Assert.IsNotNull(getRoomsForLocation);

            var sites = ((System.Collections.IEnumerable)getCoreSites.Invoke(null, null)).Cast<object>().ToList();
            var locationIds = sites
                .Select(item => item.GetType().GetProperty("LocationId").GetValue(item) as string)
                .ToList();
            CollectionAssert.AreEquivalent(new[] { "clinic", "supermarket", "bike_shop" }, locationIds);

            var clinicRooms = ((System.Collections.IEnumerable)getRoomsForLocation.Invoke(null, new object[] { "clinic" }))
                .Cast<object>()
                .Select(item => item.GetType().GetProperty("RoomId").GetValue(item) as string)
                .ToList();
            CollectionAssert.IsSubsetOf(new[] { "waiting", "exam_a", "pharmacy" }, clinicRooms);
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

            Assert.Greater(_state.Lin.Fatigue, startingFatigue);
            Assert.IsTrue(result.Contains("天色"));
        }

        #endregion

        #region ======== CORE: Night & Shelter (lane: Code) ========

        [Test]
        public void TestHighInfectionRiskWorsensAtNight()
        {
            _state.Phase = "evening";
            _state.Lin.InfectionRisk = 5;
            int startingHealth = _state.Lin.Health;
            int startingStress = _state.Lin.Stress;

            string result = GameSimulation.SleepAndResolveNight(_state);

            Assert.Less(_state.Lin.Health, startingHealth);
            Assert.Greater(_state.Lin.Stress, startingStress);
            Assert.IsTrue(result.Contains("感染"));
        }

        [Test]
        public void TestTreatWoundSpendsMedicineAndReducesInfection()
        {
            _state.Phase = "evening";
            _state.Lin.Health = 7;
            _state.Lin.InfectionRisk = 4;
            _state.Resources.Meds = 2;

            string result = GameSimulation.PerformShelterAction(_state, "treat_wound");
            Assert.AreEqual(1, _state.Resources.Meds);
            Assert.AreEqual(8, _state.Lin.Health);
            Assert.AreEqual(3, _state.Lin.InfectionRisk);
            Assert.IsTrue(result.Contains("处理伤口"));
        }

        [Test]
        public void TestTreatWoundWithoutMedicine()
        {
            _state.Phase = "evening";
            _state.Lin.Health = 6;
            _state.Lin.InfectionRisk = 3;
            _state.Resources.Meds = 0;

            string result = GameSimulation.PerformShelterAction(_state, "treat_wound");
            Assert.AreEqual(6, _state.Lin.Health);
            Assert.AreEqual(3, _state.Lin.InfectionRisk);
            Assert.IsTrue(result.Contains("没有药品"));
        }

        [Test]
        public void TestBuildableShelterFacilitiesGateActions()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 8;
            _state.Resources.Parts = 3;
            _state.Resources.Fuel = 3;

            string repairBeforeWorkbench = GameSimulation.PerformShelterAction(_state, "workbench_repair");
            Assert.IsTrue(repairBeforeWorkbench.Contains("工作台"));
            Assert.AreEqual(1, _state.Bike.Range);

            _state.Phase = "evening";
            string buildWorkbench = GameSimulation.PerformShelterAction(_state, "build_workbench");
            Assert.IsTrue(_state.Shelter.Facilities["workbench"].Built);
            Assert.IsTrue(buildWorkbench.Contains("工作台"));

            _state.Phase = "evening";
            int partsAfterBuild = _state.Resources.Parts;
            string repairAfterWorkbench = GameSimulation.PerformShelterAction(_state, "workbench_repair");
            Assert.Less(_state.Resources.Parts, partsAfterBuild);
            Assert.GreaterOrEqual(_state.Bike.Range, 2);
            Assert.IsTrue(repairAfterWorkbench.Contains("自行车"));
        }

        [Test]
        public void TestBedAndStoveConstructionChangeShelterOptions()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 8;
            _state.Resources.Parts = 3;
            _state.Resources.Fuel = 3;
            _state.Lin.Fatigue = 6;
            _state.Lin.Stress = 6;

            string restBeforeBed = GameSimulation.PerformShelterAction(_state, "rest_bed");
            Assert.IsTrue(restBeforeBed.Contains("没有床"));
            Assert.AreEqual(6, _state.Lin.Fatigue);

            _state.Phase = "evening";
            GameSimulation.PerformShelterAction(_state, "build_bed");
            Assert.IsTrue(_state.Shelter.Facilities["bed"].Built);

            _state.Phase = "evening";
            GameSimulation.PerformShelterAction(_state, "rest_bed");
            Assert.Less(_state.Lin.Fatigue, 6);
            Assert.Less(_state.Lin.Stress, 6);

            _state.Phase = "evening";
            GameSimulation.PerformShelterAction(_state, "build_stove");
            Assert.IsTrue(_state.Shelter.Facilities["stove"].Built);
            Assert.IsTrue(_state.Shelter.Facilities["stove"].UsedToday);
        }

        [Test]
        public void TestFacilityActionsDriveRecoveryAndEvacuation()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 8;
            _state.Resources.Parts = 3;
            _state.Resources.Fuel = 3;
            GameSimulation.PerformShelterAction(_state, "build_bed");
            GameSimulation.PerformShelterAction(_state, "build_workbench");

            // Bed
            _state.Phase = "evening";
            _state.Lin.Fatigue = 5;
            _state.Lin.Stress = 5;
            GameSimulation.PerformShelterAction(_state, "rest_bed");
            Assert.Less(_state.Lin.Fatigue, 5);
            Assert.Less(_state.Lin.Stress, 5);
            Assert.IsTrue(_state.Shelter.Facilities["bed"].UsedToday);

            // Workbench
            _state.Phase = "evening";
            int startingParts = _state.Resources.Parts;
            GameSimulation.PerformShelterAction(_state, "workbench_repair");
            Assert.Less(_state.Resources.Parts, startingParts);
            Assert.GreaterOrEqual(_state.Bike.Range, 2);

            // Barricade
            _state.Phase = "evening";
            _state.Resources.Materials = 4;
            int startingDefense = _state.Shelter.Defense;
            GameSimulation.PerformShelterAction(_state, "barricade_windows");
            Assert.Greater(_state.Shelter.Defense, startingDefense);
            Assert.GreaterOrEqual(_state.Shelter.Facilities["barricade"].Level, 2);

            // Radio
            _state.Phase = "evening";
            _state.Day = 9;
            _state.Resources.Fuel = 2;
            GameSimulation.PerformShelterAction(_state, "radio_broadcast");
            Assert.IsTrue(_state.Evacuation.SafezoneConfirmed);
            Assert.IsTrue(_state.Evacuation.AddressKnown);
        }

        [Test]
        public void TestShelterInteractionCatalogMapsFacilitiesToActions()
        {
            Type catalogType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("BeyondSafeZone.World.ShelterInteractionCatalog"))
                .FirstOrDefault(type => type != null);
            Assert.IsNotNull(catalogType);

            var getAll = catalogType.GetMethod("GetAll");
            var getAction = catalogType.GetMethod("GetActionForFacility");
            Assert.IsNotNull(getAll);
            Assert.IsNotNull(getAction);

            var interactions = ((System.Collections.IEnumerable)getAll.Invoke(null, null)).Cast<object>().ToList();
            Assert.AreEqual(6, interactions.Count);

            var facilityIds = interactions
                .Select(item => item.GetType().GetProperty("FacilityId").GetValue(item) as string)
                .ToList();
            CollectionAssert.AreEquivalent(new[] { "bed", "workbench", "stove", "barricade", "radio", "storage" }, facilityIds);

            Assert.AreEqual("build_workbench", getAction.Invoke(null, new object[] { _state, "workbench" }));
            _state.Shelter.Facilities["workbench"].Built = true;
            Assert.AreEqual("workbench_repair", getAction.Invoke(null, new object[] { _state, "workbench" }));

            Assert.AreEqual("build_bed", getAction.Invoke(null, new object[] { _state, "bed" }));
            _state.Shelter.Facilities["bed"].Built = true;
            Assert.AreEqual("rest_bed", getAction.Invoke(null, new object[] { _state, "bed" }));

            Assert.AreEqual("radio_broadcast", getAction.Invoke(null, new object[] { _state, "radio" }));
            Assert.AreEqual("barricade_windows", getAction.Invoke(null, new object[] { _state, "barricade" }));
            Assert.AreEqual("organize_storage", getAction.Invoke(null, new object[] { _state, "storage" }));
        }

        [Test]
        public void TestStoragePreservesSuppliesForEscape()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 3;
            GameSimulation.PerformShelterAction(_state, "organize_storage");
            Assert.IsTrue(_state.Shelter.Facilities["storage"].UsedToday);
            Assert.AreEqual(1, _state.Shelter.SupplyPreservation);

            _state.Day = 15;
            _state.Resources.Food = 1;
            _state.Resources.Water = 1;
            _state.Evacuation.SafezoneConfirmed = true;
            _state.Evacuation.AddressKnown = true;
            _state.Evacuation.BikeReady = true;
            GameSimulation.SleepAndResolveNight(_state);
            Assert.IsTrue(_state.Reveal.Summary.Contains("带着整理好的物资"));
        }

        #endregion

        #region ======== CORE: Shelter Action Availability (lane: Code, A-001) ========

        [Test]
        public void TestAllShelterActionsAvailableInEveningPhase()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 4;
            _state.Resources.Parts = 2;
            _state.Resources.Fuel = 2;
            _state.Resources.Meds = 2;

            string[] alwaysAvailable = { "quiet", "organize_storage" };
            foreach (var actionId in alwaysAvailable)
            {
                var a = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.IsTrue(a.Available, $"{actionId} 应该在初始 evening 状态可用，但返回: {a.FailureReason}");
                Assert.AreEqual("", a.FailureReason);
            }

            string[] reqResources = { "barricade_windows", "radio_broadcast", "treat_wound", "mask_scent" };
            foreach (var actionId in reqResources)
            {
                var a = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.IsTrue(a.Available, $"{actionId} 应该在资源充足的 evening 状态可用，但返回: {a.FailureReason}");
            }

            // build actions: facilities not yet built, resources enough → available
            string[] buildActions = { "build_bed", "build_workbench", "build_stove" };
            foreach (var actionId in buildActions)
            {
                var a = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.IsTrue(a.Available, $"{actionId} 应该在设施未建造且资源充足的 evening 状态可用，但返回: {a.FailureReason}");
            }
        }

        [Test]
        public void TestShelterActionAvailabilityPhaseGate()
        {
            // morning/day 允许（UI 层 EnsureShelterActionPhase 自动转 evening）
            string[] allowedPhases = { "morning", "day", "evening" };
            foreach (var phase in allowedPhases)
            {
                _state.Phase = phase;
                var a = GameSimulation.CheckShelterActionAvailability(_state, "quiet");
                Assert.IsTrue(a.Available, $"phase={phase} 时 quiet 应该可用");
            }

            // searching/night/reveal 阻止
            string[] blockedPhases = { "searching", "night", "reveal" };
            foreach (var phase in blockedPhases)
            {
                _state.Phase = phase;
                var a = GameSimulation.CheckShelterActionAvailability(_state, "quiet");
                Assert.IsFalse(a.Available, $"phase={phase} 时 quiet 应该不可用");
                Assert.IsTrue(a.FailureReason.Contains("时机"));
            }
        }

        [Test]
        public void TestBuildActionUnavailableWhenAlreadyBuilt()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 6;
            _state.Resources.Parts = 3;

            // 建造后再次查询应不可用
            var bed = new (string actionId, string facilityId)[]
            {
                ("build_bed", "bed"),
                ("build_workbench", "workbench"),
                ("build_stove", "stove"),
            };

            foreach (var (actionId, facilityId) in bed)
            {
                var before = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.IsTrue(before.Available, $"建造前 {actionId} 应该可用");

                GameSimulation.PerformShelterAction(_state, actionId);
                Assert.IsTrue(_state.Shelter.Facilities[facilityId].Built);

                _state.Phase = "evening";
                var after = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.IsFalse(after.Available, $"建造后 {actionId} 应该不可用");
                Assert.IsTrue(after.FailureReason.Contains("已经能用了"));
            }
        }

        [Test]
        public void TestUseActionUnavailableWithoutRequiredFacility()
        {
            _state.Phase = "evening";
            _state.Resources.Parts = 2;
            _state.Resources.Materials = 4;

            var reqFacility = new (string actionId, string facilityId)[]
            {
                ("rest_bed", "bed"),
                ("workbench_repair", "workbench"),
                ("workbench_car", "workbench"),
                ("fortify", "workbench"),
            };

            foreach (var (actionId, facilityId) in reqFacility)
            {
                Assert.IsFalse(_state.Shelter.Facilities[facilityId].Built,
                    $"{facilityId} 初始不应已建造");

                var a = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.IsFalse(a.Available, $"{actionId} 在 {facilityId} 未建造时应不可用");
                Assert.IsTrue(a.FailureReason.Contains("需要先建造") || a.FailureReason.Contains("工作台"),
                    $"失败原因应提到缺少 {facilityId}，实际: {a.FailureReason}");
            }
        }

        [Test]
        public void TestWorkbenchCarUnavailableWithoutFoundCar()
        {
            _state.Phase = "evening";
            _state.Shelter.Facilities["workbench"].Built = true;
            Assert.IsFalse(_state.Car.Found);

            var a = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(a.Available);
            Assert.IsTrue(a.FailureReason.Contains("还没找到"));
        }

        [Test]
        public void TestWorkbenchCarUnavailableWhenCarAlreadyReady()
        {
            _state.Phase = "evening";
            _state.Shelter.Facilities["workbench"].Built = true;
            _state.Car.Found = true;
            _state.Car.Ready = true;

            var a = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(a.Available);
            Assert.IsTrue(a.FailureReason.Contains("已经修好"));
        }

        [Test]
        public void TestWorkbenchCarBlocksInsufficientEngineMaterials()
        {
            _state.Phase = "evening";
            _state.Shelter.Facilities["workbench"].Built = true;
            _state.Car.Found = true;
            // StepEngine 未完成，资源不足
            _state.Resources.Materials = 0;
            _state.Resources.Parts = 5;

            var a = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(a.Available);
            Assert.IsTrue(a.FailureReason.Contains("引擎线路"));

            // 资源足够后应可用
            _state.Resources.Materials = 5;
            _state.Resources.Parts = 5;
            var b = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsTrue(b.Available);
        }

        [Test]
        public void TestWorkbenchCarBlocksInsufficientTireBatteryOrGasoline()
        {
            _state.Phase = "evening";
            _state.Shelter.Facilities["workbench"].Built = true;
            _state.Car.Found = true;
            _state.Car.StepEngine = true; // 引擎已完成，进入轮胎步骤
            _state.Resources.Materials = 5;
            _state.Resources.Parts = 5;
            _state.Resources.Fuel = 5;

            // 缺轮胎
            _state.CarParts.Tire = 0;
            var tireCheck = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(tireCheck.Available);
            Assert.IsTrue(tireCheck.FailureReason.Contains("轮胎"));

            // 给轮胎，进入电瓶步骤
            _state.CarParts.Tire = 1;
            _state.Car.StepTire = true;
            _state.CarParts.Battery = 0;
            var batteryCheck = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(batteryCheck.Available);
            Assert.IsTrue(batteryCheck.FailureReason.Contains("电瓶"));

            // 给电瓶和燃料，进入汽油步骤
            _state.CarParts.Battery = 1;
            _state.Resources.Fuel = 5;
            _state.Car.StepBattery = true;
            _state.CarParts.Gasoline = 0;
            var gasolineCheck = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(gasolineCheck.Available);
            Assert.IsTrue(gasolineCheck.FailureReason.Contains("汽油"));

            // 所有步骤完成后应可用（Car.Ready）
            _state.CarParts.Gasoline = 2;
            _state.Car.StepFueled = true;
            _state.Car.Ready = true;
            var readyCheck = GameSimulation.CheckShelterActionAvailability(_state, "workbench_car");
            Assert.IsFalse(readyCheck.Available); // 修好后不可再修
            Assert.IsTrue(readyCheck.FailureReason.Contains("已经修好"));
        }

        [Test]
        public void TestResourceCostBlocksShelterAction()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 0;
            _state.Resources.Parts = 2;
            _state.Resources.Fuel = 2;
            _state.Resources.Meds = 2;

            var matBlocked = GameSimulation.CheckShelterActionAvailability(_state, "barricade_windows");
            Assert.IsFalse(matBlocked.Available);
            Assert.IsTrue(matBlocked.FailureReason.Contains("建材不足"));

            var maskBlocked = GameSimulation.CheckShelterActionAvailability(_state, "mask_scent");
            Assert.IsFalse(maskBlocked.Available);
            Assert.IsTrue(maskBlocked.FailureReason.Contains("不足"));

            _state.Resources.Materials = 4;
            _state.Resources.Fuel = 0;
            var fuelBlocked = GameSimulation.CheckShelterActionAvailability(_state, "radio_broadcast");
            Assert.IsFalse(fuelBlocked.Available);
            Assert.IsTrue(fuelBlocked.FailureReason.Contains("燃料不足"));

            _state.Resources.Fuel = 2;
            _state.Resources.Meds = 0;
            var medsBlocked = GameSimulation.CheckShelterActionAvailability(_state, "treat_wound");
            Assert.IsFalse(medsBlocked.Available);
            Assert.IsTrue(medsBlocked.FailureReason.Contains("药品不足"));

            // build_bed should also be blocked by materials
            _state.Resources.Materials = 0;
            _state.Resources.Parts = 2;
            var buildBlocked = GameSimulation.CheckShelterActionAvailability(_state, "build_bed");
            Assert.IsFalse(buildBlocked.Available);
            Assert.IsTrue(buildBlocked.FailureReason.Contains("材料不够"));
        }

        [Test]
        public void TestShelterActionAliasesUseSameAvailability()
        {
            _state.Phase = "evening";
            _state.Resources.Parts = 2;

            // repair_bike → workbench_repair: 工作台未建造
            var bike = GameSimulation.CheckShelterActionAvailability(_state, "repair_bike");
            var wr = GameSimulation.CheckShelterActionAvailability(_state, "workbench_repair");
            Assert.AreEqual(wr.Available, bike.Available);
            Assert.AreEqual(wr.FailureReason, bike.FailureReason);
            // 别名必须保留原始 ActionId
            Assert.AreEqual("repair_bike", bike.ActionId,
                "repair_bike 查询返回的 ActionId 应为 repair_bike，不应变成 workbench_repair");

            // radio → radio_broadcast: 燃料充足则可用
            _state.Resources.Fuel = 1;
            var r = GameSimulation.CheckShelterActionAvailability(_state, "radio");
            var rb = GameSimulation.CheckShelterActionAvailability(_state, "radio_broadcast");
            Assert.AreEqual(rb.Available, r.Available);
            Assert.AreEqual(rb.FailureReason, r.FailureReason);
            Assert.AreEqual("radio", r.ActionId,
                "radio 查询返回的 ActionId 应为 radio，不应变成 radio_broadcast");
        }

        [Test]
        public void TestAvailabilityQueryDoesNotChangeGameState()
        {
            _state.Phase = "evening";
            _state.Resources.Materials = 4;
            _state.Resources.Parts = 2;
            _state.Resources.Fuel = 2;
            _state.Resources.Meds = 2;

            int foodBefore = _state.Resources.Food;
            int partsBefore = _state.Resources.Parts;
            int materialsBefore = _state.Resources.Materials;
            int fuelBefore = _state.Resources.Fuel;
            int medsBefore = _state.Resources.Meds;
            int healthBefore = _state.Lin.Health;
            string phaseBefore = _state.Phase;

            // 查询所有可用性
            string[] allActions = {
                "build_bed", "build_workbench", "build_stove",
                "rest_bed", "workbench_repair", "barricade_windows",
                "radio_broadcast", "organize_storage", "treat_wound",
                "quiet", "mask_scent", "fortify", "workbench_car",
                "repair_bike", "radio"
            };
            foreach (var actionId in allActions)
                GameSimulation.CheckShelterActionAvailability(_state, actionId);

            Assert.AreEqual(foodBefore, _state.Resources.Food);
            Assert.AreEqual(partsBefore, _state.Resources.Parts);
            Assert.AreEqual(materialsBefore, _state.Resources.Materials);
            Assert.AreEqual(fuelBefore, _state.Resources.Fuel);
            Assert.AreEqual(medsBefore, _state.Resources.Meds);
            Assert.AreEqual(healthBefore, _state.Lin.Health);
            Assert.AreEqual(phaseBefore, _state.Phase);
            // 设施建造状态不变
            Assert.IsFalse(_state.Shelter.Facilities["bed"].Built);
            Assert.IsFalse(_state.Shelter.Facilities["workbench"].Built);
            Assert.IsFalse(_state.Shelter.Facilities["stove"].Built);
        }

        [Test]
        public void TestUnknownActionReturnsUnavailable()
        {
            _state.Phase = "evening";
            var a = GameSimulation.CheckShelterActionAvailability(_state, "nonexistent_action");
            Assert.IsFalse(a.Available);
            Assert.IsTrue(a.FailureReason.Contains("未知"));
        }

        [Test]
        public void TestCheckAvailabilityReturnsCorrectActionId()
        {
            _state.Phase = "evening";
            string[] expectedActions = {
                "build_bed", "rest_bed", "quiet", "organize_storage",
                "radio_broadcast", "barricade_windows"
            };
            foreach (var actionId in expectedActions)
            {
                var a = GameSimulation.CheckShelterActionAvailability(_state, actionId);
                Assert.AreEqual(actionId, a.ActionId,
                    $"CheckShelterActionAvailability(\"{actionId}\") 返回的 ActionId 应该是 \"{actionId}\"");
            }

            // 别名必须保留原始 ActionId
            // repair_bike → workbench_repair：工作台初始未建造，必然不可用
            // radio → radio_broadcast：资源门槛是 Fuel（BalanceData.SHELTER_RADIO_FUEL=1），不是 Parts
            _state.Resources.Fuel = 0;
            var aliasBike = GameSimulation.CheckShelterActionAvailability(_state, "repair_bike");
            Assert.AreEqual("repair_bike", aliasBike.ActionId);
            Assert.IsFalse(aliasBike.Available);
            var aliasRadio = GameSimulation.CheckShelterActionAvailability(_state, "radio");
            Assert.AreEqual("radio", aliasRadio.ActionId);
            Assert.IsFalse(aliasRadio.Available);
        }

        #endregion

        #region ======== CORE: Exploration Action Availability (lane: Code, A-002) ========

        [Test]
        public void TestExplorationActionAvailabilityBlocksOutsideSearching()
        {
            // 未进入任何地点时所有搜刮行动不可用
            string[] actions = { "search_room", "lure_room", "leave_exploration" };
            foreach (var actionId in actions)
            {
                var a = GameSimulation.CheckExplorationActionAvailability(_state, actionId, "storefront");
                Assert.IsFalse(a.Available, $"{actionId} 在未进入地点时应不可用");
                Assert.IsTrue(a.FailureReason.Contains("没有进入"),
                    $"失败原因应提到未进入地点，实际: {a.FailureReason}");
            }

            // 进入地点后在 searching 阶段应可用
            GameSimulation.EnterLocation(_state, "convenience");
            Assert.AreEqual("searching", _state.Phase);
            Assert.AreEqual("convenience", _state.Exploration.ActiveLocation);

            var leaveOk = GameSimulation.CheckExplorationActionAvailability(_state, "leave_exploration");
            Assert.IsTrue(leaveOk.Available, "进入 searching 后 leave_exploration 应可用");

            var searchOk = GameSimulation.CheckExplorationActionAvailability(_state, "search_room", "storefront");
            Assert.IsTrue(searchOk.Available, "进入 searching 后 search_room 应可用");

            var lureOk = GameSimulation.CheckExplorationActionAvailability(_state, "lure_room", "storefront");
            Assert.IsTrue(lureOk.Available, "进入 searching 后 lure_room 应可用");
        }

        [Test]
        public void TestExplorationActionAvailabilityBlocksUnknownOrInvalidRoom()
        {
            GameSimulation.EnterLocation(_state, "convenience");

            // 不存在的 roomId
            var unknown = GameSimulation.CheckExplorationActionAvailability(_state, "search_room", "nonexistent_room");
            Assert.IsFalse(unknown.Available);
            Assert.IsTrue(unknown.FailureReason.Contains("还没有做进灰盒"));

            var unknownLure = GameSimulation.CheckExplorationActionAvailability(_state, "lure_room", "nonexistent_room");
            Assert.IsFalse(unknownLure.Available);
            Assert.IsTrue(unknownLure.FailureReason.Contains("还没有做进灰盒"));
        }

        [Test]
        public void TestExplorationActionAvailabilityBlocksSearchedOrLockedRoom()
        {
            // --- 已搜房间：使用 convenience，搜索 storefront 后直接查 ---
            GameSimulation.EnterLocation(_state, "convenience");
            GameSimulation.SearchRoom(_state, "storefront", "careful");
            Assert.IsTrue(_state.Locations["convenience"].Rooms["storefront"].Searched);

            var searchedCheck = GameSimulation.CheckExplorationActionAvailability(_state, "search_room", "storefront");
            Assert.IsFalse(searchedCheck.Available);
            Assert.IsTrue(searchedCheck.FailureReason.Contains("已经搜过"));

            // --- 锁房间：新建独立状态，直接进入 clinic，设置 pharmacy 上锁 ---
            // 注意：不能在同一 searching 流程中二次 EnterLocation，EnterLocation 要求 morning/day 阶段。
            var state2 = GameSimulation.NewGame();
            GameSimulation.EnterLocation(state2, "clinic");
            Assert.AreEqual("searching", state2.Phase,
                "EnterLocation(clinic) 后 phase 应为 searching");
            Assert.AreEqual("clinic", state2.Exploration.ActiveLocation,
                "EnterLocation(clinic) 后 ActiveLocation 应为 clinic");

            state2.Locations["clinic"].Rooms["pharmacy"].Locked = true;
            var lockedCheck = GameSimulation.CheckExplorationActionAvailability(state2, "search_room", "pharmacy");
            Assert.IsFalse(lockedCheck.Available);
            Assert.IsTrue(lockedCheck.FailureReason.Contains("锁着"));
        }

        [Test]
        public void TestExplorationActionAvailabilityAllowsLureAndLeaveWhenSearching()
        {
            GameSimulation.EnterLocation(_state, "convenience");

            // lure_room 不检查 Searched/Locked（可以 continue luring）
            GameSimulation.SearchRoom(_state, "storefront", "careful");
            var lureAfterSearch = GameSimulation.CheckExplorationActionAvailability(_state, "lure_room", "storefront");
            Assert.IsTrue(lureAfterSearch.Available,
                $"lure_room 在房间已搜后仍应可用，但返回: {lureAfterSearch.FailureReason}");

            // leave_exploration 始终可用
            var leave = GameSimulation.CheckExplorationActionAvailability(_state, "leave_exploration");
            Assert.IsTrue(leave.Available);

            // tactic 别名：convenience 当前真实房间为 storefront / warehouse。
            // storefront 已搜过，所以用同地点中仍未搜索的 warehouse 验证别名。
            var quick = GameSimulation.CheckExplorationActionAvailability(_state, "quick_search", "warehouse");
            Assert.IsTrue(quick.Available);
            Assert.AreEqual("quick_search", quick.ActionId);

            var careful = GameSimulation.CheckExplorationActionAvailability(_state, "careful_search", "warehouse");
            Assert.IsTrue(careful.Available);
            Assert.AreEqual("careful_search", careful.ActionId);
        }

        [Test]
        public void TestExplorationActionAvailabilityDoesNotChangeGameState()
        {
            GameSimulation.EnterLocation(_state, "convenience");
            int timeBefore = _state.Exploration.TimeUsed;
            int noiseBefore = _state.Exploration.Noise;
            int foodBefore = _state.Resources.Food;
            string phaseBefore = _state.Phase;
            int searchedCountBefore = _state.Exploration.SearchedRooms.Count;
            int luredCountBefore = _state.Exploration.LuredRooms.Count;

            // 查询所有搜刮行动
            GameSimulation.CheckExplorationActionAvailability(_state, "search_room", "storefront");
            GameSimulation.CheckExplorationActionAvailability(_state, "quick_search", "checkout");
            GameSimulation.CheckExplorationActionAvailability(_state, "careful_search", "checkout");
            GameSimulation.CheckExplorationActionAvailability(_state, "lure_room", "storefront");
            GameSimulation.CheckExplorationActionAvailability(_state, "leave_exploration");

            Assert.AreEqual(timeBefore, _state.Exploration.TimeUsed);
            Assert.AreEqual(noiseBefore, _state.Exploration.Noise);
            Assert.AreEqual(foodBefore, _state.Resources.Food);
            Assert.AreEqual(phaseBefore, _state.Phase);
            Assert.AreEqual(searchedCountBefore, _state.Exploration.SearchedRooms.Count);
            Assert.AreEqual(luredCountBefore, _state.Exploration.LuredRooms.Count);
            // 房间 Searched 状态不变
            Assert.IsFalse(_state.Locations["convenience"].Rooms["storefront"].Searched);
        }

        #endregion

        #region ======== CORE: Day Phase Action Availability (lane: Code, A-003) ========

        [Test]
        public void TestDayPhaseActionAvailabilityMatchesCurrentResolveNightHandler()
        {
            // OneRunGameController.ResolveNight 当前只在 DemoComplete 时阻止；
            // searching 会先 ReturnToShelter，其他阶段直接进入 SleepAndResolveNight。
            string[] allowedPhases = { "morning", "day", "evening", "searching", "night", "reveal" };
            foreach (var phase in allowedPhases)
            {
                _state.Phase = phase;
                var a = GameSimulation.CheckDayPhaseActionAvailability(_state, "resolve_night");
                Assert.IsTrue(a.Available, $"phase={phase} 时 resolve_night 应可用");
                Assert.AreEqual("", a.FailureReason);
            }
        }

        [Test]
        public void TestDayPhaseActionAvailabilityMatchesCurrentNextDayHandler()
        {
            // OneRunGameController.NextDay 当前只在 DemoComplete 时阻止；
            // 其他阶段都会调用 StartDay(State, Mathf.Min(State.Day + 1, 15))。
            string[] allowedPhases = { "morning", "day", "evening", "searching", "night", "reveal" };
            foreach (var phase in allowedPhases)
            {
                _state.Phase = phase;
                var a = GameSimulation.CheckDayPhaseActionAvailability(_state, "next_day");
                Assert.IsTrue(a.Available, $"phase={phase} 时 next_day 应按当前 handler 行为可用");
                Assert.AreEqual("", a.FailureReason);
            }
        }

        [Test]
        public void TestDayPhaseActionAvailabilityBlocksAfterDemoComplete()
        {
            _state.DemoComplete = true;

            var rn = GameSimulation.CheckDayPhaseActionAvailability(_state, "resolve_night");
            Assert.IsFalse(rn.Available);
            Assert.IsTrue(rn.FailureReason.Contains("演示已完成"));

            var nd = GameSimulation.CheckDayPhaseActionAvailability(_state, "next_day");
            Assert.IsFalse(nd.Available);
            Assert.IsTrue(nd.FailureReason.Contains("演示已完成"));
        }

        [Test]
        public void TestDayPhaseActionAvailabilityUnknownActionReturnsUnavailable()
        {
            var a = GameSimulation.CheckDayPhaseActionAvailability(_state, "nonexistent");
            Assert.IsFalse(a.Available);
            Assert.IsTrue(a.FailureReason.Contains("未知"));
        }

        [Test]
        public void TestDayPhaseActionAvailabilityDoesNotChangeGameState()
        {
            _state.Phase = "evening";
            int dayBefore = _state.Day;
            bool demoBefore = _state.DemoComplete;
            int foodBefore = _state.Resources.Food;
            int healthBefore = _state.Lin.Health;
            string phaseBefore = _state.Phase;
            string endingBefore = _state.EndingState;
            string lastEventBefore = _state.LastEvent;
            // Exploration 字段
            string activeLocBefore = _state.Exploration.ActiveLocation;
            int timeUsedBefore = _state.Exploration.TimeUsed;
            int noiseBefore = _state.Exploration.Noise;
            int searchedCountBefore = _state.Exploration.SearchedRooms.Count;
            int luredCountBefore = _state.Exploration.LuredRooms.Count;
            // Qimian 字段
            bool awakeBefore = _state.Qimian.Awake;
            int logCountBefore = _state.Qimian.Log.Count;
            int publicCluesBefore = _state.Qimian.PublicClues.Count;
            // Resources 全字段
            int waterBefore = _state.Resources.Water;
            int medsBefore = _state.Resources.Meds;
            int materialsBefore = _state.Resources.Materials;
            int partsBefore = _state.Resources.Parts;
            int fuelBefore = _state.Resources.Fuel;
            // Lin 全字段
            int fatigueBefore = _state.Lin.Fatigue;
            int stressBefore = _state.Lin.Stress;
            int infectionBefore = _state.Lin.InfectionRisk;
            int hopeBefore = _state.Lin.Hope;

            GameSimulation.CheckDayPhaseActionAvailability(_state, "resolve_night");
            GameSimulation.CheckDayPhaseActionAvailability(_state, "next_day");
            GameSimulation.CheckDayPhaseActionAvailability(_state, "nonexistent");

            Assert.AreEqual(dayBefore, _state.Day);
            Assert.AreEqual(demoBefore, _state.DemoComplete);
            Assert.AreEqual(foodBefore, _state.Resources.Food);
            Assert.AreEqual(healthBefore, _state.Lin.Health);
            Assert.AreEqual(phaseBefore, _state.Phase);
            Assert.AreEqual(endingBefore, _state.EndingState);
            Assert.AreEqual(lastEventBefore, _state.LastEvent);
            // Exploration
            Assert.AreEqual(activeLocBefore, _state.Exploration.ActiveLocation);
            Assert.AreEqual(timeUsedBefore, _state.Exploration.TimeUsed);
            Assert.AreEqual(noiseBefore, _state.Exploration.Noise);
            Assert.AreEqual(searchedCountBefore, _state.Exploration.SearchedRooms.Count);
            Assert.AreEqual(luredCountBefore, _state.Exploration.LuredRooms.Count);
            // Qimian
            Assert.AreEqual(awakeBefore, _state.Qimian.Awake);
            Assert.AreEqual(logCountBefore, _state.Qimian.Log.Count);
            Assert.AreEqual(publicCluesBefore, _state.Qimian.PublicClues.Count);
            // Resources
            Assert.AreEqual(waterBefore, _state.Resources.Water);
            Assert.AreEqual(medsBefore, _state.Resources.Meds);
            Assert.AreEqual(materialsBefore, _state.Resources.Materials);
            Assert.AreEqual(partsBefore, _state.Resources.Parts);
            Assert.AreEqual(fuelBefore, _state.Resources.Fuel);
            // Lin
            Assert.AreEqual(fatigueBefore, _state.Lin.Fatigue);
            Assert.AreEqual(stressBefore, _state.Lin.Stress);
            Assert.AreEqual(infectionBefore, _state.Lin.InfectionRisk);
            Assert.AreEqual(hopeBefore, _state.Lin.Hope);
        }

        #endregion

        #region ======== CORE: Qimian & Reveal (lane: Code) ========

        [Test]
        public void TestQimianWakesOnDayFive()
        {
            QimianController.ResolveForDay(_state, 4);
            Assert.IsFalse(_state.Qimian.Awake);
            Assert.AreEqual(0, _state.Qimian.Log.Count);

            QimianController.ResolveForDay(_state, 5);
            Assert.IsTrue(_state.Qimian.Awake);
            Assert.AreEqual("寻找祁烬", _state.Qimian.PersonalityCard.MainGoal);
            Assert.GreaterOrEqual(_state.Qimian.Log.Count, 1);
            if (_state.Qimian.Log.Count > 0)
            {
                Assert.IsNotNull(_state.Qimian.Log[0].AiReplay);
                Assert.IsNotNull(_state.Qimian.Log[0].SubjectiveFragment);
            }
        }

        [Test]
        public void TestQimianReadsClinicHelpMarkOnWakeNight()
        {
            GameSimulation.AddPlayerMark(_state, "clinic", "help", "这里需要药，也可能有人会看懂这个记号。");
            _state.Day = 5;

            QimianController.ResolveForDay(_state, 5);

            Assert.IsTrue(_state.Qimian.Awake);
            Assert.IsTrue(_state.Qimian.Log.Any(entry =>
                    entry.AiReplay.Contains("社区诊所") &&
                    entry.AiReplay.Contains("求助标记")));
        }

        [Test]
        public void TestNightResultShowsQimianReadClinicHelpMark()
        {
            GameSimulation.AddPlayerMark(_state, "clinic", "help", "这里需要药，也可能有人会看懂这个记号。");
            _state.Day = 5;

            string result = GameSimulation.SleepAndResolveNight(_state);

            Assert.IsTrue(result.Contains("社区诊所"));
            Assert.IsTrue(result.Contains("求助标记"));
        }

        [Test]
        public void TestNightResultDoesNotDuplicateExistingQimianPublicClue()
        {
            GameSimulation.AddPlayerMark(_state, "clinic", "help", "这里需要药，也可能有人会看懂这个记号。");
            _state.Day = 5;

            string result = GameSimulation.SleepAndResolveNight(_state);

            int occurrences = result.Split(new[] { "远处旧楼有一扇门从里面被打开，又被人小心合上。" }, StringSplitOptions.None).Length - 1;
            Assert.AreEqual(1, occurrences);
        }

        [Test]
        public void TestClinicHelpMarkCreatesAnonymousMedicineFeedback()
        {
            GameSimulation.AddPlayerMark(_state, "clinic", "help", "这里需要药，也可能有人会看懂这个记号。");
            _state.Day = 5;
            int medsBefore = _state.Locations["clinic"].Resources["meds"];

            string result = GameSimulation.SleepAndResolveNight(_state);

            Assert.AreEqual(medsBefore + 1, _state.Locations["clinic"].Resources["meds"]);
            Assert.IsTrue(_state.Locations["clinic"].QimianTrace);
            Assert.IsTrue(_state.AnomalyDossier.Any(entry =>
                    entry.LocationId == "clinic" &&
                    entry.ClueText.Contains("匿名药品") &&
                    entry.Conclusion.Contains("理解标记")));
            Assert.IsTrue(result.Contains("匿名药品"));
        }

        [Test]
        public void TestEndingRevealExplainsClinicHelpMarkCausality()
        {
            GameSimulation.AddPlayerMark(_state, "clinic", "help", "这里需要药，也可能有人会看懂这个记号。");
            _state.Day = 5;
            GameSimulation.SleepAndResolveNight(_state);

            _state.Day = 15;
            _state.Lin.Health = 10;
            _state.Resources.Food = 3;
            _state.Resources.Water = 3;
            _state.Shelter.Door = 4;
            _state.Shelter.Defense = 2;
            _state.Shelter.Noise = 0;
            _state.Shelter.Scent = 0;
            _state.Shelter.Light = 0;
            _state.Car.Found = true;
            _state.Car.Ready = true;
            _state.Evacuation.SafezoneConfirmed = true;
            _state.Evacuation.AddressKnown = true;
            _state.Evacuation.CarReady = true;

            GameSimulation.SleepAndResolveNight(_state);

            Assert.IsTrue(_state.Reveal.Unlocked);
            Assert.IsTrue(_state.Reveal.Summary.Contains("人格卡"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("感知输入"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("候选行动"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("排序"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("最终选择"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("地图影响"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("社区诊所"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("求助标记"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("匿名药品"));
        }

        [Test]
        public void TestMinimumVerticalSliceCoversClinicAiChain()
        {
            string dayOneEntry = GameSimulation.EnterLocation(_state, "convenience");
            string dayOneSearch = GameSimulation.SearchRoom(_state, "storefront", "careful");
            string dayOneLeave = GameSimulation.LeaveExploration(_state);
            string dayOneNight = GameSimulation.SleepAndResolveNight(_state);

            Assert.IsTrue(dayOneEntry.Contains("进入"));
            Assert.IsTrue(dayOneSearch.Contains("带回"));
            Assert.IsTrue(dayOneLeave.Contains("回到据点"));
            Assert.IsTrue(dayOneNight.Contains("第 2 天清晨"));
            Assert.AreEqual(2, _state.Day);

            GameSimulation.StartDay(_state, 5);
            GameSimulation.EnterLocation(_state, "clinic");
            string clinicSearch = GameSimulation.SearchRoom(_state, "exam_a", "careful");
            GameSimulation.AddPlayerMark(_state, "clinic", "help", "这里需要药，也可能有人会看懂这个记号。");
            GameSimulation.LeaveExploration(_state);
            string qimianNight = GameSimulation.SleepAndResolveNight(_state);

            Assert.IsTrue(clinicSearch.Contains("隔离记录"));
            Assert.IsTrue(_state.PlayerMarks.ContainsKey("clinic"));
            Assert.IsTrue(GameSimulation.GetAnomalyDossierText(_state).Contains("诊所隔离记录"));
            Assert.IsTrue(qimianNight.Contains("求助标记"));
            Assert.IsTrue(qimianNight.Contains("匿名药品"));
            Assert.IsTrue(GameSimulation.GetLocationCardText(_state, "clinic").Contains("祁眠异常"));
            Assert.IsTrue(GameSimulation.GetAnomalyDossierText(_state).Contains("匿名药品"));

            _state.Day = 15;
            _state.Lin.Health = 10;
            _state.Resources.Food = 3;
            _state.Resources.Water = 3;
            _state.Shelter.Door = 4;
            _state.Shelter.Defense = 2;
            _state.Shelter.Noise = 0;
            _state.Shelter.Scent = 0;
            _state.Shelter.Light = 0;
            _state.Car.Found = true;
            _state.Car.Ready = true;
            _state.Evacuation.SafezoneConfirmed = true;
            _state.Evacuation.AddressKnown = true;
            _state.Evacuation.CarReady = true;

            GameSimulation.SleepAndResolveNight(_state);

            Assert.IsTrue(_state.Reveal.Unlocked);
            Assert.IsTrue(_state.Reveal.Summary.Contains("人格卡"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("感知输入"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("最终选择"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("地图影响"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("社区诊所"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("求助标记"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("匿名药品"));
        }

        [Test]
        public void TestDemoRevealContainsHiddenCausality()
        {
            for (int day = 1; day <= 15; day++)
                GameSimulation.PlaySafeDemoDay(_state, day);

            Assert.IsTrue(_state.DemoComplete);
            Assert.IsTrue(_state.Reveal.Unlocked);
            Assert.GreaterOrEqual(_state.Qimian.Log.Count, 5);
            Assert.IsTrue(_state.BloodMoonsResolved.Contains(7));
            Assert.IsTrue(_state.BloodMoonsResolved.Contains(15));
            Assert.IsTrue(new[] { "reached_gate_quarantine", "barely_reached_gate", "collapsed" }
                .Contains(_state.EndingState));
            Assert.IsTrue(_state.Reveal.Summary.Contains("保护区"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("隔离"));
            Assert.IsTrue(_state.Reveal.Summary.Contains("尸群"));
        }

        [Test]
        public void TestDayFifteenAssignsEndingState()
        {
            for (int day = 1; day <= 15; day++)
                GameSimulation.PlaySafeDemoDay(_state, day);

            Assert.AreEqual("reveal", _state.Phase);
            Assert.IsTrue(new[] { "reached_gate_quarantine", "barely_reached_gate" }
                .Contains(_state.EndingState));
            Assert.IsTrue(_state.Reveal.Summary.Contains("初筛") || _state.Reveal.Summary.Contains("隔离观察"));
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

            Assert.IsTrue(_state.DemoComplete);
            Assert.AreEqual("collapsed", _state.EndingState);
        }

        #endregion

    }
}
