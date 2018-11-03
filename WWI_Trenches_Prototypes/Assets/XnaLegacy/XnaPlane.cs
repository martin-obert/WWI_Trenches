using System;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.XnaLegacy
{
    [Serializable]
    public struct XnaPlane : IBoundingGeometry
    {

        public double D;
        public float3 Normal;
        public static bool operator !=(XnaPlane plane1, XnaPlane plane2)
        {
            return !plane1.Equals(plane2);
        }
        public static bool operator ==(XnaPlane plane1, XnaPlane plane2)
        {
            return plane1.Equals(plane2);
        }
        public override bool Equals(object other)
        {
            return (other is XnaPlane) && this.Equals((XnaPlane)other);
        }
        public bool Equals(XnaPlane other)
        {

            return Normal.Equals(other.Normal) && Math.Abs(D - other.D) < .0000001f;
        }
        public override int GetHashCode()
        {
            return Normal.GetHashCode() ^ D.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{{Normal:{0} D:{1}}}", Normal, D);
        }

    }

    public static class XnaPlaneHelper
    {
        #region Public Methods
        public static XnaPlane FromFloat4(this float4 value)
        {
            return FromFloat3(new float3(value.x, value.y, value.z), value.w);
        }

        public static XnaPlane FromPoints(float a, float b, float c, float d)
        {
            return FromFloat3(new float3(a, b, c), d);
        }

        public static XnaPlane FromFloat3(this float3 value, double d)
        {
            return new XnaPlane { D = d, Normal = value };
        }

        /// <summary>
        /// Returns a value indicating what side (positive/negative) of a plane a point is
        /// </summary>
        /// <param name="point">The point to check with</param>
        /// <param name="plane">The plane to check against</param>
        /// <returns>Greater than zero if on the positive side, less than zero if on the negative size, 0 otherwise</returns>
        public static double ClassifyPoint(ref float3 point, ref XnaPlane plane)
        {
            return point.x * plane.Normal.x + point.y * plane.Normal.y + point.z * plane.Normal.z + plane.D;
        }

        /// <summary>
        /// Returns the perpendicular distance from a point to a plane
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <param name="plane">The place to check</param>
        /// <returns>The perpendicular distance from the point to the plane</returns>
        public static double PerpendicularDistance(ref float3 point, ref XnaPlane plane)
        {
            // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
            return math.abs((plane.Normal.x * point.x + plane.Normal.y * point.y + plane.Normal.z * point.z)
                            / Math.Sqrt(plane.Normal.x * plane.Normal.x + plane.Normal.y * plane.Normal.y + plane.Normal.z * plane.Normal.z));
        }

        public static double Dot(this XnaPlane plane, Vector4 value)
        {
            return plane.Normal.x * value.x + plane.Normal.y * value.y + plane.Normal.z * value.z + plane.D * value.w;
        }
        public static void Dot(this XnaPlane plane, ref Vector4 value, out double result)
        {
            result = ((plane.Normal.x * value.x) + plane.Normal.y * value.y) + plane.Normal.z * value.z + plane.D * value.w;
        }
        public static double DotCoordinate(this XnaPlane plane, float3 value)
        {
            return ((((plane.Normal.x * value.x) + (plane.Normal.y * value.y)) + (plane.Normal.z * value.z)) + plane.D);
        }
        public static void DotCoordinate(this XnaPlane plane, ref float3 value, out double result)
        {
            result = (((plane.Normal.x * value.x) + (plane.Normal.y * value.y)) + (plane.Normal.z * value.z)) + plane.D;
        }
        public static float DotNormal(this XnaPlane plane, float3 value)
        {
            return (((plane.Normal.x * value.x) + (plane.Normal.y * value.y)) + (plane.Normal.z * value.z));
        }
        public static void DotNormal(this XnaPlane plane, ref float3 value, out double result)
        {
            result = ((plane.Normal.x * value.x) + (plane.Normal.y * value.y)) + (plane.Normal.z * value.z);
        }
        public static void Transform(this XnaPlane self, ref Plane plane, ref Quaternion rotation, out Plane result)
        {
            throw new NotImplementedException();
        }
        public static void Transform(this XnaPlane self, ref Plane plane, ref Matrix4x4 matrix, out Plane result)
        {
            throw new NotImplementedException();
        }
        public static Plane Transform(this XnaPlane self, Plane plane, Quaternion rotation)
        {
            throw new NotImplementedException();
        }
        public static Plane Transform(this XnaPlane self, Plane plane, Matrix4x4 matrix)
        {
            throw new NotImplementedException();
        }
        public static void Normalize(this XnaPlane plane)
        {
            float factor;
            float3 normal = plane.Normal;
            plane.Normal = math.normalize(plane.Normal);
            factor = math.sqrt(plane.Normal.x * plane.Normal.x + plane.Normal.y * plane.Normal.y + plane.Normal.z * plane.Normal.z) /
                    math.sqrt(normal.x * normal.x + normal.y * normal.y + normal.z * normal.z);
            plane.D = plane.D * factor;
        }
        public static XnaPlane Normalize(this XnaPlane plane, XnaPlane value)
        {
            XnaPlane ret;
            Normalize(plane, ref value, out ret);
            return ret;
        }
        public static void Normalize(this XnaPlane plane, ref XnaPlane value, out XnaPlane result)
        {
            float factor;
            result.Normal = math.normalize(value.Normal);
            factor = math.sqrt(result.Normal.x * result.Normal.x + result.Normal.y * result.Normal.y + result.Normal.z * result.Normal.z) /
                     math.sqrt(value.Normal.x * value.Normal.x + value.Normal.y * value.Normal.y + value.Normal.z * value.Normal.z);
            result.D = value.D * factor;
        }
        public static PlaneIntersectionType Intersects(this XnaPlane plane, XnaBoundingBox box)
        {
            return box.Intersects(plane);
        }
        public static void Intersects(this XnaPlane plane, ref XnaBoundingBox box, out PlaneIntersectionType result)
        {
            result = plane.Intersects(box);
        }
        public static PlaneIntersectionType Intersects(this XnaPlane plane, XnaBoundingSphere sphere)
        {
            return sphere.Intersects(plane);
        }
        public static void Intersects(this XnaPlane plane, ref XnaBoundingSphere sphere, out PlaneIntersectionType result)
        {
            result = plane.Intersects(sphere);
        }
        public static PlaneIntersectionType Intersectsthis(this XnaPlane self, XnaPlane plane, XnaBoundingFrustum frustum)
        {
            return frustum.Intersects(self);
        }
        #endregion
    }
}