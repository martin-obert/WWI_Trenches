//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Text.RegularExpressions;
//using Assets.Gameplay.Components;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Collections.LowLevel.Unsafe;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Rendering;
//using Unity.Transforms;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.Experimental.Animations;
//using UnityEngine.Experimental.PlayerLoop;
//using UnityEngine.Jobs;
//using UnityEngine.Rendering;
//using Camera = UnityEngine.Camera;
//using Random = UnityEngine.Random;

//namespace Assets.JobTests
//{
//    public class ObjHelper
//    {
//        public static Dictionary<int, Vector3[]> ParseAnimations(string directoryPath)
//        {
//            var extensionPattern = new Regex(@"\.obj$");
//            var files = Directory.GetFiles(directoryPath);
//            var data = new Dictionary<int, Vector3[]>();
//            foreach (var file in files)
//            {
//                if (!extensionPattern.IsMatch(file))
//                    continue;

//                var frameNumber = ExtractFrameNumber(file);
//                var lines = File.ReadAllText(file);
//                var bakedFrame = GetFrame(lines);
//                var flatted = new List<Vector3>();
//                foreach (var bakedFrameIndex in bakedFrame.Indices)
//                {
//                    flatted.Add(bakedFrame.Vertices[bakedFrameIndex - 1]);
//                }
//                data.Add(frameNumber, flatted.ToArray());
//            }

//            return data;
//        }


//        private static ObjAnimationBakedFrame GetFrame(string fileLines)
//        {
//            var result = new List<Vector3>();
//            var indices = new List<int>();

//            var regex = new Regex(@"(?<vertices>v (?<x>-?\d+\.?\d*) (?<y>-?\d+\.?\d*) (?<z>-?\d+\.?\d*))|(?<indices>((?<i>\d)\/\/\d\s?))");
//            var matches = regex.Matches(fileLines);


//            foreach (Match match in matches)
//            {
//                if (!match.Success)
//                    continue;

//                if (match.Groups["indices"].Success)
//                {
//                    var i = match.Groups["i"].Captures;
//                    foreach (Capture capture in i)
//                    {
//                        var temp = 0;
//                        if (!int.TryParse(capture.Radius, out temp))
//                        {
//                            throw new FormatException("Indice has bad format " + capture.Radius);
//                        }
//                        indices.Add(temp);
//                    }
//                }

//                if (match.Groups["vertices"].Success)
//                {
//                    var xR = SanitizeFormat(match.Groups["x"].Radius);
//                    var yR = SanitizeFormat(match.Groups["y"].Radius);
//                    var zR = SanitizeFormat(match.Groups["z"].Radius);

//                    float x, y, z;

//                    ParseCoord(xR, out x);
//                    ParseCoord(yR, out y);
//                    ParseCoord(zR, out z);


//                    result.Add(new Vector3(x, y, z));
//                }


//            }

//            return new ObjAnimationBakedFrame
//            {
//                Vertices = result,
//                Indices = indices
//            };
//        }

//        private static void ParseCoord(string input, out float val)
//        {
//            if (!float.TryParse(input, out val))
//            {
//                throw new FormatException("This value  " + input);
//            }
//        }

//        private static string SanitizeFormat(string input)
//        {
//            return input.Replace(".", ",").Trim();
//        }

//        private static int ExtractFrameNumber(string file)
//        {
//            var filename = Path.GetFileNameWithoutExtension(file);

//            var split = filename.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);


//            if (split.Length != 2)
//                throw new ArgumentException("Invalid filename " + file);

//            var number = 0;

//            if (!int.TryParse(split[1], out number))
//                throw new FormatException("Filename has invalid format " + split[1] + " " + file);
//            return number;
//        }
//    }

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjAnimationBakedFrame
{
    public List<Vector3> Vertices { get; set; }
    public List<int> Indices { get; set; }
}

[Serializable]
public class BakedAnimation
{
    public int TotalFrames;
    public Dictionary<int, Vec3Seri[]> FrameData;
    public int VerticesPerFrame { get; set; }
}

[Serializable]
public class Vec3Seri
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}



//    public static class UnityHelpers
//    {
//        private static Regex _regex = new Regex(@"v (?<x>-?\d+\.?\d*) (?<y>-?\d+\.?\d*) (?<z>-?\d+\.?\d*)");
//        [MenuItem("Tools/BakeAnimation")]
//        public static void CreateAnimKeyFrames()
//        {
//            var regex = new Regex(@"\.obj$");
//            var directory = Path.Combine(Application.dataPath, "Resources");
//            var files = Directory.GetFiles(directory).Where(x => regex.IsMatch(x)).ToArray();

