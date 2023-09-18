using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Timers;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    // stored objects
    private SplineContainer spline;
    private Collider splineCollider;
    public Transform cameraObj;

    //deprecated
    public Vector3 playerVel = Vector3.zero;
    private float storedSign = 0;
    

    //Inputs
    private Vector2 inputVector = Vector2.zero;
    private bool jumpInput = false;
    //rigid body
    public ConfigurableJoint joint;
    public Rigidbody rb;
    
    public bool OnRail = true;
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
                        if (Vector3.Distance((Vector3)p,transform.position) < Vector3.Distance(nearest,transform.position) || nearest.magnitude == Mathf.Infinity)
                        {
                            spline = container;
                            Debug.Log(Vector3.Distance(nearest,transform.position));
                            nearest = p;
                            Debug.Log( Vector3.Distance(nearest,transform.position));
                           
                            
                        }
                    }
                }
            }
        splineCollider = spline.GetComponent<Collider>();
        splineCollider.enabled = false;
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
        jumpInput = Input.GetButton("Jump");
    }

    private void DisableJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Free;
		joint.zMotion = ConfigurableJointMotion.Free;
		joint.angularXMotion = ConfigurableJointMotion.Free;
		joint.angularYMotion = ConfigurableJointMotion.Free;
		joint.angularZMotion = ConfigurableJointMotion.Free;
    }
    private void PartiallyDisableJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Free;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
    }
    private void EnableJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
    }
    // Update is called once per frame
    Vector3 lastNearestPoint =Vector3.zero;
    void FixedUpdate()
    {
        Vector3 oldPos = transform.position;
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
        HandleInput();
        
        //get player input relative to the camera
        Vector3 VelocityChange = cameraObj.forward * inputVector.y + cameraObj.right * inputVector.x;
        //prevent the player from being able to affect the y velocity
        VelocityChange.z = VelocityChange.y;
        VelocityChange.y = 0;
        lastNearestPoint = (Vector3) point;
        if(jumpInput && OnRail)
        {
            PartiallyDisableJoint();
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            transform.position += Vector3.up * .5f;
            OnRail = false;
        }
        //this is both axeese added together essentially. i think
        //actually idk if this is even used anymore lmao
        Vector3 funkyAxis = Vector3.Cross(tangentVector,upVector);
        
        //the force for playermovement
        rb.AddForce(VelocityChange * 10,ForceMode.Acceleration);
        
        //move the anchors of the join to allow the rail physics to work correctly
        //should be somewhat disabled when jumping
        joint.connectedAnchor = (Vector3)point;//spline.transform.TransformPoint((Vector3)point);
        if(!OnRail)
        {
            splineCollider.enabled = true;
            RaycastHit hit;
            PartiallyDisableJoint();
            //splines should be given their own layer!!!
           
            //get the nearest postiion to the hit, this is a lot more precise than getting the closest
            //position directly relative to the player
            if(Physics.SphereCast(transform.position, 1,Vector3.down,out hit))
            {
                SplineUtility.GetNearestPoint(native, hit.point,out point,out time);
                Vector3 fromPoint = (Vector3)point;
                lastNearestPoint = (Vector3)point;
                var newAnchor = new Vector3(point.x,transform.position.y,point.z);
                joint.connectedAnchor = spline.transform.TransformPoint(newAnchor);
                //recalculate the vectors
                upVector = spline.EvaluateUpVector(time);
                upVector = upVector.normalized;
                tangentVector = spline.EvaluateTangent(time);
                tangentVector = tangentVector.normalized;
            }
            else
            {
                Debug.Log("Misalined from rail? thats fucked up.");
                var newAnchor = new Vector3(lastNearestPoint.x,transform.position.y,lastNearestPoint.z);
                joint.connectedAnchor = spline.transform.TransformPoint(newAnchor);
                //this is to make sure you don't go past the rail through fucky bullshit that happens
                if(transform.position.y < lastNearestPoint.y)
                {
                    OnRail = true;

                }
                //DisableJoint();
            }
        }
        else
        {
            //renable the join when you ARE on a rail
            EnableJoint();
        }
        joint.axis = tangentVector;//spline.transform.TransformPoint(tangentVector);
        
        axi = joint.axis;
        anch = joint.connectedAnchor;
        //transform.rotation = Quaternion.LookRotation(tangentVector);
        Debug.DrawRay(transform.position,playerVel,Color.red);
        Debug.DrawRay(transform.position,tangentVector,Color.green);

    }
    private Vector3 anch = Vector3.zero;
    private Vector3 axi = Vector3.zero;
    private void OnDrawGizmos() {
        Gizmos.DrawSphere(anch,.5f);
        Debug.DrawRay(anch,axi,Color.blue);
    }

    private float textPos = 0;
    private void DrawText(string text)
    {
        GUI.Box(new Rect(0,textPos,150,20), text);
        textPos += 20;
    }
    private void OnGUI()
    {
        textPos = 0;
        DrawText(OnRail?"ONRAIL":"CRINGEFAIL");
        DrawText(anch.ToString());
        DrawText("Rail: " + spline.name);
    }

    private void OnCollisionEnter(Collision other) {
        //reattatch the player to the rail
        //also set the spline to the new spline object
        if(other.collider.tag == "Rail")
        {
            spline = other.gameObject.GetComponent<SplineContainer>();
            if(spline)
            {
                //renable the previous collider!!!
                splineCollider.enabled = true;
                splineCollider = spline.GetComponent<Collider>();
                splineCollider.enabled = false;
                EnableJoint();
                OnRail = true;
            }
            else
            {
                Debug.LogError("WARNING! Object tagged 'Rail' needs to have a SplineContainer! If you see this error, either add a spline to the object OR contact Mars and ask if they can add a feature!");
            }
        }
    }
}

