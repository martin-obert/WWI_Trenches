using Assets.TileGenerator;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Camera _camera;

        [SerializeField] private float _minDistance = 3;
        [SerializeField] private float _maxDistance = 10;

        [SerializeField] private float _distance = 3;

        [SerializeField] private int _terrainWidth = 3;

        [SerializeField] private int _terrainHeight = 10;

        [SerializeField] private TerrainTileBuilder _terrainTileBuilder;

        [SerializeField] private Player.Player _playerPrefab;

        private TiledTerrain _currentTerrain;

        private Player.Player _player;

        public TerrainTileBuilder TerrainTileBuilder => _terrainTileBuilder;

        void Start()
        {
            var instance = FindObjectOfType<GameManager>();

            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }

        void Update()
        {
            if (_player && _camera)
            {

                var updatedPOsition = _player.transform.position - _player.transform.forward * _distance + Vector3.up * _distance;

                _camera.transform.position = new Vector3(_camera.transform.position.x, updatedPOsition.y, updatedPOsition.z);
            }
        }

        public void RegisterTerrainBuilder(TerrainTileBuilder builder)
        {
            if (_terrainTileBuilder && builder != _terrainTileBuilder)
                Destroy(_terrainTileBuilder);

            _terrainTileBuilder = builder;

            _terrainTileBuilder.TerrainProgress += TerrainTileBuilderOnTerrainProgress;
        }

        private void TerrainTileBuilderOnTerrainProgress(TerrainTileBuilder sender, TerrainTileBuilder.TerrainBuilderEventArgs args)
        {
            if (args.Progress < 1)
                return;

            if (_playerPrefab)
            {
                _player = Instantiate(_playerPrefab);

                _player.transform.position = _currentTerrain.StartPoint.position;
            }

            if (_camera)
            {
                _camera.transform.position = _currentTerrain.StartPoint.position;
            }
        }

        public void StartLevel()
        {
            if (_currentTerrain)
                DestroyImmediate(_currentTerrain.gameObject);

            _currentTerrain = _terrainTileBuilder.CreateTiledTerrain(_terrainWidth, _terrainHeight);

            _terrainTileBuilder.GenerateTerrainTiles(_currentTerrain);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        private GameManager _gameManager;

        void OnEnable()
        {
            _gameManager = target as GameManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();

            var height = serializedObject.FindProperty("_terrainHeight");
            height.intValue = EditorGUILayout.IntField("Height", height.intValue);

            var width = serializedObject.FindProperty("_terrainWidth");
            width.intValue = EditorGUILayout.IntField("Width", width.intValue);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Simulate start"))
            {
                if (_gameManager.TerrainTileBuilder)
                {
                    _gameManager.StartLevel();
                }
                else
                {
                    Debug.LogError("No terrain builder was registered");
                }
            }
            GUILayout.EndHorizontal();

            var camera = serializedObject.FindProperty("_camera").objectReferenceValue as Camera;

            var player = serializedObject.FindProperty("_playerPrefab").objectReferenceValue as Player.Player;

            var distance = serializedObject.FindProperty("_distance").floatValue;

            if (camera && player)
            {
                camera.transform.position = player.transform.position - player.transform.forward * distance + Vector3.up * distance;
            }

            var minDistance = serializedObject.FindProperty("_minDistance").floatValue;

            var maxDistance = serializedObject.FindProperty("_maxDistance").floatValue;

            serializedObject.FindProperty("_distance").floatValue = EditorGUILayout.Slider("Camera distance", distance, minDistance, maxDistance);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
#endif
}