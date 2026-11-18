using System;
using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;
using Rumors.Monsters;
using Rumors.Buildings;

namespace Rumors.Overhaul
{
    public enum RulerKind { Unchosen=0, BlackFlame=1, BloodMoon=2, Wraith=3 }
    [Serializable] public class VillageResident
    {
        public string Name, Role, LastHeard;
        public int Fear, Trust, Doubt;
    }
    [Serializable] public class RumorsOverhaulSave
    {
        public int Version=1;
        public int Ruler;
        public int Threat=8;
        public int SavedAuthority=-1;
        public int HumanTrust=43;
        public int BreedingCount;
        public int RaidWins;
        public int RaidLosses;
        public int LastDayProcessed=-1;
        public List<VillageResident> People=new List<VillageResident>();
        public List<string> Chronicle=new List<string>();
    }

    /// <summary>
    /// Companion simulation which uses the EXISTING managers as authoritative state.
    /// Avoids introducing a parallel economy. Save via existing GameSaveData (OverhaulJson field).
    /// </summary>
    public sealed class RumorsWorldDirector : MonoBehaviour
    {
        public static RumorsWorldDirector Instance {get;private set;}
        public RumorsOverhaulSave State {get;private set;} = new RumorsOverhaulSave();
        public event Action OnChanged;
        public event Action<string> OnNotice;
        private bool _hooked;
        private System.Random _rng=new System.Random(99117);

