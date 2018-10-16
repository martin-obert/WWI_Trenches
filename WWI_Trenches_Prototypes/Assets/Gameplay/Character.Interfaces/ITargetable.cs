using System;
using UnityEngine;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ITargetable
    {
        string DisplayName { get; }

        GameObject GameObject { get; }

        event EventHandler<ITargetable> EliminatedByOtherTarget;

        void GotKilledBy(ITargetable killer);
    }
}