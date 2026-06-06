using BeyondSafeZone.UI;
using BeyondSafeZone.World;
using UnityEngine;

namespace BeyondSafeZone.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SideViewShelterPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private OneRunGameController gameController;

        private Rigidbody2D body;
        private ShelterInteractable nearbyInteractable;
        private ShelterStairZone nearbyStairZone;
        private float horizontalInput;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
        }

        private void Update()
        {
            horizontalInput = ReadHorizontalInput();

            if (nearbyStairZone != null)
            {
                float verticalInput = ReadVerticalInput();
                if (Mathf.Abs(verticalInput) > 0.1f)
                    body.position = nearbyStairZone.GetTargetPosition(body.position, verticalInput);
            }

            if (Input.GetKeyDown(KeyCode.E) && nearbyInteractable != null)
                nearbyInteractable.Interact(gameController);
        }

        private void FixedUpdate()
        {
            body.MovePosition(body.position + Vector2.right * (horizontalInput * moveSpeed * Time.fixedDeltaTime));
        }

        public void Configure(OneRunGameController controller)
        {
            gameController = controller;
        }

        private float ReadHorizontalInput()
        {
            int direction = 0;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                direction--;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                direction++;

            if (direction != 0)
                return Mathf.Clamp(direction, -1, 1);

            return Input.GetAxisRaw("Horizontal");
        }

        private float ReadVerticalInput()
        {
            int direction = 0;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                direction--;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                direction++;

            if (direction != 0)
                return Mathf.Clamp(direction, -1, 1);

            return Input.GetAxisRaw("Vertical");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var stairZone = other.GetComponent<ShelterStairZone>();
            if (stairZone != null)
            {
                nearbyStairZone = stairZone;
                gameController?.ShowPrompt("stairs");
                return;
            }

            var interactable = other.GetComponent<ShelterInteractable>();
            if (interactable == null) return;
            if (nearbyInteractable != null && nearbyInteractable != interactable)
                nearbyInteractable.SetHighlighted(false);
            nearbyInteractable = interactable;
            nearbyInteractable.SetHighlighted(true);
            gameController?.ShowPrompt(interactable.FacilityId);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var stairZone = other.GetComponent<ShelterStairZone>();
            if (stairZone != null && stairZone == nearbyStairZone)
            {
                nearbyStairZone = null;
                gameController?.ShowPrompt(string.Empty);
                return;
            }

            var interactable = other.GetComponent<ShelterInteractable>();
            if (interactable == null || interactable != nearbyInteractable) return;
            nearbyInteractable.SetHighlighted(false);
            nearbyInteractable = null;
            gameController?.ShowPrompt(string.Empty);
        }
    }
}
