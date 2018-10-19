using System;
using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player
{
    [Obsolete("User character brain")]
    public class PlayerCharacterMemory : ICharacterMemory<PlayerController>
    {
        public event EventHandler<CharacterMemoryEventArgs> StateChanged;
        public void ChangeStance(CharacterStance stance)
        {
            throw new NotImplementedException();
        }

        public CharacterStance LastStance { get; }
        public CharacterStance CurrentStance { get; }
    }
}