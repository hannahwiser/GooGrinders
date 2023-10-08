/**
 * File: RagdollController
 * Programmer: Sagar Patel
 * Description: Simple ragdoll script so the player can go into ragdoll mode when he hits a deadly object. 
 * Make sure to attach all the rigidbodies of Sporet's body to the "bodyParts" array in the RagdollController component
 * Date: Oct 8, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Rigidbody[] bodyParts;

    // disable ragdoll physics by default
    private void Awake()
    {
        DisableRagdoll();
        //EnableRagdoll();
    }

    // disable ragdoll physics
    public void DisableRagdoll()
    {
        foreach (var bodyPart in bodyParts)
        {
            bodyPart.isKinematic = true;
            //bodyPart.detectCollisions = false;
        }
    }

    // enable ragdoll physics
    public void EnableRagdoll()
    {
        foreach (var bodyPart in bodyParts)
        {
            bodyPart.isKinematic = false;
            bodyPart.detectCollisions = true;
        }
    }
}
