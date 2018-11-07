using System;
using System.Collections.Generic;
using Assets.IoC;
using Assets.ObjAnimations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

        [SerializeField] public ObjAnimationSo[] Animation;

        public IReadOnlyDictionary<int, ObjAnimationSo> ObjAnimations { get; private set; }
        public string SelectionColorName;

        public void GetBasicUnitArchetypeAsync(EntityManager manager, Action<EntityArchetype> callback)
        {
            var componentTypes = new ComponentType[]
            {
                typeof(Position),
                typeof(Rotation),
                typeof(Scale)
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
                
                callback?.Invoke(entity);
            });
        }

        protected override void OnAwakeHandle()
        {
            var objAnimation = new Dictionary<int, ObjAnimationSo>();

            var counter = 0;

            foreach (var objAnimationSo in objAnimation)
            {
                objAnimation.Add(counter, objAnimationSo.Value);
            }

            ObjAnimations = objAnimation;
        }

        protected override void OnDestroyHandle()
        {

        }
    }
}
