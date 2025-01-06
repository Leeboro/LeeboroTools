// -----------------------------------------------------------------------
// Copyright (c) 2025 Leeboro. All rights reserved.
// -----------------------------------------------------------------------
// Author: 
// Creation Date: 
// Description: Description of the script
// -----------------------------------------------------------------------


using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Leeboro.SplineAnimator
{

    [Serializable]
    public class SplineMovementClipData
    {
        [Tooltip("Mapping of (0->clip duration) to speed.")]
        public AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [Tooltip("Set this true if you want to force speed to 0 at clip end.")]
        public bool stopAtEnd = false;
    }

    public class SplineMovementClip : PlayableAsset, ITimelineClipAsset
    {
        public SplineMovementClipData clipData = new SplineMovementClipData();

        // Timeline will call this to create the playable
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<SplineMovementBehaviour> playable = ScriptPlayable<SplineMovementBehaviour>.Create(graph);

            // Retrieve the behaviour instance to store the clipData
            SplineMovementBehaviour behaviour = playable.GetBehaviour();
            behaviour.clipData = clipData;

            return playable;
        }

        // Settings that describe the clip's capabilities (e.g., looping, etc.)
        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending | ClipCaps.Extrapolation | ClipCaps.ClipIn; }
        }
    }


}