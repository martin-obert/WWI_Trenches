using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Tutorials
{
    public class TutorialPartCondition : MonoBehaviour
    {
        [SerializeField] private string _onFullfillTutorialPartId;
        [SerializeField] private string _onFailTutorialPartId;


        public void InvokeFullfilled()
        {
            InjectService.Instance.GetInstance<TutorialManager>(manager => manager.Play(_onFullfillTutorialPartId));
        }

        public void InvokeFailed()
        {
            InjectService.Instance.GetInstance<TutorialManager>(manager => manager.Play(_onFailTutorialPartId));
        }
    }

    public static class TutorialPartConditionHelper
    {
        public static void TutorialConditionSuccessfull(this GameObject gameObject)
        {
            var condition = gameObject.GetComponent<TutorialPartCondition>();
            if (condition)
            {
                condition.InvokeFullfilled();
            }
            else
            {
                Debug.LogError("Cannot find component " + nameof(TutorialPartCondition) + " on GameObject " + gameObject + " and therefore cannot invoke tutorial condition ");
            }
        }
        public static void TutorialConditionFailed(this GameObject gameObject)
        {
            var condition = gameObject.GetComponent<TutorialPartCondition>();
            if (condition)
            {
                condition.InvokeFailed();
            }
            else
            {
                Debug.LogError("Cannot find component " + nameof(TutorialPartCondition) + " on GameObject " + gameObject + " and therefore cannot invoke tutorial condition ");
            }
        }
    }
}