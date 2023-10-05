using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
using System.Collections.Generic;

[ExecuteInEditMode()]
public class RailMeshGen : MonoBehaviour
{
    [SerializeField]
    private SplineContainer splineContainer;

    private int splineIndex = 0;
    private float time = 0;
    public float width = 1;
    public int resolution = 10;

    public Transform displayPoint1;
    public Transform displayPoint2;
    private Vector3 p1;
    private Vector3 p2;
    public List<Vector3> vertsP1 = new List<Vector3>();
    private List<Vector3> vertsP2 = new List<Vector3>();
    
    // Start is called before the first frame update
    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        //splineIndex = 
    }

    // Update is called once per frame
    void Update()
    {
        GetVerts();
        //displayPoint1.position = point1;      
        //displayPoint2.position = point2;
    }
    private void GetWidth(float t, out Vector3 p1, out Vector3 p2)
    {
        splineContainer.Evaluate(splineIndex,t,out float3 pos, out float3 tangent,out float3 upvector);
        float3 right = Vector3.Cross(tangent,upvector).normalized;
        Vector3 point1 = pos;
        Vector3 point2 = pos + upvector * width;
        p1 = point1;
        p2 = point2;
       
    }
    private void GetVerts()
    {
        vertsP1 = new List<Vector3>();
        vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        for(int i = 0; i < resolution; i++)
        {
            float t = step * i;
            GetWidth(t, out p1, out p2);
            vertsP1.Add(p1);
            vertsP2.Add(p2);
        }
    }

    private void OnDrawGizmos()
    {
        // Added a checker to make sure vertsP1 is not null - Sagar
        if (vertsP1 != null)
        {
            for (int i = 0; i < vertsP1.Count; i++)
            {
                Gizmos.color = Color.white;
                if (i < vertsP1.Count)
                    Gizmos.DrawSphere(vertsP1[i], 0.1f);

                Gizmos.color = Color.red;
                if (i < vertsP2.Count)
                    Gizmos.DrawSphere(vertsP2[i], 0.05f);

                if (i < vertsP1.Count && i < vertsP2.Count)
                    Debug.DrawRay(vertsP2[i], vertsP1[i] - vertsP2[i], Color.white);
            }
        }
        else
        {
            Debug.LogWarning("vertsP1 is null in RailMeshGen. Make sure it's properly initialized.");
        }
    }

}
