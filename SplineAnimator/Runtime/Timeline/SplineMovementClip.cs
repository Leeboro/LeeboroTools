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
        [Tooltip("If true, we switch the SplineNavigator's index at the start of this clip.")]
        public bool overrideSplineIndex = false;
        public int newSplineIndex = 0;

        [Range(0f, 1f)] public float startProgress = 0f;
        [Range(0f, 1f)] public float endProgress = 1f;

        [Tooltip("A curve that maps normalized time (0..1) to (0..1). " +
                 "We'll LERP from startProgress..endProgress using the curve's output.")]
        public AnimationCurve progressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }

    public class SplineMovementClip : PlayableAsset, ITimelineClipAsset
    {
        public SplineMovementClipData clipData = new SplineMovementClipData();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplineMovementBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.clipData = clipData;
            return playable;
        }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.Blending; }
        }
    }



}