using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentVial : MonoBehaviour
{
    public GameObject nameText;
    public GameObject thisModel;

    public float lerpDuration;

    public Vector3 myStartPos;
    public Quaternion myStartRot;
    public Transform endPoint;

    public bool isRaised;

    public MainMenuBehavior mainScript;

    public float timeElapsed;

    public Coroutine spinCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        nameText.SetActive(false);
        myStartPos = gameObject.transform.position;
        myStartRot = gameObject.transform.rotation;
        endPoint = GameObject.Find("Vial End Point").GetComponent<Transform>();
        mainScript = GameObject.Find("flask export v2").GetComponent<MainMenuBehavior>();
    }

    private void OnMouseDown()
    {
        if (mainScript.CheckIfNull() != this.gameObject && mainScript.CheckIfNull() != null) //if there is another vial already up
        {
            mainScript.currentVial.GetComponent<PresentVial>().ForceDown();
        }

        if (!isRaised) //if its not already presented
        {
            mainScript.SetCurrentVial(this.gameObject);
            isRaised = true;
            nameText.SetActive(true);
            StartCoroutine(Lerp(myStartPos, endPoint.position, myStartRot, endPoint.rotation));
            spinCoroutine = StartCoroutine(Spin());
        }
        else if (isRaised)
        {
            ForceDown();
        }
    }

    public void ForceDown()
    {
        StopCoroutine(spinCoroutine);
        isRaised = false;
        mainScript.NullifyVial();
        nameText.SetActive(false);
        StartCoroutine(Lerp(endPoint.position, myStartPos, endPoint.rotation, myStartRot));
    }

    public IEnumerator Spin()
    {
        while (true)
        {
            Debug.Log("Spinnin");
            thisModel.transform.Rotate(new Vector3(0,1,0), 2f * Time.deltaTime / 1f);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Lerp(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / lerpDuration);
            gameObject.transform.rotation = Quaternion.Lerp(startRot, endRot, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.position = endPos;
        gameObject.transform.rotation = endRot;
    }
}
