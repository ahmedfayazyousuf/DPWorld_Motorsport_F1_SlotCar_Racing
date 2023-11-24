using System.Collections;
using System.Collections.Generic;
using TheHangingHouse.Utility.Extensions;
using UnityEngine;
using UnityEngine.Events;
using TheHangingHouse.JsonSerializer;

public abstract class AnimatableElement : MonoBehaviourID
{
    public UnityEvent onShow;
    public UnityEvent onHide;

    private List<Coroutine> m_animationCoroutines;

    [HideInInspector] public bool IsAnimating { get; private set; }

    public abstract void OnBeforeShow();
    public abstract void OnAfterShow();
    public abstract void OnBeforeHide();
    public abstract void OnAfterHide();

    public abstract IEnumerator ShowingClip(System.Action callback);
    public abstract IEnumerator NormalClip();
    public abstract IEnumerator HidingClip(System.Action callback);

    public void Show(System.Action callback = null)
    {
        StopAllClips();
        OnBeforeShow();
        IsAnimating = true;
        StartCoroutine(ShowingClip(() =>
        {
            IsAnimating = false;
            callback?.Invoke();
            OnAfterShow();
            onShow?.Invoke();
        }));
    }

    public void Hide(System.Action callback = null)
    {
        StopAllClips();
        OnBeforeHide();
        IsAnimating = true;
        StartCoroutine(HidingClip(() =>
        {
            IsAnimating = false;
            callback?.Invoke();
            OnAfterHide();
            onHide?.Invoke();
        }));
    }

    public void Play()
    {
        StartCoroutine(NormalClip());
    }

    protected new Coroutine StartCoroutine(IEnumerator routine)
    {
        if (!gameObject.activeSelf) return null;

        var coroutine = base.StartCoroutine(routine);
        m_animationCoroutines ??= new List<Coroutine>();
        m_animationCoroutines.Add(coroutine);
        return coroutine;
    }

    public void StopAllClips()
    {
        if (m_animationCoroutines != null)
            foreach (var coroutine in m_animationCoroutines)
                if (coroutine != null)
                    StopCoroutine(coroutine);
        m_animationCoroutines = null;
        IsAnimating = false;
    }
}
