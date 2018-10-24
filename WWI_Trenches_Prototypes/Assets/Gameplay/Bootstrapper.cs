using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Inventory.Items;
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

    public class Bootstrapper : Singleton<Bootstrapper>, IBootstrapper
    {
        [SerializeField] private BasicCharacter _playerPrefabPlayer;

        [SerializeField] private  TerrainTile[] _terrainTilesPrefabs;

        public BasicCharacter PlayerPrefab => _playerPrefabPlayer;

        public TerrainTile[] TerrainTilesPrefabs => _terrainTilesPrefabs;

        public RangedWeapon PlayerMainWeapon;

        void OnEnable()
        {
           CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }
    }
}