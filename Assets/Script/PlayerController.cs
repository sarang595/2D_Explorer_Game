using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Represents Player Health/Life State
    public enum PlayerState { Alive,Dead}

    //Represents Player physical position
    public enum PlayerLocomotionState { Grounded, InAir}

    //Represents player action
    public enum PlayerActionstate { idle,run,crouch,jump,attack,death }

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
        playerAttacking = PlayerInputHandler.Instance.Attacking();
        playerCrouching = PlayerInputHandler.Instance.Crouching();
        playerRunning = Mathf.Abs(PlayerInputHandler.Instance.Horizontal()) > 0.01f;
        playerIdle = Mathf.Abs(PlayerInputHandler.Instance.Horizontal()) < 0.01f;
        playerInAir = getLocomotionState() == PlayerLocomotionState.InAir;
        playerJumping = PlayerInputHandler.Instance.Jump();
    }
    // Player State Conditions
    public bool CanRun() => playerAlive && playerRunning;
    public bool CanAttack() => playerAlive && playerAttacking && !playerCrouching;

    public bool CanJump() => playerAlive && playerGrounded && playerJumping;

    public bool CanCrouch() => playerAlive && playerGrounded&& playerCrouching;
     
    public bool CrouchUp() => playerAlive && playerGrounded && !playerCrouching;

    public bool PlayerinAir() => playerAlive && playerInAir;
    public bool JumpAttack() => playerAlive && playerAttacking && playerInAir;
    public bool PlayerAttacking() => playerAttacking;






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


}
