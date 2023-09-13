using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Timers;


public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private SplineContainer spline;
    public Vector3 playerVel = Vector3.zero;
    private float storedSign = 0;
    void Start()
    {
        //find the nearest spline object for a rail
        if(!spline)
        {
            GameObject[] m_SplineContainer = GameObject.FindGameObjectsWithTag("Rail");
            Vector3 nearest = Vector3.positiveInfinity;

            foreach (var bozo in m_SplineContainer)
                {
                    SplineContainer container = bozo.GetComponent<SplineContainer>();
                    if(container)
                    {
                        using var native = new NativeSpline(container.Spline, container.transform.localToWorldMatrix);
                        float d = SplineUtility.GetNearestPoint(native, transform.position, out float3 p, out float t);
                        if (d < nearest.magnitude)
                        {
                            nearest = p;
                            spline = container;
                        }
                    }
                }
            }
            float outTime = 0;
            float3 point = 0;
            using var gay = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
            float3 balls = SplineUtility.GetNearestPoint(gay, transform.position,out point,out outTime);
            transform.position = (Vector3)point;

            //replace this when we get proper gravity 
            storedSign = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
        float time = 0;
        float3 point = 0;
        //we can probably cache this result until the player jumps or touches a new rail
        //for now i am lazy
        using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, transform.position,out point,out time);

        Vector3 upVector = spline.EvaluateUpVector(time);
        upVector = upVector.normalized;
        Vector3 tangentVector = spline.EvaluateTangent(time);
        tangentVector = tangentVector.normalized;
        //playerVel.y += 9f * Time.deltaTime *   Mathf.Abs(Vector3.Dot(Vector3.down,tangentVector * -storedSign)) * Time.deltaTime ;
        playerVel.y -= 30 * Time.deltaTime  * Mathf.Abs(Vector3.Dot(Vector3.down,tangentVector * -storedSign)) ;
         storedSign = Mathf.Sign(Vector3.Dot(playerVel.normalized,tangentVector));
        playerVel = playerVel.magnitude * tangentVector * storedSign;
       

        transform.position = (Vector3)point + playerVel * Time.deltaTime;
        Debug.DrawRay(transform.position,playerVel,Color.red);
        Debug.DrawRay(transform.position,tangentVector,Color.green);



    }
}
