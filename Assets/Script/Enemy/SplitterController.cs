using System;
using System.Collections;
using UnityEngine;

public class SplitterController : MonoBehaviour
{
    [SerializeField] float PatrollSpeed = 5f;
    [SerializeField] float Distance = 3f;
    [SerializeField] private LayerMask Target;
    [SerializeField, Range(0f, 10f)] private float patrolRadius;
    Transform Player;
    public GameObject Projectile;
    public Transform ProjectilePos;
    public float Projectileforce;
    Rigidbody2D rb2;
    
    private GameObject SpawnProjectile;

    public enum EnemyMode { Patrol, Survilance }
    [SerializeField] public EnemyMode enemyMode = EnemyMode.Patrol; // Only this will show in inspector
    bool canPatrol;

    public Animator EnemyAimation;

    Vector3 PosA;
    Vector3 PosB;
    Vector3 TargetPos;
    private float originalScaleX;
    bool facingRight = true;
    bool IsPlayerEntered = false;
    public float attackCooldown = .25f;
    private float attackTimer = 0f;

    void Start()
    {
   
        if (PlayerController.Instance != null)
        {
            Player = PlayerController.Instance.transform;
        }
        canPatrol = true;
        InitialPosition();
    }

    void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        patrollCheck();
       
       
;    }

    private void InitialPosition()
    {
        originalScaleX = Math.Abs(transform.localScale.x);
        facingRight = transform.localScale.x > 0;
        PosA = transform.position;
        if (facingRight)
        {
            PosB = PosA + Vector3.right * Distance;
        }
        else
        {
            PosB = PosA + Vector3.left * Distance;
        }
        TargetPos = PosB;
    }

    void patrollCheck()
    {
        if (enemyMode == EnemyMode.Patrol && canPatrol)
        {
            Patroll();
        }
        else if (enemyMode == EnemyMode.Survilance)
        {
            survilance();
        }
    }

    void Patroll()
    {
        EnemyAimation.SetBool("IsSplitterWalk", true);

        transform.position = Vector3.MoveTowards(
            transform.position,
            TargetPos,
            PatrollSpeed * Time.deltaTime
        );

        SplitterSwitchPos();
    }

    void survilance()
    {
        if (!IsPlayerEntered && enemyMode == EnemyMode.Survilance)
        {
            EnemyAimation.SetBool("IsSplitterWalk", false);
            EnemyAimation.SetBool("IsSplitterIdle", true);
            SplitterSwitchPos();
        }
    }

    private void PlayerEntered()
    {
        Debug.Log("Player entered Boundary");
        if (Player == null) return;

        Vector3 scale = transform.localScale;
        bool wasFacingRight = facingRight;

        if (Player.transform.position.x < transform.position.x)
        {
            scale.x = -Mathf.Abs(scale.x);
            facingRight = false;
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
            facingRight = true;
        }

        if (wasFacingRight != facingRight)
        {
            if (facingRight)
                TargetPos = PosB;
            else
                TargetPos = PosA;
        }

        transform.localScale = scale;
    }

    private void SplitterFlip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        if (facingRight)
        {
            scale.x = originalScaleX;
        }
        else
        {
            scale.x = -originalScaleX;
        }
        transform.localScale = scale;
       
    }


    void SplitterSwitchPos()
    {
        if (Vector3.Distance(transform.position, TargetPos) < 0.1f)
        {
            if (TargetPos == PosA)
            {
                TargetPos = PosB;
                SplitterFlip();
            }
            else if (TargetPos == PosB)
            {
                TargetPos = PosA;
                SplitterFlip();
            }
        }
    }
    

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canPatrol = false;
        if (collision.gameObject.transform == Player)
        {
            if (EnemyAimation != null)
            IsPlayerEntered = true;
            Invoke("attack", 1f);
            EnemyAimation.SetBool("IsSplitterAttack", true);
            PlayerEntered();
           
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        canPatrol = false;
        if (EnemyAimation != null)
        IsPlayerEntered = true;
        Invoke("attack", 1f);
        EnemyAimation.SetBool("IsSplitterAttack", true);
        PlayerEntered();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPatrol = true;
        if (EnemyAimation != null)
        EnemyAimation.SetBool("IsSplitterAttack", false);
        IsPlayerEntered = false;
        InitialPosition();
    }
    
   void attack()
    {
        if (attackTimer <= 0f)
        {
            ProjectileBehaviour projectile = Instantiate(Projectile, ProjectilePos.position, ProjectilePos.rotation).GetComponent<ProjectileBehaviour>();
            projectile.InitializeProjectile(Player, Projectileforce);
            //Rigidbody2D projRb = SpawnProjectile.GetComponent<Rigidbody2D>();
            // Calculate direction from projectile spawn to player
            //Vector2 direction = (Player.position - ProjectilePos.position).normalized;
            //projRb.AddForce(Vector2.right * direction * Projectileforce, ForceMode2D.Impulse);
            
            Invoke("animationoff", 0.5f);
            attackTimer = attackCooldown;
        }
        

    }
    void animationoff()
    {
        EnemyAimation.SetBool("IsSplitterAttack", false);
        EnemyAimation.SetBool("isSplitterAttackDown", true);
    }
   
}