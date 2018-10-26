using System;

namespace Assets.Gameplay.Character
{
    public class CharacterMemoryStateEventArgs : EventArgs
    {
        public BasicStance LastStance { get; set; }

        public BasicStance CurrentStance { get; set; }
    }

    public class CharacterMemoryAttitudeEventArgs : EventArgs
    {
        public Attitude LastAttitude { get; set; }
        public Attitude CurrentAttitude { get; set; }

    }

    public interface ICharacterMemory<T>
    {
        event EventHandler<CharacterMemoryStateEventArgs> StateChanged;

        event EventHandler<CharacterMemoryAttitudeEventArgs> AttitudeChanged;

        void ChangeAttitude(Attitude attitude);

        void ChangeStance(BasicStance stance);

        BasicStance CurrentStance { get; }

        Attitude CurrentAttitude { get; }
    }
}