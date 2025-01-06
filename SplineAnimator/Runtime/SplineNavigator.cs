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

        [Tooltip("Which spline in the container to use (0-based).")]
        public int splineIndex = 0;

        [Range(0f, 1f)]
        [Tooltip("Where we are along the spline: 0=start, 1=end.")]
        public float progress = 0f;

        [Header("Rotation Settings")]
        [Tooltip("Max degrees per second to rotate toward the spline tangent in Play mode.")]
        public float turnSpeed = 360f;

        private Spline _spline;
        private float _splineLengthCached = 0f;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (splineContainer && splineIndex < splineContainer.Splines.Count)
            {
                _spline = splineContainer.Splines[splineIndex];
                CacheSplineLength();
            }
            else
            {
                Debug.LogWarning("SplineNavigator: SplineContainer or index not set properly.");
            }
        }

        /// <summary>
        /// Re-caches the spline length for performance. 
        /// Useful if you change the spline or transform at runtime, but typically done once in Awake().
        /// </summary>
        private void CacheSplineLength()
        {
            Matrix4x4 unityMatrix = Matrix4x4.TRS(
                splineContainer.transform.position,
                splineContainer.transform.rotation,
                splineContainer.transform.lossyScale
            );
            float4x4 localToWorld = MatrixConversion.ToFloat4x4(unityMatrix);
            _splineLengthCached = SplineUtility.CalculateLength(_spline, localToWorld);
        }

        /// <summary>
        /// Public method for the Timeline (or other code) to set progress [0..1].
        /// This physically moves and orients the character on the spline.
        /// </summary>
        public void SetProgress(float newProgress)
        {
            progress = Mathf.Clamp01(newProgress);
            if (_spline == null) return;

            // Position
            Vector3 localPos = SplineUtility.EvaluatePosition(_spline, progress);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);

            // Tangent (for forward direction)
            Vector3 localTan = SplineUtility.EvaluateTangent(_spline, progress);
            Vector3 splineTangent = splineContainer.transform.TransformDirection(localTan).normalized;

            // Rotate
            if (splineTangent.sqrMagnitude > 0.0001f)
            {
                if (Application.isPlaying)
                {
                    // Smooth turn
                    Quaternion targetRot = Quaternion.LookRotation(splineTangent, Vector3.up);
                    float dt = Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * dt);
                }
                else
                {
                    // Snap rotation in Edit Mode if this is called
                    transform.rotation = Quaternion.LookRotation(splineTangent, Vector3.up);
                }
            }

            // Finally set position
            transform.position = worldPos;
        }

        /// <summary>
        /// Optionally switch to another spline in the container if you want to jump around.
        /// </summary>
        public void SetSplineIndex(int newIndex)
        {
            if (!splineContainer || newIndex < 0 || newIndex >= splineContainer.Splines.Count)
                return;

            splineIndex = newIndex;
            _spline = splineContainer.Splines[newIndex];
            CacheSplineLength();
            SetProgress(0f);
        }
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