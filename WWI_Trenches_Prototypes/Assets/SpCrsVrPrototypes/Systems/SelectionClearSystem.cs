using Assets.SpCrsVrPrototypes.ComponentDatas;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.SpCrsVrPrototypes.Systems
{
    //Todo: beter move to mono behav? Or use input up/down
    [UpdateBefore(typeof(RayCastSystem))]
    public class SelectionClearSystem : JobComponentSystem
    {
        struct Data
        {
            [ReadOnly] public ComponentDataArray<Selected> Selected;
            public EntityArray Entities;
            public readonly int Length;
        }

        [Inject] private Data _data;

        [Inject] private EndFrameBarrier _endFrameBarrier;

        private JobHandle _handle;


        struct DeselectJob : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer CommandBuffer;


            [ReadOnly] public EntityArray Entities;



            public void Execute(int index)
            {

                var entity = Entities[index];

                CommandBuffer.RemoveComponent(entity, typeof(Selected));
            }

        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButton((int)MouseButton.LeftMouse) && !Input.GetKey(KeyCode.LeftControl))
            {
                _handle.Complete();

                var job = new DeselectJob
                {
                    CommandBuffer = _endFrameBarrier.CreateCommandBuffer(),
                    Entities = _data.Entities,
                };

                _handle = job.Schedule(_data.Length, 64, inputDeps);

                return _handle;
            }

            

            return base.OnUpdate(inputDeps);
        }
    }
}