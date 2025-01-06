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

        private bool _splineIndexSet = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var navigator = playerData as SplineNavigator;
            if (navigator == null)
                return;

            // 1) If it's near the start of the clip, override the spline index (once).
            double localTime = playable.GetTime();
            if (!_splineIndexSet && localTime < 0.01 && clipData.overrideSplineIndex)
            {
                navigator.SetSplineIndex(clipData.newSplineIndex);
                _splineIndexSet = true;
            }

            // 2) Compute local fraction of the clip
            double duration = playable.GetDuration();
            float t = (duration > 0.0001) ? (float)(localTime / duration) : 0f;

            // 3) Evaluate the user-defined curve to shape progress
            float curveVal = clipData.progressCurve.Evaluate(t);

            // 4) LERP from startProgress..endProgress
            float finalProgress = Mathf.Lerp(clipData.startProgress, clipData.endProgress, curveVal);

            // 5) Set the navigator's progress each frame
            navigator.SetProgress(finalProgress);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Reset the bool so we can set the spline index if this clip restarts
            _splineIndexSet = false;
        }
    }


}