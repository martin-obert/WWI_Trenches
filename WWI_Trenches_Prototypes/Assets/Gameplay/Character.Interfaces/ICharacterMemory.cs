using System;

namespace Assets.Gameplay.Character.Interfaces
{
    public class CharacterMemoryEventArgs : EventArgs
    {
     

        public CharacterStance LastStance { get; set; }

        public CharacterStance CurrentStance { get; set; }
    }

    public interface ICharacterMemory<T>
    {
        event EventHandler<CharacterMemoryEventArgs> StateChanged;

        void ChangeStance(CharacterStance stance);

        CharacterStance LastStance { get; }

        CharacterStance CurrentStance { get; }
    }
}