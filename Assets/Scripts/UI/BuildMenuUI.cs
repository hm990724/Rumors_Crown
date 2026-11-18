using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;
using Rumors.Buildings;

namespace Rumors.UI
{
    /// <summary>
    /// Bottom slide-up panel showing buildable buildings with rich gothic cards.
    /// </summary>
    public class BuildMenuUI : MonoBehaviour
    {
        public static BuildMenuUI Instance { get; private set; }

        private GameObject _root;
        private Transform  _cardContainer;
        private Text       _titleText;
        private Vector2Int _pendingPos;
        private bool       _isOpen;
        private Coroutine  _slideCR;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            // Panel: slides up from bottom (Height expanded to 340px for generous breathing room)
            _root = RumorUIHelper.CreatePanel(canvasRoot, "BuildMenuPanel",
                new Vector2(0, 0), new Vector2(0, 280),
                new Vector2(0.06f, 0), new Vector2(0.94f, 0),
                Color.white);
            var rt = _root.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, -290f);

            // Title
            _titleText = RumorUIHelper.CreateText(_root.transform, "Title", "✦ 영지 건설 ✦",
                new Vector2(0f, -22f), new Vector2(420f, 36f), 22,
                new Color(0.98f, 0.88f, 0.45f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            // Close button
            RumorUIHelper.CreateButton(_root.transform, "CloseBtn", "✕",
                new Vector2(42f, 36f), new Vector2(-18f, -18f),
                Close, new Color(0.7f, 0.2f, 0.2f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            // Scroll view for cards
            var scrollGo = new GameObject("CardScroll");
            scrollGo.transform.SetParent(_root.transform, false);
            var srt = scrollGo.AddComponent<RectTransform>();
            srt.anchorMin = new Vector2(0f, 0f);
            srt.anchorMax = new Vector2(1f, 1f);
            srt.offsetMin = new Vector2(16f, 16f);
            srt.offsetMax = new Vector2(-16f, -54f);
            var sr = scrollGo.AddComponent<ScrollRect>();
            sr.horizontal = true;
            sr.vertical = false;
            scrollGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);

            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollGo.transform, false);
            var vrt = viewport.AddComponent<RectTransform>();
            vrt.anchorMin = Vector2.zero;
            vrt.anchorMax = Vector2.one;
            vrt.sizeDelta = Vector2.zero;
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            sr.viewport = vrt;

            var content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            var crt = content.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 0);
            crt.anchorMax = new Vector2(0, 1);
            crt.pivot = new Vector2(0, 0.5f);
            crt.sizeDelta = new Vector2(0, 0);
            var hlg = content.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 14f;
            hlg.padding = new RectOffset(10, 10, 6, 6);
            hlg.childControlHeight = true;
            hlg.childControlWidth = false;
            content.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sr.content = crt;
            _cardContainer = content.transform;

            _root.SetActive(false);
        }

        public void Open(Vector2Int gridPos)
        {
            _pendingPos = gridPos;
            if (_titleText) _titleText.text = $"✦ 영지 건설 ✦ ({gridPos.x}, {gridPos.y})";
            PopulateCards();
            _root.SetActive(true);
            _isOpen = true;

            if (_slideCR != null) StopCoroutine(_slideCR);
            _slideCR = StartCoroutine(SlideCR(0f));
        }

