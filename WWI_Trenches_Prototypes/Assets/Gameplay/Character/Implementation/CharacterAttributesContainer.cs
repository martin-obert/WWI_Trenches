using Assets.Gameplay.Attributes;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    [CreateAssetMenu(fileName = "Basic Attributes", menuName = "Character/Basic/Attributes")]
    public class CharacterAttributesContainer : NavigatorAttributes
    {
        [SerializeField, Tooltip("Min, Current, Max")]
        private Vector3 _speed;

        [SerializeField, Tooltip("Min, Current, Max")]
        private Vector3 _health;

        public ObservableAttribute<float> Health { get; protected set; }

        void OnEnable()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", _speed.x, _speed.y, _speed.z);

            Health = new ObservableAttribute<float>("hp", "HP", _health.x, _health.y, _health.z);
        }
    }
}