using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    public class RumorManager : MonoBehaviour
    {
        public static RumorManager Instance { get; private set; }

        private readonly List<RumorData> _rumors = new List<RumorData>();
        public IReadOnlyList<RumorData> Rumors => _rumors;
        public int TotalStack { get; private set; }

        public event Action<RumorData> OnRumorAdded;
        public event Action<int>       OnStackChanged;

        private static readonly string[] Locations  = {"북쪽 늪","서쪽 광산","동쪽 폐허","마왕성 근처 숲","인간 왕국 국경"};
        private static readonly string[] Abilities  = {"불꽃을 뿜는다","어둠을 삼킨다","돌을 녹인다","공중을 날아다닌다","독을 퍼뜨린다"};
        private static readonly string[] Weaknesses = {"빛에 약하다","물에 녹는다","소금에 약하다","불에 취약하다","신성한 힘에 무너진다"};
        private static readonly string[] Creatures  = {"괴물","거대한 짐승","어둠의 존재","전설의 몬스터","뼈의 괴물"};

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            Generate(RumorType.Truth);
            Generate(RumorType.Exaggeration);
        }

        public void Generate(RumorType type = RumorType.Truth)
        {
            var r = new RumorData
            {
                Id            = Guid.NewGuid().ToString("N"),
                Type          = type,
                Location      = Locations[UnityEngine.Random.Range(0, Locations.Length)],
                Ability       = Abilities[UnityEngine.Random.Range(0, Abilities.Length)],
                Weakness      = Weaknesses[UnityEngine.Random.Range(0, Weaknesses.Length)],
                Stack         = 0,
                RequiredStack = UnityEngine.Random.Range(25, 50),
                TargetSpecies = MonsterSpecies.RumorBeast,
                DangerRating  = UnityEngine.Random.Range(1, 5),
            };
            string creature = Creatures[UnityEngine.Random.Range(0, Creatures.Length)];
            string prefix   = type switch {
                RumorType.Exaggeration    => "[과장] ",
                RumorType.Fabrication     => "[날조] ",
                RumorType.Prophecy        => "[예언] ",
                RumorType.Misunderstanding=> "[오해] ",
                _ => ""
            };
            r.Text = $"{prefix}\"{r.Location}에는 {r.Ability}는 {creature}가 살며, {r.Weakness}.\"";
            _rumors.Add(r);
            OnRumorAdded?.Invoke(r);
        }

        public void AddStack(int amount)
        {
            TotalStack += amount;
            foreach (var r in _rumors)
                if (!r.CanMaterialize)
                    r.Stack = Mathf.Min(r.Stack + Mathf.Max(1, amount / Mathf.Max(1, _rumors.Count)), r.RequiredStack + 10);
            OnStackChanged?.Invoke(TotalStack);
        }

        public void SetStack(int v) { TotalStack = v; OnStackChanged?.Invoke(v); }

        public bool TryMaterialize(string id, out string desc)
        {
            var r = _rumors.Find(x => x.Id == id);
            if (r == null || !r.CanMaterialize) { desc="조건 미충족"; return false; }
            desc = $"소문이 실체화되었습니다: {r.Text}";
            _rumors.Remove(r);
            TotalStack -= r.RequiredStack;
            if (_rumors.Count < 2) Generate();
            return true;
        }
    }
}
