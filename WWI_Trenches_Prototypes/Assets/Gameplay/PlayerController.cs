using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character;
using Assets.Gameplay.Character.Implementation;

namespace Assets.Gameplay
{
    public class PlayerController : Singleton<PlayerController>
    {
        public ICharacterProxy<BasicCharacter> Character;

        protected override void OnEnableHandle()
        {
            CreateSingleton(this);

        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);

        }
    }
}
