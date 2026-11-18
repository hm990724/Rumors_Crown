using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;
using Rumors.Monsters;

namespace Rumors.UI
{
    /// <summary>
    /// Bottom horizontal bar showing monster thumbnail icons with gothic framing.
    /// </summary>
    public class MonsterListBarUI : MonoBehaviour
    {
        public static MonsterListBarUI Instance { get; private set; }

        private Transform _iconContainer;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            // Panel: bottom bar, 60px tall, centered at bottom
            var panel = RumorUIHelper.CreatePanel(canvasRoot, "MonsterBarPanel",
                new Vector2(0, 0), new Vector2(0, 60),
                new Vector2(0.2f, 0), new Vector2(0.8f, 0),
                Color.white);
            var rt = panel.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = Vector2.zero;

            // Scroll container
            var scrollGo = new GameObject("MonsterIconScroll");
            scrollGo.transform.SetParent(panel.transform, false);
            var srt = scrollGo.AddComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = Vector2.one;
            srt.offsetMin = new Vector2(10f, 6f);
            srt.offsetMax = new Vector2(-10f, -6f);
            var sr = scrollGo.AddComponent<ScrollRect>();
            sr.horizontal = true;
            sr.vertical = false;
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
            crt.anchorMin = new Vector2(0, 0);
            crt.anchorMax = new Vector2(0, 1);
            crt.pivot = new Vector2(0, 0.5f);
            crt.sizeDelta = Vector2.zero;
            var hlg = content.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8f;
            hlg.padding = new RectOffset(6, 6, 2, 2);
            hlg.childControlHeight = true;
            hlg.childControlWidth = false;
            content.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sr.content = crt;
            _iconContainer = content.transform;
        }

        private void Start()
        {
            if (MonsterManager.Instance != null)
            {
                MonsterManager.Instance.OnMonsterAdded += OnMonsterAdded;
                foreach (var m in MonsterManager.Instance.AllMonsters)
                    AddIcon(m);
            }
        }

        private void OnDestroy()
        {
            if (MonsterManager.Instance != null)
                MonsterManager.Instance.OnMonsterAdded -= OnMonsterAdded;
        }

        private void OnMonsterAdded(MonsterData m) => AddIcon(m);

        private void AddIcon(MonsterData m)
        {
            var go = new GameObject("Icon_" + m.Name);
            go.transform.SetParent(_iconContainer, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(48f, 0f);

            var img = go.AddComponent<Image>();
            img.sprite = SpriteRegistry.GetButton("Normal");
            img.type = Image.Type.Sliced;
            img.color = Color.white;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.transition = Selectable.Transition.SpriteSwap;
            btn.spriteState = new SpriteState
            {
                highlightedSprite = SpriteRegistry.GetButton("Hover"),
                pressedSprite     = SpriteRegistry.GetButton("Pressed"),
                disabledSprite    = SpriteRegistry.GetButton("Normal")
            };

            var spr = SpriteRegistry.GetMonster(m.Species);
            if (spr != null)
            {
                var iconGo = new GameObject("Spr");
                iconGo.transform.SetParent(go.transform, false);
                var irt = iconGo.AddComponent<RectTransform>();
                irt.anchorMin = new Vector2(0.12f, 0.12f);
                irt.anchorMax = new Vector2(0.88f, 0.88f);
                irt.sizeDelta = Vector2.zero;
                var sImg = iconGo.AddComponent<Image>();
                sImg.sprite = spr;
                sImg.preserveAspect = true;
            }

            var captured = m;
            btn.onClick.AddListener(() => MonsterPanelUI.Instance?.Show(captured));
        }
    }
}
