using UnityEngine;

namespace Assets.TileGenerator
{
    public class TerrainTile : MonoBehaviour
    {
        [SerializeField] private int _sizeX;

        [SerializeField] private int _sizeY;

        public int SizeX => _sizeX;

        public int SizeY => _sizeY;

        public TerrainTileMetadata Metadata { get; set; }

        public int CornerX => Metadata.PositionX + SizeX;

        public int CornerY => Metadata.PositionY + SizeY;
    }
}