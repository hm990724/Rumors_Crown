using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;
using Rumors.Monsters;

namespace Rumors.Overhaul
{
    /// <summary>
    /// Actual interactive destination screens. Town residents are separately selectable and animate,
    /// raids display a time-stepped battle before committing a result to the existing world director.
    /// This uses the same game state as the castle; no duplicate resource counters.
    /// </summary>
    public sealed class RumorsPlayableScenes : MonoBehaviour
    {
        public static RumorsPlayableScenes Instance { get; private set; }
        private RectTransform _root, _scene, _actors, _info;
        private Text _title, _subtitle, _details, _battleStatus;
        private Image _heroHP, _defenderHP;
        private readonly List<RectTransform> _townActors = new List<RectTransform>();
        private readonly List<RectTransform> _battleActors = new List<RectTransform>();
        private readonly List<Image> _battleActorImages = new List<Image>();
        private HeroData _queuedHero;
        private float _animTime, _battleTimer;
        private int _turn;
        private int _heroMax, _defenderMax, _heroCurrent, _defenderCurrent;
        private bool _battleFinished;
        private string _mode;
        private bool _restorePause;
        private readonly System.Random _random = new System.Random(197201);
        private static readonly Color Gold = new Color(.98f,.79f,.47f,1f);
        private static readonly Color White = new Color(.93f,.89f,.82f,1f);
        private static readonly Color Ink = new Color(.085f,.055f,.126f,.98f);
        private static readonly Color Purple = new Color(.32f,.17f,.42f,.98f);

