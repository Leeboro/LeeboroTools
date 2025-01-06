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
    using UnityEngine.Timeline;

    [TrackColor(0.8f, 0.3f, 0.3f)]       // arbitrary color for the Timeline track
    [TrackBindingType(typeof(SplineDriver))]
    [TrackClipType(typeof(CharacterMovementClip))]
    public class CharacterMovementTrack : TrackAsset
    {
        // Called internally when building the PlayableGraph.
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            // Create a ScriptPlayable for our mixer
            var playable = ScriptPlayable<CharacterMovementMixerBehaviour>.Create(graph, inputCount);
            return playable;
        }
    }

}