using Assets.Gameplay.Attributes;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{

    public abstract class NavigatorAttributes : ScriptableObject, INavigatorAttributes
    {
        public ObservableAttribute<float> Speed { get; protected set; }
    }
}