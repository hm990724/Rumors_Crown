using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rumors.Expansion
{
    [Serializable] public class RumorsCharacter
    {
        public string id, name, species, faction, temperament;
        public int power, loyalty = 65, ambition = 25, age, generation = 1, hunger = 1;
        public int attackGene = 1, resilienceGene = 1, magicGene = 1;
        public bool alive = true;
    }
    [Serializable] public class RumorsFaction
    {
        public string name;
        public int support = 60, influence = 10;
        public bool plotting;
    }
    [Serializable] public class HumanCitizen
    {
        public string name, occupation;
        public int skepticism = 35, fear = 15;
        public string latestRumor;
    }
    [Serializable] public class RumorsStory
    {
        public string subject, trait, cost;
        public int believers, strength;
    }
    [Serializable] public class RumorsWorld
    {
        public int day = 1, gold = 220, food = 90, mana = 70, ore = 25, demonPower = 60;
        public int authority = 65, threat = 5, rulerGeneration = 1;
        public int nextRaidDay = 8, nextCitizenId = 4, nextMonsterId = 6;
        public uint rngState = 41829;
        public string ruler = "첫 번째 마왕";
        public List<RumorsCharacter> monsters = new List<RumorsCharacter>();
        public List<HumanCitizen> people = new List<HumanCitizen>();
        public List<RumorsFaction> factions = new List<RumorsFaction>();
        public List<RumorsStory> stories = new List<RumorsStory>();
        public List<string> chronicle = new List<string>();
    }
    public sealed class RumorsSimulation
    {
        public RumorsWorld World { get; private set; }
        public event Action<string> Notice;
        public event Action RaidStarted;
        public event Action ThroneChanged;
        public RumorsSimulation(RumorsWorld world) { World = world ?? CreateDefault(); }
        public static RumorsWorld CreateDefault()
        {
            var w = new RumorsWorld();
            string[] names = {"작은 임프", "늪 슬라임", "뼈 강아지", "그림자 눈", "고블린 기술자"};
            string[] factions = {"화염단", "늪지 연합", "야수단", "망령단", "장인 조합"};
            for (int i=0;i<names.Length;i++)
            {
                w.monsters.Add(new RumorsCharacter { id="M"+(i+1), name=names[i], species=names[i], faction=factions[i], temperament=i%2==0?"야심가":"신중함", power=18+i*7, ambition=20+i*12 });
                w.factions.Add(new RumorsFaction { name=factions[i] });
            }
            w.people.Add(new HumanCitizen {name="촌장 마르코", occupation="촌장", skepticism=60});
            w.people.Add(new HumanCitizen {name="상인 루아", occupation="상인", skepticism=25});
            w.people.Add(new HumanCitizen {name="나무꾼 에른", occupation="나무꾼", skepticism=30});
            w.stories.Add(new RumorsStory { subject="북쪽 숲의 붉은 눈",trait="야간 시야",cost="낮 공격력 -20%",believers=12,strength=1});
            w.chronicle.Add("DAY 1: 최초의 마왕성이 세워졌다.");
            return w;
        }
        // Saveable RNG. No time-dependent UnityEngine.Random calls, so reloading preserves outcomes.
        int Roll(int min, int maxExclusive)
        {
            uint v=World.rngState;
            if(v==0) v=41829;
            v ^= v << 13; v ^= v >> 17; v ^= v << 5; World.rngState=v;
            return min+(int)(v%(uint)Math.Max(1,maxExclusive-min));
        }
        void Log(string s)
        {
            World.chronicle.Add("DAY "+World.day+": "+s);
            if(World.chronicle.Count>120) World.chronicle.RemoveAt(0);
            Notice?.Invoke(s);
        }
        public void NextDay()
        {
            World.day++;
            int workers=0;
            foreach(var m in World.monsters)
            {
                if(!m.alive)continue;
                workers++;
                m.age++;
                m.loyalty=Mathf.Clamp(m.loyalty+(World.food>workers ? 1 : -8),0,100);
            }
            World.food-=workers;
            World.gold+=10+workers*2;
            World.ore+=3;
            World.food+=9;
            World.mana+=2;
            World.gold-=Math.Max(1,workers/2); // payroll & maintenance
            if(World.food<0){World.food=0;Log("식량 부족! 몬스터들의 충성도가 흔들린다.");}
            HumanDaily();
            FactionDaily();
            if(World.day>=World.nextRaidDay) Raid();
            if(World.day%10==1) Log("새로운 달이 시작되었다. 결산 기록은 연대기에서 확인할 수 있다.");
        }
        void HumanDaily()
        {
            foreach(var p in World.people)
            {
                p.fear=Mathf.Clamp(p.fear + (World.threat>35?2:0),0,100);
                if(Roll(0,100)<18)
                {
                    var story=World.stories[Roll(0,World.stories.Count)];
                    int gain=Math.Max(0,Roll(1,5) + p.fear/30-p.skepticism/30);
                    story.believers+=gain;
                    p.latestRumor=story.subject;
                    if(story.believers>=30 && story.strength==1)
                    {story.strength=2;World.mana+=5;Log("인간 마을의 소문이 커졌다: "+story.subject);}
                }
            }
        }
        void FactionDaily()
        {
            foreach(var f in World.factions)
            {
                int total=0,n=0; RumorsCharacter contender=null;
                foreach(var m in World.monsters)
                {
                    if(!m.alive||m.faction!=f.name)continue;
                    total+=m.loyalty;n++;
                    if(m.ambition>=75 && m.power>World.demonPower && m.loyalty<=5) contender=m;
                }
                if(n==0) continue;
                int avg=total/n;
                f.support=Mathf.Clamp(f.support+(avg<30?-4:avg>70?1:0),0,100);
                if(f.support<25&&!f.plotting){f.plotting=true;Log(f.name+"에서 비밀 집회가 시작됐다.");}
                if(contender!=null && f.support<=12 && Roll(0,100)<35)
                {
                    World.ruler=contender.name;World.demonPower=contender.power;
                    World.rulerGeneration++;World.authority=35;contender.loyalty=100;
                    f.support=65;f.plotting=false;
                    Log(contender.name+"이(가) 반란으로 옥좌를 차지했다!");
                    ThroneChanged?.Invoke();
                }
            }
        }
        void Raid()
        {
            RaidStarted?.Invoke();
            int heroPower=24 + World.day/2 + World.threat/2 + Roll(0,18);
            int guards=World.demonPower;
            foreach(var m in World.monsters) if(m.alive) guards+=m.power/4;
            if(guards>=heroPower)
            {
                int reward=12+heroPower/2;
                World.gold+=reward; World.authority=Mathf.Clamp(World.authority+2,0,100);
                World.threat=Mathf.Clamp(World.threat+5,0,100);
                Log("용사 원정 격퇴! 전력 "+heroPower+", 전리품 "+reward+"G");
            }
            else
            {
                World.gold=Math.Max(0,World.gold-30);
                World.authority=Mathf.Clamp(World.authority-12,0,100);
                foreach(var m in World.monsters) if(m.alive) m.loyalty=Mathf.Clamp(m.loyalty-8,0,100);
                Log("용사에게 패배. 왕권이 흔들리고 자산을 약탈당했다.");
                if(heroPower>guards+25)
                {
                    World.ruler="용사 출신 제"+(World.rulerGeneration+1)+"대 마왕";
                    World.demonPower=Math.Max(45,heroPower-12);
                    World.rulerGeneration++;
                    World.authority=30;
                    Log("용사가 옥좌를 차지했다. 새로운 시대가 시작된다.");
                    ThroneChanged?.Invoke();
                }
            }
            World.nextRaidDay=World.day+Roll(7,11);
        }
        public bool Manifest(int storyIndex)
        {
            if(storyIndex<0||storyIndex>=World.stories.Count)return false;
            var story=World.stories[storyIndex];
            if(story.believers<30||World.mana<25)return false;
            World.mana-=25; story.believers-=20;World.threat+=5;
            var m=new RumorsCharacter {id="M"+World.nextMonsterId++,name=story.subject, species="소문체", faction="망령단",power=30+story.strength*7,ambition=Roll(20,85),loyalty=Roll(45,80),magicGene=3};
            World.monsters.Add(m);
            Log("소문이 실체화됐다: "+m.name+" / 대가: "+story.cost);
            return true;
        }
        public bool Breed(int a,int b)
        {
            if(a==b||a<0||b<0||a>=World.monsters.Count||b>=World.monsters.Count ||World.food<20||World.mana<15)return false;
            var ma=World.monsters[a];var mb=World.monsters[b];
            if(!ma.alive||!mb.alive)return false;
            World.food-=20;World.mana-=15;
            var child=new RumorsCharacter {
                id="M"+World.nextMonsterId++, name=ma.name+"의 후손",species=ma.species+" × "+mb.species,
                faction=ma.faction, generation=Math.Max(ma.generation,mb.generation)+1,
                attackGene=Roll(0,2)==0?ma.attackGene:mb.attackGene,
                resilienceGene=Roll(0,2)==0?ma.resilienceGene:mb.resilienceGene,
                magicGene=Roll(0,2)==0?ma.magicGene:mb.magicGene,
                ambition=Mathf.Clamp((ma.ambition+mb.ambition)/2+Roll(-15,16),0,100),
                power=Math.Max(8,(ma.power+mb.power)/3+Roll(-8,9)),loyalty=75
            };
            if(Roll(0,100)<8){child.magicGene++;child.hunger=2;}
            World.monsters.Add(child);
            Log("새로운 혈통 탄생: "+child.name+" ("+child.generation+"세대)");
            return true;
        }
        public string SaveJson() => JsonUtility.ToJson(World,true);
        public static RumorsSimulation FromJson(string json) => new RumorsSimulation(JsonUtility.FromJson<RumorsWorld>(json));
    }
}
