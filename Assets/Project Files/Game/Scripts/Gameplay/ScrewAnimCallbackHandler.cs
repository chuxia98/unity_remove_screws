using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class ScrewAnimCallbackHandler : MonoBehaviour
    {
        [SerializeField] ScrewBehavior behavior;

        public void OnUnscrewPUAnimEnded()
        {
            behavior.OnUnscrewPUAnimEnded();
        }
    }
}
