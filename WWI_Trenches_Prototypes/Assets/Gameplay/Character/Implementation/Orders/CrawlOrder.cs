using Assets.Gameplay.Character.Implementation.Player;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Orders
{
    [CreateAssetMenu(menuName = "Character/Basic/Crawl order", fileName = "Crawl order")]
    public class CrawlOrder : Order
    {
        [SerializeField, Tooltip("Crawl speed percentage from Max speed value"), Range(0, 100)]
        private float _crawlSpeed = 30f;

        public CrawlOrder(string name) : base(name)
        {
        }

        public override void Execute(CharacterOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, -1);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 0);

            arguments.Attributes.Speed.CurrentValue = _crawlSpeed * ((float)arguments.Attributes.Speed.MaxValue / 100);
        }
    }
}