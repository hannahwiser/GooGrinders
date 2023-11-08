using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GorgerDeath : MonoBehaviour
{
    public int pointsToAward;
    public ScoreHUD scoreHUDScript;
    public TextMeshPro textObject;
    float currentOpacity = 1;
    public GameObject myMesh;

    // Start is called before the first frame update
    void Start()
    {
        if (pointsToAward == 0) pointsToAward = 100;
        textObject.SetText("");
        scoreHUDScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<ScoreHUD>();
    }

    public void KillTheBug()
    {
        StartCoroutine(KillMe());
    }

    public IEnumerator KillMe()
    {
        textObject.SetText("+" + pointsToAward.ToString());
        myMesh.SetActive(false);
        this.GetComponent<SphereCollider>().enabled = false;
        scoreHUDScript.SetScore(pointsToAward);
        while (textObject.color.a >= 0)
        {
            textObject.color = (new Color(255, 255, 255, currentOpacity -= .01f));
            yield return new WaitForEndOfFrame();
        }
        yield return null;
        Destroy(this);
    }
}
