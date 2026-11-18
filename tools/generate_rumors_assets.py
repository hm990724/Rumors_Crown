"""
RUMORS — Phase 0 Pixel Art Asset Generator
Generates placeholder-quality but visually distinct pixel sprites
for tiles, buildings, and monsters.

All sprites use a consistent dark fantasy palette.
"""

from PIL import Image, ImageDraw
import os, math, random

# ─── Output Directories ─────────────────────────────────────────────────────
ROOT = r"C:\Users\lookm\Documents\GitHub\Rumors_Crown\Assets\Resources"
TILES_DIR     = os.path.join(ROOT, "Tiles")
BUILDINGS_DIR = os.path.join(ROOT, "Buildings")
MONSTERS_DIR  = os.path.join(ROOT, "Monsters")
UI_DIR        = os.path.join(ROOT, "UI")

for d in [TILES_DIR, BUILDINGS_DIR, MONSTERS_DIR, UI_DIR]:
    os.makedirs(d, exist_ok=True)

# ─── Palette ────────────────────────────────────────────────────────────────
# Dark Fantasy palette — consistent across all sprites
P = {
    # Ground
    "dark_grass":   (25, 42, 28),
    "grass":        (38, 65, 40),
    "grass_hi":     (55, 90, 55),
    "dirt":         (72, 52, 32),
    "dirt_hi":      (96, 72, 46),
    "stone":        (60, 58, 68),
    "stone_hi":     (85, 82, 92),
    "swamp":        (30, 52, 38),
    "swamp_water":  (22, 48, 44),
    "cobble":       (55, 50, 68),
    "cobble_hi":    (75, 70, 90),
    "castle_floor": (42, 38, 55),
    "castle_hi":    (60, 55, 75),

    # Building
    "black":        (12, 10, 16),
    "dark_purple":  (40, 22, 60),
    "purple":       (88, 44, 128),
    "purple_hi":    (130, 80, 180),
    "mana_glow":    (160, 100, 220),
    "dark_wood":    (45, 30, 18),
    "wood":         (72, 48, 28),
    "wood_hi":      (100, 68, 42),
    "dark_stone":   (38, 36, 44),
    "mid_stone":    (60, 58, 68),
    "light_stone":  (90, 88, 100),
    "ore":          (88, 70, 110),
    "ore_vein":     (120, 90, 160),
    "iron":         (68, 72, 80),
    "iron_hi":      (110, 115, 128),
    "gold_ore":     (180, 148, 48),
    "gold":         (220, 180, 60),
    "flag_red":     (160, 32, 32),
    "flag_dark":    (100, 20, 20),
    "mushroom_cap": (200, 80, 180),
    "mushroom_stem":(180, 150, 120),
    "mushroom_glow":(230, 120, 210),
    "dark_green":   (22, 55, 28),
    "tree_dark":    (18, 45, 22),
    "tree_mid":     (28, 68, 32),
    "tree_hi":      (45, 92, 48),

    # Monster
    "imp_body":     (200, 80, 60),
    "imp_dark":     (140, 50, 35),
    "imp_horn":     (220, 180, 80),
    "imp_eye":      (240, 220, 60),
    "imp_wing":     (160, 60, 50),

    "slime_body":   (60, 180, 100),
    "slime_dark":   (30, 120, 65),
    "slime_hi":     (140, 230, 160),
    "slime_eye":    (20, 60, 30),

    "bone_body":    (220, 210, 180),
    "bone_dark":    (160, 150, 120),
    "bone_hi":      (240, 235, 215),
    "bone_eye":     (60, 200, 220),

    "shadow_body":  (40, 20, 65),
    "shadow_dark":  (20, 10, 40),
    "shadow_hi":    (100, 60, 160),
    "shadow_eye":   (200, 160, 255),
    "shadow_smoke": (70, 40, 110),

    "goblin_body":  (100, 155, 60),
    "goblin_dark":  (65, 100, 38),
    "goblin_hi":    (140, 200, 90),
    "goblin_eye":   (255, 220, 60),
    "goblin_gear":  (140, 120, 80),
    "goblin_metal": (160, 165, 175),

    # UI
    "ui_bg":        (18, 14, 26),
    "ui_border":    (80, 60, 110),
    "ui_border_hi": (120, 90, 160),
    "ui_text":      (220, 210, 240),

    # Common
    "white":        (255, 255, 255),
    "transparent":  (0, 0, 0, 0),
}

def rgba(key, a=255):
    c = P[key]
    return (c[0], c[1], c[2], a)

