using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffObject : MonoBehaviour
{
    public GameObject objectToTurnOff;
    
    public void Disable()
    {
        objectToTurnOff.SetActive(false);
    }

}
