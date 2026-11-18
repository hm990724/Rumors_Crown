using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rumors.Core
{
    // ─── Building ───────────────────────────────────────────────────────────
    public enum BuildingType
    {
        None = 0,
        // Tier 1
        CastleCore      = 1,
        Mine            = 2,
        MushroomFarm    = 3,
        Dormitory       = 4,
        Warehouse       = 5,
        // Tier 2
        ManaTower       = 6,
        TrainingGround  = 7,
        RumorArchive    = 8,
        MobExtractor    = 9,
        // Tier 3+
        BreedingLab     = 10,
        Hatchery        = 11,
        TrapWorkshop    = 12,
        DemonAltar      = 13,
        Barracks        = 14,
        FactionCouncil  = 15,
    }

    public enum BuildingCategory { Production, Military, Research, Magic, Storage, Special }

    // ─── Tile ────────────────────────────────────────────────────────────────
    public enum TileType
    {
        DarkGrass = 0, Dirt = 1, Stone = 2, Swamp = 3,
        CastleFloor = 4, Cobblestone = 5,
        Forest = 6, DeepForest = 7, MineralVein = 8,
        SwampDeep = 9, Wasteland = 10, CastleWall = 11,
        Road = 12,
    }

    // ─── Monster ─────────────────────────────────────────────────────────────
    public enum MonsterSpecies
    {
        ImpScrap = 0, MireSlime = 1, BonePup = 2,
        GrimShadow = 3, GoblinTinker = 4, RumorBeast = 5
    }

    public enum MonsterElement { Physical=0, Fire=1, Darkness=2, Poison=3, Undead=4, Arcane=5 }

    public enum JobType
    {
        Idle=0, Mining=1, Farming=2, Training=3,
        Researching=4, Guarding=5, Resting=6, Building=7, Patrolling=8
    }

    public enum MonsterAIState { Idle, MovingToWork, Working, MovingToRest, Resting, Wandering }

    // ─── Rumor ───────────────────────────────────────────────────────────────
    public enum RumorType { Truth=0, Exaggeration=1, Misunderstanding=2, Fabrication=3, Prophecy=4 }

    // ─── Game Speed ──────────────────────────────────────────────────────────
    public enum GameSpeed { Paused=0, Normal=1, Fast=2, UltraFast=4 }

    // ─── Faction ─────────────────────────────────────────────────────────────
    public enum FactionId { None=0, GoblinUnion=1, DragonBloodNobility=2, UndeadOrder=3, BeastTribe=4, SpiritCouncil=5 }

    // ─── Data Records ────────────────────────────────────────────────────────
    [Serializable]
    public class RumorData
    {
        public string Id;
        public string Text;
        public string Title { get => Text; set => Text = value; }
        public RumorType Type;
        public int Stack;
        public int RequiredStack;
        public MonsterSpecies TargetSpecies;
        public string Location;
        public string Ability;
        public string Weakness;
        public int DangerRating;
        public bool CanMaterialize => Stack >= RequiredStack;
    }

    [Serializable]
    public class HeroData
    {
        public string Id;
        public string Name;
        public int Age;
        public string Job;
        public int Power;
        public int Fame;
        public string Personality;
        public List<string> Equipment = new List<string>();
        public int DungeonKnowledge;
        public int InvasionCount;
        public bool IsAlive = true;
    }

    [Serializable]
    public class DailyReportData
    {
        public int Day;
        public int GoldEarned;
        public int GoldSpent;
        public int ManaEarned;
        public int FoodProduced;
        public int FoodConsumed;
        public int OreEarned;
        public int RumorStackGained;
        public int NetGold => GoldEarned - GoldSpent;
        public int NetFood => FoodProduced - FoodConsumed;
        public int GoldDelta => NetGold;
        public int FoodDelta => NetFood;
        public int ManaDelta => ManaEarned;
        public int OreDelta => OreEarned;
        public int RumorStackDelta => RumorStackGained;
        public int ActiveWorkers;
        public int TrainingCount;
        public bool HadFoodShortage;
        public List<string> EventLogs = new List<string>();
        public List<string> Events => EventLogs;
    }

    // ─── Save Data ───────────────────────────────────────────────────────────
    [Serializable]
    public class MonsterSaveItem
    {
        public string Id, Name, Personality;
        public int Species, Element, Age, Generation, Level, Exp;
        public int MaxHP, CurrentHP, ATK, DEF, Speed;
        public int Loyalty, Fatigue, Ambition, Fame;
        public List<string> Traits = new List<string>();
        public int CurrentJob;
        public int AssignedBuildingX = -1, AssignedBuildingY = -1;
        public float WorldX, WorldY;
    }

    [Serializable]
    public class BuildingSaveItem
    {
        public int Type, GridX, GridY, Level;
        public bool IsConstructed;
        public int DaysUntilBuilt;
        public List<string> WorkerMonsterIds = new List<string>();
    }

    [Serializable]
    public class GameSaveData
    {
        public int CurrentDay=1, CurrentMonth=1, CurrentYear=1;
        public int Gold, Mana, Food, Ore;
        public int RumorStack, DemonFame;
        public string RumorsOverhaulJson;
        public List<BuildingSaveItem> Buildings = new List<BuildingSaveItem>();
        public List<MonsterSaveItem> Monsters   = new List<MonsterSaveItem>();
    }
}
