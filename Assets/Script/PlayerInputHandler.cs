using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private static PlayerInputHandler instance;
    public static PlayerInputHandler Instance {  get { return instance; } }
    float horizontal;
    bool jump;
    bool crouching;
    bool attacking;

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
        
        horizontal = Input.GetAxis("Horizontal");
        jump = Input.GetKeyDown(KeyCode.Space);
        crouching = Input.GetKey(KeyCode.LeftControl);
        attacking = Input.GetMouseButtonUp(1);
    }
    public float Horizontal() => horizontal;
    public bool Jump() => jump;
    public bool Crouching() => crouching;
    public bool Attacking() => attacking;   
}
