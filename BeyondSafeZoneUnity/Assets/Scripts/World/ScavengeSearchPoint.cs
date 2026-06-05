using BeyondSafeZone.Core;
using BeyondSafeZone.Model;
using BeyondSafeZone.UI;
using TMPro;
using UnityEngine;

namespace BeyondSafeZone.World
{
    [RequireComponent(typeof(Collider2D))]
    public class ScavengeSearchPoint : MonoBehaviour
    {
        [SerializeField] private string locationId;
        [SerializeField] private string roomId;
        [SerializeField] private string displayName;
        [SerializeField] private string tactic = "careful";
        [SerializeField] private bool lureBeforeSearch;
        [SerializeField] private TMP_Text labelText;

        public string LocationId => locationId;
        public string RoomId => roomId;
        public string DisplayName => displayName;

        public void Configure(SearchPointDefinition definition, TMP_Text label)
        {
            locationId = definition.LocationId;
            roomId = definition.RoomId;
            displayName = definition.DisplayName;
            tactic = definition.Tactic;
            lureBeforeSearch = definition.LureBeforeSearch;
            labelText = label;
        }

        public void Refresh(GameState state)
        {
            if (labelText == null || state == null) return;
            bool searched = state.Locations.TryGetValue(locationId, out var location)
                && location.Rooms.TryGetValue(roomId, out var room)
                && room.Searched;
            labelText.text = searched ? displayName + "\n已搜" : displayName + "\n按 E 搜索";
            labelText.color = searched ? new Color(0.65f, 0.65f, 0.65f) : Color.white;
        }

        public string Interact(OneRunGameController controller)
        {
            if (controller == null || controller.State == null)
                return string.Empty;

            if (controller.State.Phase != "searching" || controller.State.Exploration.ActiveLocation != locationId)
                return string.Empty;

            if (lureBeforeSearch)
                GameSimulation.LureRoom(controller.State, roomId);

            string result = GameSimulation.SearchRoom(controller.State, roomId, tactic);
            controller.Report(result);
            Refresh(controller.State);
            return result;
        }

        private void Reset()
        {
            var trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }
    }
}