using System;
using Unity.Entities;

namespace Assets.JobTests
{
    [Serializable]
    public struct GroupComponent : ISharedComponentData
    {
        public int GroupId;
        public bool IsSelected;
    }
}