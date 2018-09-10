using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.TileGenerator
{
    public class TerrainTileBuilder : MonoBehaviour
    {
        /// <summary>
        /// Usable prefabs of terrain tiles
        /// </summary>
        public TerrainTile[] TerrainTiles = new TerrainTile[0];

        public GameObject Spawn;

        [SerializeField] private int _terrainWidth = 3;

        [SerializeField] private int _terrainHeight = 10;

        [SerializeField] private int _distanceUnit = 11;

        [SerializeField, Range(0, 1)] private float _overlap = 1f;

        public TiledTerrain CreateTiledTerrain(int sizeX, int sizeY)
        {
            if (!Spawn)
                Spawn = new GameObject("Tile_Terrain_Spawn");

            foreach (Transform go in Spawn.transform)
            {
                DestroyImmediate(go.gameObject);
            }

            var tiledTerrain = Spawn.GetComponent<TiledTerrain>() ?? Spawn.AddComponent<TiledTerrain>();

            var boxCollider = Spawn.GetComponent<BoxCollider>() ?? Spawn.AddComponent<BoxCollider>();

            var realSize = new Vector3(sizeX, 1, sizeY) * _distanceUnit;

            boxCollider.center = realSize / 2f;

            boxCollider.size = realSize;

            tiledTerrain.SizeX = sizeX;

            tiledTerrain.SizeY = sizeY;


            var navMesh = Spawn.GetComponent<NavMeshSurface>();

            if (navMesh)
            {
                navMesh.BuildNavMesh();
            }

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
        /// 
        /// </summary>
        /// <param name="terrain"></param>
        public void GenerateTerrainTiles(TiledTerrain terrain)
        {
            if (TerrainTiles == null || TerrainTiles.Length == 0)
            {
                Debug.LogWarning("No tiles has been setted up.");
                return;
            }

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
                }
            }

            terrain.TerrainTiles = tiles.ToArray();
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

            if (GUILayout.Button("Generate terrain"))
            {
                var terrainWidthProp = serializedObject.FindProperty("terrainWidth");
                var terrainHeightProp = serializedObject.FindProperty("terrainHeight");


                var terrainWidth = EditorGUILayout.IntField("Width", terrainWidthProp.intValue);
                var terrainHeight = EditorGUILayout.IntField("Height", terrainHeightProp.intValue);
                var terrain = _target.CreateTiledTerrain(terrainWidth, terrainHeight);
                _target.GenerateTerrainTiles(terrain);
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }

            base.OnInspectorGUI();
        }
    }
#endif
}