using System;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Units;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private int _terrainWidth = 3;

        [SerializeField] private int _terrainHeight = 10;

        [SerializeField] private IBootstrapper _bootstrapper;

        [SerializeField] private TerrainTileBuilder _terrainTileBuilder;

        [SerializeField] private bool _autoStart = false;

        void Awake()
        {
            Dependency<Bootstrapper>(bootstrapper => { _bootstrapper = bootstrapper; });
            Dependency<TerrainTileBuilder>(RegisterTerrainBuilder);
        }

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        private void RegisterTerrainBuilder(TerrainTileBuilder builder)
        {
            if (_terrainTileBuilder && builder != _terrainTileBuilder)
            {
                _terrainTileBuilder.TerrainProgress -= TerrainTileBuilderOnTerrainProgress;
                Destroy(_terrainTileBuilder);
            }

            _terrainTileBuilder = builder;

            _terrainTileBuilder.TerrainProgress += TerrainTileBuilderOnTerrainProgress;

            if (_autoStart)
                StartLevel();
        }

        private void TerrainTileBuilderOnTerrainProgress(TerrainTileBuilder sender, TerrainTileBuilder.TerrainBuilderEventArgs args)
        {
            if (args.Progress < 1)
                return;




            var player = Instantiate(_bootstrapper.PlayerPrefab);

            player.Spawn(args.BuildedTerrain.StartPoint.position, PlayerHelpers.GetEndPoint(player, args.BuildedTerrain));

        }

        public void StartLevel()
        {
            if (_bootstrapper == null)
            {
                Debug.LogError("No bootstrapper detected.");
                return;
            }

            if (_bootstrapper != null && _bootstrapper.TerrainTilesPrefabs.Length > 0)
                _terrainTileBuilder.TerrainTiles = _bootstrapper.TerrainTilesPrefabs;

            var currentTerrain = _terrainTileBuilder.CreateTiledTerrain(_terrainWidth, _terrainHeight);

            _terrainTileBuilder.GenerateTerrainTiles(currentTerrain);

            TerrainManager.Instance.CurrentTerrain = currentTerrain;
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
                _gameManager.StartLevel();
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}