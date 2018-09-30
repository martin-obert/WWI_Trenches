using System;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Gameplay.Units.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public LookAtConstraint LookAtConstraint;

        public ProxyZone ProxyZone;

        public GameObject ProjectilePrefab;
        public Transform ProjectileSpawn;
        public Player CurrentTarget { get; private set; }
        public Animator Animator;
        public AnimationClip FireAnimationClip;
        public float FireSpeed = 0.833f;

        private void Awake()
        {
            LookAtConstraint = GetComponent<LookAtConstraint>();

            ProxyZone = GetComponentInChildren<ProxyZone>();

            if (ProxyZone)
            {
                ProxyZone.PlayerInZone += ProxyZonePlayerInZone;
                ProxyZone.PlayerOutZone += ProxyZonePlayerOutZone;
            }

            Animator = GetComponentInChildren<Animator>();


        }

        private void ProxyZonePlayerOutZone(object sender, EventArgs e)
        {
            CurrentTarget = null;
            LookAtConstraint.RemoveSource(0);
            LookAtConstraint.constraintActive = false;
            CancelInvoke(nameof(FireAnimation));
        }

        private void ProxyZonePlayerInZone(object sender, EventArgs e)
        {
            var zoneargs = e as ProxyZone.ProxyZoneEvent;

            if (zoneargs != null)
            {
                CurrentTarget = zoneargs.ZonedPlayer;

                LookAtConstraint.AddSource(new ConstraintSource
                {
                    sourceTransform = CurrentTarget.transform,
                    weight = 1
                });

                LookAtConstraint.constraintActive = true;

                InvokeRepeating(nameof(FireAnimation), 1, FireSpeed);
            }
        }

        public void FireAnimation()
        {
            Animator.Play(FireAnimationClip.name, -1, 0);
            Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);
        }


        private void OnDestroy()
        {
            if (ProxyZone)
            {
                ProxyZone.PlayerInZone -= ProxyZonePlayerInZone;
                ProxyZone.PlayerOutZone -= ProxyZonePlayerOutZone;
            }
            CancelInvoke(nameof(FireAnimation));

        }
    }
}