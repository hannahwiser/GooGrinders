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
    public float fogSpeed = 1.0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * fogSpeed * Time.deltaTime);
    }

    public void ResetPositionToStart()
    {
        transform.position = startPosition;
    }
}