def save(img, path):
    img.save(path)
    print(f"  SAVED: {os.path.basename(path)}")

# ─── Helper: set pixel with bounds check ────────────────────────────────────
def px(d, x, y, color, size=None):
    if size:
        if 0 <= x < size[0] and 0 <= y < size[1]:
            d.point((x, y), fill=color)
    else:
        d.point((x, y), fill=color)

def block(d, x, y, w, h, color):
    d.rectangle([x, y, x+w-1, y+h-1], fill=color)

def vline(d, x, y1, y2, color):
    d.line([(x, y1), (x, y2)], fill=color)

def hline(d, x1, x2, y, color):
    d.line([(x1, y), (x2, y)], fill=color)

# ════════════════════════════════════════════════════════════════════════════
# TILES  (16×16)
# ════════════════════════════════════════════════════════════════════════════

def make_tile_dark_grass():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    block(d, 0, 0, 16, 16, rgba("dark_grass"))
    # Grass tufts
    for (x, y) in [(2,2),(7,1),(12,3),(1,8),(9,7),(14,10),(4,13),(11,12),(6,5)]:
        d.point((x, y), rgba("grass"))
        if y > 0: d.point((x, y-1), rgba("grass_hi"))
    # Subtle variation dots
    for (x, y) in [(3,5),(8,9),(13,6),(5,12),(10,14),(2,11)]:
        d.point((x, y), rgba("grass"))
    return img

def make_tile_dirt():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    block(d, 0, 0, 16, 16, rgba("dirt"))
    for (x, y) in [(3,2),(10,4),(6,8),(13,11),(2,13),(8,1),(14,7)]:
        d.point((x, y), rgba("dirt_hi"))
    for (x, y) in [(1,6),(7,10),(12,2),(5,14),(9,5)]:
        d.point((x, y), (55, 38, 22, 255))  # darker
    return img

def make_tile_stone():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    block(d, 0, 0, 16, 16, rgba("stone"))
    # Stone crack pattern
    hline(d, 2, 7, 5, rgba("dark_stone"))
    hline(d, 9, 14, 5, rgba("dark_stone"))
    hline(d, 2, 10, 11, rgba("dark_stone"))
    vline(d, 8, 0, 5, rgba("dark_stone"))
    vline(d, 3, 5, 11, rgba("dark_stone"))
    vline(d, 11, 5, 11, rgba("dark_stone"))
    # Highlights on top of blocks
    hline(d, 2, 7, 1, rgba("stone_hi"))
    hline(d, 9, 14, 1, rgba("stone_hi"))
    hline(d, 2, 10, 7, rgba("stone_hi"))
    return img

def make_tile_swamp():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    block(d, 0, 0, 16, 16, rgba("swamp"))
    # Water pools
    block(d, 3, 4, 4, 3, rgba("swamp_water"))
    block(d, 10, 9, 4, 3, rgba("swamp_water"))
    d.point((4, 5), rgba("slime_hi"))   # small glint
    d.point((11, 10), rgba("slime_hi"))
    # Bubbles
    for (x, y) in [(5, 3), (12, 8), (2, 11), (13, 13)]:
        d.point((x, y), rgba("slime_body"))
    return img

def make_tile_castle_floor():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    block(d, 0, 0, 16, 16, rgba("castle_floor"))
    # Tile grout lines
    hline(d, 0, 15, 7, rgba("dark_stone"))
    vline(d, 7, 0, 7, rgba("dark_stone"))
    vline(d, 7, 8, 15, (30, 26, 42, 255))
    # Slight highlights on tile corners
    for (x, y) in [(1,1),(9,1),(1,9),(9,9)]:
        d.point((x, y), rgba("castle_hi"))
    return img

def make_tile_cobblestone():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    block(d, 0, 0, 16, 16, (38, 35, 48, 255))
    # Cobble stones
    stones = [(1,1,5,4),(7,1,6,4),(1,6,5,4),(7,6,7,4),(1,11,5,4),(7,11,7,4)]
    for (x,y,w,h) in stones:
        block(d, x, y, w, h, rgba("cobble"))
        hline(d, x, x+w-2, y, rgba("cobble_hi"))
        d.point((x, y+h-1), rgba("dark_stone"))
    return img

def make_tile_highlight():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    # Dashed border
    for i in range(0, 16, 2):
        d.point((i, 0),  (255, 220, 60, 200))
        d.point((i, 15), (255, 220, 60, 200))
        d.point((0,  i), (255, 220, 60, 200))
        d.point((15, i), (255, 220, 60, 200))
    block(d, 1, 1, 14, 14, (255, 220, 60, 50))
    return img

