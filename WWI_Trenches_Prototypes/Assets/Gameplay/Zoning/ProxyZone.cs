using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Zoning
{


    public class ProxyZone : MonoBehaviour
    {
        public class ProxyZoneEvent : EventArgs
        {
            public GameObject ZonedObject { get; set; }
        }

        public event EventHandler<ProxyZoneEvent> ObjectInZone;
        public event EventHandler<ProxyZoneEvent> ObjectOutZone;
        public Color HandleColor = Color.red;
        public float RangeRadius = 1f;
        public string CheckTag = "Player";

        private void Start()
        {
            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = Vector3.zero;
            sphereCollider.radius = RangeRadius;

            var riggid = gameObject.AddComponent<Rigidbody>();
            riggid.isKinematic = true;
            riggid.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(CheckTag))
            {
                OnObjectInZone(other.gameObject);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(CheckTag))
            {
                OnObjectOutZone(other.gameObject);
            }
        }


        protected virtual void OnObjectInZone(GameObject zonedObject)
        {
            ObjectInZone?.Invoke(this, new ProxyZoneEvent
            {
                ZonedObject = zonedObject
            });
        }

        protected virtual void OnObjectOutZone(GameObject zonedObject)
        {
            ObjectOutZone?.Invoke(this, new ProxyZoneEvent
            {
                ZonedObject = zonedObject
            });
        }

        public void SubscribeTriggers(EventHandler<ProxyZoneEvent> inzone, EventHandler<ProxyZoneEvent> outzone)
        {
            ObjectInZone += inzone;
            ObjectOutZone += outzone;
        }

        public void UnsubscribeTriggers(EventHandler<ProxyZoneEvent> inzone, EventHandler<ProxyZoneEvent> outzone)
        {
            ObjectInZone -= inzone;
            ObjectOutZone = outzone;
        }

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ProxyZone))]
    public class ProxyZoneEditor : Editor
    {
        void OnSceneGUI()
        {
            var zone = target as ProxyZone;
            DrawZone(zone);
        }

        public static void DrawZone(ProxyZone zone)
        {

            if (zone == null)
                return;
            Handles.color = zone.HandleColor;

            Handles.DrawWireDisc(zone.transform.position, Vector3.up, zone.RangeRadius);
            Handles.Label(zone.transform.position + zone.transform.forward * zone.RangeRadius, $"Check tag: {zone.CheckTag}", new GUIStyle { normal = new GUIStyleState { textColor = zone.HandleColor } });
        }
    }
#endif
}
