using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.Patterns;
using Assets.XnaLegacy;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public sealed class EntityFactory : SingletonDisposable<EntityFactory>, IEntityFactory
    {
        private readonly EntityManager _entityManager;
        private EntityMapper _entityMapper;
        public EntityFactory()
        {
            _entityManager = World.Active.GetOrCreateManager<EntityManager>();
            Dependency<EntityMapper>(mapper => _entityMapper = mapper);
        }

        public override void Dispose()
        {
        }

        public Entity CreateBasicUnit(string unitName, EntityData data)
        {
            var id = _entityMapper.NameToId(unitName);

            return CreateBasicUnit(id, data);
        }

        //Todo: bulk create to native array 
        public Entity CreateBasicUnit(int unitId, EntityData data)
        {
            var entity = _entityManager.CreateEntity(data.Archetype);

            _entityManager.SetComponentData(entity, new Position { Value = data.Position });

            _entityManager.SetComponentData(entity, new Rotation { Value = data.Rotation });

            _entityManager.SetComponentData(entity, new Scale { Value = data.Scale });

            if (_entityManager.HasComponent<RayCastData>(entity))
            {
                _entityManager.SetComponentData(entity, new RayCastData
                {
                    SelectLayer = (int)RayCastLayer.UI
                });
            }

            _entityManager.SetComponentData(entity, new XnaBoundingSphere
            {
                Radius = data.SphereRadius,
                Offset = data.SphereOffset,
                Position = data.Position
            });

            _entityManager.SetComponentData(entity, new Navigation
            {
                IsAtDestination = 0,
                MoveSpeed = data.MoveSpeed,
                StoppingRadius = data.StoppingRadius,
                TurningSpeed = data.TurningSpeed,
                Velocity = data.InitialVelocity
            });

            if (_entityManager.HasComponent(entity, typeof(AnimatedMeshSequence)))
            {
                _entityManager.SetComponentData(entity, new AnimatedMeshSequence
                {
                    AnimationType = (int)AnimationType.Idle,
                    FrameCount = data.Animations[AnimationType.Idle].FrameCount,
                    FrameRate = data.Animations[AnimationType.Idle].FrameRate,
                    UnitId = unitId
                });
            }

            return entity;
        }
    }
}