using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation;
using Assets.IoC;

namespace Assets.Gameplay.Factories
{
    public class CharacterFactory : Singleton<CharacterFactory>
    {
        private Bootstrapper _bootstrapper;

        protected override void OnEnableHandle()
        {
            Dependency<Bootstrapper>(x => _bootstrapper = x);
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }

        public BasicCharacter CreatePlayerInstance()
        {
            var instance = Instantiate(_bootstrapper.PlayerPrefab);

            var itemInstance = Instantiate(_bootstrapper.PlayerMainWeapon);

            //Todo: equipment provider
            instance.Equipment.MainWeapon.SetItem(itemInstance, instance);


            return instance;
        }
    }
}
