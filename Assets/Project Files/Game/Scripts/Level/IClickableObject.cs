using UnityEngine;

namespace Watermelon
{
    public interface IClickableObject
    {
        public void OnObjectClicked(Vector3 clickPosition);

        public Vector3 Position { get; }
    }
}