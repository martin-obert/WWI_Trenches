using System;
using UnityEngine;

namespace Assets.TileGenerator
{
    [Serializable]
    public class TiledTerrain : MonoBehaviour
    {
        public TerrainTile[] TerrainTiles { get; set; }

        public int SizeX { get; set; }

        public int SizeY { get; set; }

        
    }
}