using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class InputHandler : MonoBehaviour
    {
        public float fHorizontal;
        public float fVertical;
        public float fMoveAmount;
        public float fMouseX;
        public float fMouseY;

        public bool bInput;
        public bool bRollFlag;
        public bool bSprintFlag;
        public float fRollInputTimer;

        PlayerControls inputActions;

        Vector2 v2MovementInput;
        Vector2 v2CameraInput;

        

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => v2MovementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => v2CameraInput = i.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float a_fDelta)
        {
            MoveInput(a_fDelta);
            HandleRollInput(a_fDelta);
        }

        private void MoveInput(float a_fDelta)
        {
            fHorizontal = v2MovementInput.x;
            fVertical = v2MovementInput.y;
            fMoveAmount = Mathf.Clamp01(Mathf.Abs(fHorizontal) + Mathf.Abs(fVertical));
            fMouseX = v2CameraInput.x;
            fMouseY = v2CameraInput.y;
        }

        private void HandleRollInput(float a_fDelta)
        {
            bInput = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            if (bInput)
            {
                fRollInputTimer += a_fDelta;
                bSprintFlag = true;
            }
            else
            {
                if (fRollInputTimer > 0 && fRollInputTimer < 0.5f)
                {
                    bSprintFlag = false;
                    bRollFlag = true;
                }

                fRollInputTimer = 0;
            }
        }
    }
}
