using Assets.Gameplay.Attributes;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface IHumanoidSkeletonHandProxy
    {
        Transform Palm { get; }

        Transform IKTarget { get; }

        BasicAttribute<float> PositionWeight { get; }

        BasicAttribute<float> RotationWeight { get; }
    }
}