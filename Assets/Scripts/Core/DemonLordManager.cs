using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    public class DemonLordManager : MonoBehaviour
    {
        public static DemonLordManager Instance { get; private set; }

        public string Name      { get; private set; } = "마왕 각하";
        public int    Authority { get; private set; } = 50;
        public int    Fame      { get; private set; } = 10;
        public int    Power     { get; private set; } = 100;
        public int    MaxHP     { get; private set; } = 500;
        public int    CurrentHP { get; private set; } = 500;

        public List<string> Skills { get; } = new List<string>();

        public event Action<int> OnFameChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }


        // Ruler class influences REAL game state, including defensive power.
        public int RulerKind { get; private set; }
        public void SetAuthority(int amount) => Authority = Mathf.Clamp(amount,0,100);
        public void SetFame(int amount) { Fame = Mathf.Max(0,amount); OnFameChanged?.Invoke(Fame); }
        public void ConfigureArchetype(int kind)
        {
            RulerKind = kind;
            switch(kind)
            {
                case 1: // Black Flame: authority & military strength; greater external threat handled by world director
                    Name = "흑염 군주"; Authority = 84; Power = 165; MaxHP = 720; break;
                case 2: // Blood Moon: rumor/trade power, fragile defense
                    Name = "혈월 여왕"; Authority = 57; Power = 83; MaxHP = 390; break;
                case 3: // Wraith: magically oriented ruler
                    Name = "망령 군주"; Authority = 65; Power = 104; MaxHP = 500; break;
                default: return;
            }
            CurrentHP = MaxHP;
        }

        public void GainFame(int v)
        {
            Fame += v;
            OnFameChanged?.Invoke(Fame);
        }

        public void GainAuthority(int v, string reason)
        {
            Authority = Mathf.Clamp(Authority + v, 0, 100);
            Debug.Log($"[Demon] 권위 +{v} ({reason}) → {Authority}");
        }

        public void LoseAuthority(int v, string reason)
        {
            Authority = Mathf.Clamp(Authority - v, 0, 100);
            Debug.Log($"[Demon] 권위 -{v} ({reason}) → {Authority}");
        }
    }
}
