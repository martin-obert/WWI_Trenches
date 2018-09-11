using Assets.Gameplay.Abstract;
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
            CGSingleton(this);
        }

        void Update()
        {
            var player = Player.Instance;

            var terrain = TerrainManager.Instance?.CurrentTerrain;

            if (player && _camera && terrain)
            {
                var z = terrain.StartPoint.position.z;
                var y = player.transform.position.y + _distance;
                var x = player.transform.position.x - _distance;

                _target.Set(_camera.transform.position.x + x, _camera.transform.position.y + y, _camera.transform.position.z + z);

                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _target, Time.deltaTime * 2f);
            }
        }
    }
}