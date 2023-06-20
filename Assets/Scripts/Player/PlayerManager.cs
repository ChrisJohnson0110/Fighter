using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class PlayerManager : MonoBehaviour
    {
        InputHandler InputHandlerRef;
        Animator AnimatorRef;
        CameraHandler CameraHandlerRef;
        PlayerLocomotion PlayerLocomotionRef;

        public bool bIsInteracting;

        [Header("Player flags")]
        public bool bIsSprinting;
        public bool bIsInAir;
        public bool bIsGrounded;

        private void Awake()
        {
            CameraHandlerRef = CameraHandler.instance;
        }

        // Start is called before the first frame update
        void Start()
        {
            InputHandlerRef = GetComponent<InputHandler>();
            AnimatorRef = GetComponentInChildren<Animator>();
            PlayerLocomotionRef = GetComponent<PlayerLocomotion>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;

            bIsInteracting = AnimatorRef.GetBool("IsInteracting");

            //update inputs
            InputHandlerRef.TickInput(delta);
            //movement
            PlayerLocomotionRef.HandleMovemenet(delta);
            PlayerLocomotionRef.HandleRollingAndSprinting(delta);
            PlayerLocomotionRef.HandleFalling(delta, PlayerLocomotionRef.v3MoveDirection);

        }

        private void FixedUpdate()
        {
            float fDelta = Time.deltaTime;

            if (CameraHandlerRef != null)
            {
                CameraHandlerRef.FollowTarget(fDelta);
                CameraHandlerRef.HandleCameraRotation(fDelta, InputHandlerRef.fMouseX, InputHandlerRef.fMouseY);
            }

        }

        private void LateUpdate()
        {
            bIsSprinting = InputHandlerRef.bInput;

            InputHandlerRef.bRollFlag = false;
            InputHandlerRef.bSprintFlag = false;

            if (bIsInAir == true)
            {
                PlayerLocomotionRef.inAirTimer = PlayerLocomotionRef.inAirTimer + Time.deltaTime;
            }
        }

    }
}