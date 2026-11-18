using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;
using Rumors.World;

namespace Rumors.Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        private readonly Dictionary<Vector2Int, Building> _placed = new Dictionary<Vector2Int, Building>();
        private Transform _container;

        public IReadOnlyDictionary<Vector2Int, Building> Placed => _placed;

        public event Action<Building> OnPlaced;
        public event Action<Building> OnBuilt;
        public event Action<Building> OnDemolished;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _container = new GameObject("BuildingsRoot").transform;
            _container.SetParent(transform);
        }

        private void Start()
        {
            // Place castle core at start
            if (!_placed.ContainsKey(WorldGrid.CastleGridPos))
                PlaceInternal(BuildingType.CastleCore, WorldGrid.CastleGridPos, true);
        }

        // ── Placement ──────────────────────────────────────────────────────
        public bool TryPlace(BuildingType type, Vector2Int gp)
        {
            if (!WorldGrid.Instance.IsBuildable(gp))
            { Debug.LogWarning($"[BM] Cannot build at {gp}"); return false; }

            var data = BuildingData.Get(type);
            if (data == null) return false;

            var rm = ResourceManager.Instance;
            if (rm == null || !rm.CanAfford(data.GoldCost, data.ManaCost, 0, data.OreCost))
            { Debug.LogWarning($"[BM] Not enough resources for {data.DisplayName}"); return false; }

            rm.Spend(data.GoldCost, data.ManaCost, 0, data.OreCost);
            bool instant = data.ConstructionDays <= 0;
            PlaceInternal(type, gp, instant);
            return true;
        }

        public Building PlaceInternal(BuildingType type, Vector2Int gp, bool built)
        {
            if (_placed.ContainsKey(gp)) Demolish(gp);

            var b = new Building(type, gp, built);
            _placed[gp] = b;
            WorldGrid.Instance.SetBuilding(gp, b);

            var wp = WorldGrid.Instance.GridToWorld(gp);
            var go = new GameObject($"B_{type}_{gp.x}_{gp.y}");
            go.transform.SetParent(_container);
            go.transform.position = wp;

            go.AddComponent<SpriteRenderer>();
            var vis = go.AddComponent<BuildingVisual>();
            vis.Init(type, built, 1);
            b.Visual  = vis;
            b.WorldGO = go;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = type == BuildingType.CastleCore ? new Vector2(3f, 2.5f) : Vector2.one;
            if (type == BuildingType.CastleCore) col.offset = new Vector2(0f, 0.2f);
            col.isTrigger = true;

            var rec = go.AddComponent<BuildingReceiver>();
            rec.Set(b);

            OnPlaced?.Invoke(b);
            return b;
        }

        public bool Demolish(Vector2Int gp)
        {
            if (!_placed.TryGetValue(gp, out var b)) return false;
            if (b.Type == BuildingType.CastleCore) return false;

            // Partial refund
            var data = b.Data;
            if (data != null && ResourceManager.Instance != null)
                ResourceManager.Instance.Add(data.GoldCost / 3, 0, 0, data.OreCost / 3);

            WorldGrid.Instance.ClearBuilding(gp);
            _placed.Remove(gp);
            if (b.WorldGO) Destroy(b.WorldGO);
            OnDemolished?.Invoke(b);
            return true;
        }

        public Building GetAt(Vector2Int gp)
        {
            _placed.TryGetValue(gp, out var b);
            return b;
        }

        // ── Worker Assignment ──────────────────────────────────────────────
        public bool AssignWorker(Building b, Rumors.Core.MonsterData m)
        {
            if (b == null || m == null || !b.CanAddWorker()) return false;
            if (m.AssignedBuildingPos.HasValue)
                UnassignWorker(GetAt(m.AssignedBuildingPos.Value), m);

            if (!b.AddWorker(m.Id)) return false;
            m.AssignedBuildingPos = b.GridPos;
            m.CurrentJob = b.Type switch
            {
                BuildingType.Mine           => JobType.Mining,
                BuildingType.MushroomFarm   => JobType.Farming,
                BuildingType.TrainingGround => JobType.Training,
                BuildingType.ManaTower      => JobType.Researching,
                BuildingType.RumorArchive   => JobType.Researching,
                BuildingType.Dormitory      => JobType.Resting,
                BuildingType.Barracks       => JobType.Guarding,
                _                           => JobType.Idle,
            };
            return true;
        }

        public bool UnassignWorker(Building b, Rumors.Core.MonsterData m)
        {
            if (b == null || m == null || !b.RemoveWorker(m.Id)) return false;
            m.AssignedBuildingPos = null;
            m.CurrentJob = JobType.Idle;
            return true;
        }

        // ── Daily Production ───────────────────────────────────────────────
        public void ProcessDaily(DailyReportData report)
        {
            int totalRumor = 0;
            var rm = ResourceManager.Instance;

            foreach (var kv in _placed)
            {
                var b = kv.Value;

                // Construction tick
                if (!b.IsBuilt)
                {
                    bool done = b.TickConstruct();
                    if (done)
                    {
                        report.EventLogs.Add($"[건설 완료] {b.Data?.DisplayName ?? b.Type.ToString()}!");
                        OnBuilt?.Invoke(b);
                    }
                    continue;
                }

                if (rm == null) continue;

                // Upkeep
                int upkeep = b.DailyUpkeep();
                if (upkeep > 0) { rm.Spend(upkeep, 0, 0, 0, true); report.GoldSpent += upkeep; }

                // Production
                int gp = b.DailyGold();
                if (gp > 0) { rm.Add(gp, 0, 0, 0, true); report.GoldEarned += gp; }

                int fp = b.DailyFood();
                if (fp > 0) { rm.Add(0, 0, fp, 0, true); report.FoodProduced += fp; }

                int mp = b.DailyMana();
                if (mp > 0) { rm.Add(0, mp, 0, 0, true); report.ManaEarned += mp; }

                int op = b.DailyOre();
                if (op > 0) { rm.Add(0, 0, 0, op, true); report.OreEarned += op; }

                totalRumor += b.DailyRumor();

                // Dormitory: restore fatigue
                if (b.IsDormitory && Rumors.Monsters.MonsterManager.Instance != null)
                {
                    foreach (var mid in b.WorkerIds)
                    {
                        var m = Rumors.Monsters.MonsterManager.Instance.GetById(mid);
                        if (m != null)
                        {
                            m.Fatigue  = Mathf.Clamp(m.Fatigue  - 20, 0, 100);
                            m.Loyalty  = Mathf.Clamp(m.Loyalty  + 2,  0, 100);
                        }
                    }
                }

                if (b.Type == BuildingType.TrainingGround)
                    report.TrainingCount += b.WorkerIds.Count;

                report.ActiveWorkers += b.WorkerIds.Count;
            }

            if (totalRumor > 0)
            {
                report.RumorStackGained = totalRumor;
                Rumors.Core.RumorManager.Instance?.AddStack(totalRumor);
            }

            // Warehouse bonus (applied on first place, done here for save/load)
        }
    }
}
