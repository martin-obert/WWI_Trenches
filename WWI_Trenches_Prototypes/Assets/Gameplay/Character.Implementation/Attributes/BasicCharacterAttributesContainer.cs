namespace Assets.Gameplay.Character.Implementation.Attributes
{
    public class BasicCharacterAttributesContainer
    {
        public ObservableAttribute<float> Speed { get; }
        public ObservableAttribute<float> Health { get; }

        public BasicCharacterAttributesContainer()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", 3, 0, 5);
            Health = new ObservableAttribute<float>("hp", "HP", 3, 0, 5);
        }
    }
}