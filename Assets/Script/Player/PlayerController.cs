using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Represents Player Health/Life State
    public enum PlayerState { Alive,Dead}

    //Represents Player physical position
    public enum PlayerLocomotionState { Grounded, InAir}

    //Represents player action
    public enum PlayerActionstate { idle,run,crouch,jump,attack,push,death }

    private static PlayerController instance;
    public static PlayerController Instance {  get { return instance; } }

    

    [Header("Player Locomotion checker")]
    [SerializeField] Transform GroundChecker;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] float GroundRadius;

   [Header("Player Components")]
   [SerializeField] private PlayerAction playerMovement;
   [SerializeField] public float PlayerHealth;
   [SerializeField] public float PlayerSpeed;
   [SerializeField] public float JumpVelocity;

   public PlayerLocomotionState locomotionState;
   public PlayerState state;
   public PlayerActionstate actions;
   private bool live = true;
    private bool playerDead;
    private bool playerAlive;
    private bool playerGrounded;
    private bool playerAttacking;
    private bool playerCrouching;
    private bool playerRunning;
    private bool playerIdle;
    private bool playerInAir;
    private bool playerJumping;
    private bool playerpushing;
    private bool landingFrame;
    private bool wasGrounded; // Track previous frame's grounded state
    private bool PushPower=false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);    
        }
        
    }

    void Start()
    {
        
    }

   
    void Update()
    {
        CurrentPlayerState();
        PlayerInputHandler.Instance.ReadInput();
        playerMovement.SwordAttack();
        playerMovement.Crouch();
        playerMovement.Jump();
        Debug.Log(getLocomotionState().ToString());
        
    }
    private void FixedUpdate()
    {
        playerMovement.Run();
        

    }
    public PlayerLocomotionState getLocomotionState()
    {
        if(Isgrounded())
        {
            locomotionState = PlayerLocomotionState.Grounded;
        }
        else
        {
            locomotionState = PlayerLocomotionState.InAir;
        }
        return locomotionState;

    }
    public PlayerState getPlayerState()
    {
        if (Live()==true)
        {
            state = PlayerState.Alive;
        }
        else
        {
            state = PlayerState.Dead;
        }
        return state;
    }
   

    private void CurrentPlayerState()
    {
        playerDead = getPlayerState() == PlayerState.Dead;
        playerAlive = getPlayerState() == PlayerState.Alive;
        playerGrounded = getLocomotionState() == PlayerLocomotionState.Grounded;
        playerInAir = getLocomotionState() == PlayerLocomotionState.InAir;
        playerIdle = Mathf.Abs(PlayerInputHandler.Instance.Horizontal()) < 0.01f;
        playerRunning = Mathf.Abs(PlayerInputHandler.Instance.Horizontal()) > 0.01f;
        playerAttacking = PlayerInputHandler.Instance.Attacking();
        playerJumping = PlayerInputHandler.Instance.Jump();
        playerCrouching = PlayerInputHandler.Instance.Crouching();
        playerpushing = PlayerInputHandler.Instance.Pushing();
       
        
       
       
    }
    // Player State Conditions
    public bool CanRun() => playerAlive && playerRunning && !playerCrouching;
    public bool CanAttack() => playerAlive && playerAttacking && !playerCrouching;

    public bool CanJump() => playerAlive && playerGrounded && playerJumping;

    public bool CanCrouch() => playerAlive && playerGrounded&& playerCrouching;
     
    public bool CrouchUp() => playerAlive && playerGrounded && !playerCrouching;

    public bool PlayerinAir() => playerAlive && playerInAir;
    public bool JumpAttack() => playerAlive && playerAttacking && playerInAir;
    public bool PlayerAttacking() => playerAttacking;

    public bool PlayerGrounded() => playerGrounded;

    public bool Jumping() => playerAlive && playerJumping;
    public bool Crouching() => playerAlive && playerCrouching;

    public bool CanPush() => playerAlive && playerGrounded && playerpushing;







    public bool Isgrounded()
    {
        return Physics2D.OverlapCircle(GroundChecker.position, GroundRadius, GroundLayer);
    }
    public bool Live()
    {
        if(PlayerHealth>0)
        {
            live = true;
        }
        else if(PlayerHealth<=0)
        {
            live = false;
        }
        return live;
    }

   public float VerticalVelocity()
    {
        float CurrentVerticalVelocity = playerMovement.GetVerticalVelocity();
        return CurrentVerticalVelocity;
    }
    private void OnDrawGizmos()
    {
        if (GroundChecker != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(GroundChecker.position, GroundRadius);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("PushTrigger"))
        {
            PushPower = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PushTrigger"))
        {
            PushPower = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PushTrigger"))
        {
            PushPower = false;
        }
    }
    public bool CanPushPower() => PushPower;
}
