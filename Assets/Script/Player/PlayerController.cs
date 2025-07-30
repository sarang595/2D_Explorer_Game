using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : PlayerService <PlayerController>
{
    //Represents Player Health/Life State
    public enum PlayerState { Alive,Dead}

    //Represents Player physical position
    public enum PlayerLocomotionState { Grounded, InAir}

    //Represents player action
    public enum PlayerActionstate { idle,run,crouch,jump,attack,push,death }

    [Header("Player Locomotion checker")]
    [SerializeField] Transform GroundChecker;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] float GroundRadius;

   [Header("Player Components")]
   [SerializeField] public float PlayerHealth;
   [SerializeField] public float PlayerSpeed;
   [SerializeField] public float JumpVelocity;

    [Header("Spawn Settings")]
    [SerializeField] private bool shouldSpawnOnStart = true;
    [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;


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
              
            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
       
        
    }

    void Start()
    {
     

        if (shouldSpawnOnStart)
        {
            SpawnPlayer();
        }

        
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event to prevent memory leaks
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        // Move player to the new scene's hierarchy
        SceneManager.MoveGameObjectToScene(gameObject, scene);
        StartCoroutine(SpawnPlayerDelayed());
    }

    private IEnumerator SpawnPlayerDelayed()
    {
        yield return new WaitForEndOfFrame(); // Wait for scene to fully load
      
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // Try to find a spawn point in the current scene
        PlayerSpawnPoint spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();

        if (spawnPoint != null)
        {
            // Disable CharacterController/Rigidbody2D temporarily to prevent physics issues
            CharacterController controller = GetComponent<CharacterController>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            
            if (controller != null) controller.enabled = false;
            
            // Spawn at the designated spawn point
            transform.position = spawnPoint.GetSpawnPosition();
            transform.rotation = spawnPoint.GetSpawnRotation();

            // Re-enable components
            if (controller != null) controller.enabled = true;

          
            
            Debug.Log($"Player spawned at: {transform.position} in scene: {SceneManager.GetActiveScene().name}");
        }
        else
        {
            // Fallback to default spawn position
            transform.position = defaultSpawnPosition;
            transform.rotation = Quaternion.identity;

            Debug.LogWarning($"No PlayerSpawnPoint found in scene '{SceneManager.GetActiveScene().name}'. Using default spawn position: {defaultSpawnPosition}");
        }
    }

  
    void Update()
    {
        CurrentPlayerState();
        PlayerInputHandler.Instance.ReadInput();
        Debug.Log(getLocomotionState().ToString());
        
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