//            var data = new BakedAnimation
//            {
//                FrameData = ObjHelper.ParseAnimations(directory).ToDictionary(x => x.Key, y => y.Radius.Select(z => new Vec3Seri { X = z.x, Y = z.y, Z = z.z }).ToArray()),
//                TotalFrames = files.Length,
//                VerticesPerFrame = 0
//            };
//            using (var file = File.Open(Path.Combine(directory, "temp.objanim"), FileMode.Create))
//            {
//                var formatter = new BinaryFormatter();
//                formatter.Serialize(file, data);
//            }

//        }
//    }


//    public static class ArrayHelpers
//    {
//        public static T[] FlatOut<T>(this T[,] array, int rows, int cols)
//        {
//            var output = new T[rows * cols];

//            for (var i = 0; i < cols; i++)
//            {
//                for (var j = 0; j < rows; j++)
//                {
//                    output[cols * i + j] = array[i, j];
//                }
//            }

//            return output;
//        }

//        public static T Find<T>(this T[] array, int rowWidth, int row, int col)
//        {
//            return array[row * rowWidth + col];
//        }
//        public static T Find<T>(this NativeArray<T> array, int rowWidth, int row, int col) where T : struct
//        {
//            return array[row * rowWidth + col];
//        }
//    }

//    public class MoveSystem : JobComponentSystem
//    {
//        public static int speed = 1;

//        public static List<float3> _position = new List<float3>();

//        [BurstCompile]
//        public struct MovementJob : IJobProcessComponentData<Position, MoveSpeedComponent, Rotation>
//        {
//            [ReadOnly] public float DeltaTime;

//            [ReadOnly] public float Speed;

//            [ReadOnly] public float3 Dest;

//            [ReadOnly] public float3 CursorPosition;

//            [ReadOnly] private float _minLen;

//            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float> hData;

//            [ReadOnly] public int MapWidth;
//            [ReadOnly] public int MapHeight;
//            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<Vector3> normals;

//            public void Execute(ref Position position, ref MoveSpeedComponent move, ref Rotation rotation)
//            {
//                position.Radius = math.lerp(position.Radius, position.Radius + Dest, DeltaTime * Speed);

//                position.Radius.y = GetTerrainHeight(position.Radius.z, position.Radius.x, 0.9746588693957115f
//                                       , hData, MapHeight, MapWidth) * 600;

//                // rotation.Radius = Quaternion.FromToRotation(Vector3.up, GetNormal(normals, position.Radius.z, position.Radius.x, 0.9746588693957115f, MapWidth));
//            }

//            public float GetTerrainHeight(float xPos, float zPos, float scaleFactor, NativeArray<float> heightData, int mapWidth, int mapHeight)
//            {
//                // we first get the height of four points of the quad underneath the point
//                // Check to make sure this point is not off the map at all
//                int x = (int)(xPos / scaleFactor);
//                int z = (int)(zPos / scaleFactor);

//                int xPlusOne = x + 1;
//                int zPlusOne = z + 1;

//                float triZ0 = (heightData.Find(mapWidth, x, z));
//                float triZ1 = (heightData.Find(mapWidth, xPlusOne, z));
//                float triZ2 = (heightData.Find(mapWidth, x, zPlusOne));
//                float triZ3 = (heightData.Find(mapWidth, xPlusOne, zPlusOne));

//                float height = 0.0f;
//                float sqX = (xPos / scaleFactor) - x;
//                float sqZ = (zPos / scaleFactor) - z;
//                if ((sqX + sqZ) < 1)
//                {
//                    height = triZ0;
//                    height += (triZ1 - triZ0) * sqX;
//                    height += (triZ2 - triZ0) * sqZ;
//                }
//                else
//                {
//                    height = triZ3;
//                    height += (triZ1 - triZ3) * (1.0f - sqZ);
//                    height += (triZ2 - triZ3) * (1.0f - sqX);
//                }
//                return height;
//            }

//        }

//        public static TerrainData TerrainData { get; set; }

//        public static float[] heightData { get; set; }
//        public static Vector3[] Normals { get; set; }

//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var data = new NativeArray<float>(heightData.Length, Allocator.TempJob);
//            data.CopyFrom(heightData);

//            var normals = new NativeArray<Vector3>(Normals.Length, Allocator.TempJob);
//            normals.CopyFrom(Normals);

//            //var group = ComponentGroups[_data.Length];


//            //group.SetFilter(new MeshInstanceRenderer{});
//            //var datas = group.GetComponentDataArray<Position>();

