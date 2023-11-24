using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TheHangingHouse.UI
{
    public class ContextMenuPopup : MonoBehaviour
    {
        public struct Option
        {
            public string label;
        }

        [Header("Self Paramters")]
        [SerializeField] private RectTransform m_sourceTemplate;
        [SerializeField] private ContextMenuPopupItem m_sourceItem;

        private Transform m_container;
        private System.Action<ContextMenuPopupItem, int> m_onClickItem;
        private List<ContextMenuPopupItem> m_items = new();
        private RectTransform m_rectTransform;

        private void OnEnable()
        {
            m_sourceTemplate.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
                Hide();
        }

        public void Request(IEnumerable<Option> options, Vector3 position, System.Action<int> callback)
        {
            transform.position = (Vector2)position;

            ClearOptions();
            Generate(options);
            AlignSize();
            Show();

            m_onClickItem += (x, i) =>
            {
                callback?.Invoke(i);
                Hide();
            };
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void AlignSize()
        {
            m_rectTransform ??= GetComponent<RectTransform>();
            var itemHeight = m_sourceItem.GetComponent<RectTransform>().sizeDelta.y;
            var size = m_rectTransform.sizeDelta;
            size.y = itemHeight * m_items.Count;
            m_rectTransform.sizeDelta = size;
        }

        private void Generate(IEnumerable<Option> options)
        {
            var i = 0;
            foreach(var option in options)
            {
                var _i = i;
                var item = CreatePopupItem(option);
                item.button.onClick.AddListener(() => 
                    m_onClickItem?.Invoke(item, _i));
                i++;
            }
        }

        private ContextMenuPopupItem CreatePopupItem(Option option)
        {
            m_container ??= CreateContainer();
            var go = Instantiate(m_sourceItem.gameObject, m_container);
            var item = go.GetComponent<ContextMenuPopupItem>();
            item.label.text = option.label;
            m_items.Add(item);
            return item;
        }

        private RectTransform CreateContainer()
        {
            var go = Instantiate(m_sourceTemplate.gameObject, transform);
            var rt = go.GetComponent<RectTransform>();
            while (rt.childCount > 0)
            {
                var child = rt.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
            go.SetActive(true);
            return rt;
        }

        private void ClearOptions()
        {
            foreach (var item in m_items)
                if (item != null)
                    Destroy(item.gameObject);
            m_items?.Clear();
            m_onClickItem = null;
        }
    }
}