        private void Awake()
        {
            if(Instance!=null && Instance!=this){Destroy(gameObject);return;}
            Instance=this;
        }
        public void BuildUI(Transform canvas)
        {
            _root=Panel(canvas,"RUMORS Destination World",Vector2.zero,Vector2.zero,Vector2.one,Vector2.zero,Ink);
            _scene=Panel(_root,"View",new Vector2(0,-48),new Vector2(.025f,.17f),new Vector2(.975f,.93f),Vector2.zero,
                new Color(.17f,.13f,.27f));
            _actors=Panel(_scene,"Actors",Vector2.zero,Vector2.zero,Vector2.one,Vector2.zero,new Color(0,0,0,0));
            _info=Panel(_root,"Info",new Vector2(0,12),new Vector2(.025f,.018f),new Vector2(.975f,.18f),Vector2.zero,
                new Color(.11f,.075f,.17f,1f));
            _title=Label(_root,"Title","",new Vector2(0,-15),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(740,46),28,Gold);
            _subtitle=Label(_root,"Subtitle","",new Vector2(0,-57),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(900,25),16,White);
            _details=Label(_info,"Details","",new Vector2(20,0),new Vector2(0,.5f),new Vector2(0,.5f),new Vector2(750,90),19,White);
            _details.alignment=TextAnchor.MiddleLeft;
            Button(_root,"BackToCastle","마왕성으로 돌아가기",new Vector2(-22,-17),new Vector2(188,39),Close,new Vector2(1,1));
            _root.gameObject.SetActive(false);
        }
        private void Start()
        {
            if(HeroManager.Instance!=null)HeroManager.Instance.OnHeroInvading+=OnHeroInvading;
        }
        private void OnDestroy()
        {
            if(HeroManager.Instance!=null)HeroManager.Instance.OnHeroInvading-=OnHeroInvading;
            if(Instance==this)Instance=null;
        }
        private void Update()
        {
            if(_root==null || !_root.gameObject.activeSelf)return;
            _animTime+=Time.unscaledDeltaTime;
            if(_mode=="town")
            {
                for(int i=0;i<_townActors.Count;i++)
                    if(_townActors[i]!=null)
                    {
                        float x=Mathf.Sin(_animTime*.4f+i*2f)*12f;
                        float y=Mathf.Sin(_animTime*1.7f+i)*2.5f;
                        _townActors[i].anchoredPosition=new Vector2(-330+i*155+x,-14+(i%2)*68+y);
                    }
            }
            if(_mode=="battle" && !_battleFinished)AdvanceBattle(Time.unscaledDeltaTime);
            if(Input.GetKeyDown(KeyCode.Escape) && (_mode!="battle" || _battleFinished))Close();
        }
        private void Show(string mode)
        {
            if(_root==null)return;
            if(_root.gameObject.activeSelf)Close();
            _mode=mode;
            _animTime=0;
            _restorePause=TimeManager.Instance!=null && TimeManager.Instance.IsPaused;
            TimeManager.Instance?.Pause();
            _root.gameObject.SetActive(true);
            foreach(Transform child in _actors)Destroy(child.gameObject);
            foreach(Transform child in _scene)
                if(child!=_actors && child.name!="Scenery")Destroy(child.gameObject);
            _townActors.Clear();_battleActors.Clear();_battleActorImages.Clear();
            _scene.GetComponent<Image>().color=mode=="town"?new Color(.23f,.17f,.24f):new Color(.16f,.11f,.23f);
            if(mode=="town")BuildTown(); else BuildBattle();
            // Environment is painted first, actors must be in the frontmost canvas sibling.
            _actors.SetAsLastSibling();
        }
        public void OpenTown()
        {
            Show("town");
            _title.text="인간 저잣거리";
            _subtitle.text="주민의 말을 듣고 거래하세요. 소문을 퍼뜨리면 마왕성에도 영향을 줍니다.";
        }
        private void BuildTown()
        {
            var director=RumorsWorldDirector.Instance;
            var state=director!=null?director.State:null;
            // Scenery is a navigable, standalone environment composition, not a text-only popup.
            Image sky=Flat(_scene,"Dusk sky",new Color(.32f,.20f,.40f),new Vector2(0,0),new Vector2(0,.48f),new Vector2(1,1));
            Image road=Flat(_scene,"Village square",new Color(.40f,.30f,.33f),new Vector2(0,0),new Vector2(0,0),new Vector2(1,.48f));
            for(int i=0;i<30;i++)
            {
                int j=i;
                float x=((i*71)%1000)/1000f;
                float y=((i*47)%280)/1000f;
                Flat(road.transform,"Paving "+i,new Color(.55f,.43f,.43f,.6f),new Vector2(0,0),new Vector2(x,y),new Vector2(x+.027f,y+.018f));
            }
            var roof=new Color(.39f,.16f,.30f);
            var timber=new Color(.50f,.29f,.23f);
            House("여관",-360,62,roof,timber);
            House("시장",-100,96,new Color(.50f,.20f,.27f),new Color(.62f,.38f,.24f));
            House("길드",220,72,new Color(.22f,.25f,.46f),timber);
            House("교회",420,95,new Color(.29f,.26f,.40f),new Color(.50f,.44f,.43f));
            Flat(_scene,"Square basin",new Color(.69f,.59f,.48f),new Vector2(-12,-40),new Vector2(.5f,.41f),new Vector2(.5f,.41f),new Vector2(132,52));
            Flat(_scene,"Water",new Color(.30f,.72f,.91f),new Vector2(-12,-42),new Vector2(.5f,.41f),new Vector2(.5f,.41f),new Vector2(104,30));
            if(state!=null)
            {
                for(int i=0;i<state.People.Count;i++)
                {
                    int k=i;
                    var p=state.People[i];
                    var pawn=Panel(_actors,"Resident_"+i,new Vector2(-330+i*155,-14+(i%2)*68),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(75,112),new Color(0,0,0,0));
                    _townActors.Add(pawn);
                    var portrait=MakeCitizen(pawn,p.Role);
                    portrait.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,16);
                    Button(pawn,"Talk","대화",new Vector2(0,-40),new Vector2(72,30),()=>Speak(k));
                    Label(pawn,"Name",p.Name.Split(' ')[0],new Vector2(0,65),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(130,24),16,White);
                }
                _details.text="인간 경계도 "+state.Threat+" / 100    신뢰도 "+state.HumanTrust+" / 100\n"+
                              "인물을 선택하면 실제 NPC의 소문 기록을 열 수 있습니다.";
            }
            Button(_info,"Trade","상인과 거래",new Vector2(-180,0),new Vector2(170,43),()=>Act(RumorsWorldDirector.Instance.Trade),new Vector2(1,.5f));
            Button(_info,"Whisper","소문 수집",new Vector2(-18,0),new Vector2(145,43),CollectRumor,new Vector2(1,.5f));
        }
        private void House(string sign,float x,float y,Color roof,Color wall)
        {
            var h=Panel(_scene,"House "+sign,new Vector2(x,y),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(205,170),new Color(0,0,0,0));
            Flat(h,"Wall",wall,new Vector2(0,-24),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(180,104));
            Flat(h,"Roof",roof,new Vector2(0,43),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(195,64));
            Flat(h,"Door",new Color(.22f,.12f,.15f),new Vector2(0,-51),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(44,64));
            Flat(h,"WindowL",new Color(1f,.76f,.35f),new Vector2(-61,-18),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(31,30));
            Flat(h,"WindowR",new Color(1f,.76f,.35f),new Vector2(61,-18),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(31,30));
            Label(h,"Sign",sign,new Vector2(0,78),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(170,32),21,Gold);
        }
        private Image MakeCitizen(RectTransform parent,string role)
        {
            var face=Flat(parent,"Citizen face",new Color(.91f,.73f,.59f),new Vector2(0,13),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(34,35));
            Color c=role=="경비"?new Color(.35f,.49f,.68f):role=="사제"?new Color(.76f,.77f,.86f):new Color(.63f,.31f,.43f);
            Flat(parent,"Garment",c,new Vector2(0,-14),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(44,35));
            Flat(parent,"Hair",role=="사제"?Color.white:new Color(.25f,.16f,.17f),new Vector2(0,32),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(41,14));
            Flat(parent,"EyeL",new Color(.1f,.08f,.13f),new Vector2(-9,14),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(4,4));
            Flat(parent,"EyeR",new Color(.1f,.08f,.13f),new Vector2(9,14),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(4,4));
            return face;
        }
        private void Speak(int index)
        {
            var d=RumorsWorldDirector.Instance;
            if(d==null||index<0||index>=d.State.People.Count)return;
            var p=d.State.People[index];
            string heard=string.IsNullOrWhiteSpace(p.LastHeard)?"최근 들은 소문이 없습니다.":p.LastHeard;
            _details.text=p.Name+" · "+p.Role+"\n"+
                "공포 "+p.Fear+"   신뢰 "+p.Trust+"   의심 "+p.Doubt+"\n"+heard;
        }
        private void CollectRumor()
        {
            var d=RumorsWorldDirector.Instance;
            if(d==null)return;
            if(ResourceManager.Instance==null || !ResourceManager.Instance.Spend(30))
                {_details.text="정보 수집에는 금화 30이 필요합니다.";return;}
            RumorManager.Instance?.Generate(_random.Next(3)==0?RumorType.Exaggeration:RumorType.Truth);
            RumorManager.Instance?.AddStack(3);
            d.Record("인간 저잣거리에서 새로운 소문을 수집했다.");
            _details.text="정보 수집 성공! 소문 생성 및 믿음 +3 (금화 -30)";
        }
        private void Act(RumorAction action)
        {
            if(action==null)return;
            string msg;
            bool success=action(out msg);
            _details.text=(success?"성공: ":"실패: ")+msg;
        }
        private delegate bool RumorAction(out string msg);

