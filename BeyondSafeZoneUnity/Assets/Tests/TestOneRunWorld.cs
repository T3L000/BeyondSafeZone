using System;
using System.Linq;
using System.Reflection;
using BeyondSafeZone.Player;
using BeyondSafeZone.UI;
using BeyondSafeZone.World;
using NUnit.Framework;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace BeyondSafeZone.Tests
{
    [TestFixture]
    public class TestOneRunWorld
    {
        [Test]
        public void TestOneRunControllerExposesHelpMarkAction()
        {
            Type controllerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("BeyondSafeZone.UI.OneRunGameController"))
                .FirstOrDefault(type => type != null);
            Assert.IsNotNull(controllerType);

            var method = controllerType.GetMethod("LeaveHelpMarkAtActiveLocation");
            Assert.IsNotNull(method);
            Assert.AreEqual(typeof(void), method.ReturnType);
            Assert.AreEqual(0, method.GetParameters().Length);
        }

        [Test]
        public void TestOneRunShelterUsesSideViewCutawayController()
        {
            Type controllerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("BeyondSafeZone.Player.SideViewShelterPlayerController"))
                .FirstOrDefault(type => type != null);
            Assert.IsNotNull(controllerType);

            GameObject controllerObject = new GameObject("OneRunController_W1");
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

                Transform player = shelter.transform.Find("LinXing_Player");
                Assert.IsNotNull(player);
                Assert.IsNotNull(player.GetComponent(controllerType));
                Assert.IsNull(player.GetComponent<TopDownPlayerController>());
                Assert.IsNotNull(controllerType.GetMethod("ReadHorizontalInput", BindingFlags.Instance | BindingFlags.NonPublic));

                var playerRenderer = player.GetComponent<SpriteRenderer>();
                Assert.IsNotNull(playerRenderer);
                Assert.Greater(playerRenderer.sprite.texture.width, 1);
                Assert.Greater(playerRenderer.sprite.texture.height, 1);

                foreach (string facilityId in new[] { "bed", "workbench", "stove", "barricade", "radio", "storage" })
                {
                    Transform facility = shelter.transform.Find("Facility_" + facilityId);
                    Assert.IsNotNull(facility);
                    Assert.IsNotNull(facility.Find("State_" + facilityId));
                }
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestOneRunVisualReadabilityScaffold()
        {
            GameObject controllerObject = new GameObject("OneRunController_W2");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                Camera cam = Camera.main;
                Assert.IsNotNull(cam);
                Assert.AreEqual(CameraClearFlags.SolidColor, cam.clearFlags);
                Assert.LessOrEqual(cam.orthographicSize, 5.0f);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);
                Assert.IsNotNull(hud.transform.Find("ReadabilitySafeFrame"));
                Assert.IsNotNull(hud.transform.Find("StatusPanel"));
                Assert.IsNotNull(hud.transform.Find("LogPanel"));
                Assert.IsNotNull(hud.transform.Find("PromptPanel"));

                Component header = OneRunTestHelpers.GetTextComponent(hud.transform.Find("ReadabilitySafeFrame/Header"));
                Component status = OneRunTestHelpers.GetTextComponent(hud.transform.Find("StatusPanel/Status"));
                Component prompt = OneRunTestHelpers.GetTextComponent(hud.transform.Find("PromptPanel/Prompt"));
                Assert.GreaterOrEqual(OneRunTestHelpers.GetFontSize(header), 24);
                Assert.GreaterOrEqual(OneRunTestHelpers.GetFontSize(status), 18);
                Assert.GreaterOrEqual(OneRunTestHelpers.GetFontSize(prompt), 18);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);
                Transform player = shelter.transform.Find("LinXing_Player");
                Transform workbench = shelter.transform.Find("Facility_workbench");
                Assert.IsNotNull(player);
                Assert.IsNotNull(workbench);

                SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
                SpriteRenderer workbenchRenderer = workbench.GetComponent<SpriteRenderer>();
                Assert.Greater(playerRenderer.sortingOrder, workbenchRenderer.sortingOrder);

                Component bedLabel = OneRunTestHelpers.GetTextComponent(shelter.transform.Find("Facility_bed/Label_bed"));
                Assert.GreaterOrEqual(OneRunTestHelpers.GetFontSize(bedLabel), 1.05f);
                Assert.LessOrEqual(OneRunTestHelpers.GetTextValue(bedLabel).Length, 4);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestShelterFacilityVisualsExposeBuildUseAndDamageState()
        {
            GameObject controllerObject = new GameObject("OneRunController_W3");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);

                Transform bed = shelter.transform.Find("Facility_bed");
                Transform workbench = shelter.transform.Find("Facility_workbench");
                Transform barricade = shelter.transform.Find("Facility_barricade");
                Transform radio = shelter.transform.Find("Facility_radio");
                Assert.IsNotNull(bed);
                Assert.IsNotNull(workbench);
                Assert.IsNotNull(barricade);
                Assert.IsNotNull(radio);

                Assert.IsNotNull(bed.Find("Blueprint_bed"));
                Assert.IsNotNull(workbench.Find("Built_workbench"));
                Assert.IsNotNull(workbench.Find("UsedMarker_workbench"));
                Assert.IsNotNull(barricade.Find("DamageMarker_barricade"));
                Assert.IsNotNull(radio.Find("Feedback_radio"));

                controller.State.Shelter.Facilities["bed"].Built = false;
                controller.State.Shelter.Facilities["workbench"].Built = true;
                controller.State.Shelter.Facilities["workbench"].UsedToday = true;
                controller.State.Shelter.Door = 2;

                MethodInfo refreshMethod = typeof(OneRunGameController).GetMethod("RefreshAll", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(refreshMethod);
                refreshMethod.Invoke(controller, null);

                Assert.IsTrue(bed.Find("Blueprint_bed").gameObject.activeSelf);
                Assert.IsFalse(bed.Find("Built_bed").gameObject.activeSelf);
                Assert.IsTrue(workbench.Find("Built_workbench").gameObject.activeSelf);
                Assert.IsTrue(workbench.Find("UsedMarker_workbench").gameObject.activeSelf);
                Assert.IsTrue(barricade.Find("DamageMarker_barricade").gameObject.activeSelf);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestShelterInteractionShowsVisibleFeedbackText()
        {
            GameObject controllerObject = new GameObject("OneRunController_W4");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);

                Transform radio = shelter.transform.Find("Facility_radio");
                Assert.IsNotNull(radio);
                ShelterInteractable interactable = radio.GetComponent<ShelterInteractable>();
                Assert.IsNotNull(interactable);

                string result = interactable.Interact(controller);
                Component feedbackText = OneRunTestHelpers.GetTextComponent(radio.Find("Feedback_radio"));
                string feedbackValue = OneRunTestHelpers.GetTextValue(feedbackText);
                Assert.IsFalse(string.IsNullOrWhiteSpace(feedbackValue));
                Assert.AreEqual(result, feedbackValue);

                GameObject hud = GameObject.Find("OneRunHUD");
                Assert.IsNotNull(hud);
                Component logText = OneRunTestHelpers.GetTextComponent(hud.transform.Find("LogPanel/Log"));
                Assert.IsTrue(OneRunTestHelpers.GetTextValue(logText).Contains(result));
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestShelterInteractableHighlightOnApproach()
        {
            GameObject controllerObject = new GameObject("OneRunController_W5");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);

                Transform radio = shelter.transform.Find("Facility_radio");
                Assert.IsNotNull(radio);
                ShelterInteractable interactable = radio.GetComponent<ShelterInteractable>();
                Assert.IsNotNull(interactable);

                Transform stateRadio = radio.Find("State_radio");
                Assert.IsNotNull(stateRadio);
                SpriteRenderer stateRenderer = stateRadio.GetComponent<SpriteRenderer>();
                Assert.IsNotNull(stateRenderer);
                Color normalColor = stateRenderer.color;

                // call SetHighlighted(true) and verify color changed
                interactable.SetHighlighted(true);
                Assert.IsTrue(interactable.IsHighlighted);
                Color highlightedColor = stateRenderer.color;
                Assert.AreNotEqual(normalColor, highlightedColor);

                // call SetHighlighted(false) and verify color restores
                interactable.SetHighlighted(false);
                Assert.IsFalse(interactable.IsHighlighted);
                Assert.AreEqual(normalColor, stateRenderer.color);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestShelterInteractableHighlightOnOverlappingRange()
        {
            GameObject controllerObject = new GameObject("OneRunController_W6");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);

                Transform radio = shelter.transform.Find("Facility_radio");
                Transform stove = shelter.transform.Find("Facility_stove");
                Assert.IsNotNull(radio);
                Assert.IsNotNull(stove);

                ShelterInteractable radioInteractable = radio.GetComponent<ShelterInteractable>();
                ShelterInteractable stoveInteractable = stove.GetComponent<ShelterInteractable>();
                Assert.IsNotNull(radioInteractable);
                Assert.IsNotNull(stoveInteractable);

                SpriteRenderer radioState = radio.Find("State_radio").GetComponent<SpriteRenderer>();
                SpriteRenderer stoveState = stove.Find("State_stove").GetComponent<SpriteRenderer>();
                Assert.IsNotNull(radioState);
                Assert.IsNotNull(stoveState);

                Color radioNormal = radioState.color;
                Color stoveNormal = stoveState.color;

                // enter facility A (radio)
                radioInteractable.SetHighlighted(true);
                Assert.IsTrue(radioInteractable.IsHighlighted);
                Assert.IsFalse(stoveInteractable.IsHighlighted);

                // enter facility B (stove) — A should unhighlight, B should highlight
                radioInteractable.SetHighlighted(false);
                stoveInteractable.SetHighlighted(true);
                Assert.IsFalse(radioInteractable.IsHighlighted);
                Assert.IsTrue(stoveInteractable.IsHighlighted);
                Assert.AreEqual(radioNormal, radioState.color);
                Assert.AreNotEqual(stoveNormal, stoveState.color);

                // exit A again (already not current) — B must stay highlighted
                radioInteractable.SetHighlighted(false);
                Assert.IsFalse(radioInteractable.IsHighlighted);
                Assert.IsTrue(stoveInteractable.IsHighlighted);
                Assert.AreEqual(radioNormal, radioState.color);
                Assert.AreNotEqual(stoveNormal, stoveState.color);

                // exit B — both unhighlighted
                stoveInteractable.SetHighlighted(false);
                Assert.IsFalse(radioInteractable.IsHighlighted);
                Assert.IsFalse(stoveInteractable.IsHighlighted);
                Assert.AreEqual(radioNormal, radioState.color);
                Assert.AreEqual(stoveNormal, stoveState.color);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }

        [Test]
        public void TestSideViewShelterPlayerControllerSwitchesHighlightOnTriggerOverlap()
        {
            GameObject controllerObject = new GameObject("OneRunController_W7");
            OneRunGameController controller = controllerObject.AddComponent<OneRunGameController>();

            try
            {
                MethodInfo startMethod = typeof(OneRunGameController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(startMethod);
                startMethod.Invoke(controller, null);

                GameObject shelter = GameObject.Find("WalkableShelterGreybox");
                Assert.IsNotNull(shelter);

                Transform player = shelter.transform.Find("LinXing_Player");
                Assert.IsNotNull(player);
                var playerController = player.GetComponent<SideViewShelterPlayerController>();
                Assert.IsNotNull(playerController);

                Transform radio = shelter.transform.Find("Facility_radio");
                Transform stove = shelter.transform.Find("Facility_stove");
                Assert.IsNotNull(radio);
                Assert.IsNotNull(stove);

                Collider2D radioCollider = radio.GetComponent<Collider2D>();
                Collider2D stoveCollider = stove.GetComponent<Collider2D>();
                Assert.IsNotNull(radioCollider);
                Assert.IsNotNull(stoveCollider);

                ShelterInteractable radioInteractable = radio.GetComponent<ShelterInteractable>();
                ShelterInteractable stoveInteractable = stove.GetComponent<ShelterInteractable>();
                Assert.IsNotNull(radioInteractable);
                Assert.IsNotNull(stoveInteractable);

                SpriteRenderer radioState = radio.Find("State_radio").GetComponent<SpriteRenderer>();
                SpriteRenderer stoveState = stove.Find("State_stove").GetComponent<SpriteRenderer>();
                Assert.IsNotNull(radioState);
                Assert.IsNotNull(stoveState);

                Type playerType = typeof(SideViewShelterPlayerController);
                MethodInfo enterMethod = playerType.GetMethod("OnTriggerEnter2D", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo exitMethod = playerType.GetMethod("OnTriggerExit2D", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(enterMethod);
                Assert.IsNotNull(exitMethod);

                Color radioNormal = radioState.color;
                Color stoveNormal = stoveState.color;

                // enter radio
                enterMethod.Invoke(playerController, new object[] { radioCollider });
                Assert.IsTrue(radioInteractable.IsHighlighted);
                Assert.IsFalse(stoveInteractable.IsHighlighted);
                Assert.AreNotEqual(radioNormal, radioState.color);

                // enter stove — radio unhighlights, stove highlights
                enterMethod.Invoke(playerController, new object[] { stoveCollider });
                Assert.IsFalse(radioInteractable.IsHighlighted);
                Assert.AreEqual(radioNormal, radioState.color);
                Assert.IsTrue(stoveInteractable.IsHighlighted);
                Assert.AreNotEqual(stoveNormal, stoveState.color);

                // exit radio (not current) — stove stays highlighted
                exitMethod.Invoke(playerController, new object[] { radioCollider });
                Assert.IsFalse(radioInteractable.IsHighlighted);
                Assert.IsTrue(stoveInteractable.IsHighlighted);
                Assert.AreNotEqual(stoveNormal, stoveState.color);

                // exit stove — stove unhighlights
                exitMethod.Invoke(playerController, new object[] { stoveCollider });
                Assert.IsFalse(radioInteractable.IsHighlighted);
                Assert.IsFalse(stoveInteractable.IsHighlighted);
                Assert.AreEqual(radioNormal, radioState.color);
                Assert.AreEqual(stoveNormal, stoveState.color);
            }
            finally
            {
                UnityObject.DestroyImmediate(controllerObject);
                OneRunTestHelpers.DestroyKnownSceneObjects();
            }
        }
    }
}
