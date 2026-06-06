using System.Collections.Generic;
using BeyondSafeZone.Core;
using BeyondSafeZone.Model;
using BeyondSafeZone.Player;
using BeyondSafeZone.World;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeyondSafeZone.UI
{
    public class OneRunGameController : MonoBehaviour
    {
        private const string LinXingSpriteResourcePath = "Sprites/Characters/lin_xing_player";

        private readonly List<ShelterInteractable> shelterInteractables = new();
        private readonly List<ScavengeSearchPoint> searchPoints = new();
        private readonly List<string> logLines = new();

        private Sprite sharedOnePixelSprite;
        private GameObject shelterRoot;
        private GameObject scavengeRoot;
        private Transform playerTransform;

        private TMP_Text headerText;
        private TMP_Text statusText;
        private TMP_Text promptText;
        private TMP_Text logText;
        private GameObject dossierPanel;
        private TMP_Text dossierBodyText;
        private GameObject qimianLogPanel;
        private TMP_Text qimianLogBodyText;
        private Button returnShelterButton;
        private Button leaveHelpMarkButton;
        private Button exploreClinicButton;
        private Button exploreSupermarketButton;
        private Button exploreGarageButton;
        private Button resolveNightButton;
        private Button nextDayButton;
        private TMP_Text objectiveTitleText;
        private TMP_Text objectiveBodyText;
        private readonly Dictionary<string, TMP_Text> locationCardNameTexts = new();
        private readonly Dictionary<string, TMP_Text> locationCardInfoTexts = new();

        public GameState State { get; private set; }

        private void Start()
        {
            State = GameSimulation.NewGame();
            BuildCamera();
            BuildShelterGreybox();
            BuildHud();
            Report("林行回到据点。用 WASD/方向键移动，靠近设施按 E 互动。");
            RefreshAll();
        }

        public void EnsureShelterActionPhase()
        {
            if (State == null || State.DemoComplete) return;
            if (State.Phase == "morning" || State.Phase == "day")
                State.Phase = "evening";
        }

        public void Report(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            logLines.Add(text);
            while (logLines.Count > 10)
                logLines.RemoveAt(0);
            RefreshAll();
        }

    public void ShowPrompt(string facilityId)
    {
        if (promptText == null || State == null) return;
        if (facilityId == "stairs")
        {
            promptText.text = "楼梯：按 W/S 或 ↑/↓ 上下楼";
            return;
        }

        if (string.IsNullOrEmpty(facilityId))
        {
            promptText.text = GetDefaultPromptText();
            return;
        }

        string basePrompt = ShelterInteractionCatalog.GetPrompt(State, facilityId) + "  按 E";

        // 检查据点行动可用性，不满足时显示失败原因
        string actionId = ShelterInteractionCatalog.GetActionForFacility(State, facilityId);
        if (!string.IsNullOrEmpty(actionId))
        {
            var availability = GameSimulation.CheckShelterActionAvailability(State, actionId);
            if (!availability.Available)
                basePrompt = availability.FailureReason;
        }

        promptText.text = basePrompt;
    }

        public void ShowSearchPrompt(string searchPointName)
        {
            if (promptText == null) return;
            promptText.text = string.IsNullOrEmpty(searchPointName)
                ? "靠近搜索点按 E 搜索"
                : $"{searchPointName}：按 E 搜索";
        }

        private void BuildCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cam = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.orthographicSize = 4.9f;
            cam.transform.position = new Vector3(0f, 0.35f, -10f);
            cam.backgroundColor = new Color(0.055f, 0.065f, 0.065f);
        }

        private void BuildShelterGreybox()
        {
            shelterRoot = new GameObject("WalkableShelterGreybox");
            CreateBlock("CutawayShelterFrame", shelterRoot.transform, new Vector2(0f, 0.15f), new Vector2(12.2f, 7.2f), new Color(0.11f, 0.12f, 0.12f), false);
            CreateBlock("BackWall_Ground", shelterRoot.transform, new Vector2(0f, -1.75f), new Vector2(11.4f, 2.8f), new Color(0.18f, 0.19f, 0.18f), false);
            CreateBlock("BackWall_Upper", shelterRoot.transform, new Vector2(0f, 1.95f), new Vector2(11.4f, 2.8f), new Color(0.17f, 0.18f, 0.18f), false);
            CreateBlock("RoofLine", shelterRoot.transform, new Vector2(0f, 3.55f), new Vector2(11.8f, 0.28f), new Color(0.40f, 0.39f, 0.35f), true);
            CreateBlock("LeftOuterWall", shelterRoot.transform, new Vector2(-5.85f, 0f), new Vector2(0.28f, 7.1f), new Color(0.40f, 0.39f, 0.35f), true);
            CreateBlock("RightOuterWall", shelterRoot.transform, new Vector2(5.85f, 0f), new Vector2(0.28f, 7.1f), new Color(0.40f, 0.39f, 0.35f), true);
            CreateBlock("CenterDivider_Ground", shelterRoot.transform, new Vector2(-1.85f, -1.6f), new Vector2(0.16f, 2.2f), new Color(0.30f, 0.30f, 0.28f), true);
            CreateBlock("CenterDivider_Upper", shelterRoot.transform, new Vector2(1.7f, 1.9f), new Vector2(0.16f, 2.2f), new Color(0.30f, 0.30f, 0.28f), true);
            CreateBlock("ShelterFloor_Ground", shelterRoot.transform, new Vector2(0f, -3.05f), new Vector2(11.6f, 0.34f), new Color(0.46f, 0.43f, 0.36f), true);
            CreateBlock("ShelterFloor_Upper", shelterRoot.transform, new Vector2(0f, 0.45f), new Vector2(11.3f, 0.28f), new Color(0.42f, 0.40f, 0.34f), true);
            CreateBlock("RoomHint_LeftGround", shelterRoot.transform, new Vector2(-3.95f, -1.75f), new Vector2(2.9f, 2.25f), new Color(0.20f, 0.21f, 0.19f), false);
            CreateBlock("RoomHint_RightGround", shelterRoot.transform, new Vector2(2.55f, -1.75f), new Vector2(5.7f, 2.25f), new Color(0.19f, 0.20f, 0.19f), false);
            CreateBlock("RoomHint_LeftUpper", shelterRoot.transform, new Vector2(-2.6f, 1.95f), new Vector2(6.2f, 2.25f), new Color(0.19f, 0.20f, 0.20f), false);
            CreateBlock("RoomHint_RightUpper", shelterRoot.transform, new Vector2(3.65f, 1.95f), new Vector2(3.3f, 2.25f), new Color(0.20f, 0.19f, 0.18f), false);

            CreateFacility(shelterRoot.transform, "bed", new Vector2(-3.75f, 0.95f), new Color(0.35f, 0.45f, 0.75f));
            CreateFacility(shelterRoot.transform, "workbench", new Vector2(-0.35f, -2.2f), new Color(0.62f, 0.45f, 0.27f));
            CreateFacility(shelterRoot.transform, "stove", new Vector2(3.55f, -2.2f), new Color(0.65f, 0.25f, 0.18f));
            CreateFacility(shelterRoot.transform, "barricade", new Vector2(-5.05f, -1.55f), new Color(0.42f, 0.42f, 0.38f));
            CreateFacility(shelterRoot.transform, "radio", new Vector2(3.6f, 1.15f), new Color(0.25f, 0.55f, 0.55f));
            CreateFacility(shelterRoot.transform, "storage", new Vector2(-2.95f, -2.2f), new Color(0.50f, 0.40f, 0.25f));
            BuildStairs(shelterRoot.transform);

            var playerObject = CreateBlock("LinXing_Player", shelterRoot.transform, new Vector2(-4.55f, -2.45f), new Vector2(0.48f, 0.72f), new Color(0.82f, 0.82f, 0.72f), true);
            ApplyLinXingSprite(playerObject);
            playerObject.AddComponent<Rigidbody2D>();
            playerObject.AddComponent<CircleCollider2D>();
            ConfigurePlayerForShelter(playerObject);
            playerTransform = playerObject.transform;
        }

        private GameObject CreateBlock(string name, Transform parent, Vector2 position, Vector2 scale, Color color, bool collider)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.position = new Vector3(position.x, position.y, 0f);
            obj.transform.localScale = new Vector3(scale.x, scale.y, 1f);

            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateOnePixelSprite();
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = GetSortingOrderFor(name);

            if (collider)
                obj.AddComponent<BoxCollider2D>();

            return obj;
        }

        private void CreateFacility(Transform parent, string facilityId, Vector2 position, Color color)
        {
            var definition = ShelterInteractionCatalog.Get(facilityId);
            string objectName = definition == null ? facilityId : definition.DisplayName;
            var obj = CreateBlock("Facility_" + facilityId, parent, position, new Vector2(1.25f, 0.9f), color, true);
            obj.GetComponent<BoxCollider2D>().isTrigger = true;

            var label = CreateWorldLabel(obj.transform, "Label_" + facilityId, objectName, new Vector3(0f, 0.8f, 0f));
            var stateVisual = CreateFacilityStateVisual(obj.transform, facilityId);
            var blueprintVisual = CreateFacilityLayer(obj.transform, "Blueprint_" + facilityId, new Vector2(0.86f, 0.58f),
                new Color(0.32f, 0.58f, 0.95f, 0.36f), 9);
            var builtVisual = CreateFacilityLayer(obj.transform, "Built_" + facilityId, new Vector2(0.80f, 0.54f),
                new Color(0.68f, 0.72f, 0.62f, 0.88f), 10);
            var usedMarker = CreateFacilityLayer(obj.transform, "UsedMarker_" + facilityId, new Vector2(0.30f, 0.10f),
                new Color(0.90f, 0.86f, 0.36f, 0.95f), 12);
            usedMarker.transform.localPosition = new Vector3(0.34f, -0.30f, -0.12f);

            GameObject damageMarker = null;
            if (facilityId == "barricade")
            {
                damageMarker = CreateFacilityLayer(obj.transform, "DamageMarker_" + facilityId, new Vector2(0.16f, 0.72f),
                    new Color(0.92f, 0.18f, 0.16f, 0.95f), 13);
                damageMarker.transform.localPosition = new Vector3(-0.18f, 0.02f, -0.13f);
            }

            var feedbackText = CreateWorldLabel(obj.transform, "Feedback_" + facilityId, string.Empty, new Vector3(0f, -0.86f, 0f));
            feedbackText.fontSize = 0.58f;
            feedbackText.fontStyle = FontStyles.Normal;
            feedbackText.color = new Color(0.98f, 0.92f, 0.68f);
            feedbackText.alignment = TextAlignmentOptions.Top;
            feedbackText.rectTransform.sizeDelta = new Vector2(3.2f, 1.10f);
            feedbackText.gameObject.SetActive(false);

            var interactable = obj.AddComponent<ShelterInteractable>();
            interactable.Configure(facilityId, label, stateVisual, blueprintVisual, builtVisual, usedMarker, damageMarker, feedbackText);
            shelterInteractables.Add(interactable);
        }

        private GameObject CreateFacilityLayer(Transform parent, string name, Vector2 scale, Color color, int sortingOrder)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = new Vector3(0f, 0f, -0.08f);
            obj.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateOnePixelSprite();
            renderer.sortingOrder = sortingOrder;
            renderer.color = color;
            return obj;
        }

        private SpriteRenderer CreateFacilityStateVisual(Transform parent, string facilityId)
        {
            var obj = new GameObject("State_" + facilityId);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = new Vector3(0f, 0f, -0.05f);
            obj.transform.localScale = new Vector3(0.78f, 0.52f, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateOnePixelSprite();
            renderer.sortingOrder = 8;
            renderer.color = new Color(0.58f, 0.64f, 0.58f, 0.92f);
            return renderer;
        }

        private void BuildStairs(Transform parent)
        {
            var stairs = CreateBlock("Stairs_GroundToUpper", parent, new Vector2(1.55f, -1.28f), new Vector2(1.15f, 3.05f), new Color(0.54f, 0.47f, 0.34f, 0.75f), true);
            stairs.GetComponent<BoxCollider2D>().isTrigger = true;
            CreateWorldLabel(stairs.transform, "Label_Stairs", "楼梯", new Vector3(0f, 1.6f, 0f));

            var groundPoint = new GameObject("StairsPoint_Ground");
            groundPoint.transform.SetParent(parent);
            groundPoint.transform.position = new Vector3(1.55f, -2.45f, 0f);

            var upperPoint = new GameObject("StairsPoint_Upper");
            upperPoint.transform.SetParent(parent);
            upperPoint.transform.position = new Vector3(1.55f, 0.95f, 0f);

            stairs.AddComponent<ShelterStairZone>().Configure(groundPoint.transform, upperPoint.transform);
        }

        private TMP_Text CreateWorldLabel(Transform parent, string name, string text, Vector3 localPosition)
        {
            var labelObject = new GameObject(name);
            labelObject.transform.SetParent(parent);
            labelObject.transform.localPosition = localPosition;
            var label = labelObject.AddComponent<TextMeshPro>();
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 1.12f;
            label.color = new Color(0.94f, 0.96f, 0.90f);
            label.text = text;
            label.rectTransform.sizeDelta = new Vector2(2.6f, 0.9f);
            label.fontStyle = FontStyles.Bold;
            return label;
        }

        private void BuildHud()
        {
            EnsureEventSystem();

            var canvasObject = new GameObject("OneRunHUD");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            Transform safeFrame = CreateHudContainer(canvasObject.transform, "ReadabilitySafeFrame",
                Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            headerText = CreateHudText(safeFrame, "Header", new Vector2(0f, -14f), new Vector2(760f, 42f), 24,
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            Transform statusPanel = CreateHudPanel(canvasObject.transform, "StatusPanel", new Vector2(16f, -62f), new Vector2(300f, 118f),
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Color(0.035f, 0.045f, 0.045f, 0.84f));
            statusText = CreateHudText(statusPanel, "Status", new Vector2(12f, -10f), new Vector2(276f, 96f), 18,
                TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            Transform promptPanel = CreateHudPanel(canvasObject.transform, "PromptPanel", new Vector2(0f, 8f), new Vector2(690f, 34f),
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Color(0.035f, 0.045f, 0.045f, 0.86f));
            promptText = CreateHudText(promptPanel, "Prompt", Vector2.zero, new Vector2(660f, 28f), 18,
                TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));

            Transform logPanel = CreateHudPanel(canvasObject.transform, "LogPanel", new Vector2(-16f, -62f), new Vector2(360f, 118f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Color(0.035f, 0.045f, 0.045f, 0.78f));
            logText = CreateHudText(logPanel, "Log", new Vector2(-12f, -10f), new Vector2(336f, 96f), 16,
                TextAlignmentOptions.TopRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            exploreClinicButton = CreateHudButton(canvasObject.transform, "ExploreClinic", "去诊所", new Vector2(-276f, 84f), () => EnterScavengeLocation("clinic"));
            exploreSupermarketButton = CreateHudButton(canvasObject.transform, "ExploreSupermarket", "去超市", new Vector2(-138f, 84f), () => EnterScavengeLocation("supermarket"));
            exploreGarageButton = CreateHudButton(canvasObject.transform, "ExploreGarage", "去车库", new Vector2(0f, 84f), () => EnterScavengeLocation("bike_shop"));
            returnShelterButton = CreateHudButton(canvasObject.transform, "ReturnShelter", "返回据点", new Vector2(138f, 84f), ReturnToShelter);
            leaveHelpMarkButton = CreateHudButton(canvasObject.transform, "LeaveHelpMark", "留下求助", new Vector2(276f, 84f), LeaveHelpMarkAtActiveLocation);
            resolveNightButton = CreateHudButton(canvasObject.transform, "ResolveNight", "夜晚结算", new Vector2(-210f, 42f), ResolveNight);
            nextDayButton = CreateHudButton(canvasObject.transform, "NextDay", "下一天", new Vector2(-70f, 42f), NextDay);
            CreateHudButton(canvasObject.transform, "DossierButton", "档案", new Vector2(70f, 42f), ToggleDossierPanel);
            CreateHudButton(canvasObject.transform, "QimianLogButton", "日志", new Vector2(210f, 42f), ToggleQimianLogPanel);
            BuildDossierPanel(canvasObject.transform);
            BuildQimianLogPanel(canvasObject.transform);
            BuildObjectivePanel(canvasObject.transform);
            BuildLocationCards(canvasObject.transform);
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private Transform CreateHudContainer(Transform parent, string name, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 pivot, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            return obj.transform;
        }

        private Transform CreateHudPanel(Transform parent, string name, Vector2 anchoredPosition, Vector2 size,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            var image = obj.AddComponent<Image>();
            image.color = color;
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            return obj.transform;
        }

        private TMP_Text CreateHudText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize,
            TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Ellipsis;
            var rect = text.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            return text;
        }

        private Button CreateHudButton(Transform parent, string name, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.18f, 0.23f, 0.28f, 0.92f);
            var button = obj.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(118f, 36f);

            var textObject = new GameObject("Text");
            textObject.transform.SetParent(obj.transform);
            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 16;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Ellipsis;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        private void BuildDossierPanel(Transform parent)
        {
            dossierPanel = new GameObject("DossierPanel");
            dossierPanel.transform.SetParent(parent);
            var image = dossierPanel.AddComponent<Image>();
            image.color = new Color(0.04f, 0.06f, 0.07f, 0.92f);

            var rect = dossierPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(230f, 10f);
            rect.sizeDelta = new Vector2(520f, 360f);

            CreateHudText(dossierPanel.transform, "DossierTitle", new Vector2(18f, -16f), new Vector2(430f, 34f), 20,
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f)).text = "未知行动者档案";

            dossierBodyText = CreateHudText(dossierPanel.transform, "DossierBody", new Vector2(18f, -58f), new Vector2(484f, 266f), 15,
                TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            dossierBodyText.overflowMode = TextOverflowModes.Overflow;

            CreateHudButton(dossierPanel.transform, "CloseDossier", "关闭", new Vector2(0f, 14f), CloseDossierPanel);
            dossierPanel.SetActive(false);
        }

        private void BuildQimianLogPanel(Transform parent)
        {
            qimianLogPanel = new GameObject("QimianLogPanel");
            qimianLogPanel.transform.SetParent(parent);
            var image = qimianLogPanel.AddComponent<Image>();
            image.color = new Color(0.05f, 0.045f, 0.04f, 0.93f);

            var rect = qimianLogPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-230f, 10f);
            rect.sizeDelta = new Vector2(520f, 360f);

            CreateHudText(qimianLogPanel.transform, "QimianLogTitle", new Vector2(18f, -16f), new Vector2(430f, 34f), 20,
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f)).text = "祁眠行动日志";

            qimianLogBodyText = CreateHudText(qimianLogPanel.transform, "QimianLogBody", new Vector2(18f, -58f), new Vector2(484f, 266f), 15,
                TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            qimianLogBodyText.overflowMode = TextOverflowModes.Overflow;

            CreateHudButton(qimianLogPanel.transform, "CloseQimianLog", "关闭", new Vector2(0f, 14f), CloseQimianLogPanel);
            qimianLogPanel.SetActive(false);
        }

        private void BuildLocationCards(Transform parent)
        {
            var panel = new GameObject("LocationCardPanel");
            panel.transform.SetParent(parent);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 130f);
            rect.sizeDelta = new Vector2(660f, 68f);

            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0.035f, 0.045f, 0.045f, 0.82f);

            string[] locationIds = { "clinic", "supermarket", "bike_shop" };
            float[] cardX = { -218f, 0f, 218f };

            for (int i = 0; i < locationIds.Length; i++)
            {
                string locationId = locationIds[i];
                Transform card = CreateHudPanel(panel.transform, "LocationCard_" + locationId,
                    new Vector2(cardX[i], -6f), new Vector2(198f, 56f),
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                    new Color(0.06f, 0.07f, 0.08f, 0.85f));

                var nameText = CreateHudText(card, "LocationName_" + locationId,
                    new Vector2(0f, -6f), new Vector2(180f, 20f), 15,
                    TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
                nameText.color = new Color(0.88f, 0.86f, 0.78f);
                locationCardNameTexts[locationId] = nameText;

                var infoText = CreateHudText(card, "LocationInfo_" + locationId,
                    new Vector2(0f, -26f), new Vector2(180f, 26f), 13,
                    TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
                infoText.color = new Color(0.72f, 0.76f, 0.80f);
                locationCardInfoTexts[locationId] = infoText;
            }
        }

        private void BuildObjectivePanel(Transform parent)
        {
            var panel = new GameObject("ObjectivePanel");
            panel.transform.SetParent(parent);

            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -78f);
            rect.sizeDelta = new Vector2(680f, 58f);

            var image = panel.AddComponent<Image>();
            image.color = new Color(0.04f, 0.05f, 0.06f, 0.88f);

            objectiveTitleText = CreateHudText(panel.transform, "ObjectiveTitle", new Vector2(14f, -8f), new Vector2(640f, 22f), 15,
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            objectiveTitleText.text = "当前目标";
            objectiveTitleText.color = new Color(0.72f, 0.80f, 0.92f);

            objectiveBodyText = CreateHudText(panel.transform, "ObjectiveBody", new Vector2(14f, -32f), new Vector2(640f, 28f), 16,
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            objectiveBodyText.color = new Color(0.92f, 0.90f, 0.82f);
        }

    private void EnterScavengeLocation(string locationId)
    {
        if (State == null || State.DemoComplete || State.Phase == "searching")
        {
            string reason = State == null ? string.Empty
                : State.DemoComplete ? "演示已完成，无法再外出。"
                : State.Phase == "searching" ? "正在搜刮中，无法再次外出。先返回据点。"
                : string.Empty;
            if (!string.IsNullOrEmpty(reason)) Report(reason);
            return;
        }

        string result = GameSimulation.EnterLocation(State, locationId);
        if (State.Phase != "searching")
        {
            Report(result);
            return;
        }

        BuildScavengeGreybox(locationId);
        Report(result);
        ShowSearchPrompt(string.Empty);
    }

        private void BuildScavengeGreybox(string locationId)
        {
            if (scavengeRoot != null)
                Destroy(scavengeRoot);

            searchPoints.Clear();
            scavengeRoot = new GameObject("ScavengeGreybox_" + locationId);
            shelterRoot.SetActive(false);
            scavengeRoot.SetActive(true);

            var site = ExplorationSiteCatalog.GetSite(locationId);
            string siteName = site == null ? locationId : site.DisplayName;
            CreateBlock("ScavengeFloor", scavengeRoot.transform, Vector2.zero, new Vector2(11f, 7f), new Color(0.14f, 0.16f, 0.18f), false);
            CreateBlock("ScavengeNorthWall", scavengeRoot.transform, new Vector2(0f, 3.65f), new Vector2(11.5f, 0.35f), new Color(0.28f, 0.31f, 0.34f), true);
            CreateBlock("ScavengeSouthWall", scavengeRoot.transform, new Vector2(0f, -3.65f), new Vector2(11.5f, 0.35f), new Color(0.28f, 0.31f, 0.34f), true);
            CreateBlock("ScavengeWestWall", scavengeRoot.transform, new Vector2(-5.65f, 0f), new Vector2(0.35f, 7.2f), new Color(0.28f, 0.31f, 0.34f), true);
            CreateBlock("ScavengeEastWall", scavengeRoot.transform, new Vector2(5.65f, 0f), new Vector2(0.35f, 7.2f), new Color(0.28f, 0.31f, 0.34f), true);
            CreateWorldLabel(scavengeRoot.transform, "ScavengeTitle", siteName, new Vector3(0f, 3.05f, 0f));

            Vector2[] positions = { new(-3.4f, 1.2f), new(0f, -0.6f), new(3.3f, 1.1f), new(-2.6f, -2f), new(2.5f, -2f) };
            var rooms = ExplorationSiteCatalog.GetRoomsForLocation(locationId);
            for (int i = 0; i < rooms.Count; i++)
                CreateSearchPoint(scavengeRoot.transform, rooms[i], positions[i % positions.Length]);

            if (playerTransform != null)
            {
                playerTransform.SetParent(scavengeRoot.transform);
                playerTransform.position = new Vector3(0f, -2.7f, 0f);
                ConfigurePlayerForScavenge(playerTransform.gameObject);
            }
        }

        private void CreateSearchPoint(Transform parent, SearchPointDefinition definition, Vector2 position)
        {
            var obj = CreateBlock("SearchPoint_" + definition.RoomId, parent, position, new Vector2(1.25f, 0.85f), new Color(0.30f, 0.38f, 0.48f), true);
            obj.GetComponent<BoxCollider2D>().isTrigger = true;
            var label = CreateWorldLabel(obj.transform, "Label_" + definition.RoomId, definition.DisplayName, new Vector3(0f, 0.8f, 0f));
            var point = obj.AddComponent<ScavengeSearchPoint>();
            point.Configure(definition, label);
            point.Refresh(State);
            searchPoints.Add(point);
        }

    private void ReturnToShelter()
    {
        if (State == null || State.Phase != "searching")
        {
            if (State != null && State.Phase != "searching")
                Report("当前不在搜刮中，无需返回据点。");
            return;
        }

        string result = GameSimulation.LeaveExploration(State);
            if (playerTransform != null)
            {
                playerTransform.SetParent(shelterRoot.transform);
                playerTransform.position = new Vector3(-4.55f, -2.45f, 0f);
                ConfigurePlayerForShelter(playerTransform.gameObject);
            }

            shelterRoot.SetActive(true);
            if (scavengeRoot != null)
                Destroy(scavengeRoot);
            scavengeRoot = null;
            searchPoints.Clear();
            ShowPrompt(string.Empty);
            Report(result);
        }

    public void LeaveHelpMarkAtActiveLocation()
    {
        if (State == null || State.Phase != "searching")
        {
            if (State != null && State.Phase != "searching")
                Report("只有在搜刮中才能留下求助标记。");
            return;
        }

            string locationId = State.Exploration.ActiveLocation;
            if (string.IsNullOrEmpty(locationId)) return;

            GameSimulation.AddPlayerMark(State, locationId, "help", "这里需要药，也可能有人会看懂这个记号。");
            Report($"林行在{GetLocationName(locationId)}留下求助标记。");
        }

        public void ToggleDossierPanel()
        {
            if (dossierPanel == null) return;

            bool shouldShow = !dossierPanel.activeSelf;
            if (shouldShow)
            {
                RefreshDossierPanel();
                if (qimianLogPanel != null)
                    qimianLogPanel.SetActive(false);
            }
            dossierPanel.SetActive(shouldShow);
        }

        public void RefreshDossierPanel()
        {
            if (dossierBodyText == null || State == null) return;

            dossierBodyText.text = GameSimulation.GetAnomalyDossierText(State);
        }

        private void CloseDossierPanel()
        {
            if (dossierPanel == null) return;
            dossierPanel.SetActive(false);
        }

        public void ToggleQimianLogPanel()
        {
            if (qimianLogPanel == null) return;

            bool shouldShow = !qimianLogPanel.activeSelf;
            if (shouldShow)
            {
                RefreshQimianLogPanel();
                if (dossierPanel != null)
                    dossierPanel.SetActive(false);
            }
            qimianLogPanel.SetActive(shouldShow);
        }

        public void RefreshQimianLogPanel()
        {
            if (qimianLogBodyText == null || State == null) return;

            qimianLogBodyText.text = State.Reveal.Unlocked
                ? GameSimulation.GetQimianEndingRevealText(State)
                : "通关后解锁祁眠行动日志。";
        }

        private void CloseQimianLogPanel()
        {
            if (qimianLogPanel == null) return;
            qimianLogPanel.SetActive(false);
        }

        private string GetPhaseLabel()
        {
            if (State == null) return string.Empty;

            return State.Phase switch
            {
                "morning" => "清晨",
                "day" => "白天",
                "searching" => "搜刮中",
                "evening" => "黄昏",
                "night" => "夜晚",
                "reveal" => "结尾揭示",
                _ => State.Phase
            };
        }

        private bool HasClinicHelpMark()
        {
            if (State == null || State.PlayerMarks == null) return false;
            return State.PlayerMarks.ContainsKey("clinic") && State.PlayerMarks["clinic"].Type == "help";
        }

        private string GetObjectiveText()
        {
            if (State == null) return string.Empty;

            if (State.DemoComplete || (State.Reveal != null && State.Reveal.Unlocked))
                return "祁眠行动日志已解锁。打开「日志」查看隐藏行动链。";

            if (State.Phase == "searching")
                return "搜索房间，必要时留下求助标记；完成后返回据点。";

            if (State.Day >= 5 && State.AnomalyDossier != null && State.AnomalyDossier.Count == 0)
                return "未知行动者开始留下痕迹。去诊所寻找异常记录。";

            if (State.Day >= 5 && !HasClinicHelpMark())
                return "诊所异常已出现。进入诊所后留下求助标记，观察夜晚变化。";

            if (HasClinicHelpMark() && !State.DemoComplete && (State.Reveal == null || !State.Reveal.Unlocked))
                return "求助标记已经留下。推进夜晚结算，第二天查看档案反馈。";

            if (State.Day < 5 && State.Phase != "searching")
                return "白天外出搜刮，夜晚修整据点。先熟悉诊所、超市和车库。";

            return "白天外出搜刮，夜晚修整据点。先熟悉诊所、超市和车库。";
        }

        private void RefreshObjectivePanel()
        {
            if (objectiveTitleText != null)
                objectiveTitleText.text = "当前目标";
            if (objectiveBodyText != null)
                objectiveBodyText.text = GetObjectiveText();
        }

        private string GetLocationCardInfo(string locationId)
        {
            if (State == null || !State.Locations.TryGetValue(locationId, out var location))
                return string.Empty;

            string resourceTendency = string.IsNullOrEmpty(location.ResourceTendency) ? "资源" : location.ResourceTendency;
            string danger = string.IsNullOrEmpty(location.DangerLevel) ? "未知" : location.DangerLevel;
            string anomaly;
            if (location.QimianTrace)
                anomaly = "有痕迹";
            else if (locationId == "clinic")
                anomaly = "待调查";
            else
                anomaly = "暂无";
            string info = $"资源：{resourceTendency} / 危险：{danger} / 异常：{anomaly}";

            if (location.QimianTrace)
                info += " / 有新痕迹";

            if (State.PlayerMarks != null && State.PlayerMarks.TryGetValue(locationId, out _))
                info += " / 已留标记";

            int totalResources = 0;
            if (location.Resources != null)
                foreach (var kv in location.Resources)
                    totalResources += kv.Value;

            if (totalResources <= 2 && location.Visited)
                info += " / 资源减少";

            return info;
        }

        private void RefreshLocationCards()
        {
            foreach (var kv in locationCardNameTexts)
            {
                string locationId = kv.Key;
                string displayName = GetLocationName(locationId);
                kv.Value.text = displayName;
            }

            foreach (var kv in locationCardInfoTexts)
            {
                kv.Value.text = GetLocationCardInfo(kv.Key);
            }
        }

        private string GetLocationName(string locationId)
        {
            if (State != null && State.Locations.TryGetValue(locationId, out var location))
                return location.Name;
            return locationId;
        }

        private static void SetButtonVisual(Button button, bool available)
        {
            if (button == null) return;

            button.interactable = true;
            var image = button.GetComponent<Image>();
            if (image != null)
                image.color = available
                    ? new Color(0.18f, 0.23f, 0.28f, 0.92f)
                    : new Color(0.10f, 0.13f, 0.16f, 0.50f);
        }

        private string GetDefaultPromptText()
        {
            if (State == null) return string.Empty;

            if (State.DemoComplete)
                return "演示已完成，所有行动按钮已锁定。";

            if (State.Phase == "searching")
                return "正在搜刮中——先返回据点再使用其他按钮。";

            return "靠近设施按 E 互动";
        }

    private void ResolveNight()
    {
        if (State == null || State.DemoComplete)
        {
            if (State != null && State.DemoComplete)
                Report("演示已完成，无法再进行夜晚结算。");
            return;
        }
        if (State.Phase == "searching")
            ReturnToShelter();
        EnsureShelterActionPhase();
        Report(GameSimulation.SleepAndResolveNight(State));
    }

    private void NextDay()
    {
        if (State == null || State.DemoComplete)
        {
            if (State != null && State.DemoComplete)
                Report("演示已完成，无法再推进天数。");
            return;
        }
        GameSimulation.StartDay(State, Mathf.Min(State.Day + 1, 15));
        Report(State.LastEvent);
    }

        private void RefreshAll()
        {
            if (State == null) return;
            if (headerText != null)
                headerText.text = $"《保护区之外》一周目林行篇  第 {State.Day} 天  {GetPhaseLabel()}";
            if (statusText != null)
            {
                statusText.text =
                    $"生命 {State.Lin.Health}  疲劳 {State.Lin.Fatigue}  压力 {State.Lin.Stress}  感染 {State.Lin.InfectionRisk}\n" +
                    $"食物 {State.Resources.Food}  水 {State.Resources.Water}  药 {State.Resources.Meds}\n" +
                    $"建材 {State.Resources.Materials}  零件 {State.Resources.Parts}  燃料 {State.Resources.Fuel}\n" +
                    $"墙体 {State.Shelter.Door}  防御 {State.Shelter.Defense}  汽车 {State.Evacuation.CarReady}";
            }
            if (promptText != null && string.IsNullOrEmpty(promptText.text))
                promptText.text = GetDefaultPromptText();
            if (logText != null)
                logText.text = string.Join("\n", logLines);
            RefreshObjectivePanel();
            RefreshLocationCards();
            if (dossierPanel != null && dossierPanel.activeSelf)
                RefreshDossierPanel();
            if (qimianLogPanel != null && qimianLogPanel.activeSelf)
                RefreshQimianLogPanel();
            // 更新所有行动按钮视觉状态（始终可点击，不可用时视觉弱化）
            bool canScavenge = !State.DemoComplete && State.Phase != "searching";
            SetButtonVisual(exploreClinicButton, canScavenge);
            SetButtonVisual(exploreSupermarketButton, canScavenge);
            SetButtonVisual(exploreGarageButton, canScavenge);
            SetButtonVisual(returnShelterButton, State.Phase == "searching");
            SetButtonVisual(leaveHelpMarkButton, State.Phase == "searching");
            SetButtonVisual(resolveNightButton, !State.DemoComplete);
            SetButtonVisual(nextDayButton, !State.DemoComplete);

            foreach (var interactable in shelterInteractables)
                interactable.Refresh(State);
            foreach (var point in searchPoints)
                point.Refresh(State);
        }

        private Sprite CreateOnePixelSprite()
        {
            if (sharedOnePixelSprite != null)
                return sharedOnePixelSprite;

            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            sharedOnePixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return sharedOnePixelSprite;
        }

        private void ApplyLinXingSprite(GameObject playerObject)
        {
            if (playerObject == null) return;

            var renderer = playerObject.GetComponent<SpriteRenderer>();
            if (renderer == null) return;

            var sprite = Resources.Load<Sprite>(LinXingSpriteResourcePath);
            if (sprite == null) return;

            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingOrder = 20;
        }

        private int GetSortingOrderFor(string objectName)
        {
            if (objectName == "LinXing_Player")
                return 20;
            if (objectName.StartsWith("Label_"))
                return 30;
            if (objectName.StartsWith("Facility_") || objectName.StartsWith("SearchPoint_"))
                return 6;
            if (objectName.StartsWith("State_"))
                return 8;
            if (objectName.StartsWith("ShelterFloor") || objectName.Contains("Wall") || objectName == "RoofLine")
                return 4;
            if (objectName.StartsWith("RoomHint") || objectName.Contains("BackWall") || objectName.Contains("Floor"))
                return 1;
            return 0;
        }

        private void ConfigurePlayerForShelter(GameObject playerObject)
        {
            if (playerObject == null) return;

            var topDown = playerObject.GetComponent<TopDownPlayerController>();
            if (topDown != null)
                Destroy(topDown);

            var sideView = playerObject.GetComponent<SideViewShelterPlayerController>();
            if (sideView == null)
                sideView = playerObject.AddComponent<SideViewShelterPlayerController>();
            sideView.Configure(this);
        }

        private void ConfigurePlayerForScavenge(GameObject playerObject)
        {
            if (playerObject == null) return;

            var sideView = playerObject.GetComponent<SideViewShelterPlayerController>();
            if (sideView != null)
                Destroy(sideView);

            var topDown = playerObject.GetComponent<TopDownPlayerController>();
            if (topDown == null)
                topDown = playerObject.AddComponent<TopDownPlayerController>();
            topDown.Configure(this);
        }
    }
}
