using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Represents Player Health/Life State
    public enum PlayerState { Alive,Dead}

    //Represents Player physical position
    public enum PlayerLocomotionState { Grounded, InAir}

    //Represents player action
    public enum PlayerActions { Idle,Run,Crouch,Jump,Attack }

    private static PlayerController instance;
    public static PlayerController Instance {  get { return instance; } }

    [Header("Player Locomotion checker")]
    [SerializeField] Transform GroundChecker;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] float GroundRadius;

   [Header("Player Components")]
   [SerializeField] private PlayerMovement playerMovement;
   [SerializeField] public float PlayerHealth;
   [SerializeField] public float PlayerSpeed;
   [SerializeField] public float JumpVelocity;

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
      
      
           
       
    }
    private void FixedUpdate()
    {
        playerMovement.Run();
    
    }
}
