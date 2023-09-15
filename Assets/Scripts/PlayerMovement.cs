using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(horizontalInput * -5f, rb.velocity.y, verticalInput * -5f);

        if (Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector3(rb.velocity.x, 3f, rb.velocity.z);
        }
    }
}
