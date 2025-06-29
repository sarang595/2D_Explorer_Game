using System;
using UnityEngine;

public class EnemyPatroll : MonoBehaviour
{
    [SerializeField] float PatrollSpeed = 5f;
    [SerializeField] float Distance = 3f;
    [SerializeField] private Transform patrollboundary;
    [SerializeField] private LayerMask Target;
    [SerializeField, Range(0f, 10f)] private float patrolRadius;
    [SerializeField] Transform Player;
   
   
    public Animator EnemyAimation;

    Vector3 PosA;
    Vector3 PosB;
    Vector3 TargetPos;
    private float originalScaleX;
    bool facingRight = true;
    bool CanPatroll = true;

   

    void Start()
    {
        InitialPosition();

       
    }

   

    void Update()
    {
        patrollCheck();

       
    }
    
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

        if (CanPatroll)
        {
            Patroll();
        }
        else
        {
            return;
        }

    }
    void Patroll()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            TargetPos,
            PatrollSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, TargetPos) < 0.1f)
        {
            if (TargetPos == PosA)
            {
                TargetPos = PosB;
                EnemyFlip();
            }
            else if (TargetPos == PosB)
            {
                TargetPos = PosA;
                EnemyFlip();
            }
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
            facingRight = false; // Sync variable with visual
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
            facingRight = true; // Sync variable with visual
        }

        // Update target position if direction changed
        if (wasFacingRight != facingRight)
        {
            if (facingRight)
                TargetPos = PosB;
            else
                TargetPos = PosA;
        }

        transform.localScale = scale;

    }

    private void EnemyFlip()
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

    // Only checks if player is in range, does not change any state
    private bool IsPlayerInRange()
    {
        CanPatroll = false;
        return Physics2D.OverlapCircle(patrollboundary.position, patrolRadius, Target);
    }

    private void OnDrawGizmos()
    {
        if (patrollboundary != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(patrollboundary.position, patrolRadius);
        }
    }

  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
          
            CanPatroll = false;
            if (EnemyAimation != null)
                EnemyAimation.SetBool("IsSplitterAttack", true);
            PlayerEntered();
          

        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        CanPatroll = false;
        if (EnemyAimation != null)
            EnemyAimation.SetBool("IsSplitterAttack", true);
        PlayerEntered();
       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        CanPatroll = true;
        EnemyAimation.SetBool("IsSplitterAttack", false);
        InitialPosition();
    }
}
