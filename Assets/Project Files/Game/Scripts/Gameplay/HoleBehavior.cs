using UnityEngine;

namespace Watermelon
{
    public abstract class HoleBehavior : MonoBehaviour, IClickableObject
    {
        public Vector3 Position => transform.position;

        public bool IsActive { get; protected set; }

        private void Awake()
        {
            IsActive = false;
        }

        public virtual void OnObjectClicked(Vector3 clickPosition)
        {

        }

        public virtual void Discard()
        {

        }
    }
}