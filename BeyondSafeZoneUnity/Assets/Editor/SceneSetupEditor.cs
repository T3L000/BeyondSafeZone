using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using BeyondSafeZone.UI;

namespace BeyondSafeZone.Editor
{
    public static class SceneSetupEditor
    {
        [MenuItem("BeyondSafeZone/Setup Main Scene")]
        public static void SetupMainScene()
        {
            // ---- Canvas ----
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // ---- EventSystem ----
            var esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // ---- GameRoot + MainUI ----
            var gameRoot = new GameObject("GameRoot");
            var mainUI = gameRoot.AddComponent<MainUI>();

            // ---- UI References creation helper ----
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // 1. statusText (top full width)
            mainUI.statusText = CreateText(canvasObj.transform, "StatusText",
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(-10, -60),
                "第 1 天  白天  目标：前往军区基地", font, 18, TextAnchor.UpperLeft);

            // 2. statsText (left upper)
            mainUI.statsText = CreateText(canvasObj.transform, "StatsText",
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -70), new Vector2(-10, -160),
                "林行状态", font, 14, TextAnchor.UpperLeft);

            // 3. resourcesText (left middle)
            mainUI.resourcesText = CreateText(canvasObj.transform, "ResourcesText",
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -170), new Vector2(-10, -210),
                "资源", font, 14, TextAnchor.UpperLeft);

            // 4. shelterInfoText (left lower)
            mainUI.shelterInfoText = CreateText(canvasObj.transform, "ShelterInfoText",
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -220), new Vector2(-10, -320),
                "据点信息", font, 14, TextAnchor.UpperLeft);

            // 5. locationCardText (right upper)
            mainUI.locationCardText = CreateText(canvasObj.transform, "LocationCardText",
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(-310, -10), new Vector2(-10, -120),
                "地点卡片", font, 14, TextAnchor.UpperLeft);

            // 6. roomCardText (right middle)
            mainUI.roomCardText = CreateText(canvasObj.transform, "RoomCardText",
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(-310, -130), new Vector2(-10, -240),
                "房间卡片", font, 14, TextAnchor.UpperLeft);

            // 7. eventLogText (right lower, scrollable area)
            mainUI.eventLogText = CreateText(canvasObj.transform, "EventLogText",
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(-310, -250), new Vector2(-10, -400),
                "事件日志", font, 12, TextAnchor.UpperLeft);

            // 8. mapPanel (center-left)
            mainUI.mapPanel = CreatePanel(canvasObj.transform, "MapPanel",
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(10, -50), new Vector2(300, 200),
                new Color(0.1f, 0.1f, 0.15f, 0.8f));

            // 9. explorerPanel (center-left, same area as map)
            mainUI.explorerPanel = CreatePanel(canvasObj.transform, "ExplorerPanel",
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(10, -50), new Vector2(300, 200),
                new Color(0.15f, 0.1f, 0.1f, 0.8f));
            mainUI.explorerPanel.SetActive(false);

            // 10. locationButtonsParent (inside mapPanel)
            var locBtnParent = new GameObject("LocationButtonsParent");
            locBtnParent.transform.SetParent(mainUI.mapPanel.transform);
            var locRect = locBtnParent.AddComponent<RectTransform>();
            locRect.anchorMin = Vector2.zero;
            locRect.anchorMax = Vector2.one;
            locRect.offsetMin = Vector2.zero;
            locRect.offsetMax = Vector2.zero;
            mainUI.locationButtonsParent = locBtnParent.transform;

            // 11. roomButtonsParent (inside explorerPanel)
            var roomBtnParent = new GameObject("RoomButtonsParent");
            roomBtnParent.transform.SetParent(mainUI.explorerPanel.transform);
            var roomRect = roomBtnParent.AddComponent<RectTransform>();
            roomRect.anchorMin = Vector2.zero;
            roomRect.anchorMax = Vector2.one;
            roomRect.offsetMin = Vector2.zero;
            roomRect.offsetMax = Vector2.zero;
            mainUI.roomButtonsParent = roomBtnParent.transform;

            // 12. actionPanel (bottom center)
            var actionPanelObj = CreatePanel(canvasObj.transform, "ActionPanel",
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-300, 10), new Vector2(300, 180),
                new Color(0.1f, 0.12f, 0.1f, 0.9f));
            mainUI.actionPanel = actionPanelObj.transform;

            // 13. sleepButton
            mainUI.sleepButton = CreateButton(canvasObj.transform, "SleepButton",
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(-110, 10), new Vector2(-10, 50),
                "😴 睡觉", font, () => mainUI.OnSleep());

            // 14. restartButton
            mainUI.restartButton = CreateButton(canvasObj.transform, "RestartButton",
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(-220, 10), new Vector2(-120, 50),
                "🔄 重新开始", font, () => mainUI.OnRestart());

            // 15. leaveExplorationButton
            mainUI.leaveExplorationButton = CreateButton(canvasObj.transform, "LeaveExplorationButton",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-80, -120), new Vector2(80, -80),
                "← 离开", font, () => mainUI.OnLeaveExploration());
            mainUI.leaveExplorationButton.gameObject.SetActive(false);

            // ---- Save scene ----
            string scenePath = "Assets/Scenes/Main.unity";
            if (!System.IO.Directory.Exists("Assets/Scenes"))
                System.IO.Directory.CreateDirectory("Assets/Scenes");

            var existingScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (existingScene != null)
                AssetDatabase.DeleteAsset(scenePath);

            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single);

            // Re-parent all created objects to the scene
            foreach (var obj in new[] { canvasObj, esObj, gameRoot })
                UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(obj, scene);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.Refresh();

            Debug.Log($"[BeyondSafeZone] Main scene created at {scenePath}");
            EditorUtility.DisplayDialog("Scene Setup", "Main.unity 已创建完成！\n请在 Hierarchy 中检查 GameRoot 的 MainUI 字段是否已绑定。", "OK");
        }

        private static Text CreateText(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
            string defaultText, Font font, int fontSize, TextAnchor alignment)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            var text = obj.AddComponent<Text>();
            text.text = defaultText;
            text.font = font;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
            Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            var img = obj.AddComponent<Image>();
            img.color = color;
            return obj;
        }

        private static Button CreateButton(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
            string label, Font font, UnityEngine.Events.UnityAction onClick)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            var img = obj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.25f, 0.3f, 1f);
            var btn = obj.AddComponent<Button>();
            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(obj.transform, false);
            var txtRt = txtObj.AddComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = Vector2.zero;
            txtRt.offsetMax = Vector2.zero;
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.font = font;
            txt.fontSize = 14;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            btn.onClick.AddListener(onClick);
            return btn;
        }
    }
}
