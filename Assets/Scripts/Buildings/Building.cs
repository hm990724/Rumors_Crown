using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Buildings
{
    /// <summary>
    /// Runtime data for a placed building instance.
    /// </summary>
    public class Building
    {
        public BuildingType   Type          { get; }
        public BuildingData   Data          => BuildingData.Get(Type);
        public Vector2Int     GridPos       { get; }
        public int            Level         { get; set; } = 1;
        public bool           IsBuilt       { get; set; }
        public int            DaysLeft      { get; set; }

        public List<string>   WorkerIds     { get; } = new List<string>();
        public GameObject     WorldGO       { get; set; }
        public BuildingVisual Visual        { get; set; }

        public Building(BuildingType t, Vector2Int gp, bool built = true)
        {
            Type = t; GridPos = gp;
            IsBuilt  = built;
            DaysLeft = built ? 0 : (Data?.ConstructionDays ?? 1);
        }

        public bool CanAddWorker() => IsBuilt && Data != null && WorkerIds.Count < Data.MaxWorkers;

        public bool AddWorker(string id)
        {
            if (!CanAddWorker() || WorkerIds.Contains(id)) return false;
            WorkerIds.Add(id); return true;
        }

        public bool RemoveWorker(string id) => WorkerIds.Remove(id);

        /// <summary>Returns true when construction completes.</summary>
        public bool TickConstruct()
        {
            if (IsBuilt) return true;
            DaysLeft--;
            if (DaysLeft > 0) return false;
            IsBuilt = true; DaysLeft = 0;
            Visual?.SetBuilt(true);
            return true;
        }

        public int DailyUpkeep()   => IsBuilt && Data != null ? Data.DailyUpkeep * Level : 0;
        public int DailyGold()     => IsBuilt && Data != null ? Data.GoldPerWorker * WorkerIds.Count * Level : 0;
        public int DailyFood()     => IsBuilt && Data != null ? Data.FoodPerWorker * WorkerIds.Count * Level : 0;
        public int DailyMana()     => IsBuilt && Data != null ? Data.ManaPerWorker * WorkerIds.Count * Level : 0;
        public int DailyOre()      => IsBuilt && Data != null ? Data.OrePerWorker  * WorkerIds.Count * Level : 0;
        public int DailyRumor()
        {
            if (!IsBuilt || Data == null) return 0;
            int bonus = (Type == BuildingType.RumorArchive) ? WorkerIds.Count * 2 : 0;
            return (Data.RumorPerDay + bonus) * Level;
        }

        // Dormitory restores fatigue
        public bool IsDormitory => Type == BuildingType.Dormitory;

        public void Upgrade()
        {
            Level++;
            Visual?.SetLevel(Level);
        }

        public void SetLevel(int level)
        {
            Level = level;
            Visual?.SetLevel(Level);
        }
    }

    // ── Visual component ─────────────────────────────────────────────────────
    public enum BuildVisualState { UnderConstruction, Built, Upgraded }

    [RequireComponent(typeof(SpriteRenderer))]
    public class BuildingVisual : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private BuildingType   _type;
        private int            _level;
        private bool           _blinking;
        private float          _blinkT;

        public BuildVisualState State { get; private set; }

        public void Init(BuildingType t, bool built, int level)
        {
            _type  = t;
            _level = level;
            _sr    = GetComponent<SpriteRenderer>();
            _sr.sortingOrder = 10;
            if (SpriteRegistry.DefaultMaterial != null)
                _sr.material = SpriteRegistry.DefaultMaterial;

            if (t == BuildingType.CastleCore)
            {
                _sr.sortingOrder = 15;
                // Scale up the Grand Citadel to 1.4x for imposing architectural presence (~4.2x4.2 tiles)
                transform.localScale = new Vector3(1.4f, 1.4f, 1f);
                // Slight upward offset so the gate perfectly meets the courtyard cobblestones
                transform.position += new Vector3(0f, 0.45f, 0f);
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            SetBuilt(built);
        }

        public void SetLevel(int level)
        {
            _level = level;
            SetBuilt(State != BuildVisualState.UnderConstruction);
        }

        public void SetBuilt(bool built)
        {
            bool wasUnbuilt = State == BuildVisualState.UnderConstruction;
            State    = !built ? BuildVisualState.UnderConstruction
                     : _level >= 2 ? BuildVisualState.Upgraded
                     : BuildVisualState.Built;
            _blinking = !built;
            Refresh();
            if (built && wasUnbuilt) PlayConstructEffect();
        }

        public void PlayConstructEffect()
        {
            StartCoroutine(PopScaleCR());
        }

        private System.Collections.IEnumerator PopScaleCR()
        {
            var baseScale = transform.localScale;
            float t = 0f;
            while (t < 0.25f)
            {
                t += Time.deltaTime;
                float s = 1f + Mathf.Sin(t / 0.25f * Mathf.PI) * 0.2f;
                transform.localScale = baseScale * s;
                yield return null;
            }
            transform.localScale = baseScale;
        }

        private void Refresh()
        {
            if (_sr == null) return;
            bool built = State != BuildVisualState.UnderConstruction;
            _sr.sprite = SpriteRegistry.GetBuilding(_type, built, _level);
            if (!_blinking) _sr.color = Color.white;
        }

        private void Update()
        {
            if (!_blinking || _sr == null) return;
            _blinkT += Time.deltaTime;
            float a = Mathf.Lerp(0.5f, 1f, (Mathf.Sin(_blinkT * 3f) + 1f) * 0.5f);
            _sr.color = new Color(1f, 0.85f, 0.4f, a);
        }
    }

    // ── Click receiver ───────────────────────────────────────────────────────
    public class BuildingReceiver : MonoBehaviour
    {
        public Building Building { get; private set; }
        public void Set(Building b) { Building = b; }
    }
}
