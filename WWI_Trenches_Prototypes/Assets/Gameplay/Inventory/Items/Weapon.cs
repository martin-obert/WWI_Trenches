using System;
using Assets.Gameplay.Character.Interfaces;
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
            Handles.Label(zone.transform.position + zone.transform.forward * zone.Range, $"Check tag: {zone.Name}", new GUIStyle { normal = new GUIStyleState { textColor = Color.red } });
        }
    }
#endif

}