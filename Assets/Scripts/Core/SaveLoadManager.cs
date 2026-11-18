using System.IO;
using UnityEngine;
using Rumors.Core;
using Rumors.Buildings;
using Rumors.Monsters;

namespace Rumors.Core
{
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance { get; private set; }

        private string SavePath => Path.Combine(Application.persistentDataPath, "rumors_save.json");

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void SaveGame() => Save();
        public bool LoadGame() => Load();

        public void Save()
        {
            var data = new GameSaveData
            {
                CurrentDay   = TimeManager.Instance?.CurrentDay   ?? 1,
                CurrentMonth = TimeManager.Instance?.CurrentMonth ?? 1,
                CurrentYear  = TimeManager.Instance?.CurrentYear  ?? 1,
                Gold = ResourceManager.Instance?.Gold ?? 0,
                Mana = ResourceManager.Instance?.Mana ?? 0,
                Food = ResourceManager.Instance?.Food ?? 0,
                Ore  = ResourceManager.Instance?.Ore  ?? 0,
                RumorStack = RumorManager.Instance?.TotalStack ?? 0,
                DemonFame  = DemonLordManager.Instance?.Fame    ?? 0,
            };

            if (BuildingManager.Instance != null)
                foreach (var kv in BuildingManager.Instance.Placed)
                {
                    var b = kv.Value;
                    data.Buildings.Add(new BuildingSaveItem
                    {
                        Type = (int)b.Type, GridX=b.GridPos.x, GridY=b.GridPos.y,
                        Level=b.Level, IsConstructed=b.IsBuilt, DaysUntilBuilt=b.DaysLeft,
                        WorkerMonsterIds = new System.Collections.Generic.List<string>(b.WorkerIds),
                    });
                }

            if (MonsterManager.Instance != null)
                foreach (var m in MonsterManager.Instance.Monsters)
                {
                    var ctrl = MonsterManager.Instance.GetAgent(m.Id);
                    var pos  = ctrl != null ? ctrl.transform.position : Vector3.zero;
                    data.Monsters.Add(new MonsterSaveItem
                    {
                        Id=m.Id, Name=m.Name, Species=(int)m.Species, Element=(int)m.Element,
                        Age=m.Age, Generation=m.Generation, Level=m.Level, Exp=m.Exp,
                        MaxHP=m.MaxHP, CurrentHP=m.CurrentHP, ATK=m.ATK, DEF=m.DEF, Speed=m.Speed,
                        Personality=m.Personality, Loyalty=m.Loyalty, Fatigue=m.Fatigue,
                        Ambition=m.Ambition, Fame=m.Fame,
                        Traits = new System.Collections.Generic.List<string>(m.Traits),
                        CurrentJob=((int)m.CurrentJob),
                        AssignedBuildingX = m.AssignedBuildingPos?.x ?? -1,
                        AssignedBuildingY = m.AssignedBuildingPos?.y ?? -1,
                        WorldX=pos.x, WorldY=pos.y,
                    });
                }

            if (Rumors.Overhaul.RumorsWorldDirector.Instance != null)
                data.RumorsOverhaulJson = Rumors.Overhaul.RumorsWorldDirector.Instance.ExportJson();
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[Save] 저장 완료: {SavePath}");
        }

        public bool Load()
        {
            if (!File.Exists(SavePath)) { Debug.LogWarning("[Save] 세이브 파일 없음"); return false; }
            var data = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(SavePath));
            if (data == null) return false;

            TimeManager.Instance?.SetDate(data.CurrentDay, data.CurrentMonth, data.CurrentYear);
            ResourceManager.Instance?.SetAll(data.Gold, data.Mana, data.Food, data.Ore);
            RumorManager.Instance?.SetStack(data.RumorStack);

            if (BuildingManager.Instance != null)
            {
                var keys = new System.Collections.Generic.List<Vector2Int>(BuildingManager.Instance.Placed.Keys);
                foreach (var k in keys) BuildingManager.Instance.Demolish(k);
                foreach (var bs in data.Buildings)
                {
                    var gp=new Vector2Int(bs.GridX,bs.GridY);
                    var building=BuildingManager.Instance.GetAt(gp);
                    if(building==null || building.Type!=(BuildingType)bs.Type)
                        building=BuildingManager.Instance.PlaceInternal((BuildingType)bs.Type,gp,bs.IsConstructed);
                    building.SetLevel(bs.Level);
                    building.IsBuilt=bs.IsConstructed;
                    building.DaysLeft=bs.DaysUntilBuilt;
                    building.Visual?.SetBuilt(bs.IsConstructed);
                }
            }

            if (MonsterManager.Instance != null)
            {
                MonsterManager.Instance.ClearAll();
                foreach (var ms in data.Monsters)
                {
                    var m = new MonsterData(ms.Name,(MonsterSpecies)ms.Species,(MonsterElement)ms.Element,
                        ms.MaxHP, ms.ATK, ms.DEF, ms.Speed, ms.Personality, ms.Loyalty, ms.Ambition, ms.Fame)
                    {
                        Age=ms.Age, Generation=ms.Generation, Level=ms.Level, Exp=ms.Exp,
                        CurrentHP=ms.CurrentHP, Fatigue=ms.Fatigue,
                        CurrentJob=(JobType)ms.CurrentJob,
                        AssignedBuildingPos = ms.AssignedBuildingX>=0
                            ? new Vector2Int(ms.AssignedBuildingX, ms.AssignedBuildingY)
                            : (Vector2Int?)null,
                    };
                    m.RestoreId(ms.Id);
                    m.Traits.AddRange(ms.Traits);
                    MonsterManager.Instance.Spawn(m, new Vector3(ms.WorldX, ms.WorldY));
                }
            }

            // Reattach workers by their stable IDs after both buildings and monsters exist.
            if (BuildingManager.Instance != null && MonsterManager.Instance != null)
                foreach (var bs in data.Buildings)
                {
                    var b=BuildingManager.Instance.GetAt(new Vector2Int(bs.GridX,bs.GridY));
                    if(b==null)continue;
                    foreach(var id in bs.WorkerMonsterIds)
                    {
                        var m=MonsterManager.Instance.GetById(id);
                        if(m!=null)BuildingManager.Instance.AssignWorker(b,m);
                    }
                }
            DemonLordManager.Instance?.SetFame(data.DemonFame);
            if (Rumors.Overhaul.RumorsWorldDirector.Instance != null)
                Rumors.Overhaul.RumorsWorldDirector.Instance.ImportJson(data.RumorsOverhaulJson);
            Debug.Log("[Save] 불러오기 완료");
            return true;
        }
    }
}
