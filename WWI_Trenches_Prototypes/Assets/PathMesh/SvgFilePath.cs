using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.PathMesh
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class SvgFilePath : MonoBehaviour
    {
        public string SourceFilePath;
        public string SourceFilename;

        [HideInInspector]
        public PathPoint[] SourcePoints;

        public float Thickness = 1f;
        private MeshFilter _meshFilter;

        void Awake()
        {
            var data = SvgImporter.ImportPathFromFile(SourceFilePath, SourceFilename);

            SourcePoints = data.Points.Select(x => new PathPoint { Position = x }).ToArray();
        }

        void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();

            _meshFilter.mesh = GetMesh();
        }

        public Mesh GetMesh()
        {
            var vertices = new List<Vector3>();

            var indices = new List<int>();

            for (int i = 0; i < SourcePoints.Length - 1; i++)
            {

                var a = i == 0 ? null : SourcePoints[i - 1];

                var b = SourcePoints[i];

                var c = SourcePoints[i + 1];

                var d = i + 2 < SourcePoints.Length ? SourcePoints[i + 2] : null;

                PathUtils.AddPathSegment(a, b, c, d, Thickness, vertices, indices);
            }

            return new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = indices.ToArray()
            };
        }
    }

    [CustomEditor(typeof(SvgFilePath))]
    public class SvgFilePathEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Preview"))
            {
                var path = target as SvgFilePath;

                path.GetComponent<MeshFilter>().mesh = path.GetMesh();

            }
            base.OnInspectorGUI();
        }
    }
}