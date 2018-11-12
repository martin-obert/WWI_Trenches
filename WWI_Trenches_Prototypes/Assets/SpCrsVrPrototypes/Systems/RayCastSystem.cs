using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.Enums;
using Assets.XnaLegacy;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.SpCrsVrPrototypes.Systems
{
    public class RayCastSystem : JobComponentSystem
    {
        struct Data
        {
            [ReadOnly] public ComponentDataArray<RayCastData> RayCastObjects;
            [ReadOnly] public ComponentDataArray<Position> Positions;
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Spheres;
            [ReadOnly] public SubtractiveComponent<Selected> UnSelected;
            public EntityArray Entities;
            public readonly int Length;
        }

        struct SelectJob : IJob
        {
            [ReadOnly] public NativeList<int> Indices;
            public float3 CameraPosition;
            [ReadOnly] public EntityCommandBuffer CommandBuffer;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<Position> Positions;

            public void Execute()
            {
                if (Indices.Length <= 0) return;

                if (Indices.Length == 1)
                {
                    CommandBuffer.AddComponent(Entities[Indices[0]], new Selected());
                    return;
                }

                var index = Indices[0];

                var position = Positions[0].Value;

                float shortest = math.distance(position, CameraPosition);

                for (int i = 1; i < Indices.Length; i++)
                {
                    position = Positions[Indices[i]].Value;

                    var dist = math.distance(position, CameraPosition);

                    if (dist < shortest)
                    {
                        shortest = dist;
                        index = Indices[i];
                    }
                }

                CommandBuffer.AddComponent(Entities[index], new Selected());
            }
        }
        struct RayCastJob : IJobParallelForFilter
        {

            [ReadOnly] public ComponentDataArray<RayCastData> RayCastData;

            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Spheres;


            public XnaRay Ray;

            public int RayLayer;

            public bool Execute(int index)
            {
                var rayData = RayCastData[index];

                if (rayData.SelectLayer != RayLayer)
                    return false;

                var sphere = Spheres[index];

                var hitDistance = Ray.Intersects(sphere);

                return hitDistance.HasValue;
            }
        }

        [Inject] private Data _data;

        [Inject] private EndFrameBarrier _endFrameBarrier;

        private JobHandle _handle;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                inputDeps.Complete();

                var indices = new NativeList<int>(Allocator.TempJob);

                var rayCastHandle = new RayCastJob
                {
                    Ray = new XnaRay { Position = ray.origin, Direction = ray.direction },
                    RayLayer = (int)RayCastFlag.Unit,
                    RayCastData = _data.RayCastObjects,
                    Spheres = _data.Spheres,
                }.ScheduleAppend(indices, _data.Length, 64, inputDeps);

                rayCastHandle.Complete();

                _handle = new SelectJob
                {
                    CommandBuffer = _endFrameBarrier.CreateCommandBuffer(),
                    Indices = indices,
                    Entities = _data.Entities,
                    Positions = _data.Positions,
                    CameraPosition = ray.origin
                }.Schedule(rayCastHandle);


                _handle.Complete();

                indices.Dispose();

                return _handle;
            }
            
            return inputDeps;
        }
    }
}
