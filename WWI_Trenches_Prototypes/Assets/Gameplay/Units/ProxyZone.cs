using System;
using System.Collections;
using Assets.IoC;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class ProxyZone : MonoBehaviour
    {
        public class ProxyZoneEvent : EventArgs
        {
            public Player ZonedPlayer { get; set; }
            public bool IsPlayerInZone { get; set; }
        }

        public event EventHandler PlayerInZone;
        public event EventHandler PlayerOutZone;

        public Color HandleColor = Color.red;
        public float RangeRadius = 1f;

        public Player ZonedPlayer { get; private set; }

        public bool IsPlayerInZone => ZonedPlayer;
        public float ScanInterval = 1;
        void Start()
        {
            InvokeRepeating("ScanForPlayer", 1, ScanInterval);
        }

        void OnDestroy()
        {
            CancelInvoke("ScanForPlayer");
        }

        private void ScanForPlayer()
        {
            var player = InjectService.Instance.GetInstance<Player>();
            if (!player) return;
            if (Mathf.Abs((player.transform.position - transform.position).magnitude) < RangeRadius)
            {
                if (!ZonedPlayer)
                {
                    ZonedPlayer = player;
                    Debug.Log("Player in zone " + this.gameObject);
                    OnPlayerInZone();
                }
            }
            else
            {
                if (ZonedPlayer)
                    OnPlayerOutZone();
                ZonedPlayer = null;
            }

        }

        protected virtual void OnPlayerInZone()
        {
            PlayerInZone?.Invoke(this, new ProxyZoneEvent
            {
                ZonedPlayer = ZonedPlayer,
                IsPlayerInZone = IsPlayerInZone
            });
        }

        protected virtual void OnPlayerOutZone()
        {
            PlayerOutZone?.Invoke(this, EventArgs.Empty);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ProxyZone))]
    public class ProxyZoneEditor : Editor
    {
        public override void OnInspectorGUI()
        {


            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            var zone = target as ProxyZone;
            if (zone == null)
                return;
            Handles.color = zone.HandleColor;

            Handles.DrawWireDisc(zone.transform.position, Vector3.up, zone.RangeRadius);
        }
    }

#endif
}
