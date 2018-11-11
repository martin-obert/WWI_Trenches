using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Assets.SpCrsVrPrototypes.Singletons;
using Unity.Entities;

namespace Assets.SpCrsVrPrototypes
{
    public interface IEntitiesDataProvider
    {
        EntityArchetype GetEntityArchetype(string unitName);

        EntityArchetype GetEntityArchetype(int unitId);

     

        void RegisterEntity(MonoStripping monoStripping);

        EntityData GetEntityData(int unitId);

        EntityData GetEntityData(string unitName);
    }
}
