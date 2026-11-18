using System;
using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;
using Rumors.Buildings;
using Rumors.Monsters;

namespace Rumors.Overhaul
{
    /// <summary>Native in-game UI, NOT a screenshot overlay. Selectors trigger live managers.</summary>
    public sealed class RumorsOverhaulUI : MonoBehaviour
    {
        public static RumorsOverhaulUI Instance { get; private set; }
        private void Awake() { if(Instance==null)Instance=this; else Destroy(gameObject); }
        private RectTransform _canvas, _rulerRoot, _roomRoot, _content;
        private Text _roomTitle, _roomBody, _toast, _rulerDetail;
        private Image _selectedFrame;
        private RulerKind _selection=RulerKind.BlackFlame;
        private string _room="";
        private float _toastUntil;
        private bool _pausedBefore;
        private static readonly Color Gold=new Color(1f,.83f,.53f);
        private static readonly Color Ivory=new Color(.95f,.91f,.84f);
        private static readonly Color Ink=new Color(.09f,.064f,.125f,.97f);
        private static readonly Color Violet=new Color(.38f,.19f,.48f);

        public void BuildUI(Transform canvasRoot)
        {
            _canvas=(RectTransform)canvasRoot;
            // Small edge actions, rather than permanent sidebar or huge overlay.
            var bar=Panel(canvasRoot,"RumorsActionDock",new Vector2(0,0),new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(650,62),Ink);
            string[] rooms={"왕좌","저잣거리","소문","교배","파벌","전투","연대기"};
            for(int i=0;i<rooms.Length;i++)
            {
                int idx=i;
                Button(bar,"Dock_"+rooms[i],rooms[i],new Vector2(-274+i*91,0),new Vector2(84,42),()=>OpenRoom(rooms[idx]));
            }
            _toast=Text(canvasRoot,"Toast","",new Vector2(0,-90),new Vector2(.5f,1),new Vector2(0.5f,1),new Vector2(760,44),19,Gold);
            _toast.gameObject.SetActive(false);
            BuildRoom(canvasRoot);
            BuildRulerSelection(canvasRoot);
        }
        private void Start()
        {
            var director=RumorsWorldDirector.Instance;
            if(director!=null){director.OnNotice+=Toast;director.OnChanged+=RefreshIfOpen;}
            if(director==null || director.State.Ruler==0)
            {
                if(_rulerRoot)_rulerRoot.gameObject.SetActive(true);
                _pausedBefore=TimeManager.Instance!=null&&TimeManager.Instance.IsPaused;
                TimeManager.Instance?.Pause();
            }
            else if(_rulerRoot)_rulerRoot.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            if(Instance==this)Instance=null;
            var d=RumorsWorldDirector.Instance;
            if(d!=null){d.OnNotice-=Toast;d.OnChanged-=RefreshIfOpen;}
        }
        private void Update()
        {
            if(_toast!=null && _toast.gameObject.activeSelf && Time.unscaledTime>=_toastUntil)_toast.gameObject.SetActive(false);
            if(_roomRoot!=null && _roomRoot.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))CloseRoom();
        }
        private void Toast(string message)
        {
            if(_toast==null)return;
            _toast.text=message;
            _toast.gameObject.SetActive(true);
            _toastUntil=Time.unscaledTime+5f;
        }
        private void BuildRulerSelection(Transform parent)
        {
            _rulerRoot=Panel(parent,"RulerSelection",Vector2.zero,Vector2.zero,Vector2.one,new Vector2(.5f,.5f),Vector2.zero,Ink);
            var root=_rulerRoot;
            Image background=CreateUIImage(root,"Backdrop",new Vector2(0,0),new Vector2(0,0),Vector2.one,new Vector2(.5f,.5f),Vector2.zero);
            background.sprite=ReadArt("RUMORS_Menu_Backdrop");
            background.color=new Color(.43f,.32f,.51f,.45f);background.type=Image.Type.Simple;
            background.rectTransform.SetAsFirstSibling();
            Panel(root,"BackdropShade",Vector2.zero,Vector2.zero,Vector2.one,new Vector2(.5f,.5f),Vector2.zero,new Color(.055f,.038f,.095f,.70f));
            Text(root,"Title","R U M O R S",new Vector2(0,-50),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(780,70),44,Gold);
            Text(root,"Subtitle","왕좌의 주인을 선택하라",new Vector2(0,-107),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(780,42),27,Ivory);
            string[] arts={"Ruler_Flame","Ruler_Succubus","Ruler_Wraith"};
            string[] names={"흑염 군주","혈월 여왕","망령 군주"};
            string[] subtitles={"공포와 강력한 군사력","매혹과 소문 경제","저주와 마력, 불안한 충성"};
            for(int i=0;i<3;i++)
            {
                var kind=(RulerKind)(i+1);
                float x=(i-1)*300;
                var card=Panel(root,"RulerCard"+i,new Vector2(x,17),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(270,326),new Color(.12f,.07f,.16f,.98f));
                var portrait=CreateUIImage(card,"Portrait",new Vector2(0,-16),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(245,181));
                portrait.sprite=ReadArt(arts[i]);portrait.preserveAspect=false;portrait.type=Image.Type.Simple;
                Text(card,"Name",names[i],new Vector2(0,-215),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(250,38),23,Gold);
                Text(card,"Attribute",subtitles[i],new Vector2(0,-252),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(250,34),16,Ivory);
                var btn=Button(card,"Pick","선택",new Vector2(0,-294),new Vector2(135,40),()=>Select(kind));
                if(i==0)_selectedFrame=card.GetComponent<Image>();
            }
            _rulerDetail=Text(root,"Description","",new Vector2(0,91),new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(980,66),19,Ivory);
            Button(root,"StartGame","새 시대 시작",new Vector2(148,29),new Vector2(260,54),Begin);
            Button(root,"LoadExisting","기존 기록 불러오기",new Vector2(-148,29),new Vector2(260,54),LoadExisting);
            Select(RulerKind.BlackFlame);
        }
        private void Select(RulerKind kind)
        {
            _selection=kind;
            if(_rulerDetail!=null)
                _rulerDetail.text=kind==RulerKind.BlackFlame?
                  "권위와 전투에 강하지만 인간 위협도 상승이 빠릅니다. 반란과 강한 용사를 조심하세요.":
                  kind==RulerKind.BloodMoon?
                  "소문과 교역에 유리한 매혹의 여왕입니다. 군사력이 낮아 초기 방어 준비가 중요합니다.":
                  "마력과 저주가 강한 망령 군주입니다. 높은 야망의 몬스터가 쉽게 충돌할 수 있습니다.";
            for(int i=0;i<3;i++)
            {
                var card=_rulerRoot.Find("RulerCard"+i);
                if(card!=null)card.GetComponent<Image>().color=(int)kind==i+1?new Color(.45f,.29f,.46f,1):new Color(.12f,.07f,.16f,.98f);
            }
        }
        private void LoadExisting()
        {
            if(SaveLoadManager.Instance==null || !SaveLoadManager.Instance.Load())
            {
                Toast("저장 파일을 찾지 못했습니다.");return;
            }
            _rulerRoot.gameObject.SetActive(false);
            if(!_pausedBefore)TimeManager.Instance?.Resume();
            Toast("기존 왕국 기록을 불러왔습니다.");
        }
        private void Begin()
        {
            RumorsWorldDirector.Instance?.ChooseRuler(_selection);
            _rulerRoot.gameObject.SetActive(false);
            if(!_pausedBefore)TimeManager.Instance?.Resume();
            Toast(RumorsWorldDirector.RulerName(_selection)+"의 시대가 시작되었습니다.");
        }
        private void BuildRoom(Transform parent)
        {
            _roomRoot=Panel(parent,"GameplayRoom",new Vector2(0,26),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(755,500),Ink);
            _roomTitle=Text(_roomRoot,"RoomTitle","",new Vector2(0,-40),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(670,50),28,Gold);
            _roomBody=Text(_roomRoot,"RoomBody","",new Vector2(0,-99),new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(685,242),19,Ivory);
            _roomBody.alignment=TextAnchor.UpperLeft;
            _roomBody.horizontalOverflow=HorizontalWrapMode.Wrap;
            _roomBody.verticalOverflow=VerticalWrapMode.Truncate;
            Button(_roomRoot,"CloseRoom","닫기  ×",new Vector2(324,-35),new Vector2(92,37),CloseRoom,new Vector2(1,1));
            _content=Panel(_roomRoot,"Actions",new Vector2(0,38),new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(.5f,.5f),new Vector2(678,86),new Color(.16f,.10f,.21f,.92f));
            _roomRoot.gameObject.SetActive(false);
        }
        public void OpenRoom(string room)
        {
            if(_rulerRoot!=null && _rulerRoot.gameObject.activeSelf)return;
            // Town and battlefield are actual independently rendered destinations.
            if(room=="저잣거리")
            {
                _roomRoot.gameObject.SetActive(false);
                RumorsPlayableScenes.Instance?.OpenTown();
                return;
            }
            if(room=="전투")
            {
                _roomRoot.gameObject.SetActive(false);
                RumorsPlayableScenes.Instance?.OpenBattleRecord();
                return;
            }
            // One primary window at a time, so old UGUI overlays cannot stack.
            if(Rumors.UI.RumorPanelUI.Instance!=null)Rumors.UI.RumorPanelUI.Instance.Hide();
            if(Rumors.UI.SceneNavigatorUI.Instance!=null)Rumors.UI.SceneNavigatorUI.Instance.Hide();
            _room=room;
            _roomRoot.gameObject.SetActive(true);
            RefreshRoom();
        }
        private void CloseRoom(){_roomRoot.gameObject.SetActive(false);}
        private void RefreshIfOpen(){if(_roomRoot!=null && _roomRoot.gameObject.activeSelf)RefreshRoom();}
        private void RefreshRoom()
        {
            if(_roomBody==null)return;
            var d=RumorsWorldDirector.Instance;
            if(d==null)return;
            _roomTitle.text="✦  "+_room+"  ✦";
            while(_content.childCount>0)
            {
                var child=_content.GetChild(0);
                child.SetParent(null,false);
                Destroy(child.gameObject);
            }
            var s=d.State;
            if(_room=="왕좌")
            {
                _roomBody.text=RumorsWorldDirector.RulerName((RulerKind)s.Ruler)+"\n\n"+
                    "위협도: "+s.Threat+" / 100      인간 신뢰: "+s.HumanTrust+" / 100\n"+
                    "원정 방어 성공 "+s.RaidWins+"회 / 실패 "+s.RaidLosses+"회\n"+
                    "마왕 권위: "+(DemonLordManager.Instance?.Authority??0)+"\n\n"+
                    "초기 성장: 광산과 농장을 건설하고 작업자를 배치하세요.\n"+
                    "소문 기록소와 추출기로 새로운 몬스터를 확보하세요.";
                ActionButton("일일 기록",()=>{CloseRoom();if(Rumors.UI.DailySettlementUI.Instance!=null)Rumors.UI.DailySettlementUI.Instance.ShowLastReport();});
            }
            else if(_room=="저잣거리")
            {
                string people="";
                foreach(var p in s.People)people+=p.Name+" ("+p.Role+")    공포 "+p.Fear+"    "+(string.IsNullOrEmpty(p.LastHeard)?"소문 없음":p.LastHeard)+"\n";
                _roomBody.text="인간 경계도 "+s.Threat+" / 100   ·  신뢰 "+s.HumanTrust+" / 100\n\n"+people;
                ActionButton("상인 거래",()=>Act(d.Trade),"금 80 → 식량 22, 광석 4");
            }
            else if(_room=="소문")
            {
                string text="";
                if(RumorManager.Instance!=null)
                    foreach(var r in RumorManager.Instance.Rumors)
                    {text+=(r.CanMaterialize?"[실체화 가능] ":"[전파 중] ")+r.Text+"\n"+r.Stack+" / "+r.RequiredStack+"\n";if(text.Length>500)break;}
                _roomBody.text="기록된 믿음: "+(RumorManager.Instance?.TotalStack??0)+"\n\n"+text;
                ActionButton("소문 확산",()=>Act(d.SpreadRumor),"금 70 + 마력 12");
                ActionButton("실체화",()=>Act(d.Materialize),"추출기 + 마력 40");
            }
            else if(_room=="교배")
            {
                _roomBody.text="서로 다른 두 부모의 능력과 성격을 물려받습니다.\n\n"+
                    "연구소 상태: "+(d.HasFacility(BuildingType.BreedingLab)?"가동 가능":"잠김 — 교배 연구소 건설 필요")+"\n"+
                    "개체 수: "+(MonsterManager.Instance?.Monsters.Count??0)+"\n"+
                    "탄생한 후손: "+s.BreedingCount+"\n\n"+
                    "돌연변이는 공격력을 높이지만 유전병이 발생할 수도 있습니다.";
                ActionButton("교배 시도",()=>Act(d.Breed),"금 130 · 마력 35 · 식량 10");
            }
            else if(_room=="파벌")
            {
                string txt="";
                if(MonsterManager.Instance!=null)
                    foreach(var m in MonsterManager.Instance.Monsters)
                    {txt+=m.Name+"     충성 "+m.Loyalty+" / 야망 "+m.Ambition+"\n";if(txt.Length>470)break;}
                _roomBody.text="파벌 의회: "+(d.HasFacility(BuildingType.FactionCouncil)?"열림":"잠김 — 파벌 의회 건설 필요")+"\n\n"+txt;
                ActionButton("파벌 회유",()=>Act(d.Negotiate),"금 160 / 권위 -2");
            }
            else if(_room=="전투")
            {
                _roomBody.text="용사는 8일차부터 위협도에 따라 출현합니다.\n\n"+
                    "원정 방어 성공: "+s.RaidWins+"회\n원정 방어 실패: "+s.RaidLosses+"회\n"+
                    "몬스터 전투력과 훈련장·병영의 방어 효과가 합산됩니다.\n\n"+
                    "현재 단계는 전투 결과 시뮬레이션입니다. 실제 2D 자동전투 연출은 아직 별도 구현이 필요합니다.";
                ActionButton("수비 훈련",()=>Act(d.Reinforce),"금 60 · 식량 5");
            }
            else if(_room=="연대기")
            {
                string txt="";
                for(int i=s.Chronicle.Count-1;i>=0;i--)
                {txt+=s.Chronicle[i]+"\n\n";if(txt.Length>550)break;}
                _roomBody.text=txt.Length==0?"아직 기록된 사건이 없습니다.":txt;
                ActionButton("게임 저장",()=>{SaveLoadManager.Instance?.Save();Toast("저장을 요청했습니다.");});
            }
        }
        private delegate bool ActionWithMessage(out string message);
        private void Act(ActionWithMessage action)
        {
            string message;
            bool ok=action(out message);
            Toast((ok?"성공 · ":"실행 불가 · ")+message);
            RefreshRoom();
        }
        private int _actions;
        private void ActionButton(string label,Action click,string note="")
        {
            // Count child buttons, so refresh does not rely on mutable stale counters.
            int idx=_content.childCount;
            float offset=(idx-1)*211f;
            Button(_content,"RoomAction_"+idx,label,new Vector2(offset,6),new Vector2(198,43),click);
            if(!string.IsNullOrEmpty(note))Text(_content,"Note_"+idx,note,new Vector2(offset,-29),new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(204,20),12,new Color(.85f,.74f,.83f));
        }
        private static RectTransform Panel(Transform parent,string name,Vector2 pos,Vector2 min,Vector2 max,Vector2 pivot,Vector2 size,Color tint)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=min;r.anchorMax=max;r.pivot=pivot;r.anchoredPosition=pos;r.sizeDelta=size;
            var img=go.GetComponent<Image>();
            var sprite=SpriteRegistry.GetPanel();
            if(sprite!=null){img.sprite=sprite;img.type=Image.Type.Sliced;img.color=tint;} else img.color=tint;
            return r;
        }
        private static Image CreateUIImage(Transform parent,string name,Vector2 pos,Vector2 min,Vector2 max,Vector2 pivot,Vector2 size)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=min;r.anchorMax=max;r.pivot=pivot;r.anchoredPosition=pos;r.sizeDelta=size;
            return go.GetComponent<Image>();
        }
        private static Image CreateUIImage(Transform parent,string name,Vector2 pos,Vector2 min,Vector2 max,Vector2 size)
            =>CreateUIImage(parent,name,pos,min,max,new Vector2(.5f,.5f),size);
        private static Text Text(Transform parent,string name,string content,Vector2 pos,Vector2 anchorMin,Vector2 anchorMax,Vector2 size,int fontSize,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=anchorMin;r.anchorMax=anchorMax;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;
            var t=go.GetComponent<Text>();t.font=Rumors.UI.RumorUIHelper.GetKoreanFont();t.fontSize=fontSize;t.text=content;t.color=color;
            t.alignment=TextAnchor.MiddleCenter;t.resizeTextForBestFit=false;t.raycastTarget=false;
            var sh=go.AddComponent<Shadow>();sh.effectColor=new Color(0,0,0,.85f);sh.effectDistance=new Vector2(1,-1);
            return t;
        }
        private static Button Button(Transform parent,string name,string label,Vector2 pos,Vector2 size,Action click,Vector2? anchor=null)
        {
            var a=anchor??new Vector2(.5f,.5f);
            var img=CreateUIImage(parent,name,pos,a,a,size);img.color=new Color(.29f,.19f,.36f,.98f);
            var outline=img.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.75f,.52f,.30f,.86f);outline.effectDistance=new Vector2(1.5f,-1.5f);
            var b=img.gameObject.AddComponent<Button>();b.targetGraphic=img;
            var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1.2f,1.04f,1.26f);colors.pressedColor=new Color(.75f,.65f,.85f);b.colors=colors;
            b.onClick.AddListener(()=>click());
            Text(img.transform,"Label",label,Vector2.zero,Vector2.zero,Vector2.one,Vector2.zero,17,Ivory);
            // Labels stretch to the button rect when both anchors differ.
            return b;
        }
        private static Sprite ReadArt(string name)
        {
            var tex=Resources.Load<Texture2D>("RumorsOverhaul/"+name);
            if(tex==null)return null;
            tex.filterMode=FilterMode.Bilinear; // only UI concept portraits, not pixel-world assets
            return Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(.5f,.5f),100f);
        }
    }
}
