using System;
using Assets.Gameplay.Units;

using UnityEngine;

namespace Assets.Gameplay
{

    [CreateAssetMenu(fileName = "PlayerBehavior", menuName = "Gameplay/Behaviors/Mappers/Player Behavior mapper", order = 1)]
    public class PlayerBehavMapper : BehaviorMapper<Player>
    {
        private PlayerBehavior _runningBehavior;
        private PlayerBehavior _idleBehavior;
        private PlayerBehavior _crawlingBehavior;

        public PlayerBehavMapper()
        {
            //Todo: prenest do editoru
            _idleBehavior = new PlayerIdleBehav("Idle");
            _runningBehavior = new PlayerRunBehavior("Run");
            _crawlingBehavior = new PlayerCrawlBehavior("Crawl");
        }

        public override ICharacterBehavior<Player> GetBehaviorFromState(Player character)
        {
            if (character.IsRunning)
            {
                return _runningBehavior;
            }

            if (character.IsIdle)
            {
                return _idleBehavior;
            }

            if (character.IsCrawling)
            {
                return _crawlingBehavior;
            }

            throw new NotSupportedException("This state is not supported");
        }
    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(PlayerBehavMapper))]
//    public class PlayerBehavMapperEditor: Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            var sobj = serializedObject;

//            if (GUILayout.Button("+"))
//            {
//                var array = (PlayerBehavior[]) sobj.FindProperty("_playerBehaviors").;
//                array = new PlayerBehavior[] { new PlayerIdleBehav() };
//            }

//            base.OnInspectorGUI();
//        }
//    }
//#endif
}