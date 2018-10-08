using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Scripts
{
    public enum FadeDirection
    {
        FadeIn,
        FadeOut
    }

    public class Fading : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private FadeDirection _fadeDirection;

        private int _fadeInParamHandle = Animator.StringToHash("FadingIn");
        private int _fadeOutParamHandle = Animator.StringToHash("FadingOut");

        public bool IsFading { get; private set; }
        
        public event EventHandler FadingComplete;
        
        void Awake()
        {
            if (!_animator)
                _animator = GetComponent<Animator>();

            if (!_image)
                _image = GetComponent<Image>();
            _image.raycastTarget = true;

            switch (_fadeDirection)
            {
                case FadeDirection.FadeIn:
                    FadeIn();
                    break;
                case FadeDirection.FadeOut:
                    FadeOut();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void FadeIn()
        {
            if (IsFading) return;

            _animator.SetBool(_fadeInParamHandle, true);
            StartCoroutine(FadingCoroutine());
        }

        public void FadeOut()
        {
            if (IsFading) return;

            _animator.SetBool(_fadeOutParamHandle, true);
            StartCoroutine(FadingCoroutine());
        }

        private IEnumerator FadingCoroutine()
        {
            if (IsFading)
            {
                Debug.LogError("Fading in is in progress!");
                yield return null;
            }

            IsFading = true;

            yield return new WaitForSecondsRealtime(_fadeDuration);

            _image.raycastTarget = false;

            
            OnFadingComplete();
        }

        protected virtual void OnFadingComplete()
        {
            FadingComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}