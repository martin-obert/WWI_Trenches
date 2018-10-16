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

        //if (_player)
        //{
        //    if (_player.IsRunning)
        //        _player.Crawl();
        //    else
        //    {
        //        _player.Run();
        //    }
        //}
    }

    public void LeaveCoverToEnd()
    {
        //_player.RunToEnd();
    }

    public void Attack()
    {
        _player.Attack();
    }
}
