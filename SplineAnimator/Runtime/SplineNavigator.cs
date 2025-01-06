// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // for float4x4
using System;             // for [Serializable], if needed

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

        [Tooltip("Current normalized position along the Spline (0 = start, 1 = end). Used for in-game movement logic.")]
        [Range(0f, 1f)]
        public float progress = 0f;

        [Header("Motion Settings")]
        [Tooltip("Movement speed along the spline, in scene units per second.")]
        public float speed = 0f;

        [Tooltip("Maximum degrees per second to rotate towards the spline's forward direction.")]
        public float turnSpeed = 360f;

        private Spline _spline;
        private Animator _animator;
        private float _splineLengthCached = 0f;

        private void Awake()
        {
            // 1) Get the animator
            _animator = GetComponent<Animator>();

            // 2) Retrieve the specific Spline from the container
            if (splineContainer != null && splineIndex < splineContainer.Splines.Count)
            {
                _spline = splineContainer.Splines[splineIndex];
            }
            else
            {
                Debug.LogWarning("SplineNavigator: SplineContainer or index not set properly.");
                return;
            }

            // 3) Cache the length (assuming the spline doesn't change at runtime).
            Matrix4x4 unityMatrix = Matrix4x4.TRS(
                splineContainer.transform.position,
                splineContainer.transform.rotation,
                splineContainer.transform.lossyScale
            );

            float4x4 localToWorld = MatrixConversion.ToFloat4x4(unityMatrix);
            _splineLengthCached = SplineUtility.CalculateLength(_spline, localToWorld);
        }

        private void Update()
        {
            // Normal game-play update. 
            // If in a Timeline preview scenario, the Timeline track might also set speed, progress, etc.
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
            if (_spline == null)
                return;

            // Instead of recalculating the length each frame, use the cached value.
            float splineLength = _splineLengthCached;

            float distanceThisFrame = speed * deltaTime;
            float normalizedDistance = (splineLength > 0f)
                ? distanceThisFrame / splineLength
                : 0f;

            progress += normalizedDistance;
            progress = Mathf.Clamp01(progress);

            // Evaluate position & tangent at 'progress'
            Vector3 localPos = SplineUtility.EvaluatePosition(_spline, progress);
            Vector3 newPos = splineContainer.transform.TransformPoint(localPos);

            Vector3 localTan = SplineUtility.EvaluateTangent(_spline, progress);
            Vector3 splineTangent = splineContainer.transform.TransformDirection(localTan).normalized;

            // Rotate to face direction of travel
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
        /// Called by the Timeline each frame to set the speed (e.g., from a Timeline clip).
        /// </summary>
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        /// <summary>
        /// If the Timeline or code wants to forcibly set progress (rarely needed if you rely on speed).
        /// </summary>
        public void SetProgress(float newProgress)
        {
            progress = Mathf.Clamp01(newProgress);

            if (_spline != null)
            {
                Vector3 localPos = SplineUtility.EvaluatePosition(_spline, progress);
                transform.position = splineContainer.transform.TransformPoint(localPos);
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