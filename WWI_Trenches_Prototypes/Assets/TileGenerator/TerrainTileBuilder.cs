using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Gameplay.Abstract;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.TileGenerator
{
    public class TerrainTileBuilder : Singleton<TerrainTileBuilder>
    {
        [Tooltip("Usable prefabs of terrain tiles")]
        public TerrainTile[] TerrainTiles = new TerrainTile[0];

        [Tooltip("Use to build navmesh")]
        public LayerMask LayerMask;

        [Tooltip("Width of terrain in \"ttu\"")]
        [SerializeField] private int _terrainWidth = 3;

        [Tooltip("Height of terrain in \"ttu\"")]
        [SerializeField] private int _terrainHeight = 10;


        [Tooltip("Length of single \"ttu\"")]
        [SerializeField] private int _distanceUnit = 11;

        [Tooltip("Overlap in %")]
        [SerializeField, Range(0, 1)] private float _overlap = 1f;

        #region Events
        public delegate void TerrainBuilderEventHandler(TerrainTileBuilder sender, TerrainBuilderEventArgs args);

        public class TerrainBuilderEventArgs
        {
            public TiledTerrain BuildedTerrain { get; set; }
            public float Progress { get; set; }
            public string Message { get; set; }
        }

        public event TerrainBuilderEventHandler TerrainProgress;
        #endregion

        public TiledTerrain CreateTiledTerrain(int sizeX, int sizeY)
        {

            var terrain = new GameObject("Tile_Terrain_Spawn");

            var currentTerrain = FindObjectOfType(typeof(TiledTerrain)) as TiledTerrain;
            if (currentTerrain)
            {
                foreach (Transform go in currentTerrain.transform)
                {
                    DestroyImmediate(go.gameObject);
                }
                DestroyImmediate(currentTerrain.gameObject);
            }



            var tiledTerrain = terrain.GetComponent<TiledTerrain>();
            if (!tiledTerrain)
                tiledTerrain = terrain.AddComponent<TiledTerrain>();

            var boxCollider = terrain.GetComponent<BoxCollider>();
            if (!boxCollider)
                boxCollider = terrain.AddComponent<BoxCollider>();

            var realSize = new Vector3(sizeX, -1, sizeY) * _distanceUnit;

            //Setup collider for NavMesh Surface
            boxCollider.center = realSize / 2f;

            boxCollider.size = realSize;

            tiledTerrain.SizeX = sizeX;

            tiledTerrain.SizeY = sizeY;

            //Setup NavMesh Surface
            var navMesh = terrain.GetComponent<NavMeshSurface>();

            if (!navMesh)
                terrain.AddComponent<NavMeshSurface>();

            navMesh.layerMask = LayerMask;

            navMesh.BuildNavMesh();

            //Setup starting point
            var startPoint = new GameObject("Start_Point");

            startPoint.transform.position = new Vector3(tiledTerrain.SizeX * _distanceUnit / 2f, terrain.transform.position.y, tiledTerrain.transform.position.z+1);

            startPoint.transform.SetParent(tiledTerrain.transform, true);

            tiledTerrain.StartPoint = startPoint.transform;

            //Setup ending point
            var endPoint = new GameObject("End_Point");

            endPoint.transform.position = new Vector3(tiledTerrain.SizeX * _distanceUnit / 2f, terrain.transform.position.y, tiledTerrain.SizeY * _distanceUnit);

            endPoint.transform.SetParent(tiledTerrain.transform, true);

            tiledTerrain.EndPoint = endPoint.transform;

            return tiledTerrain;
        }

        private TerrainTile RandomTile(int width, int height)
        {
            var tiles = TerrainTiles.Where(x => x.SizeX <= width && x.SizeY <= height).ToArray();
            return tiles[Random.Range(0, tiles.Length)];
        }

        private TerrainTile ChooseTile(IEnumerable<TerrainTile> currentTiles, int row, int col, int currentTerrainWidth, int currentTerrainHeight)
        {
            var colliding = currentTiles.Where(x => x.CornerY > row).ToArray();

            if (!colliding.Any())
                return RandomTile(currentTerrainWidth - col, currentTerrainHeight - row);

            if (colliding.Any(x => col >= x.Metadata.PositionX && x.CornerX > col))
                return null;

            var sufficientTiles = colliding.Where(x => x.Metadata.PositionX > col).ToArray();

            if (!sufficientTiles.Any())
                return RandomTile(currentTerrainWidth - col, currentTerrainHeight - row);

            return RandomTile(sufficientTiles.Min(x => x.Metadata.PositionX) - col, currentTerrainHeight - row);
        }

        /// <summary>
        /// Generates randomly terrain tiles according to bounds set in inputed terrain
        /// </summary>
        /// <param name="terrain"></param>
        public void GenerateTerrainTiles(TiledTerrain terrain)
        {
            if (TerrainTiles == null || TerrainTiles.Length == 0)
            {
                Debug.LogWarning("No tiles has been setted up.");
                return;
            }

            OnTerrainProgress(new TerrainBuilderEventArgs
            {
                BuildedTerrain = terrain,
                Progress = 0
            });

            var tiles = new List<TerrainTile>();

            var rowLen = new string('0', terrain.SizeY.ToString().Length);

            var colLen = new string('0', terrain.SizeX.ToString().Length);


            for (int row = 0; row < terrain.SizeY; row++)
            {
                for (int col = 0; col < terrain.SizeX; col++)
                {
                    var tile = ChooseTile(tiles, row, col, terrain.SizeX, terrain.SizeY);

                    if (!tile)
                        continue;

                    var tileName = $"row_{row.ToString(rowLen)}_col_{col.ToString(colLen)}_{tile.name}";

                    var tileInstance = Instantiate(tile, terrain.transform);

                    tileInstance.Metadata = new TerrainTileMetadata
                    {
                        PositionX = col,
                        PositionY = row
                    };

                    tileInstance.name = tileName;

                    tileInstance.transform.position = new Vector3(col * _distanceUnit - _overlap, 0, row * _distanceUnit - _overlap);

                    tiles.Add(tileInstance);

                    // Update col value to sync with current tile size
                    col += tile.SizeX - 1;

                    OnTerrainProgress(new TerrainBuilderEventArgs
                    {
                        BuildedTerrain = terrain,
                        Progress = (float)(row * col + col) / (terrain.SizeX * terrain.SizeY)
                    });
                }
            }

            terrain.TerrainTiles = tiles.ToArray();

            OnTerrainProgress(new TerrainBuilderEventArgs
            {
                BuildedTerrain = terrain,
                Progress = 1
            });
        }

        protected virtual void OnTerrainProgress(TerrainBuilderEventArgs args)
        {
            TerrainProgress?.Invoke(this, args);
        }

        protected override void OnAwakeHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TerrainTileBuilder))]
    public class TerrainTileBuilderEditor : Editor
    {
        private TerrainTileBuilder _target;
        void OnEnable()
        {
            _target = target as TerrainTileBuilder;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use bootstrapper for generating tiles dynamically, these tiles should be used only for prototyping/testing purpouses!", MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Terrain tiles");

            if (GUILayout.Button("Add"))
            {
                var temp = _target.TerrainTiles;

                _target.TerrainTiles = new TerrainTile[_target.TerrainTiles.Length + 1];

                temp.CopyTo(_target.TerrainTiles, 0);
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }

            if (GUILayout.Button("Remove"))
            {
                if (_target.TerrainTiles.Length > 0)
                {
                    var temp = _target.TerrainTiles;

                    _target.TerrainTiles = new TerrainTile[_target.TerrainTiles.Length - 1];

                    Array.Copy(temp, _target.TerrainTiles, _target.TerrainTiles.Length);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                }
            }

            EditorGUILayout.EndHorizontal();

          

            base.OnInspectorGUI();
        }
    }
#endif
}