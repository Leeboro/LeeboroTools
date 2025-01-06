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

    public class SplineMovementBehaviour : PlayableBehaviour
    {
        public SplineMovementClipData clipData;

        private float _lastEvalSpeed;
        private bool _clipEnded;

        // Called each frame while the timeline is playing or scrubbing
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            SplineNavigator navigator = playerData as SplineNavigator;
            if (navigator == null) return;

            double time = playable.GetTime();       // Local time within this clip
            double duration = playable.GetDuration(); // Duration of this clip

            // Evaluate the speed curve
            float normalizedTime = (duration > 0.0) ? (float)(time / duration) : 0f;
            float evaluatedSpeed = clipData.speedCurve.Evaluate(normalizedTime);

            // Apply the speed to the SplineNavigator
            navigator.SetSpeed(evaluatedSpeed);
            _lastEvalSpeed = evaluatedSpeed;

            // Also set this to the animator’s speed parameter (assuming your controller uses "speed")
            var anim = navigator.GetAnimator();
            if (anim != null)
            {
                anim.SetFloat("speed", evaluatedSpeed);
            }

            // If you want to forcibly move along the spline during scrubbing (Edit Mode),
            // you could do something like:
            // if (!Application.isPlaying) {
            //     // Manually move along spline using a "fake" deltaTime.
            //     navigator.SetProgress(navigator.progress); 
            // }
        }

        // Called when the clip finishes
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // This is invoked when the timeline finishes playing this clip or stops early
            if (!info.seekOccurred && clipData.stopAtEnd && !_clipEnded)
            {
                SplineNavigator navigator = info.output.GetUserData() as SplineNavigator;
                if (navigator != null)
                {
                    navigator.SetSpeed(0f);

                    var anim = navigator.GetAnimator();
                    if (anim != null)
                    {
                        anim.SetFloat("speed", 0f);
                    }
                }
                _clipEnded = true; // so we don't re-trigger if user scrubs back
            }
        }

        // Called when the clip starts
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _clipEnded = false;
        }
    }


}