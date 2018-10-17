using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Units;
using Assets.IoC;
using UnityEngine;

public class HUDButtons : MonoBehaviour
{

    private PlayerController _player;

    private void Start()
    {
        _player = InjectService.Instance.GetInstance<PlayerController>(player => _player = player);
    }

    public void CrawlToggle()
    {
        _player.Crawl();

    }

    public void Run()
    {
        _player.Run();
    }

    public void Attack()
    {
        _player.AttackEnemy();
    }
}
