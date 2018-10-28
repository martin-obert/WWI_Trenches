using System;
using Unity.Entities;

namespace Assets.JobTests
{
    [Serializable]
    public struct MoveSpeedComponent :  IComponentData
    {
        public float Speed;
        public int Name;
    }
}