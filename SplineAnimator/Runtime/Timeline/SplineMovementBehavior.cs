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

        // We won't do much here, because we typically let the mixer handle the final logic each frame.
        // But we can handle OnBehaviourPlay, etc.

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Typically used if we want to call SetSplineIndex once at clip start
            // But we might also do it in a mixer. We'll illustrate in the mixer.
        }
    }



}