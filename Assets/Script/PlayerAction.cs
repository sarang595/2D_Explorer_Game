using UnityEngine;


public class PlayerAction : MonoBehaviour
{ 
    Rigidbody2D rb;
    CapsuleCollider2D playerCollider;
    [SerializeField] private GameObject sword;
    BoxCollider2D swordcollider;
    float horizontalInput;
    bool isfacingRight = true;
    bool iscrouched = false;
    bool isAttacking = false;
    Vector2 currentcrouchcollidersize;
    Vector2 currentcrouchcollideroffset;
   
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = rb.GetComponent<CapsuleCollider2D>();
        swordcollider = sword.GetComponent<BoxCollider2D>();
        originalcrouchcollidersize();
        SwordAttackOff();
    }
    

    public void Idle()
    {

    }
   public void Run()
    {
        // Handles Player Movement Logic
        horizontalInput = PlayerInputHandler.Instance.Horizontal();
        float moveSpeed = PlayerController.Instance.PlayerSpeed;
        rb.linearVelocity = new Vector2 (horizontalInput * moveSpeed, rb.linearVelocity.y) ;
        Flip();
      
    }
  public void Jump()
    {
        // Handles Player Jump Logic
        bool jumpInput = PlayerInputHandler.Instance.Jump();
        float jumpVelocity = PlayerController.Instance.JumpVelocity;
        bool canJump = jumpInput && PlayerController.Instance.getLocomotionState() == PlayerController.PlayerLocomotionState.Grounded;
        if (canJump)
        {
           
            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }

    }
  public void Flip()
    {
        // Handles Player flip
        Vector3 scale = transform.localScale;
        bool flipRight = horizontalInput > 0 && !isfacingRight;
        bool flipLeft = horizontalInput < 0 && isfacingRight;
        if (flipRight)
        {
            scale.x = Mathf.Abs(scale.x); 
            transform.localScale = scale;
            isfacingRight = true ;
        }
        else if (flipLeft)
        {
            scale.x = -Mathf.Abs(scale.x); 
            transform.localScale = scale;
            isfacingRight = false ;
        }
    }
    private void originalcrouchcollidersize()
    {
        currentcrouchcollidersize = playerCollider.size;
        currentcrouchcollideroffset = playerCollider.offset;
    }
   

    public void Crouch()
    {
        bool crouchInput = PlayerInputHandler.Instance.Crouching();
        if (crouchInput && !iscrouched)
        {
            currentcrouchcollideroffset.y = playerCollider.offset.y / 2;
            currentcrouchcollidersize.y = playerCollider.size.y / 2;
            playerCollider.size = currentcrouchcollidersize;
            playerCollider.offset = currentcrouchcollideroffset;
            iscrouched =true;
           
        }
        else if (!crouchInput && iscrouched)
        {
            {
                currentcrouchcollideroffset.y = playerCollider.offset.y * 2;
                currentcrouchcollidersize.y = playerCollider.size.y *2;
                playerCollider.size = currentcrouchcollidersize;
                playerCollider.offset = currentcrouchcollideroffset;
                iscrouched = false;
            }
        }
       
    }
   
   
  public void SwordAttack()
    {
        bool attackInput = PlayerInputHandler.Instance.Attacking();
        if(attackInput && !isAttacking)
        {
            sword.gameObject.SetActive(true);
            isAttacking = true;
            Invoke("SwordAttackOff", 0.1f);
        }  
           
    }
    private void SwordAttackOff()
    {
        
            sword.gameObject.SetActive(false);
            isAttacking = false;
       
    }
}
