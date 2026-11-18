using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Monsters
{
    public class MonsterManager : MonoBehaviour
    {
        public static MonsterManager Instance { get; private set; }

        private readonly List<MonsterData> _monsters = new List<MonsterData>();
        private readonly Dictionary<string, MonsterAgent> _agents = new Dictionary<string, MonsterAgent>();
        private Transform _container;

        public IReadOnlyList<MonsterData> Monsters => _monsters;
        public IReadOnlyList<MonsterData> AllMonsters => _monsters;

        public event Action<MonsterData> OnMonsterAdded;
        public event Action<MonsterData> OnMonsterUpdated;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _container = new GameObject("MonstersContainer").transform;
            _container.SetParent(transform);
        }

        private void Start()
        {
            if (_monsters.Count == 0) SpawnStarters();
        }

        // ── Starter Pack ───────────────────────────────────────────────────
        private void SpawnStarters()
        {
            Spawn(new MonsterData("불꽃 꼬마 임프", MonsterSpecies.ImpScrap,     MonsterElement.Fire,
                45, 12, 4, 11, "호기심왕", 85, 40, 10));
            Spawn(new MonsterData("늪의 슬라임",   MonsterSpecies.MireSlime,    MonsterElement.Poison,
                60, 8,  8, 6,  "느긋함",   78, 25, 5));
            Spawn(new MonsterData("뼈의 강아지",   MonsterSpecies.BonePup,      MonsterElement.Undead,
                50, 14, 5, 13, "충직함",   92, 50, 15));
            Spawn(new MonsterData("어둠의 망령",   MonsterSpecies.GrimShadow,   MonsterElement.Darkness,
                35, 15, 3, 10, "냉소적",   80, 60, 20));
            Spawn(new MonsterData("발명왕 고블린", MonsterSpecies.GoblinTinker, MonsterElement.Physical,
                42, 10, 6, 9,  "탐구심",   82, 45, 12));
        }

        // ── Spawn ──────────────────────────────────────────────────────────
        public void Spawn(MonsterData data, Vector3? worldPos = null)
        {
            _monsters.Add(data);

            Vector3 pos = worldPos ?? GetDefaultSpawnPos(data);
            var go = new GameObject($"Monster_{data.Name}");
            go.transform.SetParent(_container);

            var agent = go.AddComponent<MonsterAgent>();
            agent.Init(data, pos);
            _agents[data.Id] = agent;

            OnMonsterAdded?.Invoke(data);
        }

        private Vector3 GetDefaultSpawnPos(MonsterData data)
        {
            if (Rumors.World.WorldGrid.Instance == null) return Vector3.zero;
            var center = Rumors.World.WorldGrid.Instance.GridToWorld(Rumors.World.WorldGrid.CastleGridPos);
            return center + new Vector3(
                UnityEngine.Random.Range(-2f, 2f),
                UnityEngine.Random.Range(-2f, 2f), 0f);
        }

        // ── Queries ────────────────────────────────────────────────────────
        public MonsterData      GetById(string id)   => _monsters.Find(m => m.Id == id);
        public MonsterAgent     GetAgent(string id)  { _agents.TryGetValue(id, out var a); return a; }
        public List<MonsterData>GetIdle()            => _monsters.FindAll(m => m.CurrentJob == JobType.Idle);

        // ── Daily ──────────────────────────────────────────────────────────
        public void ProcessDaily(DailyReportData report)
        {
            int need = _monsters.Count;
            var rm   = ResourceManager.Instance;
            bool fed = rm != null && rm.Food >= need;

            if (fed)        rm.Spend(0, 0, need, 0, true);
            else if (rm != null) { rm.Spend(0, 0, rm.Food, 0, true); }

            report.FoodConsumed += fed ? need : (rm?.Food ?? 0);
            if (!fed) report.HadFoodShortage = true;

            foreach (var m in _monsters)
            {
                m.ProcessDailyTick(fed, report);
                OnMonsterUpdated?.Invoke(m);
            }
        }

        public void ClearAll()
        {
            foreach (var kv in _agents)
                if (kv.Value) Destroy(kv.Value.gameObject);
            _agents.Clear();
            _monsters.Clear();
        }
    }
}
