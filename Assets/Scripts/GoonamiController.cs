/**
 * File: GoonamiController
 * Programmer: Sagar Patel
 * Description: Simple script to controll the Goonami wave. The actual killing and playing the death audio of the player happens in PlayerLife.cs.
 * Date: Oct 11, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonamiController : MonoBehaviour
{
    // adjust the follow speed. The lower the number, the faster the goonami will follow (you want it to be a bit slow so that it feels more like a wave
    public float followSpeed = 2.0f;

    // The distance that the goonami will stay behind before it stops moving
    public float xOffset = -45.0f;
    private float savexOffset;
    // offset for the very first few seconds of the game
    public float initialXOffset = -70.0f;
    // number of seconds to apply the initial offset
    public float xOffsetTransitionTime = 9.0f;
    private float xOffsetTimer = 0.0f;


    private Vector3 startPosition;
    private Transform followTarget;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        startPosition = transform.position;
        // automatically find the object named "FollowTarget"
        followTarget = GameObject.Find("FollowTarget").transform;

        savexOffset = xOffset;
        StartCoroutine(TransitionXOffset());
    }

    void Update()
    {
        // calculate the target position behind the FollowTarget
        //targetPosition = new Vector3(followTarget.position.x + xOffset, transform.position.y, transform.position.z);

        if (xOffsetTimer < xOffsetTransitionTime)
        {
            xOffset = Mathf.Lerp(initialXOffset, xOffset, xOffsetTimer / xOffsetTransitionTime);
            xOffsetTimer += Time.deltaTime;
        }

        targetPosition = new Vector3(
            followTarget.position.x + xOffset,
            followTarget.position.y,
            transform.position.z
        ); // follow the player along the Y axis as well

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            followSpeed
        );
    }

    public void ResetPositionToStart()
    {
        transform.position = startPosition;
    }

    IEnumerator TransitionXOffset()
    {
        float timer = 0.0f;
        while (timer < xOffsetTransitionTime)
        {
            xOffset = Mathf.Lerp(initialXOffset, xOffset, timer / xOffsetTransitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        xOffset = savexOffset;
    }

}
