using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaHitUI : MonoBehaviour
{
    public Image uiHit;

    // Start is called before the first frame update
    void Start()
    {
        uiHit.alphaHitTestMinimumThreshold = 1f;
    }

}
