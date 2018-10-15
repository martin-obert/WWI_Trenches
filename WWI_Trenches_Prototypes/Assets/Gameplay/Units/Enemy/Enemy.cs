using System;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Gameplay.Units.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public LookAtConstraint LookAtConstraint;

        public ProxyZone ProxyZone;
        public BasicProjectile BasicProjectilePrefab;
        public Transform ProjectileSpawn;
        public Player CurrentTarget { get; private set; }
        public Animator Animator;
        public AnimationClip FireAnimationClip;
        public float FireSpeed = 0.833f;
        public bool IsLocked;
        private void Awake()
        {
            LookAtConstraint = GetComponent<LookAtConstraint>();

            ProxyZone = GetComponentInChildren<ProxyZone>();

            if (ProxyZone)
            {
                ProxyZone.ObjectInZone += ProxyZoneObjectInZone;
                ProxyZone.ObjectOutZone += ProxyZoneObjectOutZone;
            }

            Animator = GetComponentInChildren<Animator>();


        }

        private void ProxyZoneObjectOutZone(object sender, EventArgs e)
        {
            var zoneargs = e as ProxyZone.ProxyZoneEvent;
            if (zoneargs != null)
            {
                //var player = zoneargs.ZonedObject.GetComponent<Player>();
                //if (player)
                //{
                //    player.ThreatLevel -= 1;
                //    player.CurrentEnemy = null;
                //}
            }

            CurrentTarget = null;
            LookAtConstraint.RemoveSource(0);
            LookAtConstraint.constraintActive = false;
            CancelInvoke(nameof(Fire));
        }

        private void ProxyZoneObjectInZone(object sender, EventArgs e)
        {
            var zoneargs = e as ProxyZone.ProxyZoneEvent;

            if (zoneargs != null)
            {
                CurrentTarget = zoneargs.ZonedObject.GetComponent<Player>();
                if (!CurrentTarget)
                {
                    Debug.LogWarning("Player component missing on " + zoneargs.ZonedObject);
                }
                else
                {
                    //CurrentTarget.ThreatLevel += 1;
                    //CurrentTarget.CurrentEnemy = this;
                }
            }
        }

        private void Update()
        {
            if (CurrentTarget)
            {
                //if (CurrentTarget.IsRunning && !IsLocked)
                //{
                //    IsLocked = true;
                //    LookAtConstraint.AddSource(new ConstraintSource
                //    {
                //        sourceTransform = CurrentTarget.transform,
                //        weight = 1
                //    });

                //    LookAtConstraint.constraintActive = true;

                //    InvokeRepeating(nameof(Fire), 1, FireSpeed);
                //}
            }

        }

        public void Fire()
        {
            Animator.Play(FireAnimationClip.name, -1, 0);
            Instantiate(BasicProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);

        }


        private void OnDestroy()
        {
            if (ProxyZone)
            {
                ProxyZone.ObjectInZone -= ProxyZoneObjectInZone;
                ProxyZone.ObjectOutZone -= ProxyZoneObjectOutZone;
            }
            CancelInvoke(nameof(Fire));

        }
    }
}