using UnityEngine;

namespace Watermelon
{
    public class BaseHoleBehavior : HoleBehavior
    {
        [SerializeField] CircleCollider2D circleCollider;

        public float ColliderRadius => circleCollider.radius;
        public float PhysicsRadius => circleCollider.radius * GameController.Data.HoleVisibleAmountToEnableScrew;

        public event SimpleBoolCallback StateChanged;

        public void Init(HoleData data)
        {
            StateChanged = null;

            transform.position = data.Position.SetZ(0.9f);

            IsActive = false;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;

            StateChanged?.Invoke(isActive);
        }

        public override void Discard()
        {
            IsActive = false;
            gameObject.SetActive(false);

            StateChanged?.Invoke(false);
        }
    }
}
