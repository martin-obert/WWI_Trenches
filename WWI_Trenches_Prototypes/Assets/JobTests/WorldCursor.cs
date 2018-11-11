using Assets.Gameplay.Abstract;
using UnityEngine;

namespace Assets.JobTests
{
    public class WorldCursor : Singleton<WorldCursor>
    {
        public Vector3? GetTerrainCursorPosition()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (!Physics.Raycast(ray, out rayHit)) return null;

            if (rayHit.collider != Terrain.activeTerrain.GetComponent<TerrainCollider>()) return null;

            return rayHit.point;
        }

     

        protected override void OnEnableHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }
    }
}