using System;
using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface IIdentificable
    {
        int Id { get; }
    }

    public enum ThreatLevel
    {
        None,
        Low,
        Medium,
        High
    }

    public interface ITargetable : IIdentificable
    {
        ICoverable CurrentCover { get; }

        string DisplayName { get; }

        GameObject GameObject { get; }

        void GotHitMelee(IWeapon weapon);
        void GotHitRanged(IProjectile projectile);

        float Visibility { get; }
        CharacterAttributesContainer Attributes { get; }
    }
}