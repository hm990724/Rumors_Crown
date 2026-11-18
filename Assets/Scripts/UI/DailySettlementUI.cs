using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;

namespace Rumors.UI
{
    /// <summary>
    /// Compact, polished gothic daily settlement dialog.
    /// Does not block or distort the screen.
    /// </summary>
    public class DailySettlementUI : MonoBehaviour
    {
        public static DailySettlementUI Instance { get; private set; }

        private GameObject _overlay;
        private Text _titleText, _dayText, _summaryText, _logText;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            // Soft dim overlay
            _overlay = new GameObject("DailyOverlay");
            _overlay.transform.SetParent(canvasRoot, false);
            var ort = _overlay.AddComponent<RectTransform>();
            ort.anchorMin = Vector2.zero;
            ort.anchorMax = Vector2.one;
            ort.sizeDelta = Vector2.zero;
            var oImg = _overlay.AddComponent<Image>();
            oImg.color = new Color(0, 0, 0, 0.45f);

            // Centered gothic modal panel (460 x 360)
            var panel = RumorUIHelper.CreateSizedPanel(_overlay.transform, "DailyPanel",
                new Vector2(460f, 360f), Vector2.zero,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Color.white, null, new Vector2(0.5f, 0.5f));

            // Title
            _titleText = RumorUIHelper.CreateText(panel.transform, "Title", "✦ 영지 일일 결산 ✦",
                new Vector2(0f, -24f), new Vector2(400f, 30f), 22,
                new Color(0.98f, 0.88f, 0.45f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            // Day subtext
            _dayText = RumorUIHelper.CreateText(panel.transform, "DaySub", "제 1 일차",
                new Vector2(0f, -54f), new Vector2(300f, 22f), 15,
                new Color(0.75f, 0.70f, 0.85f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            // Balance Summary
            _summaryText = RumorUIHelper.CreateText(panel.transform, "Summary", "",
                new Vector2(0f, -90f), new Vector2(400f, 70f), 16,
                new Color(0.95f, 0.95f, 0.95f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            // Event Log
            _logText = RumorUIHelper.CreateText(panel.transform, "Log", "",
                new Vector2(0f, -175f), new Vector2(380f, 100f), 14,
                new Color(0.80f, 0.78f, 0.85f), TextAnchor.UpperLeft,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            // Centered Continue Button
            RumorUIHelper.CreateButton(panel.transform, "NextDayBtn", "▶  다음 날 시작",
                new Vector2(220f, 44f), new Vector2(0f, 24f),
                OnNextDay, Color.white,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));

            _overlay.SetActive(false);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnDaySettled += OnSettled;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnDaySettled -= OnSettled;
        }

        private void OnSettled(DailyReportData report)
        {
            if (_dayText) _dayText.text = $"제 {report.Day} 일차 정산";

            if (_summaryText)
            {
                string gSign = report.GoldDelta >= 0 ? "+" : "";
                string fSign = report.FoodDelta >= 0 ? "+" : "";
                string mSign = report.ManaDelta >= 0 ? "+" : "";
                string oSign = report.OreDelta  >= 0 ? "+" : "";

                _summaryText.text =
                    $"금화: <color=#ffdf55>{gSign}{report.GoldDelta}</color>   식량: <color=#ff7777>{fSign}{report.FoodDelta}</color>\n" +
                    $"마력: <color=#cc88ff>{mSign}{report.ManaDelta}</color>   광석: <color=#ddccaa>{oSign}{report.OreDelta}</color>";
            }

            if (_logText)
            {
                var sb = new System.Text.StringBuilder();
                if (report.EventLogs.Count > 0)
                {
                    foreach (var ev in report.EventLogs)
                        sb.AppendLine("• " + ev);
                }
                else
                {
                    sb.AppendLine("• 마왕성의 하루가 평화롭게 저물었습니다.");
                }
                _logText.text = sb.ToString();
            }

            _overlay.SetActive(true);
            TimeManager.Instance?.SetSpeed(GameSpeed.Paused);
        }

        private void OnNextDay()
        {
            _overlay.SetActive(false);
            TimeManager.Instance?.Resume();
        }
    }
}
