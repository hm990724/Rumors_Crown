using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Buildings
{
    // ─── BuildingData (ScriptableObject-like but plain class for now) ─────────
    public class BuildingData
    {
        public BuildingType     Type         { get; }
        public string           DisplayName  { get; }
        public string           Desc         { get; }
        public BuildingCategory Category     { get; }

        public int GoldCost      { get; }
        public int ManaCost      { get; }
        public int OreCost       { get; }
        public int DailyUpkeep   { get; }
        public int MaxWorkers    { get; }

        public int GoldPerWorker { get; }
        public int FoodPerWorker { get; }
        public int ManaPerWorker { get; }
        public int OrePerWorker  { get; }
        public int RumorPerDay   { get; }

        public int  ConstructionDays  { get; }
        public int  RequiredFame      { get; }   // 명성 해금 조건
        public bool IsInitiallyLocked { get; }

        // Build requirement description
        public string UnlockConditionDesc { get; }

        public BuildingData(BuildingType type, string name, string desc, BuildingCategory cat,
            int gold, int mana, int ore, int upkeep, int maxW,
            int gProd, int fProd, int mProd, int oProd, int rProd,
            int buildDays, int reqFame = 0, string unlockDesc = "")
        {
            Type=type; DisplayName=name; Desc=desc; Category=cat;
            GoldCost=gold; ManaCost=mana; OreCost=ore;
            DailyUpkeep=upkeep; MaxWorkers=maxW;
            GoldPerWorker=gProd; FoodPerWorker=fProd;
            ManaPerWorker=mProd; OrePerWorker=oProd; RumorPerDay=rProd;
            ConstructionDays=buildDays; RequiredFame=reqFame;
            IsInitiallyLocked = reqFame > 0;
            UnlockConditionDesc = string.IsNullOrEmpty(unlockDesc)
                ? (reqFame > 0 ? $"명성 {reqFame} 필요" : "") : unlockDesc;
        }

        // ── Static Catalogue ─────────────────────────────────────────────────
        public static readonly BuildingData CastleCore = new BuildingData(
            BuildingType.CastleCore, "마왕성 본관", "마왕성의 중심. 모든 시스템의 기반.", BuildingCategory.Special,
            0,0,0, 0, 0,  0,0,0,0, 2,  0, 0);

        public static readonly BuildingData Mine = new BuildingData(
            BuildingType.Mine, "어둠 광산", "광석과 금화를 채굴한다. 일꾼 1인당 금화 15/일.", BuildingCategory.Production,
            80,0,10, 4, 3,  15,0,0,2, 0,  2, 0);

        public static readonly BuildingData MushroomFarm = new BuildingData(
            BuildingType.MushroomFarm, "독버섯 농장", "발광 버섯을 재배해 식량을 생산한다. 일꾼 1인당 식량 5/일.", BuildingCategory.Production,
            60,20,0, 3, 3,  0,5,0,0, 0,  2, 0);

        public static readonly BuildingData Dormitory = new BuildingData(
            BuildingType.Dormitory, "몬스터 숙소", "피로를 회복한다. 최대 4마리 수용.", BuildingCategory.Production,
            70,0,5, 2, 4,  0,0,0,0, 0,  2, 0);

        public static readonly BuildingData Warehouse = new BuildingData(
            BuildingType.Warehouse, "창고", "자원 보관 한도 증가. (+500금화, +200식량, +200광석)", BuildingCategory.Storage,
            90,0,10, 2, 0,  0,0,0,0, 0,  2, 0);

        public static readonly BuildingData ManaTower = new BuildingData(
            BuildingType.ManaTower, "마력 정제소", "마력을 채취·정제한다. 일꾼 1인당 마력 8/일.", BuildingCategory.Magic,
            140,0,20, 5, 2,  0,0,8,0, 1,  3, 20, "명성 20 달성 필요");

        public static readonly BuildingData TrainingGround = new BuildingData(
            BuildingType.TrainingGround, "훈련장", "몬스터를 훈련시켜 경험치와 스탯을 올린다.", BuildingCategory.Military,
            110,30,0, 6, 2,  0,0,0,0, 0,  3, 0);

        public static readonly BuildingData RumorArchive = new BuildingData(
            BuildingType.RumorArchive, "소문 기록소", "소문을 분석·수집한다. 소문 스택 +3/일.", BuildingCategory.Research,
            160,50,0, 7, 2,  0,0,0,0, 5,  3, 20, "명성 20 달성 필요");

        public static readonly BuildingData MobExtractor = new BuildingData(
            BuildingType.MobExtractor, "몬스터 추출기", "소문을 소비해 새 몬스터를 실체화한다.", BuildingCategory.Magic,
            220,100,0, 10, 1,  0,0,0,0, 0,  4, 30, "명성 30 달성 필요");

        // ── Registry ─────────────────────────────────────────────────────────
        private static readonly Dictionary<BuildingType, BuildingData> _all =
            new Dictionary<BuildingType, BuildingData>
        {
            {BuildingType.CastleCore,     CastleCore},
            {BuildingType.Mine,           Mine},
            {BuildingType.MushroomFarm,   MushroomFarm},
            {BuildingType.Dormitory,      Dormitory},
            {BuildingType.Warehouse,      Warehouse},
            {BuildingType.ManaTower,      ManaTower},
            {BuildingType.TrainingGround, TrainingGround},
            {BuildingType.RumorArchive,   RumorArchive},
            {BuildingType.MobExtractor,   MobExtractor},
        };

        public static BuildingData Get(BuildingType t)
        {
            _all.TryGetValue(t, out var d);
            return d;
        }

        public static List<BuildingData> GetAvailable(int demonFame)
        {
            var list = new List<BuildingData>();
            foreach (var kv in _all)
            {
                if (kv.Key == BuildingType.CastleCore) continue;
                list.Add(kv.Value);
            }
            return list;  // All shown, locked ones display unlock condition
        }
    }
}
