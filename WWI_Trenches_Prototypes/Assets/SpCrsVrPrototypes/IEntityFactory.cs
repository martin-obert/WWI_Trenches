using Assets.SpCrsVrPrototypes.Singletons;
using Unity.Entities;

namespace Assets.SpCrsVrPrototypes
{
    public interface IEntityFactory
    {
        Entity CreateBasicUnit(string unitName, EntityData data);

        Entity CreateBasicUnit(int unitId, EntityData data);
    }
}