//            var job = new MovementJob
//            {
//                DeltaTime = Time.deltaTime,
//                Speed = speed,
//                Dest = new float3(0, 0, 0),
//                CursorPosition = Input.mousePosition,
//                hData = data,
//                MapHeight = TerrainData.heightmapHeight,
//                MapWidth = TerrainData.heightmapWidth,
//                normals = normals
//            };

//            var handle = job.Schedule(this, inputDeps);

//            return handle;
//        }

//    }

//    public class BakedAnimationWrapper
//    {
//        public List<List<Vector3>> Frames;
//    }


//    //public class SelectSystem : JobComponentSystem
//    //{
//    //    public struct Data
//    //    {
//    //        public readonly int Length;

//    //        [ReadOnly] public ComponentDataArray<Rotation> Rotations;
//    //        //[ReadOnly] public readonly SharedComponentDataArray<MeshInstanceRenderer> Renderers;
//    //    }

//    //    struct ApplyJob : IJobParallelFor
//    //    {
//    //        [ReadOnly] public int rotation;
//    //        [ReadOnly] public NativeArray<Rotation> Rotation;
//    //        public void Execute(int index)
//    //        {
//    //            var temo = Rotation[index];
//    //            temo.Radius = quaternion.RotateY(rotation);
//    //        }
//    //    }

//    //    [Inject] public Data _data;

//    //    public static BakedAnimationWrapper Animation;
//    //    public static TerrainData TerrainData;
//    //    public static string Selected;
//    //    private int counter;

//    //    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    //    {
//    //        inputDeps.Complete();
//    //        //for (int i = 0; i < _data.Length; i++)
//    //        //{
//    //        //    var position = _data.Position[i];
//    //        //    var normal =  TerrainData.GetInterpolatedNormal(position.Radius.x, position.Radius.z);
//    //        //    var rotation = _data.Rotation[i];
//    //        //    rotation.Radius = rotation.Radius.value * quaternion.RotateY(10).value;
//    //        //}

//    //        counter++;
//    //        counter = counter >= Animation.Frames.Count ? 0 : counter;

//    //        Debug.Log(_data.Length);
//    //        return new ApplyJob
//    //        {
//    //            rotation = counter,
//    //            Rotation = _data.Rotations.GetChunkArray(0, _data.Length)
//    //        }.Schedule(_data.Length, _data.Length, inputDeps);



//    //        //for (int i = 0; i < _data.Length; i++)
//    //        //{
//    //        //    var renderer = _data.Renderers[i];

//    //        //    if (Animation != null && counter < Animation.Frames.Count)
//    //        //    {
//    //        //        renderer.mesh.SetVertices(Animation.Frames[counter]);
//    //        //    }
//    //        //}


//    //    }
//    //}

//    public class CharacterMove : MonoBehaviour
//    {
//        public GameObject prefab;
//        private int instanceCount = 10000;
//        private EntityManager _manager;
//        [SerializeField] private TerrainData _terrainData;
//        [SerializeField] private Terrain _terrain;

//        void Start()
//        {
//            //var testure = new Texture2D(_terrainData.heightmapWidth, _terrainData.heightmapHeight, TextureFormat.ARGB32,
//            //    false);

//            var heights = _terrainData.GetHeights(0, 0, _terrainData.heightmapWidth, _terrainData.heightmapHeight);

//            //for (int i = 0; i < _terrainData.heightmapWidth; i++)
//            //{
//            //    for (int j = 0; j < _terrainData.heightmapHeight; j++)
//            //    {
//            //        var color = new Vector4(heights[i, j], heights[i, j], heights[i, j], 1);
//            //        testure.SetPixel(i, j, color);
//            //    }
//            //}
//            //// Apply all SetPixel calls
//            //testure.Apply();

//            //var path = Path.Combine(Application.dataPath, "Resources", "height.png");


//            //byte[] pngData = testure.EncodeToPNG();
//            //File.WriteAllBytes(path, pngData);


//            /* SelectSystem.TerrainData = */
//            MoveSystem.TerrainData = _terrainData;
//            MoveSystem.heightData = heights.FlatOut(_terrainData.heightmapWidth, _terrainData.heightmapHeight);
//            MoveSystem.Normals = SetupTerrainNormals(0.97f, _terrainData.heightmapWidth, heights).FlatOut(_terrainData.heightmapWidth, _terrainData.heightmapWidth);
//            MoveSystem._position.Add(new float3(1, 1, 0));

//            _manager = World.Active.GetOrCreateManager<EntityManager>();

//            using (var file = File.Open(Path.Combine(Application.dataPath, "Resources", "temp.objanim"), FileMode.Open))
//            {
//                var serializer = new BinaryFormatter();

