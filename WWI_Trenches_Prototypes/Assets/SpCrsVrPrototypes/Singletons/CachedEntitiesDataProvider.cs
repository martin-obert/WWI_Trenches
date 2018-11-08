using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.IoC;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Assets.SpCrsVrPrototypes.Patterns;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public sealed class CachedEntitiesDataProvider : SingletonDisposable<CachedEntitiesDataProvider>, IEntitiesDataProvider
    {

        private IEntityMapper _entityMapper;

        private readonly IDictionary<int, EntityData> _monoStrippings;

        private readonly EntityManager _entityManager;

        public CachedEntitiesDataProvider()
        {
            _monoStrippings = new ConcurrentDictionary<int, EntityData>();

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
            return _monoStrippings[unitId].Archetype;
        }

        public Entity CreateEntity(string unitName, float3? position = null, quaternion? rotation = null, float3? scale = null)
        {
            var id = _entityMapper.NameToId(unitName);

            return CreateEntity(id, position, rotation, scale);
        }

        //Todo: bulk create to native array 
        public Entity CreateEntity(int unitId, float3? position = null, quaternion? rotation = null, float3? scale = null)
        {
            var renderData = _monoStrippings[unitId];

            var archeType = renderData.Archetype;

            var entity = _entityManager.CreateEntity(archeType);

            if (position.HasValue)
                _entityManager.SetComponentData(entity, new Position { Value = position.Value });

            if(rotation.HasValue)
                _entityManager.SetComponentData(entity, new Rotation { Value = rotation.Value });

            if (scale.HasValue)
                _entityManager.SetComponentData(entity, new Scale { Value = scale.Value });

            if (_entityManager.HasComponent(entity, typeof(AnimatedMeshSequence)))
            {
                var defaultAnimation = renderData.Animations[AnimationType.Idle];

                _entityManager.SetComponentData(entity, new AnimatedMeshSequence
                {
                    AnimationType = (int)AnimationType.Idle,
                    FrameCount = defaultAnimation.FrameCount,
                    FrameRate = defaultAnimation.FrameRate,
                    UnitId = unitId
                });
            }

            return entity;
        }

       
        public override void Dispose()
        {
            _monoStrippings.Clear();
        }

        public void RegisterEntity(MonoStripping monoStripping)
        {
            var unitId = _entityMapper.NameToId(monoStripping.UniqueName);

            var animationData = new ConcurrentDictionary<AnimationType, ObjAnimationSoCache>();

            _monoStrippings.Add(unitId, new EntityData
            {
                Stripping = monoStripping,
                Archetype = monoStripping.StripEntityArchetype(_entityManager),
                Material = monoStripping.GetMaterial(),
                Animations = animationData
            });

            if (monoStripping.IdleAnimation)
            {
                RegisterAnimation(monoStripping.IdleAnimation, animationData);
            }
        }

        private void RegisterAnimation(ObjAnimationSo objAnimation, IDictionary<AnimationType, ObjAnimationSoCache> cached)
        {
            cached.Add(objAnimation.Type, new ObjAnimationSoCache
            {
                Meshes = objAnimation.ToMesh(),
                FrameCount = objAnimation.SubMeshCount,
                FrameRate = objAnimation.FrameRate
            });
        }

        public EntityData GetMeshAnimationSet(int unitId)
        {
            return _monoStrippings[unitId];
        }
    }
}