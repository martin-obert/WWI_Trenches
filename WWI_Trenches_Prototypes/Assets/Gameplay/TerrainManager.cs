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

        public void DestroyCurrentTerrain()
        {
            CurrentTerrain = null;
        }

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

    }
}