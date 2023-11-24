using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.Utility.Extensions;
using TheHangingHouse.JsonSerializer;
using UnityEngine.UI;

public class PagesManager : MonoBehaviourID
{
    public static PagesManager MainInstance;

    [Header("Set In Inspector")]
    public bool enableTransitionByArrows = true;
    public List<Page> pages;
    public Button restartButton;

    [Header("Json Settings")]
    [JsonSerializeField] public Data pagesManagerData;

    public int CurrentPageIndex => m_currentPageIndex;

    private int m_currentPageIndex;
    private float m_someEventTime = 0;

    private new void Awake()
    {
        MainInstance = this;

        m_currentPageIndex = 0;
        pages[m_currentPageIndex].Show();

        // Magic restart button setup
        if (restartButton)
        {
            restartButton.gameObject.SetActive(pagesManagerData.enableRestartLeftTopCornerRestartButton);

            restartButton.onClick.AddListener(() =>
            {
                ShowPage(0);
            });
        }
    }

    private new void OnValidate()
    {
        if (pages == null || pages.Count == 0)
            pages = new List<Page>(transform.GetComponentsInChildren<Page>());
    }

    private void Update()
    {
        if (enableTransitionByArrows)
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
                ShowPage(m_currentPageIndex + 1);
            if (Input.GetKeyUp(KeyCode.LeftArrow))
                ShowPage(m_currentPageIndex - 1);
        }

        if (Input.anyKeyDown || m_currentPageIndex == 0)
        {
            m_someEventTime = Time.time;
        }

        if (Time.time - m_someEventTime > pagesManagerData.gameSleepDuration)
        {
            ShowPage(0);
        }
    }

    public void ShowPage(int index)
    {
        StartCoroutine(ShowPageClip(index));
    }

    public void ShowPage(Page page)
    {
        if (!pages.Contains(page))
            Debug.LogError("Page is not exists in the pages manager.");
        var index = pages.IndexOf(page);
        ShowPage(index);
    }

    private IEnumerator ShowPageClip(int index)
    {
        index = Mathf.Min(Mathf.Max(0, index), pages.Count - 1);
        if (m_currentPageIndex == index) yield break;
        if (pages[m_currentPageIndex].gameObject.activeSelf)
            pages[m_currentPageIndex].Hide();
        yield return new WaitForSeconds(0.5f);
        pages[index].Show();
        m_currentPageIndex = index;
    }

    [System.Serializable]
    public class Data
    {
        public float gameSleepDuration = 5;
        public bool enableRestartLeftTopCornerRestartButton = true;
    }
}
