using UnityEngine;

namespace CJ
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager PlayerManagerRef;
        InputHandler InputHandlerRef;
        public AnimatorHandler AnimatorHandlerRef;

        Transform tCameraObject;
        public Vector3 v3MoveDirection;

        [HideInInspector] public Transform tThisTransform;

        public new Rigidbody rigidbody;
        public GameObject goNormalCamera;

        [Header("Ground and air detection stats")]
        [SerializeField] float fGroundDetectionRayStartPoint = 0.5f; //distance from collider start point from gorund
        [SerializeField] float fMinimumDistanceNeededToBeginFall = 1f;
        [SerializeField] float fGroundDetectionRayDistance = 0.2f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;


        [Header("Movement Stats")]
        [SerializeField] float fMovementSpeed = 5f;
        [SerializeField] float fSprintSpeed = 8f;
        [SerializeField] float fRotationSpeed = 10f;
        [SerializeField] float fFallingSpeed = 45f;

        private void Start()
        {
            PlayerManagerRef = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            InputHandlerRef = GetComponent<InputHandler>();
            AnimatorHandlerRef = GetComponentInChildren<AnimatorHandler>();
            tCameraObject = goNormalCamera.transform;
            tThisTransform = transform;

            AnimatorHandlerRef.Initialize();

            PlayerManagerRef.bIsGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11); //bitwise op

        }         

        #region Movement

        Vector3 v3NormalVector;
        Vector3 v3TargetPosition;

        private void HandleRotation(float a_fDelta)
        {
            //calculate direction
            Vector3 v3TargetDir = Vector3.zero;
            float fMoveOverride = InputHandlerRef.fMoveAmount;

            v3TargetDir = tCameraObject.forward * InputHandlerRef.fVertical;
            v3TargetDir += tCameraObject.right * InputHandlerRef.fHorizontal;
            v3TargetDir.Normalize();
            v3TargetDir.y = 0;

            if (v3TargetDir == Vector3.zero)
            {
                v3TargetDir = tThisTransform.forward;
            }

            //calculate rotation
            //float rs = fRotationSpeed;
            //Quaternion tr = Quaternion.LookRotation(v3TargetDir);
            Quaternion targetRotation = Quaternion.Slerp(tThisTransform.rotation, Quaternion.LookRotation(v3TargetDir), fRotationSpeed * a_fDelta);

            //apply rotation
            tThisTransform.rotation = targetRotation;

        }

        public void HandleMovemenet(float a_fDelta)
        {
            if (InputHandlerRef.bRollFlag == true)
            {
                return;
            }

            if (PlayerManagerRef.bIsInteracting == true)
            {
                return;
            }


            //get movement
            v3MoveDirection = tCameraObject.forward * InputHandlerRef.fVertical;
            v3MoveDirection += tCameraObject.right * InputHandlerRef.fHorizontal;
            v3MoveDirection.Normalize();
            v3MoveDirection.y = 0;

            //apply speed
            float speed = fMovementSpeed;

            if (InputHandlerRef.bSprintFlag == true)
            {
                speed = fSprintSpeed;
                PlayerManagerRef.bIsSprinting = true;
                v3MoveDirection *= speed;
            }
            else
            {
                v3MoveDirection *= speed;
            }

            //apply movement
            rigidbody.velocity = Vector3.ProjectOnPlane(v3MoveDirection, v3NormalVector);

            AnimatorHandlerRef.UpdateAmimatorValues(InputHandlerRef.fMoveAmount, 0, PlayerManagerRef.bIsSprinting);

            //apply rotation
            if (AnimatorHandlerRef.bCanRotate)
            {
                HandleRotation(Time.deltaTime);
            }
        }

        public void HandleRollingAndSprinting(float a_fDelta)
        {
            if (AnimatorHandlerRef.animator.GetBool("IsInteracting"))
            {
                return;
            }

            if (InputHandlerRef.bRollFlag == true)
            {
                v3MoveDirection = tCameraObject.forward * InputHandlerRef.fVertical;
                v3MoveDirection += tCameraObject.right * InputHandlerRef.fHorizontal;

                if (InputHandlerRef.fMoveAmount > 0)
                {
                    AnimatorHandlerRef.PlayTargetAnimation("RollForward", true);
                    v3MoveDirection.y = 0;
                    Quaternion qRollRotation = Quaternion.LookRotation(v3MoveDirection);
                    tThisTransform.rotation = qRollRotation;
                }
                else
                {
                    AnimatorHandlerRef.PlayTargetAnimation("BackStep", true);
                }


            }

        }

        public void HandleFalling(float a_fDelta, Vector3 v3MoveDirection)
        {
            PlayerManagerRef.bIsGrounded = false;
            RaycastHit hit;
            Vector3 origin = tThisTransform.position;
            origin.y += fGroundDetectionRayStartPoint;

            if (Physics.Raycast(origin, tThisTransform.forward, out hit, 0.4f))
            {
                v3MoveDirection = Vector3.zero;
            }


            if (PlayerManagerRef.bIsInAir == true)
            {
                rigidbody.AddForce(-Vector3.up * fFallingSpeed);
                rigidbody.AddForce(v3MoveDirection * fFallingSpeed / 8f); //walking off ledge directional force
            }

            Vector3 dir = v3MoveDirection;
            dir.Normalize();
            origin = origin + dir * fGroundDetectionRayDistance;

            v3TargetPosition = tThisTransform.position;

            //show the cast ray
            Debug.DrawRay(origin, -Vector3.up * fMinimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

            if (Physics.Raycast(origin, -Vector3.up, out hit, fMinimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                v3NormalVector = hit.normal;
                Vector3 tp = hit.point;
                PlayerManagerRef.bIsGrounded = true;
                v3TargetPosition.y = tp.y;

                //play landing
                if (PlayerManagerRef.bIsInAir == true)
                {
                    if (inAirTimer > 0.5f)
                    {
                        AnimatorHandlerRef.PlayTargetAnimation("Land", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        AnimatorHandlerRef.PlayTargetAnimation("Locomotion", false);
                        inAirTimer = 0;
                    }

                    PlayerManagerRef.bIsInAir = false;

                }
            }
            else
            {
                if (PlayerManagerRef.bIsGrounded == true)
                {
                    PlayerManagerRef.bIsGrounded = false;
                }

                if (PlayerManagerRef.bIsInAir == false)
                {
                    if (PlayerManagerRef.bIsInteracting == false)
                    {
                        AnimatorHandlerRef.PlayTargetAnimation("Falling", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (fMovementSpeed / 2);
                    PlayerManagerRef.bIsInAir = true;
                }

            }

            if (PlayerManagerRef.bIsGrounded == true)
            {
                if (PlayerManagerRef.bIsInteracting == true || InputHandlerRef.fMoveAmount > 0)
                {
                    tThisTransform.position = Vector3.Lerp(tThisTransform.position, v3TargetPosition, Time.deltaTime);
                }
                else
                {
                    tThisTransform.position = v3TargetPosition;
                }
            }

        }

        #endregion


    }
}
