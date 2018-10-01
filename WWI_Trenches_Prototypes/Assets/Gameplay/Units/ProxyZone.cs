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
            public GameObject ZonedObject { get; set; }
        }

        public event EventHandler ObjectInZone;
        public event EventHandler ObjectOutZone;
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
            PlayerOutZone?.Invoke(this, EventArgs.Empty);
            {
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ProxyZone))]
    public class ProxyZoneEditor : Editor
    {
        public override void OnInspectorGUI()
        {


            base.OnInspectorGUI();
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
