using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.TileGenerator
{
    [Serializable, RequireComponent(typeof(NavMeshSurface))]
    public class TiledTerrain : MonoBehaviour
    {
        public Transform EndPoint { get; set; }

        public Transform StartPoint { get; set; }

        public TerrainTile[] TerrainTiles { get; set; }

        public int SizeX { get; set; }

        public int SizeY { get; set; }

        private NavMeshSurface _navMeshSurface;

        void Start()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
            if (_navMeshSurface)
            {
                _navMeshSurface.BuildNavMesh();
            }
        }
    }
}