# ════════════════════════════════════════════════════════════════════════════
# BUILDINGS  (32×32 or 48×48)
# ════════════════════════════════════════════════════════════════════════════

def make_building_castle(size=48):
    """마왕성 — 검은 첨탑, 보랏빛 화염, 낡은 깃발"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    s = size
    # Base walls
    block(d, 4, 28, s-8, 20, rgba("dark_stone"))
    block(d, 6, 30, s-12, 18, rgba("mid_stone"))
    # Battlements
    for bx in [4, 10, 16, 22, 28, 34]:
        if bx < s-6:
            block(d, bx, 24, 4, 4, rgba("mid_stone"))
    # Main tower
    block(d, 16, 10, 16, 20, rgba("dark_purple"))
    block(d, 18, 12, 12, 18, rgba("dark_stone"))
    # Tower top / spire
    for i in range(10):
        w = max(2, 8-i)
        block(d, 24-w//2, 10-i, w, 1, rgba("black"))
    # Spire tip
    block(d, 23, 2, 2, 2, rgba("purple"))
    # Mana flame at top
    for (fx, fy, fc) in [(23,4,"mana_glow"),(24,3,"purple_hi"),(23,3,"purple_hi")]:
        d.point((fx, fy), rgba(fc, 200))
    # Gate arch
    block(d, 21, 32, 6, 10, rgba("black"))
    for i in range(3):
        d.point((21+i, 32-i), rgba("dark_stone"))
        d.point((26-i, 32-i), rgba("dark_stone"))
    # Flag
    vline(d, 23, 2, 10, rgba("flag_dark"))
    for i in range(5):
        block(d, 24, 4+i, 5-i, 1, rgba("flag_red"))
    # Windows
    block(d, 20, 18, 3, 4, rgba("mana_glow"))
    block(d, 25, 18, 3, 4, rgba("mana_glow"))
    # Wall detail
    for x in range(4, s-4, 6):
        d.point((x, 28), rgba("stone_hi"))
    # Shadow
    block(d, 4, 46, s-8, 2, (0,0,0,60))
    return img

def make_building_mine(size=32):
    """광산 — 암석 입구, 목재 지지대, 광석"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Rock base
    block(d, 0, 18, 32, 14, rgba("dark_stone"))
    block(d, 2, 16, 28, 16, rgba("mid_stone"))
    # Rock face detail
    for (x,y) in [(4,20),(10,18),(18,22),(26,19),(7,25),(20,26)]:
        d.point((x,y), rgba("stone_hi"))
    for (x,y) in [(6,22),(15,19),(23,24)]:
        d.point((x,y), rgba("ore_vein"))
    # Tunnel entrance
    block(d, 10, 10, 12, 8, rgba("black"))
    # Arch stones
    for i in range(5):
        d.point((10+i, 10-i//2), rgba("mid_stone"))
        d.point((21-i, 10-i//2), rgba("mid_stone"))
    # Wooden supports
    vline(d, 10, 10, 18, rgba("dark_wood"))
    vline(d, 21, 10, 18, rgba("dark_wood"))
    hline(d, 10, 21, 10, rgba("wood"))
    # Ore sparkle inside
    for (x,y) in [(14,14),(17,12),(15,16)]:
        d.point((x,y), rgba("gold_ore"))
    # Pick axe silhouette on side
    hline(d, 2, 7, 8, rgba("iron"))
    d.point((2, 7), rgba("iron_hi"))
    d.point((7, 9), rgba("dark_wood"))
    vline(d, 7, 9, 14, rgba("dark_wood"))
    # Cart
    block(d, 24, 22, 6, 4, rgba("wood"))
    block(d, 25, 23, 4, 2, rgba("dark_wood"))
    d.ellipse([24,25,26,27], fill=rgba("iron"))
    d.ellipse([27,25,29,27], fill=rgba("iron"))
    return img

def make_building_farm(size=32):
    """버섯 농장 — 거대 발광 버섯 + 재배 시설"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Soil base
    block(d, 0, 22, 32, 10, rgba("dirt"))
    block(d, 2, 20, 28, 12, (60,42,24,255))
    # Soil marks
    for x in range(4, 28, 4):
        d.point((x, 22), rgba("dirt_hi"))
    # Main big mushroom (center)
    # Stem
    block(d, 13, 10, 6, 12, rgba("mushroom_stem"))
    block(d, 14, 11, 4, 11, (200, 170, 135, 255))
    # Cap
    d.ellipse([6, 4, 26, 15], fill=rgba("mushroom_cap"))
    d.ellipse([7, 5, 25, 13], fill=rgba("mushroom_glow"))
    block(d, 7, 10, 18, 5, rgba("mushroom_cap"))
    # Cap highlight
    d.ellipse([10, 5, 18, 10], fill=rgba("mushroom_glow"))
    # Small mushrooms
    for (mx, my, h) in [(3, 18, 4), (26, 16, 6)]:
        block(d, mx, my, 4, h, rgba("mushroom_stem"))
        d.ellipse([mx-2, my-4, mx+5, my+1], fill=rgba("mushroom_cap"))
    # Glow dots
    for (x,y) in [(8,8),(18,7),(12,12),(22,15),(4,17),(27,14)]:
        d.point((x,y), rgba("mushroom_glow"))
    # Fence
    for fx in [1, 6, 11, 20, 25, 30]:
        vline(d, fx, 20, 24, rgba("dark_wood"))
    hline(d, 1, 30, 20, rgba("wood"))
    hline(d, 1, 30, 23, rgba("wood"))
    return img

def make_building_dormitory(size=32):
    """숙소 — 작은 나무 건물과 굴뚝"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Foundation
    block(d, 2, 24, 28, 8, rgba("mid_stone"))
    # Walls
    block(d, 4, 14, 24, 14, rgba("wood"))
    block(d, 5, 15, 22, 13, rgba("dark_wood"))
    # Wall plank lines
    for y in [17, 20, 23]:
        hline(d, 5, 26, y, rgba("wood"))
    # Roof
    for i in range(10):
        x0 = 2 + i
        x1 = 29 - i
        hline(d, x0, x1, 14-i, rgba("dark_stone"))
    # Roof highlight
    hline(d, 14, 17, 5, rgba("stone_hi"))
    # Chimney
    block(d, 24, 7, 4, 8, rgba("mid_stone"))
    block(d, 24, 5, 5, 3, rgba("mid_stone"))
    # Smoke puffs
    for (cx, cy, a) in [(25,4,160),(26,3,120),(27,2,80)]:
        d.ellipse([cx-1,cy-1,cx+1,cy+1], fill=(200,200,200,a))
    # Windows
    block(d, 7, 17, 5, 5, rgba("mana_glow"))
    block(d, 8, 18, 3, 3, (200,230,255,180))
    block(d, 20, 17, 5, 5, rgba("mana_glow"))
    block(d, 21, 18, 3, 3, (200,230,255,180))
    # Door
    block(d, 13, 21, 6, 7, rgba("dark_wood"))
    block(d, 14, 22, 4, 6, rgba("dark_stone"))
    d.point((16, 25), rgba("gold"))
    return img

def make_building_warehouse(size=32):
    """창고"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    block(d, 2, 20, 28, 12, rgba("mid_stone"))
    block(d, 4, 10, 24, 14, rgba("wood"))
    block(d, 5, 11, 22, 13, rgba("dark_wood"))
    # Plank lines
    for y in [13,16,19,22]:
        hline(d, 5, 26, y, rgba("wood"))
    # Barrel roof (arched)
    for i in range(6):
        hline(d, 4+i, 27-i, 10-i//2, rgba("dark_stone"))
    block(d, 10, 7, 12, 4, rgba("mid_stone"))
    # Big double door
    block(d, 8, 20, 7, 10, rgba("dark_wood"))
    block(d, 17, 20, 7, 10, rgba("dark_wood"))
    hline(d, 8, 23, 20, rgba("wood"))
    hline(d, 8, 23, 25, rgba("wood"))
    hline(d, 8, 23, 20, rgba("iron"))
    d.point((15, 25), rgba("iron_hi"))
    d.point((16, 25), rgba("iron_hi"))
    return img

def make_building_construction(size=32):
    """건설 중 플레이스홀더"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Base outline
    block(d, 4, 20, 24, 12, rgba("dirt"))
    # Scaffolding
    vline(d, 6, 4, 20, rgba("wood"))
    vline(d, 25, 4, 20, rgba("wood"))
    hline(d, 6, 25, 4, rgba("wood"))
    hline(d, 6, 25, 10, rgba("dark_wood"))
    hline(d, 6, 25, 16, rgba("dark_wood"))
    # Diagonal braces
    for y in [4, 10, 16]:
        d.line([(6,y),(25,y+6)], fill=rgba("dark_wood"))
    # Stacked stones
    for i in range(3):
        block(d, 10+i*4, 20-i*2, 3, 2, rgba("mid_stone"))
    # Warning stripes
    for i in range(0, 32, 4):
        d.point((i, 31), rgba("gold"))
        d.point((i+1, 31), (0,0,0,255))
    # Hammer icon
    block(d, 20, 6, 6, 3, rgba("iron"))
    block(d, 22, 9, 2, 6, rgba("dark_wood"))
    return img

def make_building_mana_tower(size=32):
    """마력 정제소"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Base ring
    block(d, 8, 24, 16, 8, rgba("dark_stone"))
    d.ellipse([6,22,25,28], fill=rgba("mid_stone"))
    # Tower body (octagonal feel)
    for i in range(14):
        w = 10 - abs(i-7)
        block(d, 11-(w//3), 10+i, w+(w//3)*2, 1, rgba("dark_purple"))
    # Crystal at top
    for i in range(6):
        w = max(1, 4-i)
        block(d, 15-w//2, 4+i, w, 1, rgba("purple_hi"))
    block(d, 14, 3, 4, 2, rgba("mana_glow"))
    # Mana orbits (3 dots around tower)
    for angle, color in [(0,"purple_hi"),(120,"mana_glow"),(240,"purple")]:
        a = math.radians(angle)
        cx = int(16 + 7 * math.cos(a))
        cy = int(17 + 7 * math.sin(a))
        d.ellipse([cx-2, cy-2, cx+2, cy+2], fill=rgba(color))
    # Glow lines on body
    for y in range(10, 24, 3):
        d.point((15, y), rgba("mana_glow"))
        d.point((16, y), rgba("mana_glow"))
    return img

def make_building_training(size=32):
    """훈련장"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    block(d, 2, 22, 28, 10, rgba("dark_stone"))
    # Sand arena floor
    block(d, 4, 20, 24, 12, (90, 68, 40, 255))
    block(d, 6, 22, 20, 8, (110, 85, 52, 255))
    # Arena border stones
    hline(d, 4, 27, 20, rgba("mid_stone"))
    hline(d, 4, 27, 29, rgba("dark_stone"))
    vline(d, 4, 20, 29, rgba("mid_stone"))
    vline(d, 27, 20, 29, rgba("mid_stone"))
    # Training dummy
    block(d, 15, 12, 4, 10, rgba("dark_wood"))
    block(d, 13, 10, 7, 5, rgba("wood"))
    # Scarecrow arms
    hline(d, 10, 22, 13, rgba("dark_wood"))
    # Flag on pole
    vline(d, 25, 6, 20, rgba("dark_wood"))
    for i in range(5):
        block(d, 25, 7+i, 5-i, 1, rgba("flag_red"))
    # Footprints in sand
    for (x,y) in [(9,25),(12,27),(16,24),(20,26),(23,25)]:
        d.point((x,y), (80,60,35,180))
    return img

# ════════════════════════════════════════════════════════════════════════════
# MONSTERS  (32×32)
# ════════════════════════════════════════════════════════════════════════════

def make_monster_imp(size=32):
    """꼬마 악마 (Imp Scrap) — 빨간 몸, 뿔, 날개"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Shadow
    d.ellipse([10,28,22,31], fill=(0,0,0,80))
    # Body
    d.ellipse([10,14,22,26], fill=rgba("imp_body"))
    d.ellipse([11,15,21,25], fill=rgba("imp_dark"))
    block(d, 13,18,6,8, rgba("imp_body"))
    # Belly highlight
    d.ellipse([13,17,19,23], fill=(220,100,80,120))
    # Head
    d.ellipse([10,8,22,18], fill=rgba("imp_body"))
    d.ellipse([11,9,21,17], fill=rgba("imp_dark"))
    # Horns
    d.line([(12,8),(10,4)], fill=rgba("imp_horn"), width=1)
    d.line([(20,8),(22,4)], fill=rgba("imp_horn"), width=1)
    d.point((10,4), rgba("gold"))
    d.point((22,4), rgba("gold"))
    # Eyes
    block(d, 12,11,3,2, rgba("imp_eye"))
    block(d, 18,11,3,2, rgba("imp_eye"))
    d.point((13,12), rgba("black"))
    d.point((19,12), rgba("black"))
    # Mouth
    d.line([(13,15),(19,15)], fill=rgba("imp_horn"))
    d.point((13,15), rgba("black"))
    d.point((14,16), rgba("black"))
    d.point((18,16), rgba("black"))
    d.point((19,15), rgba("black"))
    # Wings
    for i in range(5):
        d.point((9-i, 14+i), rgba("imp_wing"))
        d.point((23+i, 14+i), rgba("imp_wing"))
    # Tail
    d.line([(16,24),(13,28),(15,30)], fill=rgba("imp_horn"))
    # Arms
    block(d, 7,18,4,3, rgba("imp_body"))
    block(d, 21,18,4,3, rgba("imp_body"))
    # Claws
    for cx in [7,8,9]:
        d.point((cx,21), rgba("imp_horn"))
    for cx in [22,23,24]:
        d.point((cx,21), rgba("imp_horn"))
    # Feet
    block(d, 11,24,5,3, rgba("imp_dark"))
    block(d, 17,24,5,3, rgba("imp_dark"))
    return img

def make_monster_slime(size=32):
    """늪 슬라임 — 초록, 반투명 느낌"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    d.ellipse([8,30,24,32], fill=(0,0,0,70))
    # Body blob
    d.ellipse([6,16,26,30], fill=rgba("slime_body"))
    d.ellipse([8,14,24,28], fill=(70,200,110,255))
    block(d, 6,22,20,8, rgba("slime_body"))
    # Highlight
    d.ellipse([10,16,20,24], fill=rgba("slime_hi"))
    d.ellipse([11,17,17,21], fill=(180,255,200,180))
    # Eyes
    d.ellipse([11,20,15,24], fill=rgba("slime_eye"))
    d.ellipse([17,20,21,24], fill=rgba("slime_eye"))
    d.point((12,21), rgba("slime_hi"))
    d.point((18,21), rgba("slime_hi"))
    # Mouth
    d.arc([13,23,19,27], 0, 180, fill=rgba("slime_dark"))
    # Drip
    for i in range(4):
        d.point((16, 28+i), rgba("slime_body"))
    d.point((6,22), rgba("slime_hi"))
    d.point((24,20), rgba("slime_hi"))
    # Bubbles on surface
    for (bx,by) in [(9,15),(21,17),(14,13)]:
        d.ellipse([bx,by,bx+2,by+2], outline=rgba("slime_hi"))
    return img

def make_monster_bonepup(size=32):
    """뼈 늑대 (Bone Pup) — 해골 느낌"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    d.ellipse([6,30,22,32], fill=(0,0,0,70))
    # Body
    block(d, 8,16,16,12, rgba("bone_body"))
    block(d, 9,17,14,10, rgba("bone_dark"))
    # Spine bumps
    for i in range(3):
        d.point((12+i*3, 16), rgba("bone_hi"))
    # Rib lines
    for i in range(3):
        d.line([(10, 18+i*3),(14,17+i*3)], fill=rgba("bone_hi"))
        d.line([(18, 18+i*3),(22,17+i*3)], fill=rgba("bone_hi"))
    # Head
    d.ellipse([6,10,20,22], fill=rgba("bone_body"))
    d.ellipse([7,11,19,21], fill=rgba("bone_dark"))
    # Jaw
    block(d, 7,19,13,4, rgba("bone_dark"))
    # Teeth
    for tx in [9,12,15]:
        block(d, tx, 20, 2, 3, rgba("bone_body"))
    # Eye sockets
    d.ellipse([8,12,12,16], fill=rgba("black"))
    d.ellipse([14,12,18,16], fill=rgba("black"))
    d.point((10,14), rgba("bone_eye"))
    d.point((16,14), rgba("bone_eye"))
    # Ears (bone spikes)
    d.line([(8,10),(5,6)], fill=rgba("bone_body"))
    d.line([(18,10),(21,6)], fill=rgba("bone_body"))
    # Tail
    d.line([(24,18),(28,14),(30,18)], fill=rgba("bone_body"))
    # Legs
    block(d, 9,26,4,6, rgba("bone_dark"))
    block(d, 15,26,4,6, rgba("bone_dark"))
    block(d, 10,30,3,2, rgba("bone_body"))
    block(d, 16,30,3,2, rgba("bone_body"))
    # Paws
    for px in [9,10,11]:
        d.point((px,32), rgba("bone_hi"))
    for px in [15,16,17]:
        d.point((px,32), rgba("bone_hi"))
    return img

def make_monster_grimshadow(size=32):
    """어둠 그림자 — 연기 같은 형태"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Wispy base (transparency gradient)
    for i in range(5):
        a = 60 + i*20
        d.ellipse([8+i,22+i,24-i,32-i//2], fill=(40,20,65,a))
    # Cloak body
    d.ellipse([8,10,24,28], fill=rgba("shadow_body"))
    block(d, 6,18,20,10, rgba("shadow_body"))
    # Cloak ripple edges
    for i in range(3):
        d.point((6+i,28+i), rgba("shadow_smoke"))
        d.point((25-i,28+i), rgba("shadow_smoke"))
    # Smoke wisps
    for (wx,wy,a) in [(10,30,100),(16,31,80),(12,29,120),(20,30,90)]:
        d.ellipse([wx-1,wy-1,wx+2,wy+2], fill=(70,40,110,a))
    # Hood highlights
    d.ellipse([9,9,23,22], fill=rgba("shadow_dark"))
    d.ellipse([10,10,22,21], fill=rgba("shadow_smoke"))
    # Glowing eyes
    d.ellipse([11,13,15,17], fill=rgba("shadow_eye"))
    d.ellipse([17,13,21,17], fill=rgba("shadow_eye"))
    block(d, 12,14,2,2, rgba("white"))
    block(d, 18,14,2,2, rgba("white"))
    # Claws
    for i in range(3):
        d.point((7+i*2,25), rgba("shadow_hi"))
        d.point((19+i*2,25), rgba("shadow_hi"))
    return img

def make_monster_goblin(size=32):
    """고블린 기술자 — 초록, 고글, 도구"""
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    d.ellipse([10,30,22,32], fill=(0,0,0,70))
    # Legs
    block(d, 11,22,4,8, rgba("goblin_dark"))
    block(d, 17,22,4,8, rgba("goblin_dark"))
    block(d, 10,28,5,4, rgba("goblin_body"))
    block(d, 17,28,5,4, rgba("goblin_body"))
    # Body (overalls feel)
    block(d, 9,14,14,10, rgba("goblin_dark"))
    block(d, 10,15,12,9, rgba("goblin_body"))
    # Belt
    hline(d, 9, 22, 20, rgba("goblin_gear"))
    # Head
    d.ellipse([9,6,23,18], fill=rgba("goblin_body"))
    d.ellipse([10,7,22,17], fill=rgba("goblin_hi"))
    # Ears (big)
    d.ellipse([6,9,12,14], fill=rgba("goblin_body"))
    d.ellipse([20,9,26,14], fill=rgba("goblin_body"))
    d.point((7,11), rgba("goblin_hi"))
    d.point((24,11), rgba("goblin_hi"))
    # Goggles
    block(d, 10,9,5,4, rgba("goblin_metal"))
    block(d, 16,9,5,4, rgba("goblin_metal"))
    block(d, 15,10,2,2, rgba("goblin_dark"))
    d.ellipse([11,10,14,13], fill=rgba("gold_ore"))
    d.ellipse([17,10,20,13], fill=rgba("gold_ore"))
    d.point((12,11), rgba("gold"))
    d.point((18,11), rgba("gold"))
    # Nose
    d.point((15,13), rgba("goblin_dark"))
    d.point((16,13), rgba("goblin_dark"))
    # Mouth / grin
    d.line([(12,15),(20,15)], fill=rgba("goblin_dark"))
    for tx in [13,15,17,19]:
        d.point((tx,16), rgba("goblin_dark"))
    # Arms
    block(d, 5,14,5,4, rgba("goblin_body"))
    block(d, 22,14,5,4, rgba("goblin_body"))
    # Wrench in right hand
    block(d, 25,16,5,2, rgba("goblin_metal"))
    block(d, 26,14,2,4, rgba("goblin_metal"))
    block(d, 29,15,2,3, rgba("goblin_metal"))
    return img

# ════════════════════════════════════════════════════════════════════════════
# UI SPRITES
# ════════════════════════════════════════════════════════════════════════════

def make_ui_panel(w=16, h=16):
    """9-slice panel sprite"""
    img = Image.new("RGBA", (w, h), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Fill
    block(d, 2, 2, w-4, h-4, rgba("ui_bg"))
    # Borders
    hline(d, 2, w-3, 1, rgba("ui_border"))
    hline(d, 2, w-3, h-2, rgba("ui_border"))
    vline(d, 1, 2, h-3, rgba("ui_border"))
    vline(d, w-2, 2, h-3, rgba("ui_border"))
    # Corners
    d.point((1,1), rgba("ui_border"))
    d.point((w-2,1), rgba("ui_border"))
    d.point((1,h-2), rgba("ui_border"))
    d.point((w-2,h-2), rgba("ui_border"))
    # Outer edge
    hline(d, 1, w-2, 0, rgba("ui_border_hi"))
    hline(d, 1, w-2, h-1, (10,8,16,255))
    vline(d, 0, 1, h-2, rgba("ui_border_hi"))
    vline(d, w-1, 1, h-2, (10,8,16,255))
    return img

def make_icon_gold(size=16):
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    d.ellipse([2,2,13,13], fill=(180,140,30,255))
    d.ellipse([3,3,12,12], fill=rgba("gold"))
    d.ellipse([4,4,9,9], fill=(240,210,100,255))
    d.point((5,5), rgba("white"))
    return img

def make_icon_food(size=16):
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    d.ellipse([4,4,13,13], fill=rgba("mushroom_cap"))
    d.ellipse([5,5,12,12], fill=rgba("mushroom_glow"))
    block(d, 7,10,3,5, rgba("mushroom_stem"))
    d.point((7,6), rgba("white"))
    return img

def make_icon_mana(size=16):
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    # Crystal shape
    for i in range(6):
        w = max(1, 4-abs(i-3))
        block(d, 8-w, 2+i*2, w*2, 2, rgba("mana_glow"))
    block(d, 6,3,4,10, rgba("purple"))
    block(d, 7,4,2,8, rgba("mana_glow"))
    d.point((7,4), rgba("white"))
    return img

def make_icon_worker(size=16):
    img = Image.new("RGBA", (size, size), (0,0,0,0))
    d = ImageDraw.Draw(img)
    d.ellipse([5,2,11,8], fill=rgba("goblin_body"))
    block(d, 4,8,8,6, rgba("goblin_dark"))
    block(d, 2,9,3,4, rgba("goblin_body"))
    block(d, 11,9,3,4, rgba("goblin_body"))
    return img

# ════════════════════════════════════════════════════════════════════════════
# MAIN
# ════════════════════════════════════════════════════════════════════════════

print("=== RUMORS Phase 0 — Asset Generation ===\n")

print("[Tiles]")
save(make_tile_dark_grass(),    os.path.join(TILES_DIR, "Tile_Ground_DarkGrass.png"))
save(make_tile_dirt(),          os.path.join(TILES_DIR, "Tile_Ground_Dirt.png"))
save(make_tile_stone(),         os.path.join(TILES_DIR, "Tile_Ground_Stone.png"))
save(make_tile_swamp(),         os.path.join(TILES_DIR, "Tile_Ground_Swamp.png"))
save(make_tile_castle_floor(),  os.path.join(TILES_DIR, "Tile_Castle_Floor.png"))
save(make_tile_cobblestone(),   os.path.join(TILES_DIR, "Tile_Cobblestone.png"))
save(make_tile_highlight(),     os.path.join(TILES_DIR, "Tile_Highlight.png"))

print("\n[Buildings]")
save(make_building_castle(48),          os.path.join(BUILDINGS_DIR, "Building_Castle.png"))
save(make_building_mine(32),            os.path.join(BUILDINGS_DIR, "Building_Mine.png"))
save(make_building_farm(32),            os.path.join(BUILDINGS_DIR, "Building_Farm.png"))
save(make_building_dormitory(32),       os.path.join(BUILDINGS_DIR, "Building_Dormitory.png"))
save(make_building_warehouse(32),       os.path.join(BUILDINGS_DIR, "Building_Warehouse.png"))
save(make_building_construction(32),    os.path.join(BUILDINGS_DIR, "Building_Construction.png"))
save(make_building_mana_tower(32),      os.path.join(BUILDINGS_DIR, "Building_ManaTower.png"))
save(make_building_training(32),        os.path.join(BUILDINGS_DIR, "Building_Training.png"))

print("\n[Monsters]")
save(make_monster_imp(32),          os.path.join(MONSTERS_DIR, "Monster_ImpScrap.png"))
save(make_monster_slime(32),        os.path.join(MONSTERS_DIR, "Monster_MireSlime.png"))
save(make_monster_bonepup(32),      os.path.join(MONSTERS_DIR, "Monster_BonePup.png"))
save(make_monster_grimshadow(32),   os.path.join(MONSTERS_DIR, "Monster_GrimShadow.png"))
save(make_monster_goblin(32),       os.path.join(MONSTERS_DIR, "Monster_GoblinTinker.png"))

print("\n[UI Icons]")
save(make_ui_panel(16,16),  os.path.join(UI_DIR, "UI_Panel_9Slice.png"))
save(make_icon_gold(16),    os.path.join(UI_DIR, "Icon_Gold.png"))
save(make_icon_food(16),    os.path.join(UI_DIR, "Icon_Food.png"))
save(make_icon_mana(16),    os.path.join(UI_DIR, "Icon_Mana.png"))
save(make_icon_worker(16),  os.path.join(UI_DIR, "Icon_Worker.png"))

print("\n=== Done! All assets generated. ===")
