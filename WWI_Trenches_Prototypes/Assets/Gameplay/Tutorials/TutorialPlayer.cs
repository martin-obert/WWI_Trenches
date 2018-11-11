using System;
using Assets.Gameplay.Abstract;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gameplay.Tutorials
{
    public class TutorialPlayer : Singleton<TutorialPlayer>
    {
        [SerializeField] private CanvasGroup _uiGroup;

        [SerializeField]
        private TextMeshProUGUI _dialogText;

        [SerializeField] private Button _nextButton;

        [SerializeField] private Button _finishButton;

        [SerializeField] private Button _repeatButton;

        //Todo: skip functions
        [SerializeField] private Button _skipButton;

        public CanvasGroup DialogGroup => _uiGroup;

        public TextMeshProUGUI DialogText => _dialogText;

        public Button NextButton => _nextButton;

        public Button FinishButton => _finishButton;

        public Button RepeatButton => _repeatButton;

        

        private TutorialPart _currentPart;

        public event EventHandler<TutorialPart> PartFinished;

        public TutorialPart CurrentPart
        {
            get { return _currentPart; }
            set
            {
                if (_currentPart)
                {
                    _currentPart.IndexChanged -= CurrentPartOnIndexChanged;
                    _currentPart.ResetSelf();
                }

                _currentPart = value;

                if (_currentPart)
                {
                    _currentPart.IndexChanged += CurrentPartOnIndexChanged;
                }

                RefreshUi();
            }
        }

        private void CurrentPartOnIndexChanged(object sender, EventArgs eventArgs)
        {
            RefreshUi();
        }

        private void RefreshUi()
        {
            if (!CurrentPart) return;

            CurrentPart.Apply(this);

        }

        public void Show()
        {
            _uiGroup.alpha = 1;
        }

        public void Hide()
        {
            _uiGroup.alpha = 0;
        }

        protected override void OnEnableHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }

        #region ButtonFunctions

        public void OnNextClick()
        {
            CurrentPart.Continue();
        }

        public void OnFinishClick()
        {
            OnPartFinished(CurrentPart);
        }

        public void OnRepeatClick()
        {
            CurrentPart.Repeat();
        }

        #endregion

        protected virtual void OnPartFinished(TutorialPart e)
        {
            PartFinished?.Invoke(this, e);
        }
    }
}