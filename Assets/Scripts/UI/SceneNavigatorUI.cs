using UnityEngine;
using UnityEngine.UI;
using Rumors.Core;

namespace Rumors.UI
{
    /// <summary>
    /// Popup menu when player clicks the Castle Core — navigate to sub-areas.
    /// </summary>
    public class SceneNavigatorUI : MonoBehaviour
    {
        public static SceneNavigatorUI Instance { get; private set; }

        private GameObject _root;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void BuildUI(Transform canvasRoot)
        {
            _root = RumorUIHelper.CreateSizedPanel(canvasRoot, "SceneNavigatorPanel",
                new Vector2(340f, 280f), Vector2.zero,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Color.white, null, new Vector2(0.5f, 0.5f));

            RumorUIHelper.CreateText(_root.transform, "Title", "✦ 마왕성 집무실 ✦",
                new Vector2(0f, -22f), new Vector2(300f, 32f), 20,
                new Color(0.98f, 0.88f, 0.45f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            RumorUIHelper.CreateButton(_root.transform, "CloseBtn", "✕",
                new Vector2(34f, 30f), new Vector2(-12f, -12f),
                Hide, new Color(0.7f, 0.2f, 0.2f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            float y = -75f;
            MakeNavButton("🏘  저잣거리 (인간 소문)", "인간 사회의 소문과 용사 정보를 확인합니다. (준비 중)", y);
            MakeNavButton("⚔  던전 관전 (자동전투)", "침입한 용사와의 전투를 관전합니다. (준비 중)", y - 62f);
            MakeNavButton("🔬  마력 연구소 (소문 실체화)", "소문을 몬스터로 실체화하는 연구실. (준비 중)", y - 124f);

            _root.SetActive(false);
        }

        private void MakeNavButton(string label, string tooltip, float y)
        {
            RumorUIHelper.CreateButton(_root.transform, "Nav_" + label, label,
                new Vector2(290f, 46f), new Vector2(0f, y),
                () => ShowTooltip(tooltip),
                Color.white,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        }

        private void ShowTooltip(string msg)
        {
            Debug.Log("[RUMORS] " + msg);
        }

        public void ShowCastleGateMenu() => _root.SetActive(true);

        public void Hide() => _root.SetActive(false);
    }
}
