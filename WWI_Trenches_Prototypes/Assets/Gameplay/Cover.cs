using Assets.Gameplay.Character.Implementation.Player;
using Assets.IoC;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Gameplay.Units
{
    public class Cover : MonoBehaviour
    {
        public Transform JumpDestination;



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
            var player = InjectService.Instance.GetInstance<PlayerController>();
            if (player)
            {
                //Todo: cover characte
               // player.TakeCover(this);
            }

        }

        public void PointerExitHandler(BaseEventData data)
        {
            GetComponent<Renderer>().material.color = _color;
        }
    }
}