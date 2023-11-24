using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TheHangingHouse.Animations
{
    public static class CoroutineClips
    {
        public static IEnumerator Numerical(float first, float target, Action<float> onUpdate,
            AnimationKey animationKey, Action callback = null, bool finalPose = false)
        {
            var animationCurve = animationKey.animationCurve;
            var speed = animationKey.speed;

            if (!finalPose)
            {
                var minTime = animationCurve.keys[0].time;
                var maxTime = animationCurve.keys[animationCurve.length - 1].time;

                var t = minTime;

                while (t <= maxTime)
                {
                    t += speed * Time.fixedDeltaTime;
                    onUpdate?.Invoke(first + (target - first) * animationCurve.Evaluate(t));
                    yield return new WaitForFixedUpdate();
                }

                callback?.Invoke();
            }
            else
            {
                onUpdate?.Invoke(target * animationCurve.Evaluate(animationCurve.keys[animationCurve.length - 1].time));
                callback?.Invoke();
            }

        }

        public static IEnumerator Numerical(Action<float> onUpdate, AnimationKey animationKey,
            Action callback = null, bool finalPose = false) =>
            Numerical(0f, 1f, onUpdate, animationKey, callback, finalPose);

        public static IEnumerator FadeClip(CanvasGroup element, bool fadeIn, AnimationKey animationKey, Action callback = null, bool finalPose = false)
        {
            var first = element.alpha;
            var target = fadeIn ? 1 : 0;

            yield return Numerical(
                (value) => element.alpha = Mathf.Lerp(first, target, value),
                animationKey,
                callback,
                finalPose
                );
        }

        public static IEnumerator ScaleClip(Transform element, Vector3 targetScale,
            AnimationKey animationKey, Action callback = null, bool finalPose = false)
        {
            var first = element.localScale;
            var final = targetScale;

            yield return Numerical(
                (percent) => element.localScale = first + (final - first) * percent,
                animationKey,
                callback,
                finalPose
                );
        }

        public static IEnumerator MoveClip(Transform element, Vector3 target,
            AnimationKey animationKey, Action callback = null, bool finalPose = false)
        {
            var first = element.position;
            var final = target;

            yield return Numerical(
                (percent) => element.position = first + (final - first) * percent,
                animationKey,
                callback,
                finalPose
                );
        }

        public static IEnumerator MoveUIClip(RectTransform element, Vector2 target,
            AnimationKey animationKey, Action callback = null, bool finalPose = false)
        {
            var first = element.anchoredPosition;
            var final = target;

            yield return Numerical(
                (percent) => element.anchoredPosition = first + (final - first) * percent,
                animationKey,
                callback,
                finalPose
                );
        }

        public static IEnumerator TypingClip(TMP_Text element, float speed = 1f, System.Action onEnd = null)
        {
            var t = 0f;
            var characterNum = 0;

            while (characterNum < element.text.Length)
            {
                t += Time.fixedDeltaTime * speed * 0.5f;
                characterNum = (int)(t / Time.fixedDeltaTime);
                element.maxVisibleCharacters = characterNum;
                yield return new WaitForFixedUpdate();
            }
            onEnd?.Invoke();
        }
    }
}