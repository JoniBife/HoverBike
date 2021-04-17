using System.Collections;
using UnityEngine;

namespace Assets
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform Target;
        // camera will follow this object
        public Transform Anchor;
        // offset between camera and target
        public Vector3 LookOffset;
        // change this value to get desired smoothness
        public float SmoothTime = 0.3f;

        private Vector3 velocity = Vector3.zero;

        private void FixedUpdate()
        {
            // update position
            Vector3 targetPosition = Anchor.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

            // update rotation
            Vector3 targetDirection = (Target.position + LookOffset) - Anchor.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = targetRotation;
        }
    }
}