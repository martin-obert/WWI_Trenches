using System;
using Assets.Gameplay.Character.Implementation.Player.Orders;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{

    [Obsolete, CreateAssetMenu(fileName = "PlayerBehavior", menuName = "Gameplay/Behaviors/Mappers/Player Behavior mapper", order = 1)]
    public class PlayerOrderMapper : OrderMapper<PlayerController>
    {
        private PlayerOrder _runningOrder;
        private PlayerOrder _idleOrder;
        private PlayerOrder _crawlingOrder;
        private PlayerOrder _attackOrder;
        public PlayerOrderMapper()
        {
            //Todo: prenest do editoru
            _idleOrder = new PlayerIdleOrder("Idle");
            _runningOrder = new PlayerRunOrder("Run");
            _crawlingOrder = new PlayerCrawlOrder("Crawl");
            _attackOrder = new PlayerAimOrder("Attack");
        }

        public override ICharacterOrder<PlayerController> GetBehaviorFromState(PlayerController character)
        {
            throw new ArgumentOutOfRangeException();
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