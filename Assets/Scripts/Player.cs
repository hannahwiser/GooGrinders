using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;
using Unity.Mathematics;
using System;
using System.Collections;
using Unity.Burst.Intrinsics;

public class Player : MonoBehaviour
{
    //Speed limit
    public float horizontalSpeedLimit = 30.0f;

    // Toggle GUI Box
    [SerializeField]
    private bool enableDebuggingBox = false;

    // Toggle player control (true by default)
    private bool isPlayerControlEnabled = true;

    // stored objects
    public SplineContainer spline;
    private Collider splineCollider;
    public Transform cameraObj;
    public Transform model;
    private SphereCollider playerCollider;
    public ParticleSystem tempFlingParticle;

    //deprecated
    public Vector3 playerVel = Vector3.zero;

    //Inputs
    public Vector2 inputVector = Vector2.zero;
    public bool jumpInput = false;
    public bool jumpInputRelease = false;
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
    public float gooflingMultiplier = 6;

    public bool OnRail = true;
    public bool BelowRail = false;
    public bool startAttached = false;

    //HOPEFULLY prevent reattaching to the rail
    private float jumpRegroundCooldown = .2f;
    public bool ForceEnablePlayerControl = false;

    //audio assets
    public AudioSource jump;
    public AudioSource land;
    public AudioSource charge;

    //
    public Transform spawnPoint;
    public GameObject sporetParent;

    //private NativeSpline native;
    private Vector3 storedVelocity = Vector3.zero;
    public ScoreHUD GUIScript;
    [SerializeField]
    private int railDistanceCastPoints = 15;

    // Track the total time grinded for Erriks leaderboard stuff
    public float totalTimeOnRail = 0f;

    // Jakes' rail particle emitter
    public ParticleSystem railParticles;
    public float minimumSpeedLimit = 0f;
    void Start()
    {
        if (!playerCollider)
            playerCollider = GetComponent<SphereCollider>();
        if (!rb)
            rb = GetComponent<Rigidbody>();
        //get the camera object pls :)
        cameraObj = Camera.main.transform;
        if (startAttached)
            AttachToRail();
        else
        {
            FindInitialSpline();
            OnRail = false;
        }
    }

    private void FindInitialSpline()
    {
        if (!spline)
        {
            GameObject[] m_SplineContainer = GameObject.FindGameObjectsWithTag("Rail");
            Vector3 nearest = Vector3.positiveInfinity;

            foreach (var bozo in m_SplineContainer)
            {
                SplineContainer container = bozo.GetComponent<SplineContainer>();
                if (container)
                {
                    using var native = new NativeSpline(
                        container.Spline,
                        container.transform.localToWorldMatrix
                    );
                    float d = SplineUtility.GetNearestPoint(
                        native,
                        transform.position,
                        out float3 p,
                        out float t
                    );
                    if (
                        Vector3.Distance((Vector3)p, transform.position)
                            < Vector3.Distance(nearest, transform.position)
                        || nearest.magnitude == Mathf.Infinity
                    )
                    {
                        spline = container;
                        nearest = p;
                    }
                }
            }
        }
        splineCollider = spline.GetComponent<Collider>();
        splineCollider.enabled = false;
    }

