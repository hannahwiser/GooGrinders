using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    // Start is called before the first frame update
    public Player player;
    public Animator animator;
    private bool prevBelowRail = false;

    void Update()
    {
        animator.SetBool("OnRail", player.OnRail);
        if (player.BelowRail != prevBelowRail)
        {
            if(prevBelowRail)
            animator.Play("BottomToTop");
            else
            animator.Play("TopToBottom");
        }

        if (player.jumpInput)
            animator.SetTrigger("Jumpped");

        animator.SetFloat("MovementInput", player.inputVector.x + .5f);
        prevBelowRail = player.BelowRail;
    }
}
