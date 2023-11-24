using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheHangingHouse.Animations
{
    [System.Serializable]
    public struct AnimationKey
    {
        public AnimationCurve animationCurve;
        public float speed;

        public static AnimationKey EaseInOut => new AnimationKey
        {
            animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f),
            speed = 1f
        };

        public static AnimationKey Linear => new AnimationKey
        {
            animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f),
            speed = 1f
        };

        public static AnimationKey operator *(AnimationKey animationKey, float num) => new AnimationKey
        {
            animationCurve = animationKey.animationCurve,
            speed = animationKey.speed * num
        };

        public static AnimationKey operator *(float num, AnimationKey animationKey) => new AnimationKey
        {
            animationCurve = animationKey.animationCurve,
            speed = animationKey.speed * num
        };
    }
}
