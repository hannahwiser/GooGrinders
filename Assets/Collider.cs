using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.tag == "Fly")
        {
            print("Enter");
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
