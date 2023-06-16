using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CJ
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator animator;
        int iVertical;
        int iHorizontal;
        public bool bCanRotate;

        public void Initialize()
        {
            animator = GetComponent<Animator>();
            iVertical = Animator.StringToHash("Vertical");
            iHorizontal = Animator.StringToHash("Horizontal");

        }

        public void UpdateAmimatorValues(float a_fVerticalMovement, float a_fHorizontalMovement)
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
            animator.SetFloat(iVertical, v, 0.1f, Time.deltaTime);
            animator.SetFloat(iHorizontal, h, 0.1f, Time.deltaTime);
        }


        public void CanRotate()
        {
            bCanRotate = true;
        }

        public void StopRotation()
        {
            bCanRotate = false;
        }

    }
}