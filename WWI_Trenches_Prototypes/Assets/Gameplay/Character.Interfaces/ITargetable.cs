using System;
using Assets.Gameplay.Character.Implementation.Enemies;
using UnityEngine;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ITargetable
    {
        string DisplayName { get; }

        GameObject GameObject { get; }

        event EventHandler<ITargetable> EliminatedByOtherTarget;

        void GotKilledBy(ITargetable killer);

        bool IsVisibleTo(ITargetable targetable);

        event EventHandler VisibilityChanged;
    }
}