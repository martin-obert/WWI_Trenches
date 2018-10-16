using System;
using UnityEngine.Events;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterState<T>
    {
        event EventHandler<T> StateChanged;
    }
}