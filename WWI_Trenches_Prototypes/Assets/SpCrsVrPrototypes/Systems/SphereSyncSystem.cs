using Assets.XnaLegacy;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Assets.SpCrsVrPrototypes.Systems
{
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