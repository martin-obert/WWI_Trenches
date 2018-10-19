using System;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Instructions;
using Assets.Gameplay.Inventory;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Character.Merge
{
    public class CharacterMemory : ICharacterMemory<Character>
    {
        public event EventHandler<CharacterMemoryEventArgs> StateChanged;

        private readonly CharacterMemoryEventArgs _args = new CharacterMemoryEventArgs();

        public void ChangeStance(CharacterStance stance)
        {
            if (stance != CurrentStance) return;

            _args.LastStance = _args.CurrentStance;

            _args.CurrentStance = stance;

            StateChanged?.Invoke(this, _args);
        }

        public CharacterStance LastStance => _args.LastStance;
        public CharacterStance CurrentStance => _args.CurrentStance;
    }

    [CreateAssetMenu(fileName = "Basic brain", menuName = "Character/Basic/Brain")]
    public class CharacterBrain : ICharacterBrain<Character>
    {

        private ISequence _currentSequence;
        private ISequence _startingSequence;
        private int _safeLoopBreakCounter = 100;


        public ISequence CurrentSequence
        {
            get { return _currentSequence; }
            set
            {
                if (_startingSequence == null)
                    _startingSequence = value;

                _currentSequence?.Chain(value);
                _currentSequence = value;
            }
        }

        public void Execute<T>(IOrderArguments<T> arguments, ISequence sequence = null)
        {

            _safeLoopBreakCounter = 100;

            while (_safeLoopBreakCounter >= 0)
            {
                _safeLoopBreakCounter--;

                var current = sequence ?? _startingSequence;

                if (sequence == _startingSequence || sequence == null) return;

                var orders = current.Orders;
                if (orders != null && orders.Length > 0)
                {
                    foreach (var order in orders)
                    {
                        var playerOrder = order;

                        if (playerOrder != null)
                        {
                            playerOrder.Execute(arguments);
                        }
                        else
                        {
                            Debug.LogError("Cannot find type of " + order.Name);
                        }
                    }
                }

                if (current.Next != null)
                {
                    sequence = current.Next;
                    continue;
                }

                break;
            }
        }
        public ICharacterMemory<Character> Memory { get; } = new CharacterMemory();


    }

    [CreateAssetMenu(fileName = "Basic behavior", menuName = "Character/Basic/Behavior")]
    public class CharacterBehavior : ScriptableObject, ICharacterBehavior<Character>
    {
        public ISequenceExecutor PrepareToAttack(ISequenceExecutor sequenceExecutor)
        {
            throw new NotImplementedException();
        }

        public ISequenceExecutor CourseChanged(ISequenceExecutor sequenceExecutor, ICharacterMemory<Character> memory)
        {
            throw new NotImplementedException();
        }

        public ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor)
        {
            throw new NotImplementedException();
        }
    }

    
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterNavigator : MonoBehaviour, ICharacterNavigator<Character>
    {
        [SerializeField]
        private  NavMeshAgent _navMeshAgent;

        [SerializeField, Tooltip("Root object of character")]
        private Transform _transform;

        [SerializeField]
        private CharacterAttributesContainer _attributes;

        void Awake()
        {
            _attributes.Speed.Subscribe(SpeedChanged);
        }

        void OnDestroy()
        {
            _attributes.Speed.Unsubscribe(SpeedChanged);
        }

        private void SpeedChanged(object sender, float f)
        {
            if (_navMeshAgent)
            {
                _navMeshAgent.speed = f;
            }
        }

        public void LookOn(Transform target)
        {
            _transform.LookAt(target.position);
        }

        public void LookAtDirection(Quaternion roatation)
        {
            _transform.rotation = roatation;
        }

        public void Teleport(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Enable();
            }

            _navMeshAgent.Warp(position);
        }

        public void Move(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Enable();
            }

            _navMeshAgent.SetDestination(position);
        }

        public void Stop()
        {
            //Todo: asi resit jinak, neco jako speed 0 nebo tak pokud to bude hazet errory + nastudovat na gitu jak to vlastne funguje :)
            Disable();

        }

        public void Disable()
        {
            _navMeshAgent.enabled = false;
        }

        public void Enable()
        {

            _navMeshAgent.enabled = true;
        }

        private bool CheckNavMesh()
        {
            return _navMeshAgent;
        }

        public Vector3? Destination => _navMeshAgent.destination;
    }

    public class CharacterOrderArguments : IOrderArguments<Character>
    {
        public ICharacterNavigator<Character> Navigator { get; }

        public ITargetable CurrentTarget { get; }

        public Animator Animator { get; }

        public CharacterAttributesContainer Attributes { get; }

        public CharacterInventory Inventory { get; }

        public Vector3? Destination { get; }

        public CharacterOrderArguments(Character character)
        {
            Navigator = character.Navigator;
            Animator = character.Animator;
            Attributes = character.Attributes;
            Destination = character.Navigator.Destination;
            Inventory = character.Inventory;
            CurrentTarget = character.CurrentTarget;
        }
    }



    [RequireComponent(typeof(CharacterBrain), typeof(CharacterBehavior), typeof(CharacterInventory))]
    public class Character : MonoBehaviour, ICharacterProxy<Character>
    {
        private readonly CharacterBrain _brain = new CharacterBrain();

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private CharacterBehavior _behavior;

        [SerializeField]
        private CharacterNavigator _navigator;

        [SerializeField]
        private CharacterInventory _inventory;

        [SerializeField]
        private Animator _animator;

        [SerializeField] private CharacterAttributesContainer _attributesContainer;

        public CharacterAttributesContainer Attributes => _attributesContainer;

        public int Id => GetInstanceID();

        public string DisplayName => _displayName;

        public GameObject GameObject => gameObject;

        #region Tohle je defakto jen implementace IOrderArguments?
        public ICharacterBrain<Character> Brain => _brain;

        public ICharacterBehavior<Character> Behavior => _behavior;
        
        public ICharacterNavigator<Character> Navigator => _navigator;

        public Animator Animator => _animator;

        public IOrderArguments<Character> OrderArguments => new CharacterOrderArguments(this);

        public CharacterInventory Inventory => _inventory;

        public ITargetable CurrentTarget { get; set; }

        #endregion

        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        public event EventHandler VisibilityChanged;


        public void Attack()
        {
            Behavior.PrepareToAttack(Brain).Execute(OrderArguments);
            
        }

        public void ChangeCourse()
        {
            Behavior.CourseChanged(Brain, Brain.Memory).Execute(OrderArguments);
        }

        public void Stop()
        {

            Behavior.CourseChanged(Brain, Brain.Memory).Execute(OrderArguments);
        }

        public void Shoot()
        {
            throw new System.NotImplementedException();
        }

        public void GotKilledBy(ITargetable killer)
        {
            throw new NotImplementedException();
        }

        public bool IsVisibleTo(ITargetable targetable)
        {
            throw new NotImplementedException();
        }

        public void OnProjectileTriggered(IProjectile projectile)
        {
            throw new NotImplementedException();
        }

        
    }
}
