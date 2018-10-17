using System;
using Assets.Gameplay.Zoning;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public class Weapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private WeaponData _data;

        [SerializeField] private string _name;

        [SerializeField]
        private ProxyZone _weaponRangeProxyZone;

        public bool IsStackable => false;

        public int Id => GetInstanceID();

        public int OwnerId { get; private set; }

        public WeaponData Data => _data;

        public Vector3 ProjectileSpawnLocation => _projectileSpawn.position;

        public string Name => _name;

        public int MaxStack => 1;

        public int Amount => 1;

        public bool CanFire { get; } = true;

        public bool IsInRange { get; }

        public Vector3 Target { get; private set; }

        public float Range => _data.Range;

        public float AttackSpeed => _data.AttackSpeed;

        public bool IsFiring { get; private set; }

        [SerializeField]
        private Transform _projectileSpawn;

        void Awake()
        {
            _weaponRangeProxyZone.RangeRadius = _data.Range;
        }

        void OnDisable()
        {
            StopFiring();
        }

        void OnDestroy()
        {
            StopFiring();
        }


        public void StartFiring(Vector3 target, int shooterId)
        {
            if (IsFiring) return;

            Target = target;

            IsFiring = true;
            OwnerId = shooterId;

            InvokeRepeating(nameof(Fire), 0, AttackSpeed);
        }

        public void StopFiring()
        {
            if (!IsFiring) return;
            IsFiring = false;
            CancelInvoke(nameof(Fire));
        }

        private void Fire()
        {
            ProjectilesManager.Instance.ShootProjectile(this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : Editor
    {
        void OnSceneGUI()
        {
            var zone = target as Weapon;
            DrawZone(zone);
        }

        public static void DrawZone(Weapon zone)
        {

            if (zone == null)
                return;
            Handles.color = Color.red;

            Handles.DrawWireDisc(zone.transform.position, Vector3.up, zone.Range);
            Handles.Label(zone.transform.position + zone.transform.forward * zone.Range,String.Empty, new GUIStyle { normal = new GUIStyleState { textColor = Color.red } });
        }
    }
#endif

}