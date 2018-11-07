using System;
using UnityEngine;

namespace Assets.ObjAnimations
{
    [Serializable]
    public class ObjAnimationSo  : ScriptableObject
    {
        public Mesh[] FrameMeshes;

        public string Name;

        public bool IsRepeatable;
    }
}