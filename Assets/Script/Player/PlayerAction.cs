using System.Collections;
using UnityEngine;



public class PlayerAction : MonoBehaviour
{ 
    Rigidbody2D rb;
    CapsuleCollider2D playerCollider;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject flysword;
    BoxCollider2D swordcollider;
    float horizontalInput;
    bool isfacingRight = true;
    bool iscrouched = false;
    bool isAttacking = false;
    Vector2 currentcrouchcollidersize;
    Vector2 currentcrouchcollideroffset;
    [SerializeField] private PlayerAnimation playeranimation;
    private Coroutine swordCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        swordcollider = sword.GetComponent<BoxCollider2D>();
        originalcrouchcollidersize();
        GroundedSwordAttackOff();
    }

    private void Update()
    {

        JumpAttack();



    }

    public void Run()
    {
        bool CanRun = PlayerController.Instance.CanRun() && !PlayerController.Instance.Crouching();
        horizontalInput = PlayerInputHandler.Instance.Horizontal();
        // Handles Player Movement Logic
        if (CanRun && Mathf.Abs(horizontalInput) > 0.01f)
        {
            float moveSpeed = PlayerController.Instance.PlayerSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

            if (playeranimation != null)
                playeranimation.RunAnim();
        }
        else
        {
            // Reset horizontal velocity only (not vertical)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        Flip();
    }
  public void Jump()
    {
        // Handles Player Jump Logic
        
       
        bool CanJump = PlayerController.Instance.CanJump();
        float jumpVelocity = PlayerController.Instance.JumpVelocity;
        bool canJump = CanJump && PlayerController.Instance.getLocomotionState() == PlayerController.PlayerLocomotionState.Grounded &&!isAttacking;
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

        bool canattack = PlayerController.Instance.CanAttack() && PlayerController.Instance.Isgrounded() && !isAttacking;
        if (canattack )
        {
            playeranimation.AttackAnim();
            Invoke("SwordAttackOn", 0.2f);
            isAttacking = true;
            Invoke("GroundedSwordAttackOff", 0.5f);
        }

    }
    private void SwordAttackOn()
    {
        sword.gameObject.SetActive(true);
    }
    private void GroundedSwordAttackOff()
    {
       
        playeranimation.AttackAnim();
        sword.gameObject.SetActive(false);
        isAttacking = false;

    }
    private void FlySwordAttackOff()
    {


        flysword.gameObject.SetActive(false);
       

    }
    private void FlySwordAttackOn()
    {


        flysword.gameObject.SetActive(true);


    }
  

    public void JumpAttack()
    {
        bool isJumpAttack = playeranimation.FlyAttack();

        if (isJumpAttack && swordCoroutine == null)
        {
            swordCoroutine = StartCoroutine(EnableSwordAfterDelay(0.48f));
        }
        else if (!isJumpAttack)
        {
            if (swordCoroutine != null)
            {
                StopCoroutine(swordCoroutine);
                swordCoroutine = null;
            }
            FlySwordAttackOff();
        }
    }

    private IEnumerator EnableSwordAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FlySwordAttackOn();
        swordCoroutine = null;
    }

    public float GetVerticalVelocity() => rb.linearVelocity.y;
}
