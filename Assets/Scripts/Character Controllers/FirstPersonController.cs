using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;


        public float radius;
        private float rayCastLengthCheck;
        public LayerMask layer;
        public Boolean isWallRunning;
        public float wallRunTimer;
        public Boolean hasJustWallJumped;
        public float wallJumpReset;
        public float wallJumpTimer;
        public Vector3 wallDirection;

        // Use this for initialization
        private void Start()
        {
            wallDirection = new Vector3();
            isWallRunning = false;
            hasJustWallJumped = false;
            wallJumpTimer = 0f;
            wallJumpReset = 0.2f;

            m_CharacterController = GetComponent<CharacterController>();
            radius = m_CharacterController.radius;
            rayCastLengthCheck = radius + 0.5f;
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            if (m_CharacterController.isGrounded || IsWallToLeftOrRight())
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            // Landing execution
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                isWallRunning = false;
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            // ?
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            // player grounded during the last frame ?
            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            IsWallToLeftOrRight();
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
            if (!m_Jumping) {
                m_MoveDir.x = desiredMove.x*speed;
                m_MoveDir.z = desiredMove.z*speed;
            }
            if (hasJustWallJumped){
                if(wallJumpTimer < wallJumpReset ){
                    wallJumpTimer += Time.fixedDeltaTime;
                } else {
                    wallJumpTimer = 0f;
                    hasJustWallJumped = false;
                }
            }

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                    m_MoveDir.y = m_JumpSpeed;
                }
            }
            else
            {
                if (m_Jump && IsWallToLeftOrRight()){
                    PlayJumpSound();
                    isWallRunning = false;
                    hasJustWallJumped = true;
                    m_Jump = false;
                    m_Jumping = true;
                    m_MoveDir.y = m_JumpSpeed;
                    if (IsWallToRight()) {
                        m_MoveDir.x = -m_JumpSpeed * transform.right.x;
                        m_MoveDir.z = -m_JumpSpeed * transform.right.z;
                        Debug.Log("Wall on Right");
                    }
                    if (IsWallToLeft()) {
                        m_MoveDir.x = m_JumpSpeed * transform.right.x;
                        m_MoveDir.z = m_JumpSpeed * transform.right.z;
                        Debug.Log("Wall on Left");
                    }
                    m_MoveDir.x += m_JumpSpeed * transform.forward.x;
                    m_MoveDir.z += m_JumpSpeed * transform.forward.z;
                } else {
                    //Moment when the WallRun must be initiated.
                    //Must give a new speed to the player
                    //Conditions to initiate a wallRun:
                    // => Must be on a wall
                    // => Must be running
                    // => Must not already be wallRunning
                    // => Must not have wallJumped on the last frame
                    if(!hasJustWallJumped){
                        if (IsWallToLeftOrRight() && !m_IsWalking && !isWallRunning){
                            Debug.Log("WallRunning engaged");
                            wallRunTimer += Time.fixedDeltaTime;
                            isWallRunning = true;
                            m_MoveDir.y = m_JumpSpeed;
                            if (IsWallToRight()){
                                wallDirection = GetRightWallForward();
                                if ( wallDirection != new Vector3()) {
                                    wallDirection = Vector3.Project(transform.forward, wallDirection);
                                    m_MoveDir.x = m_JumpSpeed * wallDirection.x;
                                    m_MoveDir.z = m_JumpSpeed * wallDirection.z;
                                }
                            } else {
                                wallDirection = GetLeftWallForward();
                                if ( wallDirection != new Vector3()) {
                                    wallDirection = Vector3.Project(transform.forward, wallDirection);
                                    m_MoveDir.x = m_JumpSpeed * wallDirection.x;
                                    m_MoveDir.z = m_JumpSpeed * wallDirection.z;
                                }
                            }
                        } else {
                            if(IsWallToLeftOrRight() && !m_IsWalking){
                                wallRunTimer += Time.fixedDeltaTime;
                                if (wallRunTimer > 0.5f){
                                    m_MoveDir -= 0.5f * Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
                                    if (IsWallToRight()){
                                        wallDirection = GetRightWallForward();
                                        if ( wallDirection != new Vector3()) {
                                            wallDirection = Vector3.Project(transform.forward, wallDirection);
                                            m_MoveDir.x = 2 * m_JumpSpeed * wallDirection.x;
                                            m_MoveDir.z = 2 * m_JumpSpeed * wallDirection.z;
                                        }
                                    } else {
                                        wallDirection = GetLeftWallForward();
                                        if ( wallDirection != new Vector3()) {
                                            wallDirection = Vector3.Project(transform.forward, wallDirection);
                                            m_MoveDir.x = 2 * m_JumpSpeed * wallDirection.x;
                                            m_MoveDir.z = 2 * m_JumpSpeed * wallDirection.z;
                                        }
                                    }

                                }
                            }
                        }

                    }
                    m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
                }
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform, isWallRunning);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        private Boolean IsWallToLeftOrRight(){
            //Debug.Log(mask);
            return (IsWallToLeft() || IsWallToRight());
        }
        private Boolean IsWallToRight(){
            bool wallOnRight = Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.right, rayCastLengthCheck, layer);
            Debug.DrawRay(new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.right * rayCastLengthCheck, Color.green);
            return wallOnRight;
        }
        private Boolean IsWallToLeft(){
            bool wallOnLeft = Physics.Raycast(new Vector3( transform.position.x, transform.position.y, transform.position.z ), -transform.right, rayCastLengthCheck, layer);
            Debug.DrawRay(new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.right * rayCastLengthCheck, Color.red);
            return wallOnLeft;
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
        // private Vector3 GetWallVector(String side, String direction) {
        //     RaycastHit hit;
        //     GameObject wallInContact;
        //     if (Physics)
        //
        //     return new Vector3();
        // }
        // private void OnTriggerEnter(Collider other){
        //   if (other.gameObject.transform.name == "Wall"){
        //     Debug.Log("Touching the wall");
        //     wallInContact = other.gameObject;
        //     Debug.Log(wallInContact.transfrom.position);
        //   }
        // }
        // private void OnTriggerExit(Collider other){
        //     if (other.gameObject.transform.name == "Wall"){
        //       Debug.Log("not touching");
        //     }
        // }
    }
}
