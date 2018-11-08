using System.Collections.Generic;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.MonoBehaviours;
using Unity.Entities;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public class EntityData
    {
        public MonoStripping Stripping { get; set; }

        public EntityArchetype Archetype { get; set; }

        public Material Material { get; set; }

        public IDictionary<AnimationType, ObjAnimationSoCache> Animations { get; set; }
    }
}