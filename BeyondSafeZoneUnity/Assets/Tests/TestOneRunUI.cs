using System;
using System.Linq;
using System.Reflection;
using BeyondSafeZone.Model;
using BeyondSafeZone.Player;
using BeyondSafeZone.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace BeyondSafeZone.Tests
{
    [TestFixture]
    public class TestOneRunUI
    {
        [Test]
        public void TestOneRunControllerExposesDossierPanelActions()
        {
            Type controllerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("BeyondSafeZone.UI.OneRunGameController"))
                .FirstOrDefault(type => type != null);
            Assert.IsNotNull(controllerType);

            var toggleMethod = controllerType.GetMethod("ToggleDossierPanel");
            Assert.IsNotNull(toggleMethod);
            Assert.AreEqual(typeof(void), toggleMethod.ReturnType);
            Assert.AreEqual(0, toggleMethod.GetParameters().Length);

            var refreshMethod = controllerType.GetMethod("RefreshDossierPanel");
            Assert.IsNotNull(refreshMethod);
            Assert.AreEqual(typeof(void), refreshMethod.ReturnType);
            Assert.AreEqual(0, refreshMethod.GetParameters().Length);
        }

        [Test]
        public void TestOneRunControllerExposesQimianLogPanelActions()
        {
            Type controllerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("BeyondSafeZone.UI.OneRunGameController"))
                .FirstOrDefault(type => type != null);
            Assert.IsNotNull(controllerType);

            var toggleMethod = controllerType.GetMethod("ToggleQimianLogPanel");
            Assert.IsNotNull(toggleMethod);
            Assert.AreEqual(typeof(void), toggleMethod.ReturnType);
            Assert.AreEqual(0, toggleMethod.GetParameters().Length);

            var refreshMethod = controllerType.GetMethod("RefreshQimianLogPanel");
            Assert.IsNotNull(refreshMethod);
            Assert.AreEqual(typeof(void), refreshMethod.ReturnType);
            Assert.AreEqual(0, refreshMethod.GetParameters().Length);
        }

        [Test]
        public void TestDossierButtonOpensEmptyDossierPanel()
        {
            GameObject controllerObject = new GameObject("OneRunController_U1");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);
                Assert.IsNotNull(shelter.transform.Find("CutawayShelterFrame"));
                Assert.IsNotNull(shelter.transform.Find("ShelterFloor_Ground"));
                Assert.IsNotNull(shelter.transform.Find("ShelterFloor_Upper"));
                Assert.IsNotNull(shelter.transform.Find("Stairs_GroundToUpper"));

                Type sideViewControllerType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("BeyondSafeZone.Player.SideViewShelterPlayerController"))
                    .FirstOrDefault(type => type != null);
                Assert.IsNotNull(sideViewControllerType);

                Transform player = shelter.transform.Find("LinXing_Player");
                Assert.IsNotNull(player);
                Assert.IsNotNull(player.GetComponent(sideViewControllerType));
                Assert.IsNull(player.GetComponent<TopDownPlayerController>());

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);

                Transform dossierPanel = hud.transform.Find("DossierPanel");
                Transform dossierButton = hud.transform.Find("DossierButton");
                Assert.IsNotNull(dossierPanel);
                Assert.IsNotNull(dossierButton);
                Assert.IsFalse(dossierPanel.gameObject.activeSelf);

                Button button = dossierButton.GetComponent<Button>();
                Assert.IsNotNull(button);
                button.onClick.Invoke();

                Assert.IsTrue(dossierPanel.gameObject.activeSelf);
                Component bodyText = dossierPanel.Find("DossierBody")
                    .GetComponents<Component>()
                    .First(component => component.GetType().FullName == "TMPro.TextMeshProUGUI");
                string bodyValue = bodyText.GetType().GetProperty("text").GetValue(bodyText) as string;
                Assert.AreEqual("暂无异常记录。", bodyValue);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestQimianLogButtonOpensLockedLogPanel()
        {
            GameObject controllerObject = new GameObject("OneRunController_U2");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);

                Transform logPanel = hud.transform.Find("QimianLogPanel");
                Transform logButton = hud.transform.Find("QimianLogButton");
                Assert.IsNotNull(logPanel);
                Assert.IsNotNull(logButton);
                Assert.IsFalse(logPanel.gameObject.activeSelf);

                Button button = logButton.GetComponent<Button>();
                Assert.IsNotNull(button);
                button.onClick.Invoke();

                Assert.IsTrue(logPanel.gameObject.activeSelf);
                Component bodyText = logPanel.Find("QimianLogBody")
                    .GetComponents<Component>()
                    .First(component => component.GetType().FullName == "TMPro.TextMeshProUGUI");
                string bodyValue = bodyText.GetType().GetProperty("text").GetValue(bodyText) as string;
                Assert.AreEqual("通关后解锁祁眠行动日志。", bodyValue);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestOneRunHudShowsCurrentObjectiveAndChinesePhase()
        {
            GameObject controllerObject = new GameObject("OneRunController_U3");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);

                Transform objectivePanel = hud.transform.Find("ObjectivePanel");
                Assert.IsNotNull(objectivePanel);
                Assert.IsTrue(objectivePanel.gameObject.activeSelf);

                Component objectiveTitle = OneRunTestHelpers.GetTextComponent(objectivePanel.Find("ObjectiveTitle"));
                Assert.AreEqual("当前目标", OneRunTestHelpers.GetTextValue(objectiveTitle));

                Component objectiveBody = OneRunTestHelpers.GetTextComponent(objectivePanel.Find("ObjectiveBody"));
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(objectiveBody).Contains("白天外出搜刮"));

                Component header = OneRunTestHelpers.GetTextComponent(hud.transform.Find("ReadabilitySafeFrame/Header"));
                string headerValue = OneRunTestHelpers.GetTextValue(header);
                Assert.IsTrue(headerValue.Contains("第 1 天"));
                Assert.IsTrue(headerValue.Contains("清晨"));
                Assert.IsFalse(headerValue.Contains("Phase morning"));

                controller.State.Day = 5;
                MethodInfo refreshMethod = typeof(OneRunGameController).GetMethod("RefreshAll", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(refreshMethod);
                refreshMethod.Invoke(controller, null);

                string bodyDay5 = OneRunTestHelpers.GetTextValue(objectiveBody);
                Assert.IsTrue(bodyDay5.Contains("未知行动者"));
                Assert.IsTrue(bodyDay5.Contains("诊所"));
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestOneRunHudShowsLocationCards()
        {
            GameObject controllerObject = new GameObject("OneRunController_U4");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);

                Transform cardPanel = hud.transform.Find("LocationCardPanel");
                Assert.IsNotNull(cardPanel);

                Transform clinicCard = cardPanel.Find("LocationCard_clinic");
                Transform supermarketCard = cardPanel.Find("LocationCard_supermarket");
                Transform bikeShopCard = cardPanel.Find("LocationCard_bike_shop");
                Assert.IsNotNull(clinicCard);
                Assert.IsNotNull(supermarketCard);
                Assert.IsNotNull(bikeShopCard);

                Component clinicName = OneRunTestHelpers.GetTextComponent(clinicCard.Find("LocationName_clinic"));
                Component clinicInfo = OneRunTestHelpers.GetTextComponent(clinicCard.Find("LocationInfo_clinic"));
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(clinicName).Contains("社区诊所"));
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(clinicInfo).Contains("药品"));
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(clinicInfo).Contains("危险"));
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(clinicInfo).Contains("待调查"));

                controller.State.PlayerMarks["clinic"] = new PlayerMark { Type = "help", Day = 1, Note = "这里需要药" };
                MethodInfo refreshMethod = typeof(OneRunGameController).GetMethod("RefreshAll", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(refreshMethod);
                refreshMethod.Invoke(controller, null);
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(clinicInfo).Contains("已留标记"));

                controller.State.Locations["clinic"].QimianTrace = true;
                refreshMethod.Invoke(controller, null);
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(clinicInfo).Contains("有新痕迹"));
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }
        [Test]
        public void TestShelterFacilityPromptShowsUnavailableReason()
        {
            GameObject controllerObject = new GameObject("OneRunController_B1");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                // morning/day 阶段允许据点行动查询，真实执行时 UI 会先 EnsureShelterActionPhase。
                controller.State.Phase = "morning";
                controller.ShowPrompt("bed");

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);
                Transform promptPanel = hud.transform.Find("PromptPanel");
                Assert.IsNotNull(promptPanel);
                Component prompt = OneRunTestHelpers.GetTextComponent(promptPanel.Find("Prompt"));
                string promptValue = OneRunTestHelpers.GetTextValue(prompt);
                Assert.IsTrue(promptValue.Contains("按 E"));

                // searching 阶段仍不可执行据点行动。
                controller.State.Phase = "searching";
                controller.ShowPrompt("bed");
                string searchingPrompt = OneRunTestHelpers.GetTextValue(prompt);
                Assert.IsTrue(searchingPrompt.Contains("现在不是执行据点行动的时机"));

                // 切换到 evening 阶段，bed 已建造，rest_bed 可用
                controller.State.Phase = "evening";
                controller.State.Shelter.Facilities["bed"].Built = true;
                controller.ShowPrompt("bed");
                string eveningPrompt = OneRunTestHelpers.GetTextValue(prompt);
                Assert.IsTrue(eveningPrompt.Contains("按 E"));

                // 销毁 bed 设施，使 rest_bed 不可用（需要先建造床铺）
                if (controller.State.Shelter.Facilities.TryGetValue("bed", out var bedFacility))
                    bedFacility.Built = false;
                controller.ShowPrompt("bed");
                // 未建造时 GetActionForFacility 返回 build_bed，检查建材：默认建材=4，建造需要2，所以可用
                // 直接测试不可用场景：建材不足
                controller.State.Resources.Materials = 0;
                controller.State.Shelter.Facilities["bed"].Built = false;
                controller.ShowPrompt("bed");
                string noMaterialsPrompt = OneRunTestHelpers.GetTextValue(prompt);
                Assert.IsTrue(noMaterialsPrompt.Contains("材料不够"));
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        public void TestHudUnavailableButtonsRemainClickableAndReportReasons()
        {
            GameObject controllerObject = new GameObject("OneRunController_B2");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);
                MethodInfo refreshMethod = typeof(OneRunGameController).GetMethod("RefreshAll", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(refreshMethod);

                Component logText = OneRunTestHelpers.GetTextComponent(hud.transform.Find("LogPanel/Log"));
                Transform exploreTransform = hud.transform.Find("ExploreClinic");
                Transform returnTransform = hud.transform.Find("ReturnShelter");
                Transform helpTransform = hud.transform.Find("LeaveHelpMark");
                Transform resolveTransform = hud.transform.Find("ResolveNight");
                Transform nextDayTransform = hud.transform.Find("NextDay");
                Assert.IsNotNull(exploreTransform, "ExploreClinic button exists");
                Assert.IsNotNull(returnTransform, "ReturnShelter button exists");
                Assert.IsNotNull(helpTransform, "LeaveHelpMark button exists");
                Assert.IsNotNull(resolveTransform, "ResolveNight button exists");
                Assert.IsNotNull(nextDayTransform, "NextDay button exists");

                Button exploreButton = exploreTransform.GetComponent<Button>();
                Button returnButton = returnTransform.GetComponent<Button>();
                Button helpButton = helpTransform.GetComponent<Button>();
                Button resolveButton = resolveTransform.GetComponent<Button>();
                Button nextDayButton = nextDayTransform.GetComponent<Button>();
                Assert.IsNotNull(exploreButton, "ExploreClinic has Button");
                Assert.IsNotNull(returnButton, "ReturnShelter has Button");
                Assert.IsNotNull(helpButton, "LeaveHelpMark has Button");
                Assert.IsNotNull(resolveButton, "ResolveNight has Button");
                Assert.IsNotNull(nextDayButton, "NextDay has Button");

                // 搜刮中点击"去诊所"：按钮仍可点击，日志显示原因
                controller.State.Phase = "searching";
                refreshMethod.Invoke(controller, null);
                Assert.IsTrue(exploreButton.interactable, "搜刮中外出按钮应保持可点击");
                exploreButton.onClick.Invoke();
                string logAfterExplore = OneRunTestHelpers.GetTextValue(logText);
                Debug.Log("B-FIX-001 diagnose after explore: " + logAfterExplore);
                Assert.IsTrue(logAfterExplore.Contains("正在搜刮中，无法再次外出"), logAfterExplore);

                // 非搜刮中点击"返回据点"
                controller.State.Phase = "morning";
                refreshMethod.Invoke(controller, null);
                Assert.IsTrue(returnButton.interactable, "非搜刮中返回按钮应保持可点击");
                returnButton.onClick.Invoke();
                string logAfterReturn = OneRunTestHelpers.GetTextValue(logText);
                Debug.Log("B-FIX-001 diagnose after return: " + logAfterReturn);
                Assert.IsTrue(logAfterReturn.Contains("当前不在搜刮中"), logAfterReturn);

                // 非搜刮中点击"留下求助"
                Assert.IsTrue(helpButton.interactable, "非搜刮中求助按钮应保持可点击");
                helpButton.onClick.Invoke();
                string logAfterHelp = OneRunTestHelpers.GetTextValue(logText);
                Debug.Log("B-FIX-001 diagnose after help: " + logAfterHelp);
                Assert.IsTrue(logAfterHelp.Contains("只有在搜刮中才能留下求助标记"), logAfterHelp);

                // DemoComplete 后点击"夜晚结算"
                controller.State.DemoComplete = true;
                refreshMethod.Invoke(controller, null);
                Assert.IsTrue(resolveButton.interactable, "DemoComplete 夜晚结算按钮应保持可点击");
                resolveButton.onClick.Invoke();
                string logAfterResolve = OneRunTestHelpers.GetTextValue(logText);
                Debug.Log("B-FIX-001 diagnose after resolve: " + logAfterResolve);
                Assert.IsTrue(logAfterResolve.Contains("演示已完成，无法再进行夜晚结算"), logAfterResolve);

                // DemoComplete 后点击"下一天"
                Assert.IsTrue(nextDayButton.interactable, "DemoComplete 下一天按钮应保持可点击");
                nextDayButton.onClick.Invoke();
                string logAfterNextDay = OneRunTestHelpers.GetTextValue(logText);
                Debug.Log("B-FIX-001 diagnose after next day: " + logAfterNextDay);
                Assert.IsTrue(logAfterNextDay.Contains("演示已完成，无法再推进天数"), logAfterNextDay);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestShelterPromptShowsPhaseAndResourceReasons()
        {
            GameObject controllerObject = new GameObject("OneRunController_B3");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);
                Component prompt = OneRunTestHelpers.GetTextComponent(hud.transform.Find("PromptPanel/Prompt"));

                // day 阶段允许据点行动查询，真实执行时 UI 会自动转 evening。
                controller.State.Phase = "day";
                controller.ShowPrompt("workbench");
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(prompt).Contains("按 E"));

                // searching 阶段显示阶段不允许。
                controller.State.Phase = "searching";
                controller.ShowPrompt("workbench");
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(prompt).Contains("不是执行据点行动的时机"));

                // evening 阶段，但资源不足时显示材料不够
                controller.State.Phase = "evening";
                controller.State.Resources.Materials = 0;
                controller.State.Resources.Parts = 0;
                // 销毁 workbench，使 GetActionForFacility 返回 build_workbench
                if (controller.State.Shelter.Facilities.TryGetValue("workbench", out var wb))
                    wb.Built = false;
                controller.ShowPrompt("workbench");
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(prompt).Contains("材料不够"));

                // radio_broadcast 燃料不足
                controller.State.Resources.Fuel = 0;
                controller.ShowPrompt("radio");
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(prompt).Contains("燃料不足"));
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }
    }
}
