using UnityEngine;
using Rumors.Core;

namespace Rumors.World
{
    public class TileNode
    {
        public Vector2Int GridPos       { get; set; }
        public Vector3    WorldPos      { get; set; }
        public TileType   Type          { get; set; }
        public bool       IsBuildable   { get; set; }
        public bool       IsWalkable    { get; set; }
        public Rumors.Buildings.Building OccupyingBuilding { get; set; }
    }

    public class WorldGrid : MonoBehaviour
    {
        public static WorldGrid Instance { get; private set; }

        public const int   Width    = 25;
        public const int   Height   = 25;
        public const int   GridWidth = Width;
        public const int   GridHeight = Height;
        public const float CellSize = 1f;

        public static readonly Vector2Int CastleGridPos = new Vector2Int(12, 12);

        private TileNode[,] _nodes;

        public System.Action<Vector2Int, Rumors.Buildings.Building> OnBuildingSet;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            Build();
        }

        // ── Grid Init ──────────────────────────────────────────────────────
        private void Build()
        {
            _nodes = new TileNode[Width, Height];
            var origin = Origin();
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width;  x++)
            {
                var gp = new Vector2Int(x, y);
                var wp = origin + new Vector3(x * CellSize, y * CellSize);
                var tt = CalcType(x, y);
                int dx = Mathf.Abs(x - CastleGridPos.x);
                int dy = Mathf.Abs(y - CastleGridPos.y);

                _nodes[x,y] = new TileNode
                {
                    GridPos     = gp,
                    WorldPos    = wp,
                    Type        = tt,
                    IsBuildable = CanBuild(tt, dx, dy),
                    IsWalkable  = tt != TileType.CastleWall,
                };
            }
        }

        private bool CanBuild(TileType t, int dx, int dy)
        {
            if (dx <= 1 && dy <= 1) return false;   // castle footprint
            return t switch {
                TileType.CastleFloor => false,
                TileType.CastleWall  => false,
                TileType.DeepForest  => false,
                TileType.SwampDeep   => false,
                _ => true,
            };
        }

        private TileType CalcType(int x, int y)
        {
            int cx = CastleGridPos.x, cy = CastleGridPos.y;
            int dx = Mathf.Abs(x - cx), dy = Mathf.Abs(y - cy);
            int dist = dx + dy;

            // 1. Castle interior floor (3x3 core)
            if (dx <= 1 && dy <= 1) return TileType.CastleFloor;

            // 2. Castle boundary wall pillars (4 corners)
            if (dx == 4 && dy == 4) return TileType.CastleWall;

            // 3. Castle courtyard & entrance avenue (Cobblestone flagstones)
            // Central courtyard around keep + grand entrance avenue leading South
            if ((dx <= 3 && dy <= 3 && dist <= 5) || (dx <= 1 && y < cy && dy <= 6))
                return TileType.Cobblestone;

            int seed = (x * 7 + y * 13 + x * y * 3) % 100;

            // 4. North rocky crags / mineral outcrop (Deep North isolated outcrop, reduced from 25% to ~5%)
            if (y >= cy + 7 && dx <= 3)
            {
                return (seed < 40) ? TileType.MineralVein : TileType.Stone;
            }

            // 5. West deep forest grove
            if (x <= cx - 6 && dy <= 7)
            {
                return (seed < 50) ? TileType.DeepForest : TileType.Forest;
            }

            // 6. South murky swamp border (far south perimeter)
            if (y <= cy - 8 && dx <= 7)
            {
                return (seed < 40) ? TileType.SwampDeep : TileType.Swamp;
            }

            // 7. Natural organic paths / trodden dirt roads connecting zones
            // Eastern trade path and northern mine trail
            if ((dy <= 1 && x > cx + 3) || (dx <= 1 && y > cy + 3 && y < cy + 7))
            {
                return TileType.Dirt;
            }

            // 8. General terrain: Dominant lush Olive Grassland with natural dirt clearings
            if (seed < 22)
            {
                return TileType.Dirt;
            }
            if (seed < 32 && (x < cx - 2 || y > cy + 2))
            {
                return TileType.Forest;
            }

            return TileType.DarkGrass;
        }

        private Vector3 Origin()
            => new Vector3(-Width * 0.5f * CellSize + 0.5f * CellSize,
                           -Height * 0.5f * CellSize + 0.5f * CellSize);

        // ── Public API ─────────────────────────────────────────────────────
        public bool      Valid(Vector2Int p) => p.x>=0 && p.x<Width && p.y>=0 && p.y<Height;
        public bool      IsValidGridPos(Vector2Int p) => Valid(p);
        public TileNode  GetNode(Vector2Int p) => Valid(p) ? _nodes[p.x,p.y] : null;

        public Vector2Int WorldToGrid(Vector3 w)
        {
            var o = Origin();
            return new Vector2Int(
                Mathf.RoundToInt((w.x-o.x)/CellSize),
                Mathf.RoundToInt((w.y-o.y)/CellSize));
        }

        public Vector3 GridToWorld(Vector2Int g)
        {
            var o = Origin();
            return o + new Vector3(g.x*CellSize, g.y*CellSize);
        }

        public bool IsBuildable(Vector2Int p)
        {
            var n = GetNode(p);
            return n != null && n.IsBuildable && n.OccupyingBuilding == null;
        }

        public void SetBuilding(Vector2Int p, Rumors.Buildings.Building b)
        {
            var n = GetNode(p);
            if (n != null) { n.OccupyingBuilding = b; OnBuildingSet?.Invoke(p, b); }
        }

        public void ClearBuilding(Vector2Int p)
        {
            var n = GetNode(p);
            if (n != null) { n.OccupyingBuilding = null; OnBuildingSet?.Invoke(p, null); }
        }
    }
}
