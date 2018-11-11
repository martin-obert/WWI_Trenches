using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.XnaLegacy;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.SpCrsVrPrototypes.Systems
{
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
            
            [ReadOnly] public ComponentDataArray<Position> Positions;

            public void Execute(int index)
            {
                if (Navigations[index].Destination.Equals(float3.zero)) return;

                var nav = Navigations[index];

                var position = Positions[index];

                nav.IsAtDestination = math.distance(nav.Destination, position.Value) <= nav.StoppingRadius ? 1 : 0;

                if (nav.IsAtDestination != 1)
                {
                    nav.Velocity = math.normalizesafe(nav.Destination - position.Value, position.Value);
                }

                Navigations[index] = nav;
            }
        }

        struct PositionUpdateJob : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<Navigation> Navigations;
            public NativeArray<Position> Positions;
            public NativeArray<Rotation> Rotations;

            public void Execute(int index)
            {
                if(Navigations[index].IsAtDestination == 1) return;

                var nav = Navigations[index];

                var pos = Positions[index];

                pos.Value += nav.Velocity;

                Positions[index] = pos;
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
                    Positions = _data.Positions,
                    Navigations = navigations
                }.Schedule(_data.Length, 64, inputDeps);

            _handle = new PositionUpdateJob
            {
                Navigations = _data.Navigations,
                Positions = positions,
                Rotations = rotations,
            }.Schedule(_data.Length, 64, destinationUpdate);

            _handle.Complete();

            navigations.CopyToAndDispose(_data.Navigations);

            positions.CopyToAndDispose(_data.Positions);

            rotations.CopyToAndDispose(_data.Rotations);

            return _handle;

            //return inputDeps;
        }
    }

    public class SphereSyncSystem : JobComponentSystem
    {
        struct Data
        {
            [ReadOnly] public ComponentDataArray<Position> Positions;

            public ComponentDataArray<XnaBoundingSphere> Spheres;

            public readonly int Length;
        }

        [Inject] private Data _data;

        struct SyncJob : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<Position> Positions;

            public NativeArray<XnaBoundingSphere> Spheres;

            public void Execute(int index)
            {
                var sphere = Spheres[index];

                sphere.Position = Positions[index].Value + sphere.Offset;

                Spheres[index] = sphere;
            }
        }

        private JobHandle _handle;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps.Complete();

            var spheres = _data.Spheres.ToNativeArray(Allocator.TempJob);

            _handle = new SyncJob
            {
                Positions = _data.Positions,
                Spheres = spheres
            }.Schedule(_data.Length, 64, inputDeps);

            _handle.Complete();

            spheres.CopyToAndDispose(_data.Spheres);

            return _handle;
        }
    }
}