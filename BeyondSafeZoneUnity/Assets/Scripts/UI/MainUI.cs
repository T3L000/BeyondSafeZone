using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BeyondSafeZone.Core;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.UI
{
    /// <summary>主界面 —— 对应 Godot main.gd。MonoBehaviour 作为 Unity UGUI 入口。</summary>
    public class MainUI : MonoBehaviour
    {
        [Header("UI References (bind in Unity Editor)")]
        public Text statusText;
        public Text statsText;
        public Text resourcesText;
        public Transform actionPanel;
        public Text eventLogText;
        public Text locationCardText;
        public Text roomCardText;
        public Text shelterInfoText;

        public GameObject mapPanel;
        public GameObject explorerPanel;
        public Transform locationButtonsParent;
        public Transform roomButtonsParent;

        public Button sleepButton;
        public Button restartButton;
        public Button leaveExplorationButton;

        private GameState _state;

        void Start()
        {
            NewGame();
        }

        public void NewGame()
        {
            _state = GameSimulation.NewGame();
            RefreshUI();
        }

        public void OnExploreLocation(string locationId)
        {
            if (_state == null) return;
            GameSimulation.EnterLocation(_state, locationId);
            RefreshUI();
        }

        public void OnSearchRoom(string roomId)
        {
            if (_state == null) return;
            GameSimulation.SearchRoom(_state, roomId);
            RefreshUI();
        }

        public void OnLureRoom(string roomId)
        {
            if (_state == null) return;
            GameSimulation.LureRoom(_state, roomId);
            RefreshUI();
        }

        public void OnLeaveExploration()
        {
            if (_state == null) return;
            GameSimulation.LeaveExploration(_state);
            RefreshUI();
        }

        public void OnShelterAction(string actionId)
        {
            if (_state == null) return;
            GameSimulation.PerformShelterAction(_state, actionId);
            RefreshUI();
        }

        public void OnSleep()
        {
            if (_state == null) return;
            GameSimulation.SleepAndResolveNight(_state);
            RefreshUI();
        }

        public void OnRestart()
        {
            NewGame();
        }

        public void OnSafeDemoDay()
        {
            if (_state == null || _state.DemoComplete) return;
            GameSimulation.PlaySafeDemoDay(_state, _state.Day);
            RefreshUI();
        }

        // ============ UI Refresh ============

        private void RefreshUI()
        {
            if (_state == null) return;

            RefreshStatus();
            RefreshStats();
            RefreshResources();
            RefreshShelter();
            RefreshPhasePanels();
            RefreshActionButtons();
            RefreshEventLog();
        }

        private void RefreshStatus()
        {
            if (statusText == null) return;
            string pressureLabel = _state.MorningContext?.PressureType switch
            {
                "tutorial" => "教程期",
                "scarcity" => "物资紧张",
                "stress" => "心理压迫",
                "mobility" => "出行受限",
                "qimian" => "祁眠苏醒",
                "warning" => "血月预警",
                "blood_moon" => "血月降临",
                "aftermath" => "血月余波",
                "foreshadow" => "伏笔日",
                "red_tide" => "红潮侵蚀",
                "unknown" => "未知",
                _ => "正常"
            };
            string shortCtx = $"态势：{pressureLabel}";
            string warn = _state.MorningContext?.BloodMoonWarning ?? "";
            if (!string.IsNullOrEmpty(warn)) shortCtx += $" | ⚠️ {warn}";
            if (_state.DemoComplete) shortCtx += $" | 结局：{EndingLabel(_state.EndingState)}";

            statusText.text = $"{_state.LastEvent}\n" +
                $"第 {_state.Day} 天  {PhaseLabel(_state.Phase)}  目标：{_state.Goal}  |  {shortCtx}";
        }

        private void RefreshStats()
        {
            if (statsText == null) return;
            string carStep = "";
            if (_state.Car.Found)
            {
                var parts = new List<string>();
                if (_state.Car.StepEngine) parts.Add("引擎✓"); else parts.Add("引擎✗");
                if (_state.Car.StepTire) parts.Add("轮胎✓"); else parts.Add("轮胎✗");
                if (_state.Car.StepBattery) parts.Add("电池✓"); else parts.Add("电池✗");
                if (_state.Car.StepFueled) parts.Add("加油✓"); else parts.Add("加油✗");
                carStep = string.Join(" ", parts);
            }
            if (_state.Car.Ready) carStep = "已完成";

            statsText.text =
                $"🧑 林行：{GameSimulation.GetLinConditionText(_state)}\n" +
                $"撤离：📻{FlagLabel(_state.Evacuation.SafezoneConfirmed)} 📍{FlagLabel(_state.Evacuation.AddressKnown)} 🚗{FlagLabel(_state.Evacuation.CarReady)}\n" +
                $"零件：🔋{_state.CarParts.Battery} 🛢️{_state.CarParts.Gasoline} 🛞{_state.CarParts.Tire} | 修理：{carStep}";
        }

        private void RefreshResources()
        {
            if (resourcesText == null) return;
            resourcesText.text =
                $"🍞{_state.Resources.Food} 💧{_state.Resources.Water} 💊{_state.Resources.Meds}  " +
                $"🧱{_state.Resources.Materials} 🔧{_state.Resources.Parts} ⛽{_state.Resources.Fuel}";
        }

        private void RefreshShelter()
        {
            if (shelterInfoText == null) return;
            var parts = new List<string>();
            foreach (var kv in _state.Shelter.Facilities)
                parts.Add($"{kv.Value.Name} Lv.{kv.Value.Level}{(kv.Value.UsedToday ? " ✓" : "")}");
            shelterInfoText.text =
                $"🏠 据点 | 门 {_state.Shelter.Door} 防御 {_state.Shelter.Defense} " +
                $"噪音 {_state.Shelter.Noise} 气味 {_state.Shelter.Scent} 光线 {_state.Shelter.Light}\n" +
                $"设施：{string.Join("  ", parts)}";
        }

        private void RefreshPhasePanels()
        {
            bool isSearching = _state.Phase == "searching";
            if (mapPanel != null) mapPanel.SetActive(!isSearching);
            if (explorerPanel != null) explorerPanel.SetActive(isSearching);

            if (isSearching && locationCardText != null)
                locationCardText.text = GameSimulation.GetLocationCardText(_state, _state.Exploration.ActiveLocation);
        }

        private void RefreshActionButtons()
        {
            if (actionPanel == null) return;
            foreach (Transform child in actionPanel)
                Destroy(child.gameObject);

            if (_state.DemoComplete)
            {
                AddActionButton("Demo 结束 — 祁眠日志已解锁", null, false);
                AddActionButton("🔄 重新开始", OnRestart);
                return;
            }

            if (_state.Phase == "searching")
            {
                var tipObj = new GameObject("Tip");
                tipObj.transform.SetParent(actionPanel);
                var tipText = tipObj.AddComponent<Text>();
                tipText.text = "👆 点击房间进行搜索\n暗房 → 先引开尸群\n搜完 → 点击离开";
                return;
            }

            bool phaseOk = _state.Phase == "evening" || _state.Phase == "night";

            var actions = new (string id, string label)[]
            {
                ("rest_bed", "🛏️ 休息"),
                ("workbench_repair", "🔧 修自行车"),
                ("barricade_windows", "🪟 封窗(建材-2)"),
                ("radio_broadcast", "📻 听广播(燃料-1)"),
                ("organize_storage", "📦 整理物资"),
                ("treat_wound", "💊 处理伤口"),
                ("workbench_car", "🚗 修理汽车"),
                ("fortify", "🛡️ 加固(建材-2)"),
                ("quiet", "🤫 降低噪音"),
                ("mask_scent", "🫙 遮蔽气味"),
            };

            foreach (var (id, label) in actions)
            {
                if (id == "workbench_car" && !(_state.Car.Found && !_state.Car.Ready)) continue;
                AddActionButton(label, () => OnShelterAction(id), phaseOk);
            }

            // Sleep button
            AddActionButton("😴 睡觉结算夜晚", OnSleep, phaseOk);

            // Restart
            AddActionButton("🔄 重新开始", OnRestart);
        }

        private void AddActionButton(string label, UnityEngine.Events.UnityAction onClick, bool enabled = true)
        {
            var btnObj = new GameObject(label);
            btnObj.transform.SetParent(actionPanel);
            var btn = btnObj.AddComponent<Button>();
            var btnText = btnObj.AddComponent<Text>();
            btnText.text = enabled ? label : label + " (等待夜晚)";
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btnText.fontSize = 14;
            btnText.color = Color.white;
            btn.interactable = enabled;
            if (enabled && onClick != null) btn.onClick.AddListener(onClick);
        }

        private void RefreshEventLog()
        {
            if (eventLogText == null) return;
            var lines = new List<string>();
            lines.Add("普通事件");
            lines.Add(_state.LastEvent);
            lines.Add("");

            lines.Add("祁眠异常线索");
            if (_state.Qimian.PublicClues.Count == 0)
                lines.Add("暂时没有。");
            else
                foreach (var clue in _state.Qimian.PublicClues)
                    lines.Add($"- {clue}");

            lines.Add("");
            lines.Add("异常档案 (anomaly_dossier)");
            string dossierText = GameSimulation.GetAnomalyDossierText(_state);
            lines.Add(string.IsNullOrEmpty(dossierText) ? "暂无异常记录。" : dossierText);

            if (_state.Reveal.Unlocked)
            {
                // ---- 玩家标记 → 祁眠感知链 ----
                string markChain = GameSimulation.GetPlayerMarkPerceptionChain(_state);
                if (!string.IsNullOrEmpty(markChain))
                {
                    lines.Add("");
                    lines.Add(markChain);
                }

                lines.Add("");
                lines.Add("═══ 祁眠行动日志 · 一周目回放 ═══");
                lines.Add($"结局：{EndingLabel(_state.EndingState)}");
                lines.Add(_state.Reveal.Summary);
                lines.Add("");
                lines.Add("▸ 祁眠人格卡");
                lines.Add($"主目标：{_state.Qimian.PersonalityCard.MainGoal} | 暴露：{_state.Qimian.PersonalityCard.Exposure} | 道德：{_state.Qimian.PersonalityCard.MoralRule}");
                lines.Add("");
                lines.Add("▸ AI 运行状态");
                lines.Add($"暴露值：{_state.Qimian.AiState.Exposure}/10 | 摩托：Lv.{_state.Qimian.AiState.MotoTier} | 祁烬线索：{_state.Qimian.AiState.QijinClues}/3");
                lines.Add("");
                lines.Add("▸ 逐日行动回放");
                foreach (var entry in _state.Qimian.Log)
                {
                    lines.Add($"── 第 {entry.Day} 天 ──");
                    lines.Add($"   行动：{entry.Title}");
                    lines.Add($"   真相：{entry.Truth}");
                    if (!string.IsNullOrEmpty(entry.AiReplay))
                        lines.Add($"   AI 决策：{entry.AiReplay}");
                    if (!string.IsNullOrEmpty(entry.SubjectiveFragment))
                        lines.Add($"   祁眠记录：{entry.SubjectiveFragment}");
                }
            }
            else
            {
                lines.Add("");
                lines.Add("祁眠日志仍被隐藏。通关 Demo 后解锁。");
            }

            eventLogText.text = string.Join("\n", lines);
        }

        // ============ Text Helpers ============

        private static string PhaseLabel(string phase) => phase switch
        {
            "morning" => "☀️ 清晨",
            "day" => "🌤️ 白天",
            "searching" => "🔍 探索中",
            "evening" => "🌇 黄昏",
            "night" => "🌙 夜晚",
            "reveal" => "📖 揭示",
            _ => phase
        };

        private static string FlagLabel(bool flag) => flag ? "✓" : "✗";

        private static string EndingLabel(string ending) => ending switch
        {
            "collapsed" => "崩溃",
            "barely_reached_gate" => "勉强抵达",
            "reached_gate_quarantine" => "隔离抵达",
            "in_progress" => "进行中",
            _ => ending
        };
    }
}
