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
        [SerializeField] float fRotationSpeed = 10f;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            InputHandlerRef = GetComponent<InputHandler>();
            //AnimatorHandlerRef = GetComponent<AnimatorHandler>();
            tCameraObject = goNormalCamera.transform;
            tThisTransform = transform;

            AnimatorHandlerRef.Initialize();
        }         

        private void Update()
        {
            //update inputs
            InputHandlerRef.TickInput(Time.deltaTime);

            //get movement
            v3MoveDirection = tCameraObject.forward * InputHandlerRef.fVertical;
            v3MoveDirection += tCameraObject.right * InputHandlerRef.fHorizontal;
            v3MoveDirection.Normalize();

            //apply speed
            v3MoveDirection *= fMovementSpeed;

            //apply movement
            rigidbody.velocity = Vector3.ProjectOnPlane(v3MoveDirection, v3NormalVector);

            AnimatorHandlerRef.UpdateAmimatorValues(InputHandlerRef.fMoveAmount, 0);

            //apply rotation
            if (AnimatorHandlerRef.bCanRotate)
            {
                HandleRotation(Time.deltaTime);
            }

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

        #endregion


    }
}
