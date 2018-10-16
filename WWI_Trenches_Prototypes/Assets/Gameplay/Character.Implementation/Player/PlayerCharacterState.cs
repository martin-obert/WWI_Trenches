using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Units;
using UnityEngine.Events;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerCharacterState : ICharacterState<PlayerController>
    {
        public UnityEvent<PlayerController> StateChanged { get; } = new PlayerStateChangedEvent();

        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

        public void ChangeState(PlayerState state, PlayerController player)
        {
            var invoke = state != CurrentState;

            CurrentState = state;

            if (invoke)
                StateChanged.Invoke(player);
        }
    }
}