using System;
using System.Collections.Generic;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Inventory.Items;
using Assets.Gameplay.Projectiles;
using UnityEngine;

namespace Assets.Gameplay
{
    public class ProjectilesManager : Singleton<ProjectilesManager>
    {
        private ProjectileLogic _projectileLogic = new ProjectileLogic();

        protected override void OnAwakeHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }

        public void RegisterWeapon(int characterId,IWeapon weapon)
        {
        }

        public void UnregisterWeapon(int characterId, IWeapon weapon)
        {
        }

        public void ShootProjectile(RangedWeapon weapon)
        {
            if (weapon.Target != null)
            {
                var hitbox = weapon.Target.GameObject.GetComponent<CapsuleCollider>();

                var dir = weapon.Target.GameObject.transform.position + hitbox.center - weapon.ProjectileSpawnLocation;
                _projectileLogic.RayCastShot(weapon, weapon.ProjectileSpawnLocation, dir, weapon.Data.Damage);
                //_characterWeaponProjectiles[weapon.Owner.Id][weapon.Id].FirstOrDefault(x => !x.IsFired)?.RayCastShot(weapon.Owner, weapon.ProjectileSpawnLocation, dir.normalized);
            }
        }
    }
}