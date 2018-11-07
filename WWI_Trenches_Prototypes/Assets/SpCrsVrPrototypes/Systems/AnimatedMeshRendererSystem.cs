using System.Collections.Generic;
using Assets.IoC;
using Assets.JobTests;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
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

            [ReadOnly] public ComponentDataArray<Position> Positions;
            [ReadOnly] public ComponentDataArray<Rotation> Rotations;
            [ReadOnly] public ComponentDataArray<Scale> Scales;

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

        private IReadOnlyDictionary<int, Mesh[]> _animations;
        protected override void OnCreateManager()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();

            Injection.Instance.Get<Bootstrapper>(bootstrapper =>
            {
                _animations = bootstrapper.ObjAnimations;
                _tempMaterial = bootstrapper.TempMaterial;
                _tempMesh = bootstrapper.TempMash;
            });
        }

        private Material _tempMaterial;
        private Mesh _tempMesh;

        protected override void OnUpdate()
        {
            if (_animations == null)
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

                var mesh = _animations[0];

                //Todo: select property change
                if (EntityManager.HasComponent(_data.Entities[i], typeof(Selected)))
                {
                    //_materialPropertyBlock.Set
                }

                Graphics.DrawMesh(mesh[animationComponent.FrameIndex], position.Value, rotations.Value, _tempMaterial, 0, null, 0);
            }

        }
    }
}
