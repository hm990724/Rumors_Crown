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
