using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.JobTests
{
    [Serializable]
    public struct UnitRenderer : ISharedComponentData
    {
        public Mesh Mesh;
        public Material Material;
    }
}