using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) {
            GetComponent<Rigidbody>().velocity = new Vector3(0,3,0);
        }

        if (Input.GetKey("up")) {
            GetComponent<Rigidbody>().velocity = new Vector3(0,0,-3);
        }

        if (Input.GetKey("down"))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 3);
        }

        if (Input.GetKey("right"))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(-3, 0, 0);
        }

        if (Input.GetKey("left"))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(3, 0, 0);
        }
    }
}
