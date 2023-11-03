using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionTest : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Transform point4;

    // Update is called once per frame
    void Update()
    {
        Vector3 dir1 = point2.position - point1.position;
        Vector3 dir2 = point4.position - point3.position;
        Debug.DrawRay(point1.position,dir1,Color.black);
        Debug.DrawRay(point3.position,dir2,Color.blue);
        Debug.DrawRay(transform.position,Vector3.Project(dir1,dir2),Color.green);
        Debug.DrawRay(transform.position,Vector3.ProjectOnPlane(dir1,dir2),Color.red);
    }
}
