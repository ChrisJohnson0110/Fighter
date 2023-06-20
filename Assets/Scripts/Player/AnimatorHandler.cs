using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CJ
{
    public class AnimatorHandler : MonoBehaviour
    {
        PlayerManager PlayerManagerRef;
        public Animator animator;
        InputHandler InputHandlerRef;
        PlayerLocomotion PlayerLocomotionRef;

        int iVertical;
        int iHorizontal;
        public bool bCanRotate;

        public void Initialize()
        {
            PlayerManagerRef = GetComponentInParent<PlayerManager>();
            animator = GetComponent<Animator>();
            InputHandlerRef = GetComponentInParent<InputHandler>();
            PlayerLocomotionRef = GetComponentInParent<PlayerLocomotion>();

            iVertical = Animator.StringToHash("Vertical");
            iHorizontal = Animator.StringToHash("Horizontal");

        }

        public void UpdateAmimatorValues(float a_fVerticalMovement, float a_fHorizontalMovement, bool a_bIsSprinting)
        {
            //clamp vertical input
            #region Vertical
            float v = 0;

            if (a_fVerticalMovement > 0 && a_fVerticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (a_fVerticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (a_fVerticalMovement < 0 && a_fVerticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (a_fVerticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion
            //clamp horizontal input
            #region Horizontal
            float h = 0;

            if (a_fHorizontalMovement > 0 && a_fHorizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (a_fHorizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (a_fHorizontalMovement < 0 && a_fHorizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (a_fHorizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            if (a_bIsSprinting == true)
            {
                v = 2;
                h = a_fHorizontalMovement;
            }

            animator.SetFloat(iVertical, v, 0.1f, Time.deltaTime);
            animator.SetFloat(iHorizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string a_sTargetaAnimation, bool a_bIsInteracting)
        {
            animator.applyRootMotion = a_bIsInteracting;
            animator.SetBool("IsInteracting", a_bIsInteracting);
            animator.CrossFade(a_sTargetaAnimation, 0.2f);
        }


        public void CanRotate()
        {
            bCanRotate = true;
        }

        public void StopRotation()
        {
            bCanRotate = false;
        }

        private void OnAnimatorMove()
        {
            if (PlayerManagerRef.bIsInteracting == false)
            {
                return;
            }
            float delta = Time.deltaTime;

            PlayerLocomotionRef.rigidbody.drag = 0;
            Vector3 v3DeltaPosition = animator.deltaPosition;
            v3DeltaPosition.y = 0;
            Vector3 v3Velocity = v3DeltaPosition / delta;
            PlayerLocomotionRef.rigidbody.velocity = v3Velocity;

        }
    }
}