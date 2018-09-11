using Assets.Gameplay.Units;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEngine;

namespace Assets.Gameplay
{
    public interface IBootstrapper
    {
        Player PlayerPrefab { get; }
        TerrainTile[] TerrainTilesPrefabs { get; }
    }

    public class Bootstrapper : MonoBehaviour, IBootstrapper
    {
        [SerializeField] private Player _playerPrefabPlayer;
        [SerializeField] private  TerrainTile[] _terrainTilesPrefabs;

        public Player PlayerPrefab => _playerPrefabPlayer;

        public TerrainTile[] TerrainTilesPrefabs => _terrainTilesPrefabs;

        void Start()
        {
            InjectService.Instance.Register(this);
        }


    }
}