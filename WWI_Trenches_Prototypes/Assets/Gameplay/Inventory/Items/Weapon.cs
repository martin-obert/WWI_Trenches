using System;
using Assets.Gameplay.Zoning;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MeleeWeapon))]
    public class WeaponEditor : Editor
    {
        void OnSceneGUI()
        {
            ProxyZoneEditor.DrawZone(serializedObject.FindProperty("_weaponMeleeProxyZoneRange").objectReferenceValue as ProxyZone);
        }

        public static void DrawZone(MeleeWeapon zone)
        {

            if (zone == null)
                return;
            Handles.color = Color.red;

            Handles.DrawWireDisc(zone.transform.position, Vector3.up, zone.MeleeRange);
            Handles.Label(zone.transform.position + zone.transform.forward * zone.MeleeRange, String.Empty, new GUIStyle { normal = new GUIStyleState { textColor = Color.red } });
        }
    }

    [CustomEditor(typeof(RangedWeapon))]
    public class RangedWeaponEditor : Editor
    {
        void OnSceneGUI()
        {
            ProxyZoneEditor.DrawZone(serializedObject.FindProperty("_weaponMeleeProxyZoneRange").objectReferenceValue as ProxyZone);
            ProxyZoneEditor.DrawZone(serializedObject.FindProperty("_rangedProxyZone").objectReferenceValue as ProxyZone);
        }
    }
#endif


}