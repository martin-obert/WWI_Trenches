﻿using System.Collections;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Factories;
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

        private TerrainManager _terrainManager;

        public BasicCharacter CurrentPlayer { get; private set; }

        private CharacterFactory _characterFactory;

        void Awake()
        {
            Dependency<Bootstrapper>(bootstrapper => { _bootstrapper = bootstrapper; });
            Dependency<TerrainTileBuilder>(RegisterTerrainBuilder);
            Dependency<TerrainManager>(terrainManager => _terrainManager = terrainManager);
            Dependency<CharacterFactory>(factory => _characterFactory = factory);
        }

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        protected override void DependenciesResolved()
        {
            if(_autoStart)
                StartLevel();

            base.DependenciesResolved();
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
        }

        private void TerrainTileBuilderOnTerrainProgress(TerrainTileBuilder sender, TerrainTileBuilder.TerrainBuilderEventArgs args)
        {
            if (args.Progress < 1)
                return;

            StartCoroutine(DelayedStart(args));
        }

        private IEnumerator DelayedStart(TerrainTileBuilder.TerrainBuilderEventArgs args)
        {
            yield return new WaitForSecondsRealtime(1.5f);

            CurrentPlayer = _characterFactory.CreatePlayerInstance();

            args.BuildedTerrain.SpawnAtStart(CurrentPlayer.gameObject);

            CurrentPlayer.MoveTo(PlayerHelpers.GetEndPoint(CurrentPlayer.transform, args.BuildedTerrain));
            
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

            TerrainManager.Instance.CurrentTerrain = currentTerrain;

            _terrainTileBuilder.GenerateTerrainTiles(currentTerrain);
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