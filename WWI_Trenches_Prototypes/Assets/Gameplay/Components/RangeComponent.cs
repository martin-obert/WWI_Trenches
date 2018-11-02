using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Gameplay.Components
{
    [Serializable]
    public struct CustomMeshRenderer : ISharedComponentData
    {
        public Mesh Mesh;

        public Material Material;
        public int SubMesh;
        public ShadowCastingMode CastShadows;
        public bool ReceiveShadows;
    }

    [Serializable]
    public struct RangeComponent : IComponentData
    {
        public float Range;
    }

    public class RangeComponentWrapper : ComponentDataWrapper<RangeComponent>
    {

    }
}
