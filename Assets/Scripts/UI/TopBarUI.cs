using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;

namespace Rumors.UI
{
    /// <summary>
    /// Top status bar: resources, date, speed buttons, save/load.
    /// Anchored to top edge of screen with dark fantasy gothic framing.
    /// </summary>
    public class TopBarUI : MonoBehaviour
    {
        public static TopBarUI Instance { get; private set; }

        private Text _goldText, _manaText, _foodText, _oreText, _dateText, _rumorText;
        private Image _dayProgressFill;
        private Button _pauseBtn, _speed1Btn, _speed2Btn, _speed4Btn;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            // Panel: full-width, 64px tall, anchored top with gothic frame
            var panel = RumorUIHelper.CreatePanel(canvasRoot, "TopBarPanel",
                new Vector2(0, -64), new Vector2(0, 0),
                new Vector2(0, 1), new Vector2(1, 1),
                Color.white);
            var rt = panel.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 1f);

            // --- Left side: resources ---
            float rx = 16f;
            float step = 110f;
            _goldText  = MakeResourceSlot(panel.transform, "Gold",  SpriteRegistry.GetIcon("Gold"),   rx + step * 0, 0f, new Color(1f, 0.88f, 0.35f));
            _manaText  = MakeResourceSlot(panel.transform, "Mana",  SpriteRegistry.GetIcon("Mana"),   rx + step * 1, 0f, new Color(0.85f, 0.55f, 1f));
            _foodText  = MakeResourceSlot(panel.transform, "Food",  SpriteRegistry.GetIcon("Food"),   rx + step * 2, 0f, new Color(0.95f, 0.45f, 0.45f));
            _oreText   = MakeResourceSlot(panel.transform, "Ore",   SpriteRegistry.GetIcon("Worker"), rx + step * 3, 0f, new Color(0.85f, 0.80f, 0.65f));
            _rumorText = MakeResourceSlot(panel.transform, "Rumor", SpriteRegistry.GetIcon("Rumor"),  rx + step * 4, 0f, new Color(0.9f, 0.65f, 1f));

            // --- Center: date & day progress ---
            _dateText = RumorUIHelper.CreateText(panel.transform, "DateText", "1년 1월 1일",
                new Vector2(0f, 8f), new Vector2(260f, 26f), 19,
                new Color(0.98f, 0.92f, 0.75f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            // Day progress bar (below center date)
            var progBg = new GameObject("ProgressBG");
            progBg.transform.SetParent(panel.transform, false);
            var pbrt = progBg.AddComponent<RectTransform>();
            pbrt.anchorMin = new Vector2(0.5f, 0.5f);
            pbrt.anchorMax = new Vector2(0.5f, 0.5f);
            pbrt.pivot = new Vector2(0.5f, 0.5f);
            pbrt.sizeDelta = new Vector2(200f, 8f);
            pbrt.anchoredPosition = new Vector2(0f, -14f);
            var bgImg = progBg.AddComponent<Image>();
            bgImg.color = new Color(0.12f, 0.10f, 0.18f, 0.9f);

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(progBg.transform, false);
            var frt = fillGo.AddComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = new Vector2(0f, 1f);
            frt.pivot = new Vector2(0, 0.5f);
            frt.sizeDelta = Vector2.zero;
            frt.anchoredPosition = Vector2.zero;
            _dayProgressFill = fillGo.AddComponent<Image>();
            _dayProgressFill.color = new Color(0.85f, 0.22f, 0.35f, 1f);

            // --- Right side: speed buttons + save/load ---
            // Anchored right, aligned vertically center
            float btnH = 34f;
            float curX = -16f;

            // Load (rightmost)
            float loadW = 68f;
            curX -= loadW * 0.5f;
            RumorUIHelper.CreateButton(panel.transform, "LoadBtn", "불러오기",
                new Vector2(loadW, btnH), new Vector2(curX, 0f),
                () => SaveLoadManager.Instance?.Load(), null,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));
            curX -= (loadW * 0.5f + 6f);

            // Save
            float saveW = 52f;
            curX -= saveW * 0.5f;
            RumorUIHelper.CreateButton(panel.transform, "SaveBtn", "저장",
                new Vector2(saveW, btnH), new Vector2(curX, 0f),
                () => SaveLoadManager.Instance?.Save(), null,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));
            curX -= (saveW * 0.5f + 12f);

