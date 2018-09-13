using Assets.Gameplay.Abstract;
using Assets.Gameplay.Units;
using Assets.IoC;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Gameplay
{
    public class InputManager : MonoBehaviour
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
            var player = InjectService.Instance.GetInstance<Player>();
            Debug.Log("Clicked");
            if (player)
            {
                player.Move(transform.position);
            }
        }

        public void PointerExitHandler(BaseEventData data)
        {
            GetComponent<Renderer>().material.color = _color;
        }
    }
}