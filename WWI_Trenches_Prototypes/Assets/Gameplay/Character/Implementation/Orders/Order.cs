using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Orders
{
    public abstract class Order : ScriptableObject, ICharacterOrder<BasicCharacter>
    {
        protected Order(string name)
        {
            Name = name;
        }

        public void Execute(IOrderArguments<BasicCharacter> arguments)
        {
            Execute((CharacterOrderArguments)arguments);
        }

        public string Name { get; }

        public void Execute<T>(IOrderArguments<T> arguments)
        {
            Execute((CharacterOrderArguments)arguments);
        }

        public abstract void Execute(CharacterOrderArguments arguments);
    }
    }