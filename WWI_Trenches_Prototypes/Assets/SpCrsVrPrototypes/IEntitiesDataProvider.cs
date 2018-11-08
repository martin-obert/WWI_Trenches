using System;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Assets.SpCrsVrPrototypes.Singletons;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes
{
    public interface IEntitiesDataProvider
    {
        EntityArchetype GetEntityArchetype(string unitName);

        EntityArchetype GetEntityArchetype(int unitId);

        Entity CreateEntity(string unitName, float3? position = null, quaternion? rotation = null, float3? scale = null);

        Entity CreateEntity(int unitId, float3? position = null, quaternion? rotation = null, float3? scale = null);

        void RegisterEntity(MonoStripping monoStripping);

        EntityData GetMeshAnimationSet(int unitId);

    }
}
