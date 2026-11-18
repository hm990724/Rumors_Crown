using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;

namespace Rumors.UI
{
    /// <summary>
    /// Rich gothic dark fantasy UI helper with 9-slice frames, stateful buttons, and drop shadows.
    /// </summary>
    public static class RumorUIHelper
    {
        private static Font _korFont;

        public static Font GetKoreanFont()
        {
            if (_korFont != null) return _korFont;
            _korFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Malgun Gothic", "맑은 고딕", "Apple SD Gothic Neo", "Noto Sans CJK KR", "NanumGothic", "Arial" }, 16);
            return _korFont;
        }

        public static Canvas CreateScreenCanvas(string name, int sortOrder = 0)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0f;
            go.AddComponent<GraphicRaycaster>();
            Object.DontDestroyOnLoad(go);
            return canvas;
        }

        public static GameObject CreatePanel(Transform parent, string name,
            Vector2 offsetMin, Vector2 offsetMax,
            Vector2 anchorMin, Vector2 anchorMax,
            Color? bg = null, Sprite sprite = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            rt.pivot = new Vector2(0.5f, 0.5f);

            var img = go.AddComponent<Image>();
            img.sprite = sprite ?? SpriteRegistry.GetPanel();
            img.type = Image.Type.Sliced;
            img.color = bg ?? Color.white;

            // Subtle gothic frame shadow
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.65f);
            shadow.effectDistance = new Vector2(2f, -2f);

            return go;
        }

        public static GameObject CreateSizedPanel(Transform parent, string name,
            Vector2 size, Vector2 anchoredPos,
            Vector2 anchorMin, Vector2 anchorMax,
            Color? bg = null, Sprite sprite = null, Vector2? pivot = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot ?? new Vector2(
                Mathf.Approximately(anchorMin.x, anchorMax.x) ? anchorMin.x : 0.5f,
                Mathf.Approximately(anchorMin.y, anchorMax.y) ? anchorMin.y : 0.5f
            );
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;

            var img = go.AddComponent<Image>();
            img.sprite = sprite ?? SpriteRegistry.GetPanel();
            img.type = Image.Type.Sliced;
            img.color = bg ?? Color.white;

            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
            shadow.effectDistance = new Vector2(2f, -3f);

            return go;
        }

        public static Text CreateText(Transform parent, string name, string content,
            Vector2 anchoredPos, Vector2 size, int fontSize, Color color,
            TextAnchor align = TextAnchor.MiddleLeft,
            Vector2? anchorMin = null, Vector2? anchorMax = null,
            Vector2? pivot = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var aMin = anchorMin ?? new Vector2(0, 1);
            var aMax = anchorMax ?? new Vector2(0, 1);
            rt.anchorMin = aMin;
            rt.anchorMax = aMax;
            rt.pivot = pivot ?? new Vector2(
                Mathf.Approximately(aMin.x, aMax.x) ? aMin.x : 0.5f,
                Mathf.Approximately(aMin.y, aMax.y) ? aMin.y : 0.5f
            );
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            var t = go.AddComponent<Text>();
            t.font = GetKoreanFont();
            t.fontSize = Mathf.Max(16, fontSize);
            t.color = color;
            t.text = content;
            t.alignment = align;

            // Crisp 16-bit text shadow
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
            shadow.effectDistance = new Vector2(1f, -1f);

            return t;
        }

        public static Button CreateButton(Transform parent, string name, string label,
            Vector2 size, Vector2 anchoredPos, System.Action onClick,
            Color? bg = null,
            Vector2? anchorMin = null, Vector2? anchorMax = null,
            Vector2? pivot = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var aMin = anchorMin ?? new Vector2(1, 0.5f);
            var aMax = anchorMax ?? new Vector2(1, 0.5f);
            rt.anchorMin = aMin;
            rt.anchorMax = aMax;
            rt.pivot = pivot ?? new Vector2(
                Mathf.Approximately(aMin.x, aMax.x) ? aMin.x : 0.5f,
                Mathf.Approximately(aMin.y, aMax.y) ? aMin.y : 0.5f
            );
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;

            var img = go.AddComponent<Image>();
            img.sprite = SpriteRegistry.GetButton("Normal");
            img.type = Image.Type.Sliced;
            img.color = bg ?? Color.white;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            // Dynamic sprite swap state
            btn.transition = Selectable.Transition.SpriteSwap;
            var ss = new SpriteState
            {
                highlightedSprite = SpriteRegistry.GetButton("Hover"),
                pressedSprite     = SpriteRegistry.GetButton("Pressed"),
                disabledSprite    = SpriteRegistry.GetButton("Normal")
            };
            btn.spriteState = ss;

            if (onClick != null) btn.onClick.AddListener(() => onClick());

            // Button label
            var txt = CreateText(go.transform, "Label", label,
                Vector2.zero, size, 15, new Color(0.95f, 0.92f, 0.85f),
                TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
            var trt = txt.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero; trt.anchoredPosition = Vector2.zero;

            return btn;
        }

        public static Image CreateImage(Transform parent, string name, Sprite sprite,
            Vector2 size, Vector2 anchoredPos,
            Vector2? anchorMin = null, Vector2? anchorMax = null,
            Vector2? pivot = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin ?? new Vector2(0, 1);
            rt.anchorMax = anchorMax ?? new Vector2(0, 1);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;
            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = true;
            return img;
        }
    }
}
