using Assets.Gameplay.Attributes;

namespace Assets.Gameplay.Character
{
    public interface INavigatorAttributes
    {
        ObservableAttribute<float> Speed { get; }
    }
}