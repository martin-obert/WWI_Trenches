using System;
using UnityEngine;

namespace Assets.Gameplay.Character
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