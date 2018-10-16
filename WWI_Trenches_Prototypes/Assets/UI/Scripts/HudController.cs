using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Units;
using Assets.IoC;
using Assets.UI.Scripts;
using UnityEngine;

public class HudController : MenuController
{
    public CanvasGroup _basicActions;
    public CanvasGroup _coverActions;


    private PlayerController _player;

    private void Start()
    {
        _basicActions.alpha = 0;
        _coverActions.alpha = 0;

        var playerInstance = InjectService.Instance.GetInstance<PlayerController>(BindPlayer);
        if (playerInstance)
        {
            BindPlayer(playerInstance);
        }
    }

    private void BindPlayer(PlayerController player)
    {
        //if (!player) return;

        //_player = player;
        //_player.PlayerStateChanged.AddListener(StateChanged);
    }

    private void StateChanged(PlayerController value)
    {
        //_basicActions.alpha = value.ThreatLevel > 0 ? 1 : 0;
        //_coverActions.alpha = value.IsInCover ? 1 : 0;
    }
}
