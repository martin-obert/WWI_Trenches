using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.ObjAnimations
{

    [Serializable]
    [CreateAssetMenu(menuName = "Obj Anim", fileName = "Obj Animation")]
    public class ObjAnimationSo : ScriptableObject
    {
        public string Name;

        public bool IsRepeatable;

        public float3[] Vertices { get; set; }

        public int[] Indices { get; set; }

        public int SubMeshCount { get; set; }

        public float3[] Normals { get; set; }
        public int IndicesPerMesh { get; set; }
        public int VerticesPerMesh { get; set; }
    }

    public static class ObjAnimationSoHelper
    {
        public static Mesh[] ToMesh(this ObjAnimationSo so)
        {
            Debug.Log("Transfroming obj anim so " + so.Vertices.Length);
            Debug.Log("Transfroming obj anim so " + so.Indices.Length);
            var result = new Mesh[so.SubMeshCount];
            for (int i = 0; i < so.SubMeshCount; i++)
            {
                result[i] = new Mesh
                {
                    vertices = so.Vertices.Skip(i*so.VerticesPerMesh).Take(so.VerticesPerMesh).Select(x => (Vector3)x).ToArray(),
                    triangles = so.Indices.Skip(i * so.IndicesPerMesh).Take(so.IndicesPerMesh).Select(x => x).ToArray(),
                    //normals = so.Normals.Skip(i * so.VerticesPerMesh).Take(so.VerticesPerMesh).Select(x => (Vector3)x).ToArray()
                };

                result[i].RecalculateNormals();
            }
            Debug.Log(result.Length);
            return result.ToArray();
        }
    }
}