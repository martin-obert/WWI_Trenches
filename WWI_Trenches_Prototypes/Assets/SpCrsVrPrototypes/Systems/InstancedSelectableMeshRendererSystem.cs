using System.Collections.Generic;
using Assets.IoC;
using Assets.JobTests;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.Singletons;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.SpCrsVrPrototypes.Systems
{
    public class InstancedSelectableMeshRendererSystem : ComponentSystem
    {
        private List<InstancedSelectableMeshRenderer> _instancedRenderers = new List<InstancedSelectableMeshRenderer>(10);

        private readonly Matrix4x4[] _matrices = new Matrix4x4[1023];

        private MaterialPropertyBlock _materialPropertyBlock;

        private string BlockPropertyName;

        private ComponentGroup _groupSelected;

        private ComponentGroup _groupUnselected;


        // This is the ugly bit, necessary until Graphics.DrawMeshInstanced supports NativeArrays pulling the data in from a job.
        private static unsafe void CopyMatrices(ComponentDataArray<LocalToWorld> transforms, int beginIndex, int length, Matrix4x4[] outMatrices)
        {
            // @TODO: This is using unsafe code because the Unity DrawInstances API takes a Matrix4x4[] instead of NativeArray.
            // We want to use the ComponentDataArray.CopyTo method
            // because internally it uses memcpy to copy the data,
            // if the nativeslice layout matches the layout of the component data. It's very fast...
            fixed (Matrix4x4* matricesPtr = outMatrices)
            {
                Assert.AreEqual(sizeof(Matrix4x4), sizeof(LocalToWorld));
                var matricesSlice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<LocalToWorld>(matricesPtr, sizeof(Matrix4x4), length);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref matricesSlice, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
#endif

                transforms.CopyTo(matricesSlice, beginIndex);
            }

        }

        protected override void OnCreateManager()
        {
            _groupSelected = GetComponentGroup(typeof(InstancedSelectableMeshRenderer), typeof(LocalToWorld), typeof(Selected));

            _groupUnselected = GetComponentGroup(typeof(InstancedSelectableMeshRenderer), typeof(LocalToWorld), ComponentType.Subtractive<Selected>());

            _materialPropertyBlock = new MaterialPropertyBlock();

            Injection.Instance.Get<Bootstrapper>(bootstrapper => BlockPropertyName = bootstrapper.SelectionColorName);
        }


        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentData(_instancedRenderers);

            for (var i = 0; i < _instancedRenderers.Count; i++)
            {
                var instancedRenderer = _instancedRenderers[i];

                if (instancedRenderer.Mesh == null)
                    continue;

                _groupSelected.SetFilter(instancedRenderer);

                _materialPropertyBlock.SetFloat(BlockPropertyName, 1);

                var matricesA = _groupSelected.GetComponentDataArray<LocalToWorld>();

                RenderGroup(instancedRenderer, matricesA);
            }

            for (var i = 0; i < _instancedRenderers.Count; i++)
            {
                var instancedRenderer = _instancedRenderers[i];

                if (instancedRenderer.Mesh == null)
                    continue;

                _groupUnselected.SetFilter(instancedRenderer);

                _materialPropertyBlock.SetFloat(BlockPropertyName, 0);

                var matricesB = _groupUnselected.GetComponentDataArray<LocalToWorld>();

                RenderGroup(instancedRenderer, matricesB);
            }

            _instancedRenderers.Clear();
        }

        private void RenderGroup(InstancedSelectableMeshRenderer instancedRenderer, ComponentDataArray<LocalToWorld> position)
        {
            int beginIndex = 0;

            while (beginIndex < position.Length)
            {
                var length = math.min(_matrices.Length, position.Length - beginIndex);

                CopyMatrices(position, beginIndex, length, _matrices);

                Graphics.DrawMeshInstanced(instancedRenderer.Mesh, 0, instancedRenderer.Material, _matrices, length, _materialPropertyBlock);

                beginIndex += length;
            }
        }
    }
}