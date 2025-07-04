using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private static PlayerInputHandler instance;
    public static PlayerInputHandler Instance {  get { return instance; } }
    float horizontal;
    bool jump;
    bool crouching;
    bool push;
    bool attacking;
    int RightMouseClick = 1;

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
        horizontal = Input.GetAxis("Horizontal"); //Handles Run
        jump = Input.GetKeyDown(KeyCode.Space); //Handles Jump
        crouching = Input.GetKey(KeyCode.LeftControl); //Handles Crouch
        attacking = Input.GetMouseButtonUp(RightMouseClick); //Right MouseClick Handles SwordAttack
        push = Input.GetKey(KeyCode.E);
    }
    public float Horizontal() => horizontal;
    public bool Jump() => jump;
    public bool Crouching() => crouching;
    public bool Attacking() => attacking;
    public bool Pushing() => push;
}
