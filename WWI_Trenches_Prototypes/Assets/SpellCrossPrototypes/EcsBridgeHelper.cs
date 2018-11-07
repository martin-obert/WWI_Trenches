using Unity.Transforms;
using UnityEngine;

namespace Assets.SpellCrossPrototypes
{
    public static class EcsBridgeHelper
    {
        public static Position StripPosition(this GameObject g)
        {
            var position = new Position
            {
                Value = g.transform.position
            };
            return position;
        }

        public static bool HasBehavior<T>(this GameObject g) where T : MonoBehaviour
        {
            return g.GetComponent<T>();
        }


    }
}