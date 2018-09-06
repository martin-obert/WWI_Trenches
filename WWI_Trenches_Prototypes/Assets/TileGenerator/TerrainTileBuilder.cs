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
        public TerrainTile[] TerrainTiles { get; set; }

        

        public TiledTerrain CreateTiledTerrain(int sizeX, int sizeY)
        {
            var go = new GameObject("Tiled_Terrain");

            var tiledTerrain = go.AddComponent<TiledTerrain>();

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

                    var tileInstance = Instantiate(tile);

                    tileInstance.Metadata = new TerrainTileMetadata
                    {
                        PositionX = row,
                        PositionY = col
                    };

                    tileInstance.name = tileName;

                    tileInstance.transform.Translate(row, col, 0);

                    tiles.Add(tileInstance);

                    // Update col value to sync with current tile size
                    col = tile.SizeX;
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
                var temp = _target.TerrainTiles;

                _target.TerrainTiles = new TerrainTile[_target.TerrainTiles.Length - 1];

                Array.Copy(temp, _target.TerrainTiles, _target.TerrainTiles.Length);
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
                Debug.Log("Generating terrain");
            }


            base.OnInspectorGUI();
        }
    }
#endif
}