using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;

namespace Rumors.UI
{
    /// <summary>
    /// Side panel listing active rumors and their materialization status with gothic framing.
    /// </summary>
    public class RumorPanelUI : MonoBehaviour
    {
        public static RumorPanelUI Instance { get; private set; }

        private GameObject _root;
        private Transform  _listContainer;
        private bool       _isOpen;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            // Toggle button (top-right area, below TopBar)
            RumorUIHelper.CreateButton(canvasRoot, "RumorToggleBtn", "📜 소문 목록",
                new Vector2(110f, 36f), new Vector2(-188f, -76f),
                TogglePanel, Color.white,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            _root = RumorUIHelper.CreateSizedPanel(canvasRoot, "RumorPanel",
                new Vector2(320f, 500f), new Vector2(-188f, -120f),
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                Color.white, null, new Vector2(1f, 1f));

            RumorUIHelper.CreateText(_root.transform, "Title", "✦ 소문 아카이브 ✦",
                new Vector2(0f, -20f), new Vector2(280f, 30f), 18,
                new Color(0.95f, 0.85f, 0.5f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            RumorUIHelper.CreateButton(_root.transform, "CloseBtn", "✕",
                new Vector2(34f, 30f), new Vector2(-12f, -12f),
                Hide, new Color(0.7f, 0.2f, 0.2f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            // Scroll list
            var scrollGo = new GameObject("RumorScroll");
            scrollGo.transform.SetParent(_root.transform, false);
            var srt = scrollGo.AddComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = Vector2.one;
            srt.offsetMin = new Vector2(12f, 12f);
            srt.offsetMax = new Vector2(-12f, -54f);
            var sr = scrollGo.AddComponent<ScrollRect>();
            sr.horizontal = false;
            sr.vertical = true;
            scrollGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);

            var vp = new GameObject("Viewport");
            vp.transform.SetParent(scrollGo.transform, false);
            var vrt = vp.AddComponent<RectTransform>();
            vrt.anchorMin = Vector2.zero;
            vrt.anchorMax = Vector2.one;
            vrt.sizeDelta = Vector2.zero;
            vp.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
            vp.AddComponent<Mask>().showMaskGraphic = false;
            sr.viewport = vrt;

            var content = new GameObject("Content");
            content.transform.SetParent(vp.transform, false);
            var crt = content.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 1);
            crt.anchorMax = new Vector2(1, 1);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.sizeDelta = Vector2.zero;
            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8f;
            vlg.padding = new RectOffset(4, 4, 4, 4);
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sr.content = crt;
            _listContainer = content.transform;

            _root.SetActive(false);
        }

        private void Start()
        {
            if (RumorManager.Instance != null)
                RumorManager.Instance.OnRumorAdded += OnRumorAdded;
        }

        private void OnDestroy()
        {
            if (RumorManager.Instance != null)
                RumorManager.Instance.OnRumorAdded -= OnRumorAdded;
        }

        private void OnRumorAdded(RumorData r) => AddRumorRow(r);

        private void AddRumorRow(RumorData r)
        {
            var row = new GameObject("Rumor_" + r.Id);
            row.transform.SetParent(_listContainer, false);
            var rt = row.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 78f);
            var bg = row.AddComponent<Image>();
            bg.sprite = SpriteRegistry.GetButton("Normal");
            bg.type = Image.Type.Sliced;
            bg.color = Color.white;

            var layout = row.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            AddLine(row.transform, r.Text, 14, new Color(0.98f, 0.90f, 0.60f), 24f);
            AddLine(row.transform, $"소문 신뢰도: {r.Stack} / {r.RequiredStack}", 13, new Color(0.85f, 0.82f, 0.90f), 20f);
            if (r.CanMaterialize)
                AddLine(row.transform, "✦ 마력 실체화 준비 완료!", 13, new Color(0.45f, 1f, 0.65f), 20f);
        }

        private void AddLine(Transform parent, string text, int size, Color color, float h)
        {
            var go = new GameObject("L");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, h);
            var t = go.AddComponent<Text>();
            t.font = RumorUIHelper.GetKoreanFont();
            t.fontSize = size;
            t.color = color;
            t.text = text;
            t.alignment = TextAnchor.MiddleLeft;

            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.85f);
            shadow.effectDistance = new Vector2(1f, -1f);
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            _root.SetActive(_isOpen);
            if (_isOpen) RefreshAll();
        }

        public void Hide()
        {
            _isOpen = false;
            _root.SetActive(false);
        }

        private void RefreshAll()
        {
            foreach (Transform child in _listContainer) Destroy(child.gameObject);
            if (RumorManager.Instance == null) return;
            foreach (var r in RumorManager.Instance.Rumors)
                AddRumorRow(r);
        }
    }
}
