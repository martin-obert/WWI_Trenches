using Unity.Entities;

namespace Assets.SpCrsVrPrototypes.ComponentDatas
{
    public struct Health : IComponentData
    {
        public float Value;
        public float Max;
        public float Min;
    }
}
