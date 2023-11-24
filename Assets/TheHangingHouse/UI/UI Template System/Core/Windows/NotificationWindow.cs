using System;
using System.Collections;
using System.Collections.Generic;
using TheHangingHouse.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationWindow : Window
{
    [Header("Self Parameters")]
    [SerializeField] private TMP_Text m_titleText;
    [SerializeField] private TMP_Text m_messageText;
    [SerializeField] private Button m_okButton;

    [Header("Animation Paramters")]
    private AnimationKey m_animationKey = AnimationKey.EaseInOut * 2;

    protected event Action OnClickOk;

    private void Start()
    {
        m_okButton.onClick.AddListener(() => { OnClickOk?.Invoke(); Hide(); });
    }

    public void Request(string title, string message, Action callback)
    {
        OnClickOk = () => callback?.Invoke();
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
