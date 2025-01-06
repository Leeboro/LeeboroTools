// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Leeboro.SplineAnimator
{

    [TrackBindingType(typeof(SplineNavigator))]
    [TrackClipType(typeof(SplineMovementClip))]
    public class SplineMovementTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<SplineMovementMixer> playable = ScriptPlayable<SplineMovementMixer>.Create(graph, inputCount);
            return playable;
        }
    }


}