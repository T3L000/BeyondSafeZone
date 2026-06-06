using UnityEngine;

namespace BeyondSafeZone.World
{
    [RequireComponent(typeof(Collider2D))]
    public class ShelterStairZone : MonoBehaviour
    {
        [SerializeField] private Transform groundPoint;
        [SerializeField] private Transform upperPoint;

        public void Configure(Transform ground, Transform upper)
        {
            groundPoint = ground;
            upperPoint = upper;
        }

        public Vector3 GetTargetPosition(Vector3 currentPosition, float verticalInput)
        {
            if (verticalInput > 0f && upperPoint != null)
                return upperPoint.position;
            if (verticalInput < 0f && groundPoint != null)
                return groundPoint.position;
            return currentPosition;
        }

        private void Reset()
        {
            var trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }
    }
}
