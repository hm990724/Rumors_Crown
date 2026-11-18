"""
tools/generate_hd_pixel_art.py
High-definition, professional 16-bit retro dark fantasy pixel art generator for RUMORS.
Generates 100% original, cleanroom-crafted pixel assets matching reference aesthetic.
"""

from PIL import Image, ImageDraw
import os

TILES_DIR = r"Assets\Resources\Tiles"
BUILDINGS_DIR = r"Assets\Resources\Buildings"
PROPS_DIR = r"Assets\Resources\Props"
MONSTERS_DIR = r"Assets\Resources\Monsters"
UI_DIR = r"Assets\Resources\UI"

ART_TILES_DIR = r"Assets\Art\Sprites\Tiles"
ART_BUILDINGS_DIR = r"Assets\Art\Sprites\Buildings"
ART_PROPS_DIR = r"Assets\Art\Sprites\Props"
ART_MONSTERS_DIR = r"Assets\Art\Sprites\Monsters"
ART_UI_DIR = r"Assets\Art\Sprites\UI"

for d in [TILES_DIR, BUILDINGS_DIR, PROPS_DIR, MONSTERS_DIR, UI_DIR,
          ART_TILES_DIR, ART_BUILDINGS_DIR, ART_PROPS_DIR, ART_MONSTERS_DIR, ART_UI_DIR]:
    os.makedirs(d, exist_ok=True)

# ── Color Palette Definitions ──────────────────────────────────────────────
# Natural Terrain (Olive Grass & Warm Earth - Reference 1 & 2)
C_GRASS_BRIGHT = (197, 201, 44, 255)   # #C5C92C
C_GRASS_LIT    = (163, 167, 29, 255)   # #A3A71D
C_GRASS_MID    = (136, 139, 33, 255)   # #888B21 (Dominant Olive Grass)
C_GRASS_SHADOW = (90, 93, 21, 255)     # #5A5D15
C_GRASS_DARK   = (59, 62, 14, 255)     # #3B3E0E

C_DIRT_LIT     = (191, 160, 117, 255)  # Warm tan dirt
C_DIRT_MID     = (150, 116, 73, 255)   # Earthy brown
C_DIRT_SHADOW  = (105, 78, 44, 255)    # Shadow dirt
C_DIRT_DEEP    = (67, 48, 25, 255)     # Deep furrow brown

# Cobblestone / Flagstone (Irregular stone plates - Reference 1)
C_FLAG_HILIGHT = (200, 196, 183, 255)  # #C8C4B7
C_FLAG_LIT     = (170, 165, 152, 255)  # #AAA598
C_FLAG_MID     = (130, 126, 116, 255)  # #827E74
C_FLAG_SHADOW  = (88, 85, 78, 255)     # #58554E
C_FLAG_CREVICE = (45, 43, 39, 255)     # #2D2B27 (Dark mortar gap)
C_MOSS_GREEN   = (123, 130, 36, 255)   # Crevice moss

# Gothic Obsidian Citadel (Reference 1, 4)
C_OBSID_HI     = (116, 112, 138, 255)  # Slate highlight
C_OBSID_LIT    = (75, 71, 94, 255)     # Lit stone
C_OBSID_MID    = (50, 46, 66, 255)     # Base obsidian
C_OBSID_DARK   = (32, 29, 43, 255)     # Deep shadow
C_OBSID_DEEP   = (18, 16, 25, 255)     # Outlines / voids

# Timber & Wood
C_WOOD_HI      = (196, 154, 108, 255)
C_WOOD_LIT     = (158, 116, 72, 255)
C_WOOD_MID     = (115, 80, 46, 255)
C_WOOD_SHADOW  = (77, 51, 27, 255)
C_WOOD_DARK    = (44, 28, 13, 255)

# Roof Slate & Tiles
C_ROOF_LIT     = (92, 53, 119, 255)
C_ROOF_MID     = (66, 40, 84, 255)
C_ROOF_DARK    = (45, 30, 58, 255)

# Glows & Accents
C_MANA_CYAN    = (80, 220, 240, 255)
C_MANA_PURPLE  = (185, 75, 255, 255)
C_MANA_GOLD    = (246, 199, 68, 255)
C_GOLD_DARK    = (160, 120, 20, 255)
C_CRIMSON_GLOW = (245, 55, 95, 255)
C_CRIMSON_DARK = (130, 20, 45, 255)
C_TORCH_YEL    = (255, 235, 90, 255)
C_TORCH_ORG    = (255, 136, 26, 255)
C_TORCH_RED    = (214, 40, 0, 255)

# Helper primitives
def pt(d, x, y, c):
    d.point((x, y), fill=c)

def hline(d, x0, x1, y, c):
    d.line([(x0, y), (x1, y)], fill=c)

def vline(d, x, y0, y1, c):
    d.line([(x, y0), (x, y1)], fill=c)

def rect(d, x, y, w, h, fill_c, outline=None):
    d.rectangle([x, y, x + w - 1, y + h - 1], fill=fill_c, outline=outline)

# ══════════════════════════════════════════════════════════════════════════════
# 1. TILES (16x16 Seamless & Organic)
# ══════════════════════════════════════════════════════════════════════════════

def make_tile_dark_grass():
    """Natural, rich olive-green lawn with tufts and tiny wildflowers (Ref 1, 2)."""
    img = Image.new("RGBA", (16, 16), C_GRASS_MID)
    d = ImageDraw.Draw(img)
    # Organic grass clustering
    for y in range(16):
        for x in range(16):
            hash_val = ((x * 19) ^ (y * 31) ^ (x * y * 7)) % 100
            if hash_val < 25:
                pt(d, x, y, C_GRASS_LIT)
            elif hash_val < 38:
                pt(d, x, y, C_GRASS_SHADOW)
            elif hash_val > 92:
                pt(d, x, y, C_GRASS_BRIGHT)
            elif hash_val > 86:
                pt(d, x, y, C_GRASS_DARK)

    # Distinctive grass tufts (curved blade highlights)
    tufts = [(3, 4), (11, 2), (7, 10), (13, 12), (2, 13)]
    for tx, ty in tufts:
        pt(d, tx, ty, C_GRASS_BRIGHT)
        pt(d, tx, ty + 1, C_GRASS_LIT)
        pt(d, tx + 1, ty + 1, C_GRASS_SHADOW)

    # A couple of subtle white/yellow wildflowers
    pt(d, 5, 3, (255, 250, 220, 255))
    pt(d, 12, 9, (255, 230, 100, 255))
    return img

def make_tile_dirt():
    """Warm, textured trodden earth path (Ref 2, 3)."""
    img = Image.new("RGBA", (16, 16), C_DIRT_MID)
    d = ImageDraw.Draw(img)
    for y in range(16):
        for x in range(16):
            hash_val = ((x * 23) ^ (y * 17) ^ (x * y * 13)) % 100
            if hash_val < 22:
                pt(d, x, y, C_DIRT_LIT)
            elif hash_val < 40:
                pt(d, x, y, C_DIRT_SHADOW)
            elif hash_val > 94:
                pt(d, x, y, C_DIRT_DEEP)

    # Pebbles and wheel-rut shadows
    pebbles = [(4, 5), (10, 3), (8, 12), (13, 8)]
    for px, py in pebbles:
        pt(d, px, py, C_DIRT_LIT)
        pt(d, px + 1, py, C_DIRT_LIT)
        pt(d, px, py + 1, C_DIRT_DEEP)
        pt(d, px + 1, py + 1, C_DIRT_DEEP)
    return img

