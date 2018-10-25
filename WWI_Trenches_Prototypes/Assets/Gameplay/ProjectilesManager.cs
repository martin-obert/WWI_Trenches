using System.Collections.Generic;
using System.Linq;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Inventory.Items;

namespace Assets.Gameplay
{
    public class ProjectilesManager : Singleton<ProjectilesManager>
    {
        private readonly IDictionary<int, IDictionary<int, IProjectile[]>> _characterWeaponProjectiles = new Dictionary<int, IDictionary<int, IProjectile[]>>();

        protected override void OnAwakeHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            foreach (var characterWeaponProjectile in _characterWeaponProjectiles)
            {
                characterWeaponProjectile.Value.Clear();
            }

            _characterWeaponProjectiles.Clear();

            GCSingleton(this);
        }

        public void RegisterWeapon(int characterId, IWeapon weapon)
        {
            var weaponId = weapon.Id;

            IDictionary<int, IProjectile[]> weaponProjectiles;

            if (!_characterWeaponProjectiles.TryGetValue(characterId, out weaponProjectiles))
            {
                _characterWeaponProjectiles.Add(characterId, weaponProjectiles = new Dictionary<int, IProjectile[]>());
            }

            IProjectile[] projectiles;

            if (weaponProjectiles.TryGetValue(weaponId, out projectiles))
                return;

            projectiles = new IProjectile[weapon.Data.ClipMaxSize];

            for (var i = 0; i < projectiles.Length; i++)
            {
                projectiles[i] = Instantiate(weapon.Data.Projectile, transform);
                projectiles[i].ResetToStack();
            }

            weaponProjectiles.Add(weaponId, projectiles);
        }

        public void UnregisterWeapon(int characterId, IWeapon weapon)
        {
            IDictionary<int, IProjectile[]> wepons;

            if (!_characterWeaponProjectiles.TryGetValue(characterId, out wepons)) return;

            wepons.Remove(weapon.Id);
        }

        public void ShootProjectile(RangedWeapon weapon)
        {
            var dir = weapon.Target.GameObject.transform.position - weapon.ProjectileSpawnLocation;

            _characterWeaponProjectiles[weapon.Owner.Id][weapon.Id].FirstOrDefault(x => !x.IsFired)?.Shoot(weapon.ProjectileSpawnLocation, dir.normalized);
        }
    }
}