namespace Assets.Gameplay
{
    public interface IProjectileTrigger
    {
        void OnProjectileTriggered(IProjectile projectile);
        int Id { get; }
    }
}