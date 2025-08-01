using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator PlayerAnimator;
    private PlayerAction PlayerAction;
    private bool flyAttack = false;
    bool PushPower;
    private Coroutine airAttackCoroutine;

    private void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerAction = GetComponent<PlayerAction>();
    }

    private void Update()
    {
        RunAnim();
        JumpAnim();
        PushAnim();
    }

    public void RunAnim()
    {
        bool canRun = PlayerController.Instance.CanRun() && !PlayerController.Instance.Crouching();
        float horizontalValue = PlayerInputHandler.Instance.Horizontal();

        PlayerAnimator.SetFloat("MoveSpeed", canRun ? Mathf.Abs(horizontalValue) : 0f);
    }

    public void JumpAnim()
    {
        float currentJumpVelocity = PlayerAction.GetVerticalVelocity();
        bool inAir = PlayerController.Instance.PlayerinAir();
        bool grounded = PlayerController.Instance.PlayerGrounded();
        bool attacking = PlayerController.Instance.PlayerAttacking();

       
        if (!flyAttack)
        {
            PlayerAnimator.SetBool("IsJump", inAir);

            if (inAir && !grounded)
            {
                PlayerAnimator.SetBool("JumpUP", currentJumpVelocity > 0.1f);
                PlayerAnimator.SetBool("JumpDown", currentJumpVelocity < -0.1f);
            }
            else
            {
                PlayerAnimator.SetBool("JumpUP", false);
                PlayerAnimator.SetBool("JumpDown", false);
            }
        }

        // Handle in-air attack: interrupts jump/fall anims immediately, prevents overlap
        if (attacking && inAir && !flyAttack)
        {
            // Cancel jump/fall anims
            PlayerAnimator.SetBool("JumpUP", false);
            PlayerAnimator.SetBool("JumpDown", false);

            // Start the air attack animation coroutine if not already running
            airAttackCoroutine = StartCoroutine(PlayerAirAttack());
        }
    }

    private IEnumerator PlayerAirAttack()
    {
       
        flyAttack = true;
        PlayerAnimator.SetBool("Isattack", true);

        // Keep attack anim for only a short duration for “real-time” response
        yield return new WaitForSeconds(0.5f);

        PlayerAnimator.SetBool("Isattack", false);
        flyAttack = false;
        airAttackCoroutine = null;
    }

    // This can still be called for ground attacks.
    public void AttackAnim()
    {
        bool canAttack = PlayerController.Instance.CanAttack();
        PlayerAnimator.SetBool("Isattack", canAttack);
    }

    public void CrouchAnim()
    {
        bool canCrouch = PlayerController.Instance.CanCrouch();
        PlayerAnimator.SetBool("IsCrouch", canCrouch);
        if (canCrouch)
        PlayerAnimator.SetFloat("MoveSpeed", 0);
    }

    public void PushAnim()
    {
       
        PushPower = PlayerController.Instance.CanPushPower();
        if (PushPower)
        {
            float horizontalValue = PlayerInputHandler.Instance.Horizontal();
            bool CanPush = PlayerController.Instance.CanPush();

            // Set IsPush based on PushPower
            PlayerAnimator.SetBool("IsPush", CanPush);

            // Only set IsPushMove if we can push AND there's horizontal input
            if (CanPush && Mathf.Abs(horizontalValue) >= 0.2f)
            {
                PlayerAnimator.SetBool("IsPush", false);

                PlayerAnimator.SetBool("IsPushMove", true);

            }
            else
            {
                PlayerAnimator.SetBool("IsPushMove", false);

            }
            PushPower = false;
        }
       
         else
        {
            PlayerAnimator.SetBool("IsPushMove", false);

        }
    }

    public bool FlyAttack() => flyAttack;

   
}
