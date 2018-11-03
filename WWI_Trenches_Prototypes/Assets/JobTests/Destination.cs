using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.JobTests
{
    [Serializable]
    public struct Health : IComponentData
    {
        public float Value;
        
    }


    [Serializable]
    public struct Destination : IComponentData
    {
        public float3 Value;
    }
}