using BeyondSafeZone.Core;
using BeyondSafeZone.Model;
using BeyondSafeZone.UI;
using TMPro;
using UnityEngine;

namespace BeyondSafeZone.World
{
    [RequireComponent(typeof(Collider2D))]
    public class ShelterInteractable : MonoBehaviour
    {
        [SerializeField] private string facilityId;
        [SerializeField] private TMP_Text labelText;

        public string FacilityId => facilityId;

        public void Configure(string id, TMP_Text label)
        {
            facilityId = id;
            labelText = label;
        }

        public void Refresh(GameState state)
        {
            if (labelText == null || state == null) return;
            labelText.text = ShelterInteractionCatalog.GetPrompt(state, facilityId);
        }

        public string Interact(OneRunGameController controller)
        {
            if (controller == null || controller.State == null)
                return string.Empty;

            controller.EnsureShelterActionPhase();
            string actionId = ShelterInteractionCatalog.GetActionForFacility(controller.State, facilityId);
            string result = GameSimulation.PerformShelterAction(controller.State, actionId);
            controller.Report(result);
            return result;
        }

        private void Reset()
        {
            var trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }
    }
}