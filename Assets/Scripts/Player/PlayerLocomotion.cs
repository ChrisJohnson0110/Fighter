using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class PlayerLocomotion : MonoBehaviour
    {
        InputHandler InputHandlerRef;
        public AnimatorHandler AnimatorHandlerRef;

        Transform tCameraObject;
        Vector3 v3MoveDirection;

        [HideInInspector] public Transform tThisTransform;

        public new Rigidbody rigidbody;
        public GameObject goNormalCamera;

        [Header("Stats")]
        [SerializeField] float fMovementSpeed = 5f;
        [SerializeField] float fSprintSpeed = 8f;
        [SerializeField] float fRotationSpeed = 10f;

        public bool bIsSprinting;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            InputHandlerRef = GetComponent<InputHandler>();
            AnimatorHandlerRef = GetComponentInChildren<AnimatorHandler>();
            tCameraObject = goNormalCamera.transform;
            tThisTransform = transform;

            AnimatorHandlerRef.Initialize();
        }         

        private void Update()
        {
            float delta = Time.deltaTime;

            bIsSprinting = InputHandlerRef.bInput;

            //update inputs
            InputHandlerRef.TickInput(delta);
            //movement
            HandleMovemenet(delta);
            HandleRollingAndSprinting(delta);

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

        private void HandleMovemenet(float a_fDelta)
        {
            if (InputHandlerRef.bRollFlag == true)
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
                bIsSprinting = true;
                v3MoveDirection *= speed;
            }
            else
            {
                v3MoveDirection *= speed;
            }

            //apply movement
            rigidbody.velocity = Vector3.ProjectOnPlane(v3MoveDirection, v3NormalVector);

            AnimatorHandlerRef.UpdateAmimatorValues(InputHandlerRef.fMoveAmount, 0, bIsSprinting);

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

        #endregion


    }
}