        private void OnHeroInvading(HeroData hero)
        {
            if(hero==null)return;
            BeginRaid(hero);
        }
        public void OpenBattleRecord()
        {
            if(_queuedHero!=null&&!_battleFinished)return;
            Show("battle");
            _title.text="용사 원정대 · 전투 관전";
            _subtitle.text="실시간으로 전투를 관전합니다. 생존한 몬스터와 시설의 전력이 방어에 사용됩니다.";
        }
        private void BeginRaid(HeroData hero)
        {
            _queuedHero=hero;
            Show("battle");
            _title.text="용사 침입!  "+hero.Name;
            _subtitle.text="수비대가 마왕성 앞 전장에서 교전합니다. 전투 종료 후 결과가 세계에 반영됩니다.";
            _battleTimer=0;
            _turn=0;
            _battleFinished=false;
            var director=RumorsWorldDirector.Instance;
            _heroMax=Mathf.Max(85,hero.Power*3+(director!=null?director.State.Threat:0)*2);
            _defenderMax=Mathf.Max(90,DemonLordManager.Instance!=null?DemonLordManager.Instance.Power:100);
            if(MonsterManager.Instance!=null)foreach(var m in MonsterManager.Instance.Monsters)
                _defenderMax+=Mathf.Max(1,m.ATK+m.DEF-m.Fatigue/10)*2;
            _heroCurrent=_heroMax;_defenderCurrent=_defenderMax;
            UpdateBars();
        }
        private void BuildBattle()
        {
            var floor=Flat(_scene,"BattleFloor",new Color(.28f,.22f,.30f),Vector2.zero,Vector2.zero,Vector2.one);
            for(int i=0;i<14;i++)
            {
                float x=i/14f;
                Flat(floor.transform,"Flagstone",new Color(.37f,.31f,.38f,.5f),Vector2.zero,
                    new Vector2(x,.08f),new Vector2(Mathf.Min(1,x+.052f),.65f));
            }
            // Combatant sprite images come from existing Resources, not fake numerical battle buttons.
            var monsters=MonsterManager.Instance!=null?MonsterManager.Instance.Monsters:null;
            int count=monsters==null?0:Mathf.Min(5,monsters.Count);
            for(int i=0;i<count;i++)
            {
                var m=monsters[i];
                var actor=ImageSprite(_actors,"Defender "+m.Name,SpriteRegistry.GetMonster(m.Species),
                    new Vector2(-440+i*104,-45+(i%2)*60),new Vector2(80,80));
                _battleActors.Add(actor.rectTransform);_battleActorImages.Add(actor);
                Label(_actors,"DefName"+i,m.Name,new Vector2(-440+i*104,-107+(i%2)*60),
                    new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(115,25),15,White);
            }
            for(int i=0;i<3;i++)
            {
                var enemy=ImageSprite(_actors,"Hero "+i,LoadHero(i),
                    new Vector2(260+i*95,-20+(i%2)*55),new Vector2(82,82));
                enemy.color=Color.white;
                enemy.rectTransform.localScale=new Vector3(-1,1,1);
                _battleActors.Add(enemy.rectTransform);_battleActorImages.Add(enemy);
            }
            _heroHP=Flat(_scene,"Hero HP",new Color(.90f,.31f,.35f),new Vector2(313,127),
                new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(245,13));
            _defenderHP=Flat(_scene,"Defender HP",new Color(.42f,.94f,.59f),new Vector2(-320,127),
                new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(245,13));
            Label(_scene,"Hero Label","용사 원정대",new Vector2(313,156),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(245,33),21,Gold);
            Label(_scene,"DefLabel","마왕성 수비대",new Vector2(-320,156),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(245,33),21,Gold);
            _battleStatus=Label(_scene,"Battle log","전투 준비 중",new Vector2(0,-155),
                new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(670,44),19,White);
            _details.text="이벤트에 따라 용사가 침입합니다. 수비 몬스터의 피로도·전투력과 시설 보너스가 승패에 영향을 줍니다.";
        }
        private void AdvanceBattle(float dt)
        {
            if(_queuedHero==null){_battleFinished=true;return;}
            _battleTimer+=dt;
            foreach(var actor in _battleActors)
                if(actor!=null)
                {
                    var s=actor.localScale;
                    actor.localScale=new Vector3(s.x,1f+Mathf.Sin(_animTime*5f+actor.GetSiblingIndex())*.035f,1f);
                }
            if(_battleTimer<.8f)return;
            _battleTimer=0;
            _turn++;
            int defensePower=Mathf.Max(5,_defenderMax/24);
            int attackPower=Mathf.Max(5,_heroMax/24);
            int dealt=UnityEngine.Random.Range(defensePower/2,defensePower+1);
            int taken=UnityEngine.Random.Range(attackPower/2,attackPower+1);
            _heroCurrent=Mathf.Max(0,_heroCurrent-dealt);
            _defenderCurrent=Mathf.Max(0,_defenderCurrent-taken);
            _battleStatus.text="교전 "+_turn+"회차  ·  수비대 피해 "+taken+"   /   용사 피해 "+dealt;
            foreach(var image in _battleActorImages)if(image!=null)image.color=Color.white;
            if(_battleActorImages.Count>0)
                _battleActorImages[_turn%_battleActorImages.Count].color=new Color(1f,.51f,.55f);
            UpdateBars();
            if(_heroCurrent<=0 || _defenderCurrent<=0 || _turn>=40)
            {
                _battleFinished=true;
                bool won=_heroCurrent<=0 || (_defenderCurrent>0 && _defenderCurrent>=_heroCurrent);
                var d=RumorsWorldDirector.Instance;
                d?.ResolveRaid(_queuedHero,won);
                _details.text=won?"수비 성공! 전리품과 명성이 왕국에 반영됐습니다.":"수비 실패. 금화와 권위가 감소했습니다.";
                _battleStatus.text=won?"승리 · 침입자 격퇴":"패배 · 방어선 붕괴";
                _queuedHero=null;
            }
        }
        private void UpdateBars()
        {
            if(_heroHP!=null)_heroHP.rectTransform.localScale=new Vector3(Mathf.Clamp01(_heroCurrent/(float)Mathf.Max(1,_heroMax)),1,1);
            if(_defenderHP!=null)_defenderHP.rectTransform.localScale=new Vector3(Mathf.Clamp01(_defenderCurrent/(float)Mathf.Max(1,_defenderMax)),1,1);
        }
        private void Close()
        {
            if(_queuedHero!=null&&!_battleFinished)return;
            _root.gameObject.SetActive(false);
            _mode="";
            if(!_restorePause)TimeManager.Instance?.Resume();
        }
        private static RectTransform Panel(Transform parent,string name,Vector2 pos,Vector2 a,Vector2 b,Vector2 pivot,Color tint)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=a;r.anchorMax=b;r.pivot=pivot;r.anchoredPosition=pos;r.sizeDelta=Vector2.zero;
            go.GetComponent<Image>().color=tint;
            return r;
        }
        private static RectTransform Panel(Transform parent,string name,Vector2 pos,Vector2 a,Vector2 b,Vector2 pivot,Vector2 size,Color tint)
        {
            var r=Panel(parent,name,pos,a,b,pivot,tint);r.sizeDelta=size;return r;
        }
        private static Image Flat(Transform parent,string name,Color color,Vector2 pos,Vector2 a,Vector2 b,Vector2? size=null)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=a;r.anchorMax=b;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size??Vector2.zero;
            var img=go.GetComponent<Image>();img.color=color;img.raycastTarget=false;return img;
        }
        private static Sprite LoadHero(int i)
        {
            string name=i==0?"Hero_Knight":i==1?"Hero_Ranger":"Hero_Mage";
            var tex=Resources.Load<Texture2D>("RumorsOverhaul/"+name);
            if(tex==null)return SpriteRegistry.GetMonster(MonsterSpecies.GoblinTinker);
            tex.filterMode=FilterMode.Point;
            return Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(.5f,.5f),48f);
        }
        private static Image ImageSprite(Transform parent,string name,Sprite sprite,Vector2 pos,Vector2 size)
        {
            var img=Flat(parent,name,Color.white,pos,new Vector2(.5f,.5f),new Vector2(.5f,.5f),size);
            img.sprite=sprite;img.preserveAspect=true;
            return img;
        }
        private static Text Label(Transform parent,string name,string value,Vector2 pos,Vector2 a,Vector2 b,Vector2 size,int fontSize,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=a;r.anchorMax=b;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;
            var text=go.GetComponent<Text>();text.font=Rumors.UI.RumorUIHelper.GetKoreanFont();text.fontSize=fontSize;text.text=value;text.color=color;text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
            return text;
        }
        private static Button Button(Transform parent,string name,string label,Vector2 pos,Vector2 size,Action onClick,Vector2? anchor=null)
        {
            var a=anchor??new Vector2(.5f,.5f);
            var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);
            var rt=(RectTransform)go.transform;rt.anchorMin=a;rt.anchorMax=a;rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;
            var img=go.GetComponent<Image>();img.color=Purple;
            var outline=go.AddComponent<Outline>();outline.effectColor=Gold;outline.effectDistance=new Vector2(1,-1);
            var btn=go.GetComponent<Button>();btn.targetGraphic=img;btn.onClick.AddListener(()=>onClick());
            Label(go.transform,"Label",label,Vector2.zero,Vector2.zero,Vector2.one,Vector2.zero,17,White);
            return btn;
        }
    }
}
