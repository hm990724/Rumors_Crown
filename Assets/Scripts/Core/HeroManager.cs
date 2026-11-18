using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    public class HeroManager : MonoBehaviour
    {
        public static HeroManager Instance { get; private set; }

        private readonly List<HeroData> _heroes = new List<HeroData>();
        private int _nextRaidDay = 8;
        public IReadOnlyList<HeroData> Heroes => _heroes;

        public event Action<HeroData> OnHeroInvading;

        private static readonly string[] Names = {
            "빛의 기사 카엘","그림자 사냥꾼 레이나","성스러운 창 테오도르","복수의 불꽃 아이리","전설의 검사 오렌",
        };
        private static readonly string[] Jobs = {"기사","사냥꾼","마법사","승려","도적"};

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void CheckSpawn(int demonFame, int day)
        {
            if (day < _nextRaidDay) return;
            _nextRaidDay = day + UnityEngine.Random.Range(7,11);

            var h = new HeroData
            {
                Id              = Guid.NewGuid().ToString("N"),
                Name            = Names[UnityEngine.Random.Range(0, Names.Length)],
                Age             = UnityEngine.Random.Range(18, 45),
                Job             = Jobs[UnityEngine.Random.Range(0, Jobs.Length)],
                Power           = Mathf.Max(30, demonFame / 2 + UnityEngine.Random.Range(-10, 20)),
                Fame            = UnityEngine.Random.Range(5, 30),
                DungeonKnowledge= UnityEngine.Random.Range(0, 50),
                IsAlive         = true,
            };
            _heroes.Add(h);
            OnHeroInvading?.Invoke(h);
            Debug.Log($"[Hero] 침입! {h.Name} (전투력: {h.Power})");
        }
    }
}
