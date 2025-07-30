using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyControl : MonoBehaviour
{
    public Animator EnemyAimation; // Enemy Animator 
    [Range(0, 10)]
    [SerializeField] float PatrolSpeed = 5f; // Variable to Patrolspeed
    [Range(0, 10)]
    [SerializeField] float PatrolDistance = 5f; // Variable to Enemy Patrol Distance
    bool FacingRight; // To check whether enemy facing right or not
    bool CanPatrol;  // To specify condition when can patrol
    Vector2 PosA; // Enemy Primary Position
    Vector2 PosB; // Enemy Secondary Position
    Vector2 TargetPos; // Enemy Target Position
    float Originalx; // Original LocalScale X axis Component
    Vector3 Scale; // To store Localscale
    Vector3 EnemyDirection; // To store the Enemy direction
    [Range(-180, 360)]
    [SerializeField] float FovAngle; // To specify FOV Angle according to the need
    [Range(0, 10)]
    [SerializeField] float FovRadius; //  To specify Fov Radius according to the need
    Vector3 enemymaxDir; // To Store Eemy Max FOV vector which calculated from FOV Angle 
    Transform Player; // To Declare PlayerController. Instance
    bool IsPlayerEntered = false; //To track the Player entry to FOV
    float Attacktimer = 0f;
    [SerializeField] float AttackCoolDown = .25f;
    public GameObject Projectile;
    public Transform ProjectilePos;
    public float Projectileforce;
    private Awaitable CurrentAttack;
    public float attackCooldown = .25f;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    public enum EnemyMode { Patrol, Survilance }
    [SerializeField] public EnemyMode enemyMode = EnemyMode.Patrol; // Only this will show in inspector
    private void Start()
    {
        InitializePosition();
        CanPatrol = true;
    }


    private void Update()
    {
        PatrolCheck();
        EnemyMaxDirection();
        PlayerenteredBoundary();

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

    }
    private void PatrolCheck()
    {
        // Allowing enemy to patrol if enemy Canpatrol is true and not inside the boundary
        if (enemyMode == EnemyMode.Patrol && CanPatrol)
        {
            Patroll();
        }
        else if (enemyMode == EnemyMode.Survilance)
        {
            survilance();
        }
      
    }
    Vector3 getDirection()
    {
        //Declaring the enemy direction according to the declared conditions
        if (!FacingRight)
        {
            EnemyDirection = Vector2.left;
        }
        else
        {
            EnemyDirection = Vector2.right;
        }
        return EnemyDirection;
    }
    private void OnValidate()
    {
        if (EnemyDirection == Vector3.zero)
        {
            FacingRight = transform.localScale.x > 0;
            getDirection();
        }
    }

    private void InitializePosition()
    {
        //Initializes the enemy initial function and specifies Right Facing condition 
        Originalx = Mathf.Abs(transform.localScale.x);
        FacingRight = transform.localScale.x > 0;
        PosA = transform.position;
        if (FacingRight)
        {
            PosB = PosA + Vector2.right * PatrolDistance;

        }
        else
        {
            PosB = PosA + Vector2.left * PatrolDistance;

        }
        getDirection();
        TargetPos = PosB;
    }
    void survilance()
    {
        //disable the movement and enables enemy to turn towards player
        if (!IsPlayerEntered && enemyMode == EnemyMode.Survilance)
        {
            EnemyAimation.SetBool("IsSplitterWalk", false);
            EnemyAimation.SetBool("IsSplitterIdle", true);
            TurnEnemytoPlayer();
        }
    }

    private void Patroll()
    {
        //Moves enemy between PosA and PosB 
        EnemyAimation.SetBool("IsSplitterWalk", true);
        transform.position = Vector2.MoveTowards(transform.position, TargetPos, PatrolSpeed * Time.deltaTime);
        switchPosition();
    }
    private void switchPosition()
    {
        //Switches the enemy between PosA and PosB
        if (Vector2.Distance(transform.position, TargetPos) < 0.1f)
        {
            if (TargetPos == PosA)
            {
                TargetPos = PosB;
                FacingRight = true;

            }
            else
            {
                TargetPos = PosA;
                FacingRight = false;

            }
            getDirection();
            Flip();

        }
    }
    private void Flip()
    {
        // Flip the enemy according to the direction 
        Scale = transform.localScale;
        if (FacingRight)
        {
            Scale.x = Originalx;
        }
        else
        {
            Scale.x = -Originalx;
        }
        transform.localScale = Scale;
    }

    private void OnDrawGizmos()
    {
        // Visualizing the gizmos in editor to enemy positioning and customization
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + EnemyDirection * 2);
        UnityEditor.Handles.color = new Color(0, 1, 0, 0.05f);
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, getDirection(), relativeangle(), FovRadius);
        EnemyMaxDirection();
        Gizmos.DrawLine(transform.position, transform.position + EnemyMaxDirection() * 2);
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position,transform.position+ Playerlookup * 2);
    }
    float relativeangle()
    {
        //Changing the FOV angle to counter the direction flip
        if (!FacingRight)
        {
            return -FovAngle;
        }
        else
        {
            return FovAngle;
        }
    }
    private Vector3 EnemyMaxDirection()
    {
        // Calculating the Enemy Max FOV Vector using the know FOV (Serialized)
        float angleInRadians = FovAngle * Mathf.Deg2Rad;

        if (FacingRight)
        {
            enemymaxDir = new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0);
        }
        else
        {

            enemymaxDir = new Vector3(-Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0);
        }

        return enemymaxDir;
    }
    private void TurnEnemytoPlayer()
    {
        //Flips enemy according to the Player position
        if (Player == null) return;

        Vector3 scale = transform.localScale;
        bool wasFacingRight = FacingRight;

        if (Player.transform.position.x < transform.position.x)
        {
            scale.x = -Originalx;
            FacingRight = false;
        }
        else
        {
            scale.x = Originalx;
            FacingRight = true;
        }

        if (wasFacingRight != FacingRight)
        {
            if (FacingRight)
                TargetPos = PosB;
            else
                TargetPos = PosA;
        }

        transform.localScale = scale;
    }
    private async Awaitable PlayerenteredBoundary()
    {
        //Stores the Player and checks whether the player is inside the declared boundary

        if (this.Player == null)
        {
            PlayerController playercontrollerInstance = PlayerController.Instance;
            if (playercontrollerInstance != null)
            {
                this.Player = PlayerController.Instance.transform;
            }
            if (this.Player == null)
            {
                return;
            }
        }
        Debug.Log("FirstPhase");

        // calculating the distance between enemy position and player position

        Vector2 EnemyPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 PlayerPosition = new Vector2(Player.gameObject.transform.position.x, Player.gameObject.transform.position.y);
        float distanceToPlayer = Vector2.Distance(EnemyPosition, PlayerPosition);
        Debug.Log("Distance" + distanceToPlayer);

        // Checks the Distance between player and enemy is less than declared radius to figure out player is inside the boundary or not

        if (distanceToPlayer <= FovRadius)
        {
            IsPlayerEntered = true;
            CanPatrol = false;
            Debug.Log("Player Inside Boundary");

            TurnEnemytoPlayer();

            if (!isAttacking && attackTimer <= 0f)
            {
                isAttacking = true;
                await Attack();
                isAttacking = false;
            }
        }
        else
        {
            CanPatrol = true;
            IsPlayerEntered = false;
            
        }
       
    }
   private async Awaitable Attack()
    {
        //Projectile initialed from ProjectileBehaviour after 1f; of attack animation set to true and animation diabled after 0.5f of projectile initialization
        EnemyAimation.SetBool("IsSplitterAttack", true);
        await Awaitable.WaitForSecondsAsync(1f);
        ProjectileBehaviour projectile = Instantiate(Projectile, ProjectilePos.position, ProjectilePos.rotation).GetComponent<ProjectileBehaviour>();
        projectile.InitializeProjectile(Player, Projectileforce); 
        await Awaitable.WaitForSecondsAsync(0.5f);
        EnemyAimation.SetBool("IsSplitterAttack", false);
        EnemyAimation.SetBool("isSplitterAttackDown", true);
        attackTimer = attackCooldown;
    }

}
