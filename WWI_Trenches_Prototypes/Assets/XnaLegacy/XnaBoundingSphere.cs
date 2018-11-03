using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.XnaLegacy
{
    [Serializable]
    public struct XnaBoundingSphere : IBoundingGeometry
    {
        #region Public Fields

        public float3 Center;
        public float Radius;

        #endregion Public Fields

        public static bool operator ==(XnaBoundingSphere a, XnaBoundingSphere b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(XnaBoundingSphere a, XnaBoundingSphere b)
        {
            return !a.Equals(b);
        }
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Center:{0} Radius:{1}}}", this.Center.ToString(), this.Radius.ToString());
        }
        public bool Equals(XnaBoundingSphere other)
        {
            return Center.Equals(other.Center) && Math.Abs(this.Radius - other.Radius) < .0000001f;
        }
        public override bool Equals(object obj)
        {
            if (obj is XnaBoundingSphere)
                return this.Equals((XnaBoundingSphere)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode();
        }
    }

    public static class BoundingShpereHelper
    {
        #region Public Methods
        public static XnaBoundingSphere Transform(this XnaBoundingSphere self, Matrix4x4 matrix)
        {
            XnaBoundingSphere sphere = new XnaBoundingSphere();
            sphere.Center = math.transform(matrix, self.Center);
            sphere.Radius = self.Radius * (math.sqrt(math.max(((matrix.m00 * matrix.m00) + (matrix.m01 * matrix.m01)) + (matrix.m02 * matrix.m02), math.max(((matrix.m10 * matrix.m10) + (matrix.m11 * matrix.m11)) + (matrix.m12 * matrix.m12), ((matrix.m20 * matrix.m20) + (matrix.m21 * matrix.m21)) + (matrix.m22 * matrix.m22)))));
            return sphere;
        }
        public static void Transform(this XnaBoundingSphere self, ref Matrix4x4 matrix, out XnaBoundingSphere result)
        {
            result.Center = math.transform(matrix, self.Center);
            result.Radius = self.Radius * math.sqrt(math.max(matrix.m00 * matrix.m00 + matrix.m01 * matrix.m01 + matrix.m02 * matrix.m02, math.max(matrix.m10 * matrix.m10 + matrix.m11 * matrix.m11 + matrix.m12 * matrix.m12, matrix.m20 * matrix.m20 + matrix.m21 * matrix.m21 + matrix.m22 * matrix.m22)));
        }
        public static ContainmentType Contains(this XnaBoundingSphere self, XnaBoundingBox box)
        {
            //check if all corner is in sphere
            bool inside = true;
            foreach (float3 corner in box.GetCorners())
            {
                if (self.Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }

            if (inside)
                return ContainmentType.Contains;

            //check if the distance from sphere center to cube face < radius
            float dmin = 0;

            if (self.Center.x < box.Min.x)
                dmin += (self.Center.x - box.Min.x) * (self.Center.x - box.Min.x);

            else if (self.Center.x > box.Max.x)
                dmin += (self.Center.x - box.Max.x) * (self.Center.x - box.Max.x);

            if (self.Center.y < box.Min.y)
                dmin += (self.Center.y - box.Min.y) * (self.Center.y - box.Min.y);

            else if (self.Center.y > box.Max.y)
                dmin += (self.Center.y - box.Max.y) * (self.Center.y - box.Max.y);

            if (self.Center.z < box.Min.z)
                dmin += (self.Center.z - box.Min.z) * (self.Center.z - box.Min.z);

            else if (self.Center.z > box.Max.z)
                dmin += (self.Center.z - box.Max.z) * (self.Center.z - box.Max.z);

            if (dmin <= self.Radius * self.Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;

        }
        public static void Contains(this XnaBoundingSphere self, ref XnaBoundingBox box, out ContainmentType result)
        {
            result = self.Contains(box);
        }
        public static ContainmentType Contains(this XnaBoundingSphere self, XnaBoundingFrustum frustum)
        {
            //check if all corner is in sphere
            bool inside = true;

            float3[] corners = frustum.GetCorners();
            foreach (float3 corner in corners)
            {
                if (self.Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }
            if (inside)
                return ContainmentType.Contains;

            //check if the distance from sphere center to frustrum face < radius
            double dmin = 0;
            //TODO : calcul dmin

            if (dmin <= self.Radius * self.Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;
        }
        public static ContainmentType Contains(this XnaBoundingSphere self, XnaBoundingSphere sphere)
        {
            double val = math.distance(sphere.Center, self.Center);

            if (val > sphere.Radius + self.Radius)
                return ContainmentType.Disjoint;

            else if (val <= self.Radius - sphere.Radius)
                return ContainmentType.Contains;

            else
                return ContainmentType.Intersects;
        }
        public static void Contains(this XnaBoundingSphere self, ref XnaBoundingSphere sphere, out ContainmentType result)
        {
            result = self.Contains(sphere);
        }
        public static ContainmentType Contains(this XnaBoundingSphere self, float3 point)
        {
            double distance = math.distance(point, self.Center);

            if (distance > self.Radius)
                return ContainmentType.Disjoint;

            else if (distance < self.Radius)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }
        public static void Contains(this XnaBoundingSphere self, ref float3 point, out ContainmentType result)
        {
            result = self.Contains(point);
        }
        public static XnaBoundingSphere CreateFromBoundingBox(this XnaBoundingSphere self, XnaBoundingBox box)
        {
            // Find the center of the box.
            float3 center = new float3((box.Min.x + box.Max.x) / 2.0f,
                                         (box.Min.y + box.Max.y) / 2.0f,
                                         (box.Min.z + box.Max.z) / 2.0f);

            // Find the distance between the center and one of the corners of the box.
            var radius = math.distance(center, box.Max);

            return FromCenterRadius(center, radius);
        }

        public static XnaBoundingSphere FromCenterRadius(float3 center, float radius)
        {
            return new XnaBoundingSphere
            {
                Center = center,
                Radius = radius
            };
        }

        public static void CreateFromBoundingBox(this XnaBoundingSphere self, ref XnaBoundingBox box, out XnaBoundingSphere result)
        {
            result = self.CreateFromBoundingBox(box);
        }
        public static XnaBoundingSphere CreateFromFrustum(this XnaBoundingSphere self, XnaBoundingFrustum frustum)
        {
            return CreateFromPoints(self, frustum.GetCorners());
        }
        public static XnaBoundingSphere CreateFromPoints(this XnaBoundingSphere self, IEnumerable<float3> points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            var radius = 0f;
            float3 center = new float3();
            // First, we'll find the center of gravity for the point 'cloud'.
            int num_points = 0; // The number of points (there MUST be a better way to get this instead of counting the number of points one by one?)

            foreach (float3 v in points)
            {
                center += v;    // If we actually knew the number of points, we'd get better accuracy by adding v / num_points.
                ++num_points;
            }

            center /= num_points;

            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (float3 v in points)
            {
                var distance = math.distance(v, center);

                if (distance > radius)
                    radius = distance;
            }

            return FromCenterRadius(center, radius);
        }
        public static XnaBoundingSphere CreateMerged(this XnaBoundingSphere self, XnaBoundingSphere original, XnaBoundingSphere additional)
        {

            float3 ocenterToaCenter = additional.Center - original.Center;
            float distance = math.length(ocenterToaCenter);
            if (distance <= original.Radius + additional.Radius)//intersect
            {
                if (distance <= original.Radius - additional.Radius)//original contain additional
                    return original;
                if (distance <= additional.Radius - original.Radius)//additional contain original
                    return additional;
            }

            //else find center of new sphere and radius
            float leftRadius = Mathf.Max(original.Radius - distance, additional.Radius);
            float Rightradius = Mathf.Max(original.Radius + distance, additional.Radius);
            ocenterToaCenter = ocenterToaCenter + (((leftRadius - Rightradius) / (2 * math.length(ocenterToaCenter))) * ocenterToaCenter);//oCenterToResultCenter

            XnaBoundingSphere result = new XnaBoundingSphere();
            result.Center = original.Center + ocenterToaCenter;
            result.Radius = (leftRadius + Rightradius) / 2;
            return result;
        }
        public static void CreateMerged(this XnaBoundingSphere self, ref XnaBoundingSphere original, ref XnaBoundingSphere additional, out XnaBoundingSphere result)
        {
            result = CreateMerged(self, original, additional);
        }
        public static bool Intersects(this XnaBoundingSphere self, XnaBoundingBox box)
        {
            return box.Intersects(self);
        }
        public static void Intersects(this XnaBoundingSphere self, ref XnaBoundingBox box, out bool result)
        {
            result = self.Intersects(box);
        }
        public static bool Intersects(this XnaBoundingSphere self, XnaBoundingFrustum frustum)
        {
            if (frustum == null)
                throw new NullReferenceException();

            throw new NotImplementedException();
        }
        public static bool Intersects(this XnaBoundingSphere self, XnaBoundingSphere sphere)
        {
            double val = math.distance(sphere.Center, self.Center);
            if (val > sphere.Radius + self.Radius)
                return false;
            return true;
        }
        public static void Intersects(this XnaBoundingSphere self, ref XnaBoundingSphere sphere, out bool result)
        {
            result = self.Intersects(sphere);
        }
        public static PlaneIntersectionType Intersects(this XnaBoundingSphere self, XnaPlane plane)
        {
            double distance = math.dot(plane.Normal, self.Center) + plane.D;
            if (distance > self.Radius)
                return PlaneIntersectionType.Front;
            if (distance < -self.Radius)
                return PlaneIntersectionType.Back;
            //else it intersect
            return PlaneIntersectionType.Intersecting;
        }
        public static void Intersects(this XnaBoundingSphere self, ref XnaPlane plane, out PlaneIntersectionType result)
        {
            result = self.Intersects(plane);
        }
        public static Nullable<double> Intersects(this XnaBoundingSphere self, XnaRay ray)
        {
            return ray.Intersects(self);
        }
        public static void Intersects(this XnaBoundingSphere self, ref XnaRay ray, out Nullable<double> result)
        {
            result = self.Intersects(ray);
        }

        #endregion Public Methods
    }


    public enum ContainmentType
    {
        Disjoint,
        Contains,
        Intersects
    }
}