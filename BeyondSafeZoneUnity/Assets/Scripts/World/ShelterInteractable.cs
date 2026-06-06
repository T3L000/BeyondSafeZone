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
        [SerializeField] private SpriteRenderer stateRenderer;
        [SerializeField] private GameObject blueprintVisual;
        [SerializeField] private GameObject builtVisual;
        [SerializeField] private GameObject usedMarker;
        [SerializeField] private GameObject damageMarker;
        [SerializeField] private TMP_Text feedbackText;

        private bool isHighlighted;
        private Color baseColor = Color.white;

        public string FacilityId => facilityId;

        public bool IsHighlighted => isHighlighted;

        public void Configure(string id, TMP_Text label, SpriteRenderer stateVisual = null,
            GameObject blueprint = null, GameObject built = null, GameObject used = null,
            GameObject damage = null, TMP_Text feedback = null)
        {
            facilityId = id;
            labelText = label;
            stateRenderer = stateVisual;
            blueprintVisual = blueprint;
            builtVisual = built;
            usedMarker = used;
            damageMarker = damage;
            feedbackText = feedback;
        }

        public void SetHighlighted(bool highlighted)
        {
            if (isHighlighted == highlighted) return;
            isHighlighted = highlighted;
            ApplyColorTint();
        }

        public void Refresh(GameState state)
        {
            if (state == null) return;
            if (labelText != null)
            {
                var definition = ShelterInteractionCatalog.Get(facilityId);
                labelText.text = definition == null ? facilityId : definition.DisplayName;
            }

            if (!state.Shelter.Facilities.TryGetValue(facilityId, out var facility)) return;

            bool isDamagedBarricade = facilityId == "barricade" && state.Shelter.Door <= 2;
            SetActive(blueprintVisual, !facility.Built);
            SetActive(builtVisual, facility.Built);
            SetActive(usedMarker, facility.Built && facility.UsedToday);
            SetActive(damageMarker, isDamagedBarricade);

            if (!facility.Built)
                baseColor = new Color(0.45f, 0.65f, 0.95f, 0.42f);
            else if (facility.UsedToday)
                baseColor = new Color(0.44f, 0.44f, 0.38f, 0.72f);
            else if (isDamagedBarricade)
                baseColor = new Color(0.78f, 0.22f, 0.18f, 0.88f);
            else
                baseColor = new Color(0.58f, 0.64f, 0.58f, 0.92f);

            ApplyColorTint();
        }

        private void ApplyColorTint()
        {
            if (stateRenderer == null) return;
            stateRenderer.color = isHighlighted ? HighlightColor(baseColor) : baseColor;
        }

        private static Color HighlightColor(Color baseColor)
        {
            return new Color(
                Mathf.Min(baseColor.r * 1.5f, 1f),
                Mathf.Min(baseColor.g * 1.5f, 1f),
                Mathf.Min(baseColor.b * 1.5f, 1f),
                Mathf.Min(baseColor.a + 0.08f, 1f)
            );
        }

        public string Interact(OneRunGameController controller)
        {
            if (controller == null || controller.State == null)
                return string.Empty;

            controller.EnsureShelterActionPhase();
            string actionId = ShelterInteractionCatalog.GetActionForFacility(controller.State, facilityId);
            string result = GameSimulation.PerformShelterAction(controller.State, actionId);
            ShowFeedback(result);
            controller.Report(result);
            return result;
        }

        private void ShowFeedback(string result)
        {
            if (feedbackText == null) return;

            feedbackText.text = result;
            feedbackText.gameObject.SetActive(!string.IsNullOrEmpty(result));
        }

        private static void SetActive(GameObject obj, bool active)
        {
            if (obj != null)
                obj.SetActive(active);
        }

        private void Reset()
        {
            var trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }
    }
}
