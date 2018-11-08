using System;
using System.Collections.Generic;
using Assets.IoC;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public class Bootstrapper : MonoBehaviorDependencyResolver, IBootstrapper
    {
        public string SelectionColorName;

        [SerializeField] private MonoStripping[] Units;

        protected override void OnAwakeHandle()
        {

            Injection.Instance.Register<EntityMapper>();

            Injection.Instance.Register<CachedEntitiesDataProvider>();


            Injection.Instance.Get<CachedEntitiesDataProvider>(provider =>
            {
                foreach (var monoStripping in Units)
                {
                    provider.RegisterEntity(monoStripping);
                }

                foreach (var monoStripping in Units)
                {
                    for (int i = 0; i < TestCount; i++)
                    {
                        print("Spawn ");
                        provider.CreateEntity(monoStripping.UniqueName, new float3(0,0,i), quaternion.identity, new float3(1,1,1));
                    }
                }
            });
            
        }

        public int TestCount = 100;

        protected override void OnDestroyHandle()
        {

        }
    }
}
