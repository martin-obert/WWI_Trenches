﻿using System;
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
    public struct UnitStance : IComponentData
    {
        //Todo: To Enum
        public int Value;
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

   

    //Todo: neni tohle redundantni v ramci weapony?
    [Serializable]
    public struct Target : IComponentData
    {
        public Entity Entity;
    }

}