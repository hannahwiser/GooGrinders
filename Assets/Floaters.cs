using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floaters : MonoBehaviour
{
    // Start is called before the first frame update
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.77f;
    public float frequency = 3f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();
    void Start()
    {
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}

