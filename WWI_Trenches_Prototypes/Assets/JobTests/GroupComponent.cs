using System;
using Unity.Entities;

namespace Assets.JobTests
{
    [Serializable]
    public struct GroupComponent : IComponentData
    {
        public int GroupId;
    }
}