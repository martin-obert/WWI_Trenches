using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character;
using Assets.Gameplay.Character.Implementation;
using UnityEngine;

namespace Assets.Gameplay
{
    public class PlayerController : Singleton<PlayerController>
    {
        public ICharacterProxy<BasicCharacter> Character;

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }
    }
}
