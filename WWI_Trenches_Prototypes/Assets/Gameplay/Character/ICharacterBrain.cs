using Assets.Gameplay.Attributes;
using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface ICharacterBrain<TCharacter> : ISequenceExecutor
    {
        ICharacterMemory<TCharacter> Memory { get; }
    }

    public interface IHumanoidSkeletonProxy
    {
        IHumanoidSkeletonHandProxy LeftHandProxy { get; }
        IHumanoidSkeletonHandProxy RightHandProxy { get; }
    }

    public interface IHumanoidSkeletonHandProxy
    {
        Transform Palm { get; }

        Transform IKTarget { get; }

        BasicAttribute<float> PositionWeight { get; }

        BasicAttribute<float> RotationWeight { get; }
    }
}
