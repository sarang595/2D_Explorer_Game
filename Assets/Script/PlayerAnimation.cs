using UnityEngine;
using static PlayerController;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnimator;
     
    public void RunAnim()
    {

        bool CanRun = PlayerController.Instance.CanRun();


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

        Debug.Log(CurrentJumpVelocity); // Always log velocity for debugging

        // Set IsJump if the player is not grounded (i.e., in the air)
        PlayerAnimator.SetBool("IsJump", inAir);
        if (inAir)
        {
            bool isjumpup = CurrentJumpVelocity > 0.1f;
            bool isFalling = CurrentJumpVelocity < -0.1f;

            // Set JumpUP if the player is moving upwards
            PlayerAnimator.SetBool("JumpUP", isjumpup);
           
            // Set JumpDown if the player is moving downwards
            PlayerAnimator.SetBool("JumpDown", isFalling);
            
        }
        

    }
    public void JumpAttackcalling()
        
    {
      
        bool jumpattack = PlayerController.Instance.JumpAttack();
        if (jumpattack)
        {
            PlayerAnimator.SetBool("JumpUP", false);
            PlayerAnimator.SetBool("JumpDown", false);
            PlayerAnimator.SetBool("FlyAttack", true);
            PlayerAnimator.SetBool("IsJump", true);

        }

       else
    {
        // Only reset when not attacking
        //PlayerAnimator.SetBool("FlyAttack", false);
    }
    }
   

    public void AttackAnim()
    {
        bool canattack = PlayerController.Instance.CanAttack() && PlayerController.Instance.Isgrounded();
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


        PlayerAnimator.SetBool("IsCrouch", CanCrouch);

    }

   
}
