using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.SpCrsVrPrototypes.ComponentDatas
{
    [Serializable]
    public struct InstancedSelectableMeshRenderer : ISharedComponentData
    {
        public Mesh Mesh;

        public Material Material;

        public ShadowCastingMode CastShadows;
    }
}