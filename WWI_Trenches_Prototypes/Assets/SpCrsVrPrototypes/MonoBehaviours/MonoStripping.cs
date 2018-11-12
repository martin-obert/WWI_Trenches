using System.Collections.Generic;
using Assets.ObjAnimations;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Assets.SpCrsVrPrototypes.Enums;
using Assets.SpCrsVrPrototypes.Singletons;
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
        [SerializeField] private RayCastFlag _rayCastFlag;
        [SerializeField] private bool _IsSelectable;
        [SerializeField] private float _StoppingRadius;
        [SerializeField] private float _TurningSpeed;
        [SerializeField] private float _InitialVelocity;
        [SerializeField] private float _MoveSpeed;

        public string UniqueName => _uniqueName;

        public ObjAnimationSO IdleAnimation;


        public EntityData DefaultUnitData { get; private set; }

        public void StripEntityArchetype(EntityManager manager)
        {
            var components = new List<ComponentType>();

            DefaultUnitData = new EntityData();

            if (_stripBoundingVolume)
            {
                if (GetComponent<SphereCollider>())
                {
                    components.Add(typeof(XnaBoundingSphere));
                    DefaultUnitData.SphereRadius = GetComponent<SphereCollider>().radius;
                    DefaultUnitData.SphereOffset = GetComponent<SphereCollider>().center;
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
                    DefaultUnitData.Material =   GetComponent<MeshRenderer>().sharedMaterial;
                }
                else if (GetComponent<MeshRenderer>())
                {
                    components.Add(typeof(InstancedSelectableMeshRenderer));
                    DefaultUnitData.Material = GetComponent<MeshRenderer>().sharedMaterial;
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
                    DefaultUnitData.Position = transform.position;
                    DefaultUnitData.Rotation = transform.rotation;
                    DefaultUnitData.Scale = transform.lossyScale;
                }
            }

            if (_IsSelectable)
            {
                components.Add(typeof(RayCastData));
            }

            components.Add(typeof(Navigation));

            foreach (var componentType in components)
            {
                print(componentType);
            }

            DefaultUnitData.TurningSpeed = _TurningSpeed;
            DefaultUnitData.InitialVelocity = _InitialVelocity;
            DefaultUnitData.StoppingRadius = _StoppingRadius;
            DefaultUnitData.MoveSpeed = _MoveSpeed;


            DefaultUnitData.Archetype = manager.CreateArchetype(components.ToArray());

            DefaultUnitData.Animations = new Dictionary<AnimationType, ObjAnimationSoCache>
            {
                {AnimationType.Idle,new ObjAnimationSoCache
                {
                    Meshes = IdleAnimation.ToMesh(),
                    FrameRate = IdleAnimation. FrameRate,FrameCount = IdleAnimation.SubMeshCount
                }}
            };
        }
    }
}