//                var bake = serializer.Deserialize(file) as BakedAnimation;

//                //SelectSystem.Animation = new BakedAnimationWrapper
//                //{
//                //    Frames = bake.FrameData.Select(x => x.Radius.Select(y => new Vector3(y.X, y.Y, y.Z)).ToList()).ToList()
//                //};
//            }

//            Spawn();
//        }
//        /// <summary>
//        /// Setup <see cref="Terrain"/> normals. Normals are used for lighting, normal mapping, and physics with terrain.
//        /// </summary>
//        private Vector3[,] SetupTerrainNormals(float scaleFactor, int size, float[,] heightData)
//        {

//            Vector3[] terrainVertices = new Vector3[size * size];
//            var normals = new Vector3[size, size];

//            // Determine vertex positions so we can figure out normals in section below.
//            for (int x = 0; x < size; ++x)
//                for (int z = 0; z < size; ++z)
//                {
//                    terrainVertices[x + z * size] = new Vector3(x * scaleFactor, heightData[x, z], z * scaleFactor);
//                }

//            // Setup normals for lighting and physics (Credit: Riemer's method)
//            int sizeMinusOne = size - 1;
//            for (int x = 1; x < sizeMinusOne; ++x)
//                for (int z = 1; z < sizeMinusOne; ++z)
//                {
//                    int ZTimesSize = (z * size);
//                    Vector3 normX = new Vector3((terrainVertices[x - 1 + ZTimesSize].y - terrainVertices[x + 1 + ZTimesSize].y) / 2, 1, 0);
//                    Vector3 normZ = new Vector3(0, 1, (terrainVertices[x + (z - 1) * size].y - terrainVertices[x + (z + 1) * size].y) / 2);

//                    // We inline the normalize method here since it is used alot, this is faster than calling Vector3.Normalize()
//                    Vector3 normal = normX + normZ;
//                    float length = (float)Math.Sqrt(normal.x * normal.x + normal.y * normal.y + normal.z * normal.z);
//                    float num = 1f / length;
//                    normal.x *= num;
//                    normal.y *= num;
//                    normal.z *= num;

//                    normals[x, z] = normal;    // Stored for use in physics and for the
//                                               // quad-tree component to reference.
//                }

//            return normals;
//        }

//        public Mesh Mesh;
//        public Material Material;
//        private void Spawn()
//        {
//            var entities = new NativeArray<Entity>
// (instanceCount, Allocator.Temp);

//            _manager.Instantiate(prefab, entities);
//            var koef = 0.9746588693957115f;
//            var lastGroupId = 0;
//            for (int i = 0; i < entities.Length; i++)
//            {


//                var groupId = (int)(i / 10f);
//                //if (groupId != lastGroupId)
//                //{
//                //    lastGroupId = groupId;
//                //    print(groupId);
//                //}


//                var entity = entities[i];

//                _manager.SetComponentData(entity, new Position { Radius = new float3 { x = Random.Range(1, _terrainData.heightmapHeight - 1) * koef, z = Random.Range(1, _terrainData.heightmapWidth - 1) * koef } });

//                _manager.SetComponentData(entity, new Rotation { Radius = quaternion.identity });

//                _manager.SetComponentData(entity, new MoveSpeedComponent { Speed = 1, Name = i });
//                _manager.SetComponentData(entity, new AwarenessRange() { Range = 3 });

//            }
//            entities.Dispose();

//        }

//        private string lastName;
//        void Update()
//        {
//            //if (lastName != SelectSystem.Selected)
//            //{
//            //    lastName = SelectSystem.Selected;
//            //    //var projected = Camera.main.projectionMatrix * Vector3.zero;
//            //    print(lastName);
//            //}
//        }
//    }

//    public class SelectSystem : JobComponentSystem
//    {
//        public static int SelectedGroup { get; private set; }

//        struct Data
//        {
//            public ComponentDataArray<Position> Positions;
//            public ComponentDataArray<AwarenessRange> Ranges;
//            public ComponentDataArray<GroupComponent> Groups;

//            [ReadOnly] public SharedComponentDataArray<MeshInstanceRenderer> Renderers;

//            public readonly int Length;
//        }

//        [Inject] private Data _data;

