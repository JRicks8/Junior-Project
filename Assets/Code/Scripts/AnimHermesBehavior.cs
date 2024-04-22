using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHermesBehavior : StateMachineBehaviour
{
    private enum Parameters
    {
        xzVelMag,
        yVelocity,
        isGrounded,
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

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (referencesSet)
        {
            animator.SetFloat(Parameters.xzVelMag.ToString(), new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
            animator.SetFloat(Parameters.yVelocity.ToString(), rb.velocity.y);
            animator.SetBool(Parameters.isGrounded.ToString(), playerController.IsGrounded());
        }
    }
}