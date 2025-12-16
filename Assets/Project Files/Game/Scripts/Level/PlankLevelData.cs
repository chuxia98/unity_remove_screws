using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class PlankLevelData
    {
        [SerializeField] PlankType plankType;
        public PlankType PlankType => plankType;

        [SerializeField] int plankLayer;
        public int PlankLayer => plankLayer;

        [SerializeField] Vector3 position;
        public Vector3 Position => position;
        [SerializeField] Vector3 rotation;
        public Vector3 Rotation => rotation;
        [SerializeField] Vector3 scale;
        public Vector3 Scale => scale;

        [SerializeField] List<Vector3> screwsPositions;
        public List<Vector3> ScrewsPositions => screwsPositions;

        public PlankLevelData()
        {
        }

        public PlankLevelData(PlankType plankType, int plankLayer, Vector3 position, Vector3 rotation, Vector3 scale, List<Vector3> screwsPositions)
        {
            this.plankType = plankType;
            this.plankLayer = plankLayer;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.screwsPositions = screwsPositions;
        }
    }
}