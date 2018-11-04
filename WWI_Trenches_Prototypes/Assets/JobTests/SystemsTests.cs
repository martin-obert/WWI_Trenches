using System;
using System.Collections.Generic;
using Assets.XnaLegacy;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Experimental.UIElements;

namespace Assets.JobTests
{
    public class SystemsTests : MonoBehaviour
    {
        private Texture2D _greenTexture;

        public GameObject Cu;
        public GameObject UnitPartPrefab;
        public int UnitsCount = 10;
        private EntityManager _manager;
        public Mesh mesh;
        public Material material;
        public Material barMaterial;
        public Material barMaterial2;
        void Start()
        {
            _manager = World.Active.GetOrCreateManager<EntityManager>();

            HealthDrawSystem.BarMeshMaterial = barMaterial;
            HealthDrawSystem.BarMeshMaterial2 = barMaterial2;
        }

        private int GroupId = 1;

        void Update()
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Cu.transform.position
                        = hit.point;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))

            {

                var unitParts = new NativeArray<Entity>(UnitsCount, Allocator.Temp);

                _manager.Instantiate(UnitPartPrefab, unitParts);

                var unitDestination = new Vector3(UnityEngine.Random.Range(0, 60), 0,
                    UnityEngine.Random.Range(0, 60));

                for (int i = 0; i < unitParts.Length; i++)
                {
                    var part = unitParts[i];
                    _manager.AddSharedComponentData(part, new Group { Id = GroupId });
                    _manager.AddSharedComponentData(part, new UnitRenderer
                    {
                        Mesh = mesh,
                        Material = material
                    }); ;
                    _manager.AddComponentData(part, new LocalToWorld { Value = UnitPartPrefab.transform.localToWorldMatrix });
                    _manager.AddComponentData(part, new Position { Value = new Vector3(0, 0, UnityEngine.Random.Range(0, UnitsCount)) });
                    _manager.AddComponentData(part, new Rotation { Value = Quaternion.identity });
                    _manager.AddComponentData(part, new Destination { Value = unitDestination });
                    _manager.AddComponentData(part, new Health { Value = 100 });

                    _manager.AddComponentData(part, new XnaBoundingSphere { Radius = 2f, Offset = new float3(0, 1.7f, 0) });

                }

                unitParts.Dispose();

                GroupId++;
            }
        }

    }

    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    [ExecuteInEditMode]
    public class HealthDrawSystem : ComponentSystem
    {
        private Matrix4x4 _worldToScreen;
        private Mesh _barMesh;
        private ComponentGroup _group;
        public static Material BarMeshMaterial2;

        protected override void OnCreateManager()
        {
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
            _group = GetComponentGroup(typeof(Group), typeof(Position), typeof(Health));
        }

        //struct Data
        //{
        //    [ReadOnly] public ComponentDataArray<Health> Health;
        //    [ReadOnly] public ComponentDataArray<Position> Positions;
        //    [ReadOnly] public SharedComponentDataArray<Group> Groups;
        //    public readonly int Length;
        //}

        public static Material BarMeshMaterial;



        //[Inject] private Data _data;

        private List<Group> groupsUniqed = new List<Group>(10);

        protected override void OnUpdate()
        {
            //EntityManager.GetAllUniqueSharedComponentData(groupsUniqed);
            //for (int i = 0; i < groupsUniqed.Count; i++)
            //{
            //    if(groupsUniqed[i].Id != SelectionSystem.SelectedGroup)
            //        continue;

            //}
            if (SelectionSystem.SelectedGroup <= 0)
                return;

            _group.SetFilter(new Group { Id = SelectionSystem.SelectedGroup });
            var positions = _group.GetComponentDataArray<Position>();
            var healths = _group.GetComponentDataArray<Health>();
            var availibleHealth = new Matrix4x4[math.min(positions.Length, healths.Length)];
            var unavailibleHealth = new Matrix4x4[math.min(positions.Length, healths.Length)];

            for (int i = 0; i < positions.Length && i < healths.Length; i++)
            {
                var position = positions[i];
                var health = healths[i];
                availibleHealth[i] = CreateHealtMatrix(position.Value, health.Value, 5);

                unavailibleHealth[i] = CreateHealtMatrix(position.Value, 100 - health.Value, 5, 100 - health.Value);
            }

            //var projection = Camera.main.worldToCameraMatrix * Camera.main.projectionMatrix.inverse;
            Graphics.DrawMeshInstanced(_barMesh, 0, BarMeshMaterial, availibleHealth);
            Graphics.DrawMeshInstanced(_barMesh, 0, BarMeshMaterial2, unavailibleHealth);

        }

        private Vector3 GetLook(float3 position)
        {
            return ((Vector3)position - Camera.main.transform.position).normalized;
        }
        private Matrix4x4 CreateHealtMatrix(Vector3 position, float health, float upOffset, float? leftOffset = null)
        {
            var vector = GetLook(position);
            return Matrix4x4.TRS(position + (Vector3.up * upOffset),
                  Quaternion.LookRotation(vector),
                  Vector3.one) * (leftOffset.HasValue ? Matrix4x4.Translate(vector * leftOffset.Value) : Matrix4x4.identity) * Matrix4x4.Scale(new Vector3(health, 1));
        }

    }

    public class HealthDecayTestSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<Health> Healths;
            public readonly int Length;
        }

        [Inject] private Data _data;
        protected override void OnUpdate()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                var health = _data.Healths[i];
                health.Value -= 6 * Time.deltaTime;
                health.Value = health.Value < 0 ? 100 : health.Value;
                _data.Healths[i] = health;
            }
        }
    }


    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    [ExecuteInEditMode]
    public class UnitRenderSystem : ComponentSystem
    {

        private List<UnitRenderer> _instancedRenderers = new List<UnitRenderer>(10);

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

        private ComponentGroup _group;

        protected override void OnCreateManager()
        {
            _group = GetComponentGroup(typeof(Group), typeof(UnitRenderer), typeof(LocalToWorld));
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        private readonly Matrix4x4[] _matrices = new Matrix4x4[1023];

        private MaterialPropertyBlock _materialPropertyBlock;

        protected override void OnUpdate()
        {

            EntityManager.GetAllUniqueSharedComponentData(_instancedRenderers);

            for (var i = 0; i < _instancedRenderers.Count; i++)
            {
                var instancedRenderer = _instancedRenderers[i];

                if (instancedRenderer.Mesh == null)
                    continue;

                _group.ResetFilter();

                _materialPropertyBlock.SetFloat("_Selection_Color", 0);

                RenderGroup(instancedRenderer);

                _group.SetFilter(new Group { Id = SelectionSystem.SelectedGroup });

                _materialPropertyBlock.SetFloat("_Selection_Color", 1);

                RenderGroup(instancedRenderer);
            }
            _instancedRenderers.Clear();
        }

        private void RenderGroup(UnitRenderer instancedRenderer)
        {
            var position = _group.GetComponentDataArray<LocalToWorld>();


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

    public class SelectionSystem : ComponentSystem
    {
        struct Data
        {
            public readonly int Length;
            [ReadOnly] public SharedComponentDataArray<Group> Units;
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Ranges;
        }

        struct FilterJob : IJob
        {
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Ranges;
            [ReadOnly] public SharedComponentDataArray<Group> Group;

            [WriteOnly] public NativeArray<int> Result;

            public int Lenght;

            public XnaRay Ray;

            public void Execute()
            {
                for (int i = 0; i < Lenght; i++)
                {
                    var range = Ranges[i];

                    var sphere = range;
                    if (Ray.Intersects(sphere) != null)
                    {
                        Result[0] = Group[i].Id;
                        return;
                    }
                }
            }
        }

        [Inject] private Data _data;
        public static int SelectedGroup;
        public JobHandle handle;
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var xnaRay = new XnaRay
                {
                    Position = ray.origin,
                    Direction = ray.direction
                };
                handle.Complete();

                var result = new NativeArray<int>(1, Allocator.TempJob);

                var job = new FilterJob
                {
                    Result = result,
                    Group = _data.Units,
                    Ranges = _data.Ranges,
                    Lenght = _data.Length,
                    Ray = xnaRay
                };

                handle = job.Schedule();

                handle.Complete();

                SelectedGroup = result[0];

                Debug.Log("Selected " + result[0]);

                result.Dispose();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectedGroup = 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectedGroup = 2;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectedGroup = 3;
            }

        }
    }

    public class SpheresSyncSystem : JobComponentSystem
    {
        struct Data
        {
            public readonly int Length;
            public ComponentDataArray<XnaBoundingSphere> Spheres;
            public ComponentDataArray<Position> Positions;
        }

        struct UpdateJob : IJobParallelFor
        {
            public ComponentDataArray<XnaBoundingSphere> Spheres;
            [ReadOnly] public ComponentDataArray<Position> Positions;
            public void Execute(int index)
            {

                var sphere = Spheres[index];
                sphere.Position = Positions[index].Value + sphere.Offset;
                Spheres[index] = sphere;
            }
        }

        [Inject] private Data _data;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //inputDeps.Complete();
            var job = new UpdateJob
            {
                Positions = _data.Positions,
                Spheres = _data.Spheres
            };

            return job.Schedule(_data.Length, 64, inputDeps);
        }
    }


    //public class NavigationSystem : ComponentSystem
    //{
    //    struct Data
    //    {
    //        [ReadOnly] public SharedComponentDataArray<UnitPart> Units;
    //        public ComponentDataArray<Destination> Destinations;
    //        public readonly int Length;
    //    }


    //    [Inject] private Data _data;

    //    private List<UnitPart> _units = new List<UnitPart>(10);

    //    private ComponentGroup _group;

    //    protected override void OnCreateManager()
    //    {
    //        _group = GetComponentGroup(typeof(UnitPart), typeof(Destination));

    //    }

    //    protected override void OnUpdate()
    //    {
    //        if (Input.GetKeyDown(KeyCode.Escape))
    //        {
    //            EntityManager.GetAllUniqueSharedComponentData(_units);

    //            foreach (var unit in _units)
    //            {
    //                _group.SetFilter(new UnitPart { UnitId = unit.UnitId });

    //                var data = _group.GetComponentDataArray<Destination>();

    //                var newdestination = new Vector3(UnityEngine.Random.Range(0, 359),
    //                    UnityEngine.Random.Range(0, 359), UnityEngine.Random.Range(0, 359));

    //                for (int i = 0; i < data.Length; i++)
    //                {
    //                    var destination = data[i];
    //                    destination.Radius = newdestination;
    //                    data[i] = destination;
    //                }
    //            }
    //        }

    //    }
    //}

    public class MoveSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<Position> Positions;
            public ComponentDataArray<Rotation> Rot;
            [ReadOnly] public ComponentDataArray<Destination> Destinations;
            public readonly int Length;
        }

        [Inject] private Data _data;
        private ComponentGroup _componentGroup;

        struct MoveJob : IJob
        {
            public ComponentDataArray<Position> Positions;
            public ComponentDataArray<Rotation> Rot;
            [ReadOnly] public ComponentDataArray<Destination> Destinations;

            public int Length;

            public void Execute()
            {
                for (int i = 0; i < Length; i++)
                {
                    var position = Positions[i];
                    position.Value += math.normalize((Destinations[i].Value) - position.Value) * .05f;
                    Positions[i] = position;

                    var rot = Rot[i];
                    rot.Value *= Quaternion.Euler(0, 1, 0);
                    Rot[i] = rot;
                }
            }
        }

        private JobHandle handle;

        protected override void OnUpdate()
        {

            var job = new MoveJob
            {
                Positions = _data.Positions,
                Destinations = _data.Destinations,
                Rot = _data.Rot,
                Length = _data.Length
            };

            handle = job.Schedule();
            handle.Complete();
        }
    }

}
