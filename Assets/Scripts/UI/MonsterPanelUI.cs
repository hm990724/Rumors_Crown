using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;
using Rumors.Monsters;

namespace Rumors.UI
{
    /// <summary>
    /// Right-side slide-in panel showing monster details with gothic framing.
    /// </summary>
    public class MonsterPanelUI : MonoBehaviour
    {
        public static MonsterPanelUI Instance { get; private set; }

        private GameObject _root;
        private Text _nameText, _speciesText, _statsText, _statusText, _traitsText;
        private Image _monsterPreview;
        private MonsterData _current;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            _root = RumorUIHelper.CreateSizedPanel(canvasRoot, "MonsterPanelUI",
                new Vector2(300f, 520f), new Vector2(-16f, -76f),
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                Color.white, null, new Vector2(1f, 1f));

            // Close button
            RumorUIHelper.CreateButton(_root.transform, "CloseBtn", "✕",
                new Vector2(36f, 32f), new Vector2(-12f, -12f),
                Hide, new Color(0.7f, 0.2f, 0.2f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            // Monster Name
            _nameText = RumorUIHelper.CreateText(_root.transform, "Name", "",
                new Vector2(16f, -18f), new Vector2(230f, 30f), 20,
                new Color(0.98f, 0.88f, 0.5f), TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Monster Sprite Preview
            _monsterPreview = RumorUIHelper.CreateImage(_root.transform, "Preview", null,
                new Vector2(64f, 64f), new Vector2(150f, -80f),
                new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Species & Level
            _speciesText = RumorUIHelper.CreateText(_root.transform, "Species", "",
                new Vector2(16f, -125f), new Vector2(260f, 24f), 15,
                new Color(0.85f, 0.82f, 0.95f), TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Combat stats
            _statsText = RumorUIHelper.CreateText(_root.transform, "Stats", "",
                new Vector2(16f, -155f), new Vector2(260f, 75f), 14,
                new Color(0.95f, 0.95f, 0.95f), TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Loyalty / Fatigue / Ambition status
            _statusText = RumorUIHelper.CreateText(_root.transform, "Status", "",
                new Vector2(16f, -235f), new Vector2(260f, 70f), 14,
                new Color(0.75f, 0.95f, 0.8f), TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Traits
            _traitsText = RumorUIHelper.CreateText(_root.transform, "Traits", "",
                new Vector2(16f, -310f), new Vector2(260f, 40f), 13,
                new Color(0.85f, 0.75f, 0.6f), TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            // Job Assignment Label
            RumorUIHelper.CreateText(_root.transform, "JobLabel", "임무 명령:",
                new Vector2(16f, -360f), new Vector2(120f, 24f), 15,
                new Color(0.98f, 0.88f, 0.5f), TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            CreateJobButtons();

            _root.SetActive(false);
        }

        private void CreateJobButtons()
        {
            var jobs = new[] {
                (JobType.Idle,     "대기"),
                (JobType.Mining,   "채광"),
                (JobType.Farming,  "농업"),
                (JobType.Training, "훈련"),
                (JobType.Resting,  "휴식"),
            };

            for (int i = 0; i < jobs.Length; i++)
            {
                var (job, label) = jobs[i];
                var j = job;
                float x = 16f + (i % 3) * 88f;
                float y = -390f - (i / 3) * 44f;

                RumorUIHelper.CreateButton(_root.transform, "Job_" + job, label,
                    new Vector2(80f, 36f), new Vector2(x, y),
                    () =>
                    {
                        if (_current != null)
                        {
                            _current.CurrentJob = j;
                            RefreshStatus();
                        }
                    },
                    Color.white,
                    new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            }
        }

        public void Show(MonsterData m)
        {
            if (m == null) return;
            _current = m;

            if (_nameText) _nameText.text = m.Name;
            if (_monsterPreview) _monsterPreview.sprite = SpriteRegistry.GetMonster(m.Species);
            if (_speciesText) _speciesText.text = $"{GetSpeciesKorean(m.Species)} (Lv.{m.Level}) • {m.Element}속성";
            RefreshStats();
            RefreshStatus();

            if (_traitsText)
                _traitsText.text = m.Traits.Count > 0
                    ? "보유 특성: " + string.Join(", ", m.Traits)
                    : "보유 특성: 없음";

            _root.SetActive(true);
        }

        public void Hide()
        {
            _root.SetActive(false);
            _current = null;
        }

        private void RefreshStats()
        {
            if (_statsText && _current != null)
            {
                _statsText.text =
                    $"  • 생명력(HP): <color=#ff7777>{_current.CurrentHP}</color> / {_current.MaxHP}\n" +
                    $"  • 공격력(ATK): <color=#ffaa55>{_current.ATK}</color>   방어력(DEF): <color=#77aaff>{_current.DEF}</color>\n" +
                    $"  • 이동속도: {_current.Speed}   세대: {_current.Generation}세대";
            }
        }

        private void RefreshStatus()
        {
            if (_statusText && _current != null)
            {
                _statusText.text =
                    $"  • 충성도: <color=#ffdf55>{_current.Loyalty}</color> / 100\n" +
                    $"  • 피로도: <color=#ff9977>{_current.Fatigue}</color> / 100\n" +
                    $"  • 현재 임무: <b><color=#88ffaa>{GetJobKorean(_current.CurrentJob)}</color></b>";
            }
        }

        private static string GetSpeciesKorean(MonsterSpecies s) => s switch
        {
            MonsterSpecies.ImpScrap     => "잡동사니 임프",
            MonsterSpecies.MireSlime    => "진흙 슬라임",
            MonsterSpecies.BonePup      => "뼈다귀 사냥개",
            MonsterSpecies.GrimShadow   => "그림자 망령",
            MonsterSpecies.GoblinTinker => "고블린 땜장이",
            MonsterSpecies.RumorBeast   => "소문 괴수",
            _                           => s.ToString()
        };

        private static string GetJobKorean(JobType j) => j switch
        {
            JobType.Idle     => "대기 중",
            JobType.Mining   => "광산 채광",
            JobType.Farming  => "버섯 재배",
            JobType.Training => "전투 훈련",
            JobType.Resting  => "숙소 휴식",
            JobType.Guarding => "성곽 경비",
            _                => j.ToString()
        };
    }
}
