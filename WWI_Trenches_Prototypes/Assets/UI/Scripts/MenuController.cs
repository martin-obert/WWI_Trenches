using UnityEngine;

namespace Assets.UI.Scripts
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private MenuType _menuType;



        public MenuType MenuType => _menuType;

        void Start()
        {
        }

        public void HideMenu()
        {
            gameObject.SetActive(false);
        }
        public void ShowMenu()
        {
            gameObject.SetActive(true);
        }
    }
}