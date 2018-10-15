using UnityEngine.Events;

namespace Assets.Gameplay
{
    public interface ICharacterState<T>
    {
        UnityEvent<T> StateChanged { get; }
    }
}