        public void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;
            if (_slideCR != null) StopCoroutine(_slideCR);
            _slideCR = StartCoroutine(SlideCR(-290f, () => _root.SetActive(false)));
        }

        private System.Collections.IEnumerator SlideCR(float targetY, System.Action onDone = null)
        {
            var rt = _root.GetComponent<RectTransform>();
            float startY = rt.anchoredPosition.y;
            float t = 0f;
            while (t < 0.18f)
            {
                t += Time.unscaledDeltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, t / 0.18f);
                rt.anchoredPosition = new Vector2(0f, Mathf.Lerp(startY, targetY, progress));
                yield return null;
            }
            rt.anchoredPosition = new Vector2(0f, targetY);
            onDone?.Invoke();
        }

        private void PopulateCards()
        {
            foreach (Transform child in _cardContainer) Destroy(child.gameObject);

            int fame = DemonLordManager.Instance?.Fame ?? 0;
            var available = BuildingData.GetAvailable(999);
            if (available == null) return;

            foreach (var data in available)
            {
                bool unlocked = data.RequiredFame <= fame;
                CreateCard(data, unlocked);
            }
        }

        private void CreateCard(BuildingData data, bool unlocked)
        {
            var card = new GameObject("Card_" + data.Type);
            card.transform.SetParent(_cardContainer, false);
            var crt = card.AddComponent<RectTransform>();
            // Expanded card width from 150f to 215f for clear typography and generous click bounds
            crt.sizeDelta = new Vector2(196f, 0f);

            var bg = card.AddComponent<Image>();
            bg.sprite = SpriteRegistry.GetPanel();
            bg.type = Image.Type.Sliced;
            bg.color = unlocked
                ? new Color(0.96f, 0.96f, 1f, 0.98f)
                : new Color(0.48f, 0.48f, 0.52f, 0.75f);

            // Card-wide button click area: Clicking anywhere on unlocked card triggers build!
            if (unlocked)
            {
                var cardBtn = card.AddComponent<Button>();
                cardBtn.targetGraphic = bg;
                var cbColors = cardBtn.colors;
                cbColors.highlightedColor = new Color(1.15f, 1.1f, 0.95f, 1f);
                cbColors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
                cardBtn.colors = cbColors;
                cardBtn.onClick.AddListener(() => ExecuteBuild(data));
            }

            var layout = card.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;

            // 1. Building Sprite Preview (Enhanced size to 64px)
            var sprGo = new GameObject("SpritePreview");
            sprGo.transform.SetParent(card.transform, false);
            var srt = sprGo.AddComponent<RectTransform>();
            srt.sizeDelta = new Vector2(0, 64f);
            var sImg = sprGo.AddComponent<Image>();
            var spr = SpriteRegistry.GetBuilding(data.Type);
            if (spr != null) { sImg.sprite = spr; sImg.preserveAspect = true; }
            sImg.color = unlocked ? Color.white : new Color(0.45f, 0.45f, 0.45f);

            // 2. Building Name (Font size 18, high-contrast gold)
            AddLabel(card.transform, data.DisplayName, 18,
                unlocked ? new Color(0.98f, 0.88f, 0.45f) : new Color(0.6f, 0.6f, 0.6f), 24f, TextAnchor.MiddleCenter);

            // 3. Building Description (2 lines of informative lore / function)
            AddLabel(card.transform, data.Desc ?? "", 12,
                unlocked ? new Color(0.78f, 0.78f, 0.82f) : new Color(0.5f, 0.5f, 0.52f), 32f, TextAnchor.MiddleCenter);

            // 4. Cost Summary (Font size 14, distinct resource color codes)
            AddLabel(card.transform,
                $"<color=#ffd24d>금 {data.GoldCost}</color>   <color=#bf7fff>마 {data.ManaCost}</color>   <color=#d8c4a4>광 {data.OreCost}</color>", 14,
                unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f), 22f, TextAnchor.MiddleCenter);

            // 5. Workers & Upkeep (Font size 13)
            AddLabel(card.transform,
                $"일꾼 {data.MaxWorkers}명 | 유지비 {data.DailyUpkeep}/일", 13,
                new Color(0.85f, 0.85f, 0.9f), 20f, TextAnchor.MiddleCenter);

            // 6. Action Button / Lock Notice (Height 38px, spacious hit area)
            if (!unlocked)
            {
                AddLabel(card.transform, $"🔒 {data.UnlockConditionDesc}", 13,
                    new Color(1f, 0.45f, 0.4f), 38f, TextAnchor.MiddleCenter);
            }
            else
            {
                var btn = RumorUIHelper.CreateButton(card.transform, "BuildBtn", "✦ 건 설 하 기 ✦",
                    new Vector2(0, 38f), Vector2.zero,
                    () => ExecuteBuild(data),
                    Color.white,
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f));
                var brt = btn.GetComponent<RectTransform>();
                brt.anchorMin = new Vector2(0, 0);
                brt.anchorMax = new Vector2(1, 0);
                brt.sizeDelta = new Vector2(0, 38f);
            }
        }

        private void ExecuteBuild(BuildingData data)
        {
            if (BuildingManager.Instance != null &&
                BuildingManager.Instance.TryPlace(data.Type, _pendingPos))
            {
                Close();
            }
        }

        private void AddLabel(Transform parent, string text, int size, Color color, float height, TextAnchor align = TextAnchor.MiddleLeft)
        {
            var go = new GameObject("Lbl");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, height);
            var t = go.AddComponent<Text>();
            t.font = RumorUIHelper.GetKoreanFont();
            t.fontSize = size;
            t.color = color;
            t.text = text;
            t.alignment = align;
            t.supportRichText = true;

            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.85f);
            shadow.effectDistance = new Vector2(1f, -1f);
        }

        private void Update()
        {
            if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
                Close();
        }
    }
}
