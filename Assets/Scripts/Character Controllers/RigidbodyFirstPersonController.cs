using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
#if !MOBILE_INPUT
	            if (true)
	            {
		            CurrentTargetSpeed *= RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
#endif
            }
#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

        public float radius;
        public float rayCastLengthCheck;
        public LayerMask layer;
        public Boolean isWallRunning;
        public float wallRunTimer;
        public Boolean hasJustWallJumped;
        public float wallJumpReset;
        public Vector3 wallDirection;
        public Boolean hasDoubleJumped;
        public float wallJumpTimer;
        public float wallJumpDelay;
        public float wallJumpUpMultiplier;
        public Boolean wallWasOnLeft;
        public Boolean wallWasOnRight;
        public Boolean previouslyWallRunning;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);

            radius = m_Capsule.radius;
            wallDirection = new Vector3();
            isWallRunning = false;
            hasJustWallJumped = false;
            wallJumpTimer = 0f;
            wallJumpReset = 0.2f;
            rayCastLengthCheck =  radius + 0.5f;
            wallWasOnLeft = false;
            wallWasOnRight = false;
            previouslyWallRunning = false;
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
        }


        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
                if (m_IsGrounded){
                    desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
                    desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
                } else {
                    if (isWallRunning){
                        desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed * 0.1f;
                        desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed * 0.1f;
                    } else {
                        desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed * 0.2f;
                        desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed * 0.2f;
                    }
                }
                desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;
                if ( input.y >= 0){
                    if (m_RigidBody.velocity.sqrMagnitude >=
                        (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
                    {
                        if (!m_IsGrounded){
                            if (isWallRunning){
                                if (m_Jump){
                                    Debug.Log("Forward for wall Jump");
                                }
                                else {
                                    previouslyWallRunning = true;
                                    if (IsWallToLeft()){
                                        desiredMove = Vector3.Project(desiredMove, GetLeftWallForward());
                                    } else {
                                        desiredMove = Vector3.Project(desiredMove, GetRightWallForward());
                                    }
                                }
                            } else {
                                desiredMove = Vector3.Project(desiredMove, cam.transform.right);
                            }
                        } else { desiredMove = new Vector3();}
                    }
                }
                m_RigidBody.AddForce(desiredMove*SlopeMultiplier(), ForceMode.Impulse);

            }
            // cam.transform.Rotate(-cam.transform.forward * 10f);
            if (m_IsGrounded)
            {
                isWallRunning = false;
                m_RigidBody.drag = 5f;
                hasDoubleJumped = false;
                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                // Initialize Wall Ride
                // wall near + running + Not already wall running
                if (IsWallToLeftOrRight() && movementSettings.Running && !isWallRunning){
                    Vector3 forward = cam.transform.forward;
                    // Quaternion qTo = Quaternion.identity;
                    // if(IsWallToLeft()){
                    //     // if (forward > 0) cam.transform.Rotate(-forward * 5f);
                    //     // else cam.transform.Rotate(forward * 5f);
                    //     qTo = Quaternion.Euler(0f, 0f, -5f);
                    //     //cam.transform.rotation = Quaternion.AngleAxis(-5f, cam.transform.forward);
                    //     //cam.transform.Rotate(0f, 0f, -5f);
                    //     wallWasOnRight = false;
                    //     wallWasOnLeft = true;
                    // } else {
                    //     // Debug.Log(cam.transform.forward);
                    //     // if (forward > 0) cam.transform.Rotate(forward * 5f);
                    //     // else cam.transform.Rotate(-forward * 5f);
                    //     qTo = Quaternion.Euler(0f, 0f, 5f);
                    //     //cam.transform.Rotate(0f, 0f, 5f);
                    //     wallWasOnRight = true;
                    //     wallWasOnLeft = false;
                    // }
                    // rotate form its initial position to qTo position, by step of the last value given
                    //cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, qTo, 1f);
                    Debug.Log("Wall ride initialized");
                    wallJumpTimer = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, 0.8f * movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                    isWallRunning = true;
                    hasDoubleJumped = false;
                } else {
                    if (!IsWallToLeftOrRight()){
                        isWallRunning = false;
                    }
                }
                //Allow wall jumps
                //Give a force up and forwards
                if (isWallRunning){
                    wallJumpTimer += Time.fixedDeltaTime;
                    // Smoothly rotate the camera
                    if (wallJumpTimer < wallJumpDelay){
                        Quaternion qTo = Quaternion.identity;
                        if(IsWallToLeft()){
                            // if (forward > 0) cam.transform.Rotate(-forward * 5f);
                            // else cam.transform.Rotate(forward * 5f);
                            //qTo = Quaternion.Euler(0f, 0f, -90f);
                            qTo = Quaternion.AngleAxis(-90f, cam.transform.forward);
                            //cam.transform.rotation = Quaternion.AngleAxis(-5f, cam.transform.forward);
                            //cam.transform.Rotate(0f, 0f, -5f);
                            wallWasOnRight = false;
                            wallWasOnLeft = true;
                        } else {
                            // Debug.Log(cam.transform.forward);
                            // if (forward > 0) cam.transform.Rotate(forward * 5f);
                            // else cam.transform.Rotate(-forward * 5f);
                            // qTo = Quaternion.Euler(0f, 0f, 90f);
                            qTo = Quaternion.AngleAxis(90f, cam.transform.forward);
                            //cam.transform.Rotate(0f, 0f, 5f);
                            wallWasOnRight = true;
                            wallWasOnLeft = false;
                        }
                        cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, qTo, 20f * Time.fixedDeltaTime);
                    }
                    if (m_Jump && wallJumpTimer > wallJumpDelay){
                        // nouvel algorithme
                        Vector3 jump = new Vector3(0f, wallJumpUpMultiplier * movementSettings.JumpForce, 0f);
                        m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                        if (IsWallToRight()){
                            Vector3 wallNormal = Vector3.Normalize(Vector3.Project(-cam.transform.right, GetRightWallNormal()));
                            m_RigidBody.AddForce(75f * wallNormal, ForceMode.Impulse);
                            m_RigidBody.AddForce(jump, ForceMode.Impulse);
                        } else {
                            // wall to left
                            Vector3 wallNormal = Vector3.Normalize(Vector3.Project(cam.transform.right, GetLeftWallNormal()));
                            m_RigidBody.AddForce(75f * wallNormal, ForceMode.Impulse);
                            m_RigidBody.AddForce(jump, ForceMode.Impulse);
                        }
                    }
                    // -------- REMEMBER ------- need to modify this part of the code
                    // hasJustWallJumped = true;
                    // forward = Vector3.ProjectOnPlane(forward, Vector3.up);
                    // forward = Vector3.Scale( new Vector3(m_RigidBody.velocity.magnitude*4, 0f, m_RigidBody.velocity.magnitude*4), forward);
                    // Vector3 jump = new Vector3(0f, 1.4f * movementSettings.JumpForce, 0f);
                    // m_RigidBody.AddForce(forward + jump, ForceMode.Impulse);
                } else {
                    //double jump
                    if (!hasDoubleJumped && m_Jump){
                        Debug.Log("TEST double jump");
                        hasDoubleJumped = true;
                        Vector3 forward = cam.transform.forward;
                        forward = Vector3.ProjectOnPlane(forward, Vector3.up);
                        forward = Vector3.Scale( new Vector3(m_RigidBody.velocity.magnitude*4, 0f, m_RigidBody.velocity.magnitude*4), forward);
                        Vector3 jump = new Vector3(0f, movementSettings.JumpForce, 0f);
                        m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x / 2f, 0f, m_RigidBody.velocity.z / 2f);
                        m_RigidBody.AddForce(forward + jump, ForceMode.Impulse);
                    }
                }
                // Landing
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            // test if the player was wallRunning on the last frame and isn't on the current frame to rotate the camera back
            if (previouslyWallRunning && !isWallRunning){
                previouslyWallRunning = false;
                Vector3 forward = cam.transform.forward;
                // if (wallWasOnRight){
                //     Debug.Log("Wall was on right");
                //     // if ( forward > 0) cam.transform.Rotate(-forward * 5f);
                //     // else cam.transform.Rotate(forward * 5f);
                //     cam.transform.Rotate(0f, 0f, -5f);
                // } else {
                //     Debug.Log("Wall was on left");
                //     // if (forward > 0) cam.transform.Rotate(forward * 5f);
                //     // else cam.transform.Rotate(-forward * 5f);
                //     cam.transform.Rotate(0f, 0f, 5f);
                // }
            }
            m_Jump = false;
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
			movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform, isWallRunning);

            if (m_IsGrounded)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
        private Boolean IsWallToLeftOrRight(){
            //Debug.Log(mask);
            return (IsWallToLeft() || IsWallToRight());
        }
        private Boolean IsWallToRight(){
            bool wallOnRight = Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.right, rayCastLengthCheck, layer);
            Debug.DrawRay(new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.right * rayCastLengthCheck, Color.green);
            //Debug.Log("Wall on right:" + wallOnRight);
            return wallOnRight;
        }
        private Boolean IsWallToLeft(){
            bool wallOnLeft = Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), -transform.right, rayCastLengthCheck, layer);
            Debug.DrawRay(new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.right * rayCastLengthCheck, Color.red);
            //Debug.Log("Wall on left:" + wallOnLeft);
            return wallOnLeft;
        }
        // return a parallel vector to the wall on the left
        private Vector3 GetLeftWallForward() {
            RaycastHit hit;
            GameObject wallInContact;
            if (Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), -transform.right, out hit, 10f * rayCastLengthCheck, layer)) {
                    wallInContact = hit.collider.gameObject;
                    return wallInContact.transform.forward;
            } else {
                return new Vector3();
            }
        }
        // return a parallel vector to the wall on the right
        private Vector3 GetRightWallForward() {
            RaycastHit hit;
            GameObject wallInContact;
            if (Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.right, out hit, 10f * rayCastLengthCheck, layer)) {
                    wallInContact = hit.collider.gameObject;
                    return wallInContact.transform.forward;
            } else {
                return new Vector3();
            }
        }
        //return the direction of the wall's normal
        private Vector3 GetLeftWallNormal() {
            RaycastHit hit;
            GameObject wallInContact;
            if (Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), -transform.right, out hit, 10f * rayCastLengthCheck, layer)) {
                    wallInContact = hit.collider.gameObject;
                    return wallInContact.transform.right;
            } else {
                return new Vector3();
            }
        }
        //return the direction of the wall's normal
        private Vector3 GetRightWallNormal() {
            RaycastHit hit;
            GameObject wallInContact;
            if (Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.right, out hit, 10f * rayCastLengthCheck, layer)) {
                    wallInContact = hit.collider.gameObject;
                    return wallInContact.transform.right;
            } else {
                return new Vector3();
            }
        }
    }
}
