using System.Collections.Generic;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.XnaLegacy;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.SpCrsVrPrototypes.MonoBehaviours
{
    public class MonoStripping : MonoBehaviour
    {
        [SerializeField] private bool _stripTransforms;
        [SerializeField] private bool _stripMesh;
        [SerializeField] private bool _stripMaterial;
        [SerializeField] private bool _stripBoundingVolume;
        [SerializeField] private string _uniqueName;
        [SerializeField] private bool _hasAnimations;

        public string UniqueName => _uniqueName;
        public ObjAnimationSo IdleAnimation;

        public EntityArchetype StripEntityArchetype(EntityManager manager)
        {
            var components = new List<ComponentType>();

            if (_stripBoundingVolume)
            {
                if (GetComponent<SphereCollider>())
                {
                    components.Add(typeof(XnaBoundingSphere));
                }
                else if (GetComponent<BoxCollider>())
                {
                    components.Add(typeof(XnaBoundingBox));
                }
                else
                {
                    Debug.LogWarning("Cannot strip bounding volume. None was found or compatible!");
                }
            }

            if (_stripMesh)
            {
                if (GetComponent<SkinnedMeshRenderer>() || _hasAnimations)
                {
                    components.Add(typeof(AnimatedMeshSequence));
                }
                else if (GetComponent<MeshRenderer>())
                {
                    components.Add(typeof(InstancedSelectableMeshRenderer));
                }
                else
                {
                    Debug.LogWarning("Cannot strip mesh. None was found or compatible!");
                }
            }

            if (_stripTransforms)
            {
                if (GetComponent<Transform>())
                {
                    components.AddRange(new ComponentType[] { typeof(Position), typeof(Rotation), typeof(Scale) });
                }
            }

            foreach (var componentType in components)
            {
                print(componentType);
            }

            return manager.CreateArchetype(components.ToArray());
        }

        public Material GetMaterial()
        {
            return
                GetComponent<MeshRenderer>()?.sharedMaterial ?? GetComponent<SkinnedMeshRenderer>()?.sharedMaterial;
        }
    }
}
