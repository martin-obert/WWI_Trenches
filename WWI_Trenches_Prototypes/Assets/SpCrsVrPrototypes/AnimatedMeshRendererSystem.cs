using System.Collections.Generic;
using Assets.IoC;
using Assets.JobTests;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

namespace Assets.SpCrsVrPrototypes
{
    public class AnimatedMeshRendererSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<AnimatedMeshRenderer> AnimatedRenderers;

            [ReadOnly] public TransformAccessArray Transforms;

            [ReadOnly] public EntityArray Entities;

            public readonly int Length;
        }

        struct AnimationSyncJob : IJobParallelFor
        {
            public NativeArray<AnimatedMeshRenderer> AnimatedMeshRenderers;

            public float DeltaTime;

            public void Execute(int index)
            {
                var renderer = AnimatedMeshRenderers[index];

                renderer.DeltaTime += DeltaTime;

                if (renderer.DeltaTime >= renderer.FrameCount)
                {
                    renderer.DeltaTime = 0;
                }

                renderer.FrameIndex = (int)renderer.DeltaTime;
            }
        }

        [Inject] private Data _data;

        private JobHandle _jobHandle;

        //Todo: set properties
        private MaterialPropertyBlock _materialPropertyBlock;

        IReadOnlyDictionary<int, ObjAnimationSo> animations;
        protected override void OnCreateManager()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();

            Injection.Instance.Get<Bootstrapper>(bootstrapper => animations = bootstrapper.ObjAnimations);
        }

        protected override void OnUpdate()
        {
            if(animations == null)
                return;
                    
            _jobHandle.Complete();

            var jobData = _data.AnimatedRenderers.ToNativeArray(Allocator.TempJob);

            _jobHandle = new AnimationSyncJob
            {
                DeltaTime = Time.deltaTime,
                AnimatedMeshRenderers = jobData
            }.Schedule(_data.Length, 64);

            _jobHandle.Complete();

            jobData.CopyToAndDispose(_data.AnimatedRenderers);


            for (int i = 0; i < _data.Length; i++)
            {
                var animationComponent = _data.AnimatedRenderers[i];

                var transform = _data.Transforms[i];

                var mesh = animations[animationComponent.AnimationId].FrameMeshes[animationComponent.FrameIndex];

                //Todo: select property change
                if (EntityManager.HasComponent(_data.Entities[i], typeof(Selected)))
                {
                    //_materialPropertyBlock.Set
                }

                Graphics.DrawMesh(mesh, transform.position, transform.rotation, animationComponent.Material, 0, Camera.main, 0, null, animationComponent.CastShadows == ShadowCastingMode.On, true, true);
            }
        }
    }
}
