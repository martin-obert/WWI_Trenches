using UnityEngine;

namespace Assets.Gameplay
{
    public class BasicProjectile : MonoBehaviour
    {
        public float Speed = 1f;

        public float Lifetime = 10f;


        void Start()
        {
            Destroy(gameObject, Lifetime);
        }

        void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * Speed);
        }

    }
}