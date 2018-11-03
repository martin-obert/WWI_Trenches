using System;
using Unity.Mathematics;

namespace Assets.XnaLegacy
{
    public struct XnaRay : IEquatable<XnaRay>
    {
        #region Public Fields
        public float3 Direction;
        public float3 Position;

        #endregion
     
        public override bool Equals(object obj)
        {
            return (obj is XnaRay) ? this.Equals((XnaRay)obj) : false;
        }

        public bool Equals(XnaRay other)
        {
            return this.Position.Equals(other.Position) && this.Direction.Equals(other.Direction);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Direction.GetHashCode();
        }

       

        public static bool operator !=(XnaRay a, XnaRay b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(XnaRay a, XnaRay b)
        {
            return a.Equals(b);
        }

        public override string ToString()
        {
            return string.Format("{{Position:{0} Direction:{1}}}", Position.ToString(), Direction.ToString());
        }
    }

    public static class XnaRayHelper
    {
        public static float? Intersects(this XnaRay ray, XnaBoundingBox box)
        {
            //first test if start in box
            if (ray.Position.x >= box.Min.x
                && ray.Position.x <= box.Max.x
                && ray.Position.y >= box.Min.y
                && ray.Position.y <= box.Max.y
                && ray.Position.z >= box.Min.z
                && ray.Position.z <= box.Max.z)
                return 0.0f;// here we concidere cube is full and origine is in cube so intersect at origine


            //Second we check each face
            float3 maxT = new float3(-1.0f);
            //float3 minT = new float3(-1.0f);
            //calcul intersection with each faces
            if (ray.Position.x < box.Min.x && ray.Direction.x != 0.0f)
                maxT.x = (box.Min.x - ray.Position.x) / ray.Direction.x;
            else if (ray.Position.x > box.Max.x && ray.Direction.x != 0.0f)
                maxT.x = (box.Max.x - ray.Position.x) / ray.Direction.x;
            if (ray.Position.y < box.Min.y && ray.Direction.y != 0.0f)
                maxT.y = (box.Min.y - ray.Position.y) / ray.Direction.y;
            else if (ray.Position.y > box.Max.y && ray.Direction.y != 0.0f)
                maxT.y = (box.Max.y - ray.Position.y) / ray.Direction.y;
            if (ray.Position.z < box.Min.z && ray.Direction.z != 0.0f)
                maxT.z = (box.Min.z - ray.Position.z) / ray.Direction.z;
            else if (ray.Position.z > box.Max.z && ray.Direction.z != 0.0f)
                maxT.z = (box.Max.z - ray.Position.z) / ray.Direction.z;


            //get the maximum maxT
            if (maxT.x > maxT.y && maxT.x > maxT.z)
            {
                if (maxT.x < 0.0f)
                    return null;// ray go on opposite of face
                //coordonate of hit point of face of cube
                double coord = ray.Position.z + maxT.x * ray.Direction.z;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                if (coord < box.Min.z || coord > box.Max.z)
                    return null;
                coord = ray.Position.y + maxT.x * ray.Direction.y;
                if (coord < box.Min.y || coord > box.Max.y)
                    return null;
                return maxT.x;
            }
            if (maxT.y > maxT.x && maxT.y > maxT.z)
            {
                if (maxT.y < 0.0f)
                    return null;// ray go on opposite of face
                //coordonate of hit point of face of cube
                double coord = ray.Position.z + maxT.y * ray.Direction.z;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                if (coord < box.Min.z || coord > box.Max.z)
                    return null;
                coord = ray.Position.x + maxT.y * ray.Direction.x;
                if (coord < box.Min.x || coord > box.Max.x)
                    return null;
                return maxT.y;
            }
            else //Z
            {
                if (maxT.z < 0.0f)
                    return null;// ray go on opposite of face
                //coordonate of hit point of face of cube
                double coord = ray.Position.x + maxT.z * ray.Direction.x;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                if (coord < box.Min.x || coord > box.Max.x)
                    return null;
                coord = ray.Position.y + maxT.z * ray.Direction.y;
                if (coord < box.Min.y || coord > box.Max.y)
                    return null;
                return maxT.z;
            }
        }

        public static void Intersects(this XnaRay ray, ref XnaBoundingBox box, out float? result)
        {
            result = ray.Intersects(box);
        }
        public static double? Intersects(this XnaRay ray, XnaBoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException("frustum");
            }
            return frustum.Intersects(ray);
        }
        public static float? Intersects(this XnaRay ray, XnaBoundingSphere sphere)
        {
            float? result;
            ray.Intersects(ref sphere, out result);
            return result;
        }
        public static float? Intersects(this XnaRay ray, XnaPlane plane)
        {
            float? result;
            ray.Intersects(ref plane, out result);
            return result;
        }
        public static void Intersects(this XnaRay ray, ref XnaPlane plane, out float? result)
        {
            var den = math.dot(ray.Direction, plane.Normal);
            if (math.abs(den) < 0.00001f)
            {
                result = null;
                return;
            }


            result = (-(float)plane.D - math.dot(plane.Normal, ray.Position)) / den;


            if (result < 0.0f)
            {
                if (result < -0.00001f)
                {
                    result = null;
                    return;
                }


                result = 0.0f;
            }
        }
        public static void Intersects(this XnaRay ray, ref XnaBoundingSphere sphere, out float? result)
        {
            // Find the vector between where the ray starts the the sphere's centre
            float3 difference = sphere.Position - ray.Position;


            float differenceLengthSquared = math.lengthsq(difference);
            float sphereRadiusSquared = sphere.Radius * sphere.Radius;


            float distanceAlongRay;


            // If the distance between the ray start and the sphere's centre is less than
            // the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
            if (differenceLengthSquared < sphereRadiusSquared)
            {
                result = 0.0f;
                return;
            }


            distanceAlongRay = math.dot(ray.Direction, difference);
            // If the ray is pointing away from the sphere then we don't ever intersect
            if (distanceAlongRay < 0)
            {
                result = null;
                return;
            }


            // Next we kinda use Pythagoras to check if we are within the bounds of the sphere
            // if x = radius of sphere
            // if y = distance between ray position and sphere centre
            // if z = the distance we've travelled along the ray
            // if x^2 + z^2 - y^2 < 0, we do not intersect
            double dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;


            result = dist < 0 ? null : distanceAlongRay - (float?)math.sqrt(dist);
        }
    }

    public enum PlaneIntersectionType
    {
        Front,
        Back,
        Intersecting
    }
}
