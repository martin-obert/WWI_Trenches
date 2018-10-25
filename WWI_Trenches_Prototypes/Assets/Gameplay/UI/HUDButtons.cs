using Assets.Gameplay;
using Assets.Gameplay.Abstract;
using Assets.IoC;

public class HUDButtons : MonoBehaviorDependencyResolver
{
    private GameManager _gameManager;

    public void CrawlToggle()
    {
        _gameManager.CurrentPlayer.Crawl();
    }

    public void Run()
    {
        _gameManager.CurrentPlayer.Run();
    }

    public void Attack()
    {
        _gameManager.CurrentPlayer.Aim();
        _gameManager.CurrentPlayer.Shoot();
    }

    protected override void OnAwakeHandle()
    {
        Dependency<GameManager>(manager => _gameManager = manager);
        ResolveDependencies();
    }

    protected override void OnDestroyHandle()
    {
        
    }
}
