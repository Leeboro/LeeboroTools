// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------

using Leeboro.SplineAnimator;
using UnityEngine;
using UnityEngine.Playables;

namespace Leeboro.SplineAnimator {

    public class SplineMovementMixer : PlayableBehaviour
    {
        private SplineNavigator _navigator;
        private float _prevProgress;
        private double _prevTime;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _navigator = playerData as SplineNavigator;
            if (_navigator == null)
                return;

            int inputCount = playable.GetInputCount();

            float finalProgress = _navigator.progress; // default to current
            bool foundActiveClip = false;
            bool doSetAnimatorSpeed = false;
            double currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
            double deltaTime = currentTime - _prevTime;

            // We'll assume only one clip is active at a time (non-overlapping)
            // If multiple are active, we pick the last with weight>0
            for (int i = 0; i < inputCount; i++)
            {
                float weight = playable.GetInputWeight(i);
                if (weight <= 0f) continue;

                ScriptPlayable<SplineMovementBehaviour> inputPlayable =
                    (ScriptPlayable<SplineMovementBehaviour>)playable.GetInput(i);

                var behaviour = inputPlayable.GetBehaviour();
                var data = behaviour.clipData;

                // On first frame of this clip, if it overrides the spline index, do that
                if (inputPlayable.GetTime() < 0.01 && data.overrideSplineIndex)
                {
                    _navigator.SetSplineIndex(data.newSplineIndex);
                }

                // Evaluate the local time within the clip
                double clipLocalTime = inputPlayable.GetTime();
                double clipDuration = inputPlayable.GetDuration();
                float normalizedTime = (clipDuration > 0.0) ? (float)(clipLocalTime / clipDuration) : 0f;

                float curveValue = data.progressCurve.Evaluate(normalizedTime);
                // Map [0..1] curve value -> [startProgress..endProgress]
                float newProgress = Mathf.Lerp(data.startProgress, data.endProgress, curveValue);

                finalProgress = newProgress;
                doSetAnimatorSpeed = data.setAnimatorSpeed;
                foundActiveClip = true;
            }

            if (foundActiveClip)
            {
                // Set the progress on the navigator
                _navigator.SetProgress(finalProgress);

                // If in Play mode, optionally set animator speed
                if (Application.isPlaying && doSetAnimatorSpeed)
                {
                    float deltaProgress = finalProgress - _prevProgress;
                    if (deltaTime > 0.0)
                    {
                        _navigator.SetAnimatorSpeedFromDeltaProgress(deltaProgress, (float)deltaTime);
                    }
                }
            }

            _prevTime = currentTime;
            _prevProgress = finalProgress;
        }

        public override void OnGraphStart(Playable playable)
        {
            _prevTime = 0.0;
            _prevProgress = 0f;
        }
    }


}