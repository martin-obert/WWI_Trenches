using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

namespace Assets.JobTests
{
    public class SystemsTests : MonoBehaviour
    {
        public GameObject UnitPrefab;
        public GameObject UnitPartPrefab;
        public int UnitsCount = 10;
        private EntityManager _manager;
        void Start()
        {
            _manager = World.Active.GetOrCreateManager<EntityManager>();

            for (int i = 0; i < UnitsCount; i++)
            {
                var unitParts = new NativeArray<Entity>(9, Allocator.Temp);

                _manager.Instantiate(UnitPartPrefab, unitParts);

                for (int j = 0; j < unitParts.Length; j++)
                {
                    var part = unitParts[j];
                    _manager.AddSharedComponentData(part, new Unit { Id = i });
                    _manager.AddComponentData(part, new Position { Value = Vector3.forward * j });
                    _manager.AddComponentData(part, new Rotation { Value = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 359), Vector3.up) });
                    _manager.AddComponentData(part, new Destination { Value = new Vector3(UnityEngine.Random.Range(0, 359), UnityEngine.Random.Range(0, 359), UnityEngine.Random.Range(0, 359)) });
                }

                unitParts.Dispose();
            }

        }


    }

    public class NavigationSystem : ComponentSystem
    {
        struct Data
        {
           [ReadOnly]  public SharedComponentDataArray<Unit> Units;
            public ComponentDataArray<Destination> Destinations;
            public readonly int Length;
        }


        [Inject] private Data _data;

        private List<Unit> _units = new List<Unit>(1);

        private ComponentGroup _group;

        protected override void OnCreateManager()
        {
            _group = GetComponentGroup(typeof(Unit), typeof(Destination));

        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EntityManager.GetAllUniqueSharedComponentData(_units);

                foreach (var unit in _units)
                {
                    _group.SetFilter(new Unit { Id = unit.Id });

                    var data = _group.GetComponentDataArray<Destination>();

                    var newdestination = new Vector3(UnityEngine.Random.Range(0, 359),
                        UnityEngine.Random.Range(0, 359), UnityEngine.Random.Range(0, 359));
                    
                    for (int i = 0; i < data.Length; i++)
                    {
                        var destination = data[i];
                        destination.Value = newdestination;
                        data[i] = destination;
                    }
                }
            }

        }
    }

    public class MoveSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<Position> Positions;
            [ReadOnly] public ComponentDataArray<Rotation> Rotations;
            [ReadOnly] public ComponentDataArray<Destination> Destinations;
            public readonly int Length;
        }

        [Inject] private Data _data;
        private ComponentGroup _componentGroup;
        //protected override void OnCreateManager()
        //{
        //    _componentGroup = GetComponentGroup( typeof(Position), typeof(Rotation));
        //}

        struct MoveJob : IJob
        {
            public ComponentDataArray<Position> Positions;

            [ReadOnly] public ComponentDataArray<Rotation> Rotations;
            [ReadOnly] public ComponentDataArray<Destination> Destinations;

            public int Length;

            public void Execute()
            {
                for (int i = 0; i < Length; i++)
                {
                    var position = Positions[i];
                    position.Value += math.normalize(Destinations[i].Value - position.Value) * .05f;
                    Positions[i] = position;
                }
            }
        }

        private JobHandle handle;

        protected override void OnUpdate()
        {

            var job = new MoveJob
            {
                Positions = _data.Positions,
                Rotations = _data.Rotations,
                Destinations = _data.Destinations,
                Length = _data.Length
            };

            handle = job.Schedule();
            handle.Complete();
        }
    }

}
