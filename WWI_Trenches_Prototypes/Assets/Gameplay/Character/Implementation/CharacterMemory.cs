using System;

namespace Assets.Gameplay.Character.Implementation
{
    public class CharacterMemory : ICharacterMemory<BasicCharacter>
    {
        public event EventHandler<CharacterMemoryEventArgs> StateChanged;

        private readonly CharacterMemoryEventArgs _args;

        public CharacterMemory()
        {
            _args = new CharacterMemoryEventArgs();
        }

        public void ChangeStance(BasicStance stance)
        {
            if (stance == CurrentStance) return;

            _args.LastStance = _args.CurrentStance;

            _args.CurrentStance = stance;

            StateChanged?.Invoke(this, _args);
        }

        public BasicStance LastStance => _args.LastStance;

        public BasicStance CurrentStance => _args.CurrentStance;
    }
}