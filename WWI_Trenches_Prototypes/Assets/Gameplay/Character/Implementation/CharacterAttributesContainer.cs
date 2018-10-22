using System;
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

        public BasicAttribute<float> Health { get; protected set; }

        public BasicAttribute<float> Visibility { get; protected set; }

        public BasicAttribute<float> NoiseLevel { get; protected set; }

        public ObservableAttribute<int> Threat { get; protected set; }

        void OnEnable()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", _speed.x, _speed.y, _speed.z, (min, current, max) => Mathf.Clamp(current, min, max));

            Health = new BasicAttribute<float>("hp", "HP", _health.x, _health.y, _health.z, (min, current, max) => Mathf.Clamp(current, min, max));

            Visibility = new BasicAttribute<float>("visibility", "Visibility", _visibility.x, _visibility.y, _visibility.z, (min, current, max) => Mathf.Clamp(current, min, max));

            NoiseLevel = new BasicAttribute<float>("noise", "Noise Level", _noiseLevel.x, _noiseLevel.y, _noiseLevel.z, (min, current, max) => Mathf.Clamp(current, min, max));

            Threat = new ObservableAttribute<int>("threat", "Threat Level", (int)ThreatLevel.None, (int)ThreatLevel.None, (int)ThreatLevel.High);
        }
    }
}