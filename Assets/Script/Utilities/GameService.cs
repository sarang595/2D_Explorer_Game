using UnityEngine;

public class GameService<T> : MonoBehaviour where T : GameService<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // First try to find existing instance
                instance = FindFirstObjectByType<T>();

                // If still null, create a new GameObject with the component
                if (instance == null)
                {
                    GameObject _instance = new GameObject(typeof(T).Name);
                    instance = _instance.AddComponent<T>();
                    DontDestroyOnLoad(_instance);
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
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
