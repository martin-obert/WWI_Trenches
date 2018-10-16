using System.Collections.Generic;
using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Attributes
{
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
}