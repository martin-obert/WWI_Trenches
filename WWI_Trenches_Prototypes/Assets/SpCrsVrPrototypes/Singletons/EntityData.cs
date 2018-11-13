using System.Collections.Generic;
using Assets.ObjAnimations;
using Unity.Entities;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public class EntityData
    {
        public EntityArchetype Archetype { get; set; }

        public Material Material { get; set; }

        public IDictionary<AnimationType, ObjAnimationSoCache> Animations { get; set; }
        public float SphereRadius { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Vector3 SphereOffset { get; set; }
        public float TurningSpeed { get; set; }
        public float StoppingRadius { get; set; }
        public float MoveSpeed { get; set; }
        public float InitialVelocity { get; set; }
        public float Health { get; set; }
    }
}