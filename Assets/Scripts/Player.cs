using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
public class Player : MonoBehaviour
{
    //Speed limit
    public float horizontalSpeedLimit = 30.0f;

    // stored objects
    private SplineContainer spline;
    private Collider splineCollider;
    public Transform cameraObj;
    public Transform model;
    public ParticleSystem tempFlingParticle;
    //deprecated
    public Vector3 playerVel = Vector3.zero;
    private float storedSign = 0;
    

    //Inputs
    private Vector2 inputVector = Vector2.zero;
    private bool jumpInput = false;
    private bool gooflingInput = false;
    //rigid body
    public ConfigurableJoint joint;
    public Rigidbody rb;
    
    //this will be used whenever the player is in the air
    private GameObject fakeObject;
    private Rigidbody fakeRB;
    private ConfigurableJoint fakeJoint;
    [SerializeField]
    private GameObject fakePrefab;
    
    private float gooflingCharge = 0;
    //time before the goofling starts to decay
    private float gooflingResetTimer = 0;
    public float gooflingMultiplier = 5;

    public bool OnRail = true;
    public bool BelowRail = false;
    //HOPEFULLY prevent reattatching to the rail
    private float jumpRegroundCooldown = .2f;

    //audio assets
    public AudioSource jump;
    public AudioSource land;

    //
    public Transform spawnPoint;
    public GameObject sporetParent;

    void Start()
    {
        if(!rb)
            rb = GetComponent<Rigidbody>();
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
        jumpInput = Input.GetButtonDown("Jump");
        gooflingInput = Input.GetButtonDown("Fire1"); // Changed the button for the goo-fling to the "Shift" key
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
    private bool toggledBelowRail = false;
    private void Update() {
        HandleInput();
        //polish this pls :)
        Debug.DrawRay(transform.position,jumpUpVector * 3,Color.blue);
        if(jumpInput && OnRail && !BelowRail)
        {
            jump.Play();
            splineCollider.enabled = false;
            HandleJump(jumpUpVector,2);
            //HandleJump(Vector3.up,1);
        }
        if(gooflingInput && OnRail && !BelowRail && gooflingCharge >0)
        {
            //jump.Play();
            splineCollider.enabled = false;
            HandleJump(jumpUpVector,Mathf.Clamp(gooflingMultiplier * gooflingCharge,2,5));
            //HandleJump(Vector3.up,1);
            tempFlingParticle.Play();
        }
    }
    private Vector3 jumpUpVector = Vector3.up;
    void FixedUpdate()
    {
        //I intended to split it into multiple functions
        //however, its a bit too complex rn, so for the sake of cleaning it up its all one function 
        //HandleInput();
        toggledBelowRail = false;
        if(inputVector.y < 0)
        {
            if(!BelowRail)
            toggledBelowRail = true;
            BelowRail = true;
        }
        if(inputVector.y > 0)
        {
            BelowRail = false;
        }


        MainState();

        // Limit horizontal movement speed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > horizontalSpeedLimit)
        {
            Vector3 clampedVelocity = horizontalVelocity.normalized * horizontalSpeedLimit;
            rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
        }
    }

    private float debugTime = 0;
    private void MainState()
    {
        //we can probably cache this result until the player jumps or touches a new rail
        //for now i am lazy
        
        using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, transform.position,out float3 point,out float time);

        Vector3 upVector = spline.EvaluateUpVector(time);
        upVector = upVector.normalized;
        Vector3 tangentVector = spline.EvaluateTangent(time);
        tangentVector = tangentVector.normalized;
        //time = Mathf.Clamp(time,0,1);
       //HandleInput();
        
        //get player input relative to the camera
        Vector3 VelocityChange = cameraObj.right * inputVector.x;
        //prevent the player from being able to affect the y velocity
        VelocityChange.z = VelocityChange.y;
        VelocityChange.y = 0;
        lastNearestPoint = (Vector3) point;
        jumpUpVector = upVector;
        
        if(!OnRail)
        {
            if(jumpRegroundCooldown >0)
                jumpRegroundCooldown -= Time.fixedDeltaTime;
            gooflingCharge = 0;
            gooflingResetTimer = 0;
            //transform.rotation = Quaternion.LookRotation(rb.velocity,Vector3.up);
            //reset the model's offset when in the air
            model.transform.localPosition = Vector3.zero;
            splineCollider.enabled = true;
            BelowRail = false;
            PartiallyDisableJoint();
            //create the fake rigidbody object
            if(!fakeObject)
            {
                fakeObject = Instantiate(fakePrefab,transform.position,transform.rotation);
                fakeObject.transform.parent = null;
                fakeRB = fakeObject.AddComponent<Rigidbody>();
                fakeJoint = fakeObject.AddComponent<ConfigurableJoint>();
                fakeJoint.xMotion = ConfigurableJointMotion.Free;
                fakeJoint.yMotion = ConfigurableJointMotion.Locked;
                fakeJoint.zMotion = ConfigurableJointMotion.Locked;
                fakeJoint.angularXMotion = ConfigurableJointMotion.Locked;
                fakeJoint.angularYMotion = ConfigurableJointMotion.Locked;
                fakeJoint.angularZMotion = ConfigurableJointMotion.Locked;
                fakeJoint.autoConfigureConnectedAnchor = false;
            }

            //keep the velocity of the fake rigidbody the same, always
            fakeRB.velocity = rb.velocity;
            fakeRB.MovePosition(new Vector3(transform.position.x,fakeObject.transform.position.y,transform.position.z));
            SplineUtility.GetNearestPoint(native, fakeObject.transform.position,out point,out time);
            //time = Mathf.Clamp(time,0,1);
            upVector = spline.EvaluateUpVector(time);
            upVector = upVector.normalized;
            tangentVector = spline.EvaluateTangent(time);
            tangentVector = tangentVector.normalized;
            

            fakeJoint.connectedAnchor = (Vector3)point;
            fakeJoint.axis = tangentVector;

            joint.connectedAnchor = transform.position;
            joint.axis = tangentVector;
            
            
            //FindNewRailCast();
        
        }
        else
        {
            jumpRegroundCooldown = .15f;
            if (fakeObject)
            {
                //land.Play();
                DestroyImmediate(fakeObject);
            }
            //renable the join when you ARE on a rail
            EnableJoint();
            //move the model's position based on if we are hanging on the rail or not
            model.transform.localPosition = BelowRail?-upVector * .5f:upVector * .5f;

            joint.connectedAnchor = (Vector3)point;
            joint.axis = tangentVector;
           

            var prevCharge = gooflingCharge;

            //build charge when below the rail
            if(BelowRail){
                gooflingCharge += Time.fixedDeltaTime ;
                gooflingResetTimer = 2;
                }

            if(gooflingResetTimer > 0)
                gooflingResetTimer -= Time.fixedDeltaTime;



            if(gooflingResetTimer <= 0 && gooflingCharge >= 0)
            {
                gooflingCharge -= Time.fixedDeltaTime * .9f;
            }
            gooflingCharge = Mathf.Clamp(gooflingCharge,0,1);
        }
        
        debugTime = time;
        if(time < 0 || time > 1)
        {
            FindNewRailCast();
            DisableJoint();
            
            //disconnectng from the rail should be its own function
            if(OnRail)
            {
                transform.position += model.localPosition;
                model.localPosition = Vector3.zero;
                
                OnRail = false;
            }
            jumpRegroundCooldown = .1f;
        }
        //the force for playermovement
        rb.AddForce(VelocityChange * 10,ForceMode.Force);
    }

