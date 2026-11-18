using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Rumors.Expansion
{
    // Self-contained demo. It does NOT hook into your existing TimeManager or overwrite the scene.
    // Use RUMORS > Expansion > Create isolated demo scene in the Editor menu.
    public sealed class RumorsWorldClock : MonoBehaviour
    {
        public float secondsPerDay = 60f;
        public float speed = 1f;
        public bool paused;
        [Range(0f,1f)] public float dayFraction;
        RumorsSimulation simulation;
        Text dateLabel, resourceLabel, logLabel, rulerLabel, timeLabel;
        Image hand;
        Font uiFont;
        string savePath;
        void Awake()
        {
            savePath=Path.Combine(Application.persistentDataPath,"rumors_expansion_demo.json");
            simulation=new RumorsSimulation(RumorsSimulation.CreateDefault());
            simulation.Notice+=s=>{if(logLabel)logLabel.text=s;};
            simulation.RaidStarted+=()=>{paused=true;};
            simulation.ThroneChanged+=()=>{paused=true;};
            uiFont=Font.CreateDynamicFontFromOSFont(new [] {"Malgun Gothic","Arial"},18);
            CreateInterface();Refresh();
        }
        void Update()
        {
            // Prevent pause toggles while typing in Unity UI input fields.
            if(Input.GetKeyDown(KeyCode.Space) && !(EventSystem.current && EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>())) paused=!paused;
            if(Input.GetKeyDown(KeyCode.Alpha1)) speed=1;
            if(Input.GetKeyDown(KeyCode.Alpha2)) speed=2;
            if(Input.GetKeyDown(KeyCode.Alpha3)) speed=4;
            if(!paused)
            {
                dayFraction+=Time.unscaledDeltaTime * speed / Mathf.Max(5f,secondsPerDay);
                while(dayFraction>=1f)
                {
                    dayFraction-=1f;
                    simulation.NextDay();
                    Save();
                }
            }
            Refresh();
        }
        public void Save(){File.WriteAllText(savePath,simulation.SaveJson());}
        public void Load()
        {
            if(!File.Exists(savePath))return;
            simulation=RumorsSimulation.FromJson(File.ReadAllText(savePath));
            simulation.Notice+=s=>{if(logLabel)logLabel.text=s;};
            simulation.RaidStarted+=()=>paused=true;
            simulation.ThroneChanged+=()=>paused=true;
            Refresh();
        }
        GameObject UI(string name,Transform parent)
        {
            var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);return go;
        }
        void Box(Transform parent,string name,Vector2 anchor,Vector2 pivot,Vector2 pos,Vector2 size,Color color)
        {
            var go=UI(name,parent);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=anchor;r.pivot=pivot;r.anchoredPosition=pos;r.sizeDelta=size;
            var image=go.AddComponent<Image>();image.color=color;
        }
        Text Label(Transform parent,string name,string contents,Vector2 anchor,Vector2 pivot,Vector2 pos,Vector2 size,int fontSize,Color color)
        {
            var go=UI(name,parent);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=anchor;r.pivot=pivot;r.anchoredPosition=pos;r.sizeDelta=size;
            var t=go.AddComponent<Text>();t.font=uiFont;t.text=contents;t.fontSize=fontSize;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.resizeTextForBestFit=true;
            return t;
        }
        void Button(Transform parent,string caption,Vector2 position,Action action)
        {
            var go=UI(caption,parent);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=new Vector2(1,1);r.pivot=new Vector2(1,1);r.anchoredPosition=position;r.sizeDelta=new Vector2(95,34);
            var img=go.AddComponent<Image>();img.color=new Color(.22f,.17f,.31f,.95f);
            var b=go.AddComponent<Button>();b.targetGraphic=img;b.onClick.AddListener(()=>action());
            Label(go.transform,"Text",caption,new Vector2(.5f,.5f),new Vector2(.5f,.5f),Vector2.zero,new Vector2(90,30),16,new Color(1f,.87f,.65f));
        }
        void CreateInterface()
        {
            if(!EventSystem.current)
            {
                var es=new GameObject("Expansion EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
                es.transform.SetParent(transform,false);
            }
            var go=UI("RUMORS Expansion UI",transform);
            var canvas=go.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=90;
            go.AddComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.GetComponent<CanvasScaler>().referenceResolution=new Vector2(1600,900);
            go.AddComponent<GraphicRaycaster>();
            Box(go.transform,"TopHUD",new Vector2(.5f,1),new Vector2(.5f,1),Vector2.zero,new Vector2(1600,48),new Color(.07f,.06f,.11f,.93f));
            dateLabel=Label(go.transform,"Day", "DAY 01",new Vector2(0,1),new Vector2(0,1),new Vector2(24,-7),new Vector2(180,40),25,new Color(1f,.85f,.55f));
            resourceLabel=Label(go.transform,"Resources","",new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(70,-8),new Vector2(800,36),17,new Color(.89f,.86f,.74f));
            rulerLabel=Label(go.transform,"Ruler","",new Vector2(0,1),new Vector2(0,1),new Vector2(24,-60),new Vector2(360,32),17,Color.white);
            Box(go.transform,"ClockBase",new Vector2(1,1),new Vector2(1,1),new Vector2(-20,-64),new Vector2(205,205),new Color(.11f,.09f,.17f,.97f));
            var clock=UI("ClockFace",go.transform);var cr=(RectTransform)clock.transform;cr.anchorMin=cr.anchorMax=new Vector2(1,1);cr.pivot=new Vector2(1,1);cr.anchoredPosition=new Vector2(-31,-75);cr.sizeDelta=new Vector2(183,183);
            var ci=clock.AddComponent<Image>();
            var ring=Resources.Load<Sprite>("RUMORS_Clock_Ring");if(ring)ci.sprite=ring;
            else ci.color=new Color(.36f,.26f,.47f,1f);
            timeLabel=Label(go.transform,"Time","",new Vector2(1,1),new Vector2(1,1),new Vector2(-60,-133),new Vector2(143,48),22,Color.white);
            logLabel=Label(go.transform,"Notice","",new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(0,15),new Vector2(800,45),16,new Color(1f,.79f,.6f));
            Button(go.transform,"일시정지",new Vector2(-20,-279),()=>paused=!paused);
            Button(go.transform,"1×",new Vector2(-120,-279),()=>{speed=1;paused=false;});
            Button(go.transform,"2×",new Vector2(-220,-279),()=>{speed=2;paused=false;});
            Button(go.transform,"4×",new Vector2(-320,-279),()=>{speed=4;paused=false;});
            Button(go.transform,"교배 테스트",new Vector2(-20,-320),()=>simulation.Breed(0,1));
            Button(go.transform,"실체화",new Vector2(-120,-320),()=>simulation.Manifest(0));
            Button(go.transform,"저장",new Vector2(-220,-320),Save);
            Button(go.transform,"불러오기",new Vector2(-320,-320),Load);
        }
        void Refresh()
        {
            var w=simulation.World;
            int hour=Mathf.FloorToInt(dayFraction*24)%24;
            if(dateLabel)dateLabel.text="DAY "+w.day.ToString("00")+" / MONTH "+(((w.day-1)/10)%12+1);
            if(resourceLabel)resourceLabel.text="G "+w.gold+"    식량 "+w.food+"    마력 "+w.mana+"    광석 "+w.ore+"    위협 "+w.threat+"%";
            if(rulerLabel)rulerLabel.text=w.ruler+"  |  권위 "+w.authority;
            if(timeLabel)timeLabel.text=hour.ToString("00")+":"+Mathf.FloorToInt(dayFraction*1440%60).ToString("00")+"\n"+(paused?"정지":speed+"×");
        }
    }
}
