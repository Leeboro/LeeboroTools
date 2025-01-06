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

    [System.Serializable]
    public class CharacterMovementClip : PlayableAsset, ITimelineClipAsset
    {
        [Tooltip("Normalized end position on the spline (0..1). The start is inferred from the previous clip.")]
        public float endSplinePosition = 1.0f;

        [Tooltip("Base speed multiplier for this segment.")]
        public float speed = 1f;

        [Tooltip("A speed curve that further modifies the base speed over this clip's time (0..1).")]
        public AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 1);

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending | ClipCaps.ClipIn | ClipCaps.SpeedMultiplier; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CharacterMovementBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.endSplinePosition = endSplinePosition;
            behaviour.baseSpeed = speed;
            behaviour.speedCurve = speedCurve;

            return playable;
        }
    }

}