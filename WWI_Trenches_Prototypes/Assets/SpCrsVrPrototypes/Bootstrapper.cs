using System;
using System.Collections.Generic;
using Assets.IoC;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.SpCrsVrPrototypes
{
    public class UnitTemplate : MonoBehaviour
    {
        public string AnimationName;

    }

    public class Bootstrapper : MonoBehaviorDependencyResolver, IBootstrapper
    {
        [SerializeField]
        private GameObject _unit;

        [SerializeField] private Material _material;
        


        [SerializeField] public ObjAnimationSo[] Animation;

        public IReadOnlyDictionary<int, Mesh[]> ObjAnimations { get; private set; }

        public Material TempMaterial => _material;
        public Mesh TempMash;

        public string SelectionColorName;

        public void GetBasicUnitArchetypeAsync(EntityManager manager, Action<EntityArchetype> callback)
        {
            var componentTypes = new ComponentType[]
            {
                typeof(Position),
                typeof(Rotation),
                typeof(Scale),
                typeof(AnimatedMeshRenderer)
            };

            var result = manager.CreateArchetype(componentTypes);

            callback(result);
        }

        public void GetBasicUnit(EntityManager manager, float3 position, quaternion rotation, float3 scale, Action<Entity> callback = null)
        {
            GetBasicUnitArchetypeAsync(manager, archetype =>
            {
                var entity = manager.CreateEntity(archetype);

                manager.SetComponentData(entity, new Position { Value = position });

                manager.SetComponentData(entity, new Rotation { Value = rotation });

                manager.SetComponentData(entity, new Scale { Value = scale });
                manager.SetComponentData(entity, new AnimatedMeshRenderer
                {
                    DeltaTime = Random.Range(0, 1000)/1000f,
                    AnimationId = 0,
                    CastShadows = (int) ShadowCastingMode.On,
                    FrameCount = 10,
                    FrameIndex = 0,
                    RepeatCount = 0,
                    FrameRate = 24
                });

                callback?.Invoke(entity);
            });
        }

        //protected override void RegisterSelf(Injection injection)
        //{
        //    injection.Register(this);
        //}

        //protected override void UnregisterSelf(Injection injection)
        //{
        //    injection.Unregister(this);
        //}

        protected override void OnAwakeHandle()
        {

            var objAnimation = new Dictionary<int, Mesh[]>();

            var counter = 0;

            foreach (var objAnimationSo in Animation)
            {
                var mesh = objAnimationSo.ToMesh();
                objAnimation.Add(counter, mesh);
                counter++;
            }

            ObjAnimations = objAnimation;

            var entityManager = World.Active.GetOrCreateManager<EntityManager>();

            for (int i = 0; i < TestCount; i++)
            {
                var k = i;
                GetBasicUnitArchetypeAsync(entityManager,
                    archetype =>
                    {
                        GetBasicUnit(entityManager, new Vector3(k,0,0), quaternion.identity, Vector3.one);
                    });
            }

        }

        public int TestCount = 100;

        protected override void OnDestroyHandle()
        {

        }
    }
}