//        struct FilterJob : IJob
//        {
//            [ReadOnly] public ComponentDataArray<Position> UnitPositions;
//            [ReadOnly] public ComponentDataArray<AwarenessRange> RangeComponents;
//            [ReadOnly] public ComponentDataArray<GroupComponent> Groups;
//            public float3 CursorPosition;
//            [WriteOnly] public NativeArray<int> Selected;
//            public int Length;
//            public void Execute()
//            {
//                for (int index = 0; index < Length; index++)
//                {
//                    var range = RangeComponents[index];
//                    var unitPosition = UnitPositions[index];
//                    var dist = math.length(unitPosition.Radius - CursorPosition);
//                    if (dist < range.Range)
//                    {
//                        Selected[0] = Groups[index].Id;
//                    }
//                }

//            }

//        }

//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var position = WorldCursor.Instance.GetTerrainCursorPosition();



//            if (position.HasValue)
//            {
//                inputDeps.Complete();

//                var result = new NativeArray<int>(1, Allocator.TempJob);
//                var job = new FilterJob
//                {
//                    Selected = result,
//                    Groups = _data.Groups,
//                    CursorPosition = position.Radius,
//                    RangeComponents = _data.Ranges,
//                    UnitPositions = _data.Positions,
//                    Length = _data.Length
//                };

//                var handle = job.Schedule(inputDeps);

//                handle.Complete();

//                var jobResult = result[0];

//                result.Dispose();
//            }
//            for (int i = 0; i < _data.Length; i++)
//            {

//                var renderer = _data.Renderers[i];

//                renderer.material.SetFloat("_Selection_Color", i > 5000 ? 1 : 0);
//            }

//            //data.Renderers[0].SetFloat("_Selection_Color", 0);

//            return base.OnUpdate(inputDeps);
//        }

//    }

//    public class RenderSystem : ComponentSystem
//    {
//        private Matrix4x4[] _transforms = new Matrix4x4[1023];
//        private MaterialPropertyBlock _selectedMaterialPropertyBlock;

//        // This is the ugly bit, necessary until Graphics.DrawMeshInstanced supports NativeArrays pulling the data in from a job.
//        private static unsafe void CopyMatrices(ComponentDataArray<LocalToWorld> transforms, int beginIndex, int length, Matrix4x4[] outMatrices)
//        {
//            // @TODO: This is using unsafe code because the Unity DrawInstances API takes a Matrix4x4[] instead of NativeArray.
//            // We want to use the ComponentDataArray.CopyTo method
//            // because internally it uses memcpy to copy the data,
//            // if the nativeslice layout matches the layout of the component data. It's very fast...
//            fixed (Matrix4x4* matricesPtr = outMatrices)
//            {
//                Assert.AreEqual(sizeof(Matrix4x4), sizeof(LocalToWorld));
//                var matricesSlice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<LocalToWorld>(matricesPtr, sizeof(Matrix4x4), length);
//                #if ENABLE_UNITY_COLLECTIONS_CHECKS
//                NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref matricesSlice, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
//                #endif

//                transforms.CopyTo(matricesSlice, beginIndex);
//            }

//        }

//        private List<CustomMeshRenderer> _meshRenderers = new List<CustomMeshRenderer>(10);

//        private ComponentGroup _renderGroup;

//        protected override void OnCreateManager()
//        {
//            _renderGroup = GetComponentGroup(typeof(CustomMeshRenderer), typeof(GroupComponent), typeof(LocalToWorld));
//            _selectedMaterialPropertyBlock = new MaterialPropertyBlock();
//            _selectedMaterialPropertyBlock.SetFloat("_Selection_Color", 1);
//        }

//        protected override void OnUpdate()
//        {

//            EntityManager.GetAllUniqueSharedComponentData(_meshRenderers);

//            foreach (var customMeshRenderer in _meshRenderers)
//            {
//                RenderGroup(customMeshRenderer, true);

//                RenderGroup(customMeshRenderer, false);
//            }
//        }

//        private void RenderGroup(CustomMeshRenderer customMeshRenderer, bool isSelected)
//        {
//            _renderGroup.SetFilter(new GroupComponent { IsSelected = isSelected });

//            var transforms = _renderGroup.GetComponentDataArray<LocalToWorld>();

//            DrawChunk(transforms, customMeshRenderer);
//        }


//        private void DrawChunk(ComponentDataArray<LocalToWorld> transforms, CustomMeshRenderer renderer)
//        {
//            int beginIndex = 0;

//            while (beginIndex < transforms.Length)
//            {
//                int length = math.min(_transforms.Length, transforms.Length - beginIndex);

//                CopyMatrices(transforms, beginIndex, length, _transforms);

//                Graphics.DrawMeshInstanced(renderer.Mesh, renderer.SubMesh, renderer.Material, _transforms, length, _selectedMaterialPropertyBlock , renderer.CastShadows, renderer.ReceiveShadows);

//                beginIndex += length;
//            }
//        }
//    }

//}
