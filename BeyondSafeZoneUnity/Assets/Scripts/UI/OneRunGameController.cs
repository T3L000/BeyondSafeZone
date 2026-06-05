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
        private readonly List<ShelterInteractable> shelterInteractables = new();
        private readonly List<ScavengeSearchPoint> searchPoints = new();
        private readonly List<string> logLines = new();

        private GameObject shelterRoot;
        private GameObject scavengeRoot;
        private Transform playerTransform;

        private TMP_Text headerText;
        private TMP_Text statusText;
        private TMP_Text promptText;
        private TMP_Text logText;
        private Button returnShelterButton;
        private Button leaveHelpMarkButton;

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
            if (string.IsNullOrEmpty(facilityId))
            {
                promptText.text = State.Phase == "searching" ? "靠近搜索点按 E 搜索" : "靠近设施按 E 互动";
                return;
            }

            promptText.text = ShelterInteractionCatalog.GetPrompt(State, facilityId) + "  按 E";
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
            cam.orthographicSize = 6f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = new Color(0.07f, 0.08f, 0.08f);
        }

        private void BuildShelterGreybox()
        {
            shelterRoot = new GameObject("WalkableShelterGreybox");
            CreateBlock("Floor", shelterRoot.transform, new Vector2(0f, 0f), new Vector2(11f, 7f), new Color(0.18f, 0.19f, 0.18f), false);
            CreateBlock("NorthWall", shelterRoot.transform, new Vector2(0f, 3.65f), new Vector2(11.5f, 0.35f), new Color(0.36f, 0.36f, 0.34f), true);
            CreateBlock("SouthWall", shelterRoot.transform, new Vector2(0f, -3.65f), new Vector2(11.5f, 0.35f), new Color(0.36f, 0.36f, 0.34f), true);
            CreateBlock("WestWall", shelterRoot.transform, new Vector2(-5.65f, 0f), new Vector2(0.35f, 7.2f), new Color(0.36f, 0.36f, 0.34f), true);
            CreateBlock("EastWall", shelterRoot.transform, new Vector2(5.65f, 0f), new Vector2(0.35f, 7.2f), new Color(0.36f, 0.36f, 0.34f), true);

            CreateFacility(shelterRoot.transform, "bed", new Vector2(-3.6f, 1.8f), new Color(0.35f, 0.45f, 0.75f));
            CreateFacility(shelterRoot.transform, "workbench", new Vector2(0f, 2.0f), new Color(0.62f, 0.45f, 0.27f));
            CreateFacility(shelterRoot.transform, "stove", new Vector2(3.6f, 1.8f), new Color(0.65f, 0.25f, 0.18f));
            CreateFacility(shelterRoot.transform, "barricade", new Vector2(-4.7f, -1.2f), new Color(0.42f, 0.42f, 0.38f));
            CreateFacility(shelterRoot.transform, "radio", new Vector2(0f, -2.2f), new Color(0.25f, 0.55f, 0.55f));
            CreateFacility(shelterRoot.transform, "storage", new Vector2(3.8f, -1.5f), new Color(0.50f, 0.40f, 0.25f));

            var playerObject = CreateBlock("LinXing_Player", shelterRoot.transform, Vector2.zero, new Vector2(0.55f, 0.55f), new Color(0.82f, 0.82f, 0.72f), true);
            playerObject.AddComponent<Rigidbody2D>();
            playerObject.AddComponent<CircleCollider2D>();
            playerObject.AddComponent<TopDownPlayerController>().Configure(this);
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
            var interactable = obj.AddComponent<ShelterInteractable>();
            interactable.Configure(facilityId, label);
            shelterInteractables.Add(interactable);
        }

        private TMP_Text CreateWorldLabel(Transform parent, string name, string text, Vector3 localPosition)
        {
            var labelObject = new GameObject(name);
            labelObject.transform.SetParent(parent);
            labelObject.transform.localPosition = localPosition;
            var label = labelObject.AddComponent<TextMeshPro>();
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 0.75f;
            label.color = Color.white;
            label.text = text;
            label.rectTransform.sizeDelta = new Vector2(3.2f, 0.7f);
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

            headerText = CreateHudText(canvasObject.transform, "Header", new Vector2(0f, -18f), new Vector2(900f, 44f), 22,
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
            statusText = CreateHudText(canvasObject.transform, "Status", new Vector2(18f, -72f), new Vector2(360f, 132f), 15,
                TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            promptText = CreateHudText(canvasObject.transform, "Prompt", new Vector2(0f, 4f), new Vector2(780f, 24f), 14,
                TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            logText = CreateHudText(canvasObject.transform, "Log", new Vector2(-18f, -72f), new Vector2(390f, 150f), 14,
                TextAlignmentOptions.TopRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            CreateHudButton(canvasObject.transform, "ExploreClinic", "去诊所", new Vector2(-264f, 84f), () => EnterScavengeLocation("clinic"));
            CreateHudButton(canvasObject.transform, "ExploreSupermarket", "去超市", new Vector2(-132f, 84f), () => EnterScavengeLocation("supermarket"));
            CreateHudButton(canvasObject.transform, "ExploreGarage", "去车库", new Vector2(0f, 84f), () => EnterScavengeLocation("bike_shop"));
            returnShelterButton = CreateHudButton(canvasObject.transform, "ReturnShelter", "返回据点", new Vector2(132f, 84f), ReturnToShelter);
            leaveHelpMarkButton = CreateHudButton(canvasObject.transform, "LeaveHelpMark", "留下求助", new Vector2(264f, 84f), LeaveHelpMarkAtActiveLocation);
            CreateHudButton(canvasObject.transform, "ResolveNight", "夜晚结算", new Vector2(-66f, 40f), ResolveNight);
            CreateHudButton(canvasObject.transform, "NextDay", "下一天", new Vector2(66f, 40f), NextDay);
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
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

        private void EnterScavengeLocation(string locationId)
        {
            if (State == null || State.DemoComplete || State.Phase == "searching") return;

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
            if (State == null || State.Phase != "searching") return;

            string result = GameSimulation.LeaveExploration(State);
            if (playerTransform != null)
            {
                playerTransform.SetParent(shelterRoot.transform);
                playerTransform.position = Vector3.zero;
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
            if (State == null || State.Phase != "searching") return;

            string locationId = State.Exploration.ActiveLocation;
            if (string.IsNullOrEmpty(locationId)) return;

            GameSimulation.AddPlayerMark(State, locationId, "help", "这里需要药，也可能有人会看懂这个记号。");
            Report($"林行在{GetLocationName(locationId)}留下求助标记。");
        }

        private string GetLocationName(string locationId)
        {
            if (State != null && State.Locations.TryGetValue(locationId, out var location))
                return location.Name;
            return locationId;
        }

        private void ResolveNight()
        {
            if (State == null || State.DemoComplete) return;
            if (State.Phase == "searching")
                ReturnToShelter();
            EnsureShelterActionPhase();
            Report(GameSimulation.SleepAndResolveNight(State));
        }

        private void NextDay()
        {
            if (State == null || State.DemoComplete) return;
            GameSimulation.StartDay(State, Mathf.Min(State.Day + 1, 15));
            Report(State.LastEvent);
        }

        private void RefreshAll()
        {
            if (State == null) return;
            if (headerText != null)
                headerText.text = $"《保护区之外》一周目林行篇  Day {State.Day}  Phase {State.Phase}";
            if (statusText != null)
            {
                statusText.text =
                    $"生命 {State.Lin.Health}  疲劳 {State.Lin.Fatigue}  压力 {State.Lin.Stress}  感染 {State.Lin.InfectionRisk}\n" +
                    $"食物 {State.Resources.Food}  水 {State.Resources.Water}  药 {State.Resources.Meds}\n" +
                    $"建材 {State.Resources.Materials}  零件 {State.Resources.Parts}  燃料 {State.Resources.Fuel}\n" +
                    $"墙体 {State.Shelter.Door}  防御 {State.Shelter.Defense}  汽车 {State.Evacuation.CarReady}";
            }
            if (promptText != null && string.IsNullOrEmpty(promptText.text))
                promptText.text = State.Phase == "searching" ? "靠近搜索点按 E 搜索" : "靠近设施按 E 互动";
            if (logText != null)
                logText.text = string.Join("\n", logLines);
            if (returnShelterButton != null)
                returnShelterButton.interactable = State.Phase == "searching";
            if (leaveHelpMarkButton != null)
                leaveHelpMarkButton.interactable = State.Phase == "searching";

            foreach (var interactable in shelterInteractables)
                interactable.Refresh(State);
            foreach (var point in searchPoints)
                point.Refresh(State);
        }

        private Sprite CreateOnePixelSprite()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}
