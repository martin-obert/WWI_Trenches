using Assets.Gameplay.Abstract;
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

        public void PointerExitHandler(BaseEventData data)
        {
            GetComponent<Renderer>().material.color = _color;
        }
    }
}