using UnityEngine;
using UnityEngine.EventSystems;
using Rumors.Core;
using Rumors.Buildings;
using Rumors.Monsters;
using Rumors.UI;

namespace Rumors.World
{
    /// <summary>
    /// Handles world tile clicks and hover highlights.
    /// Uses legacy Input API (activeInputHandler = Both).
    /// </summary>
    public class WorldClickController : MonoBehaviour
    {
        public static WorldClickController Instance { get; private set; }

        private Camera _cam;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
            _cam = Camera.main;
        }

        private void Update()
        {
            if (_cam == null) { _cam = Camera.main; return; }

            // Block UI clicks
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                TilemapVisualizer.Instance?.SetHighlightPosition(Vector2Int.zero, false);
                return;
            }

            Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            UpdateHoverHighlight(mouseWorld);

            if (Input.GetMouseButtonDown(0))
                HandleClick(mouseWorld);
        }

        private void UpdateHoverHighlight(Vector3 mouseWorld)
        {
            if (WorldGrid.Instance == null || TilemapVisualizer.Instance == null) return;
            Vector2Int gp = WorldGrid.Instance.WorldToGrid(mouseWorld);
            bool valid = WorldGrid.Instance.IsValidGridPos(gp);
            TilemapVisualizer.Instance.SetHighlightPosition(gp, valid);
        }

        private void HandleClick(Vector3 mouseWorld)
        {
            if (WorldGrid.Instance == null) return;
            Vector2Int gp = WorldGrid.Instance.WorldToGrid(mouseWorld);
            if (!WorldGrid.Instance.IsValidGridPos(gp)) return;

            // Priority 1: MonsterAgent at this world pos
            Collider2D col = Physics2D.OverlapPoint(mouseWorld);
            if (col != null)
            {
                var agent = col.GetComponent<MonsterAgent>();
                if (agent != null)
                {
                    MonsterPanelUI.Instance?.Show(agent.Data);
                    return;
                }
            }

            // Priority 2: Building
            var building = BuildingManager.Instance?.GetAt(gp);
            if (building != null)
            {
                if (building.Type == BuildingType.CastleCore)
                    SceneNavigatorUI.Instance?.ShowCastleGateMenu();
                else
                    EntityDetailUI.Instance?.ShowBuilding(building);
                return;
            }

            // Priority 3: Empty tile → open build menu
            if (WorldGrid.Instance.IsBuildable(gp))
                BuildMenuUI.Instance?.Open(gp);
        }
    }
}
