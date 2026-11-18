using UnityEngine;
using Rumors.Core;

namespace Rumors.World
{
    /// <summary>
    /// Renders the tilemap grid. Each tile gets a colored SpriteRenderer.
    /// </summary>
    public class TilemapVisualizer : MonoBehaviour
    {
        public static TilemapVisualizer Instance { get; private set; }

        private Transform    _tiles;
        private GameObject   _highlight;
        private SpriteRenderer _hlSr;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            RebuildTiles();
            CreateHighlight();
        }

        // ── Build Visual Tiles & Environment Props ─────────────────────────
        private void RebuildTiles()
        {
            if (_tiles != null) Destroy(_tiles.gameObject);
            _tiles = new GameObject("TilesRoot").transform;
            _tiles.SetParent(transform);

            var grid = WorldGrid.Instance;
            if (grid == null) return;

            var propsRoot = new GameObject("PropsRoot").transform;
            propsRoot.SetParent(transform);

            for (int y = 0; y < WorldGrid.Height; y++)
            for (int x = 0; x < WorldGrid.Width;  x++)
            {
                var gp   = new Vector2Int(x, y);
                var node = grid.GetNode(gp);
                if (node == null) continue;

                var go = new GameObject($"T_{x}_{y}");
                go.transform.SetParent(_tiles);
                go.transform.position = node.WorldPos;

                // 1. Base Layer (Lush DarkGrass background to prevent dark void seams)
                if (node.Type != TileType.DarkGrass)
                {
                    var baseGo = new GameObject($"Base_{x}_{y}");
                    baseGo.transform.SetParent(go.transform, false);
                    baseGo.transform.localPosition = Vector3.zero;
                    var baseSr = baseGo.AddComponent<SpriteRenderer>();
                    baseSr.sprite = SpriteRegistry.GetTile(TileType.DarkGrass, gp);
                    baseSr.color = Color.white;
                    baseSr.sortingOrder = -100;
                    if (SpriteRegistry.DefaultMaterial != null)
                        baseSr.material = SpriteRegistry.DefaultMaterial;
                }

                // 2. Active Terrain Layer
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite       = SpriteRegistry.GetTile(node.Type, gp);
                sr.color        = Color.white;
                sr.sortingOrder = (node.Type == TileType.CastleFloor || node.Type == TileType.Cobblestone) ? -90 : -95;
                if (SpriteRegistry.DefaultMaterial != null)
                    sr.material = SpriteRegistry.DefaultMaterial;

                var col = go.AddComponent<BoxCollider2D>();
                col.size     = Vector2.one * WorldGrid.CellSize;
                col.isTrigger= true;

                var rec = go.AddComponent<TileReceiver>();
                rec.GridPos = gp;

                // ── Environment Props Instantiation (Reference 1, 2, 4, 5) ──
                SpawnTileProps(propsRoot, node.Type, gp, node.WorldPos);
            }
        }

        private void SpawnTileProps(Transform parent, TileType type, Vector2Int gp, Vector3 wp)
        {
            int cx = WorldGrid.CastleGridPos.x, cy = WorldGrid.CastleGridPos.y;
            int dx = Mathf.Abs(gp.x - cx), dy = Mathf.Abs(gp.y - cy);

            // 1. Castle Walls (Stone masonry walls along fortress boundary)
            if (type == TileType.CastleWall)
            {
                CreateProp(parent, "Prop_Wall", "Wall", wp + new Vector3(0f, 0.25f, 0f), 5);
                return;
            }

            // 2. Torches & Braziers at castle entrance gateways
            if ((dx == 2 && dy == 2) || (dx == 3 && dy == 0) || (dx == 0 && dy == 3))
            {
                CreateProp(parent, "Prop_Torch", "Torch", wp + new Vector3(0.35f, 0.2f, 0f), 6);
                return;
            }

            // 3. Storage Barrels beside courtyard path
            if (dx == 3 && dy == 1)
            {
                CreateProp(parent, "Prop_Barrel", "Barrel", wp + new Vector3(-0.2f, 0f, 0f), -40);
                return;
            }

            // 4. Lush Trees in Forest Zones (Reference 1, 2 & 5 trees)
            if (type == TileType.Forest || type == TileType.DeepForest)
            {
                int hash = (gp.x * 17 + gp.y * 31) % 10;
                if (hash < 6)
                {
                    // Tree spans 2x3 units, offset upward so base roots touch tile
                    CreateProp(parent, "Prop_Tree", "Tree", wp + new Vector3(0f, 0.8f, 0f), 5);
                }
                else
                {
                    CreateProp(parent, "Prop_Bush", "Bush", wp, -50);
                }
                return;
            }

            // 5. Scattered Bushes in Meadow/Grassland
            if (type == TileType.DarkGrass)
            {
                int hash = (gp.x * 23 + gp.y * 7) % 12;
                if (hash == 0)
                {
                    CreateProp(parent, "Prop_Bush", "Bush", wp + new Vector3(0.2f, -0.1f, 0f), -50);
                }
            }
        }

        private GameObject CreateProp(Transform parent, string propSpriteName, string goName, Vector3 pos, int sortingOrder)
        {
            var pgo = new GameObject($"P_{goName}_{pos.x:F1}_{pos.y:F1}");
            pgo.transform.SetParent(parent);
            pgo.transform.position = pos;

            var sr = pgo.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteRegistry.GetProp(propSpriteName.Replace("Prop_", ""));
            sr.color = Color.white;
            sr.sortingOrder = sortingOrder;
            if (SpriteRegistry.DefaultMaterial != null)
                sr.material = SpriteRegistry.DefaultMaterial;

            return pgo;
        }

        // ── Highlight Cursor ───────────────────────────────────────────────
        private void CreateHighlight()
        {
            _highlight = new GameObject("HighlightCursor");
            _highlight.transform.SetParent(transform);
            _hlSr = _highlight.AddComponent<SpriteRenderer>();
            _hlSr.sprite       = SpriteRegistry.GetHighlight();
            _hlSr.sortingOrder = 60;
            if (SpriteRegistry.DefaultMaterial != null)
                _hlSr.material = SpriteRegistry.DefaultMaterial;
            _highlight.transform.localScale = Vector3.one * WorldGrid.CellSize;
            _highlight.SetActive(false);
        }

        public void SetHighlightPosition(Vector2Int gp, bool valid) => SetHighlight(gp, valid, valid);

        public void SetHighlight(Vector2Int gp, bool show, bool canBuild = true)
        {
            if (!show || !WorldGrid.Instance.Valid(gp))
            { _highlight.SetActive(false); return; }

            var node = WorldGrid.Instance.GetNode(gp);
            if (node == null) { _highlight.SetActive(false); return; }

            _highlight.transform.position = node.WorldPos;
            _hlSr.color = canBuild
                ? new Color(0.3f, 1f, 0.4f, 0.55f)
                : new Color(1f, 0.2f, 0.2f, 0.45f);
            _highlight.SetActive(true);
        }

        // ── Tile Color Palette ─────────────────────────────────────────────
        private Color TileColor(TileType t) => t switch
        {
            TileType.Swamp        => new Color(0.92f, 1f, 0.94f),
            TileType.SwampDeep    => new Color(0.85f, 0.95f, 0.9f),
            TileType.MineralVein  => new Color(0.98f, 0.92f, 1f),
            TileType.CastleWall   => new Color(0.85f, 0.85f, 0.92f),
            _                     => Color.white,
        };
    }

    public class TileReceiver : MonoBehaviour
    {
        public Vector2Int GridPos;
    }
}
