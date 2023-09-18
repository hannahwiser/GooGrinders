using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Timers;
using Unity.Burst.Intrinsics;
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private SplineContainer spline;
    public Vector3 playerVel = Vector3.zero;
    private float storedSign = 0;
    public Transform cameraObj;
    private Vector2 inputVector = Vector2.zero;
    public ConfigurableJoint joint;
    public Rigidbody rb;
    void Start()
    {
        if(!rb)
            rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 10,ForceMode.Impulse);
        //get the camera object pls :)
        cameraObj = Camera.main.transform;
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

    
    private void HandleInput()
    {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        inputVector = inputVector.normalized;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 oldPos = transform.position;
        float time = 0;
        float3 point = 0;
        //transform.position += playerVel * Time.deltaTime;
        
        //we can probably cache this result until the player jumps or touches a new rail
        //for now i am lazy
        using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, transform.position,out point,out time);

        Vector3 upVector = spline.EvaluateUpVector(time);
        upVector = upVector.normalized;
        Vector3 tangentVector = spline.EvaluateTangent(time);
        tangentVector = tangentVector.normalized;
        HandleInput();
        
        //do this to properly flatten it a bit
        Vector3 VelocityChange = cameraObj.forward * inputVector.y + cameraObj.right * inputVector.x;
        VelocityChange.z = VelocityChange.y;
        VelocityChange.y = 0;
        
        //transform.rotation = Quaternion.LookRotation(Vector3.Cross(tangentVector,upVector),upVector);

        Vector3 funkyAxis = Vector3.Cross(tangentVector,upVector);
        //playerVel.x = 30;
        
        playerVel.y -= 30 * Time.deltaTime;// * Mathf.Abs(Vector3.Dot(Vector3.down,tangentVector)) ;
        playerVel -= Vector3.Project(playerVel,funkyAxis);
        //playerVel = tangentVector * playerVel.magnitude;
        //playerVel = tangentVector * playerVel.magnitude * Mathf.Abs(Vector3.Dot(playerVel,tangentVector));
        //playerVel = playerVel * Vector3.Dot(playerVel,transform.forward);
        
        //playerVel = playerVel.magnitude * tangentVector * Mathf.Abs(Vector3.Dot(playerVel,tangentVector));//Vector3.ProjectOnPlane(playerVel,tangentVector);
        //playerVel -= Vector3.Project(playerVel,funkyAxis);

        
        //playerVel = (transform.position - oldPos) / Time.deltaTime;
        //rb.transform.position = (Vector3)point;
        rb.AddForce(VelocityChange * 10,ForceMode.Acceleration);
        joint.connectedAnchor = spline.transform.TransformPoint((Vector3)point);
        joint.axis = spline.transform.TransformPoint(tangentVector);
        
        axi = joint.axis;
        anch = joint.connectedAnchor;
        
        Debug.DrawRay(transform.position,playerVel,Color.red);
        Debug.DrawRay(transform.position,tangentVector,Color.green);

    }
    private Vector3 anch = Vector3.zero;
    private Vector3 axi = Vector3.zero;
    private void OnDrawGizmos() {
        Gizmos.DrawSphere(anch,.5f);
        Debug.DrawRay(anch,axi,Color.blue);
    }
}
