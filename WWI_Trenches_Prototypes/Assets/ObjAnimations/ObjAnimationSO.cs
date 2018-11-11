using System;
using System.Linq;
using UnityEngine;

namespace Assets.ObjAnimations
{
    public enum AnimationType
    {
        None = 0,
        Idle = 1
    }

    [Serializable]
    [CreateAssetMenu(menuName = "Obj Anim", fileName = "Obj Animation")]
    public class ObjAnimationSO : ScriptableObject
    {
        [SerializeField]
        public int FrameRate;

        [SerializeField]
        public AnimationType Type;

        [SerializeField]
        public string Name;

        [SerializeField]
        public bool IsRepeatable;

        [SerializeField, HideInInspector] public Vector3[] Vertices;

        [SerializeField, HideInInspector] public int[] Indices;

        [SerializeField] public int SubMeshCount;

        [SerializeField, HideInInspector] public Vector3[] Normals;

        [SerializeField] public int IndicesPerMesh;

        [SerializeField] public int VerticesPerMesh;
    }

    public static class ObjAnimationSoHelper
    {
        public static Mesh[] ToMesh(this ObjAnimationSO so)
        {
            Debug.Log("Transfroming obj anim so " + so);
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