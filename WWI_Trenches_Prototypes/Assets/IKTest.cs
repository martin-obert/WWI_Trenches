using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Factories;
using Assets.IoC;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class IKTest : MonoBehaviour
{
    public int Count = 20;

    private CharacterFactory _characterFactory;

    private BasicCharacter[] _squad;

    void Start()
    {
        InjectService.Instance.GetInstance<CharacterFactory>(factory =>
        {
            _characterFactory = factory;

        });
        _squad = new BasicCharacter[Count];

        for (int i = 0; i < _squad.Length; i++)
        {
            _squad[i] = _characterFactory.CreatePlayerInstance();
        }
    }

    void Update()
    {
        if (_squad != null && _squad.Length > 0 && Input.GetMouseButtonDown((int)MouseButton.RightMouse))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                print(hit.point);

                for (var i = 0; i < _squad.Length; i++)
                {
                    var basicCharacter = _squad[i];
                    if (basicCharacter)
                        basicCharacter.MoveTo(hit.point + new Vector3(i, 0, 0));

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Time.timeScale = Time.timeScale == 0 ? 1 : 0;

            foreach (var basicCharacter in _squad)
            {
                if (!basicCharacter)
                    return;
                basicCharacter.Crouch();
                basicCharacter.Stop();
                basicCharacter.Shoot();
            }
        }
    }
}
