using System;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.XnaLegacy
{
    [Serializable]
    public class XnaBoundingFrustum : IBoundingGeometry
    {

        #region Public Fields
        public const int CornerCount = 8;
        #endregion

        #region Public Properties

        public float3[] Corners;

        public XnaPlane Bottom;

        public XnaPlane Far;

        public XnaPlane Left;

        public Matrix4x4 Matrix;

        public XnaPlane Near;

        public XnaPlane Right;

        public XnaPlane Top;

        #endregion Public Properties


        public bool Equals(XnaBoundingFrustum other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            XnaBoundingFrustum f = obj as XnaBoundingFrustum;
            return (object.Equals(f, null)) ? false : (this == f);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("{Near:");
            sb.Append(this.Near.ToString());
            sb.Append(" Far:");
            sb.Append(this.Far.ToString());
            sb.Append(" Left:");
            sb.Append(this.Left.ToString());
            sb.Append(" Right:");
            sb.Append(this.Right.ToString());
            sb.Append(" Top:");
            sb.Append(this.Top.ToString());
            sb.Append(" Bottom:");
            sb.Append(this.Bottom.ToString());
            sb.Append("}");
            return sb.ToString();
        }


        public static bool operator ==(XnaBoundingFrustum a, XnaBoundingFrustum b)
        {
            if (object.Equals(a, null))
                return (object.Equals(b, null));

            if (object.Equals(b, null))
                return (object.Equals(a, null));

            return a.Matrix == (b.Matrix);
        }

        public static bool operator !=(XnaBoundingFrustum a, XnaBoundingFrustum b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            return this.Matrix.GetHashCode();
        }

    }

    public static class XnaBoundingFrustumHelper
    {
        #region Public Methods

        public static ContainmentType Contains(this XnaBoundingFrustum self, XnaBoundingBox box)
        {
            ContainmentType result;
            self.Contains(ref box, out result);
            return result;
        }
        public static void Contains(this XnaBoundingFrustum self, ref XnaBoundingBox box, out ContainmentType result)
        {
            var intersects = false;

            PlaneIntersectionType type;
            box.Intersects(ref self.Near, out type);
            if (type == PlaneIntersectionType.Front)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (type == PlaneIntersectionType.Intersecting)
                intersects = true;

            box.Intersects(ref self.Left, out type);
            if (type == PlaneIntersectionType.Front)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (type == PlaneIntersectionType.Intersecting)
                intersects = true;

            box.Intersects( ref self.Right, out type);
            if (type == PlaneIntersectionType.Front)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (type == PlaneIntersectionType.Intersecting)
                intersects = true;

            box.Intersects( ref self.Top, out type);
            if (type == PlaneIntersectionType.Front)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (type == PlaneIntersectionType.Intersecting)
                intersects = true;

            box.Intersects( ref self.Bottom, out type);
            if (type == PlaneIntersectionType.Front)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (type == PlaneIntersectionType.Intersecting)
                intersects = true;

            box.Intersects( ref self.Far, out type);
            if (type == PlaneIntersectionType.Front)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (type == PlaneIntersectionType.Intersecting)
                intersects = true;

            result = intersects ? ContainmentType.Intersects : ContainmentType.Contains;
        }
        public static ContainmentType Contains(this XnaBoundingFrustum self, XnaBoundingFrustum frustum)
        {
            if (self == frustum)                // We check to see if the two frustums are equal
                return ContainmentType.Contains;// If they are, there's no need to go any further.

            throw new NotImplementedException();
        }
        public static ContainmentType Contains(this XnaBoundingFrustum self, XnaBoundingSphere sphere)
        {
            ContainmentType result;
            self.Contains(ref sphere, out result);
            return result;
        }
        public static void Contains(this XnaBoundingFrustum self, ref XnaBoundingSphere sphere, out ContainmentType result)
        {
            double dist;
            result = ContainmentType.Contains;

            dist = math.dot(self.Bottom.Normal, sphere.Position);
            dist += self.Bottom.D;
            if (dist > sphere.Radius)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (Math.Abs(dist) < sphere.Radius)
                result = ContainmentType.Intersects;

            dist = math.dot(self.Top.Normal, sphere.Position);
            dist += self.Top.D;
            if (dist > sphere.Radius)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (Math.Abs(dist) < sphere.Radius)
                result = ContainmentType.Intersects;

            dist = math.dot(self.Near.Normal, sphere.Position);
            dist += self.Near.D;
            if (dist > sphere.Radius)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (Math.Abs(dist) < sphere.Radius)
                result = ContainmentType.Intersects;

            dist = math.dot(self.Far.Normal, sphere.Position);
            dist += self.Far.D;
            if (dist > sphere.Radius)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (Math.Abs(dist) < sphere.Radius)
                result = ContainmentType.Intersects;

            dist = math.dot(self.Left.Normal, sphere.Position);
            dist += self.Left.D;
            if (dist > sphere.Radius)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (Math.Abs(dist) < sphere.Radius)
                result = ContainmentType.Intersects;

            dist = math.dot(self.Right.Normal, sphere.Position);
            dist += self.Right.D;
            if (dist > sphere.Radius)
            {
                result = ContainmentType.Disjoint;
                return;
            }
            if (Math.Abs(dist) < sphere.Radius)
                result = ContainmentType.Intersects;
        }
        public static ContainmentType Contains(this XnaBoundingFrustum self, float3 point)
        {
            ContainmentType result;
            self.Contains(ref point, out result);
            return result;
        }
        public static void Contains(this XnaBoundingFrustum self, ref float3 point, out ContainmentType result)
        {
            double val;
            // If a point is on the POSITIVE side of the plane, then the point is not contained within the frustum

            // Check the top
            val = XnaPlaneHelper.ClassifyPoint(ref point, ref self.Top);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the bottom
            val = XnaPlaneHelper.ClassifyPoint(ref point, ref self.Bottom);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the left
            val = XnaPlaneHelper.ClassifyPoint(ref point, ref self.Left);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the right
            val = XnaPlaneHelper.ClassifyPoint(ref point, ref self.Right);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the near
            val = XnaPlaneHelper.ClassifyPoint(ref point, ref self.Near);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the far
            val = XnaPlaneHelper.ClassifyPoint(ref point, ref self.Far);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // If we get here, it means that the point was on the correct side of each plane to be
            // contained. Therefore this point is contained
            result = ContainmentType.Contains;
        }
        public static float3[] GetCorners(this XnaBoundingFrustum self)
        {
            return (float3[])self.Corners.Clone();
        }
        public static void GetCorners(this XnaBoundingFrustum self, float3[] corners)
        {
            if (corners == null) throw new ArgumentNullException("corners");
            if (corners.Length < 8) throw new ArgumentOutOfRangeException("corners");

            self.GetCorners().CopyTo(corners, 0);
        }
        public static bool Intersects(this XnaBoundingFrustum self, XnaBoundingBox box)
        {
            var result = false;
            self.Intersects(ref box, out result);
            return result;
        }
        public static void Intersects(this XnaBoundingFrustum self, ref XnaBoundingBox box, out bool result)
        {
            var containment = ContainmentType.Disjoint;
            self.Contains(ref box, out containment);
            result = containment != ContainmentType.Disjoint;
        }
        public static bool Intersects(this XnaBoundingFrustum self, XnaBoundingFrustum frustum)
        {
            throw new NotImplementedException();
        }
        public static bool Intersects(this XnaBoundingFrustum self, XnaBoundingSphere sphere)
        {
            throw new NotImplementedException();
        }
        public static void Intersects(this XnaBoundingFrustum self, ref XnaBoundingSphere sphere, out bool result)
        {
            throw new NotImplementedException();
        }
        public static PlaneIntersectionType Intersects(this XnaBoundingFrustum self, XnaPlane plane)
        {
            throw new NotImplementedException();
        }
        public static void Intersects(this XnaBoundingFrustum self, ref XnaPlane plane, out PlaneIntersectionType result)
        {
            throw new NotImplementedException();
        }
        public static Nullable<double> Intersects(this XnaBoundingFrustum self, XnaRay ray)
        {
            throw new NotImplementedException();
        }
        public static void Intersects(this XnaBoundingFrustum self, ref Ray ray, out Nullable<double> result)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods


        #region Private Methods

        private static void CreateCorners(this XnaBoundingFrustum self)
        {

            self.Corners = new float3[XnaBoundingFrustum.CornerCount];
            self.Corners[0] = IntersectionPoint(self, ref self.Near, ref self.Left, ref self.Top);
            self.Corners[1] = IntersectionPoint(self, ref self.Near, ref self.Right, ref self.Top);
            self.Corners[2] = IntersectionPoint(self, ref self.Near, ref self.Right, ref self.Bottom);
            self.Corners[3] = IntersectionPoint(self, ref self.Near, ref self.Left, ref self.Bottom);
            self.Corners[4] = IntersectionPoint(self, ref self.Far, ref self.Left, ref self.Top);
            self.Corners[5] = IntersectionPoint(self, ref self.Far, ref self.Right, ref self.Top);
            self.Corners[6] = IntersectionPoint(self, ref self.Far, ref self.Right, ref self.Bottom);
            self.Corners[7] = IntersectionPoint(self, ref self.Far, ref self.Left, ref self.Bottom);
        }
        private static void CreatePlanes(this XnaBoundingFrustum self)
        {
            // Pre-calculate the different planes needed
            self.Left = new float4(-self.Matrix.m03 - self.Matrix.m00, -self.Matrix.m13 - self.Matrix.m10,
                -self.Matrix.m23 - self.Matrix.m20, -self.Matrix.m33 - self.Matrix.m30).FromFloat4();

            self.Right = new float4(self.Matrix.m00 - self.Matrix.m03, self.Matrix.m10 - self.Matrix.m13,
                                   self.Matrix.m20 - self.Matrix.m23, self.Matrix.m30 - self.Matrix.m33).FromFloat4();

            self.Top = new float4(self.Matrix.m01 - self.Matrix.m03, self.Matrix.m11 - self.Matrix.m13,
                                 self.Matrix.m21 - self.Matrix.m23, self.Matrix.m31 - self.Matrix.m33).FromFloat4();

            self.Bottom = new float4(-self.Matrix.m03 - self.Matrix.m01, -self.Matrix.m13 - self.Matrix.m11,
                                    -self.Matrix.m23 - self.Matrix.m21, -self.Matrix.m33 - self.Matrix.m31).FromFloat4();

            self.Near = new float4(-self.Matrix.m02, -self.Matrix.m12, -self.Matrix.m22, -self.Matrix.m32).FromFloat4();


            self.Far = new float4(self.Matrix.m02 - self.Matrix.m03, self.Matrix.m12 - self.Matrix.m13,
                                 self.Matrix.m22 - self.Matrix.m23, self.Matrix.m32 - self.Matrix.m33).FromFloat4();

            self.NormalizePlane(ref self.Left);
            self.NormalizePlane(ref self.Right);
            self.NormalizePlane(ref self.Top);
            self.NormalizePlane(ref self.Bottom);
            self.NormalizePlane(ref self.Near);
            self.NormalizePlane(ref self.Far);
        }
        private static float3 IntersectionPoint(this XnaBoundingFrustum self, ref XnaPlane a, ref XnaPlane b, ref XnaPlane c)
        {
            // Formula used
            //                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
            //P =     -------------------------------------------------------------------------
            //                             N1 . ( N2 * N3 )
            //
            // Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product

            float3 v1, v2, v3;
            float f = -math.dot(a.Normal, math.cross(b.Normal, c.Normal));

            v1 = (float)a.D * math.cross(b.Normal, c.Normal);
            v2 = (float)b.D * math.cross(c.Normal, a.Normal);
            v3 = (float)c.D * math.cross(a.Normal, b.Normal);

            float3 vec = new float3(v1.x + v2.x + v3.x, v1.y + v2.y + v3.y, v1.z + v2.z + v3.z);
            return vec / f;
        }
        private static void NormalizePlane(this XnaBoundingFrustum self, ref XnaPlane p)
        {
            var factor = 1f / math.length(p.Normal);
            p.Normal.x *= factor;
            p.Normal.y *= factor;
            p.Normal.z *= factor;
            p.D *= factor;
        }

        #endregion
    }


}