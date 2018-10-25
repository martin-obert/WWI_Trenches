using Assets.Gameplay.Abstract;
using Assets.TileGenerator;

namespace Assets.Gameplay
{
    public class TerrainManager : Singleton<TerrainManager>
    {
        private TiledTerrain _currentTerrain;
        public TiledTerrain CurrentTerrain
        {
            get { return _currentTerrain;}
            set
            {
                if (_currentTerrain)
                {
                    Destroy(_currentTerrain.gameObject);
                }

                _currentTerrain = value; 
            }
        }

        protected override void OnAwakeHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);

        }
    }
}