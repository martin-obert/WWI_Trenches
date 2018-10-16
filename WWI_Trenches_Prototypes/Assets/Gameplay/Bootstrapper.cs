using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Units;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEngine;

namespace Assets.Gameplay
{

    public interface IBootstrapper
    {
        PlayerController PlayerPrefab { get; }
        TerrainTile[] TerrainTilesPrefabs { get; }
    }

    public class Bootstrapper : MonoBehaviour, IBootstrapper
    {
        [SerializeField] private PlayerController _playerPrefabPlayer;
        [SerializeField] private  TerrainTile[] _terrainTilesPrefabs;

        public PlayerController PlayerPrefab => _playerPrefabPlayer;

        public TerrainTile[] TerrainTilesPrefabs => _terrainTilesPrefabs;

        void Start()
        {
            InjectService.Instance.Register(this);
        }


    }
}