using Assets.Gameplay;
using Assets.IoC;
using UnityEngine;

public class HUDButtons : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        InjectService.Instance.GetInstance<GameManager>(manage => _gameManager = manage);
    }

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
}
