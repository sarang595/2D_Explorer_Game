using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator PlayerAnimator;
    private PlayerAction PlayerAction;
    private bool flyAttack = false;
    bool push = false;
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
        PushCheck();
    }

    public void RunAnim()
    {
        bool canRun = PlayerController.Instance.CanRun() && !PlayerController.Instance.CanDrift();
        float horizontalValue = PlayerInputHandler.Instance.Horizontal();

        PlayerAnimator.SetFloat("MoveSpeed", canRun ? Mathf.Abs(horizontalValue) : 0f);
    }

    public void JumpAnim()
    {

        float currentJumpVelocity = PlayerAction.GetVerticalVelocity();
        bool inAir = PlayerController.Instance.PlayerinAir();
        bool grounded = PlayerController.Instance.PlayerGrounded();
        bool attacking = PlayerController.Instance.PlayerAttacking();
        bool Pushing = PlayerInputHandler.Instance.Pushing();


        if (!flyAttack && !push)
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
        if (attacking && inAir && !flyAttack && !push)
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

        // Keep attack anim for only a short duration for "real-time" response
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

    public void DriftAnim()
    {
        bool CanDrift = PlayerController.Instance.CanDrift();
        PlayerAnimator.SetBool("IsDrift", CanDrift);
        if (CanDrift)
            PlayerAnimator.SetFloat("MoveSpeed", 0);
    }

    void PushCheck()
    {
        // Check if we should be pushing
        bool shouldPush = PlayerController.Instance.CanPushPower();
        bool pushPressed = PlayerInputHandler.Instance.Pushing();

        // We can only push if BOTH conditions are true: ability to push AND button is pressed
        bool canActuallyPush = shouldPush && pushPressed;

        if (canActuallyPush && !push)
        {
            // Start pushing
            PushAnim();
        }
        else if (!canActuallyPush && push)
        {
            // Stop pushing - clear all push states
            // This covers: (!shouldPush OR !pushPressed) AND currently pushing
            EndPushAnim();
        }
        else if (push && canActuallyPush)
        {
            // Continue pushing animation only if we still can and should
            PushAnim();
        }
    }

    public void PushAnim()
    {
        push = true;
        bool grounded = PlayerController.Instance.PlayerGrounded();
        bool canPushPower = PlayerController.Instance.CanPushPower();

        if (canPushPower)
        {
            float horizontalValue = PlayerInputHandler.Instance.Horizontal();
            bool canPush = PlayerController.Instance.CanPush();

            // Set IsPush based on ability to push
            PlayerAnimator.SetBool("IsPush", canPush);

            // Only set IsPushMove if we can push AND there's horizontal input
            if (canPush && Mathf.Abs(horizontalValue) >= 0.2f)
            {
                PlayerAnimator.SetBool("IsPush", false);
                PlayerAnimator.SetBool("IsPushMove", true);
            }
            else
            {
                PlayerAnimator.SetBool("IsPushMove", false);
            }
        }
    }

    private void EndPushAnim()
    {
        // Clear all push-related animation states
        PlayerAnimator.SetBool("IsPush", false);
        PlayerAnimator.SetBool("IsPushMove", false);
        push = false;

        Debug.Log("Push animation ended - jump should now work"); // Debug line
    }

    public void DeadAnim()
    {
        bool Dead = PlayerController.Instance.PlayerDead();

        PlayerAnimator.SetBool("IsDead", true);
    }

    public bool FlyAttack() => flyAttack;

    public bool Ispushing() => push;
}