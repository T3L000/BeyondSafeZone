using BeyondSafeZone.UI;
using BeyondSafeZone.World;
using UnityEngine;

namespace BeyondSafeZone.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private OneRunGameController gameController;

        private Rigidbody2D body;
        private ShelterInteractable nearbyInteractable;
        private ScavengeSearchPoint nearbySearchPoint;
        private Vector2 input;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
        }

        private void Update()
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (nearbySearchPoint != null)
                    nearbySearchPoint.Interact(gameController);
                else if (nearbyInteractable != null)
                    nearbyInteractable.Interact(gameController);
            }
        }

        private void FixedUpdate()
        {
            body.MovePosition(body.position + input * moveSpeed * Time.fixedDeltaTime);
        }

        public void Configure(OneRunGameController controller)
        {
            gameController = controller;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var searchPoint = other.GetComponent<ScavengeSearchPoint>();
            if (searchPoint != null)
            {
                nearbySearchPoint = searchPoint;
                gameController?.ShowSearchPrompt(searchPoint.DisplayName);
                return;
            }

            var interactable = other.GetComponent<ShelterInteractable>();
            if (interactable == null) return;
            nearbyInteractable = interactable;
            gameController?.ShowPrompt(interactable.FacilityId);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var searchPoint = other.GetComponent<ScavengeSearchPoint>();
            if (searchPoint != null && searchPoint == nearbySearchPoint)
            {
                nearbySearchPoint = null;
                gameController?.ShowPrompt(string.Empty);
                return;
            }

            var interactable = other.GetComponent<ShelterInteractable>();
            if (interactable == null || interactable != nearbyInteractable) return;
            nearbyInteractable = null;
            gameController?.ShowPrompt(string.Empty);
        }
    }
}