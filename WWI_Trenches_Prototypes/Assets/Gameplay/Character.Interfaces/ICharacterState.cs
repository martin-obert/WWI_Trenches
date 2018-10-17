using System;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterState<T>
    {
        event EventHandler<IOrderArguments<T>> StateChanged;

        void ChangeStance(CharacterStance stance, IOrderArguments<T> orderArguments);

        CharacterStance CurrentStance { get; }
    }
}