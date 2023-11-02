/**
 * File: FlyMovement
 * Programmer: Sagar Patel
 * Description: A script to control the main menu goo gorgers (flies). These are meant to be tiny little guys that fly arount sporatically
 * Date: Oct 13, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMovement : MonoBehaviour
{
    // Goo gorger flying speed
    public float moveSpeed = 10.0f;

    // max rotation speed, degrees per second
    public float maxRotationSpeed = 500.0f;

    // spherical area that the fly will fly around in
    public float sphereRadius = 10.0f;

    // If you want a mini goo gorger, scale it down to 0.05, moveSpeed = 5, maxRotationSpeed = 700, sphereRadius = 1
    // Place a bunch around each other and they will buzz around like a bunch of stinky stinkers

    private Vector3 initialPosition;

    private void Start()
    {
        // store the initial position
        initialPosition = transform.position;

        // start the movement coroutine
        StartCoroutine(MoveFly());
    }

    private IEnumerator MoveFly()
    {
        while (true)
        {
            // get a new random target position and calculate the rotation
            Vector3 targetPosition = GetRandomPosition();
            Quaternion targetRotation = Quaternion.LookRotation(
                targetPosition - transform.position
            );

            // rotate the fly 90 degrees to the right, because Emily's model is actually rotated left by 90 degrees.
            targetRotation *= Quaternion.Euler(0, 90, 0);

            // base rotation speed on the angle between current and target rotation to reduce the wonk
            float rotationSpeed =
                Mathf.Min(1.0f, Quaternion.Angle(transform.rotation, targetRotation) / 180.0f)
                * maxRotationSpeed;

            // move and rotate the Goo Gorger towards the target position
            // and rotate the Goo Gorger
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
                yield return null;
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        // pick a random position relative to the initial position inside the sphere
        Vector3 randomDirection = Random.onUnitSphere * sphereRadius;
        return initialPosition + randomDirection;
    }
}
