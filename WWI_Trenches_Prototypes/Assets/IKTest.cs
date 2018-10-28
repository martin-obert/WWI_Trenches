using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Factories;
using Assets.IoC;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class IKTest : MonoBehaviour
{
    public int Count = 20;
    public GameObject prefab;


    void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            Instantiate(prefab, new Vector3(Random.Range(0, 100), 0, 0), Quaternion.identity);
        }
    }

    void Update()
    {

    }
}
