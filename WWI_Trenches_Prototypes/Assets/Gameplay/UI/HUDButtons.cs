using Assets.Gameplay.Units;
using Assets.IoC;
using UnityEngine;

public class HUDButtons : MonoBehaviour {

    public void Jump()
    {
        InjectService.Instance.GetInstance<Player>().JumpOver();
    }
}
