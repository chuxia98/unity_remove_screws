using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class PlanksCatcherBehavior : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var plank = collision.attachedRigidbody.GetComponent<PlankBehavior>();

            if(plank != null && plank.IsFlying() && !plank.IsBeingDisabled) plank.Disable();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var plank = collision.attachedRigidbody.GetComponent<PlankBehavior>();

            if (plank != null && plank.IsFlying() && !plank.IsBeingDisabled) plank.Disable();
        }
    }
}
