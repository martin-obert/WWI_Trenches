using System;
using Unity.Entities;

namespace Assets.JobTests
{
    [Serializable]
    public struct RangeProximityComponent : IComponentData
    {
        public float Range;
    }
}