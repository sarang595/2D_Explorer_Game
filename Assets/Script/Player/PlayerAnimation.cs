using System.Collections;
using UnityEngine;
using static PlayerController;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnimator;
    bool flyAttack = false;
    bool PushPower;
    private void Start()
    {
        PushPower = false;
    }
    private void Update()
    {
     JumpAnim();
     PushAnim();
    }
    public void RunAnim()
    {

        bool CanRun = PlayerController.Instance.CanRun() && !PlayerController.Instance.Crouching();
        


        if (CanRun)
        {
            float horizontalValue = PlayerInputHandler.Instance.Horizontal();
            PlayerAnimator.SetFloat("MoveSpeed", Mathf.Abs(horizontalValue));
        }
        else
        {
            PlayerAnimator.SetFloat("MoveSpeed", 0);
        }
       

    }
    public void JumpAnim()
    {
       

      
        float CurrentJumpVelocity = PlayerController.Instance.VerticalVelocity();
        bool inAir = PlayerController.Instance.PlayerinAir();
        bool grounded = PlayerController.Instance.PlayerGrounded();
        bool attacking = PlayerController.Instance.PlayerAttacking();
      

     

        // Set IsJump if the player is not grounded (i.e., in the air)
        PlayerAnimator.SetBool("IsJump", inAir);
        if (inAir&&!grounded)
        {
           
            bool isjumpup = CurrentJumpVelocity > 0.1f;
            bool isFalling = CurrentJumpVelocity < -0.1f;

            // Set JumpUP if the player is moving upwards
            PlayerAnimator.SetBool("JumpUP", isjumpup);
            
            // Set JumpDown if the player is moving downwards
             PlayerAnimator.SetBool("JumpDown", isFalling);
            
        }
        else
        {
           
            PlayerAnimator.SetBool("JumpUP", false);

            // Set JumpDown if the player is moving downwards
            PlayerAnimator.SetBool("JumpDown", false);

        }
       

        if (attacking && inAir)
        {
          StartCoroutine(PlayerAirAttack());
        }

       

    }
   
    public IEnumerator PlayerAirAttack()
    {
        flyAttack = true;
        yield return new WaitForSeconds(0.48f);
        PlayerAnimator.SetBool("Isattack", true);

        //  wait for the current attack animation duration
        yield return new WaitForSeconds(PlayerAnimator.GetCurrentAnimatorStateInfo(0).length);

        PlayerAnimator.SetBool("Isattack", false);
        flyAttack =false;
       
    }

    

    public void AttackAnim()
    {
        bool canattack = PlayerController.Instance.CanAttack() ;
      
        if (canattack)
        {
          PlayerAnimator.SetBool("Isattack", canattack);

        }
        else
        {
            PlayerAnimator.SetBool("Isattack", false);
        }
       
      
    }
    

    public void CrouchAnim()
    {
        bool CanCrouch = PlayerController.Instance.CanCrouch();
      
        if (CanCrouch )
        {
            PlayerAnimator.SetBool("IsCrouch", CanCrouch);
            PlayerAnimator.SetFloat("MoveSpeed", 0);
        }
        else
        {
            PlayerAnimator.SetBool("IsCrouch", false);
        }
    }
    public void PushAnim()
    {
        PushPower = PlayerController.Instance.CanPushPower();
        if (PushPower)
        {
            float horizontalValue = PlayerInputHandler.Instance.Horizontal();
            bool CanPush = PlayerController.Instance.CanPush();

            // Set IsPush based on CanPush
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
