using System;
using UnityEngine;

namespace Assets.ObjAnimations
{
    [Serializable]
    public class ObjAnimationBakedFrame
    {
        public Vector3[] Vertices { get; set; }
        public int[] Indices { get; set; }
    }
}