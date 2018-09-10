using System;
using UnityEngine;

namespace Assets.TileGenerator
{
    [Serializable]
    public class TiledTerrain : MonoBehaviour
    {
        public Transform EndPoint { get; set; }

        public TerrainTile[] TerrainTiles { get; set; }

        public int SizeX { get; set; }

        public int SizeY { get; set; }
        
    }
}