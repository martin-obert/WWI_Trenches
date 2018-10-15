using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Gameplay.Abstract;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Units
{
    public static class PlayerHelpers
    {
        public static Vector3 GetEndPoint(Player player, TiledTerrain terrain)
        {
            return new Vector3(player.transform.position.x, terrain.EndPoint.position.y, terrain.EndPoint.position.z);
        }

    }

    public enum ThreatLevel
    {
        None,
        EnemyIsNear,
    }

    public class CharacterObjective
    {
        public Vector3 Destination { get; set; }
    }

    [RequireComponent(typeof(PlayerBrain), typeof(NavMeshAgent))]
    public class Player : Singleton<Player>
    {
        private PlayerBrain _playerBrain;

        private PlayerCharacterState _playerCharacterState;

        //Todo promitnout attributy do navcontrolleru (speed a tak)
        public BasicCharacterAttributesContainer AttributesContainer { get; private set; }

        public Vector3 Destination { get; set; }

        public bool IsRunning => _playerCharacterState.CurrentState == PlayerState.Running;

        public bool IsIdle => _playerCharacterState.CurrentState == PlayerState.Idle;

        public PlayerCharacterNavigator Navigator { get; private set; }

        public bool IsCrawling => _playerCharacterState.CurrentState == PlayerState.Crawling;

        void OnEnable()
        {
            AttributesContainer = new BasicCharacterAttributesContainer();

            _playerBrain = GetComponent<PlayerBrain>();

            _playerCharacterState = new PlayerCharacterState();

            

            Navigator = new PlayerCharacterNavigator(GetComponent<NavMeshAgent>(), AttributesContainer);
        }

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Crawl();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Run();
            }
            
        }

        private void Crawl()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Crawling) return;

            print("Crawl");
            _playerCharacterState.ChangeState(PlayerState.Crawling, this);

            _playerBrain.ChangeBehavior(this);
        }

        public void Run()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Running) return;

            print("Run");
            _playerCharacterState.ChangeState(PlayerState.Running, this);

            _playerBrain.ChangeBehavior(this);
        }

        public void Stop()
        {

        }

        public void TakeCover()
        {

        }

        public void LeaveCover()
        {

        }

        public void Attack()
        {

        }

        public void Loot()
        {

        }

        public void Spawn(Vector3 startPointPosition)
        {
            Navigator.Teleport(startPointPosition);
        }
    }
}
