using System.Collections.Generic;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    /// <summary>
    /// Runtime sprite cache. Loads from Resources/ProductionArt first, then Resources fallback.
    /// All sprites use Point filter (set in meta or via code).
    /// </summary>
    public static class SpriteRegistry
    {
        private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        private static Material _unlitMat;

        public static Material DefaultMaterial
        {
            get
            {
                if (_unlitMat == null)
                {
                    var shader = Shader.Find("Sprites/Default");
                    if (shader != null) _unlitMat = new Material(shader);
                }
                return _unlitMat;
            }
        }

        public static Sprite Load(string path, float targetPPU = 16f)
        {
            if (cache.TryGetValue(path, out var hit) && hit != null) return hit;

            // Load primary from Resources, fallback to ProductionArt
            Sprite s = Resources.Load<Sprite>(path)
                    ?? Resources.Load<Sprite>("ProductionArt/" + path);

            // Fallback: If imported as Texture2D instead of Sprite, create sprite dynamically
            if (s == null)
            {
                Texture2D tex = Resources.Load<Texture2D>(path)
                             ?? Resources.Load<Texture2D>("ProductionArt/" + path);
                if (tex != null)
                {
                    tex.filterMode = FilterMode.Point;
                    s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f), targetPPU);
                }
            }

            if (s != null)
            {
                if (s.texture != null)
                    s.texture.filterMode = FilterMode.Point;

                // Ensure exact pixel-per-unit scale at runtime
                if (!Mathf.Approximately(s.pixelsPerUnit, targetPPU))
                {
                    s = Sprite.Create(s.texture, new Rect(0, 0, s.texture.width, s.texture.height),
                        new Vector2(0.5f, 0.5f), targetPPU);
                }
                cache[path] = s;
            }
            else
            {
                Debug.LogWarning($"[SpriteRegistry] Sprite asset not found at Resources path: {path}");
            }
            return s;
        }

        public static Sprite GetProp(string name)
        {
            return Load("Props/Prop_" + name, 16f);
        }

        /// <summary>
        /// Loads a single tile slice from an external spritesheet / atlas PNG file.
        /// Enables seamless drop-in replacement with high-quality third-party tilesets.
        /// </summary>
        public static Sprite LoadFromAtlas(string atlasPath, int tileX, int tileY, int tileSize = 16, float targetPPU = 16f)
        {
            string cacheKey = $"{atlasPath}_{tileX}_{tileY}_{tileSize}";
            if (cache.TryGetValue(cacheKey, out var hit) && hit != null) return hit;

            Texture2D tex = Resources.Load<Texture2D>(atlasPath)
                         ?? Resources.Load<Texture2D>("ProductionArt/" + atlasPath)
                         ?? Resources.Load<Texture2D>("Tilesets/" + atlasPath);

            if (tex != null)
            {
                tex.filterMode = FilterMode.Point;
                int px = tileX * tileSize;
                // Unity Texture Y-axis starts from bottom
                int py = tex.height - (tileY + 1) * tileSize;
                if (px >= 0 && px + tileSize <= tex.width && py >= 0 && py + tileSize <= tex.height)
                {
                    var s = Sprite.Create(tex, new Rect(px, py, tileSize, tileSize), new Vector2(0.5f, 0.5f), targetPPU);
                    cache[cacheKey] = s;
                    return s;
                }
            }
            return null;
        }

        public static Sprite GetTile(TileType type)
        {
            return GetTile(type, Vector2Int.zero);
        }

        public static Sprite GetTile(TileType type, Vector2Int gridPos)
        {
            // Check for optional external tileset atlas first ("Tilesets/Terrain_Atlas")
            // If absent, cleanly falls back to individual resource files.
            int variant = (gridPos.x * 17 + gridPos.y * 31) % 4;
            string variantPath = variant > 0 ? $"{GetTileResourceName(type)}_{variant}" : null;
            if (variantPath != null)
            {
                var vs = Load(variantPath, 16f);
                if (vs != null) return vs;
            }

            return Load(GetTileResourceName(type), 16f);
        }

        private static string GetTileResourceName(TileType type)
        {
            return type switch
            {
                TileType.CastleFloor  => "Tiles/Tile_Castle_Floor",
                TileType.Cobblestone  => "Tiles/Tile_Cobblestone",
                TileType.CastleWall   => "Tiles/Tile_Ground_Stone",
                TileType.DarkGrass    => "Tiles/Tile_Ground_DarkGrass",
                TileType.Forest       => "Tiles/Tile_Ground_DarkGrass",
                TileType.DeepForest   => "Tiles/Tile_Ground_DarkGrass",
                TileType.Dirt         => "Tiles/Tile_Ground_Dirt",
                TileType.Wasteland    => "Tiles/Tile_Ground_Dirt",
                TileType.Stone        => "Tiles/Tile_Ground_Stone",
                TileType.MineralVein  => "Tiles/Tile_Ground_Stone",
                TileType.Swamp        => "Tiles/Tile_Ground_Swamp",
                TileType.SwampDeep    => "Tiles/Tile_Ground_Swamp",
                TileType.Road         => "Tiles/Tile_Cobblestone",
                _                     => "Tiles/Tile_Ground_DarkGrass",
            };
        }

        public static Sprite GetHighlight() => Load("Tiles/Tile_Highlight", 16f);

        public static Sprite GetBuilding(BuildingType type, bool constructed = true, int level = 1)
        {
            if (!constructed) return Load("Buildings/Building_Construction", 16f);

            if (level >= 2)
            {
                var upSprite = type switch
                {
                    BuildingType.CastleCore   => Load("Buildings/Building_Castle_Upgraded", 16f),
                    BuildingType.Mine         => Load("Buildings/Building_Mine_Upgraded", 16f),
                    BuildingType.MushroomFarm => Load("Buildings/Building_Farm_Upgraded", 16f),
                    BuildingType.Dormitory    => Load("Buildings/Building_Dormitory_Upgraded", 16f),
                    BuildingType.Warehouse    => Load("Buildings/Building_Warehouse_Upgraded", 16f),
                    _                         => null,
                };
                if (upSprite != null) return upSprite;
            }

            return type switch
            {
                BuildingType.CastleCore     => Load("Buildings/Building_Castle", 16f),
                BuildingType.Mine           => Load("Buildings/Building_Mine", 16f),
                BuildingType.MushroomFarm   => Load("Buildings/Building_Farm", 16f),
                BuildingType.Dormitory      => Load("Buildings/Building_Dormitory", 16f),
                BuildingType.Warehouse      => Load("Buildings/Building_Warehouse", 16f),
                BuildingType.ManaTower      => Load("Buildings/Building_ManaTower", 16f),
                BuildingType.TrainingGround => Load("Buildings/Building_Training", 16f),
                _                           => Load("Buildings/Building_Construction", 16f),
            };
        }

        public static Sprite GetBuilding(string name, int level = 1)
        {
            if (System.Enum.TryParse<BuildingType>(name, true, out var t))
                return GetBuilding(t, true, level);
            if (level >= 2)
            {
                var up = Load($"Buildings/Building_{name}_Upgraded", 16f);
                if (up != null) return up;
            }
            return Load($"Buildings/Building_{name}", 16f) ?? Load("Buildings/Building_Construction", 16f);
        }

        public static Sprite GetMonster(MonsterSpecies species)
        {
            return species switch
            {
                MonsterSpecies.ImpScrap     => Load("Monsters/Monster_ImpScrap", 32f),
                MonsterSpecies.MireSlime    => Load("Monsters/Monster_MireSlime", 32f),
                MonsterSpecies.BonePup      => Load("Monsters/Monster_BonePup", 32f),
                MonsterSpecies.GrimShadow   => Load("Monsters/Monster_GrimShadow", 32f),
                MonsterSpecies.GoblinTinker => Load("Monsters/Monster_GoblinTinker", 32f),
                MonsterSpecies.RumorBeast   => Load("Monsters/Monster_GrimShadow", 32f),
                _                           => Load("Monsters/Monster_ImpScrap", 32f),
            };
        }

        public static Sprite GetMonsterFrame(MonsterSpecies species, string anim, int frame)
        {
            string key = $"Monsters/Monster_{species}/{anim}_{frame}";
            return Load(key, 32f) ?? GetMonster(species);
        }

        public static Sprite GetMonster(string name)
        {
            if (System.Enum.TryParse<MonsterSpecies>(name, true, out var s))
                return GetMonster(s);
            return Load($"Monsters/Monster_{name}", 32f) ?? Load("Monsters/Monster_ImpScrap", 32f);
        }

        public static Sprite GetIcon(string name)   => Load($"UI/Icon_{name}", 16f);
        public static Sprite GetPanel()             => Load("UI/UI_Panel_9Slice", 16f);
        public static Sprite GetPanelSprite()       => GetPanel();
        public static Sprite GetButton(string state = "Normal")
            => Load($"UI/UI_Button_{state}", 16f) ?? Load("UI/UI_Panel_9Slice", 16f);
    }
}
