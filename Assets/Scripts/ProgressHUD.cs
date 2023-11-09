using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressHUD : MonoBehaviour
{
    //start x = -1243.897 finish = 1350 total = 2593

    public double current = 0;
    public GameObject player;
    public float currentPercentage;
    public Slider progressSlider;

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        currentPercentage = (player.transform.position.x + 1807) / 4952;
        progressSlider.value = currentPercentage;
    }


}
