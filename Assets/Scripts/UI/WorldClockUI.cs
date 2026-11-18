using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;

namespace Rumors.UI
{
    /// <summary>Original top-right day/night clock; requires the existing Rumors UIManager.</summary>
    public sealed class WorldClockUI : MonoBehaviour
    {
        private Text _day, _phase, _time;
        private RectTransform _hand;
        private Texture2D _texture;
        private Sprite _sprite;

        public void BuildUI(Transform canvasRoot)
        {
            var font = Font.CreateDynamicFontFromOSFont(new[] { "Malgun Gothic", "Arial" }, 16);
            var panel = new GameObject("RumorsWorldClock", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvasRoot, false);
            var p=(RectTransform)panel.transform;
            p.anchorMin=p.anchorMax=new Vector2(1,1);
            p.pivot=new Vector2(1,1);
            p.anchoredPosition=new Vector2(-12,-74);
            p.sizeDelta=new Vector2(150,181);
            panel.GetComponent<Image>().color=new Color(.09f,.07f,.15f,.95f);

            _texture=CreateRing(112);
            _sprite=Sprite.Create(_texture,new Rect(0,0,112,112),new Vector2(.5f,.5f),112);
            var wheel=new GameObject("DayNightWheel",typeof(RectTransform),typeof(Image));
            wheel.transform.SetParent(panel.transform,false);
            var wr=(RectTransform)wheel.transform;
            wr.anchorMin=wr.anchorMax=new Vector2(.5f,1);
            wr.pivot=new Vector2(.5f,1);
            wr.anchoredPosition=new Vector2(0,-6);
            wr.sizeDelta=new Vector2(112,112);
            wheel.GetComponent<Image>().sprite=_sprite;
            wheel.GetComponent<Image>().raycastTarget=false;

            var hand=new GameObject("TimeHand",typeof(RectTransform),typeof(Image));
            hand.transform.SetParent(panel.transform,false);
            _hand=(RectTransform)hand.transform;
            _hand.anchorMin=_hand.anchorMax=new Vector2(.5f,1);
            _hand.pivot=new Vector2(.5f,0);
            _hand.anchoredPosition=new Vector2(0,-62);
            _hand.sizeDelta=new Vector2(3,42);
            hand.GetComponent<Image>().color=new Color(.98f,.88f,.55f);
            hand.GetComponent<Image>().raycastTarget=false;

            _day=MakeText(panel.transform,"Day",new Vector2(0,-31),16,font);
            _time=MakeText(panel.transform,"Clock",new Vector2(0,-53),17,font);
            _phase=MakeText(panel.transform,"Phase",new Vector2(0,-78),12,font);

            var btn=new GameObject("ReportButton",typeof(RectTransform),typeof(Image),typeof(Button));
            btn.transform.SetParent(panel.transform,false);
            var br=(RectTransform)btn.transform;
            br.anchorMin=br.anchorMax=new Vector2(.5f,1);
            br.anchoredPosition=new Vector2(0,-141);
            br.sizeDelta=new Vector2(104,25);
            btn.GetComponent<Image>().color=new Color(.27f,.20f,.37f);
            btn.GetComponent<Button>().onClick.AddListener(() => {
                if(DailySettlementUI.Instance!=null) DailySettlementUI.Instance.ShowLastReport();
            });
            MakeText(btn.transform,"Report",Vector2.zero,13,font).text="일일 기록";
        }

        private static Text MakeText(Transform parent,string name,Vector2 offset,int size,Font font)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));
            go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;
            rect.anchorMin=rect.anchorMax=new Vector2(.5f,1);
            rect.anchoredPosition=offset;
            rect.sizeDelta=new Vector2(120,24);
            var t=go.GetComponent<Text>();
            t.font=font;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;
            t.color=new Color(1,.88f,.69f);t.raycastTarget=false;
            return t;
        }
        private static Texture2D CreateRing(int size)
        {
            var tex=new Texture2D(size,size,TextureFormat.RGBA32,false);
            tex.filterMode=FilterMode.Point;
            float center=(size-1)*.5f;
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float dx=x-center,dy=y-center,r=Mathf.Sqrt(dx*dx+dy*dy);
                float a=(Mathf.Atan2(dx,dy)*Mathf.Rad2Deg+360)%360;
                Color col=Color.clear;
                if(r>=38 && r<53)
                {
                    col=a<90?new Color(.8f,.61f,.33f):
                        a<210?new Color(.96f,.76f,.42f):
                        a<270?new Color(.66f,.37f,.47f):new Color(.28f,.33f,.55f);
                    if(r<41||r>50) col*=.72f;
                    col.a=1;
                }
                else if(r<38)col=new Color(.11f,.09f,.19f,1);
                tex.SetPixel(x,y,col);
            }
            tex.Apply();
            return tex;
        }
        private void Update()
        {
            var t=TimeManager.Instance;
            if(t==null)return;
            float v=t.DayProgress;
            if(_hand!=null)_hand.localRotation=Quaternion.Euler(0,0,-360*v);
            if(_day!=null)_day.text=$"DAY {t.CurrentDay:00}";
            int mins=Mathf.FloorToInt(v*1440);
            if(_time!=null)_time.text=$"{mins/60:00}:{mins%60:00}";
            if(_phase!=null)_phase.text=t.IsPaused?"일시정지":v<.25f?"밤":v<.5f?"아침":v<.75f?"낮":"저녁";
        }
        private void OnDestroy()
        {
            if(_sprite!=null)Destroy(_sprite);
            if(_texture!=null)Destroy(_texture);
        }
    }
}
