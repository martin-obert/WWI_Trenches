using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.SpellCrossPrototypes
{
    public interface IBootstrapper
    {
        void GetBasicUnitArchetypeAsync(EntityManager manager, Action<EntityArchetype> callback);

        void GetBasicUnit(EntityManager manager, float3 position, quaternion rotation, float3 scale,
            Action<Entity> callback = null);


    }
}