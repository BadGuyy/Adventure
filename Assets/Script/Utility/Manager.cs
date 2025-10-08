using UnityEngine;

public class Manager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("/UI"));
    }
}
