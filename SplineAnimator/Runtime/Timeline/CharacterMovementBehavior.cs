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

    public class CharacterMovementBehaviour : PlayableBehaviour
    {
        [HideInInspector] public float endSplinePosition;
        [HideInInspector] public float baseSpeed;
        [HideInInspector] public AnimationCurve speedCurve;

        // The mixer will set these dynamically:
        public float startSplinePosition;  // assigned by the mixer from the previous clip's end
        public float clipBlendWeight;      // how much this clip influences the final result

        private double clipDuration;
        private double clipStartTime;

        // Called by the Timeline when this playable is created
        public override void OnGraphStart(Playable playable)
        {
            // Store timeline-related info
            clipDuration = playable.GetDuration();
            clipStartTime = playable.GetTime();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            double localTime = playable.GetTime();   // time into this clip
            float clipT = 0f;

            if (clipDuration > 0)
                clipT = (float)(localTime / clipDuration); // 0..1 across the clip

            // Evaluate the speed curve
            float curveValue = speedCurve.Evaluate(clipT);
            float finalSpeed = baseSpeed * curveValue;

            // In this example, we only store the data. The mixer will decide how to blend.
            clipBlendWeight = (float)info.weight;
        }
    }


}