    private void FindNewRailCast()
    {
        if(Physics.SphereCast(transform.position,.5f,Vector3.down,out RaycastHit hit))
        {
            if(hit.collider.tag == "Rail" && hit.collider != splineCollider)
            {
                spline = hit.collider.GetComponent<SplineContainer>();
                splineCollider.enabled = true;
                splineCollider = hit.collider;
                Debug.Log("found a new rail, baby!");
            }
        }
    }
    private int ticksWithoutRail = 0;
    private void LateUpdate() {
        if(fakeObject && !OnRail)
            transform.position = new Vector3(fakeObject.transform.position.x,transform.position.y,fakeObject.transform.position.z);
        if(fakeObject)
        if(Vector3.Distance(fakeJoint.connectedAnchor, fakeObject.transform.position) > 3)
        {
            var lastSpline = spline;
            FindNewRailCast();
            if(spline == lastSpline)
                ticksWithoutRail++;
            else
                ticksWithoutRail = 0;
        }
        else if(Vector3.Distance(joint.connectedAnchor,transform.position) > 3)
        {
            var lastSpline = spline;
            FindNewRailCast();
            if(spline == lastSpline)
                ticksWithoutRail++;
            else
                ticksWithoutRail = 0;
        }
        else
            ticksWithoutRail = 0;
           
        //fully disable the joint when disconnected for 3 ticks
        if(ticksWithoutRail >3)
            DisableJoint();
    }

    private void OnDrawGizmos()
    {
        if(fakeObject)
            Gizmos.DrawSphere(fakeObject.transform.position,.5f);
    }

    void HandleJump(Vector3 direction, float force)
    {
        PartiallyDisableJoint();
        rb.AddForce(direction.normalized * (force), ForceMode.Impulse);
        transform.position += direction.normalized * .5f;
        model.transform.localPosition = Vector3.zero;
        OnRail = false;
        
    }
    private float textPos = 0;
    private void DrawText(string text)
    {
        //GUI.Box(new Rect(0,textPos,150,20), text);
        textPos += 20;
    }
    private void OnGUI()
    {
        textPos = 0;
        DrawText(OnRail?"ONRAIL":"CRINGEFAIL");
        DrawText("Rail: " + spline.name);
        DrawText(BelowRail.ToString());
        DrawText("Goofling Charge: " + gooflingCharge);
        DrawText("Goofling Charge Reset: " + gooflingResetTimer);
        DrawText("JumpCooldown: " + jumpRegroundCooldown);
        DrawText("Inputs: " + inputVector);
        DrawText("TicksWithoutRail: " + ticksWithoutRail);
        DrawText("Time: " + debugTime);
    }


    private void CheckCollider(Collision other)
    {
        //reattatch the player to the rail
        //also set the spline to the new spline object
        if(jumpRegroundCooldown <= 0)
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
    void OnCollisionEnter(Collision other) {
        CheckCollider(other);
        if (other.gameObject.tag == "Death")
        {
            sporetParent.transform.position = spawnPoint.position;
        }
    }
    private void OnCollisionStay(Collision other) {
        CheckCollider(other);
    }
}