def make_tile_cobblestone():
    """
    Irregular, natural flagstone paving with rounded stones, 
    crevice shadows, and moss/grass creeping in the gaps (Ref 1).
    """
    # Background is dark mortar crevice
    img = Image.new("RGBA", (16, 16), C_FLAG_CREVICE)
    d = ImageDraw.Draw(img)

    # Draw 5 distinct rounded stones inside 16x16
    stones = [
        # (x, y, w, h, has_moss)
        (1, 1, 6, 6, True),    # Top-left stone
        (8, 1, 7, 5, False),   # Top-right stone
        (6, 7, 5, 4, True),    # Center stone
        (1, 8, 4, 7, False),   # Bottom-left stone
        (10, 7, 5, 8, True),   # Bottom-right stone
        (5, 12, 4, 3, False),  # Bottom-mid stone
    ]

    for sx, sy, sw, sh, moss in stones:
        # Base stone
        rect(d, sx, sy, sw, sh, C_FLAG_MID)
        # Top-left highlight (3/4 lighting)
        hline(d, sx + 1, sx + sw - 2, sy, C_FLAG_LIT)
        vline(d, sx, sy + 1, sy + sh - 2, C_FLAG_LIT)
        pt(d, sx + 1, sy + 1, C_FLAG_HILIGHT)
        # Bottom-right shadow
        hline(d, sx + 1, sx + sw - 1, sy + sh - 1, C_FLAG_SHADOW)
        vline(d, sx + sw - 1, sy + 1, sy + sh - 1, C_FLAG_SHADOW)
        # Rounded corners (erase corners to reveal mortar)
        pt(d, sx, sy, C_FLAG_CREVICE)
        pt(d, sx + sw - 1, sy, C_FLAG_CREVICE)
        pt(d, sx, sy + sh - 1, C_FLAG_CREVICE)
        pt(d, sx + sw - 1, sy + sh - 1, C_FLAG_CREVICE)

        if moss:
            pt(d, sx, sy + sh - 1, C_MOSS_GREEN)
            pt(d, sx + 1, sy + sh, C_MOSS_GREEN)

    # Additional moss spots in mortar
    pt(d, 0, 7, C_MOSS_GREEN)
    pt(d, 7, 0, C_MOSS_GREEN)
    pt(d, 15, 6, C_MOSS_GREEN)
    pt(d, 7, 15, C_MOSS_GREEN)
    return img

def make_tile_castle_floor():
    """Gothic obsidian checker tile with glowing arcane purple rune lines."""
    img = Image.new("RGBA", (16, 16), C_OBSID_DARK)
    d = ImageDraw.Draw(img)

    # 4 stone tiles with beveled edges
    for (ox, oy) in [(0, 0), (8, 0), (0, 8), (8, 8)]:
        rect(d, ox + 1, oy + 1, 6, 6, C_OBSID_MID)
        hline(d, ox + 1, ox + 6, oy + 1, C_OBSID_LIT)
        vline(d, ox + 1, oy + 1, oy + 6, C_OBSID_LIT)
        hline(d, ox + 1, ox + 6, oy + 6, C_OBSID_DEEP)
        vline(d, ox + 6, oy + 1, oy + 6, C_OBSID_DEEP)

    # Center rune cross / diamond in purple
    pt(d, 7, 7, C_MANA_PURPLE)
    pt(d, 8, 7, C_MANA_PURPLE)
    pt(d, 7, 8, C_MANA_PURPLE)
    pt(d, 8, 8, C_MANA_PURPLE)
    pt(d, 7, 6, (140, 50, 190, 255))
    pt(d, 8, 6, (140, 50, 190, 255))
    pt(d, 7, 9, (140, 50, 190, 255))
    pt(d, 8, 9, (140, 50, 190, 255))
    pt(d, 6, 7, (140, 50, 190, 255))
    pt(d, 6, 8, (140, 50, 190, 255))
    pt(d, 9, 7, (140, 50, 190, 255))
    pt(d, 9, 8, (140, 50, 190, 255))
    return img

def make_tile_stone():
    """Rugged quarried rock / stone bed."""
    img = Image.new("RGBA", (16, 16), (95, 92, 95, 255))
    d = ImageDraw.Draw(img)
    for y in range(16):
        for x in range(16):
            h = ((x * 37) ^ (y * 29)) % 100
            if h < 20:
                pt(d, x, y, (125, 122, 125, 255))
            elif h < 35:
                pt(d, x, y, (70, 68, 72, 255))
            elif h > 92:
                pt(d, x, y, (45, 43, 46, 255))
    # Cracks
    d.line([(3, 3), (7, 6), (11, 6), (14, 11)], fill=(50, 48, 52, 255))
    d.line([(4, 4), (8, 7)], fill=(135, 132, 135, 255))
    return img

def make_tile_swamp():
    """Murky green swamp with toxic slime sheen."""
    img = Image.new("RGBA", (16, 16), (48, 55, 32, 255))
    d = ImageDraw.Draw(img)
    for y in range(16):
        for x in range(16):
            h = ((x * 17) ^ (y * 23)) % 100
            if h < 20:
                pt(d, x, y, (32, 40, 22, 255))
            elif h > 85:
                pt(d, x, y, (65, 82, 40, 255))
    # Bubbles
    for bx, by in [(4, 4), (11, 10), (12, 3)]:
        rect(d, bx, by, 3, 3, (85, 120, 45, 255))
        pt(d, bx, by, (140, 190, 75, 255))
        pt(d, bx + 2, by + 2, (30, 45, 18, 255))
    return img

def make_tile_highlight():
    """Subtle glowing corner brackets for cursor hover."""
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    c_hi = (255, 225, 80, 220)
    c_sub = (255, 180, 40, 160)
    # 4 corners
    for (cx, cy, dx, dy) in [(0, 0, 1, 1), (15, 0, -1, 1), (0, 15, 1, -1), (15, 15, -1, -1)]:
        pt(d, cx, cy, c_hi)
        pt(d, cx + dx, cy, c_hi)
        pt(d, cx + dx * 2, cy, c_sub)
        pt(d, cx, cy + dy, c_hi)
        pt(d, cx, cy + dy * 2, c_sub)
    return img


# ══════════════════════════════════════════════════════════════════════════════
# 2. BUILDINGS (Detailed Isometric Architectural Sprites)
# ══════════════════════════════════════════════════════════════════════════════

