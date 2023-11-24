using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TheHangingHouse.UI
{
    [ExecuteAlways]
    public class CircularSlider : MonoBehaviour
    {
        [Header("Set In Inspector")]
        public Image background;
        public Image fillImage;
        public RectTransform handle;
        public float value;

        [Header("Events"), Space(5)]
        public UnityEvent<float, float> onValueChange;

        private Canvas m_Canvas => m_canvas ??= GetCanvas(transform);
        private Canvas m_canvas;

        private RectTransform m_RectTransform => m_rectTransform ??= GetComponent<RectTransform>();
        private RectTransform m_rectTransform;

        private float m_cachedValue;
        private bool m_mouseInBoundary;
        private bool m_selecting;

        private void Update()
        {
            HandleInput();
            HandleValueChange();
            BoundValue();
            AlignFill();
            AlignHandle();

            if (m_cachedValue != value)
                onValueChange?.Invoke(m_cachedValue * 360f, value * 360f);

            m_cachedValue = value;
        }

        public void OnPointerEnter(BaseEventData e)
        {
            m_mouseInBoundary = true;
        }

        public void OnPointerExit(BaseEventData e)
        {
            m_mouseInBoundary = false;
        }

        private void HandleValueChange()
        {
            if (m_selecting)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out var mousePos);
                var deltaPosition = mousePos - m_RectTransform.anchoredPosition;
                var currentAngle = value * Mathf.PI * 2f;
                var targetAngle1 = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
                var targetAngle2 = targetAngle1 > 0 ? targetAngle1 - Mathf.PI * 2 : targetAngle1 + Mathf.PI * 2;
                var targetAngle = Mathf.Sign(currentAngle) == Mathf.Sign(targetAngle1) ? targetAngle1 : targetAngle2;
                targetAngle = Mathf.Abs(currentAngle) < 1f ? targetAngle1 : targetAngle;
                value = targetAngle / (Mathf.PI * 2f);
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
                m_selecting = m_mouseInBoundary;
            if (Input.GetMouseButtonUp(0))
                m_selecting = false;
        }

        private void BoundValue()
        {
            value = Mathf.Clamp(value, -1, 1);
        }

        private void AlignFill()
        {
            fillImage.fillClockwise = value < 0;
            fillImage.fillAmount = Mathf.Abs(value);
        }

        private void AlignHandle()
        {
            var angle = value * 360f * Mathf.Deg2Rad;

            var width = GetGlobalWidth(handle.parent.GetComponent<RectTransform>());
            var height = GetGlobalHeight(handle.parent.GetComponent<RectTransform>());

            var pos = Vector2.zero;
            pos.x = Mathf.Cos(angle) * width * 0.5f;
            pos.y = Mathf.Sin(angle) * height * 0.5f;

            handle.anchoredPosition = pos;
        }

        public float GetGlobalWidth(RectTransform rectTransform)
        {
            if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
                return rectTransform.sizeDelta.x;

            var canvas = rectTransform.GetComponent<Canvas>();
            if (canvas != null)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
                 canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    return Screen.width;
                else 
                    return m_rectTransform.sizeDelta.x;
            }

            var parent = rectTransform.parent.GetComponent<RectTransform>();
            var percent = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
            var width = percent * GetGlobalWidth(parent);
            width -= rectTransform.offsetMin.x - rectTransform.offsetMax.x;

            return width;
        }

        public float GetGlobalHeight(RectTransform rectTransform)
        {
            if (rectTransform.anchorMin.y == rectTransform.anchorMax.y)
                return rectTransform.sizeDelta.y;

            var canvas = rectTransform.GetComponent<Canvas>();
            if (canvas != null)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    return Screen.height;
                else
                    return m_rectTransform.sizeDelta.y;
            }

            var parent = rectTransform.parent.GetComponent<RectTransform>();
            var percent = rectTransform.anchorMax.y - rectTransform.anchorMin.y;
            var height = percent * GetGlobalHeight(parent);
            height -= rectTransform.offsetMin.y - rectTransform.offsetMax.y;

            return height;
        }

        private Canvas GetCanvas(Transform t)
        {
            var canvas = t.GetComponent<Canvas>();
            if (canvas == null)
                return GetCanvas(t.parent);
            return canvas;
        }
    }
}