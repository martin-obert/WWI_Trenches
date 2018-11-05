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
        public AudioClip Clip;
        public GameObject Cu;
        public GameObject UnitPartPrefab;
        public int UnitsCount = 10;
        private EntityManager _manager;
        public Mesh mesh;
        public Material material;
        public Material barMaterial;
        public Material barMaterial2;
        public AudioSource AudioSource;

        void Start()
        {
            _manager = World.Active.GetOrCreateManager<EntityManager>();

            HealthDrawSystem.BarMeshMaterial = barMaterial;
            HealthDrawSystem.BarMeshMaterial2 = barMaterial2;
            NavigationSystem.AudioClip = Clip;
            NavigationSystem.Audio = AudioSource;

            var enemiesArray = new NativeArray<Entity>(UnitsCount, Allocator.Temp);
            _manager.Instantiate(UnitPartPrefab, enemiesArray);
            for (int i = 0; i < UnitsCount; i++)
            {
                var part = enemiesArray[i];
                _manager.AddSharedComponentData(part, new Group { Id = GroupId });
                _manager.AddSharedComponentData(part, new UnitRenderer
                {
                    Mesh = mesh,
                    Material = material
                }); ;
                _manager.AddComponentData(part, new LocalToWorld { Value = UnitPartPrefab.transform.localToWorldMatrix });
                _manager.AddComponentData(part, new Position { Value = new Vector3(UnityEngine.Random.Range(-150, 150), 0, UnityEngine.Random.Range(-150, 150)) });
                _manager.AddComponentData(part, new Rotation { Value = Quaternion.identity });
                _manager.AddComponentData(part, new Speed { Value = .1f });
                _manager.AddComponentData(part, new Health { Value = 100, Max = 100 });
                _manager.AddComponentData(part, new Enemy { });

                _manager.AddComponentData(part, new XnaBoundingSphere { Radius = 2f, Offset = new float3(0, 1.7f, 0) });
            }
            GroupId++;

            enemiesArray.Dispose();
        }

        private int GroupId = 1;

        void Update()
        {
            if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
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

                var unitDestination = Vector3.zero;

                for (int i = 0; i < unitParts.Length; i++)
                {
                    var part = unitParts[i];
                    _manager.AddSharedComponentData(part, new Group { Id = GroupId });
                    _manager.AddSharedComponentData(part, new UnitRenderer
                    {
                        Mesh = mesh,
                        Material = material
                    });

                    _manager.AddComponentData(part, new LocalToWorld { Value = UnitPartPrefab.transform.localToWorldMatrix });

                    _manager.AddComponentData(part, new Position { Value = new Vector3(0, 0, UnityEngine.Random.Range(0, UnitsCount)) });

                    _manager.AddComponentData(part, new Rotation { Value = Quaternion.identity });

                    _manager.AddComponentData(part, new Destination { Value = unitDestination });

                    _manager.AddComponentData(part, new Speed { Value = .1f });

                    _manager.AddComponentData(part, new Health { Value = 100, Max = 100 });

                    _manager.AddComponentData(part, new WeaponPrototype { Range = 20 });

                    //_manager.AddComponentData(part, new Selected {  });

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
            _group = GetComponentGroup(typeof(Selected), typeof(Position), typeof(Health));
        }
        public static Material BarMeshMaterial;

        private float3 Size = new float3(15, 1, 0);



        protected override void OnUpdate()
        {
            if (!Input.GetKey(KeyCode.LeftAlt)) return;


            var positions = _group.GetComponentDataArray<Position>();

            var healths = _group.GetComponentDataArray<Health>();

            var availibleHealth = new Matrix4x4[math.min(positions.Length, healths.Length)];

            var unavailibleHealth = new Matrix4x4[math.min(positions.Length, healths.Length)];

            for (int i = 0; i < positions.Length && i < healths.Length; i++)
            {
                var position = positions[i];

                var health = healths[i];

                var healthAvailibleClamped = (health.Value / health.Max) * Size.x;
                var healthUnAvailibleClamped = Size.x - healthAvailibleClamped;


                availibleHealth[i] = CreateHealtMatrix(position.Value - Size / 2, healthAvailibleClamped, 5);

                unavailibleHealth[i] = CreateHealtMatrix(position.Value - Size / 2, -healthUnAvailibleClamped, 5, Size.x);
            }

            Graphics.DrawMeshInstanced(_barMesh, 0, BarMeshMaterial, availibleHealth);
            Graphics.DrawMeshInstanced(_barMesh, 0, BarMeshMaterial2, unavailibleHealth);

        }

        private Vector3 GetLookNorm(float3 position)
        {
            return ((Vector3)position - Camera.main.transform.position).normalized;
        }
        private Matrix4x4 CreateHealtMatrix(Vector3 position, float health, float upOffset, float? leftOffset = null)
        {
            var vector = GetLookNorm(position);
            return Matrix4x4.TRS(position + Vector3.up * upOffset,
                  Quaternion.LookRotation(vector),
                  Vector3.one) * (leftOffset.HasValue ? Matrix4x4.Translate(Vector3.right * leftOffset.Value) : Matrix4x4.identity) * Matrix4x4.Scale(new Vector3(health, Size.y));
        }

    }

    //public class HealthDecayTestSystem : ComponentSystem
    //{
    //    struct Data
    //    {
    //        public ComponentDataArray<Health> Healths;
    //        public readonly int Length;
    //    }

    //    [Inject] private Data _data;
    //    protected override void OnUpdate()
    //    {
    //        for (int i = 0; i < _data.Length; i++)
    //        {
    //            var health = _data.Healths[i];
    //            health.Value -= 6 * Time.deltaTime;
    //            health.Value = health.Value < 0 ? 100 : health.Value;
    //            _data.Healths[i] = health;
    //        }
    //    }
    //}


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
            _group = GetComponentGroup(typeof(Selected), typeof(UnitRenderer), typeof(LocalToWorld));
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

    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    [ExecuteInEditMode]
    public class SelectedUnitRenderSystem : ComponentSystem
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
            _group = GetComponentGroup(ComponentType.Subtractive<Selected>(), typeof(UnitRenderer), typeof(LocalToWorld));
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
    public class UnSelectionSystem : JobComponentSystem
    {
        struct Data
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<Selected> Selections;
            [ReadOnly] public EntityArray Entities;
        }

        struct UnSelectionJob : IJob
        {
            [ReadOnly] public ComponentDataArray<Selected> Selected;

            [ReadOnly] public EntityArray Entities;

            [ReadOnly] public EntityCommandBuffer CommandBuffer;

            public int Lenght;

            public void Execute()
            {
                for (int i = 0; i < Lenght; i++)
                {

                    CommandBuffer.RemoveComponent<Selected>(Entities[i]);
                }
            }
        }

        [Inject] private Data _data;

        [Inject] private EndFrameBarrier _selectBarrier;

        private JobHandle handle;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                handle.Complete();

                var job = new UnSelectionJob
                {
                    CommandBuffer = _selectBarrier.CreateCommandBuffer(),
                    Entities = _data.Entities,
                    Selected = _data.Selections,
                    Lenght = _data.Length,
                };

                handle = job.Schedule(inputDeps);
                handle.Complete();
            }

            return inputDeps;
        }
    }

    [UpdateAfter(typeof(UnSelectionSystem))]
    public class SelectionSystem : JobComponentSystem
    {
        struct Data
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Ranges;
            [ReadOnly] public SharedComponentDataArray<Group> Group;
            [ReadOnly] public SubtractiveComponent<Enemy> Selections;
            [ReadOnly] public EntityArray Entities;
        }

        struct RayCastJob : IJob
        {
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Spheres;

            [ReadOnly] public SharedComponentDataArray<Group> Groups;

            [WriteOnly] public NativeList<int> SelectedUnit;

            public XnaRay Ray;

            public int SelectionMode;

            public void Execute()
            {
                for (int i = 0; i < Spheres.Length; i++)
                {
                    var sphere = Spheres[i];

                    if (Ray.Intersects(sphere) != null)
                    {
                        //Todo: enums
                        if (SelectionMode == 0)
                        {
                            for (int j = 0; j < Groups.Length; j++)
                            {
                                if (Groups[j].Id == Groups[i].Id)
                                {
                                    SelectedUnit.Add(j);

                                }
                            }
                        }
                        else
                        {
                            SelectedUnit.Add(i);
                        }
                        return;
                    }
                }
            }
        }

        struct SelectJob : IJob
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public EntityCommandBuffer CommandBuffer;

            public int Lenght;

            [ReadOnly] public NativeList<int> SelectedUnits;

            public void Execute()
            {
                for (int i = 0; i < SelectedUnits.Length; i++)
                {
                    CommandBuffer.AddComponent(Entities[SelectedUnits[i]], new Selected());
                }
            }
        }

        [Inject] private Data _data;

        [Inject] private EndFrameBarrier _selectBarrier;

        private JobHandle handle;

        //private ComponentGroup _group;

        //protected override void OnCreateManager()
        //{
        //    _group = GetComponentGroup(typeof(Group), typeof(Selected));
        //}

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                handle.Complete();

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                var xnaRay = new XnaRay
                {
                    Position = ray.origin,
                    Direction = ray.direction
                };

                var selectedUnits = new NativeList<int>(50, Allocator.Temp);

                var selectionMode = Input.GetKey(KeyCode.LeftControl) ? 1 : 0;

                var rayCastHandle = new RayCastJob
                {
                    Ray = xnaRay,
                    Groups = _data.Group,
                    Spheres = _data.Ranges,
                    SelectionMode = selectionMode,
                    SelectedUnit = selectedUnits
                }.Schedule(inputDeps);



                handle = new SelectJob
                {
                    SelectedUnits = selectedUnits,
                    Entities = _data.Entities,
                    Lenght = _data.Length,
                    CommandBuffer = _selectBarrier.CreateCommandBuffer()
                }.Schedule(rayCastHandle);

                handle.Complete();

                selectedUnits.Dispose();
            }

            return inputDeps;
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

    [UpdateAfter(typeof(SelectionSystem))]
    public class NavigationSystem : ComponentSystem
    {
        struct Data
        {
            [ReadOnly] public ComponentDataArray<Selected> SelectedUnits;
            public ComponentDataArray<Destination> Destinations;
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Spheres;
            public readonly int Length;
        }


        [Inject] private Data _data;

        protected override void OnCreateManager()
        {

        }

        private int _rows = 5;
        private int _cols = 5;
        public static AudioSource Audio;
        public static AudioClip AudioClip;
        protected override void OnUpdate()
        {
            if (!Input.GetMouseButtonDown((int)MouseButton.RightMouse))
            {
                return;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }

            float3 click = hit.point;

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    var flatted = i * _rows + j;
                    if (flatted >= _data.Length)
                        goto End;

                    var destination = _data.Destinations[flatted];
                    var sphere = _data.Spheres[flatted];
                    destination.Value = click + new float3(j + sphere.Radius, 0, i);
                    _data.Destinations[flatted] = destination;
                }
            }

            End:;
        }
    }

    public class MoveSystem : ComponentSystem
    {
        struct Data
        {
            public ComponentDataArray<Position> Positions;
            [ReadOnly] public ComponentDataArray<Speed> Speeds;
            [ReadOnly] public ComponentDataArray<Destination> Destinations;
            public readonly int Length;
        }

        [Inject] private Data _data;
        private ComponentGroup _componentGroup;

        struct MoveJob : IJob
        {
            public ComponentDataArray<Position> Positions;

            [ReadOnly] public ComponentDataArray<Destination> Destinations;
            [ReadOnly] public ComponentDataArray<Speed> Speeds;

            public int Length;

            public void Execute()
            {
                for (int i = 0; i < Length; i++)
                {

                    var position = Positions[i];
                    position.Value += math.normalize(Destinations[i].Value - position.Value) * Speeds[i].Value;
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
                Destinations = _data.Destinations,
                Speeds = _data.Speeds,
                Length = _data.Length
            };

            handle = job.Schedule();
            handle.Complete();
        }
    }

    public class AttackSystem : ComponentSystem
    {

        struct Data
        {

        }

        protected override void OnUpdate()
        {
        }
    }

    public class TargetingSystem : ComponentSystem
    {
        struct Data
        {
            [ReadOnly] public ComponentDataArray<Enemy> Enemies;
            [ReadOnly] public SharedComponentDataArray<Group> Groups;
            [ReadOnly] public ComponentDataArray<XnaBoundingSphere> Spheres;
            public readonly int Length;
        }

        [Inject] private Data _data;
        
        protected override void OnUpdate()
        {
            if (!Input.GetKey(KeyCode.A) || !Input.GetMouseButtonDown((int)MouseButton.RightMouse))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var xnaRay = new XnaRay { Position = ray.origin, Direction = ray.direction };
            for (int i = 0; i < _data.Length; i++)
            {
                var shpere = _data.Spheres[i];
                var enemy = _data.Enemies[i];
                if (xnaRay.Intersects(shpere) != null)
                {
                    Debug.Log("Attack " + enemy);
                    //Todo: set attack
                    return;
                }
            }

        }
    }

}
