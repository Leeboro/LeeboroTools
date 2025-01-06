// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Splines;

namespace Leeboro.SplineAnimator
{
    [RequireComponent(typeof(Animator))]
    public class SplineNavigator : MonoBehaviour
    {
        [Header("Spline Settings")]
        public SplineContainer splineContainer;  // The SplineContainer from the Unity Splines package
        public int splineIndex = 0;              // If container has multiple Splines, specify which one
        [Range(0f, 1f)] public float progress = 0f; // 0 = start of spline, 1 = end of spline

        [Header("Motion Settings")]
        public float speed = 0f;      // Movement speed along the spline (units/sec)
        public float turnSpeed = 360f; // Max degrees per second to rotate toward movement direction

        private Spline _spline;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            // Retrieve the specific Spline from the container
            if (splineContainer != null && splineIndex < splineContainer.Splines.Count)
            {
                _spline = splineContainer.Splines[splineIndex];
            }
            else
            {
                Debug.LogWarning("SplineNavigator: SplineContainer or index not set properly.");
            }
        }

        private void Update()
        {
            // Normal game-play update; 
            // But note that Timeline 'scrubbing' will also happen via PlayableBehaviour (below)
            if (Application.isPlaying)
            {
                MoveAlongSpline(Time.deltaTime);
            }
        }

        /// <summary>
        /// Moves the character along the spline by speed * deltaTime (if in play mode).
        /// Also rotates the character to face direction of travel.
        /// </summary>
        private void MoveAlongSpline(float deltaTime)
        {
            if (_spline == null) return;

            // How long is the entire spline (approx)? If the splines package changes, adapt accordingly.
            float splineLength = SplineUtility.CalculateLength(_spline);

            // Increase progress based on speed and the total spline length
            float distanceThisFrame = speed * deltaTime;
            float normalizedDistance = (splineLength > 0f)
                ? distanceThisFrame / splineLength
                : 0f;

            progress += normalizedDistance;
            progress = Mathf.Clamp01(progress);

            // Evaluate position & tangent at 'progress'
            Vector3 newPos = splineContainer.transform.TransformPoint(
                SplineUtility.EvaluatePosition(_spline, progress)
            );

            // Evaluate the direction (tangent). 
            Vector3 splineTangent = SplineUtility.EvaluateTangent(_spline, progress);
            splineTangent = splineContainer.transform.TransformDirection(splineTangent).normalized;

            // Rotate to face direction of travel (splineTangent)
            if (splineTangent.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(splineTangent, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    turnSpeed * deltaTime
                );
            }

            // Finally set position
            transform.position = newPos;
        }

        /// <summary>
        /// Called by Timeline each frame to set the speed.
        /// </summary>
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        /// <summary>
        /// If you want Timeline to forcibly set progress (rarely needed if you rely on speed).
        /// </summary>
        public void SetProgress(float newProgress)
        {
            progress = Mathf.Clamp01(newProgress);
            if (_spline != null)
            {
                Vector3 newPos = splineContainer.transform.TransformPoint(
                    SplineUtility.EvaluatePosition(_spline, progress)
                );
                transform.position = newPos;
            }
        }

        /// <summary>
        /// Exposes the Animator so the playable can set float parameters or triggers if needed.
        /// </summary>
        public Animator GetAnimator()
        {
            return _animator;
        }
    }


}