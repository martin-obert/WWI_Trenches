using Assets.Gameplay.Character;
using Assets.Gameplay.Zoning;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public class MeleeWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected WeaponData _data;

        [SerializeField] private string _name;

        [SerializeField]
        private ProxyZone _weaponMeleeProxyZoneRange;

        public virtual bool IsRanged { get; } = false;

        public bool IsStackable => false;

        public int Id => GetInstanceID();

        public IIdentificable Owner { get; protected set; }

        public WeaponData Data => _data;

        public string Name => _name;

        public int MaxStack => _data.MaxStack;

        public int Amount => 1;

        public bool CanFire { get; private set; } = true;

        public virtual bool IsInMeleeRange { get; private set; } = false;

        public ITargetable Target { get; protected set; }

        public float MeleeRange => _data.MeleeRange;

        public float AttackSpeed => _data.AttackSpeed;

        void Awake()
        {
            _weaponMeleeProxyZoneRange.RangeRadius = _data.MeleeRange;
            BindZone();
        }

        void OnDestroy()
        {
            UnBindZone();
        }

        private void Outzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            if (proxyZoneEvent.Targetable.Value.Id == Target.Id)
            {
                IsInMeleeRange = true;
            }
        }

        private void Inzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            if (proxyZoneEvent.Targetable.Value.Id == Target.Id)
            {
                IsInMeleeRange = false;
            }
        }

        protected virtual void BindZone()
        {
            _weaponMeleeProxyZoneRange.SubscribeTriggers(Inzone, Outzone);

        }

        protected virtual void UnBindZone()
        {
            _weaponMeleeProxyZoneRange.UnsubscribeTriggers(Inzone, Outzone);
        }


        public void MeleeAttack(ITargetable target, IIdentificable shooter)
        {
            if (!CanFire) return;

            StartCooldown();
        }

        public void ApplyMeleeDamage()
        {
            Target.GotHitMelee(this);
        }

        protected void StartCooldown()
        {
            CanFire = false;

            Invoke(nameof(MeleeCooldown), AttackSpeed);
        }

        private void MeleeCooldown()
        {
            CanFire = true;
        }
    }
}