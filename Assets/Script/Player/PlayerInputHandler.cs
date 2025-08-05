using UnityEngine;

public class PlayerInputHandler : PlayerService<PlayerInputHandler>
{
    
    float horizontal;
    bool jump;
    bool drifting;
    bool push;
    bool drift;
    bool attacking;
    int RightMouseClick = 1;

   
    public void ReadInput()
    {
        horizontal = Input.GetAxis("Horizontal"); //Handles Run
        jump = Input.GetKeyDown(KeyCode.Space); //Handles Jump
        drifting = Input.GetKey(KeyCode.LeftControl); //Handles Crouch
        attacking = Input.GetMouseButtonUp(RightMouseClick); //Right MouseClick Handles SwordAttack
        push = Input.GetKey(KeyCode.E);
     
    }
    public float Horizontal() => horizontal;
    public bool Jump() => jump;
    public bool Drifting() => drifting;
    public bool Attacking() => attacking;
    public bool Pushing() => push;
 
}
