using Assets.TileGenerator;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    public static class PlayerHelpers
    {
        public static Vector3 GetEndPoint(Transform transform, TiledTerrain terrain)
        {
            return new Vector3(transform.position.x, terrain.EndPoint.position.y, terrain.EndPoint.position.z);
        }

        public static Vector3 GetMinWeaponDistanceToEnemy(Transform player, Transform enemy, float playerWeaponRange)
        {
            return (enemy.position - player.position).normalized * playerWeaponRange;
        }
    }
}