using System.Collections.Generic;
using Assets.IoC;
using Assets.JobTests;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.Singletons;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Systems
{
    public class AnimatedMeshRendererSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<AnimatedMeshSequence> AnimatedRenderers;
            

            [ReadOnly] public ComponentDataArray<Position> Positions;
            [ReadOnly] public ComponentDataArray<Rotation> Rotations;
            [ReadOnly] public ComponentDataArray<Scale> Scales;
            [ReadOnly] public EntityArray Entities;

            public readonly int Length;
        }

        struct AnimationSyncJob : IJobParallelFor
        {
            public NativeArray<AnimatedMeshSequence> AnimatedMeshRenderers;

            public float DeltaTime;

            public void Execute(int index)
            {
                var renderer = AnimatedMeshRenderers[index];


                renderer.DeltaTime += DeltaTime;

                var currentFrame = (int)(renderer.FrameRate * renderer.DeltaTime);

                if (currentFrame >= renderer.FrameCount)
                {
                    currentFrame = 0;
                    renderer.DeltaTime = 0;
                }

                renderer.FrameIndex = currentFrame;

                AnimatedMeshRenderers[index] = renderer;
            }
        }

        [Inject] private Data _data;

        private JobHandle _jobHandle;

        //Todo: set properties
        private MaterialPropertyBlock _materialPropertyBlock;

        private IEntitiesDataProvider _entitiesDataProvider;

        protected override void OnCreateManager()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();

            Injection.Instance.Get<CachedEntitiesDataProvider>(cachedEntitiesDataProvider => _entitiesDataProvider = cachedEntitiesDataProvider);
        }

        protected override void OnUpdate()
        {
            if (_entitiesDataProvider == null)
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

                var position = _data.Positions[i];

                var rotations = _data.Rotations[i];

                var renderData = _entitiesDataProvider.GetMeshAnimationSet(animationComponent.UnitId);

                var meshes = renderData.Animations[(AnimationType) animationComponent.AnimationType].Meshes;

                var material = renderData.Material;


                //Todo: select property change
                if (EntityManager.HasComponent(_data.Entities[i], typeof(Selected)))
                {
                    //_materialPropertyBlock.Set
                }

                Graphics.DrawMesh(meshes[animationComponent.FrameIndex], position.Value, rotations.Value, material, 0, null, 0);
            }

        }
    }
}
