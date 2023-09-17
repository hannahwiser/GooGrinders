/**
 * File: ClampFollowTargetY
 * Programmer: Sagar Patel
 * Description: Makes sure "FollowTarget" stays within certain up-and-down
boundaries while following the player's vertical movements. It prevents the
"FollowTarget" from going too low or too high and smoothens the camera's vertical movement to
make it less jerky. 
* Date: Sept 17, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampFollowTargetY : MonoBehaviour
{
    private Transform playerTransform;
    private Transform followTargetTransform;

    public float minY = -5f;
    public float maxY = 5f;
    // Smoothing factor to try to prevent the jumpyness of the camera
    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // "FollowTarget" is a child of player 
        playerTransform = transform.parent; 
        followTargetTransform = transform;
    }

    void Update()
    {
        // Calculate the Y position for "FollowTarget" based on the player's position
        float targetY = Mathf.Clamp(playerTransform.position.y, minY, maxY);

        // interpolate the current Y position of "FollowTarget" towards targetY
        float newY = Mathf.SmoothDamp(followTargetTransform.position.y, targetY, ref velocity.y, smoothTime);

        // Update the position of "FollowTarget"
        Vector3 newPosition = followTargetTransform.position;
        newPosition.y = newY;
        followTargetTransform.position = newPosition;
    }
}
