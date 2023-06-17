using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class CameraHandler : MonoBehaviour
    {
        public static CameraHandler instance;

        public Transform tTargetTransform;
        public Transform tCameraTransform;
        public Transform tCameraPivotTransform;

        private Transform tThisTransform;
        private Vector3 v3CameraTransformPosition;
        private LayerMask ignoreLayers;
        private Vector3 v3CameraFollowVelocity = Vector3.zero;

        [Header("Camera Settings")]
        public float fLookSpeed = 0.1f;
        public float fFollowSpeed = 0.1f;
        public float fPivotSpeed = 0.03f;

        public float fMaximumPivot = 35f;
        public float fMinimumPivot = -35f;

        private float fTargetPosition;
        private float fDefaultPosition;
        private float fLookAngle;
        private float fPivotAngle;

        public float fCameraSphereRadius = 0.2f;
        public float fCameraCollisionOffset = 0.2f;
        public float fMinimumCollisionOffset = 0.2f;



        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            tThisTransform = transform;
            fDefaultPosition = tCameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float a_fDelta)
        {
            Vector3 v3TargetPosition = Vector3.SmoothDamp(tThisTransform.position, tTargetTransform.position, 
                ref v3CameraFollowVelocity, a_fDelta / fFollowSpeed);
            tThisTransform.position = v3TargetPosition;

            HandleCameraCollisions(a_fDelta);
        }

        public void HandleCameraRotation(float a_fDelta, float a_fMouseXInput, float a_fMouseYInput)
        {
            fLookAngle += (a_fMouseXInput * fLookSpeed) / a_fDelta;
            fPivotAngle -= (a_fMouseYInput * fPivotSpeed) / a_fDelta;
            fPivotAngle = Mathf.Clamp(fPivotAngle, fMinimumPivot, fMaximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = fLookAngle;
            Quaternion qTargetRotation = Quaternion.Euler(rotation);
            tThisTransform.rotation = qTargetRotation;

            rotation = Vector3.zero;
            rotation.y = fPivotAngle;

            qTargetRotation = Quaternion.Euler(rotation);
            tCameraPivotTransform.localRotation = qTargetRotation;
        }

        private void HandleCameraCollisions(float a_fDelta)
        {
            fTargetPosition = fDefaultPosition;
            RaycastHit hit;
            Vector3 v3Direction = tCameraTransform.position - tCameraPivotTransform.position;
            v3Direction.Normalize();

            if (Physics.SphereCast(tCameraPivotTransform.position, fCameraSphereRadius, v3Direction, out hit, Mathf.Abs(fTargetPosition)
                , ignoreLayers))
            {
                float fDistance = Vector3.Distance(tCameraPivotTransform.position, hit.point);
                fTargetPosition = -(fDistance - fCameraCollisionOffset);
            }

            if (Mathf.Abs(fTargetPosition) < fMinimumCollisionOffset)
            {
                fTargetPosition = -fMinimumCollisionOffset;
            }

            v3CameraTransformPosition.z = Mathf.Lerp(tCameraTransform.localPosition.z, fTargetPosition, a_fDelta / 0.2f);
            tCameraTransform.localPosition = v3CameraTransformPosition;

        }
    }
}