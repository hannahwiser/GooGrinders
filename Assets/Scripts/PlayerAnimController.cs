using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    public Player player;
    public Animator animator;
    private bool prevBelowRail = false;
    private bool isCharging = false;
    private float chargeStartTime = 0.0f;

    void Update()
    {
        animator.SetBool("OnRail", player.OnRail);
        if (player.BelowRail != prevBelowRail)
        {
            if (prevBelowRail)
                animator.Play("BottomToTop");
            else
                animator.Play("TopToBottom");
        }

        // check if the player started charging the jump (this is the default state, because we don't
        // actually know if they're jumping immediately or charging the jump yet)
        if (player.jumpInput && !isCharging)
        {
            chargeStartTime = Time.time;
            isCharging = true;
        }

        // check if the player released the jump button or if they've exceeded 0.2 seconds of charge which means
        // that they're definitely trying to charge the jump meter
        if (!player.jumpInput || (Time.time - chargeStartTime) >= 0.2f)
        {
            isCharging = false;
        }

        // play the appropriate jump or charging animation
        if (isCharging)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            {
                // Only initialize ChargingJump once and play until the end
                animator.Play("ChargingJump", 0, 0);
            }
            else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ChargingJump"))
            {
                animator.Play("ChargingJump");
            }
        }
        else if (player.jumpInputRelease)
        {
            animator.Play("Jump");
        }

        animator.SetFloat("MovementInput", player.inputVector.x + .5f);
        prevBelowRail = player.BelowRail;
    }
}