        private void Awake()
        {
            if(Instance!=null && Instance!=this){Destroy(gameObject);return;}
            Instance=this;
            EnsurePeople();
        }
        private void Start(){Hook();}
        private void Update(){if(!_hooked)Hook();}
        private void Hook()
        {
            if(_hooked || TimeManager.Instance==null)return;
            TimeManager.Instance.OnDayEnd+=OnDayEnd;
            if(HeroManager.Instance!=null) HeroManager.Instance.OnHeroInvading+=OnRaid;
            _hooked=true;
        }
        private void OnDestroy()
        {
            if(TimeManager.Instance!=null)TimeManager.Instance.OnDayEnd-=OnDayEnd;
            if(HeroManager.Instance!=null)HeroManager.Instance.OnHeroInvading-=OnRaid;
            if(Instance==this)Instance=null;
        }
        private void EnsurePeople()
        {
            if(State.People==null)State.People=new List<VillageResident>();
            if(State.People.Count>0)return;
            State.People.Add(new VillageResident{Name="여관 주인 미라",Role="여관",Fear=22,Trust=58,Doubt=20});
            State.People.Add(new VillageResident{Name="행상인 베른",Role="상인",Fear=16,Trust=31,Doubt=43});
            State.People.Add(new VillageResident{Name="순찰병 카엘",Role="경비",Fear=42,Trust=18,Doubt=25});
            State.People.Add(new VillageResident{Name="약초상 소피",Role="약초상",Fear=25,Trust=71,Doubt=15});
            State.People.Add(new VillageResident{Name="사제 루터",Role="사제",Fear=38,Trust=10,Doubt=60});
        }
        public void ChooseRuler(RulerKind kind)
        {
            if(kind==RulerKind.Unchosen)return;
            State.Ruler=(int)kind;
            var demon=DemonLordManager.Instance;
            if(demon!=null)demon.ConfigureArchetype((int)kind);
            State.Threat=kind==RulerKind.BlackFlame?20:kind==RulerKind.Wraith?12:8;
            State.HumanTrust=kind==RulerKind.BloodMoon?56:35;
            Record("새 왕좌의 시대: "+RulerName(kind));
            OnChanged?.Invoke();
        }
        public static string RulerName(RulerKind r)
        {
            if(r==RulerKind.BlackFlame)return "흑염 군주";
            if(r==RulerKind.BloodMoon)return "혈월 여왕";
            if(r==RulerKind.Wraith)return "망령 군주";
            return "왕좌의 주인을 기다리는 중";
        }
        public string ExportJson()
        {
            if(DemonLordManager.Instance!=null)State.SavedAuthority=DemonLordManager.Instance.Authority;
            return JsonUtility.ToJson(State);
        }
        public void ImportJson(string json)
        {
            if(string.IsNullOrEmpty(json))return;
            try
            {
                var s=JsonUtility.FromJson<RumorsOverhaulSave>(json);
                if(s==null)return;
                State=s;
                if(State.Chronicle==null)State.Chronicle=new List<string>();
                EnsurePeople();
                if(DemonLordManager.Instance!=null && State.Ruler>0)
                {
                    DemonLordManager.Instance.ConfigureArchetype(State.Ruler);
                    if(State.SavedAuthority>=0)DemonLordManager.Instance.SetAuthority(State.SavedAuthority);
                }
                OnChanged?.Invoke();
            }
            catch(Exception e){Debug.LogWarning("[RUMORS] Overhaul save parse failed: "+e.Message);}
        }
        private void OnDayEnd(int completedDay)
        {
            int absoluteDay=((TimeManager.Instance.CurrentYear-1)*12+(TimeManager.Instance.CurrentMonth-1))*10+completedDay;
            if(State.LastDayProcessed==absoluteDay)return;
            State.LastDayProcessed=absoluteDay;
            var ruler=(RulerKind)State.Ruler;
            int rumorGain=ruler==RulerKind.BloodMoon?3:ruler==RulerKind.Wraith?2:1;
            if(RumorManager.Instance!=null)RumorManager.Instance.AddStack(rumorGain);
            int roll=_rng.Next(0,100);
            int panic=0;
            foreach(var person in State.People)
            {
                person.Fear=Mathf.Clamp(person.Fear+(State.Threat>50?2:0)+_rng.Next(-1,2),0,100);
                if(_rng.Next(0,100)<Mathf.Clamp(12+State.Threat/4-person.Doubt/8,3,60))
                {
                    person.LastHeard=State.Threat>45?"마왕성에서 위험한 마력이 새어 나온다":"숲속에서 이상한 발자국을 보았다";
                    panic++;
                }
            }
            State.Threat=Mathf.Clamp(State.Threat+(ruler==RulerKind.BlackFlame?2:0)+(panic>2?1:0),0,100);
            State.HumanTrust=Mathf.Clamp(State.HumanTrust+(roll<20?1:0)-(State.Threat>65?1:0),0,100);
            if(roll<16 && RumorManager.Instance!=null)
            {
                RumorManager.Instance.Generate(roll<4?RumorType.Prophecy:RumorType.Exaggeration);
                Record("인간 마을에서 새로운 소문이 확산되었다.");
            }
            if(roll<9 && MonsterManager.Instance!=null)
            {
                foreach(var m in MonsterManager.Instance.Monsters)
                    if(m.Loyalty<26 && m.Ambition>65){Record(m.Name+"의 반란 움직임이 포착됐다.");break;}
            }
            OnChanged?.Invoke();
        }
        public bool HasFacility(BuildingType type)
        {
            var bm=BuildingManager.Instance;
            if(bm==null)return false;
            foreach(var kv in bm.Placed) if(kv.Value.Type==type && kv.Value.IsBuilt)return true;
            return false;
        }
        public bool SpreadRumor(out string message)
        {
            if(!HasFacility(BuildingType.RumorArchive)){message="소문 기록소 건설이 필요합니다.";return false;}
            if(!Spend(70,12,0,0)){message="금화 70, 마력 12가 필요합니다.";return false;}
            RumorManager.Instance?.AddStack(14);
            State.Threat=Mathf.Clamp(State.Threat+5,0,100);
            State.HumanTrust=Mathf.Clamp(State.HumanTrust-3,0,100);
            message="소문을 확산했다. 믿음 +14 / 위협도 +5 / 인간 신뢰 -3";
            Record(message);return true;
        }
        public bool Materialize(out string message)
        {
            if(!HasFacility(BuildingType.MobExtractor)){message="몬스터 추출기 완공이 필요합니다.";return false;}
            if(RumorManager.Instance==null){message="소문이 없습니다.";return false;}
            RumorData choice=null;
            foreach(var r in RumorManager.Instance.Rumors){if(r.CanMaterialize){choice=r;break;}}
            if(choice==null){message="믿음이 충분한 소문이 없습니다. 소문을 더 퍼뜨리세요.";return false;}
            if(!Spend(0,40,0,0)){message="실체화에는 마력 40이 필요합니다.";return false;}
            if(!RumorManager.Instance.TryMaterialize(choice.Id,out message))return false;
            if(MonsterManager.Instance!=null)
            {
                var beast=new MonsterData("소문에서 태어난 괴수",MonsterSpecies.RumorBeast,MonsterElement.Darkness,
                    50+choice.DangerRating*12,8+choice.DangerRating*4,5+choice.DangerRating,8,
                    "불안정한 환영",55,35+choice.DangerRating*10,10);
                beast.Traits.Add(choice.Ability);
                beast.Traits.Add("약점: "+choice.Weakness);
                MonsterManager.Instance.Spawn(beast);
            }
            State.Threat=Mathf.Clamp(State.Threat+choice.DangerRating*3,0,100);
            Record(message+" (위험도 상승)");return true;
        }
        public bool Breed(out string message)
        {
            if(!HasFacility(BuildingType.BreedingLab)){message="교배 연구소 완공이 필요합니다.";return false;}
            var mm=MonsterManager.Instance;
            if(mm==null||mm.Monsters.Count<2){message="서로 다른 부모 몬스터 2마리가 필요합니다.";return false;}
            if(!Spend(130,35,10,0)){message="교배에는 금화 130 / 마력 35 / 식량 10이 필요합니다.";return false;}
            var a=mm.Monsters[_rng.Next(mm.Monsters.Count)];
            var b=mm.Monsters[_rng.Next(mm.Monsters.Count)];
            if(mm.Monsters.Count>1){while(a==b)b=mm.Monsters[_rng.Next(mm.Monsters.Count)];}
            bool mutation=_rng.Next(0,100)<13;
            bool illness=_rng.Next(0,100)<10;
            var species=_rng.Next(0,2)==0?a.Species:b.Species;
            var child=new MonsterData(a.Name.Split(' ')[0]+"의 후예",species,a.Element,
                Mathf.Max(20,(a.MaxHP+b.MaxHP)/2+_rng.Next(-9,11)),
                Mathf.Max(3,(a.ATK+b.ATK)/2+_rng.Next(-3,5)+(mutation?4:0)),
                Mathf.Max(2,(a.DEF+b.DEF)/2+_rng.Next(-2,4)),
                Mathf.Max(3,(a.Speed+b.Speed)/2+_rng.Next(-2,4)),"혼합 혈통",65,_rng.Next(15,70),0);
            child.Generation=Mathf.Max(a.Generation,b.Generation)+1;
            child.Traits.Add("부계: "+a.Name);
            child.Traits.Add("모계: "+b.Name);
            if(mutation)child.Traits.Add("희귀 돌연변이 (+공격)");
            if(illness){child.MaxHP=Mathf.Max(15,child.MaxHP-12);child.CurrentHP=child.MaxHP;child.Traits.Add("유전병: 약한 체력");}
            mm.Spawn(child);
            State.BreedingCount++;
            message="제"+child.Generation+"세대 "+child.Name+" 탄생"+(mutation?" / 돌연변이 발생!":"")+(illness?" / 유전병 주의":"");
            Record(message);return true;
        }
        public bool Negotiate(out string message)
        {
            if(!HasFacility(BuildingType.FactionCouncil)){message="파벌 의회를 건설해야 협상할 수 있습니다.";return false;}
            if(!Spend(160,0,0,0)){message="협상금 160G가 부족합니다.";return false;}
            if(MonsterManager.Instance!=null)
                foreach(var m in MonsterManager.Instance.Monsters)
                {m.Loyalty=Mathf.Clamp(m.Loyalty+8,0,100);m.Ambition=Mathf.Clamp(m.Ambition+2,0,100);}
            DemonLordManager.Instance?.GainAuthority(-2,"파벌 타협의 대가");
            message="파벌과 타협했다. 충성도 +8, 야망 +2 / 권위 -2";
            Record(message);return true;
        }
        public bool Trade(out string message)
        {
            if(State.Threat>75){message="인간 경계도가 너무 높아 상인들이 거래를 거부합니다.";return false;}
            if(!Spend(80,0,0,0)){message="금화 80이 필요합니다.";return false;}
            ResourceManager.Instance?.Add(0,0,22,4);
            State.HumanTrust=Mathf.Clamp(State.HumanTrust+2,0,100);
            message="상인과 거래했다. 식량 +22 / 광석 +4 / 인간 신뢰 +2";
            Record(message);return true;
        }
        public bool Reinforce(out string message)
        {
            if(!HasFacility(BuildingType.TrainingGround)){message="훈련장 완공이 필요합니다.";return false;}
            if(!Spend(60,0,5,0)){message="금화 60, 식량 5가 필요합니다.";return false;}
            if(MonsterManager.Instance!=null)
                foreach(var m in MonsterManager.Instance.Monsters){m.Exp+=15;m.Fatigue=Mathf.Clamp(m.Fatigue+7,0,100);}
            message="수비 훈련 완료. 전원 경험치 +15 / 피로 +7";
            Record(message);return true;
        }
        private bool Spend(int gold,int mana,int food,int ore)
        {
            var r=ResourceManager.Instance;
            if(r==null||!r.CanAfford(gold,mana,food,ore))return false;
            r.Spend(gold,mana,food,ore);return true;
        }
        // A raid is presented in the battlefield first; the actual state change happens
        // only after the last combat turn. If the UI is missing, resolve immediately.
        private void OnRaid(HeroData hero)
        {
            if(RumorsPlayableScenes.Instance==null)
                ResolveRaid(hero,ComputeDefensePower()>=hero.Power+State.Threat/2);
        }
        public int ComputeDefensePower()
        {
            int strength=DemonLordManager.Instance!=null?DemonLordManager.Instance.Power:30;
            if(MonsterManager.Instance!=null)
                foreach(var m in MonsterManager.Instance.Monsters)
                    strength+=Mathf.Max(1,m.ATK+m.DEF-m.Fatigue/10);
            if(HasFacility(BuildingType.Barracks))strength+=40;
            if(HasFacility(BuildingType.TrainingGround))strength+=15;
            return strength;
        }
        public void ResolveRaid(HeroData hero,bool won)
        {
            if(hero==null)return;
            if(won)
            {
                State.RaidWins++;
                ResourceManager.Instance?.Add(55,8,0,7);
                DemonLordManager.Instance?.GainFame(4);
                State.Threat=Mathf.Clamp(State.Threat+5,0,100);
                Record("수비 성공! "+hero.Name+"의 원정을 격퇴했다. 전리품을 획득했다.");
            }
            else
            {
                State.RaidLosses++;
                if(ResourceManager.Instance!=null)
                {
                    var gold=ResourceManager.Instance.Gold;
                    ResourceManager.Instance.Spend(Mathf.Min(gold,75),0,0,0,true);
                }
                DemonLordManager.Instance?.LoseAuthority(10,"용사 침입으로 인한 피해");
                Record("수비 실패! "+hero.Name+"가 보급 창고를 약탈했다. 금화와 권위 손실.");
            }
            OnChanged?.Invoke();
            SaveLoadManager.Instance?.Save();
        }
        public void Record(string msg)
        {
            if(State.Chronicle==null)State.Chronicle=new List<string>();
            var t=TimeManager.Instance;
            var date=t==null?"서막":$"{t.CurrentYear}년 {t.CurrentMonth}월 {t.CurrentDay}일";
            State.Chronicle.Add(date+" · "+msg);
            if(State.Chronicle.Count>120)State.Chronicle.RemoveAt(0);
            OnNotice?.Invoke(msg);
            OnChanged?.Invoke();
        }
    }
}
