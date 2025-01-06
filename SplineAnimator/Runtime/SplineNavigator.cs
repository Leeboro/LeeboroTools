// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------


using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace Leeboro.SplineAnimator
{
    public static class MatrixConversion
    {
        public static float4x4 ToFloat4x4(this Matrix4x4 m)
        {
            // Construct a float4x4 from each row of the UnityEngine.Matrix4x4
            return new float4x4(
                new float4(m.m00, m.m01, m.m02, m.m03),
                new float4(m.m10, m.m11, m.m12, m.m13),
                new float4(m.m20, m.m21, m.m22, m.m23),
                new float4(m.m30, m.m31, m.m32, m.m33)
            );
        }
    }

[RequireComponent(typeof(Animator))]
    public class SplineNavigator : MonoBehaviour
    {
        [Header("Spline Settings")]
        [Tooltip("Reference to the SplineContainer from the Unity Splines package. This holds the actual Spline data.")]
        public SplineContainer splineContainer;

        [Tooltip("If the SplineContainer has multiple Spline objects, specify which one to use. 0-based index.")]
        public int splineIndex = 0;

        [Tooltip("Current normalized position along the Spline (0 = start, 1 = end). Updated by the Timeline track.")]
        [Range(0f, 1f)]
        public float progress = 0f;

        [Header("Motion Settings")]
        [Tooltip("Maximum degrees per second to rotate towards the spline's forward direction. Applies each frame (or each Timeline update).")]
        public float turnSpeed = 360f;

        private Spline _spline;
        private float _splineLengthCached = 0f;
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (splineContainer != null && splineIndex < splineContainer.Splines.Count)
            {
                _spline = splineContainer.Splines[splineIndex];
            }
            else
            {
                Debug.LogWarning("SplineNavigator: SplineContainer or index not set properly.");
                return;
            }

            // Cache spline length once
            Matrix4x4 unityMatrix = Matrix4x4.TRS(
                splineContainer.transform.position,
                splineContainer.transform.rotation,
                splineContainer.transform.lossyScale
            );
            float4x4 localToWorld = MatrixConversion.ToFloat4x4(unityMatrix);
            _splineLengthCached = SplineUtility.CalculateLength(_spline, localToWorld);
        }

        /// <summary>
        /// The Timeline track calls SetProgress each frame (or on scrubbing).
        /// We update position & rotation accordingly.
        /// </summary>
        public void SetProgress(float newProgress)
        {
            progress = Mathf.Clamp01(newProgress);

            if (_spline == null)
                return;

            Vector3 localPos = SplineUtility.EvaluatePosition(_spline, progress);
            Vector3 newPos = splineContainer.transform.TransformPoint(localPos);

            Vector3 localTan = SplineUtility.EvaluateTangent(_spline, progress);
            Vector3 splineTangent = splineContainer.transform.TransformDirection(localTan).normalized;

            // Rotate to face direction of travel
            // Since we might be scrubbing in Editor (no real deltaTime), we do an "instant" rotation in Edit Mode
            // or a turnSpeed-based rotation in Play Mode, whichever you prefer.
            if (splineTangent.sqrMagnitude > 0.0001f)
            {
                if (Application.isPlaying)
                {
                    // Smooth turn
                    Quaternion targetRot = Quaternion.LookRotation(splineTangent, Vector3.up);
                    float deltaTime = Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        turnSpeed * deltaTime
                    );
                }
                else
                {
                    // Instant turn in Editor scrubbing to keep it straightforward
                    transform.rotation = Quaternion.LookRotation(splineTangent, Vector3.up);
                }
            }

            transform.position = newPos;
        }

        /// <summary>
        /// If you want an Animator "speed" param, you can compute it from the derivative of progress.
        /// This is only valid in Play Mode or when the timeline is actively playing forward.
        /// </summary>
        public void SetAnimatorSpeedFromDeltaProgress(float deltaProgress, float deltaTime)
        {
            // Convert deltaProgress -> distance traveled along the path
            float distance = deltaProgress * _splineLengthCached;
            float speed = (deltaTime > 0f) ? distance / deltaTime : 0f;

            // Assign to the animator's "speed" parameter
            _animator.SetFloat("speed", speed);
        }

        public Animator GetAnimator() => _animator;
    }


}