using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Zoning
{
    public class ProxyZoneGroup : MonoBehaviour
    {
        public ProxyZone[] ProxyZones;

        public void SubscribeZones(EventHandler<ProxyZone.ProxyZoneEvent> inZoneEventHandler, EventHandler<ProxyZone.ProxyZoneEvent> outZoneEventHandler)
        {
            foreach (var proxyZone in ProxyZones)
            {
                proxyZone.SubscribeTriggers(inZoneEventHandler, outZoneEventHandler);
            }
        }

        public void UnsubscribeZones(EventHandler<ProxyZone.ProxyZoneEvent> inZoneEventHandler, EventHandler<ProxyZone.ProxyZoneEvent> outZoneEventHandler)
        {
            foreach (var proxyZone in ProxyZones)
            {
                proxyZone.UnsubscribeTriggers(inZoneEventHandler, outZoneEventHandler);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ProxyZoneGroup))]
    public class ProxyZoneGroupEditor : Editor
    {
        void OnSceneGUI()
        {
            var group = target as ProxyZoneGroup;
            if (group.ProxyZones != null)
                foreach (var groupProxyZone in group.ProxyZones)
                {
                    ProxyZoneEditor.DrawZone(groupProxyZone);
                }
        }
    }
#endif
}