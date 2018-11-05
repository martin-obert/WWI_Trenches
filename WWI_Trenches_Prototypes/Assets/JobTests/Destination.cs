using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.JobTests
{
    [Serializable]
    public struct Health : IComponentData
    {
        public float Value;

        public float Max;
    }

    [Serializable]
    public struct Speed : IComponentData
    {
        public float Value;
    }

    [Serializable]
    public struct Destination : IComponentData
    {
        public float3 Value;
    }
}