def make_building_castle(level=1):
    """
    Grand Gothic Demon King Citadel (48x48):
    - 3/4 Perspective with massive stone coursing
    - Central Archway Demon Gate leading into crimson/abyssal void
    - Left & Right Watchtowers with battlements, arrow slits, and flaming braziers
    - Central Keep with gothic spires, purple mana core, and demon banners
    """
    img = Image.new("RGBA", (48, 48), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # 0. Ground contact shadow
    rect(d, 3, 38, 42, 8, (20, 15, 30, 160))

    # 1. Base Wall (Tier 1) - x: 6..41, y: 22..43
    rect(d, 6, 22, 36, 22, C_OBSID_DARK)
    rect(d, 8, 24, 32, 19, C_OBSID_MID)

    # Stone coursing masonry lines
    for cy in [28, 33, 38]:
        hline(d, 8, 39, cy, C_OBSID_DEEP)
        for cx in range(11 + (cy % 4) * 2, 38, 6):
            pt(d, cx, cy - 1, C_OBSID_LIT)
            pt(d, cx, cy, C_OBSID_DARK)

    # 2. Grand Central Gate (Archway Demon Gate)
    # Outer stone arch
    rect(d, 18, 27, 12, 17, C_OBSID_DEEP)
    rect(d, 19, 26, 10, 18, C_OBSID_DEEP)
    rect(d, 20, 25, 8, 19, C_OBSID_LIT)
    rect(d, 21, 26, 6, 18, (15, 10, 24, 255))
    # Glowing abyssal portal doorway
    rect(d, 22, 28, 4, 16, (40, 10, 30, 255))
    rect(d, 22, 31, 4, 13, (80, 15, 45, 255))
    pt(d, 23, 34, C_CRIMSON_GLOW)
    pt(d, 24, 34, C_CRIMSON_GLOW)
    pt(d, 23, 38, (180, 30, 60, 255))
    pt(d, 24, 38, (180, 30, 60, 255))

    # Iron portcullis bars
    vline(d, 22, 29, 39, (25, 22, 35, 255))
    vline(d, 25, 29, 39, (25, 22, 35, 255))
    hline(d, 21, 26, 32, (25, 22, 35, 255))

    # Demon skull arch keystone
    rect(d, 22, 23, 4, 3, C_FLAG_LIT)
    pt(d, 22, 24, (10, 10, 15, 255))
    pt(d, 25, 24, (10, 10, 15, 255))

    # 3. Flanking Watchtowers (Left & Right)
    for (tx, is_left) in [(2, True), (34, False)]:
        # Tower body
        rect(d, tx, 14, 12, 29, C_OBSID_DARK)
        rect(d, tx + 1, 16, 10, 26, C_OBSID_MID)
        # Left lit highlight, right shadow
        vline(d, tx + 1, 16, 40, C_OBSID_HI)
        vline(d, tx + 10, 16, 40, C_OBSID_DEEP)

        # Arrow slit window
        rect(d, tx + 5, 25, 2, 6, C_OBSID_DEEP)
        pt(d, tx + 5, 27, C_CRIMSON_GLOW)

        # Tower battlements (crenels)
        rect(d, tx, 11, 4, 4, C_OBSID_LIT)
        rect(d, tx + 8, 11, 4, 4, C_OBSID_LIT)
        rect(d, tx + 4, 13, 4, 2, C_OBSID_DARK)

        # Flaming brazier atop tower
        rect(d, tx + 4, 10, 4, 3, (30, 25, 40, 255))
        # Fire particles
        pt(d, tx + 5, 9, C_TORCH_RED)
        pt(d, tx + 6, 9, C_TORCH_ORG)
        pt(d, tx + 5, 8, C_TORCH_ORG)
        pt(d, tx + 6, 8, C_TORCH_YEL)
        pt(d, tx + 5, 7, C_TORCH_YEL)

    # 4. Central Keep Spire (Tier 2) - x: 15..32, y: 5..24
    rect(d, 15, 8, 18, 15, C_OBSID_DARK)
    rect(d, 17, 10, 14, 12, C_OBSID_MID)
    # Rose stained glass window (Purple/Crimson)
    rect(d, 22, 12, 4, 6, C_OBSID_DEEP)
    pt(d, 23, 14, C_MANA_PURPLE)
    pt(d, 24, 14, C_MANA_PURPLE)
    pt(d, 23, 15, C_CRIMSON_GLOW)
    pt(d, 24, 15, C_CRIMSON_GLOW)

    # Central High Gothic Roof / Steeple
    # Roof triangle
    for r in range(9):
        rx0 = 24 - r
        rx1 = 23 + r
        ry = 8 - r
        hline(d, rx0, rx1, ry, C_ROOF_MID)
        pt(d, rx0, ry, C_ROOF_LIT)
        pt(d, rx1, ry, C_ROOF_DARK)

    # Floating Spire Demon Crystal (Top)
    pt(d, 23, 0, C_MANA_PURPLE)
    pt(d, 24, 0, C_MANA_PURPLE)
    rect(d, 22, 1, 4, 3, C_MANA_PURPLE)
    pt(d, 23, 2, (255, 200, 255, 255)) # core shine

    # Red Banners hanging on walls
    for bx in [15, 30]:
        rect(d, bx, 22, 3, 10, C_CRIMSON_DARK)
        rect(d, bx, 23, 3, 8, (200, 30, 60, 255))
        # Gold trim on banner
        hline(d, bx, bx + 2, 23, C_GOLD_DARK)
        pt(d, bx + 1, 27, C_MANA_GOLD)
        # Pointed banner bottom
        pt(d, bx, 32, (0, 0, 0, 0))
        pt(d, bx + 2, 32, (0, 0, 0, 0))

    if level >= 2:
        # Upgraded additions: Golden dragon gargoyles & glowing mana aura
        for gx in [1, 43]:
            rect(d, gx, 10, 4, 4, C_MANA_GOLD)
            pt(d, gx + 1, 9, (255, 240, 150, 255))
        # Blue arcane rune circle on keep
        hline(d, 19, 28, 19, C_MANA_CYAN)
        pt(d, 24, 18, C_MANA_CYAN)

    return img

def make_building_mine(level=1):
    """
    Dark Mountain Mine (32x32):
    - Rugged crag hillside entrance (Ref 3)
    - Sturdy heavy timber frame supports & crossbeam
    - Steel rail tracks leading outward
    - Wooden minecart loaded with glowing violet & gold ores
    - Pickaxe & hanging lantern prop
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # 0. Ground shadow
    rect(d, 2, 26, 28, 6, (20, 18, 25, 140))

    # 1. Rock crag hillside background (x: 2..29, y: 4..27)
    rect(d, 2, 10, 28, 17, (75, 72, 76, 255))
    rect(d, 4, 6, 24, 21, (90, 87, 92, 255))
    rect(d, 7, 3, 18, 8, (108, 104, 110, 255))
    # Rock crag facets & cracks
    d.line([(5, 6), (9, 12), (7, 18)], fill=(55, 52, 56, 255))
    d.line([(24, 7), (21, 14), (25, 20)], fill=(55, 52, 56, 255))
    hline(d, 8, 23, 4, (135, 131, 138, 255)) # top sunlight

    # Natural moss on rocks
    pt(d, 5, 8, C_MOSS_GREEN)
    pt(d, 6, 8, C_MOSS_GREEN)
    pt(d, 25, 9, C_MOSS_GREEN)

    # 2. Mine tunnel opening (Dark Void)
    rect(d, 8, 11, 16, 17, (20, 18, 22, 255))
    rect(d, 9, 12, 14, 16, (10, 9, 12, 255))

    # 3. Heavy Timber Support Frame
    # Left & Right wooden pillars
    rect(d, 7, 10, 3, 18, C_WOOD_SHADOW)
    rect(d, 8, 10, 2, 18, C_WOOD_LIT)
    rect(d, 22, 10, 3, 18, C_WOOD_DARK)
    rect(d, 22, 10, 2, 18, C_WOOD_MID)
    # Top Crossbeam
    rect(d, 6, 9, 20, 3, C_WOOD_DARK)
    rect(d, 7, 9, 18, 2, C_WOOD_LIT)
    hline(d, 7, 24, 9, C_WOOD_HI) # top wood highlight
    # Timber diagonal brace gussets
    pt(d, 10, 12, C_WOOD_MID)
    pt(d, 21, 12, C_WOOD_MID)

    # 4. Steel Rail Tracks leading forward
    # Wood sleepers
    for sy in [22, 25, 28]:
        hline(d, 10, 21, sy, C_WOOD_DARK)
    # Steel rails (2 lines)
    vline(d, 12, 19, 30, (140, 145, 155, 255))
    vline(d, 19, 19, 30, (140, 145, 155, 255))
    # Rail specular highlight
    vline(d, 12, 21, 29, (190, 195, 210, 255))
    vline(d, 19, 21, 29, (190, 195, 210, 255))

    # 5. Wooden Minecart loaded with ores
    # Cart body
    rect(d, 11, 20, 10, 7, C_WOOD_DARK)
    rect(d, 12, 21, 8, 5, C_WOOD_MID)
    # Iron strapping on cart
    rect(d, 11, 20, 10, 1, (70, 75, 85, 255))
    rect(d, 11, 26, 10, 1, (70, 75, 85, 255))
    # Metal wheels
    rect(d, 11, 27, 2, 2, (50, 52, 60, 255))
    rect(d, 19, 27, 2, 2, (50, 52, 60, 255))

    # Glowing Ore Heap inside cart
    # Gold nuggets
    pt(d, 13, 20, C_MANA_GOLD)
    pt(d, 14, 19, (255, 235, 100, 255))
    pt(d, 15, 20, C_GOLD_DARK)
    # Purple Mana Gems
    pt(d, 16, 19, C_MANA_PURPLE)
    pt(d, 17, 20, (230, 120, 255, 255))
    pt(d, 18, 19, (140, 40, 190, 255))

    # 6. Hanging Lantern on post
    pt(d, 6, 12, (60, 50, 40, 255)) # bracket
    rect(d, 5, 13, 3, 4, (40, 35, 30, 255))
    pt(d, 6, 14, C_TORCH_YEL)
    pt(d, 6, 15, C_TORCH_ORG)

    if level >= 2:
        # Crane pulley overhead & massive crystal cluster
        rect(d, 13, 1, 6, 3, C_WOOD_LIT)
        vline(d, 16, 4, 9, (180, 180, 180, 255)) # cable
        # Giant purple crystal growing out of crag
        d.polygon([(26, 6), (28, 2), (30, 7)], fill=C_MANA_PURPLE)
        pt(d, 28, 4, (255, 200, 255, 255))

    return img

def make_building_farm(level=1):
    """
    Bioluminescent Mushroom Farm (32x32):
    - Enclosed rich fertile soil mounds & furrows (Ref 2)
    - Wooden split-rail fence perimeter
    - Vibrant cluster of cyan glowing mushrooms, poisonous violet shrooms & golden spored caps
    - Water barrel, watering bucket & canvas shade awning
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # 0. Shadow
    rect(d, 2, 24, 28, 7, (20, 25, 15, 140))

    # 1. Rich compost soil bed (Dark nutrient peat)
    rect(d, 3, 14, 26, 16, (48, 34, 18, 255))
    rect(d, 4, 15, 24, 14, (64, 46, 24, 255))
    # Soil furrows / planting rows
    for fy in [18, 22, 26]:
        hline(d, 5, 26, fy, (38, 26, 13, 255))
        hline(d, 5, 26, fy + 1, (78, 56, 30, 255))

    # 2. Wooden Paling Fence (Back & Sides)
    # Back fence posts
    for fx in range(3, 29, 4):
        rect(d, fx, 9, 2, 6, C_WOOD_LIT)
        pt(d, fx, 9, C_WOOD_HI)
    hline(d, 3, 28, 11, C_WOOD_DARK)
    hline(d, 3, 28, 13, C_WOOD_MID)

    # 3. Canvas Shade Awning (North-West corner to shield delicate spores)
    # Awning cloth stripes (Dark purple & beige)
    for c in range(11):
        color = (130, 45, 140, 255) if (c // 3) % 2 == 0 else (210, 195, 170, 255)
        hline(d, 4, 15, 4 + c // 2, color)
    rect(d, 4, 9, 12, 2, C_WOOD_DARK)
    # Support poles
    vline(d, 4, 9, 16, C_WOOD_DARK)
    vline(d, 15, 9, 16, C_WOOD_DARK)

    # 4. Bioluminescent Mushrooms (Ref 2)
    # (A) Giant Cyan Bio-Luminescent Shroom (Center)
    # Stem
    rect(d, 13, 19, 3, 6, (170, 230, 240, 255))
    # Cap (Glowing Cyan Dome)
    d.pieslice([10, 13, 18, 21], 180, 360, fill=C_MANA_CYAN)
    hline(d, 11, 17, 17, (40, 160, 180, 255)) # cap rim
    # Cap glow dots
    pt(d, 12, 15, (230, 255, 255, 255))
    pt(d, 16, 15, (230, 255, 255, 255))
    pt(d, 14, 14, (255, 255, 255, 255))

    # (B) Cluster of Poison Purple Toadstools (Left)
    # Stems
    rect(d, 6, 22, 2, 4, (210, 190, 220, 255))
    rect(d, 9, 21, 2, 5, (210, 190, 220, 255))
    # Caps
    d.pieslice([4, 18, 9, 24], 180, 360, fill=(180, 40, 190, 255))
    d.pieslice([8, 16, 13, 23], 180, 360, fill=(210, 60, 220, 255))
    pt(d, 6, 19, (255, 220, 255, 255))
    pt(d, 10, 17, (255, 220, 255, 255))

    # (C) Golden Spore Shrooms (Right)
    # Stems
    rect(d, 20, 23, 2, 4, (240, 230, 180, 255))
    rect(d, 24, 21, 2, 6, (240, 230, 180, 255))
    # Golden caps
    d.pieslice([18, 19, 23, 25], 180, 360, fill=C_MANA_GOLD)
    d.pieslice([23, 17, 27, 23], 180, 360, fill=(255, 220, 70, 255))
    pt(d, 20, 20, (255, 255, 180, 255))
    pt(d, 25, 18, (255, 255, 180, 255))

    # 5. Farm props: Oak Rainbarrel & Watering Bucket
    # Barrel
    rect(d, 23, 10, 5, 6, C_WOOD_SHADOW)
    rect(d, 24, 10, 3, 6, C_WOOD_LIT)
    hline(d, 23, 27, 11, (80, 85, 95, 255))
    hline(d, 23, 27, 14, (80, 85, 95, 255))
    # Blue water surface in barrel
    pt(d, 24, 10, (60, 140, 220, 255))
    pt(d, 25, 10, (60, 140, 220, 255))

    if level >= 2:
        # Glass greenhouse dome frame over entire farm
        d.arc([2, 4, 29, 30], 180, 360, fill=(160, 220, 240, 200))
        hline(d, 2, 29, 5, (180, 230, 250, 160))

    return img

def make_building_dormitory(level=1):
    """
    Monster Cottage & Dormitory (32x32):
    - 2-Story warm timber lodge with stone chimney and rising puff of smoke (Ref 2)
    - Dark slate shingles roof
    - Warm candlelight glowing from windows
    - Wooden entrance bench and welcoming lantern
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # 0. Shadow
    rect(d, 3, 25, 26, 6, (15, 15, 20, 140))

    # 1. First & Second Floor Cabin Walls
    rect(d, 5, 13, 22, 14, C_WOOD_SHADOW)
    rect(d, 6, 14, 20, 12, C_WOOD_LIT)
    # Horizontal log siding lines
    for ly in [16, 19, 22]:
        hline(d, 6, 25, ly, C_WOOD_DARK)

    # 2. Gable Roof with Slate Shingles
    for r in range(8):
        rx0 = 16 - r * 2
        rx1 = 15 + r * 2
        ry = 13 - r
        hline(d, max(2, rx0), min(29, rx1), ry, C_ROOF_MID)
        pt(d, max(2, rx0), ry, C_ROOF_LIT)
        pt(d, min(29, rx1), ry, C_ROOF_DARK)
    hline(d, 15, 16, 5, C_ROOF_LIT) # Ridge cap

    # 3. Stone Chimney on left roof slope
    rect(d, 6, 3, 4, 8, C_FLAG_MID)
    rect(d, 5, 2, 6, 2, C_FLAG_LIT)
    # Smoke puffs
    pt(d, 7, 0, (230, 230, 240, 200))
    pt(d, 8, 1, (210, 210, 220, 160))

    # 4. Windows with glowing candlelight
    # 2nd floor dormer window
    rect(d, 14, 9, 4, 4, C_WOOD_DARK)
    rect(d, 15, 10, 2, 3, C_TORCH_YEL)
    # 1st floor window
    rect(d, 18, 16, 4, 5, C_WOOD_DARK)
    rect(d, 19, 17, 2, 3, C_TORCH_YEL)
    pt(d, 19, 17, (255, 255, 200, 255))

    # 5. Front Door & Porch
    rect(d, 9, 18, 5, 8, C_WOOD_DARK)
    rect(d, 10, 19, 3, 6, C_WOOD_MID)
    pt(d, 12, 22, C_GOLD_DARK) # brass doorknob

    # Wooden bench beside door
    rect(d, 17, 23, 6, 2, C_WOOD_LIT)
    pt(d, 18, 25, C_WOOD_DARK)
    pt(d, 22, 25, C_WOOD_DARK)

    if level >= 2:
        # 2nd floor balcony terrace & flower box
        rect(d, 13, 13, 8, 2, C_WOOD_HI)
        pt(d, 14, 12, (255, 100, 130, 255))
        pt(d, 16, 12, (255, 230, 80, 255))
        pt(d, 18, 12, (100, 220, 255, 255))

    return img

def make_building_warehouse(level=1):
    """Logistics Depot & Warehouse (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 2, 24, 28, 7, (20, 18, 22, 140))
    # Wall
    rect(d, 4, 11, 24, 15, C_WOOD_SHADOW)
    rect(d, 5, 12, 22, 13, C_WOOD_LIT)
    # Large double cargo doors
    rect(d, 10, 15, 12, 10, C_WOOD_DARK)
    # Diagonal cross bracing on cargo doors
    d.line([(10, 15), (15, 24)], fill=C_WOOD_MID)
    d.line([(15, 15), (10, 24)], fill=C_WOOD_MID)
    d.line([(16, 15), (21, 24)], fill=C_WOOD_MID)
    d.line([(21, 15), (16, 24)], fill=C_WOOD_MID)

    # Low Pitch Roof with Tarpaulin Cover
    rect(d, 2, 8, 28, 4, (55, 75, 95, 255))
    hline(d, 2, 29, 8, (85, 115, 145, 255))

    # Crates & Barrels stacked outside
    # Crate 1
    rect(d, 22, 18, 7, 7, C_WOOD_LIT, outline=C_WOOD_DARK)
    d.line([(22, 18), (28, 24)], fill=C_WOOD_DARK)
    # Crate 2
    rect(d, 23, 12, 5, 6, C_WOOD_MID, outline=C_WOOD_DARK)
    # Barrel
    rect(d, 5, 19, 4, 6, C_WOOD_SHADOW)
    hline(d, 5, 8, 20, (80, 85, 95, 255))
    hline(d, 5, 8, 23, (80, 85, 95, 255))

    if level >= 2:
        # Crane arm on roof
        d.line([(24, 8), (28, 1)], fill=C_WOOD_HI)
        vline(d, 28, 2, 9, (180, 180, 180, 255))

    return img

def make_building_mana_tower():
    """Mana Refinery Obelisk with floating crystal (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 6, 26, 20, 5, (20, 15, 30, 140))
    # Stone Base Tier
    rect(d, 9, 22, 14, 6, C_OBSID_DARK)
    rect(d, 10, 23, 12, 4, C_OBSID_MID)
    # Obelisk Pillar
    rect(d, 12, 10, 8, 13, C_OBSID_DARK)
    rect(d, 13, 10, 6, 12, C_OBSID_MID)
    # Arcane Cyan Inscription lines
    vline(d, 15, 12, 20, C_MANA_CYAN)
    vline(d, 16, 12, 20, C_MANA_CYAN)

    # 4 Corner Focus Pylons
    for px in [7, 23]:
        rect(d, px, 16, 2, 8, C_OBSID_MID)
        pt(d, px, 15, C_MANA_PURPLE)
        pt(d, px + 1, 15, C_MANA_PURPLE)

    # Floating Mana Core Diamond
    d.polygon([(16, 1), (21, 6), (16, 10), (11, 6)], fill=C_MANA_PURPLE)
    d.polygon([(16, 3), (19, 6), (16, 8), (13, 6)], fill=(230, 170, 255, 255)) # highlight core
    return img

def make_building_training():
    """Monster Training Ground with archery target & combat dummy (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 3, 24, 26, 6, (20, 20, 25, 140))
    # Sand pit ground
    rect(d, 4, 16, 24, 12, (180, 150, 100, 255))
    rect(d, 5, 17, 22, 10, (195, 165, 115, 255))

    # Archery Target (Left)
    rect(d, 7, 18, 2, 7, C_WOOD_DARK) # stand legs
    rect(d, 11, 18, 2, 7, C_WOOD_DARK)
    d.ellipse([6, 11, 14, 19], fill=(220, 215, 200, 255), outline=C_WOOD_DARK)
    d.ellipse([8, 13, 12, 17], fill=C_CRIMSON_DARK)
    pt(d, 10, 15, C_TORCH_YEL) # bullseye!

    # Straw Combat Dummy (Right)
    # Wooden cross pole
    vline(d, 22, 10, 24, C_WOOD_DARK)
    hline(d, 18, 26, 14, C_WOOD_DARK)
    # Straw body
    rect(d, 20, 12, 5, 9, (215, 190, 110, 255))
    # Helm on dummy
    rect(d, 21, 9, 3, 3, (120, 125, 135, 255))

    # Weapon rack (Center back)
    rect(d, 14, 9, 5, 6, C_WOOD_DARK)
    vline(d, 15, 8, 14, (160, 165, 175, 255)) # spear tip
    pt(d, 15, 7, (230, 235, 245, 255))
    return img

def make_building_construction():
    """Under Construction Site (Scaffolding & warning flags) (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 4, 25, 24, 5, (20, 18, 22, 120))
    # Foundation stones laid
    for bx in range(6, 26, 5):
        rect(d, bx, 23, 4, 3, C_FLAG_MID)

    # Scaffolding poles
    for px in [6, 14, 22]:
        vline(d, px, 8, 25, C_WOOD_LIT)
    # Crossboards
    hline(d, 5, 24, 15, C_WOOD_DARK)
    hline(d, 5, 24, 9, C_WOOD_DARK)
    # Diagonal braces
    d.line([(6, 15), (14, 9)], fill=C_WOOD_MID)
    d.line([(14, 15), (22, 9)], fill=C_WOOD_MID)

    # Red warning flag
    rect(d, 22, 5, 5, 4, C_CRIMSON_GLOW)
    pt(d, 22, 4, C_GOLD_DARK)
    return img


# ══════════════════════════════════════════════════════════════════════════════
# 3. PROPS (Trees, Bushes, Barrels, Torches)
# ══════════════════════════════════════════════════════════════════════════════

def make_prop_tree():
    """
    Majestic Forest Oak Tree (32x48):
    - 3-Tier lush volumetric foliage canopy (Ref 1, 2)
    - Olive-green gradient with top-left specular highlights
    - Gnarled trunk with deep wood grain, mossy bark & rooting base
    """
    img = Image.new("RGBA", (32, 48), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # 0. Ground shadow
    d.ellipse([4, 40, 27, 47], fill=(20, 25, 15, 140))

    # 1. Gnarled Wooden Trunk & Roots
    # Main trunk
    rect(d, 13, 22, 6, 21, C_WOOD_SHADOW)
    rect(d, 14, 22, 4, 21, C_WOOD_MID)
    vline(d, 14, 22, 42, C_WOOD_LIT) # sunlit edge
    vline(d, 17, 24, 42, C_WOOD_DARK) # bark crevice
    # Spreading roots
    d.line([(13, 41), (9, 44)], fill=C_WOOD_MID)
    d.line([(18, 41), (22, 44)], fill=C_WOOD_SHADOW)
    # Moss on trunk
    pt(d, 13, 38, C_MOSS_GREEN)
    pt(d, 14, 39, C_MOSS_GREEN)

    # 2. Volumetric Foliage Canopy (3 Overlapping Cloud Clusters)
    # (A) Lower Foliage Ring (y: 18..34)
    for (cx, cy, r) in [(10, 26, 7), (21, 26, 7), (16, 24, 8)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_SHADOW)
    for (cx, cy, r) in [(9, 25, 6), (20, 25, 6), (15, 23, 7)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_MID)
    for (cx, cy, r) in [(8, 23, 5), (14, 21, 6)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_LIT)

    # (B) Mid Foliage Ring (y: 10..24)
    for (cx, cy, r) in [(11, 16, 8), (20, 16, 8), (16, 14, 9)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_SHADOW)
    for (cx, cy, r) in [(10, 15, 7), (19, 15, 7), (15, 13, 8)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_MID)
    for (cx, cy, r) in [(9, 13, 6), (15, 11, 6)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_LIT)
    for (cx, cy, r) in [(8, 11, 4), (14, 9, 4)]:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=C_GRASS_BRIGHT)

    # (C) Top Crown (y: 3..14)
    d.ellipse([9, 4, 22, 16], fill=C_GRASS_SHADOW)
    d.ellipse([9, 3, 21, 14], fill=C_GRASS_MID)
    d.ellipse([9, 2, 19, 12], fill=C_GRASS_LIT)
    d.ellipse([10, 2, 16, 8], fill=C_GRASS_BRIGHT)

    # Leaf cluster highlights (Pixel dotting for texture)
    for lx, ly in [(11, 4), (14, 5), (10, 12), (13, 11), (7, 22), (18, 13)]:
        pt(d, lx, ly, (220, 230, 80, 255))
        pt(d, lx + 1, ly, C_GRASS_BRIGHT)

    return img

def make_prop_bush():
    """Round lush shrub with red berries (16x16)."""
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    d.ellipse([2, 9, 14, 15], fill=(20, 25, 15, 130))
    d.ellipse([2, 3, 14, 14], fill=C_GRASS_SHADOW)
    d.ellipse([3, 2, 13, 13], fill=C_GRASS_MID)
    d.ellipse([3, 2, 10, 9], fill=C_GRASS_LIT)
    pt(d, 5, 3, C_GRASS_BRIGHT)
    pt(d, 6, 3, C_GRASS_BRIGHT)

    # Red berries
    for bx, by in [(4, 6), (11, 5), (7, 10), (12, 9)]:
        pt(d, bx, by, C_CRIMSON_GLOW)
        pt(d, bx, by + 1, C_CRIMSON_DARK)
    return img

def make_prop_torch():
    """Standing Iron Torch Brazier with flame (16x16)."""
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 4, 14, 8, 2, (15, 15, 20, 140))
    # Tripod stand
    d.line([(5, 14), (7, 8)], fill=(60, 55, 65, 255))
    d.line([(10, 14), (8, 8)], fill=(60, 55, 65, 255))
    vline(d, 7, 7, 14, (75, 70, 85, 255))
    vline(d, 8, 7, 14, (90, 85, 100, 255))

    # Iron bowl
    rect(d, 5, 6, 6, 2, (45, 40, 55, 255))
    pt(d, 6, 5, (100, 95, 110, 255))

    # Flame
    rect(d, 6, 3, 4, 3, C_TORCH_RED)
    rect(d, 7, 2, 2, 3, C_TORCH_ORG)
    pt(d, 7, 1, C_TORCH_YEL)
    pt(d, 8, 2, C_TORCH_YEL)
    return img

def make_prop_barrel():
    """Banded Oak Barrel (16x16)."""
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    d.ellipse([3, 11, 13, 15], fill=(20, 15, 10, 140))
    rect(d, 4, 3, 8, 11, C_WOOD_SHADOW)
    rect(d, 5, 2, 6, 12, C_WOOD_MID)
    # Barrel curve & highlight
    vline(d, 5, 3, 12, C_WOOD_LIT)
    vline(d, 9, 3, 12, C_WOOD_DARK)
    # Iron Hoops
    hline(d, 4, 11, 4, (85, 90, 100, 255))
    hline(d, 4, 11, 11, (85, 90, 100, 255))
    return img

def make_prop_wall():
    """Gothic Stone Rampart Block (16x16)."""
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 1, 2, 14, 14, C_OBSID_DARK)
    rect(d, 2, 3, 12, 12, C_OBSID_MID)
    # Crenel cutout on top
    rect(d, 6, 2, 4, 4, (0, 0, 0, 0))
    # Stone course lines
    hline(d, 2, 13, 8, C_OBSID_DEEP)
    hline(d, 2, 13, 12, C_OBSID_DEEP)
    vline(d, 7, 3, 7, C_OBSID_DEEP)
    vline(d, 10, 9, 11, C_OBSID_DEEP)
    # Highlight
    hline(d, 2, 5, 3, C_OBSID_HI)
    hline(d, 10, 13, 3, C_OBSID_HI)
    return img


# ══════════════════════════════════════════════════════════════════════════════
# 4. MONSTERS (32x32 Chibi 2.5D Demon Minions)
# ══════════════════════════════════════════════════════════════════════════════

def make_monster_imp():
    """Baby Crimson Imp with bat wings, horns & torch (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Shadow
    d.ellipse([10, 24, 21, 29], fill=(20, 10, 15, 140))
    # Bat Wings (Left & Right)
    d.polygon([(9, 14), (4, 10), (7, 18)], fill=(120, 20, 40, 255))
    d.polygon([(22, 14), (27, 10), (24, 18)], fill=(120, 20, 40, 255))
    # Body (Chubby red demon)
    rect(d, 12, 15, 8, 9, C_CRIMSON_DARK)
    rect(d, 13, 16, 6, 7, (220, 45, 75, 255))
    # Head
    rect(d, 11, 8, 10, 8, (220, 45, 75, 255))
    # Horns
    d.polygon([(11, 8), (9, 4), (13, 7)], fill=(40, 30, 45, 255))
    d.polygon([(20, 8), (22, 4), (18, 7)], fill=(40, 30, 45, 255))
    # Yellow glowing eyes
    pt(d, 13, 11, C_TORCH_YEL)
    pt(d, 14, 11, C_TORCH_YEL)
    pt(d, 17, 11, C_TORCH_YEL)
    pt(d, 18, 11, C_TORCH_YEL)
    # Torch in hand
    vline(d, 23, 15, 22, C_WOOD_DARK)
    pt(d, 23, 14, C_TORCH_RED)
    pt(d, 23, 13, C_TORCH_YEL)
    return img

def make_monster_slime():
    """Translucent Emerald Mire Slime with glowing soul core (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    d.ellipse([9, 23, 23, 28], fill=(15, 30, 15, 140))
    # Slime dome
    d.pieslice([8, 11, 24, 27], 180, 360, fill=(35, 160, 95, 230))
    rect(d, 8, 19, 16, 6, (35, 160, 95, 230))
    # Specular gloss
    d.ellipse([10, 12, 16, 16], fill=(120, 240, 170, 240))
    pt(d, 12, 13, (255, 255, 255, 255))
    # Glowing Soul Core inside
    d.ellipse([14, 18, 18, 22], fill=C_MANA_CYAN)
    pt(d, 16, 20, (255, 255, 255, 255))
    # Cute eyes
    pt(d, 12, 18, (15, 50, 30, 255))
    pt(d, 19, 18, (15, 50, 30, 255))
    return img

def make_monster_bonepup():
    """Cute Skeleton Dog with glowing blue soul fire eyes (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    d.ellipse([9, 23, 23, 28], fill=(20, 20, 25, 130))
    # Ribcage body
    rect(d, 14, 15, 7, 7, (210, 210, 200, 255))
    for ry in [16, 18, 20]:
        hline(d, 15, 19, ry, (40, 40, 50, 255))
    # Legs (Bones)
    vline(d, 14, 21, 25, (220, 220, 215, 255))
    vline(d, 19, 21, 25, (220, 220, 215, 255))
    # Head skull
    rect(d, 8, 10, 8, 7, (235, 235, 225, 255))
    rect(d, 6, 13, 4, 4, (235, 235, 225, 255)) # snout
    # Blue soul fire eye
    pt(d, 9, 12, C_MANA_CYAN)
    pt(d, 10, 12, (220, 255, 255, 255))
    # Bone tail
    d.line([(21, 17), (24, 14)], fill=(220, 220, 215, 255))
    return img

def make_monster_grim():
    """Hooded Shadow Spectre with purple astral trail (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    d.ellipse([10, 24, 22, 28], fill=(20, 15, 30, 120))
    # Floating tattered cloak
    d.polygon([(16, 8), (9, 23), (12, 25), (16, 22), (20, 25), (23, 23)], fill=C_OBSID_DARK)
    # Cloak fold highlights
    d.line([(16, 8), (14, 21)], fill=C_ROOF_LIT)
    # Hood opening
    rect(d, 13, 11, 6, 6, (10, 8, 15, 255))
    # Glowing cyclops eye lens
    d.ellipse([14, 12, 17, 15], fill=C_MANA_PURPLE)
    pt(d, 15, 13, (255, 220, 255, 255))
    return img

def make_monster_goblin():
    """Goblin Tinker with brass goggles & mechanical backpack (32x32)."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    d.ellipse([10, 24, 22, 29], fill=(20, 20, 15, 140))
    # Leather backpack
    rect(d, 18, 14, 6, 8, C_WOOD_SHADOW)
    # Green goblin body
    rect(d, 11, 15, 8, 8, (120, 160, 50, 255))
    # Big head with pointy ears
    d.polygon([(7, 10), (11, 12), (11, 14)], fill=(120, 160, 50, 255)) # left ear
    d.polygon([(24, 10), (20, 12), (20, 14)], fill=(120, 160, 50, 255)) # right ear
    rect(d, 11, 9, 9, 7, (135, 175, 60, 255))
    # Brass Goggles
    rect(d, 11, 10, 9, 3, C_GOLD_DARK)
    rect(d, 12, 10, 3, 3, C_MANA_CYAN)
    rect(d, 16, 10, 3, 3, C_MANA_CYAN)
    # Wrench in hand
    vline(d, 9, 17, 23, (160, 165, 175, 255))
    rect(d, 8, 16, 3, 2, (180, 185, 195, 255))
    return img


# ══════════════════════════════════════════════════════════════════════════════
# 5. UI (9-Slice Gothic Stone Panel & Icons)
# ══════════════════════════════════════════════════════════════════════════════

def make_ui_panel_9slice():
    """32x32 Gothic Obsidian & Gold filigree 9-slice panel."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Base panel slate
    rect(d, 2, 2, 28, 28, (24, 22, 32, 245), outline=C_OBSID_DARK)
    rect(d, 4, 4, 24, 24, (32, 30, 42, 245))
    # Inner gold filigree line
    rect(d, 5, 5, 22, 22, None, outline=(100, 80, 30, 200))
    # 4 Ornate corner studs
    for (cx, cy) in [(4, 4), (27, 4), (4, 27), (27, 27)]:
        pt(d, cx, cy, C_MANA_GOLD)
        pt(d, cx + 1, cy, C_GOLD_DARK)
    return img

def make_ui_button(state="normal"):
    """48x16 3D Gothic Button."""
    img = Image.new("RGBA", (48, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    bg = C_OBSID_MID if state == "normal" else (70, 65, 88, 255) if state == "hover" else C_OBSID_DARK
    outline = C_GOLD_DARK if state != "hover" else C_MANA_GOLD
    rect(d, 1, 1, 46, 14, bg, outline=C_OBSID_DARK)
    rect(d, 2, 2, 44, 12, None, outline=outline)
    hline(d, 3, 44, 2, C_OBSID_HI if state != "pressed" else C_OBSID_DEEP)
    return img

def make_icon_gold():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse([2, 2, 13, 13], fill=C_MANA_GOLD, outline=C_GOLD_DARK)
    d.ellipse([4, 4, 11, 11], fill=(255, 235, 110, 255))
    pt(d, 5, 5, (255, 255, 220, 255))
    return img

def make_icon_food():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    # Juicy ham / meat on bone
    d.ellipse([4, 4, 13, 12], fill=(190, 60, 50, 255), outline=(100, 25, 20, 255))
    pt(d, 7, 7, (230, 110, 100, 255))
    d.line([(2, 12), (5, 9)], fill=(240, 235, 220, 255)) # bone
    return img

def make_icon_mana():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.polygon([(8, 1), (13, 7), (8, 14), (3, 7)], fill=C_MANA_CYAN, outline=(30, 120, 150, 255))
    d.polygon([(8, 3), (11, 7), (8, 12), (5, 7)], fill=(200, 245, 255, 255))
    return img

def make_icon_rumor():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    # Wax-sealed secret parchment
    rect(d, 3, 2, 10, 12, (225, 210, 175, 255), outline=(130, 110, 80, 255))
    d.line([(5, 5), (10, 5)], fill=(150, 130, 100, 255))
    d.line([(5, 8), (11, 8)], fill=(150, 130, 100, 255))
    # Red wax seal
    d.ellipse([7, 9, 11, 13], fill=C_CRIMSON_DARK)
    pt(d, 9, 11, C_CRIMSON_GLOW)
    return img

def make_icon_worker():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    # Pickaxe icon
    d.line([(3, 13), (11, 5)], fill=C_WOOD_LIT)
    d.polygon([(10, 3), (14, 4), (13, 8)], fill=(180, 185, 195, 255))
    return img


# ══════════════════════════════════════════════════════════════════════════════
# MAIN EXPORT
# ══════════════════════════════════════════════════════════════════════════════

def save_dual(rel_folder, name, img):
    """Save to both Assets/Resources/ and Assets/Art/Sprites/"""
    p1 = os.path.join("Assets", "Resources", rel_folder, name)
    p2 = os.path.join("Assets", "Art", "Sprites", rel_folder, name)
    os.makedirs(os.path.dirname(p1), exist_ok=True)
    os.makedirs(os.path.dirname(p2), exist_ok=True)
    img.save(p1)
    img.save(p2)
    print(f" Saved: {rel_folder}/{name} ({img.size[0]}x{img.size[1]})")

def main():
    print("=== Generating High-Definition 16-Bit Pixel Assets for RUMORS ===")

    # 1. Tiles
    tiles = {
        "Tile_Ground_DarkGrass.png": make_tile_dark_grass(),
        "Tile_Ground_Dirt.png":      make_tile_dirt(),
        "Tile_Cobblestone.png":      make_tile_cobblestone(),
        "Tile_Castle_Floor.png":     make_tile_castle_floor(),
        "Tile_Ground_Stone.png":     make_tile_stone(),
        "Tile_Ground_Swamp.png":     make_tile_swamp(),
        "Tile_Highlight.png":        make_tile_highlight(),
    }
    for name, img in tiles.items():
        save_dual("Tiles", name, img)

    # 2. Buildings
    buildings = {
        "Building_Castle.png":             make_building_castle(level=1),
        "Building_Castle_Upgraded.png":    make_building_castle(level=2),
        "Building_Mine.png":               make_building_mine(level=1),
        "Building_Mine_Upgraded.png":      make_building_mine(level=2),
        "Building_Farm.png":               make_building_farm(level=1),
        "Building_Farm_Upgraded.png":      make_building_farm(level=2),
        "Building_Dormitory.png":          make_building_dormitory(level=1),
        "Building_Dormitory_Upgraded.png": make_building_dormitory(level=2),
        "Building_Warehouse.png":          make_building_warehouse(level=1),
        "Building_Warehouse_Upgraded.png": make_building_warehouse(level=2),
        "Building_ManaTower.png":          make_building_mana_tower(),
        "Building_Training.png":           make_building_training(),
        "Building_Construction.png":       make_building_construction(),
    }
    for name, img in buildings.items():
        save_dual("Buildings", name, img)

    # 3. Props
    props = {
        "Prop_Tree.png":   make_prop_tree(),
        "Prop_Bush.png":   make_prop_bush(),
        "Prop_Torch.png":  make_prop_torch(),
        "Prop_Barrel.png": make_prop_barrel(),
        "Prop_Wall.png":   make_prop_wall(),
    }
    for name, img in props.items():
        save_dual("Props", name, img)

    # 4. Monsters
    monsters = {
        "Monster_ImpScrap.png":     make_monster_imp(),
        "Monster_MireSlime.png":    make_monster_slime(),
        "Monster_BonePup.png":      make_monster_bonepup(),
        "Monster_GrimShadow.png":   make_monster_grim(),
        "Monster_GoblinTinker.png": make_monster_goblin(),
    }
    for name, img in monsters.items():
        save_dual("Monsters", name, img)

    # 5. UI
    ui = {
        "UI_Panel_9Slice.png":   make_ui_panel_9slice(),
        "UI_Button_Normal.png":  make_ui_button("normal"),
        "UI_Button_Hover.png":   make_ui_button("hover"),
        "UI_Button_Pressed.png": make_ui_button("pressed"),
        "Icon_Gold.png":         make_icon_gold(),
        "Icon_Food.png":         make_icon_food(),
        "Icon_Mana.png":         make_icon_mana(),
        "Icon_Rumor.png":        make_icon_rumor(),
        "Icon_Worker.png":       make_icon_worker(),
    }
    for name, img in ui.items():
        save_dual("UI", name, img)

    print("\n[SUCCESS] All HD pixel art assets generated and mirrored successfully!")

if __name__ == "__main__":
    main()
