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
        [Tooltip("Curve that maps (0 -> Clip Duration) to (0 -> 1) progress along the spline. " +
                 "You can also exceed 1 or go negative if you want to reverse the path.")]
        public AnimationCurve progressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("If true, we'll also set an 'Animator speed' parameter based on the derivative of this curve.")]
        public bool setAnimatorSpeed = true;
    }

    public class SplineMovementClip : PlayableAsset, ITimelineClipAsset
    {
        public SplineMovementClipData clipData = new SplineMovementClipData();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplineMovementBehavior>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.clipData = clipData;
            return playable;
        }

        public ClipCaps clipCaps => ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.Blending;
    }

}