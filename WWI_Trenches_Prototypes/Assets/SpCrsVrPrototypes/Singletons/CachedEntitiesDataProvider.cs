using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Assets.SpCrsVrPrototypes.Patterns;
using Unity.Entities;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public sealed class CachedEntitiesDataProvider : SingletonDisposable<CachedEntitiesDataProvider>, IEntitiesDataProvider
    {

        private IEntityMapper _entityMapper;

        private readonly IDictionary<int, EntityData> _entityDatas;

        private readonly EntityManager _entityManager;

        public CachedEntitiesDataProvider()
        {
            _entityDatas = new ConcurrentDictionary<int, EntityData>();

            _entityManager = World.Active.GetOrCreateManager<EntityManager>();

            Dependency<EntityMapper>(mapper => _entityMapper = mapper);
        }

        public EntityArchetype GetEntityArchetype(string unitName)
        {
            var id = _entityMapper.NameToId(unitName);
            return GetEntityArchetype(id);
        }

        public EntityArchetype GetEntityArchetype(int unitId)
        {
            return _entityDatas[unitId].Archetype;
        }




        public override void Dispose()
        {
            _entityDatas.Clear();
        }

        public void RegisterEntity(MonoStripping monoStripping)
        {
            var unitId = _entityMapper.NameToId(monoStripping.UniqueName);

            monoStripping.StripEntityArchetype(_entityManager);

            var defaultValues = monoStripping.DefaultUnitData;

            _entityDatas.Add(unitId, defaultValues);
        }
        public EntityData GetEntityData(int unitId)
        {
            return _entityDatas[unitId];
        }

        public EntityData GetEntityData(string unitName)
        {
            var id = _entityMapper.NameToId(unitName);

            return GetEntityData(id);
        }
    }
}