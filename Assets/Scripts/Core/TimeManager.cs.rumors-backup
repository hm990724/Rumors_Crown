using System;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [SerializeField] private float dayDurationSeconds = 16f;

        public int CurrentDay    { get; private set; } = 1;
        public int CurrentMonth  { get; private set; } = 1;
        public int CurrentYear   { get; private set; } = 1;
        public GameSpeed Speed   { get; private set; } = GameSpeed.Normal;
        public float DayProgress { get; private set; }
        public bool  IsPaused    => Speed == GameSpeed.Paused;

        private float _timer;

        public event Action<int, int, int> OnDateChanged;
        public event Action<float>          OnDayProgress;
        public event Action<int>            OnDayEnd;      // passes completedDay
        public event Action<GameSpeed>      OnSpeedChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            if (IsPaused) return;
            _timer += Time.deltaTime * (int)Speed;
            DayProgress = Mathf.Clamp01(_timer / dayDurationSeconds);
            OnDayProgress?.Invoke(DayProgress);
            if (_timer >= dayDurationSeconds) AdvanceDay();
        }

        public void SetSpeed(GameSpeed s)
        {
            Speed = s;
            OnSpeedChanged?.Invoke(s);
        }

        public void Pause()  => SetSpeed(GameSpeed.Paused);
        public void Resume() => SetSpeed(GameSpeed.Normal);

        public void AdvanceDayManually() => AdvanceDay();

        private void AdvanceDay()
        {
            _timer = 0f; DayProgress = 0f;
            int completed = CurrentDay;
            CurrentDay++;
            if (CurrentDay > 30) { CurrentDay = 1; CurrentMonth++; }
            if (CurrentMonth > 12) { CurrentMonth = 1; CurrentYear++; }
            OnDateChanged?.Invoke(CurrentDay, CurrentMonth, CurrentYear);
            OnDayEnd?.Invoke(completed);
        }

        public void SetDate(int d, int m, int y)
        {
            CurrentDay = d; CurrentMonth = m; CurrentYear = y;
            _timer = 0f; DayProgress = 0f;
            OnDateChanged?.Invoke(d, m, y);
        }
    }
}
