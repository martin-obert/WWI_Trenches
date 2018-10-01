using Assets.Gameplay.Units;
using Assets.IoC;
using UnityEngine;

public class HUDButtons : MonoBehaviour {


    public void CrawlToggle()
    {
        var player = InjectService.Instance.GetInstance<Player>();
        if (player)
        {
            if(player.IsRunning)
                player.Crawl();
            else
            {
                player.Run();
            }
        }
    }
}
