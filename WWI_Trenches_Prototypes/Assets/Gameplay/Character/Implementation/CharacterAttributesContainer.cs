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

        [SerializeField, Tooltip("Min, Current, Max")]
        private Vector3 _visibility;

        [SerializeField, Tooltip("Min, Current, Max")]
        private Vector3 _noiseLevel;

        public ObservableAttribute<float> Health { get; protected set; }

        public ObservableAttribute<float> Visibility { get; protected set; }

        public ObservableAttribute<float> NoiseLevel { get; protected set; }

        void OnEnable()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", _speed.x, _speed.y, _speed.z, (min, current, max) => Mathf.Clamp(current, min, max));

            Health = new ObservableAttribute<float>("hp", "HP", _health.x, _health.y, _health.z, (min, current, max) => Mathf.Clamp(current, min, max));

            Visibility = new ObservableAttribute<float>("visibility", "Visibility", _visibility.x, _visibility.y, _visibility.z, (min, current, max) => Mathf.Clamp(current, min, max));

            NoiseLevel = new ObservableAttribute<float>("noise", "noise", _noiseLevel.x, _noiseLevel.y, _noiseLevel.z, (min, current, max) => Mathf.Clamp(current, min, max));
        }
    }
}