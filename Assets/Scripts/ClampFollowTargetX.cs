/**
 * File: ClampFollowTargetX
 * Programmer: Sagar Patel
* Description: Keeps a game object named "FollowTarget" within horizontal
boundaries, making it follow the player's movements smoothly. It ensures that the
"FollowTarget" doesn't go too far to the left and adds a smoothing effect to the camera
movement to make it less jittery.
 * Date: Sept 17, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampFollowTargetX : MonoBehaviour
{
    private Transform playerTransform;
    private Transform followTargetTransform;
    private float minX;

    // Offset to prevent immediate clamping when the game starts
    public float minXOffset = 1.0f;

    // Smoothing factor to try to prevent the jumpyness of the camera
    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // "FollowTarget" is a child of player
        playerTransform = transform.parent;
        followTargetTransform = transform;

        // Initialize minX based on the initial player position + an offset
        minX = playerTransform.position.x + minXOffset;
    }

    void Update()
    {
        // Update minX if the player moves right
        if (playerTransform.position.x > minX)
        {
            minX = playerTransform.position.x;
        }

        // Calculate the target X position for "FollowTarget" based on the updated minX
        float targetX = Mathf.Max(minX, playerTransform.position.x);

        // interpolate the current X position of "FollowTarget" towards targetX
        float newX = Mathf.SmoothDamp(
            followTargetTransform.position.x,
            targetX,
            ref velocity.x,
            smoothTime
        );

        // Update the position of "FollowTarget"
        Vector3 newPosition = followTargetTransform.position;
        newPosition.x = newX;
        followTargetTransform.position = newPosition;
    }
}
