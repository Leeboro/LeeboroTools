// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------


namespace Leeboro.SplineAnimator {
    using UnityEngine;
    using UnityEngine.Playables;

    public class CharacterMovementMixerBehaviour : PlayableBehaviour
    {
        private SplineDriver splineDriver;
        private float lastEndSplinePosition = 0f;  // We'll track start/end positions across clips

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // The bound SplineDriver from the track binding
            splineDriver = playerData as SplineDriver;
            if (splineDriver == null) return;

            int inputCount = playable.GetInputCount();

            // We’ll accumulate a blended speed from all clips
            float blendedSpeed = 0f;
            float totalWeight = 0f;

            // Track who “wins” for position
            int leadClipIndex = -1;
            float highestWeight = 0f;
            float localClipTime = 0f;
            float startPos = 0f;
            float endPos = 0f;

            // Traverse each input (each clip)
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (inputWeight <= 0f)
                    continue;

                // Get the playable + behaviour
                var inputPlayable = (ScriptPlayable<CharacterMovementBehaviour>)playable.GetInput(i);
                var behaviour = inputPlayable.GetBehaviour();
                double clipDuration = inputPlayable.GetDuration();
                double clipTime = inputPlayable.GetTime();

                // The local normalized time 0..1 for the clip
                float clipT = 0f;
                if (clipDuration > 0)
                    clipT = (float)(clipTime / clipDuration);

                // Speed from this clip
                float curveValue = behaviour.speedCurve.Evaluate(clipT);
                float clipSpeed = behaviour.baseSpeed * curveValue;

                // Accumulate speed weighting
                blendedSpeed += clipSpeed * inputWeight;
                totalWeight += inputWeight;

                // Determine if this clip “wins” position
                if (inputWeight > highestWeight)
                {
                    highestWeight = inputWeight;
                    leadClipIndex = i;
                    localClipTime = clipT;
                    // We'll calculate the start/end from the behaviour’s fields
                    startPos = behaviour.startSplinePosition;
                    endPos = behaviour.endSplinePosition;
                }
            }

            // Final speed is the weighted average
            float finalSpeed = (totalWeight > 0f) ? (blendedSpeed / totalWeight) : 0f;

            // Set the speed on the Animator
            splineDriver.SetSpeed(finalSpeed);

            // Now handle the position from the “lead clip”
            if (leadClipIndex >= 0)
            {
                float currentSplinePos = Mathf.Lerp(startPos, endPos, localClipTime);
                splineDriver.SetPositionOnSpline(currentSplinePos);
            }
        }

        // Called when the Timeline graph is created or re-created
        public override void OnGraphStart(Playable playable)
        {
            // We can do some initialization if needed
        }

        // Called when the Timeline starts playing
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            lastEndSplinePosition = 0f;
            UpdateClipStartPositions(playable);
        }

        // When the timeline is updated or the editor changes clips around,
        // we can reassign each clip's start position based on the previous clip's end.
        private void UpdateClipStartPositions(Playable playable)
        {
            int inputCount = playable.GetInputCount();
            float runningEnd = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<CharacterMovementBehaviour>)playable.GetInput(i);
                var behaviour = inputPlayable.GetBehaviour();

                // Assign the start to the current runningEnd
                behaviour.startSplinePosition = runningEnd;

                // The end is whatever the user set in "CharacterMovementClip"
                // We'll read that from the behaviour’s “endSplinePosition” field
                // for clarity, we keep it as is.

                // Update runningEnd for the next clip
                runningEnd = behaviour.endSplinePosition;
            }
        }
    }

}