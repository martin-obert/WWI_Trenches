using System;
using Unity.Entities;

namespace Assets.Gameplay.Components
{
    [Serializable]
    public struct RangeComponent : IComponentData
    {
        public float Range;
    }

    public class RangeComponentWrapper : ComponentDataWrapper<RangeComponent>
    {

    }
}
