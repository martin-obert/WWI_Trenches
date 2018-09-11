﻿using Assets.Gameplay.Abstract;
using Assets.Gameplay.Units;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private float _minDistance = 3;

        [SerializeField] private float _maxDistance = 10;

        [SerializeField] private float _distance = 3;

        private Vector3 _target = Vector3.zero;

        void Start()
        {
            CreateSingleton(this);

            if (!_camera)
            {
                _camera = Camera.main ?? Camera.current;
            }

            if (!_camera)
            {
                Debug.LogError("No camera is detected or assigned!");
            }
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        void Update()
        {
            var player = Player.Instance;

            var terrain = TerrainManager.Instance?.CurrentTerrain;

            if (player && _camera && terrain)
            {
                UpdateCameraPosition(player.transform.position, terrain.StartPoint.position);
            }
        }

        public void UpdateCameraPosition(Vector3 playerPosition, Vector3 startPoint)
        {
            var z = startPoint.z;
            var y = playerPosition.y + _distance;
            var x = playerPosition.x - _distance;

            _target.Set(_camera.transform.position.x + x, _camera.transform.position.y + y, _camera.transform.position.z + z);

            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _target, Time.deltaTime * 2f);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraManager))]
    public class CameraManagerEditor : Editor
    {
        private const string _distanceProp = "_distance";
        private const string _mindistanceProp = "_minDistance";
        private const string _maxdistanceProp = "_maxDistance";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("Adjust distance with object located in 0, 0, 0", MessageType.Info);

            var distanceProp = serializedObject.FindProperty(_distanceProp);
            var minDistanceProp = serializedObject.FindProperty(_mindistanceProp);
            var maxDistanceProp = serializedObject.FindProperty(_maxdistanceProp);

            var cachedVal = distanceProp.floatValue;

            EditorGUILayout.Slider(distanceProp, minDistanceProp.floatValue, maxDistanceProp.floatValue, "Camera distance");

            if (cachedVal != distanceProp.floatValue)
            {
                Camera.main.transform.position = Vector3.forward * -1 * cachedVal;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}