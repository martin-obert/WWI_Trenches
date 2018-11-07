using System;
using Unity.Entities;

namespace Assets.SpCrsVrPrototypes.ComponentDatas
{
    [Serializable]
    public struct AnimatedMeshRenderer : IComponentData
    {
        public float DeltaTime;

        public int FrameIndex;

        public int RepeatCount;

        public int AnimationId;

        public int FrameCount;

        public int FrameRate;

        public int CastShadows;
    }
}