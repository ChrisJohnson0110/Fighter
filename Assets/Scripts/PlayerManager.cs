using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class PlayerManager : MonoBehaviour
    {
        InputHandler InputHandlerRef;
        Animator AnimatorRef;

        // Start is called before the first frame update
        void Start()
        {
            InputHandlerRef = GetComponent<InputHandler>();
            AnimatorRef = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            InputHandlerRef.bIsInteracting = AnimatorRef.GetBool("IsInteracting");
            InputHandlerRef.bRollFlag = false;
        }
    }
}