using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using UnityEngine.Rendering;
using Camera = UnityEngine.Camera;
using Random = UnityEngine.Random;

namespace Assets.JobTests
{
    public class ObjHelper
    {
        public static Dictionary<int, Vector3[]> ParseAnimations(string directoryPath)
        {
            var extensionPattern = new Regex(@"\.obj$");
            var files = Directory.GetFiles(directoryPath);
            var data = new Dictionary<int, Vector3[]>();
            foreach (var file in files)
            {
                if (!extensionPattern.IsMatch(file))
                    continue;

                var frameNumber = ExtractFrameNumber(file);
                var lines = File.ReadAllText(file);
                var bakedFrame = GetFrame(lines);
                var flatted = new List<Vector3>();
                foreach (var bakedFrameIndex in bakedFrame.Indices)
                {
                    flatted.Add(bakedFrame.Vertices[bakedFrameIndex - 1]);
                }
                data.Add(frameNumber, flatted.ToArray());
            }

            return data;
        }


        private static ObjAnimationBakedFrame GetFrame(string fileLines)
        {
            var result = new List<Vector3>();
            var indices = new List<int>();

            var regex = new Regex(@"(?<vertices>v (?<x>-?\d+\.?\d*) (?<y>-?\d+\.?\d*) (?<z>-?\d+\.?\d*))|(?<indices>((?<i>\d)\/\/\d\s?))");
            var matches = regex.Matches(fileLines);


            foreach (Match match in matches)
            {
                if (!match.Success)
                    continue;

                if (match.Groups["indices"].Success)
                {
                    var i = match.Groups["i"].Captures;
                    foreach (Capture capture in i)
                    {
                        var temp = 0;
                        if (!int.TryParse(capture.Value, out temp))
                        {
                            throw new FormatException("Indice has bad format " + capture.Value);
                        }
                        indices.Add(temp);
                    }
                }

                if (match.Groups["vertices"].Success)
                {
                    var xR = SanitizeFormat(match.Groups["x"].Value);
                    var yR = SanitizeFormat(match.Groups["y"].Value);
                    var zR = SanitizeFormat(match.Groups["z"].Value);

                    float x, y, z;

                    ParseCoord(xR, out x);
                    ParseCoord(yR, out y);
                    ParseCoord(zR, out z);


                    result.Add(new Vector3(x, y, z));
                }


            }

            return new ObjAnimationBakedFrame
            {
                Vertices = result,
                Indices = indices
            };
        }

        private static void ParseCoord(string input, out float val)
        {
            if (!float.TryParse(input, out val))
            {
                throw new FormatException("This value  " + input);
            }
        }

        private static string SanitizeFormat(string input)
        {
            return input.Replace(".", ",").Trim();
        }

        private static int ExtractFrameNumber(string file)
        {
            var filename = Path.GetFileNameWithoutExtension(file);

            var split = filename.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);


            if (split.Length != 2)
                throw new ArgumentException("Invalid filename " + file);

            var number = 0;

