
using System;
using System.Collections.Generic;
using Unity.VectorGraphics.Editor;
using UnityEngine;

namespace Assets.PathMesh
{
    [Serializable]
    public class PathPoint
    {
        public Vector2 Position;
    }


    public sealed class PathUtils
    {
        /// <summary>
        /// Creates segmet for mesh
        /// </summary>
        /// <param name="a">Predecessor of current point (can be null)</param>
        /// <param name="b">Current point</param>
        /// <param name="c">Next point</param>
        /// <param name="d">Child of next point (can be null)</param>
        /// <param name="thickness">Thickness of path</param>
        /// <param name="vertices">Current vertice list</param>
        /// <param name="indices">Current indice list</param>
        public static void AddPathSegment(PathPoint a, PathPoint b, PathPoint c, PathPoint d, float thickness, List<Vector3> vertices, List<int> indices)
        {
            var firstDirection = a == null ? c.Position - b.Position : b.Position - a.Position;
            var secondDirection = d == null ? c.Position - b.Position : d.Position - b.Position;

            var perpABNorm = Vector2.Perpendicular(firstDirection).normalized;
            var perpBDNorm = Vector2.Perpendicular(secondDirection).normalized;

            //Todo: possible missdirection invert +/-
            var nearLeft = b.Position + perpABNorm * thickness;
            var nearRight = b.Position - perpABNorm * thickness;

            var farLeft = c.Position + perpBDNorm * thickness;
            var farRight = c.Position - perpBDNorm * thickness;

            var currentCount = vertices.Count - 1;

            vertices.AddRange(new[]
            {
                new Vector3(nearLeft.x, nearLeft.y),
                new Vector3(nearRight.x, nearRight.y),
                new Vector3(farLeft.x, farLeft.y),
                new Vector3(farRight.x, farRight.y)
            });

            //A side
            indices.AddRange(new[] { currentCount + 1, currentCount + 2, currentCount + 3 });

            //B side
            indices.AddRange(new[] { currentCount + 1, currentCount + 3, currentCount + 4 });
        }
    }
}
