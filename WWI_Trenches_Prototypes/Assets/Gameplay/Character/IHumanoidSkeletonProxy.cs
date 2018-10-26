namespace Assets.Gameplay.Character
{
    public interface IHumanoidSkeletonProxy
    {
        IHumanoidSkeletonHandProxy LeftHandProxy { get; }
        IHumanoidSkeletonHandProxy RightHandProxy { get; }
    }
}