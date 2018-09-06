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
        public TerrainTile[] TerrainTiles { get; set; } = new TerrainTile[0];

        public GameObject Spawn;

        [SerializeField] private int _distanceUnit = 11;
        [SerializeField, Range(0,1)] private float _overlap = 1f;

        public TiledTerrain CreateTiledTerrain(int sizeX, int sizeY)
        {
            if(!Spawn)
                Spawn = new GameObject("Tile_Terrain_Spawn");

            var tiledTerrain = Spawn.GetComponent<TiledTerrain>() ?? Spawn.AddComponent<TiledTerrain>();

            tiledTerrain.SizeX = sizeX;
            tiledTerrain.SizeY = sizeY;
            tiledTerrain.transform.Rotate(Quaternion.Euler(0,10,0).eulerAngles, Space.World);
            return tiledTerrain;
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
                    var tile = col == 0 ? TerrainTiles[Random.Range(0, TerrainTiles.Length)] : TerrainTiles.FirstOrDefault(x => x.SizeX <= terrain.SizeX - col);

                    if (!tile)
                        continue;

                    var tileName = $"row_{row.ToString(rowLen)}_col_{col.ToString(colLen)}_{tile.name}";

                    var tileInstance = Instantiate(tile, terrain.transform);

                    tileInstance.Metadata = new TerrainTileMetadata
                    {
                        PositionX = row,
                        PositionY = col
                    };

                    tileInstance.name = tileName;

                    tileInstance.transform.Translate(row * _distanceUnit - _overlap, 0, col * _distanceUnit - _overlap, Space.Self);

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
            }

            if (GUILayout.Button("Remove"))
            {
                if (_target.TerrainTiles.Length > 0)
                {
                    var temp = _target.TerrainTiles;

                    _target.TerrainTiles = new TerrainTile[_target.TerrainTiles.Length - 1];

                    Array.Copy(temp, _target.TerrainTiles, _target.TerrainTiles.Length);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (_target.TerrainTiles != null)
            {
                for (int i = 0; i < _target.TerrainTiles.Length; i++)
                {
                    _target.TerrainTiles[i] = EditorGUILayout.ObjectField(_target.TerrainTiles[i], typeof(TerrainTile), false) as TerrainTile;
                }
            }
           
            if (GUILayout.Button("Generate terrain"))
            {
                var terrain = _target.CreateTiledTerrain(3, 10);
                _target.GenerateTerrainTiles(terrain);
            }

            base.OnInspectorGUI();
        }
    }
#endif
}