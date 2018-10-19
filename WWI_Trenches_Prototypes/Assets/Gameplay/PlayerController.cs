using Assets.Gameplay.Character;
using Assets.Gameplay.Character.Implementation;
using UnityEngine;

namespace Assets.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        public ICharacterProxy<BasicCharacter> Character;
    }
}
