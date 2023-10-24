using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGlow : MonoBehaviour
{
    public Light interiorLight;
    float currentTarget;
    float lowTarget = 1;
    float highTarget = 10;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = lowTarget;
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            Debug.Log(interiorLight.intensity);
            if (currentTarget == highTarget)
            {
                interiorLight.intensity+= .1f;
                if (interiorLight.intensity >= highTarget) currentTarget = lowTarget;
            }
            else if (currentTarget == lowTarget)
            {
                interiorLight.intensity-= .1f;
                if (interiorLight.intensity <= lowTarget) currentTarget = highTarget;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
