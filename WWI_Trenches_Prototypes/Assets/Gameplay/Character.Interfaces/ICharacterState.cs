using UnityEngine.Events;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterState<T>
    {
        UnityEvent<T> StateChanged { get; }
    }
}