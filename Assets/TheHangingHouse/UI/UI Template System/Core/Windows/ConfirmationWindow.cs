using System;
using System.Collections;
using System.Collections.Generic;
using TheHangingHouse.Animations;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConfirmationWindow : Window
{
    [Header("Self Parameters")]
    [SerializeField] private TMP_Text m_titleText;
    [SerializeField] private TMP_Text m_messageText;
    [SerializeField] private Button m_acceptButton;
    [SerializeField] private Button m_declineButton;

    [Header("Animation Paramters")]
    private AnimationKey m_animationKey = AnimationKey.EaseInOut * 2;

    protected event Action OnAccept;
    protected event Action OnDecline;

    private void Start()
    {
        m_acceptButton.onClick.AddListener(() => { OnAccept?.Invoke(); Hide(); });
        m_declineButton.onClick.AddListener(() => { OnDecline?.Invoke(); Hide(); });
    }

    public void Request(string title, string message, Action<bool> callback)
    {
        OnAccept = () => callback?.Invoke(true);
        OnDecline = () => callback?.Invoke(false);
    }

    public override void OnBeforeShow()
    {
        transform.localScale = Vector3.zero;
    }

    public override void OnAfterShow()
    {

    }

    public override void OnBeforeHide()
    {

    }

    public override void OnAfterHide()
    {

    }

    public override IEnumerator ShowingClip(Action callback)
    {
        yield return CoroutineClips.ScaleClip(
            transform,
            Vector3.one, 
            m_animationKey,
            callback
            );
    }

    public override IEnumerator NormalClip()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator HidingClip(Action callback)
    {
        yield return CoroutineClips.ScaleClip(
            transform,
            Vector3.one,
            m_animationKey,
            callback
            );
    }
}
