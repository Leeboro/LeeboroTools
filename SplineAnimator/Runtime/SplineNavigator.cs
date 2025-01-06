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

    [RequireComponent(typeof(Animator))]
    public class SplineNavigator : MonoBehaviour
    {
        [Header("Spline Settings")]
        public SplineContainer splineContainer;

        [Tooltip("Default spline index if not overridden by a clip. 0-based.")]
        public int splineIndex = 0;

        [Range(0f, 1f)]
        [Tooltip("Current normalized position along the Spline (0 = start, 1 = end).")]
        public float progress = 0f;

        [Header("Rotation Settings")]
        [Tooltip("Max degrees per second to rotate toward the spline tangent in Play mode.")]
        public float turnSpeed = 360f;

        private Spline _spline;
        private Animator _animator;
        private float _splineLengthCached = 0f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            if (splineContainer != null && splineIndex < splineContainer.Splines.Count)
            {
                _spline = splineContainer.Splines[splineIndex];
                CacheSplineLength(splineIndex);
            }
            else
            {
                Debug.LogWarning("SplineNavigator: SplineContainer or index not set properly.");
            }
        }

        /// <summary>
        /// Switch to another spline in the same container (e.g., to hop to a new path).
        /// Re-caches the length for performance.
        /// </summary>
        public void SetSplineIndex(int newIndex)
        {
            if (splineContainer == null) return;
            if (newIndex < 0 || newIndex >= splineContainer.Splines.Count)
            {
                Debug.LogWarning($"SplineNavigator: Invalid spline index {newIndex}.");
                return;
            }

            splineIndex = newIndex;
            _spline = splineContainer.Splines[newIndex];
            CacheSplineLength(newIndex);
        }

        private void CacheSplineLength(int index)
        {
            // Build matrix
            Matrix4x4 unityMatrix = Matrix4x4.TRS(
                splineContainer.transform.position,
                splineContainer.transform.rotation,
                splineContainer.transform.lossyScale
            );
            float4x4 localToWorld = MatrixConversion.ToFloat4x4(unityMatrix);

            // Cache length
            _splineLengthCached = SplineUtility.CalculateLength(splineContainer.Splines[index], localToWorld);
        }

        /// <summary>
        /// Timeline calls this each frame to set the normalized progress.
        /// This physically moves/rotates the character along the spline.
        /// </summary>
        public void SetProgress(float newProgress)
        {
            progress = Mathf.Clamp01(newProgress);

            if (_spline == null)
                return;

            // Evaluate position & tangent
            Vector3 localPos = SplineUtility.EvaluatePosition(_spline, progress);
            Vector3 newPos = splineContainer.transform.TransformPoint(localPos);

            Vector3 localTan = SplineUtility.EvaluateTangent(_spline, progress);
            Vector3 splineTangent = splineContainer.transform.TransformDirection(localTan).normalized;

            // Rotation
            if (splineTangent.sqrMagnitude > 0.0001f)
            {
                if (Application.isPlaying)
                {
                    // Smooth turn
                    Quaternion targetRot = Quaternion.LookRotation(splineTangent, Vector3.up);
                    float deltaTime = Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, targetRot, turnSpeed * deltaTime
                    );
                }
                else
                {
                    // If we happen to see a preview in editor, we just snap
                    transform.rotation = Quaternion.LookRotation(splineTangent, Vector3.up);
                }
            }

            transform.position = newPos;
        }

        /// <summary>
        /// Optionally call this to set the animator's speed param based on how quickly 'progress' changed this frame.
        /// If you want run/walk transitions in your blend tree, set up that param in your Animator.
        /// </summary>
        public void SetAnimatorSpeedFromDeltaProgress(float deltaProgress, float deltaTime)
        {
            if (deltaTime <= 0f) return;

            float distanceTraveled = deltaProgress * _splineLengthCached;
            float speedValue = distanceTraveled / deltaTime;

            // Set the Animator parameter named "speed" (adjust to your naming)
            _animator.SetFloat("speed", speedValue);
        }

        public Animator GetAnimator() => _animator;
    }

    /// <summary>
    /// Helper to convert UnityEngine.Matrix4x4 -> Unity.Mathematics.float4x4
    /// </summary>
    public static class MatrixConversion
    {
        public static float4x4 ToFloat4x4(this Matrix4x4 m)
        {
            return new float4x4(
                new Unity.Mathematics.float4(m.m00, m.m01, m.m02, m.m03),
                new Unity.Mathematics.float4(m.m10, m.m11, m.m12, m.m13),
                new Unity.Mathematics.float4(m.m20, m.m21, m.m22, m.m23),
                new Unity.Mathematics.float4(m.m30, m.m31, m.m32, m.m33)
            );
        }
    }


}