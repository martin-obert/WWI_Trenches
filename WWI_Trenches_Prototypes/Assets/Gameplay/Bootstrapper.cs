using Assets.Gameplay.Character.Implementation;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEngine;

namespace Assets.Gameplay
{

    public interface IBootstrapper
    {
        BasicCharacter PlayerPrefab { get; }
        TerrainTile[] TerrainTilesPrefabs { get; }
    }

    public class Bootstrapper : MonoBehaviour, IBootstrapper
    {
        [SerializeField] private BasicCharacter _playerPrefabPlayer;
        [SerializeField] private  TerrainTile[] _terrainTilesPrefabs;

        public BasicCharacter PlayerPrefab => _playerPrefabPlayer;

        public TerrainTile[] TerrainTilesPrefabs => _terrainTilesPrefabs;

        void Start()
        {
            InjectService.Instance.Register(this);
        }


    }
}