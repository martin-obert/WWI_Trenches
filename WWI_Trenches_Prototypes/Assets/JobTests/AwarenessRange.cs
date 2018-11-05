using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.JobTests
{
    [Serializable]
    public struct AwarenessRange : IComponentData
    {
        public float Value;
    }

    [Serializable]
    public struct WeaponPrototype : IComponentData
    {
        public float Range;
        public Target Target;
    }

    [Serializable]
    public struct Enemy : IComponentData
    {

    }


    [Serializable]
    public struct Target : IComponentData
    {

    }
}