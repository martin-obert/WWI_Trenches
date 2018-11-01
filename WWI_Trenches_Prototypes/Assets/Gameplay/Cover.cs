using Assets.IoC;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Gameplay
{
    //Todo: velikost, typ a kvalita krytí
    public interface ICoverable
    {

    }

    public class Cover : MonoBehaviour, ICoverable
    {
        private Color _color;

        void Start()
        {
            _color = GetComponent<Renderer>().material.color;
        }


        public void PointerEnterHandler(BaseEventData data)
        {
            GetComponent<Renderer>().material.color = Color.red;

        }

        public void SelectedHandler(BaseEventData data)
        {
            Injection.Instance.Get<PlayerController>(player =>
            {
                if (player)
                {
                    player.Character.MoveTo(transform.position);
                }
            });

        }

        public void PointerExitHandler(BaseEventData data)
        {
            GetComponent<Renderer>().material.color = _color;
        }
    }
}