using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.JobTests
{
  

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