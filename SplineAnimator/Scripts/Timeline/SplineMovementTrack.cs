// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Timeline;

namespace Leeboro.SplineAnimator
{
    [TrackBindingType(typeof(SplineNavigator))]
    [TrackClipType(typeof(SplineMovementClip))]
    public class SplineMovementTrack : TrackAsset
    {
        // Normally you'd override CreateTrackMixer if you want a custom mixer playable.
        // For a simple single-clip or non-overlapping usage, we can rely on each clip's PlayableBehaviour directly.
    }
}