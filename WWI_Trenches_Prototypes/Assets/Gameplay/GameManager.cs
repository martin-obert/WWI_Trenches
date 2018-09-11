using Assets.Gameplay.Abstract;
using Assets.TileGenerator;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private int _terrainWidth = 3;

        [SerializeField] private int _terrainHeight = 10;


        [SerializeField] private TerrainTileBuilder _terrainTileBuilder;

        public TerrainTileBuilder TerrainTileBuilder => _terrainTileBuilder;

        [SerializeField] private IBootstrapper _bootstrapper;

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

        }

        public void RegisterTerrainBuilder(TerrainTileBuilder builder)
        {
            if (_terrainTileBuilder && builder != _terrainTileBuilder)
            {
                _terrainTileBuilder.TerrainProgress -= TerrainTileBuilderOnTerrainProgress;
                Destroy(_terrainTileBuilder);
            }

            _terrainTileBuilder = builder;

            _terrainTileBuilder.TerrainProgress += TerrainTileBuilderOnTerrainProgress;
        }

        private void TerrainTileBuilderOnTerrainProgress(TerrainTileBuilder sender, TerrainTileBuilder.TerrainBuilderEventArgs args)
        {
            if (args.Progress < 1)
                return;

            var player = Instantiate(_bootstrapper.PlayerPrefab);

            player.Spawn(args.BuildedTerrain.StartPoint.position, args.BuildedTerrain.EndPoint.position);

        }

        public void StartLevel()
        {
            if (_bootstrapper == null)
            {
                Debug.LogError("No bootstrapper detected.");
                return;
            }

            _terrainTileBuilder.TerrainTiles = _bootstrapper.TerrainTilesPrefabs;

            var currentTerrain = _terrainTileBuilder.CreateTiledTerrain(_terrainWidth, _terrainHeight);

            _terrainTileBuilder.GenerateTerrainTiles(currentTerrain);

            TerrainManager.Instance.CurrentTerrain = currentTerrain;
        }

        public void RegisterBootstrapper(IBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
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
            base.OnInspectorGUI();

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
        }
    }
#endif
}