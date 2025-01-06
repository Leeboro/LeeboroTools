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
        // For simple usage, we don't define a custom mixer here.
        // Each clip will be handled by its own SplineProgressBehaviour in a playable.
    }

}