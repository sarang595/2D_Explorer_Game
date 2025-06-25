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
       
        PlayerInputHandler.Instance.ReadInput();
        playerMovement.SwordAttack();
        playerMovement.Jump();
        playerMovement.Crouch();
        Debug.Log (getPlayerState().ToString());
        Debug.Log (getLocomotionState().ToString());
        Debug.Log(getPlayerActionstate().ToString());

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
    public PlayerActionstate getPlayerActionstate()
    {
        bool PlayerDead = getPlayerState() == PlayerState.Dead;
        bool PlayerAlive = getPlayerState() == PlayerState.Alive;
        bool PlayerGrounded = getLocomotionState() == PlayerLocomotionState.Grounded;
        bool PlayerAttacking = PlayerInputHandler.Instance.Attacking();
        bool PlayerCrouching = PlayerInputHandler.Instance.Crouching();
        bool PlayerRuning = Mathf.Abs(PlayerInputHandler.Instance.Horizontal()) > 0.01f;
        bool PlayerInAir = getLocomotionState() == PlayerLocomotionState.InAir;

        if (PlayerDead)

        return PlayerActionstate.death;

        if (PlayerAlive)
        {
            if (PlayerGrounded)
            {
                if (PlayerAttacking)
                    return PlayerActionstate.attack;

                if (PlayerCrouching)
                    return PlayerActionstate.crouch;

                if (PlayerRuning)
                    return PlayerActionstate.run;

                return PlayerActionstate.idle;
            }
            else if (PlayerInAir)
            {
                if (PlayerAttacking)
                    return PlayerActionstate.attack;

                return PlayerActionstate.jump;
            }
        }

        return PlayerActionstate.idle;
    }
   



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
    
}
