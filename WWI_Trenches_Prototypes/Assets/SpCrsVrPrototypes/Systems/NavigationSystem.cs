using Assets.SpCrsVrPrototypes.ComponentDatas;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Systems
{
    public class NavigationActionsBarrier : BarrierSystem
    {
    }

    public class TargetFollowSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<Navigation> Navigations;
            public readonly int Length;
        }

        [Inject] private Data _data;
        [Inject] private NavigationActionsBarrier _barrier;

        protected override void OnUpdate()
        {

            for (int i = 0; i < _data.Length; i++)
            {
                var nav = _data.Navigations[i];

                if (nav.Target == Entity.Null || !EntityManager.HasComponent<Position>(nav.Target))
                    continue;

                var dest = EntityManager.GetComponentData<Position>(nav.Target);

                nav.Destination = dest.Value;

                _data.Navigations[i] = nav;
            }
        }
    }

    public class NavigationSystem : JobComponentSystem
    {
        struct Data
        {
            public readonly int Length;
            public ComponentDataArray<Navigation> Navigations;
            public ComponentDataArray<Position> Positions;
            public ComponentDataArray<Rotation> Rotations;
        }

        struct DestinationUpdateJob : IJobParallelFor
        {
            public NativeArray<Navigation> Navigations;

            [ReadOnly] public NativeArray<Position> Positions;

            public void Execute(int index)
            {
                if (Navigations[index].Destination.Equals(float3.zero)) return;

                var nav = Navigations[index];

                var position = Positions[index];

                nav.IsAtDestination = math.distance(nav.Destination, position.Value) <= nav.StoppingRadius ? 1 : 0;

                if (nav.IsAtDestination != 1)
                {
                    nav.Velocity = math.normalize(nav.Destination - position.Value);
                }

                Navigations[index] = nav;
            }
        }

        struct TransformsUpdateJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Navigation> Navigations;
            public NativeArray<Position> Positions;
            public NativeArray<Rotation> Rotations;
            public float DeltaTime;
            public void Execute(int index)
            {
                var nav = Navigations[index];

                if (nav.IsAtDestination == 1 || nav.Velocity.Equals(float3.zero)) return;

                var pos = Positions[index];

                var rot = Rotations[index];

                pos.Value = math.lerp(pos.Value, pos.Value + nav.Velocity, DeltaTime);

                var targetRotation = Quaternion.LookRotation(nav.Velocity);

                rot.Value = Quaternion.Slerp(rot.Value, targetRotation, DeltaTime * nav.TurningSpeed);

                Positions[index] = pos;
                Rotations[index] = rot;
            }
        }

        [Inject] private Data _data;

        private JobHandle _handle;
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps.Complete();

            var positions = _data.Positions.ToNativeArray(Allocator.TempJob);

            var rotations = _data.Rotations.ToNativeArray(Allocator.TempJob);

            var navigations = _data.Navigations.ToNativeArray(Allocator.TempJob);

            var destinationUpdate = new DestinationUpdateJob
            {
                Positions = positions,
                Navigations = navigations
            }.Schedule(_data.Length, 64, inputDeps);

            _handle = new TransformsUpdateJob
            {
                Navigations = navigations,
                Positions = positions,
                Rotations = rotations,
                DeltaTime = Time.deltaTime
            }.Schedule(_data.Length, 64, destinationUpdate);

            _handle.Complete();

            navigations.CopyToAndDispose(_data.Navigations);

            positions.CopyToAndDispose(_data.Positions);

            rotations.CopyToAndDispose(_data.Rotations);

            return _handle;

            //return inputDeps;
        }
    }
}