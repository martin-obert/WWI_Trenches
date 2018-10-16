using Assets.Gameplay.Character.Implementation.Player;
using UnityEngine;

namespace Assets.Gameplay
{
    public class BasicProjectile : MonoBehaviour
    {
        public float Speed = 1f;

        public float Lifetime = 10f;

        public string ShotByTag;

        void Start()
        {
            Destroy(gameObject, Lifetime);
        }

        void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * Speed);
        }


        void OnTriggerEnter(Collider collision)
        {
            print("Hit");
            if (collision.gameObject.CompareTag(TagsHelper.PlayerTag))
            {
                Destroy(gameObject);
                return;
            }

            Destroy(collision.gameObject);
        }
    }
}