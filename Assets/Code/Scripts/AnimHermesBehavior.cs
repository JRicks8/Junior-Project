using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHermesBehavior : StateMachineBehaviour
{
    public enum HermesAnimParameters
    {
        xzVelMag,
        yVelocity,
        isGrounded,
        grappling,
        jumpTrigger,
        landTrigger,
        grappleTrigger,
        stopGrappleTrigger,
        diveTrigger,
        dashTrigger,
        drownTrigger,
    }

    private bool referencesSet = false;

    private PlayerController playerController;
    private Animator playerAnimator;
    private Rigidbody rb;

    public void SetReferences(PlayerController playerController, Animator playerAnimator, Rigidbody rb)
    {
        this.playerController = playerController;
        this.playerAnimator = playerAnimator;
        this.rb = rb;

        referencesSet = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (referencesSet)
        {
            playerAnimator.SetFloat(HermesAnimParameters.xzVelMag.ToString(), new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
            playerAnimator.SetFloat(HermesAnimParameters.yVelocity.ToString(), rb.velocity.y);
            playerAnimator.SetBool(HermesAnimParameters.isGrounded.ToString(), playerController.IsGrounded());
            playerAnimator.SetBool(HermesAnimParameters.isGrounded.ToString(), playerController.IsGrappling());
        }
    }
}