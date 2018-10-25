using System;
using Object = UnityEngine.Object;

namespace Assets.Gameplay.Tutorials
{
    [Serializable]
    public class TutorialDialog : Object
    {
        public string Text { get; set; }

        public void Render(TutorialPlayer tutorialPlayer)
        {
            tutorialPlayer.DialogText.text = Text;
        }
    }
}