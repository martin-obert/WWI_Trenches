namespace Assets.Gameplay
{
    public interface ICharacterBrain<in TCharacter>
    {
        void ChangeBehavior(TCharacter character);
    }

    public enum PlayerState
    {
        Idle,
        Running,
        Crawling,
        Covering,
        
    }
}
