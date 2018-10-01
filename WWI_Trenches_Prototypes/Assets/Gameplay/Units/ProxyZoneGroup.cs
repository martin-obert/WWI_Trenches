using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class ProxyZoneGroup : MonoBehaviour
    {
        public ProxyZone[] ProxyZones;
        [Tooltip("In seconds")]
        public float ScanInterval = 1;
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