using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    /// <summary>
    /// Individual monster entity. All logic lives here; visuals in MonsterAgent.
    /// </summary>
    public class MonsterData
    {
        public string         Id          { get; private set; }
        public string         Name        { get; set; }
        public MonsterSpecies Species     { get; }
        public MonsterElement Element     { get; set; }
        public string         Personality { get; set; }

        public int Age        { get; set; } = 1;
        public int Generation { get; set; } = 1;
        public int Level      { get; set; } = 1;
        public int Exp        { get; set; }

        public int MaxHP    { get; set; }
        public int CurrentHP{ get; set; }
        public int ATK      { get; set; }
        public int DEF      { get; set; }
        public int Speed    { get; set; }

        public int Loyalty  { get; set; }  // 0–100
        public int Fatigue  { get; set; }  // 0–100
        public int Ambition { get; set; }  // 0–100
        public int Fame     { get; set; }  // 0–100

        public FactionId Faction { get; set; } = FactionId.None;
        public List<string> Traits { get; } = new List<string>();

        public JobType        CurrentJob          { get; set; } = JobType.Idle;
        public Vector2Int?    AssignedBuildingPos { get; set; }

        // Constructor
        public MonsterData(string name, MonsterSpecies species, MonsterElement element,
            int hp, int atk, int def, int spd, string personality,
            int loyalty, int ambition, int fame)
        {
            Id          = Guid.NewGuid().ToString("N");
            Name        = name;
            Species     = species;
            Element     = element;
            MaxHP = CurrentHP = hp;
            ATK = atk; DEF = def; Speed = spd;
            Personality = personality;
            Loyalty     = Mathf.Clamp(loyalty,  0, 100);
            Ambition    = Mathf.Clamp(ambition, 0, 100);
            Fame        = Mathf.Clamp(fame,     0, 100);
        }

        public void RestoreId(string id)
        {
            if (!string.IsNullOrEmpty(id)) Id = id;
        }

        /// <summary>Called once per day by MonsterManager.</summary>
        public void ProcessDailyTick(bool hadFood, DailyReportData report)
        {
            if (!hadFood)
            {
                Loyalty  = Mathf.Clamp(Loyalty - 10, 0, 100);
                Fatigue  = Mathf.Clamp(Fatigue + 10, 0, 100);
                report.EventLogs.Add($"[굶주림] {Name}이 식량 부족으로 충성도 감소 ({Loyalty})");
                return;
            }

            switch (CurrentJob)
            {
                case JobType.Idle:
                    Fatigue  = Mathf.Clamp(Fatigue  - 15, 0, 100);
                    Loyalty  = Mathf.Clamp(Loyalty  + 1,  0, 100);
                    break;

                case JobType.Mining:
                case JobType.Farming:
                case JobType.Researching:
                    Fatigue = Mathf.Clamp(Fatigue + 15, 0, 100);
                    if (Fatigue >= 90)
                    {
                        Loyalty = Mathf.Clamp(Loyalty - 3, 0, 100);
                        report.EventLogs.Add($"[과로 경고] {Name} 피로 임계치 ({Fatigue})");
                    }
                    break;

                case JobType.Training:
                    Fatigue = Mathf.Clamp(Fatigue + 20, 0, 100);
                    Exp += 25;
                    int need = Level * 50;
                    if (Exp >= need)
                    {
                        Exp -= need; Level++;
                        ATK += 2; MaxHP += 6; DEF += 1; CurrentHP = MaxHP;
                        report.EventLogs.Add($"[레벨업] {Name} → Lv.{Level}! (ATK+2, HP+6)");
                    }
                    break;

                case JobType.Resting:
                case JobType.Guarding:
                    Fatigue  = Mathf.Clamp(Fatigue  - 25, 0, 100);
                    Loyalty  = Mathf.Clamp(Loyalty  + 2,  0, 100);
                    break;

                case JobType.Patrolling:
                    Fatigue = Mathf.Clamp(Fatigue + 8, 0, 100);
                    break;
            }

            // Rebellion check
            if (Ambition >= 70 && Loyalty <= 20 && Fame >= 40)
                report.EventLogs.Add($"[위험] {Name}의 야망이 임계치! 반란 위험.");
        }
    }
}
