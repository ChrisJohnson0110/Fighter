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
        }

        private void MoveInput(float a_fDelta)
        {
            fHorizontal = v2MovementInput.x;
            fVertical = v2MovementInput.y;
            fMoveAmount = Mathf.Clamp01(Mathf.Abs(fHorizontal) + Mathf.Abs(fVertical));
            fMouseX = v2CameraInput.x;
            fMouseY = v2CameraInput.y;
        }


    }
}
