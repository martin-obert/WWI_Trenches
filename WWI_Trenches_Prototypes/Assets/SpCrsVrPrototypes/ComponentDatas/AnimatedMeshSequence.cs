using System;
using Unity.Entities;

namespace Assets.SpCrsVrPrototypes.ComponentDatas
{
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