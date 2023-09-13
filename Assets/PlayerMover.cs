// using UnityEngine;
// using UnityEngine.Splines;
// using Unity.Mathematics;
// using System.Text;

// public class PlayerMover : MonoBehaviour 
// {
//     private SplineContainer spline;
    
//     private float storedSign = 0;
    
//     private void Start() { 
//         //backup just in case you managed to enter the state without assigning the spline object.
//         if(!spline)
//         {
            
//             //using NativeSpline native = new NativeSpline(); //= new NativeSpline(container.Spline, container.transform.localToWorldMatrix);
//             GameObject[] m_SplineContainer = GameObject.FindGameObjectsWithTag("Rail");
//             Vector3 nearest = Vector3.positiveInfinity;

//             foreach (var bozo in m_SplineContainer)
//                 {
//                     SplineContainer container = bozo.GetComponent<SplineContainer>();
//                     if(container)
//                     {
//                         using var native = new NativeSpline(container.Spline, container.transform.localToWorldMatrix);
//                         float d = SplineUtility.GetNearestPoint(native, manager.transform.position, out float3 p, out float t);
//                         if (d < nearest.magnitude)
//                         {
//                             nearest = p;
//                             spline = container;
//                         }
//                     }
//                 }
//             }
        
//         spline.GetComponent<Collider>().enabled = false;
//         storedVelocity = manager.cMotor.Velocity;
//         manager.storedDecelSpeed = manager.DecelSpeed;
//         manager.DecelSpeed = 0;
//         float3 point = float3.zero;
//         float time = 0;
//         using var gay = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
//         SplineUtility.GetNearestPoint(gay, manager.transform.position,out point,out time);
//         Vector3 upVector = spline.EvaluateUpVector(time);
//         upVector = upVector.normalized;
//         Vector3 tangentVector = spline.EvaluateTangent(time);
//         tangentVector = tangentVector.normalized;
//         //find the direction that the player is travelling along the rail by getting the dot product of their velocity and the direction of the rail
//         //also recalculated each frame in the update velocity function
//         storedSign = Mathf.Sign(Vector3.Dot(storedVelocity.normalized,tangentVector));
//         //storedSign *= 
//         Debug.DrawRay(manager.transform.position,tangentVector,Color.black,10f);
//         Debug.DrawRay(manager.transform.position,storedVelocity,Color.red,10f);
        
        
//         //manager.cMotor.SetPosition((Vector3)point);
//     }
    
//     public   void UpdateState(StateManager manager)
//     {
//         //SplineUtility.GetNearestPoint()
//     }
    
//     public   void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime, StateManager manager)
//     {
//         currentVelocity = Vector3.zero;
//         float time = 0;
//         float3 point = 0;
//         using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
//         SplineUtility.GetNearestPoint(native, manager.transform.position,out point,out time);
//         Vector3 upVector = spline.EvaluateUpVector(time);
//         upVector = upVector.normalized;
//         Vector3 tangentVector = spline.EvaluateTangent(time);
//         tangentVector = tangentVector.normalized;
//         Debug.DrawRay(manager.transform.position,tangentVector,Color.black);
//         //var storedSign = )
//         manager.testDirection = (Vector3)tangentVector;
//         //set new stored velocity to the direction of the tangent vector (or away from it)
//         var nStoredVelocity = storedVelocity.magnitude * tangentVector * storedSign;//Vector3.ProjectOnPlane(storedVelocity,tangentVector);
//         //reset velocity to the new stored velocity
//         storedVelocity = nStoredVelocity;
//         //try and add gravity to the stored velocity (and maybe some friction?)
//         storedVelocity += manager.Gravity * deltaTime * Mathf.Abs(Vector3.Dot(manager.Gravity,tangentVector * -storedSign)) * deltaTime ;
//         //Debug.Log(manager.Gravity * deltaTime * Mathf.Abs(Vector3.Dot(manager.Gravity,tangentVector * -storedSign)));

       
//         //find the direction that the player is travelling along the rail by getting the dot product of their velocity and the direction of the rail
//         storedSign = Mathf.Sign(Vector3.Dot(storedVelocity.normalized,tangentVector));

//         Debug.DrawRay((Vector3)point,nStoredVelocity,Color.red);
//         manager.cMotor.SetPosition(point + (float3)nStoredVelocity * deltaTime);
        
//         manager.handleJump(ref currentVelocity, deltaTime);

//         //runs this when you are trying to jump and exit the rail state
//         if(manager.tryingToJump)
//         {
//             manager.tryingToJump = false;
//             currentVelocity += storedVelocity;
//             storedVelocity = Vector3.zero;
//         }
//     }
//     public   void UpdateRotation(ref Quaternion currentRotation, float deltaTime, StateManager manager)
//     {
//          currentRotation = Quaternion.LookRotation(manager.cMotor.Velocity.normalized, Vector3.up);
//     }
//     public   void AfterCharacterUpdate(float deltaTime, StateManager manager)
//     {

//     }
//     public   void PostGroundingUpdate(float deltaTime, StateManager manager)
//     {

//     }
//     public   bool IsColliderValidForCollisions(Collider coll)
//     {
//         return true;
//     }
//     public   void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport, StateManager manager)
//     {

//     }
//     public   void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport, StateManager manager)
//     {
//         var getSpline = hitCollider.gameObject.GetComponent<SplineContainer>();
//         if(getSpline)
//         {
//             manager.railState.spline = getSpline;
//             spline = getSpline;
//         }
//     }
//     public   void AddVelocity(Vector3 velocity, StateManager manager)
//     {

//     }
//     public   void BeforeCharacterUpdate(float deltaTime, StateManager manager)
//     {

//     }
//     public   void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport, StateManager manager)
//     {

//     }
//     public   void OnDiscreteCollisionDetected(Collider hitCollider, StateManager manager)
//     {

//     }
// }

