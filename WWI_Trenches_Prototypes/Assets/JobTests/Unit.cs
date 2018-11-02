using System;
using Unity.Entities;

namespace Assets.JobTests
{
    [Serializable]
    public struct Unit : ISharedComponentData
    {
        public int Id;
    }
}