            // Speed 4x
            float spdW = 42f;
            curX -= spdW * 0.5f;
            _speed4Btn = RumorUIHelper.CreateButton(panel.transform, "Speed4Btn", "4×",
                new Vector2(spdW, btnH), new Vector2(curX, 0f),
                () => TimeManager.Instance?.SetSpeed(GameSpeed.UltraFast), null,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));
            curX -= (spdW * 0.5f + 4f);

            // Speed 2x
            curX -= spdW * 0.5f;
            _speed2Btn = RumorUIHelper.CreateButton(panel.transform, "Speed2Btn", "2×",
                new Vector2(spdW, btnH), new Vector2(curX, 0f),
                () => TimeManager.Instance?.SetSpeed(GameSpeed.Fast), null,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));
            curX -= (spdW * 0.5f + 4f);

            // Speed 1x
            curX -= spdW * 0.5f;
            _speed1Btn = RumorUIHelper.CreateButton(panel.transform, "Speed1Btn", "1×",
                new Vector2(spdW, btnH), new Vector2(curX, 0f),
                () => TimeManager.Instance?.SetSpeed(GameSpeed.Normal), null,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));
            curX -= (spdW * 0.5f + 4f);

            // Pause
            curX -= spdW * 0.5f;
            _pauseBtn = RumorUIHelper.CreateButton(panel.transform, "PauseBtn", "⏸",
                new Vector2(spdW, btnH), new Vector2(curX, 0f),
                () => TimeManager.Instance?.SetSpeed(GameSpeed.Paused), null,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));

            RefreshAll();
        }

        private Text MakeResourceSlot(Transform parent, string key, Sprite icon,
            float x, float y, Color color)
        {
            if (icon != null)
                RumorUIHelper.CreateImage(parent, key + "Icon", icon,
                    new Vector2(24f, 24f), new Vector2(x + 12f, y),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f));
            return RumorUIHelper.CreateText(parent, key + "Text", "0",
                new Vector2(x + 30f, y), new Vector2(75f, 28f), 17, color,
                TextAnchor.MiddleLeft, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }

        private void Start()
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged += OnRes;
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnDateChanged  += OnDate;
                TimeManager.Instance.OnDayProgress  += OnProgress;
                TimeManager.Instance.OnSpeedChanged += OnSpeed;
            }
            if (RumorManager.Instance != null)
                RumorManager.Instance.OnStackChanged += OnRumor;
        }

        private void OnDestroy()
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged -= OnRes;
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnDateChanged  -= OnDate;
                TimeManager.Instance.OnDayProgress  -= OnProgress;
                TimeManager.Instance.OnSpeedChanged -= OnSpeed;
            }
            if (RumorManager.Instance != null)
                RumorManager.Instance.OnStackChanged -= OnRumor;
        }

        private void OnRes(int g, int m, int f, int o)
        {
            if (_goldText) _goldText.text = g.ToString();
            if (_manaText) _manaText.text = m.ToString();
            if (_foodText) _foodText.text = f.ToString();
            if (_oreText)  _oreText.text  = o.ToString();
        }

        private void OnDate(int d, int m, int y)
        {
            if (_dateText) _dateText.text = $"{y}년 {m}월 {d}일";
        }

        private void OnProgress(float p)
        {
            if (_dayProgressFill)
                _dayProgressFill.rectTransform.anchorMax = new Vector2(p, 1f);
        }

        private void OnSpeed(GameSpeed s)
        {
            Highlight(_pauseBtn,  s == GameSpeed.Paused);
            Highlight(_speed1Btn, s == GameSpeed.Normal);
            Highlight(_speed2Btn, s == GameSpeed.Fast);
            Highlight(_speed4Btn, s == GameSpeed.UltraFast);
        }

        private void OnRumor(int stack)
        {
            if (_rumorText) _rumorText.text = stack.ToString();
        }

        private void Highlight(Button btn, bool on)
        {
            if (btn == null) return;
            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.color = on ? new Color(1f, 0.85f, 0.4f, 1f) : Color.white;
            }
        }

        public void RefreshAll()
        {
            if (ResourceManager.Instance != null)
                OnRes(ResourceManager.Instance.Gold, ResourceManager.Instance.Mana,
                      ResourceManager.Instance.Food, ResourceManager.Instance.Ore);
            if (TimeManager.Instance != null)
            {
                OnDate(TimeManager.Instance.CurrentDay,
                       TimeManager.Instance.CurrentMonth,
                       TimeManager.Instance.CurrentYear);
                OnSpeed(TimeManager.Instance.Speed);
            }
            if (RumorManager.Instance != null)
                OnRumor(RumorManager.Instance.TotalStack);
        }
    }
}
