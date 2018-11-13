using Assets.IoC;
using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public class Bootstrapper : MonoBehaviorDependencyResolver, IBootstrapper
    {
        public string SelectionColorName;

        [SerializeField] private MonoStripping[] Units;

        private IEntitiesDataProvider _dataProvider;
        private IEntityFactory _entityFactory;
        public Material HealthBarMaterialGreen;
        public Material HealthBarMaterialRed;

        protected override void OnEnableHandle()
        {

            Injection.Instance.Register<EntityMapper>();

            Injection.Instance.Register<CachedEntitiesDataProvider>();

            Injection.Instance.Register<EntityFactory>();

            Dependency<CachedEntitiesDataProvider>(provider => _dataProvider = provider);

            Dependency<EntityFactory>(factory => _entityFactory = factory);


            ResolveDependencies();
        }

        protected override void OnDependeciesResolved()
        {
            foreach (var monoStripping in Units)
            {
                _dataProvider.RegisterEntity(monoStripping);
            }

            foreach (var monoStripping in Units)
            {
                for (int i = 0; i < TestCount; i++)
                {
                    var data = _dataProvider.GetEntityData(monoStripping.UniqueName);
                    _entityFactory.CreateBasicUnit(monoStripping.UniqueName, new EntityData
                    {
                        Archetype = data.Archetype,
                        Position = new float3(0, 0, i + i * data.SphereRadius),
                        Rotation = quaternion.identity,
                        Scale = new float3(1, 1, 1),
                        Material = data.Material,
                        Animations = data.Animations,
                        SphereRadius = data.SphereRadius,
                        MoveSpeed = data.MoveSpeed,
                        TurningSpeed = data.TurningSpeed,
                        InitialVelocity = data.InitialVelocity,
                        SphereOffset = data.SphereOffset,
                        StoppingRadius = data.StoppingRadius,
                        Health = data.Health
                    });
                }
            }
        }

        protected override void OnDestroyHandle()
        {

        }

        public int TestCount = 100;


    }


}
