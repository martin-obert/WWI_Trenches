using System.Collections.Generic;
using Assets.Gameplay.Abstract;
using UnityEngine;

namespace Assets.Gameplay.Tutorials
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        private readonly IDictionary<string, TutorialPart> _tutorialParts = new Dictionary<string, TutorialPart>();

        private TutorialPlayer _tutorialPlayer;

        public void AddPart(TutorialPart part)
        {
            _tutorialParts.Add(part.Id, part);
        }

        protected override void OnAwakeHandle()
        {
            Dependency<TutorialPlayer>(player =>
            {
                _tutorialPlayer = player;
                _tutorialPlayer.PartFinished += TutorialPlayerOnPartFinished;
            });
            CreateSingleton(this);
        }

        private void TutorialPlayerOnPartFinished(object sender, TutorialPart tutorialPart)
        {
            if (!string.IsNullOrEmpty(tutorialPart.NextPartId))
            {
                Play(tutorialPart.NextPartId);
            }
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }

        public void Play(string partId)
        {
            if (_tutorialParts.ContainsKey(partId))
            {
                Debug.LogError("Tutorial part missing - " + partId + " in container.");
                return;
            }

            var nextPart = _tutorialParts[partId];

            _tutorialPlayer.CurrentPart = nextPart;
        }
    }
}
