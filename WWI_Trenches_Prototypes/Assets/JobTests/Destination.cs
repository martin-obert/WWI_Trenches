using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.JobTests
{
    [Serializable]
    public struct Destination : IComponentData
    {
        public float3 Value;
    }
}