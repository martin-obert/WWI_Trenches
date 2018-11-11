using System;
using Unity.Entities;
using Unity.Mathematics;

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
    }

    [Serializable]
    public struct AnimatedMeshSequence : IComponentData
    {
        public int AnimationType;

        public float DeltaTime;

        public int FrameIndex;

        public int RepeatCount;

        public int UnitId;

        public int FrameCount;

        public int FrameRate;

        public int CastShadows;
    }
}