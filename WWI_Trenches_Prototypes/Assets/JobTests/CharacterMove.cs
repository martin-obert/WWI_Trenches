using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

namespace Assets.JobTests
{
    public class MoveSystem : JobComponentSystem
    {
        [BurstCompile]
        public struct MovementJob : IJobProcessComponentData<Position, Rotation, MoveSpeedComponent>
        {
            public float DeltaTime;


            public void Execute(ref Position data0, ref Rotation data1, ref MoveSpeedComponent data2)
            {
                var forward = math.forward(data1.Value) * data2.Speed * DeltaTime;

                data0.Value += forward;

            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MovementJob
            {
                DeltaTime = Time.deltaTime
            };

            var handle = job.Schedule(this);

            return handle;
        }
    }



    public class CharacterMove : MonoBehaviour
    {
        public GameObject prefab;
        public TransformAccessArray TransformAccessArray;
        private int instanceCount = 10;
        private EntityManager _manager;
        void Start()
        {
            TransformAccessArray = new TransformAccessArray(200);
            _manager = World.Active.GetOrCreateManager<EntityManager>();

            Spawn();
        }

        private void Spawn()
        {
            var entities = new NativeArray<Entity>(instanceCount, Allocator.Temp);

            _manager.Instantiate(prefab, entities);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];

                _manager.SetComponentData(entity, new Position { Value = new float3 { x = i } });

                _manager.SetComponentData(entity, new Rotation { Value = quaternion.identity });

                _manager.SetComponentData(entity, new MoveSpeedComponent { Speed = 1 });


            }
            entities.Dispose();

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                instanceCount *= 2;
                Spawn();
            }


            //JobHandle.ScheduleBatchedJobs();
        }

        void OnDisable()
        {
            TransformAccessArray.Dispose();
        }
    }
}
