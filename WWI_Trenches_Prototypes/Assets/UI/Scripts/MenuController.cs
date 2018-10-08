using UnityEngine;

namespace Assets.UI.Scripts
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private MenuType _menuType;

        private Animator _animator;

        private  int _slideInHandle = Animator.StringToHash("SlideIn");
        private  int _slideOutHandle = Animator.StringToHash("SlideOut");

        public MenuType MenuType => _menuType;

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void HideMenu()
        {
            _animator.SetBool(_slideInHandle, false);
            _animator.SetBool(_slideOutHandle, true);
        }
        public void ShowMenu()
        {
            _animator.SetBool(_slideInHandle, true);
            _animator.SetBool(_slideOutHandle, false);
        }
    }
}