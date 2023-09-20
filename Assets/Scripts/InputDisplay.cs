using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDisplay : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    public Transform cameraObj;
    private Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        cameraObj = Camera.main.transform;
        parent = transform.parent;
        transform.parent = null;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        transform.position = parent.position;
        
        Vector3 VelocityChange = cameraObj.forward * inputVector.y + cameraObj.right * inputVector.x;
        VelocityChange.z = VelocityChange.y;
        VelocityChange.y = 0;
        left.SetActive(false);
        right.SetActive(false);
        if(inputVector.x < 0)
        {
            left.SetActive(true);
        }
        if(inputVector.x > 0)
        {
            right.SetActive(true);
        }
    }
}
