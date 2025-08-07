using UnityEngine;

public class GameService<T> : MonoBehaviour where T: GameService<T>
{

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    Debug.LogError($"No instance of {typeof(T).Name} found in the scene!");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
