using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hannah's script
public class BlockRaycasts : MonoBehaviour
{
    public GameObject[] rayObjects;

    void Start()
    {
        rayObjects = GameObject.FindGameObjectsWithTag("Raycast Target");
        BlockRaycast();
    }

    public void BlockRaycast()
    {
        for (int i = 0; i < rayObjects.Length; i++)
        {
            rayObjects[i].GetComponent<Collider>().enabled = false;
        }
    }

    public void UnBlockRaycast()
    {
        for (int i = 0; i < rayObjects.Length; i++)
        {
            rayObjects[i].GetComponent<Collider>().enabled = true;
        }
    }
}
