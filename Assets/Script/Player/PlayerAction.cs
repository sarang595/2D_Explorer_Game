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
    bool isdrift = false;
    bool isAttacking = false;
    Vector2 currentdriftcollidersize;
    Vector2 currentdriftcollideroffset;
    private PlayerAnimation playeranimation;
    private Coroutine swordCoroutine;
    bool Jumped =false;
    public float jumpAttackForwardForce = 10f;
    public float driftTime;
    bool CanDrift;
    float StaminaLessRate = 0.005f;
    float StaminaBurnRate = 2f;

    private void Start()
    {
        CanDrift = true;
        rb = GetComponent<Rigidbody2D>();
        driftTime = UIManager.Instance.Stamina;
        playeranimation = GetComponent<PlayerAnimation>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        swordcollider = sword.GetComponent<BoxCollider2D>();
        originaldriftcollidersize();
        GroundedSwordAttackOff();
    }

    private void Update()
    {
       
        JumpAttack();
        SwordAttack();
        DriftCheck();
        Jump();
        Dead();
      

    }
    private void FixedUpdate()
    {
       Run();
       StaminaControl();
       JumpMove();
    }

    //public void Run()
    //{
    //    bool CanRun = PlayerController.Instance.CanRun() && !PlayerController.Instance.Crouching();
    //    horizontalInput = PlayerInputHandler.Instance.Horizontal();
    //    // Handles Player Movement Logic
    //    if (CanRun && Mathf.Abs(horizontalInput) > 0.01f)
    //    {
    //        float moveSpeed = PlayerController.Instance.PlayerSpeed;
    //        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

    //        if (playeranimation != null)
    //            playeranimation.RunAnim();
    //    }
    //    else
    //    {
    //        // Reset horizontal velocity only (not vertical)
    //        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    //    }
      
    //    Flip();

    //}
    public void Run()
    {
        bool CanRun = PlayerController.Instance.CanRun() && !PlayerController.Instance.CanDrift();
        horizontalInput = PlayerInputHandler.Instance.Horizontal();

        // Handles Player Movement Logic
        if (CanRun && Mathf.Abs(horizontalInput) > 0.01f)
        {
            float moveSpeed = PlayerController.Instance.PlayerSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

            if (playeranimation != null)
                playeranimation.RunAnim();
        }
        else if (!isdrift) // Only reset velocity if not crouching
        {
            // Reset horizontal velocity only (not vertical)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        Flip();
    }

    public void Jump()
    {
        // Handles Player Jump Logic
        Flip();
     
        bool CanJump = PlayerController.Instance.CanJump();
        float jumpVelocity = PlayerController.Instance.JumpVelocity;
        bool canJump = CanJump && PlayerController.Instance.getLocomotionState() == PlayerController.PlayerLocomotionState.Grounded &&!isAttacking;
        //bool Canjump() => Isjumping();
       
        if (canJump && playeranimation.Ispushing() == false)
        {
           
            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            Jumped = true;
        }
       

    }
    public void JumpMove()
    {
        bool isInAir = PlayerController.Instance.PlayerinAir();
        float isMoved = Mathf.Abs(horizontalInput);

        if (isInAir && Jumped && isMoved > 0.001f)
        {
            float _JumpMoveSpeed = PlayerController.Instance.JumpMoveSpeed;
            Vector2 JumpMoveVelocity = isfacingRight ? Vector2.right * _JumpMoveSpeed : Vector2.left * _JumpMoveSpeed;
            rb.linearVelocity = new Vector2(JumpMoveVelocity.x, rb.linearVelocity.y);
        }
    }

    public void Flip()
    {
        bool CanFlip = PlayerController.Instance.CanFlip();
        if (CanFlip)
        {

            // Handles Player flip
            Vector3 scale = transform.localScale;
            bool flipRight = horizontalInput > 0 && !isfacingRight;
            bool flipLeft = horizontalInput < 0 && isfacingRight;
            if (flipRight)
            {
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
                isfacingRight = true;
            }
            else if (flipLeft)
            {
                scale.x = -Mathf.Abs(scale.x);
                transform.localScale = scale;
                isfacingRight = false;
            }
        }
        else return;
       
    }
    private void originaldriftcollidersize()
    {
        currentdriftcollidersize = playerCollider.size;
        currentdriftcollideroffset = playerCollider.offset;
    }

    void DriftCheck()
    {
        if (CanDrift) Drift();
        else return;
    }
    public void Drift()
    {
        float driftSpeed = PlayerController.Instance.DriftSpeed;
       // Vector2 velocity = isfacingRight ? new Vector2(driftSpeed, rb.linearVelocity.y) : new Vector2(-driftSpeed, rb.linearVelocity.y);
        bool CanDrift = PlayerController.Instance.CanDrift();
        bool DriftUp = PlayerController.Instance.DriftUp();
        playeranimation.DriftAnim();
        if (CanDrift && !isdrift)
        {
            currentdriftcollideroffset.y = playerCollider.offset.y / 1.5f;
            currentdriftcollidersize.y = playerCollider.size.y / 1.5f;
            playerCollider.size = currentdriftcollidersize;
            playerCollider.offset = currentdriftcollideroffset;
           // rb.linearVelocity = velocity;

            isdrift = true;

        }
        if (isdrift && CanDrift)
        {
            Vector2 velocity = isfacingRight ? new Vector2(driftSpeed, rb.linearVelocity.y) : new Vector2(-driftSpeed, rb.linearVelocity.y);
            rb.linearVelocity = velocity;
        }
        else if (isdrift && DriftUp)
        {
            {
                currentdriftcollideroffset.y = playerCollider.offset.y * 1.5f;
                currentdriftcollidersize.y = playerCollider.size.y * 1.5f;
                playerCollider.size = currentdriftcollidersize;
                playerCollider.offset = currentdriftcollideroffset;
                isdrift = false;
                

            }
            // Update animation based on current crouch state
            if (playeranimation != null)
                playeranimation.DriftAnim();
        }

    }
    private void ExitDrift()
    {
        // Reset collider to original size
        currentdriftcollideroffset.y = playerCollider.offset.y * 1.5f;
        currentdriftcollidersize.y = playerCollider.size.y * 1.5f;
        playerCollider.size = currentdriftcollidersize;
        playerCollider.offset = currentdriftcollideroffset;
        playeranimation. DriftAnimOff();
        isdrift = false;
    }
    void StaminaControl()
    {

        CanDrift = true;
        driftTime -= Time.deltaTime * StaminaLessRate;
        if (isdrift && driftTime > 0f)
        {
            driftTime -= Time.deltaTime * StaminaBurnRate;
        }
        if (driftTime < 0f)
            driftTime = 0f;
        if (driftTime == 0f)
        {
            CanDrift= false;
            if(isdrift)
            {
                ExitDrift();
            }
           
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
        horizontalInput = PlayerInputHandler.Instance.Horizontal();
        bool isJumpAttack = playeranimation.FlyAttack();

        if (isJumpAttack && swordCoroutine == null)
        {
           
            StartCoroutine(flySword(0.07f));
        }
        
        else if (!isJumpAttack)
        {
            FlySwordAttackOff();
        }
    }
  private IEnumerator flySword(float delay)
    {
        yield return new WaitForSeconds (delay);
        FlySwordAttackOn();
    }
  public float GetVerticalVelocity() => rb.linearVelocity.y;
    private void Dead()
    {
        int CurrentHealth = PlayerController.Instance.CurrentPlayerHealth;
        if (CurrentHealth<= 0) 
        { 
            playeranimation.DeadAnim(); 
        }
    }
}