            if (!int.TryParse(split[1], out number))
                throw new FormatException("Filename has invalid format " + split[1] + " " + file);
            return number;
        }
    }

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



    public static class UnityHelpers
    {
        private static Regex _regex = new Regex(@"v (?<x>-?\d+\.?\d*) (?<y>-?\d+\.?\d*) (?<z>-?\d+\.?\d*)");
        [MenuItem("Tools/BakeAnimation")]
        public static void CreateAnimKeyFrames()
        {
            var regex = new Regex(@"\.obj$");
            var directory = Path.Combine(Application.dataPath, "Resources");
            var files = Directory.GetFiles(directory).Where(x => regex.IsMatch(x)).ToArray();

            var data = new BakedAnimation
            {
                FrameData = ObjHelper.ParseAnimations(directory).ToDictionary(x => x.Key, y => y.Value.Select(z => new Vec3Seri { X = z.x, Y = z.y, Z = z.z }).ToArray()),
                TotalFrames = files.Length,
                VerticesPerFrame = 0
            };
            using (var file = File.Open(Path.Combine(directory, "temp.objanim"), FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
            }

        }
    }




    public class MoveSystem : JobComponentSystem
    {
        public static int speed = 1;

        public static List<float3> _position = new List<float3>();

        [BurstCompile]
        public struct MovementJob : IJobProcessComponentData<Position, MoveSpeedComponent>
        {
            [ReadOnly] public float DeltaTime;

            [ReadOnly] public float Speed;

            [ReadOnly] public float3 Dest;

            [ReadOnly] public float3 CursorPosition;

            [ReadOnly] private float _minLen;

            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float> hData;

            [ReadOnly] public int MapWidth;
            [ReadOnly] public int MapHeight;

            public void Execute(ref Position position, ref MoveSpeedComponent moveSpeed)
            {
                position.Value = math.lerp(position.Value, position.Value + Dest, DeltaTime * Speed);

                //position.Value.y = GetTerrainHeight(position.Value.x, position.Value.z, 1, hData, MapHeight, MapWidth);
                position.Value.y = GetTerrainHeight(position.Value.x, position.Value.z, 1, hData, MapWidth, MapHeight);
            }
            public float GetTerrainHeight(float xPos, float zPos, float scaleFactor, NativeArray<float> heightData, int mapWidth, int mapHeight)
            {
                // we first get the height of four points of the quad underneath the point
                // Check to make sure this point is not off the map at all
                int x = (int)(xPos / scaleFactor);
                int z = (int)(zPos / scaleFactor);

                int xPlusOne = x + 1;
                int zPlusOne = z + 1;

                float triZ0 = (heightData[x * mapHeight + z]);
                float triZ1 = (heightData[xPlusOne * mapHeight + z]);
                float triZ2 = (heightData[x * mapHeight + zPlusOne]);
                float triZ3 = (heightData[xPlusOne * mapHeight + zPlusOne]);

                float height = 0.0f;
                float sqX = (xPos / scaleFactor) - x;
                float sqZ = (zPos / scaleFactor) - z;
                if ((sqX + sqZ) < 1)
                {
                    height = triZ0;
                    height += (triZ1 - triZ0) * sqX;
                    height += (triZ2 - triZ0) * sqZ;
                }
                else
                {
                    height = triZ3;
                    height += (triZ1 - triZ3) * (1.0f - sqZ);
                    height += (triZ2 - triZ3) * (1.0f - sqX);
                }
                return height;
            }
        }

        public static TerrainData TerrainData { get; set; }
        public static float[] heightData { get; set; }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MovementJob
            {
                DeltaTime = Time.deltaTime,
                Speed = speed,
                Dest = new float3(0, 0, 6),
                CursorPosition = Input.mousePosition,
                hData = new NativeArray<float>(heightData, Allocator.TempJob),
                MapHeight = TerrainData.heightmapHeight,
                MapWidth = TerrainData.heightmapWidth
            };

            var handle = job.Schedule(this, inputDeps);

            return handle;
        }

    }

    public class BakedAnimationWrapper
    {
        public List<List<Vector3>> Frames;
    }


    public class SelectSystem : ComponentSystem
    {
        public struct Data
        {
            public readonly int Length;
            [ReadOnly] public readonly SharedComponentDataArray<MeshInstanceRenderer> Renderers;
        }



        [Inject] public Data _data;

        public static BakedAnimationWrapper Animation;

        public static string Selected;
        private int counter;
        protected override void OnUpdate()
        {

            counter++;
            counter = counter >= Animation.Frames.Count ? 0 : counter;

            //for (int i = 0; i < _data.Length; i++)
            //{
            //    var renderer = _data.Renderers[i];

            //    if (Animation != null && counter < Animation.Frames.Count)
            //    {
            //        renderer.mesh.SetVertices(Animation.Frames[counter]);
            //    }
            //}


        }
    }

    public class CharacterMove : MonoBehaviour
    {
        public GameObject prefab;
        private int instanceCount = 10;
        private EntityManager _manager;
        [SerializeField] private TerrainData _terrainData;

        void Start()
        {
            MoveSystem.TerrainData = _terrainData;
            MoveSystem.heightData = _terrainData
                .GetHeights(0, 0, _terrainData.heightmapWidth, _terrainData.heightmapHeight).Cast<float>().ToArray();
            print(_terrainData.heightmapWidth + " " + _terrainData.heightmapHeight);

            MoveSystem._position.Add(new float3(1, 1, 0));

            _manager = World.Active.GetOrCreateManager<EntityManager>();

            using (var file = File.Open(Path.Combine(Application.dataPath, "Resources", "temp.objanim"), FileMode.Open))
            {
                var serializer = new BinaryFormatter();

                var bake = serializer.Deserialize(file) as BakedAnimation;

                SelectSystem.Animation = new BakedAnimationWrapper
                {
                    Frames = bake.FrameData.Select(x => x.Value.Select(y => new Vector3(y.X, y.Y, y.Z)).ToList()).ToList()
                };
            }

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

                _manager.SetComponentData(entity, new MoveSpeedComponent { Speed = 1, Name = i });


            }
            entities.Dispose();

        }

        private string lastName;
        void Update()
        {
            if (lastName != SelectSystem.Selected)
            {
                lastName = SelectSystem.Selected;
                //var projected = Camera.main.projectionMatrix * Vector3.zero;
                print(lastName);
            }
        }
    }
}
