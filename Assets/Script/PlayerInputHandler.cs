using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private static PlayerInputHandler instance;
    public static PlayerInputHandler Instance {  get { return instance; } }
    float horizontal;
    float vertical;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    public void ReadInput()
    {
        
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }
    public float Horizontal() => horizontal;
    public float Vertical() => vertical;
}
