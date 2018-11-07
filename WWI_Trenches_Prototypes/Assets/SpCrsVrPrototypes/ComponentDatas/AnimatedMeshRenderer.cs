using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

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

        public Material Material;

        public ShadowCastingMode CastShadows;
    }
}