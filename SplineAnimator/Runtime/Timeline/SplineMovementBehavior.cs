// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Playables;

namespace Leeboro.SplineAnimator
{
    public class SplineMovementBehavior : PlayableBehaviour
    {
        public SplineMovementClipData clipData;

        private float _prevProgress = 0f;
        private double _prevTime = 0.0;

        // Called once each frame while the Timeline is playing or scrubbing.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var navigator = playerData as SplineNavigator;
            if (navigator == null) return;

            // Current local time within this clip
            double time = playable.GetTime();
            double duration = playable.GetDuration();

            // Evaluate progress from the curve
            float normalizedTime = (duration > 0.0) ? (float)(time / duration) : 0f;
            float currentProgress = clipData.progressCurve.Evaluate(normalizedTime);

            // Set the navigator's progress. This works in play mode & editor scrubbing.
            navigator.SetProgress(currentProgress);

            // Optionally set Animator speed
            if (clipData.setAnimatorSpeed)
            {
                // We can approximate deltaProgress by (currentProgress - _prevProgress)
                float deltaProgress = currentProgress - _prevProgress;
                float deltaTime = (float)(time - _prevTime);

                // In Editor scrubbing backward or jumping, deltaTime might be negative or large.
                // We'll handle that gracefully:
                if (deltaTime > 0f && Application.isPlaying)
                {
                    // Only set speed if we're in actual Play mode (makes sense for character foot movement).
                    navigator.SetAnimatorSpeedFromDeltaProgress(deltaProgress, deltaTime);
                }
                else
                {
                    // We might zero out speed in abrupt scrubs, or just do nothing.
                    navigator.GetAnimator().SetFloat("speed", 0f);
                }
            }

            // Store state for next frame
            _prevProgress = currentProgress;
            _prevTime = time;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _prevTime = playable.GetTime();
            _prevProgress = 0f;  // or evaluate at that start time
        }
    }



}