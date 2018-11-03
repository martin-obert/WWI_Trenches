using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.XnaLegacy
{
    public interface IBoundingGeometry : IComponentData
    {

    }

    [Serializable]
    public struct XnaBoundingBox : IBoundingGeometry
    {
        #region Public Fields
        public float3 Min;
        public float3 Max;
        #endregion Public Fields


        public static bool operator ==(XnaBoundingBox a, XnaBoundingBox b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(XnaBoundingBox a, XnaBoundingBox b)
        {
            return !a.Equals(b);
        }
        public override string ToString()
        {
            return string.Format("{{Min:{0} Max:{1}}}", this.Min.ToString(), this.Max.ToString());
        }
        public bool Equals(XnaBoundingBox other)
        {
            return Min.Equals(other.Min) && this.Max.Equals(other.Max);
        }
        public override bool Equals(object obj)
        {
            return (obj is XnaBoundingBox) && this.Equals((XnaBoundingBox)obj);
        }
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }

    }

    public static class XnaBoundingBoxHelper
    {
        #region Public Methods
        public static ContainmentType Contains(this XnaBoundingBox self, XnaBoundingBox box)
        {
            //test if all corner is in the same side of a face by just checking min and max
            if (box.Max.x < self.Min.x
                || box.Min.x > self.Max.x
                || box.Max.y < self.Min.y
                || box.Min.y > self.Max.y
                || box.Max.z < self.Min.z
                || box.Min.z > self.Max.z)
                return ContainmentType.Disjoint;


            if (box.Min.x >= self.Min.x
                && box.Max.x <= self.Max.x
                && box.Min.y >= self.Min.y
                && box.Max.y <= self.Max.y
                && box.Min.z >= self.Min.z
                && box.Max.z <= self.Max.z)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }
        public static void Contains(this XnaBoundingBox self, ref XnaBoundingBox box, out ContainmentType result)
        {
            result = Contains(self, box);
        }
        public static ContainmentType Contains(this XnaBoundingBox self, XnaBoundingFrustum frustum)
        {
            //TODO: bad done here need a fix. 
            //Because question is not frustum contain box but reverse and this is not the same
            int i;
            ContainmentType contained;
            float3[] corners = frustum.GetCorners();

            // First we check if frustum is in box
            for (i = 0; i < corners.Length; i++)
            {
                Contains(self, ref corners[i], out contained);
                if (contained == ContainmentType.Disjoint)
                    break;

            }

            if (i == corners.Length) // This means we checked all the corners and they were all contain or instersect
                return ContainmentType.Contains;

            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
                return ContainmentType.Intersects;


            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < corners.Length; i++)
            {
                Contains(self, ref corners[i], out contained);
                if (contained != ContainmentType.Contains)
                    return ContainmentType.Intersects;

            }

            // If we get here, then we know all the points were actually contained, therefore result is Contains
            return ContainmentType.Contains;
        }
        public static ContainmentType Contains(this XnaBoundingBox self, XnaBoundingSphere sphere)
        {
            if (sphere.Position.x - self.Min.x > sphere.Radius
                && sphere.Position.y - self.Min.y > sphere.Radius
                && sphere.Position.z - self.Min.z > sphere.Radius
                && self.Max.x - sphere.Position.x > sphere.Radius
                && self.Max.y - sphere.Position.y > sphere.Radius
                && self.Max.z - sphere.Position.z > sphere.Radius)
                return ContainmentType.Contains;

            double dmin = 0;

            if (sphere.Position.x - self.Min.x <= sphere.Radius)
                dmin += (sphere.Position.x - self.Min.x) * (sphere.Position.x - self.Min.x);
            else if (self.Max.x - sphere.Position.x <= sphere.Radius)
                dmin += (sphere.Position.x - self.Max.x) * (sphere.Position.x - self.Max.x);
            if (sphere.Position.y - self.Min.y <= sphere.Radius)
                dmin += (sphere.Position.y - self.Min.y) * (sphere.Position.y - self.Min.y);
            else if (self.Max.y - sphere.Position.y <= sphere.Radius)
                dmin += (sphere.Position.y - self.Max.y) * (sphere.Position.y - self.Max.y);
            if (sphere.Position.z - self.Min.z <= sphere.Radius)
                dmin += (sphere.Position.z - self.Min.z) * (sphere.Position.z - self.Min.z);
            else if (self.Max.z - sphere.Position.z <= sphere.Radius)
                dmin += (sphere.Position.z - self.Max.z) * (sphere.Position.z - self.Max.z);

            if (dmin <= sphere.Radius * sphere.Radius)
                return ContainmentType.Intersects;

            return ContainmentType.Disjoint;
        }
        public static void Contains(this XnaBoundingBox self, ref XnaBoundingSphere sphere, out ContainmentType result)
        {
            result = Contains(self, sphere);
        }
        public static ContainmentType Contains(this XnaBoundingBox self, float3 point)
        {
            ContainmentType result;
            Contains(self, ref point, out result);
            return result;
        }
        public static void Contains(this XnaBoundingBox self, ref float3 point, out ContainmentType result)
        {
            //first we get if point is out of box
            if (point.x < self.Min.x
                || point.x > self.Max.x
                || point.y < self.Min.y
                || point.y > self.Max.y
                || point.z < self.Min.z
                || point.z > self.Max.z)
            {
                result = ContainmentType.Disjoint;
            }//or if point is on box because coordonate of point is lesser or equal
            else if (point.x == self.Min.x
                || point.x == self.Max.x
                || point.y == self.Min.y
                || point.y == self.Max.y
                || point.z == self.Min.z
                || point.z == self.Max.z)
                result = ContainmentType.Intersects;
            else
                result = ContainmentType.Contains;


        }
        public static XnaBoundingBox CreateFromPoints(this XnaBoundingBox self, IEnumerable<float3> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            // TODO: Just check that Count > 0
            bool empty = true;
            float3 vector2 = new float3(float.MaxValue);
            float3 vector1 = new float3(float.MinValue);
            foreach (float3 vector3 in points)
            {
                vector2 = math.min(vector2, vector3);
                vector1 = math.max(vector1, vector3);
                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return FromMinMax(vector2, vector1);
        }

        public static XnaBoundingBox FromMinMax(float3 min, float3 max)
        {
            return new XnaBoundingBox
            {
                Max = max,
                Min = min
            };
        }

        public static XnaBoundingBox CreateFromSphere(this XnaBoundingBox self, XnaBoundingSphere sphere)
        {
            float3 vector1 = new float3(sphere.Radius, sphere.Radius, sphere.Radius);
            return FromMinMax(sphere.Position - vector1, sphere.Position + vector1);
        }
        public static void CreateFromSphere(this XnaBoundingBox self, ref XnaBoundingSphere sphere, out XnaBoundingBox result)
        {
            result = CreateFromSphere(self, sphere);
        }
        public static XnaBoundingBox CreateMerged(this XnaBoundingBox self, XnaBoundingBox original, XnaBoundingBox additional)
        {
            return FromMinMax(
                math.min(original.Min, additional.Min), math.max(original.Max, additional.Max));
        }
        public static void CreateMerged(this XnaBoundingBox self, ref XnaBoundingBox original, ref XnaBoundingBox additional, out XnaBoundingBox result)
        {
            result = CreateMerged(self, original, additional);
        }
        public static float3[] GetCorners(this XnaBoundingBox self)
        {
            return new float3[] {
                new float3(self.Min.x, self.Max.y, self.Max.z),
                new float3(self.Max.x, self.Max.y, self.Max.z),
                new float3(self.Max.x, self.Min.y, self.Max.z),
                new float3(self.Min.x, self.Min.y, self.Max.z),
                new float3(self.Min.x, self.Max.y, self.Min.z),
                new float3(self.Max.x, self.Max.y, self.Min.z),
                new float3(self.Max.x, self.Min.y, self.Min.z),
                new float3(self.Min.x, self.Min.y, self.Min.z)
            };
        }
        public static void GetCorners(this XnaBoundingBox self, float3[] corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException("corners");
            }
            if (corners.Length < 8)
            {
                throw new ArgumentOutOfRangeException("corners", "Not Enought Corners");
            }
            corners[0].x = self.Min.x;
            corners[0].y = self.Max.y;
            corners[0].z = self.Max.z;
            corners[1].x = self.Max.x;
            corners[1].y = self.Max.y;
            corners[1].z = self.Max.z;
            corners[2].x = self.Max.x;
            corners[2].y = self.Min.y;
            corners[2].z = self.Max.z;
            corners[3].x = self.Min.x;
            corners[3].y = self.Min.y;
            corners[3].z = self.Max.z;
            corners[4].x = self.Min.x;
            corners[4].y = self.Max.y;
            corners[4].z = self.Min.z;
            corners[5].x = self.Max.x;
            corners[5].y = self.Max.y;
            corners[5].z = self.Min.z;
            corners[6].x = self.Max.x;
            corners[6].y = self.Min.y;
            corners[6].z = self.Min.z;
            corners[7].x = self.Min.x;
            corners[7].y = self.Min.y;
            corners[7].z = self.Min.z;
        }
        public static bool Intersects(this XnaBoundingBox self, XnaBoundingBox box)
        {
            bool result;
            Intersects(self, ref box, out result);
            return result;
        }
        public static void Intersects(this XnaBoundingBox self, ref XnaBoundingBox box, out bool result)
        {
            if ((self.Max.x >= box.Min.x) && (self.Min.x <= box.Max.x))
            {
                if ((self.Max.y < box.Min.y) || (self.Min.y > box.Max.y))
                {
                    result = false;
                    return;
                }

                result = (self.Max.z >= box.Min.z) && (self.Min.z <= box.Max.z);
                return;
            }

            result = false;
            return;
        }
        public static bool Intersects(this XnaBoundingBox self, XnaBoundingFrustum frustum)
        {
            return frustum.Intersects(self);
        }
        public static bool Intersects(this XnaBoundingBox self, XnaBoundingSphere sphere)
        {
            if (sphere.Position.x - self.Min.x > sphere.Radius
                && sphere.Position.y - self.Min.y > sphere.Radius
                && sphere.Position.z - self.Min.z > sphere.Radius
                && self.Max.x - sphere.Position.x > sphere.Radius
                && self.Max.y - sphere.Position.y > sphere.Radius
                && self.Max.z - sphere.Position.z > sphere.Radius)
                return true;

            double dmin = 0;

            if (sphere.Position.x - self.Min.x <= sphere.Radius)
                dmin += (sphere.Position.x - self.Min.x) * (sphere.Position.x - self.Min.x);
            else if (self.Max.x - sphere.Position.x <= sphere.Radius)
                dmin += (sphere.Position.x - self.Max.x) * (sphere.Position.x - self.Max.x);

            if (sphere.Position.y - self.Min.y <= sphere.Radius)
                dmin += (sphere.Position.y - self.Min.y) * (sphere.Position.y - self.Min.y);
            else if (self.Max.y - sphere.Position.y <= sphere.Radius)
                dmin += (sphere.Position.y - self.Max.y) * (sphere.Position.y - self.Max.y);

            if (sphere.Position.z - self.Min.z <= sphere.Radius)
                dmin += (sphere.Position.z - self.Min.z) * (sphere.Position.z - self.Min.z);
            else if (self.Max.z - sphere.Position.z <= sphere.Radius)
                dmin += (sphere.Position.z - self.Max.z) * (sphere.Position.z - self.Max.z);

            if (dmin <= sphere.Radius * sphere.Radius)
                return true;

            return false;
        }
        public static void Intersects(this XnaBoundingBox self, ref XnaBoundingSphere sphere, out bool result)
        {
            result = Intersects(self, sphere);
        }
        public static PlaneIntersectionType Intersects(this XnaBoundingBox self, XnaPlane plane)
        {
            PlaneIntersectionType result;
            Intersects(self, ref plane, out result);
            return result;
        }
        public static void Intersects(this XnaBoundingBox self, ref XnaPlane plane, out PlaneIntersectionType result)
        {
            // See http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html

            float3 positiveVertex;
            float3 negativeVertex;

            if (plane.Normal.x >= 0)
            {
                positiveVertex.x = self.Max.x;
                negativeVertex.x = self.Min.x;
            }
            else
            {
                positiveVertex.x = self.Min.x;
                negativeVertex.x = self.Max.x;
            }

            if (plane.Normal.y >= 0)
            {
                positiveVertex.y = self.Max.y;
                negativeVertex.y = self.Min.y;
            }
            else
            {
                positiveVertex.y = self.Min.y;
                negativeVertex.y = self.Max.y;
            }

            if (plane.Normal.z >= 0)
            {
                positiveVertex.z = self.Max.z;
                negativeVertex.z = self.Min.z;
            }
            else
            {
                positiveVertex.z = self.Min.z;
                negativeVertex.z = self.Max.z;
            }

            var distance = math.dot(plane.Normal, negativeVertex) + plane.D;
            if (distance > 0)
            {
                result = PlaneIntersectionType.Front;
                return;
            }

            distance = math.dot(plane.Normal, positiveVertex) + plane.D;
            if (distance < 0)
            {
                result = PlaneIntersectionType.Back;
                return;
            }

            result = PlaneIntersectionType.Intersecting;
        }
        public static Nullable<double> Intersects(this XnaBoundingBox self, XnaRay ray)
        {
            return ray.Intersects(self);
        }
        public static void Intersects(this XnaBoundingBox self, ref XnaRay ray, out Nullable<double> result)
        {
            result = Intersects(self, ray);
        }


        #endregion Public Methods
    }


}