    private void AttachToRail()
    {
        FindInitialSpline();
        splineCollider = spline.GetComponent<Collider>();
        splineCollider.enabled = false;
        float outTime = 0;
        float3 point = 0;
        using var gay = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        float3 balls = SplineUtility.GetNearestPoint(
            gay,
            transform.position,
            out point,
            out outTime
        );
        //model.localPosition =
        Vector3 upVector = spline.EvaluateUpVector(outTime);
        Vector3 tangent = spline.EvaluateTangent(outTime);
        playerCollider.center = BelowRail ? -upVector * .5f : upVector * .5f;
        model.localPosition = BelowRail ? -upVector * .5f : upVector * .5f;

        float _dot = Vector3.Dot(storedVelocity.normalized, tangent.normalized);
        float _angle = Vector3.Angle(Vector3.right, tangent.normalized);
        float _signedAngle = Vector3.SignedAngle(
            Vector3.right,
            tangent.normalized,
            Vector3.forward
        );

        Vector3 flatVelocity = new Vector3(storedVelocity.x, 0, storedVelocity.z);
        //this is just. taken from the sonic physics guide because i liked the way it worked
        //it feels good overall
        switch (_angle)
        {
            case < 23f:
                rb.velocity = flatVelocity.magnitude * tangent.normalized * Mathf.Sign(_dot);
               
                break;
            //disabled for the time being. everything still works this code specifically just had lots of inconsistencies
            /*case < 45f:
                
                rb.velocity = (tangent.normalized * (storedVelocity.y * .5f * -Mathf.Sign(MathF.Sin(_signedAngle + 180))) * Mathf.Sign(_dot))
                + flatVelocity * Mathf.Abs(_dot);
                
                Debug.Log("medium angle: " + _signedAngle);
            break;*/
            case < 360f:
                rb.velocity = storedVelocity.magnitude * tangent.normalized * Mathf.Sign(_dot);
                break;
        }

        //rb.velocity = storedVelocity.magnitude * tangent.normalized * Mathf.Sign(_dot);

        //Debug.Log("landing dot product: " + _dot);
        //Debug.Log("landing angle: " + _angle);
        Debug.DrawRay(transform.position, storedVelocity.normalized, Color.red, 5f);
        Debug.DrawRay(transform.position, tangent.normalized, Color.blue, 5f);

        transform.position = (Vector3)point;
    }

    private void HandleInput()
    {
        // If isPlayerControlEnabled is set to false, we just exit the method. This is so we can enable/disable movement for ragdoll mode
        if (!isPlayerControlEnabled && !ForceEnablePlayerControl)
        {
            return;
        }

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        inputVector = inputVector.normalized;
        jumpInput = Input.GetButton("Jump");
        jumpInputRelease = Input.GetButtonUp("Jump");
        gooflingInput = Input.GetButtonDown("Fire1"); // Changed the button for the goo-fling to the "Shift" key
    }

    // Public method to enable or disable player control
    public void SetPlayerControlEnabled(bool enabled)
    {
        isPlayerControlEnabled = enabled;
    }

    // Public method to set OnRail status (for PlayerLife so that we don't spawn detatched)
    //public void SetPlayerOnRail(bool value)
    //{
    //    OnRail = value;
    //}

    //public void SetPlayerStartAttached(bool value)
    //{
    //    startAttached = value;
    //}

