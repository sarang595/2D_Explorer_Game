using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
   Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Idle()
   {

    }
   public void Run()
    {
        // Handles Player Movement Logic
        float horizontalInput = PlayerInputHandler.Instance.Horizontal();
        float moveSpeed = PlayerController.Instance.PlayerSpeed;
        rb.linearVelocity = new Vector2 (horizontalInput, rb.linearVelocity.y) * moveSpeed ;
      
    }
  public void Jump()
    {
        

    }
  public void Flip()
    {

    }
  public void SwordAttack()
    {

    }
}
