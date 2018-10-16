using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterNavigator<T>
    {
        void Teleport(Vector3 position);
        void Move(Vector3 position);
        void Stop();
        void Disable();
        void Enable();
    }

    public class BasicAttributeValueChanged<T> : UnityEvent<T>
    {
        
    }

    public class AttributesContainer
    {
        private readonly List<ICharacterAttribute> _attributes;

        public AttributesContainer(List<ICharacterAttribute> attributes)
        {
            _attributes = attributes;
            if (_attributes == null)
            {
                _attributes = new List<ICharacterAttribute>();
            }
        }

        public IReadOnlyCollection<ICharacterAttribute> Attributes => _attributes;
    }

    public class BasicCharacterAttributesContainer
    {
        public ObservableAttribute<float> Speed { get; }

        public BasicCharacterAttributesContainer()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", 3, 0, 5);
        }
    }
}