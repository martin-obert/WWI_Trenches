using System;
using Assets.IoC;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.Singletons;
using Assets.XnaLegacy;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Systems
{
    public class HealthBarRendererSystem : ComponentSystem
    {
        private Mesh _barMesh;

        struct Data
        {
            public ComponentDataArray<XnaBoundingSphere> Spheres;
            public ComponentDataArray<Health> Health;
            public readonly int Length;
        }

        [Inject] private Data _data;


        public Material AvailibleHealthMaterial;

        //Todo: material block property?
        public Material UnAvailibleHealthMaterial;

        protected override void OnCreateManager()
        {
            Injection.Instance.Get<Bootstrapper>(bootstrapper =>
                {
                    AvailibleHealthMaterial = bootstrapper.HealthBarMaterialGreen;
                    UnAvailibleHealthMaterial = bootstrapper.HealthBarMaterialRed;
                });

            var vertices = new Vector3[]
            {
                        new Vector3(0,1),
                        new Vector3(1,1),
                        new Vector3(0,0),
                        new Vector3(1,0)
            };

            var indices = new[] { 0, 1, 2, 2, 1, 3 };
            _barMesh = new Mesh
            {
                vertices = vertices,
                triangles = indices
            };
        }

      

        struct MatrixTransforms : IJob
        {
            public float3 Size;
            public float3 CameraPosition;
            [ReadOnly] public ComponentDataArray<Health> Healths;
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Spheres;
            [WriteOnly] public NativeArray<Matrix4x4> GreenBar;
            [WriteOnly] public NativeArray<Matrix4x4> RedBar;
            public int Length;
            public void Execute()
            {
                for (int i = 0; i < Length; i++)
                {
                    var position = Spheres[i];

                    var health = Healths[i];

                    var healthAvailibleClamped = (health.Value / health.Max) * Size.x;

                    var healthUnAvailibleClamped = Size.x - healthAvailibleClamped/2; //Todo: remove this

                    GreenBar[i] = CreateHealtMatrix(position.Position - Size / 2, CameraPosition, healthAvailibleClamped, 5);

                    RedBar[i] = CreateHealtMatrix(position.Position - Size / 2, CameraPosition, -healthUnAvailibleClamped, 5, Size.x);
                }
            }
            private Vector3 GetLookNorm(float3 position, float3 cameraPostion)
            {
                return math.normalize(position - cameraPostion);
            }
            private Matrix4x4 CreateHealtMatrix(Vector3 position, float3 cameraPosition, float health, float upOffset, float? leftOffset = null)
            {
                var vector = GetLookNorm(position, cameraPosition);
                return Matrix4x4.TRS(position + Vector3.up * upOffset,
                           Quaternion.LookRotation(vector),
                           Vector3.one) * (leftOffset.HasValue ? Matrix4x4.Translate(Vector3.right * leftOffset.Value) : Matrix4x4.identity) * Matrix4x4.Scale(new Vector3(health, Size.y));
            }
        }

        private JobHandle _handle;
        protected override void OnUpdate()
        {
            if (!Input.GetKey(KeyCode.LeftAlt) || !AvailibleHealthMaterial || !UnAvailibleHealthMaterial) return;

            var positions = _data.Spheres;

            var healths = _data.Health;

            var availibleHealth = new NativeArray<Matrix4x4>(_data.Length, Allocator.TempJob);

            var unavailibleHealth = new NativeArray<Matrix4x4>(_data.Length, Allocator.TempJob);

            var job = new MatrixTransforms
            {
                Length = _data.Length,
                Spheres = _data.Spheres,
                Healths = _data.Health,
                GreenBar = availibleHealth,
                RedBar = unavailibleHealth,
                Size = new float3(15, 1, 0),
                CameraPosition = Camera.main.transform.position
            };

            _handle = job.Schedule();

            _handle.Complete();
            
            Graphics.DrawMeshInstanced(_barMesh, 0, AvailibleHealthMaterial, availibleHealth.ToArray());

            Graphics.DrawMeshInstanced(_barMesh, 0, UnAvailibleHealthMaterial, unavailibleHealth.ToArray());

            availibleHealth.Dispose();
            unavailibleHealth.Dispose();
        }

       
    }
}