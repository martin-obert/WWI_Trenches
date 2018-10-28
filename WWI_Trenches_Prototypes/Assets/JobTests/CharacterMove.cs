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
    [Serializable]
    public class BakedAnimation
    {
        public int TotalFrames;
        public Dictionary<int, List<Vec3Seri>> FrameData;
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
            var directory = Path.Combine(Application.dataPath, "Resources");
            var files = Directory.GetFiles(directory).Where(x => !x.Contains(".obj.meta")).ToArray();
            var data = new BakedAnimation
            {
                FrameData = new Dictionary<int, List<Vec3Seri>>(),
                TotalFrames = files.Length,
                VerticesPerFrame = 0
            };

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Debug.Log("Processing " + name);
                var parts = name.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                var frameNo = 0;

                if (!int.TryParse(parts[1], out frameNo)) Debug.Log("Wrong format " + parts[1]);

                var fileLines = File.ReadAllLines(file);
                var vertices = new List<Vector3>();
                var indicies = new List<int>();

                foreach (var fileLine in fileLines)
                {

                    var lineParts = fileLine.Split(' ');
                    if (lineParts.Length == 4)
                    {
                        if (lineParts[0] == "v")
                        {
                            var x = 0f;
                            var y = 0f;
                            var z = 0f;

                            if (!float.TryParse(lineParts[1].Replace(".", ","), out x))
                            {
                                Debug.Log("wrong format" + lineParts[1]);
                            }

                            if (!float.TryParse(lineParts[2].Replace(".", ","), out y))
                            {
                                Debug.Log("wrong format" + lineParts[2]);
                            }

                            if (!float.TryParse(lineParts[3].Replace(".", ","), out z))
                            {
                                Debug.Log("wrong format" + lineParts[3]);
                            }

                            vertices.Add(new Vector3 { x = x, y = y, z = z });
                        }

                        if (lineParts[0] == "f")
                        {
                            var a = int.Parse(lineParts[1].Substring(0, 1));
                            var b = int.Parse(lineParts[2].Substring(0, 1));
                            var c = int.Parse(lineParts[3].Substring(0, 1));
                            var d = int.Parse(lineParts[4].Substring(0, 1));

                            indicies.AddRange(new[] { a, b, c, d });
                        }
                    }
                }

                Debug.Log("Writing frame " + frameNo + " vertices  " + vertices.Count);
                var vec = new List<Vec3Seri>();
                foreach (var indicy in indicies)
                {
                    vec.Add(new Vec3Seri
                    {
                        X = vertices[indicy].x,
                        Y = vertices[indicy].y,
                        Z = vertices[indicy].z
                    });
                }
                data.FrameData.Add(frameNo, vec);
                data.VerticesPerFrame = vertices.Count;
            }

            Debug.Log("Total frames " + data.TotalFrames);

            using (var file = File.Create(Path.Combine(directory, "temp.animbake")))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(file, data);
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

            [ReadOnly] public Matrix4x4 _matrix;

            public void Execute(ref Position position, ref MoveSpeedComponent moveSpeed)
            {
                //moveSpeed.Destination = Dest;
                //position.Value = math.lerp(position.Value, moveSpeed.Destination, DeltaTime * Speed);

            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            var job = new MovementJob
            {
                DeltaTime = Time.deltaTime,
                Speed = speed,
                Dest = _position.Count > 0 ? _position[0] : float3.zero,
                CursorPosition = Input.mousePosition,
                _matrix = Camera.main.projectionMatrix
            };

            var handle = job.Schedule(this, inputDeps);


            return handle;

        }
    }

    public class BakedAnimationWrapper
    {
        public List<List<Vector3>> Frames;
    }


    public class SelectSystem : JobComponentSystem
    {
        public struct Data
        {
            public readonly int Length;
            [ReadOnly] public readonly SharedComponentDataArray<MeshInstanceRenderer> Renderers;
        }

        public struct ShapeAnimationJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector3> Keys;
            [ReadOnly] public NativeArray<MeshInstanceRenderer> Renderers;

            public void Execute(int index)
            {
                Renderers[index].mesh.SetVertices(Keys.ToList());
            }
        }


        [Inject] public Data _data;

        public static BakedAnimationWrapper Animation;

        public static string Selected;
        private int counter;
        protected override JobHandle OnUpdate(JobHandle inputDeps)
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

            var renderers = new NativeArray<MeshInstanceRenderer>(_data.Renderers.Length, Allocator.Temp);
            var keys = new NativeArray<Vector3>(Animation.Frames[counter].Count, Allocator.Temp);
            keys.CopyFrom(Animation.Frames[counter].ToArray());

            for (int i = 0; i < _data.Renderers.Length; i++)
            {
                renderers[i] = _data.Renderers[i];
            }

            var job = new ShapeAnimationJob
            {
                Renderers = renderers,
                Keys = keys
            };

            return job.Schedule(_data.Length, 6);

        }
    }
    public struct AnimationJob : IAnimationJob
    {
        public void ProcessRootMotion(AnimationStream stream)
        {

        }

        public void ProcessAnimation(AnimationStream stream)
        {
        }
    }


    public class CharacterMove : MonoBehaviour
    {
        public GameObject prefab;
        private int instanceCount = 10;
        private EntityManager _manager;
        void Start()
        {
            MoveSystem._position.Add(new float3(1, 1, 0));

            _manager = World.Active.GetOrCreateManager<EntityManager>();

            using (var file = File.Open(Path.Combine(Application.dataPath, "Resources", "temp.animbake"), FileMode.Open))
            {
                var serializer = new BinaryFormatter();
                var bake = serializer.Deserialize(file) as BakedAnimation;
                var total = new List<List<Vector3>>();
                for (int i = 0; i < 250; i++)
                {
                    var vert = new List<Vector3>();
                    for (int j = 0; j < 24; j++)
                    {
                        vert.Add(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
                    }
                    total.Add(vert);
                }

                SelectSystem.Animation = new BakedAnimationWrapper
                {
                    Frames = total
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
