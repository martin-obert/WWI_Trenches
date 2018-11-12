using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.SpCrsVrPrototypes.ComponentDatas
{
    [Serializable]
    public struct Navigation : IComponentData
    {
        public float3 Velocity;
        public float3 Destination;
        public float StoppingRadius;
        public int IsAtDestination;
        public float TurningSpeed;
        public float MoveSpeed;
        public Entity Target;
    }
}