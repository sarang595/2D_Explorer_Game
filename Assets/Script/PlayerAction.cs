using UnityEngine;
using static PlayerController;


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
    [SerializeField] private PlayerAnimation playeranimation;
   

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        swordcollider = sword.GetComponent<BoxCollider2D>();
        originalcrouchcollidersize();
        SwordAttackOff();
    }

    private void Update()
    {
        playeranimation.JumpAnim();
       
    }

    public void Run()
    {
        bool CanRun = PlayerController.Instance.CanRun();
        horizontalInput = PlayerInputHandler.Instance.Horizontal();
        // Handles Player Movement Logic
        if (CanRun)
        {
           
            float moveSpeed = PlayerController.Instance.PlayerSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            if (playeranimation != null)
                playeranimation.RunAnim();
        }
        Flip();
    }
  public void Jump()
    {
        // Handles Player Jump Logic
        if(isAttacking) return;
       
        bool CanJump = PlayerController.Instance.CanJump();
        float jumpVelocity = PlayerController.Instance.JumpVelocity;
        bool canJump = CanJump && PlayerController.Instance.getLocomotionState() == PlayerController.PlayerLocomotionState.Grounded;
        //bool Canjump() => Isjumping();
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
        
        bool CanCrouch= PlayerController.Instance.CanCrouch();
        bool CrouchUp = PlayerController.Instance.CrouchUp();
        playeranimation.CrouchAnim();
        if (CanCrouch && !iscrouched)
        {

            currentcrouchcollideroffset.y = playerCollider.offset.y / 1.5f;
            currentcrouchcollidersize.y = playerCollider.size.y / 1.5f;
            playerCollider.size = currentcrouchcollidersize;
            playerCollider.offset = currentcrouchcollideroffset;
            iscrouched = true;

        }
        else if (iscrouched && CrouchUp)
        {
            {


                currentcrouchcollideroffset.y = playerCollider.offset.y * 1.5f;
                currentcrouchcollidersize.y = playerCollider.size.y * 1.5f;
                playerCollider.size = currentcrouchcollidersize;
                playerCollider.offset = currentcrouchcollideroffset;
                iscrouched = false;
                


            }
            // Update animation based on current crouch state
            if (playeranimation != null)
                playeranimation.CrouchAnim();
        }

    }


    public void SwordAttack()
    {

        bool canattack = PlayerController.Instance.CanAttack() && PlayerController.Instance.Isgrounded();
        if (canattack && !isAttacking)
        {
            playeranimation.AttackAnim();
            Invoke("SwordAttackOn", 0.2f);
            isAttacking = true;
            Invoke("SwordAttackOff", 0.5f);
        }

    }
    private void SwordAttackOn()
    {
        sword.gameObject.SetActive(true);
    }
    private void SwordAttackOff()
    {
       
        playeranimation.AttackAnim();
        sword.gameObject.SetActive(false);
        isAttacking = false;

    }
    public float GetVerticalVelocity() => rb.linearVelocity.y;
}
