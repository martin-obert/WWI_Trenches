using Unity.Entities;

namespace Assets.SpCrsVrPrototypes.ComponentDatas
{
    public struct Selected : IComponentData
    {

    }

    public enum RayCastLayer
    {
        None = 0,
        UI = 1
    }

    public struct RayCastData : IComponentData
    {
        public int SelectLayer;
    }
}
