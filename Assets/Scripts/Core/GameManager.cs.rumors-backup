using System;
using UnityEngine;
using Rumors.Buildings;
using Rumors.Monsters;
using Rumors.World;
using Rumors.Core;

namespace Rumors.Core
{
    /// <summary>
    /// Orchestrates daily settlement and delegates to subsystems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private bool pauseAfterDay = true;

        public event Action<DailyReportData> OnDaySettled;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (TimeManager.Instance != null)
                TimeManager.Instance.OnDayEnd += HandleDayEnd;
        }

        private void OnDestroy()
        {
            if (TimeManager.Instance != null)
                TimeManager.Instance.OnDayEnd -= HandleDayEnd;
        }

        private void HandleDayEnd(int day)
        {
            var report = new DailyReportData { Day = day };

            BuildingManager.Instance?.ProcessDaily(report);
            MonsterManager.Instance?.ProcessDaily(report);
            HeroManager.Instance?.CheckSpawn(DemonLordManager.Instance?.Fame ?? 0, day);

            if (day % 30 == 0) RumorManager.Instance?.Generate(RumorType.Exaggeration);

            ResourceManager.Instance?.ResetDailyTracker();

            if (pauseAfterDay) TimeManager.Instance?.Pause();

            OnDaySettled?.Invoke(report);
            Debug.Log($"[Game] Day {day} settled. Gold earned={report.GoldEarned}, Food={report.FoodProduced - report.FoodConsumed}");
        }
    }
}
