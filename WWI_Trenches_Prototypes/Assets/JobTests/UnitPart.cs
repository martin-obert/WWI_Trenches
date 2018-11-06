using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.JobTests
{
    public interface IMonoBehaviorStrippingService
    {
        Position StripPosition(GameObject gameObject);
        Rotation StripRotation(GameObject gameObject);
        LocalToWorld StripLocalToWorld(GameObject gameObject);
        Mesh StripMesh(GameObject gameObject);
        Material StripMaterial(GameObject gameObject);
    }

    public class MonoBehaviorStrippingService
    {

    }


    [Serializable]
    public struct Group : ISharedComponentData
    {
        public int Id;
    }

    /// <summary>
    /// Tag component
    /// </summary>
    [Serializable]
    public struct Selected : IComponentData
    {
        
    }

}