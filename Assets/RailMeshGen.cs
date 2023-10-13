using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RailMeshGen : MonoBehaviour
{
    [SerializeField]
    private SplineContainer splineContainer;
    
    private int splineIndex = 0;
    public float radius = 1;
    public float resolution = 10;
    public int pointDisplayCount = 10;
    public float pointDisplayScale = .5f;
    [SerializeField]
    private MeshFilter filter;
    public Transform displayPoint1;
    public Transform displayPoint2;
    
    private Vector3 p1;
    private Vector3 p2;
    public List<Vector3> vertsP1 = new List<Vector3>();
    private List<Vector3> vertsP2 = new List<Vector3>();
    
    private bool QueueUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!filter)
            filter = GetComponent<MeshFilter>();
        splineContainer = GetComponent<SplineContainer>();
        UpdateMesh();
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
        Vector3 point2 = pos + upvector * pointDisplayScale;
        p1 = point1;
        p2 = point2;
       
    }

    private void OnEnable() {
        filter.sharedMesh = new Mesh();
        UpdateMesh();
        Spline.Changed += CheckChange;
    }
    private void OnDisable() {
        Spline.Changed -= CheckChange;
    }
    private void CheckChange(Spline spline, int knotIndex, SplineModification modificationType)
    {
        if(splineContainer.Splines.Contains(spline))
        {
            QueueUpdate = true;
        }
    }
    


    private void OnValidate() {
        if(EditorUtility.IsDirty(this))
            QueueUpdate = true;
    }
    private void GetVerts()
    {
        vertsP1 = new List<Vector3>();
        vertsP2 = new List<Vector3>();

        float step = 1f / (float)pointDisplayCount;
        for(int i = 0; i < pointDisplayCount; i++)
        {
            float t = step * i;
            GetWidth(t, out p1, out p2);
            vertsP1.Add(p1);
            vertsP2.Add(p2);
        }
    }
    
    private void UpdateMesh()
    {
        if(!filter.sharedMesh)
            filter.sharedMesh = new Mesh();
        SplineMesh.Extrude(splineContainer.Splines,filter.sharedMesh,radius, 8, resolution,true, new float2(0,1));
        if (TryGetComponent<MeshCollider>(out var meshCollider))
            meshCollider.sharedMesh = filter.sharedMesh;
        QueueUpdate = false;
    }
    private void UpdateMesh(Spline spline, int knotIndex, SplineModification modificationType)
    {
        if(!filter.sharedMesh)
            filter.sharedMesh = new Mesh();
        SplineMesh.Extrude(splineContainer.Splines,filter.sharedMesh,radius, 8, resolution,true, new float2(0,1));
        if (TryGetComponent<MeshCollider>(out var meshCollider))
            meshCollider.sharedMesh = filter.sharedMesh;
    }
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
            if(QueueUpdate)
                UpdateMesh();
        #endif
        // Added a checker to make sure vertsP1 is not null - Sagar
        if (vertsP1 != null)
        {
            for (int i = 0; i < pointDisplayCount; i++)
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
