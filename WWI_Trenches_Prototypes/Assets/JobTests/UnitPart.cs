using System;
using Unity.Entities;

namespace Assets.JobTests
{
    [Serializable]
    public struct UnitPartComponent : IComponentData
    {
        public int UnitId;
    }

    public class UnitPartComponentWrapper : ComponentDataWrapper<UnitPartComponent>
    {

    }

}