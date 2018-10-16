using System;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Zoning;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public class Weapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private WeaponData _data;

        [SerializeField] private string _name;

        private ProxyZone _weaponRangeProxyZone;

        public bool IsStackable => false;

        public string Name => _name;

        public int MaxStack => 1;

        public int Amount => 1;

        public bool IsInRange { get; private set; }

        public bool CanFire => IsInRange;


        public Vector3 Target { get; private set; }

        public float Range => _data.Range;

        public float AttackSpeed => _data.AttackSpeed;

        public bool IsFiring { get; private set; }


        void Awake()
        {
            _weaponRangeProxyZone.RangeRadius = _data.Range;
            //_weaponRangeProxyZone.SubscribeTriggers(Inzone, Outzone);
        }

        void OnDisable()
        {
            StopFiring();
        }

        void OnDestroy()
        {
            StopFiring();
        }

        //private void Outzone(object sender, ProxyZone.ProxyZoneEvent eventArgs)
        //{
        //    if (Target != null && eventArgs.ZonedObject == Target.GameObject)
        //    {
        //        IsInRange = false;
        //    }
        //}

        //private void Inzone(object sender, ProxyZone.ProxyZoneEvent eventArgs)
        //{
        //    if (Target != null && eventArgs.ZonedObject == Target.GameObject)
        //    {
        //        IsInRange = true;
        //    }
        //}

        public void StartFiring(Vector3 target)
        {
            if (IsFiring) return;

            Target = target;

            IsFiring = true;
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
            print("Boom!");
        }
    }
}