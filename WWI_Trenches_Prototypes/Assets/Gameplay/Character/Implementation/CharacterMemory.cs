using System;

namespace Assets.Gameplay.Character.Implementation
{
    public class CharacterMemory : ICharacterMemory<BasicCharacter>
    {
        public event EventHandler<CharacterMemoryStateEventArgs> StateChanged;
        public event EventHandler<CharacterMemoryAttitudeEventArgs> AttitudeChanged;

        private readonly CharacterMemoryStateEventArgs _args;
        private readonly CharacterMemoryAttitudeEventArgs _attitudeArgs;

        public CharacterMemory()
        {
            _args = new CharacterMemoryStateEventArgs();
            _attitudeArgs = new CharacterMemoryAttitudeEventArgs();
        }



        public void ChangeAttitude(Attitude attitude)
        {
            if (attitude == _attitudeArgs.CurrentAttitude) return;

            _attitudeArgs.LastAttitude = _attitudeArgs.CurrentAttitude;
            _attitudeArgs.CurrentAttitude = attitude;

            AttitudeChanged?.Invoke(this, _attitudeArgs);
        }

        public void ChangeStance(BasicStance stance)
        {
            if (stance == CurrentStance) return;

            _args.LastStance = _args.CurrentStance;

            _args.CurrentStance = stance;

            StateChanged?.Invoke(this, _args);
        }


        public BasicStance CurrentStance => _args.CurrentStance;
        public Attitude CurrentAttitude => _attitudeArgs.CurrentAttitude;
    }
}