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
        [Tooltip("If true, we'll call SetSplineIndex(newSplineIndex) at the beginning of this clip.")]
        public bool overrideSplineIndex = false;
        public int newSplineIndex = 0;

        [Range(0f, 1f)] public float startProgress = 0f;
        [Range(0f, 1f)] public float endProgress = 1f;

        [Tooltip("Curve from 0->1 on X to 0->1 on Y. This shapes how quickly we move from startProgress to endProgress.")]
        public AnimationCurve progressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("If true, we'll set the animator's 'speed' param each frame based on deltaProgress.")]
        public bool setAnimatorSpeed = true;
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
            get { return ClipCaps.Blending | ClipCaps.Extrapolation | ClipCaps.ClipIn; }
        }
    }




}