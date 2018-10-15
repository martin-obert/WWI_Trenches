using Assets.Gameplay.Units;
using UnityEngine.Events;

namespace Assets.Gameplay
{
    public class PlayerCharacterState : ICharacterState<Player>
    {
        public UnityEvent<Player> StateChanged { get; } = new PlayerStateChangedEvent();

        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

        public void ChangeState(PlayerState state, Player player)
        {
            CurrentState = state;
            StateChanged.Invoke(player);
        }
    }
}