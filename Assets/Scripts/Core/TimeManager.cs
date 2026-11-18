using System;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [SerializeField] private float dayDurationSeconds = 60f;

        public int CurrentDay    { get; private set; } = 1;
        public int CurrentMonth  { get; private set; } = 1;
        public int CurrentYear   { get; private set; } = 1;
        public GameSpeed Speed   { get; private set; } = GameSpeed.Normal;
        public float DayProgress { get; private set; }
        public bool  IsPaused    => Speed == GameSpeed.Paused;

        private float _timer;
        private GameSpeed _lastRunningSpeed = GameSpeed.Normal;

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
            if (Input.GetKeyDown(KeyCode.Space) && !IsTextInputFocused()) TogglePause();
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetSpeed(GameSpeed.Normal);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetSpeed(GameSpeed.Fast);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetSpeed(GameSpeed.UltraFast);
            if (IsPaused) return;
            _timer += Time.deltaTime * (int)Speed;
            DayProgress = Mathf.Clamp01(_timer / dayDurationSeconds);
            OnDayProgress?.Invoke(DayProgress);
            while (_timer >= dayDurationSeconds && !IsPaused) AdvanceDay();
        }

        public void SetSpeed(GameSpeed s)
        {
            if (s != GameSpeed.Paused) _lastRunningSpeed = s;
            Speed = s;
            OnSpeedChanged?.Invoke(s);
        }

        public void Pause()  => SetSpeed(GameSpeed.Paused);
        public void Resume() => SetSpeed(_lastRunningSpeed);
        public void TogglePause() { if(IsPaused) Resume(); else Pause(); }
        private static bool IsTextInputFocused()
        {
            var es=UnityEngine.EventSystems.EventSystem.current;
            if(es==null || es.currentSelectedGameObject==null) return false;
            var obj=es.currentSelectedGameObject;
            if(obj.GetComponent<UnityEngine.UI.InputField>()!=null) return true;
            foreach(var c in obj.GetComponents<MonoBehaviour>())
                if(c!=null && c.GetType().Name=="TMP_InputField") return true;
            return false;
        }

        public void AdvanceDayManually() => AdvanceDay();

        private void AdvanceDay()
        {
            _timer = Mathf.Max(0f,_timer-dayDurationSeconds); DayProgress = Mathf.Clamp01(_timer/dayDurationSeconds);
            int completed = CurrentDay;
            CurrentDay++;
            if (CurrentDay > 10) { CurrentDay = 1; CurrentMonth++; }
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