    public void LockRigidbody()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    public void UnlockRigidbody()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    public void DisableJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
    }

    public void PartiallyDisableJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    public void EnableJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    // Update is called once per frame
    Vector3 lastNearestPoint = Vector3.zero;

    private void Update()
    {
        if (spline)
            transform.parent = spline.transform;
        HandleInput();
        //polish this pls :)
        Debug.DrawRay(transform.position, jumpUpVector * 3, Color.blue);
        if (jumpInput && OnRail)
        {
            //jump.time = .1f;
            //jump.PlayOneShot(jump.clip);
            //splineCollider.enabled = false;
            //HandleJump(jumpUpVector,5);
            //HandleJump(Vector3.up,1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ChargingJump());
        }

        if (jumpInputRelease && OnRail) // gooflingInput && !BelowRail && gooflingCharge >0)
        {
            if(GUIScript){
                GUIScript.FinishCharge();
                charge.Stop();
                jump.time = .1f;
                jump.Play();
            }
            splineCollider.enabled = false;

            //HandleJump(jumpUpVector, Mathf.Clamp(gooflingMultiplier * gooflingCharge, 4, 8));
            HandleJump(
                Vector3.up.normalized,
                Mathf.Lerp(1,gooflingMultiplier,gooflingCharge ) * 3 
                //Mathf.Clamp(gooflingMultiplier * gooflingCharge, 4, 12)
            );
            //HandleJump(Vector3.up,1);
            tempFlingParticle.Play();
        }
    }

    IEnumerator ChargingJump()
    {
        yield return new WaitForSeconds(.3f);
        if (Input.GetKey(KeyCode.Space))
        {
            if(GUIScript)
            GUIScript.ChargeJump();
            charge.Play();
        }
    }

    public Vector3 jumpUpVector = Vector3.up;

    void FixedUpdate()
    {
        storedVelocity = rb.velocity;
        //I intended to split it into multiple functions
        //however, its a bit too complex rn, so for the sake of cleaning it up its all one function
        //HandleInput();
        if (OnRail)
        {
            // Track the total time for the stats screen
            totalTimeOnRail += Time.fixedDeltaTime;
            if (inputVector.y < 0)
            {
                if (
                    !Physics.SphereCast(
                        transform.position + jumpUpVector,
                        .3f,
                        Vector3.zero,
                        out RaycastHit hit
                    )
                )
                    BelowRail = true;
            }
            if (inputVector.y > 0)
            {
                if (
                    !Physics.SphereCast(
                        transform.position - jumpUpVector,
                        .3f,
                        Vector3.zero,
                        out RaycastHit hit
                    )
                )
                    BelowRail = false;
            }
        }

        MainState();

        // Limit horizontal movement speed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > horizontalSpeedLimit)
        {
            Vector3 clampedVelocity = horizontalVelocity.normalized * horizontalSpeedLimit;
            rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
        }

        ToggleRailParticles(true);
    }

    private float debugTime = 0;

    private void MainState()
    {
        //we can probably cache this result until the player jumps or touches a new rail
        //for now i am lazy
        
        using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, transform.position, out float3 point, out float time);

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
        lastNearestPoint = (Vector3)point;
        jumpUpVector = upVector;
        Debug.DrawRay(transform.position, rb.velocity, Color.red);

        if (!OnRail)
        {
            //player collider is offs
            playerCollider.center = model.localPosition; //Vector3.down;
            if (jumpRegroundCooldown > 0)
                jumpRegroundCooldown -= Time.fixedDeltaTime;

            //transform.rotation = Quaternion.LookRotation(rb.velocity,Vector3.up);
            //reset the model's offset when in the air
            model.transform.localPosition = Vector3.zero;

            splineCollider.enabled = true;
            BelowRail = false;
            
            //create the fake rigidbody object
            CreateFakeObject(time);

            //keep the velocity of the fake rigidbody the same, always
            fakeRB.velocity = rb.velocity;
            //fakeRB.MovePosition(new Vector3(transform.position.x,fakeObject.transform.position.y,transform.position.z));
            SplineUtility.GetNearestPoint(
                native,
                fakeObject.transform.position,
                out point,
                out time
            );
            //time = Mathf.Clamp(time,0,1);
           

            PartiallyDisableJoint();
            

            
            
            //so. a lot of this can be removed if the new alignment doesnt work
            float distance = Mathf.Infinity;
            float nearestTime = time;
            Vector3 closestPoint = transform.position;
            
            if(!FindNewRailCast())
                for(int i = 0; i < railDistanceCastPoints; i++)
                {
                    Vector3 pointCheck = new Vector3(transform.position.x,transform.position.y + i, transform.position.z);
                    SplineUtility.GetNearestPoint(
                    native,
                    pointCheck,
                    out point,
                    out time
                    );
                    //update values to closest position
                    if(Vector3.Distance(point,pointCheck) < distance)
                    {
                        distance = Vector3.Distance(point,pointCheck);
                        closestPoint = point;
                        nearestTime = time;
                    }
                }
            
            //these lines need to be kept, but replace the nearest time with time
            //of you remove the above section
            upVector = spline.EvaluateUpVector(nearestTime);
            upVector = upVector.normalized;
            tangentVector = spline.EvaluateTangent(nearestTime);
            tangentVector = tangentVector.normalized;
            joint.connectedAnchor = transform.position;
            joint.axis = tangentVector;
            fakeJoint.connectedAnchor = closestPoint;
            fakeJoint.axis = tangentVector;
            //FindNewRailCast();
        }
        else
        {
            jumpRegroundCooldown = .05f;
            if (fakeObject)
            {
                land.time = .15f;
                land.Play();
                DestroyImmediate(fakeObject);
            }
            //renable the join when you ARE on a rail
            EnableJoint();
            //move the model's position based on if we are hanging on the rail or not
            model.transform.localPosition = upVector * .5f;
            //this should be tweaked to allow turning around
            model.localRotation = Quaternion.LookRotation(tangentVector, upVector);
            playerCollider.center = BelowRail ? -upVector * .5f : upVector * .5f;

            joint.connectedAnchor = (Vector3)point;
            joint.axis = tangentVector;

            var prevCharge = gooflingCharge;

            //build charge when below the rail
            if (jumpInput)
            {
                gooflingCharge += Time.fixedDeltaTime * 0.333f;
                gooflingResetTimer = 2;
            }

            if (gooflingResetTimer > 0)
                gooflingResetTimer -= Time.fixedDeltaTime;

            if (gooflingResetTimer <= 0 && gooflingCharge >= 0)
            {
                gooflingCharge -= Time.fixedDeltaTime * .9f;
            }
            gooflingCharge = Mathf.Clamp(gooflingCharge, 0, 1);
            if (!jumpInput)
            {
                gooflingCharge = 0;
                gooflingResetTimer = 0;
            }
        }

        debugTime = time;

        if (time < 0 || time > 1)
        {
            FindNewRailCast();
            DisableJoint();

            //disconnectng from the rail should be its own function
            if (OnRail)
            {
                //transform.position += model.localPosition;
                rb.MovePosition(model.position);
                model.localPosition = Vector3.zero;
                OnRail = false;

                jumpRegroundCooldown = .2f;
            }
        }
        //the force for playermovement
        rb.AddForce(VelocityChange * 10, ForceMode.Force);
    }

    private void CreateFakeObject(float time)
    {
        if (!fakeObject)
        {
            fakeObject = Instantiate(fakePrefab, spline.EvaluatePosition(time), transform.rotation);
            fakeObject.transform.parent = null;
            fakeRB = fakeObject.AddComponent<Rigidbody>();
            fakeRB.interpolation = RigidbodyInterpolation.None;
            fakeRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            fakeJoint = fakeObject.AddComponent<ConfigurableJoint>();
            fakeJoint.xMotion = ConfigurableJointMotion.Free;
            fakeJoint.yMotion = ConfigurableJointMotion.Locked;
            fakeJoint.zMotion = ConfigurableJointMotion.Locked;
            fakeJoint.angularXMotion = ConfigurableJointMotion.Locked;
            fakeJoint.angularYMotion = ConfigurableJointMotion.Locked;
            fakeJoint.angularZMotion = ConfigurableJointMotion.Locked;

            fakeJoint.autoConfigureConnectedAnchor = false;
            FindNewRailCast();
        }
    }

    private bool FindNewRailCast()
    {
        if (Physics.SphereCast(transform.position, .5f, Vector3.down, out RaycastHit hit))
        {
            if (hit.collider.tag == "Rail")// && hit.collider != splineCollider)
            {
                var tempSpline = hit.collider.GetComponent<SplineContainer>();
                using var native = new NativeSpline(
                    tempSpline.Spline,
                    tempSpline.transform.localToWorldMatrix
                );
                
                
                SplineUtility.GetNearestPoint(native, hit.point, out float3 point, out float time);
                return true;
            }
        }
        return false;
    }

    private int ticksWithoutRail = 0;

    private void LateUpdate()
    {
        if (fakeObject && !OnRail)
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                fakeObject.transform.position.z
            );
        if (fakeObject)
            if (Vector3.Distance(fakeJoint.connectedAnchor, fakeObject.transform.position) > 3)
            {
                var lastSpline = spline;
                FindNewRailCast();
                if (spline == lastSpline)
                    ticksWithoutRail++;
                else
                    ticksWithoutRail = 0;
            }
            else if (Vector3.Distance(joint.connectedAnchor, transform.position) > 3)
            {
                var lastSpline = spline;
                FindNewRailCast();
                if (spline == lastSpline)
                    ticksWithoutRail++;
                else
                    ticksWithoutRail = 0;
            }
            else
                ticksWithoutRail = 0;

        //fully disable the joint when disconnected for 3 ticks
        if (ticksWithoutRail > 3)
            DisableJoint();
    }

    private void OnDrawGizmos()
    {
        // I had to add this if/endif to fix build errors - Sagar
#if UNITY_EDITOR
        if (jumpInput)
            Handles.Label(transform.position, "HOLDING SPACEBAR");
#endif
        if (fakeObject)
        {
            Gizmos.DrawSphere(fakeObject.transform.position, .5f);
            Gizmos.DrawSphere(transform.position + Vector3.down, .5f);
            Gizmos.DrawSphere(transform.position + Vector3.down * 50, .5f);
            Debug.DrawRay(
                transform.position + Vector3.forward * .25f,
                Vector3.down * 50,
                Color.red
            );
            Debug.DrawRay(transform.position + Vector3.right * .25f, Vector3.down * 50, Color.red);
            Debug.DrawRay(transform.position + Vector3.left * .25f, Vector3.down * 50, Color.red);
            Debug.DrawRay(transform.position + Vector3.back * .25f, Vector3.down * 50, Color.red);
        }
    }

    void HandleJump(Vector3 direction, float force)
    {
        PartiallyDisableJoint();
        //if (rb.velocity.y < 0)
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(direction.normalized * (force), ForceMode.Impulse);
        transform.position += direction.normalized;
        model.transform.localPosition = Vector3.zero;

        OnRail = false;
    }

    private float textPos = 0;

    private void DrawText(string text)
    {
        if (enableDebuggingBox)
        {
            GUI.Box(new Rect(0, textPos, 150, 20), text);
        }
        textPos += 20;
    }

    // activate the particle emitter
    public void ActivateRailParticles()
    {
        if (railParticles != null)
        {
            railParticles.Play();
        }
    }
    
    // deactivate the particle emitter
    public void DeactivateRailParticles()
    {
        if (railParticles != null)
        {
            railParticles.Stop();
        }
    }

    public void ToggleRailParticles(bool activate)
    {
        if (activate)
        {
            // check if the player is on the rail and going over a certain speed
            if (OnRail && rb.velocity.magnitude > minimumSpeedLimit)
            {
                // activate the particle emitter
                ActivateRailParticles();
            }
        }
        else
        {
            // deactivate the particle emitter
            DeactivateRailParticles();
        }
    }

    private void OnGUI()
    {
        textPos = 0;
        DrawText(OnRail ? "ONRAIL" : "CRINGEFAIL");
        DrawText("Rail: " + spline.name);
        DrawText(BelowRail.ToString());
        DrawText("Goofling Charge: " + gooflingCharge);
        DrawText("Goofling Charge Reset: " + gooflingResetTimer);
        DrawText("JumpCooldown: " + jumpRegroundCooldown);
        DrawText("Inputs: " + inputVector);
        DrawText("TicksWithoutRail: " + ticksWithoutRail);
        DrawText("Time: " + debugTime);
        DrawText("Score: " + PlayerPrefs.GetInt("PlayerScore"));
    }

    private void CheckCollider(Collision other)
    {
        //reattach the player to the rail
        //also set the spline to the new spline object
        /* Debug.Log("COlliderD!" + other.collider.name);*/
        if (jumpRegroundCooldown <= 0 || other.collider != splineCollider)
            if (other.collider.tag == "Rail")
            {
                //Debug.Log(splineCollider.name);
                spline = other.gameObject.GetComponent<SplineContainer>();
                if (spline)
                {
                    //renable the previous collider!!!
                    splineCollider.enabled = true;
                    splineCollider = spline.GetComponent<Collider>();
                    splineCollider.enabled = false;
                    transform.position = other.contacts[0].point;
                    AttachToRail();
                    EnableJoint();
                    
                    OnRail = true;
                }
                else
                {
                    Debug.LogError(
                        "WARNING! Object tagged 'Rail' needs to have a SplineContainer! If you see this error, either add a spline to the object OR contact Mars and ask if they can add a feature!"
                    );
                }
            }
    }

    void OnCollisionEnter(Collision other)
    {
        CheckCollider(other);

        if (other.gameObject.tag == "Death")
        {
            sporetParent.transform.position = spawnPoint.position;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        CheckCollider(other);
    }

    public void StopCharge()
    {
        charge.Stop();
    }
}
