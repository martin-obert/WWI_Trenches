using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
        [SerializeField] private int terrainWidth = 3;
        [SerializeField] private int terrainHeight = 10;
        [SerializeField] private int _distanceUnit = 11;
        [SerializeField, Range(0, 1)] private float _overlap = 1f;

        public TiledTerrain CreateTiledTerrain(int sizeX, int sizeY)
        {
            if (!Spawn)
                Spawn = new GameObject("Tile_Terrain_Spawn");

            var tiledTerrain = Spawn.GetComponent<TiledTerrain>() ?? Spawn.AddComponent<TiledTerrain>();

            tiledTerrain.SizeX = sizeX;
            tiledTerrain.SizeY = sizeY;
            tiledTerrain.transform.Rotate(Quaternion.Euler(0, 10, 0).eulerAngles, Space.World);
            return tiledTerrain;
        }

        private TerrainTile ChooseTile(IEnumerable<TerrainTile> currentTiles, int row, int col, int terrainWidth)
        {
            if (!currentTiles.Any())
                return TerrainTiles[Random.Range(0, TerrainTiles.Length)];

            var rect = new Rect(col, row, TerrainTiles.Max(x => x.SizeX), TerrainTiles.Max(x => x.SizeY));

            var iteration = 100;
            while (iteration > 0)
            {
                bool overlaps = false;

                foreach (var tiles in currentTiles.Where(x => x.Metadata.PositionY >= col - terrainWidth))
                {
                    var rect2 = new Rect(tiles.Metadata.PositionX, tiles.Metadata.PositionY, tiles.SizeX, tiles.SizeY);

                    if (rect2.Overlaps(rect))
                    {
                        overlaps = true;
                        break;
                    }
                }
                if (!overlaps)
                {
                    Debug.Log("Search for w" + rect.width + " - h" + rect.height);
                    var candidates = TerrainTiles.Where(x => x.SizeX <= rect.width && col + x.SizeX <= terrainWidth).ToArray();
                    if (candidates.Any())
                    {
                        return candidates[Random.Range(0, candidates.Length)];
                    }

                }

                if (rect.xMax > 0)
                {
                    rect.xMax--;
                }
                else if (rect.yMax > 0)
                {
                    rect.yMax--;
                }
                else
                {
                    return null;
                }



                iteration--;
            }

            return null;
        }

        /// <summary>
        /// Currently limited for tiles with size y = 1
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
                    var tile = ChooseTile(tiles, row, col, terrain.SizeX);

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

                    tileInstance.transform.localPosition = new Vector3(col * _distanceUnit - _overlap, 0, row * _distanceUnit - _overlap);

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

        private const string _distanceUnitProp = "_distanceUnit";


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