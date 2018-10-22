using Assets.Gameplay.Character;
using Assets.Gameplay.Zoning;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public class RangedWeapon : MeleeWeapon
    {
        [SerializeField]
        private Transform _projectileSpawn;

        [SerializeField] private ProxyZone _rangedProxyZone;

        public override bool IsRanged { get; } = true;

        public bool IsInFireRange { get; private set; }

        public override bool IsInMeleeRange => IsInFireRange || base.IsInMeleeRange;

        public Vector3 ProjectileSpawnLocation => _projectileSpawn.position;

        public void RangedAttack(ITargetable target, IIdentificable shooter)
        {
            if (!CanFire)
            {
                return;
            }

            Target = target;

            Owner = shooter;

            ProjectilesManager.Instance.ShootProjectile(this);

            StartCooldown();
        }

        protected override void BindZone()
        {
            _rangedProxyZone.SubscribeTriggers(Inzone, Outzone);
            base.BindZone();
        }

        protected override void UnBindZone()
        {
            _rangedProxyZone.UnsubscribeTriggers(Inzone, Outzone);
            base.UnBindZone();
        }

        private void Outzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            IsInFireRange = false;
        }

        private void Inzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            IsInFireRange = true;
        }
    }
}