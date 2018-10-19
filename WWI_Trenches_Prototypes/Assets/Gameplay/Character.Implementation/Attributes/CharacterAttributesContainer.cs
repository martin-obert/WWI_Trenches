using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Attributes
{
    [CreateAssetMenu(fileName = "Basic Attributes", menuName = "Character/Attributes")]
    public class CharacterAttributesContainer : ScriptableObject
    {
        public ObservableAttribute<float> Speed { get; }
        public ObservableAttribute<float> Health { get; }

        public CharacterAttributesContainer()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", 3, 0, 5);
            Health = new ObservableAttribute<float>("hp", "HP", 3, 0, 5);
        }
    }
}