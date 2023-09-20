/**
 * File: Rotate
 * Programmer: Sagar Patel
 * Description: Simple item collection script made using this video guide: https://www.youtube.com/watch?v=YQEq6Lkd69c
 * Date: Sept 19, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    int coins = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("googorger"))
        {
            Destroy(other.gameObject);
            coins++;
            Debug.Log("Coins: " + coins);
        }
    }
}
