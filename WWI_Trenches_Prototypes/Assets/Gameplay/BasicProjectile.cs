using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Inventory.Items;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay
{
    public class ProjectilesManager : Singleton<ProjectilesManager>
    {
        private IDictionary<int, IDictionary<int, IProjectile[]>> _characterWeaponProjectiles = new Dictionary<int, IDictionary<int, IProjectile[]>>();

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
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

        public void ShootProjectile(IWeapon weapon)
        {
            var dir = weapon.Target - weapon.ProjectileSpawnLocation;
            _characterWeaponProjectiles[weapon.OwnerId][weapon.Id].First(x => !x.IsFired).Shoot(weapon.ProjectileSpawnLocation, dir.normalized);
        }
    }

    public interface IProjectile
    {
        int ShotFromWeaponId { get; }
        int ShotByCharacterId { get; }
        float Speed { get; }
        float Lifetime { get; }
        Vector3? StartingPosition { get; }
        Vector3? DirectionNorm { get; }
        bool IsFired { get; }
        void Shoot(Vector3 fromPosition, Vector3 directionNorm);
        void ResetToStack();
    }

    public class BasicProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _lifeTime;

        public int ShotFromWeaponId { get; }

        public int ShotByCharacterId { get; }

        public float Speed => _speed;

        public float Lifetime => _lifeTime;

        public Vector3? StartingPosition { get; private set; }

        public Vector3? DirectionNorm { get; private set; }

        public bool IsFired { get; private set; }

        void Start()
        {
            ResetToStack();
        }

        public void Shoot(Vector3 fromPosition, Vector3 directionNorm)
        {
            if (IsFired)
            {
                Debug.LogError("Shooting fired projectile expand stack if neeeded");
                return;
            }

            gameObject.transform.SetPositionAndRotation(fromPosition, Quaternion.LookRotation(directionNorm));

            gameObject.SetActive(true);

            Invoke(nameof(ResetToStack), _lifeTime);

            IsFired = true;
        }

        public void ResetToStack()
        {
            IsFired = false;

            transform.localPosition = Vector3.zero;

            gameObject.SetActive(false);

            StartingPosition = null;

            DirectionNorm = null;
        }

        void LateUpdate()
        {
            if (IsFired)
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * Speed);
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetInstanceID() == ShotByCharacterId) return;

            var trigger = other.gameObject.GetComponent<IProjectileTrigger>();

            trigger?.OnProjectileTriggered(this);
        }
    }
}