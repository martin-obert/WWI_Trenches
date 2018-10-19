using System;
using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Instructions;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterBrain<TCharacter> : ISequenceExecutor
    {
        //Podle me si tim statem zadelavam na rekurzi a endless loop

        ICharacterMemory<TCharacter> Memory { get; }
       
    }

    

    //Todo: tohle prejmenuj na character a udelej abstractni class s CharacterProxy
    public interface ICharacterProxy<TCharacter> : ITargetable, IProjectileTrigger
    {
      
        IOrderArguments<TCharacter> OrderArguments { get; }

        void Attack();

        void ChangeCourse();

        void Stop();

        void Shoot();
    }
}
