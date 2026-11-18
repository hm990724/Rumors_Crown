using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rumors.UI
{
    /// <summary>
    /// Creates a single Canvas and initialises all UI controllers.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas _canvas;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            EnsureEventSystem();
            _canvas = RumorUIHelper.CreateScreenCanvas("RumorsCanvas", 10);
            CreateUI(_canvas.transform);
        }

        private void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;
            var esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(esObj);
        }

        private void CreateUI(Transform canvasRoot)
        {
            // Order matters: each controller's BuildUI adds to canvas
            var topBar = new GameObject("[TopBarUI]");
            topBar.transform.SetParent(canvasRoot, false);
            var tb = topBar.AddComponent<TopBarUI>();
            tb.BuildUI(canvasRoot);

            var buildMenu = new GameObject("[BuildMenuUI]");
            buildMenu.transform.SetParent(canvasRoot, false);
            var bm = buildMenu.AddComponent<BuildMenuUI>();
            bm.BuildUI(canvasRoot);

            var entityDetail = new GameObject("[EntityDetailUI]");
            entityDetail.transform.SetParent(canvasRoot, false);
            var ed = entityDetail.AddComponent<EntityDetailUI>();
            ed.BuildUI(canvasRoot);

            var monsterPanel = new GameObject("[MonsterPanelUI]");
            monsterPanel.transform.SetParent(canvasRoot, false);
            var mp = monsterPanel.AddComponent<MonsterPanelUI>();
            mp.BuildUI(canvasRoot);

            var monsterBar = new GameObject("[MonsterListBarUI]");
            monsterBar.transform.SetParent(canvasRoot, false);
            var mb = monsterBar.AddComponent<MonsterListBarUI>();
            mb.BuildUI(canvasRoot);

            var dailyUI = new GameObject("[DailySettlementUI]");
            dailyUI.transform.SetParent(canvasRoot, false);
            var ds = dailyUI.AddComponent<DailySettlementUI>();
            ds.BuildUI(canvasRoot);

            var sceneNav = new GameObject("[SceneNavigatorUI]");
            sceneNav.transform.SetParent(canvasRoot, false);
            var sn = sceneNav.AddComponent<SceneNavigatorUI>();
            sn.BuildUI(canvasRoot);

            var rumorPanel = new GameObject("[RumorPanelUI]");
            rumorPanel.transform.SetParent(canvasRoot, false);
            var rp = rumorPanel.AddComponent<RumorPanelUI>();
            rp.BuildUI(canvasRoot);
        }
    }
}
