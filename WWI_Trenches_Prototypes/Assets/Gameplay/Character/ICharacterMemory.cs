using System;

namespace Assets.Gameplay.Character
{
    public class CharacterMemoryEventArgs : EventArgs
    {
     

        public BasicStance LastStance { get; set; }

        public BasicStance CurrentStance { get; set; }
    }

    public interface ICharacterMemory<T>
    {
        event EventHandler<CharacterMemoryEventArgs> StateChanged;

        void ChangeStance(BasicStance stance);

        BasicStance LastStance { get; }

        BasicStance CurrentStance { get; }
    }
}