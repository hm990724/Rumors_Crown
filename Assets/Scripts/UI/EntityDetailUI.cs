using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;
using Rumors.Buildings;
using Rumors.Monsters;

namespace Rumors.UI
{
    /// <summary>
    /// Right-side slide-in panel showing building details with gothic framing.
    /// </summary>
    public class EntityDetailUI : MonoBehaviour
    {
        public static EntityDetailUI Instance { get; private set; }

        private GameObject _root;
        private Text _nameText, _levelText, _workerText, _outputText;
        private Image _previewImg;
        private Building _currentBuilding;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            _root = RumorUIHelper.CreateSizedPanel(canvasRoot, "EntityDetailPanel",
                new Vector2(300f, 480f), new Vector2(-16f, -76f),
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                Color.white, null, new Vector2(1f, 1f));

            // Close button
            RumorUIHelper.CreateButton(_root.transform, "CloseBtn", "✕",
                new Vector2(36f, 32f), new Vector2(-12f, -12f),
                Hide, new Color(0.7f, 0.2f, 0.2f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            // Building Name
            _nameText = RumorUIHelper.CreateText(_root.transform, "Name", "건물명",
                new Vector2(16f, -18f), new Vector2(230f, 32f), 20,
                new Color(0.98f, 0.88f, 0.5f), TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Sprite preview
            _previewImg = RumorUIHelper.CreateImage(_root.transform, "Preview", null,
                new Vector2(64f, 64f), new Vector2(150f, -86f),
                new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Level text
            _levelText = RumorUIHelper.CreateText(_root.transform, "Level", "단계: 1",
                new Vector2(16f, -130f), new Vector2(260f, 24f), 15,
                new Color(0.85f, 0.85f, 0.95f), TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Output text
            _outputText = RumorUIHelper.CreateText(_root.transform, "Output", "",
                new Vector2(16f, -165f), new Vector2(260f, 80f), 15,
                new Color(0.75f, 0.95f, 0.75f), TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Worker text
            _workerText = RumorUIHelper.CreateText(_root.transform, "Workers", "배치된 작업자: 없음",
                new Vector2(16f, -260f), new Vector2(260f, 50f), 14,
                new Color(0.85f, 0.82f, 0.75f), TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Demolish button
            RumorUIHelper.CreateButton(_root.transform, "DemolishBtn", "철 거",
                new Vector2(110f, 38f), new Vector2(-16f, 18f),
                Demolish, new Color(0.65f, 0.2f, 0.2f),
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f));

            _root.SetActive(false);
        }

        public void ShowBuilding(Building b)
        {
            if (b == null || b.Data == null) return;
            _currentBuilding = b;

            if (_nameText) _nameText.text = b.Data.DisplayName + (b.IsBuilt ? "" : " (공사 중)");
            if (_previewImg) _previewImg.sprite = SpriteRegistry.GetBuilding(b.Type, b.IsBuilt, b.Level);
            if (_levelText) _levelText.text = $"시설 단계: 제 {b.Level} 등급";

            if (_outputText)
            {
                _outputText.text =
                    $"일일 생산량:\n" +
                    $"  • 금화: <color=#ffdf55>{b.DailyGold():+#;-#;0}</color> / 일\n" +
                    $"  • 식량: <color=#ff7777>{b.DailyFood():+#;-#;0}</color> / 일\n" +
                    $"  • 마력: <color=#cc88ff>{b.DailyMana():+#;-#;0}</color> / 일\n" +
                    $"  • 광석: <color=#ddccaa>{b.DailyOre():+#;-#;0}</color> / 일";
            }

            if (_workerText)
            {
                _workerText.text = $"작업 인원: {b.WorkerIds.Count} / {b.Data.MaxWorkers} 명\n" +
                                   $"시설 유지비: <color=#ff7777>-{b.DailyUpkeep()}</color> 금화/일";
            }

            _root.SetActive(true);
        }

        public void Hide()
        {
            _root.SetActive(false);
            _currentBuilding = null;
        }

        private void Demolish()
        {
            if (_currentBuilding != null)
                BuildingManager.Instance?.Demolish(_currentBuilding.GridPos);
            Hide();
        }
    }
}
