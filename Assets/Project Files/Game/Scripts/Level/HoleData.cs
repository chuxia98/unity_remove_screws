using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class HoleData 
    {
        [SerializeField] Vector3 position;
        public Vector3 Position => position;

        [SerializeField] bool hasScrew;
        public bool HasScrew => hasScrew;

        public HoleData()
        {
        }

        public HoleData(Vector3 position, bool hasScrew)
        {
            this.position = position;
            this.hasScrew = hasScrew;
        }
    }
}
