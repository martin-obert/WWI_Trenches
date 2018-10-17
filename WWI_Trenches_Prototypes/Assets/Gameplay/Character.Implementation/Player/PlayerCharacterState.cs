using System;
using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerCharacterState : ICharacterState<PlayerController>
    {
        public event EventHandler<IOrderArguments<PlayerController>> StateChanged;

        public void ChangeStance(CharacterStance stance, IOrderArguments<PlayerController> orderArguments)
        {
            var invoke = stance != CurrentStance;

            CurrentStance = stance;

            if (invoke)
                StateChanged?.Invoke(this, orderArguments);
        }

        public CharacterStance CurrentStance { get; private set; } = CharacterStance.Idle;
    }
}