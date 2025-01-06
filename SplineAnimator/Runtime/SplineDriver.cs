// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------


namespace Leeboro.SplineAnimator {
    using UnityEngine;
    using UnityEngine.Splines;

    [RequireComponent(typeof(Animator))]
    public class SplineDriver : MonoBehaviour
    {
        [Header("Spline Settings")]
        public SplineContainer splineContainer;
        public bool orientToSpline = true;

        [Header("Animation")]
        public Animator characterAnimator;
        [Tooltip("Animator Speed parameter name.")]
        public string speedParam = "Speed";

        private void Reset()
        {
            // Attempt to get references automatically
            characterAnimator = GetComponent<Animator>();
            if (splineContainer == null)
            {
                splineContainer = FindObjectOfType<SplineContainer>();
            }
        }

        /// <summary>
        /// Sets the character's position (and optionally rotation) along the spline
        /// at the specified normalized t value [0..1].
        /// </summary>
        public void SetPositionOnSpline(float t)
        {
            if (splineContainer == null || splineContainer.Spline == null)
                return;

            // Evaluate world-space position from the SplineContainer
            Vector3 pos = splineContainer.Spline.EvaluatePosition(t);
            transform.position = pos;

            if (orientToSpline)
            {
                // Since EvaluateRotation() isn't available,
                // we create our rotation from EvaluateTangent() and EvaluateUpVector()
                Vector3 forward = splineContainer.Spline.EvaluateTangent(t);
                Vector3 up = splineContainer.Spline.EvaluateUpVector(t);

                // Construct a rotation looking "forward" along the tangent, with "up" as the up-vector
                transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }

        /// <summary>
        /// Sets the 'Speed' parameter in the Animator (if assigned).
        /// </summary>
        public void SetSpeed(float speed)
        {
            if (characterAnimator != null && !string.IsNullOrEmpty(speedParam))
            {
                characterAnimator.SetFloat(speedParam, speed);
            }
        }
    }


}