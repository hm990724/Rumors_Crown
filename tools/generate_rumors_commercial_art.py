"""
RUMORS — Master-Grade Commercial Pixel Art Engine
Directly benchmarked from high-end indie references in Docs/References.
Engineered with rich palettes, distinct silhouettes, organic tile connections,
authentic props (barrels, crates, braziers, minecarts, glowing mushrooms, targets),
and expressive, cute 2.5D dark fantasy monsters.
"""

from PIL import Image, ImageDraw
import os

RES_DIR = r"C:\Users\lookm\Documents\GitHub\Rumors_Crown\Assets\Resources"
TILES_DIR = os.path.join(RES_DIR, "Tiles")
BUILDINGS_DIR = os.path.join(RES_DIR, "Buildings")
MONSTERS_DIR = os.path.join(RES_DIR, "Monsters")
UI_DIR = os.path.join(RES_DIR, "UI")
PROPS_DIR = os.path.join(RES_DIR, "Props")

for d in [TILES_DIR, BUILDINGS_DIR, MONSTERS_DIR, UI_DIR, PROPS_DIR]:
    os.makedirs(d, exist_ok=True)

# -----------------------------------------------------------------------------
# DRAWING PRIMITIVES
# -----------------------------------------------------------------------------
def pt(d, x, y, c):
    d.point((x, y), fill=c)

def rect(d, x, y, w, h, c):
    if w <= 0 or h <= 0: return
    d.rectangle([x, y, x + w - 1, y + h - 1], fill=c)

def outline_rect(d, x, y, w, h, fill_c, border_c):
    if w <= 0 or h <= 0: return
    d.rectangle([x, y, x + w - 1, y + h - 1], fill=fill_c, outline=border_c)

def hline(d, x1, x2, y, c):
    d.line([(x1, y), (x2, y)], fill=c)

def vline(d, x, y1, y2, c):
    d.line([(x, y1), (x, y2)], fill=c)

# Authentic Reference Palettes (Extracted directly from Reference Screenshots 1~5)
# Grass & Environment
C_GRASS_BASE   = (153, 159, 24, 255)   # #999F18 Warm olive
C_GRASS_MID    = (136, 137, 69, 255)   # #888945 Soft meadow
C_GRASS_DARK   = (100, 83, 10, 255)    # #64530A Soil shadow
C_GRASS_DEEP   = (70, 70, 49, 255)     # #464631 Edge shade
C_GRASS_LIT    = (178, 175, 134, 255)  # #B2AF86 Sun highlight
C_FLOWER_YEL   = (255, 235, 120, 255)  # Wild yellow flora
C_FLOWER_WHT   = (255, 255, 244, 255)  # Wild white daisy

# Flagstone & Rock (References 1, 2, 3)
C_STONE_LIT    = (178, 175, 134, 255)  # Top highlight bevel
C_STONE_MID    = (162, 157, 121, 255)  # Stone body
C_STONE_DARK   = (123, 121, 110, 255)  # Shadow side
C_STONE_DEEP   = (85, 88, 85, 255)     # Outer outline
C_STONE_JOINT  = (88, 81, 70, 255)     # Mortar/earth gap

# Timber & Wood Props (References 3, 4, 5)
C_WOOD_LIT     = (165, 129, 86, 255)   # #A58156
C_WOOD_MID     = (126, 94, 66, 255)    # #7E5E42
C_WOOD_DARK    = (84, 56, 28, 255)     # #54381C
C_WOOD_DEEP    = (45, 22, 2, 255)      # #2D1602

# Obsidian & Dark Fantasy Castle
C_OBSID_LIT    = (115, 105, 140, 255)
C_OBSID_MID    = (80, 72, 102, 255)
C_OBSID_DARK   = (50, 44, 68, 255)
C_OBSID_DEEP   = (30, 25, 42, 255)

# Glows & Magic (References 3, 4)
C_RUNE_CYAN    = (111, 173, 185, 255)  # #6FADB9 Glowing cyan
C_RUNE_CORE    = (248, 255, 255, 255)
C_FIRE_ORANGE  = (255, 140, 30, 255)
C_FIRE_YEL     = (255, 225, 75, 255)
C_MANA_PURPLE  = (180, 85, 245, 255)
C_MANA_LIT     = (225, 175, 255, 255)
C_SHADOW_CAST  = (25, 22, 35, 100)


# =============================================================================
# 1. ORGANIC TILES (16x16) — True Benchmark to Reference 1 & 2
# =============================================================================

def make_tile_dark_grass():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    # Warm rich meadow base
    rect(d, 0, 0, 16, 16, C_GRASS_MID)
    # Soft light patches
    for (x, y, w, h) in [(1,1,5,4), (8,2,6,5), (3,8,6,5), (10,9,5,5)]:
        rect(d, x, y, w, h, C_GRASS_BASE)
    # Directional grass tufts (lit top-left, shadow bottom-right)
    tufts = [(2,2),(6,1),(12,3),(3,6),(8,7),(13,8),(1,11),(7,12),(13,13)]
    for (x, y) in tufts:
        pt(d, x, y, C_GRASS_LIT)
        if y < 15: pt(d, x, y+1, C_GRASS_DARK)
        if x < 15 and y < 15: pt(d, x+1, y+1, C_GRASS_DEEP)
    # Tiny wild flowers (Reference 1 & 2 flora)
    pt(d, 4, 3, C_FLOWER_WHT); pt(d, 11, 4, C_FLOWER_YEL)
    pt(d, 2, 13, C_FLOWER_YEL); pt(d, 14, 11, C_FLOWER_WHT)
    return img

def make_tile_cobblestone():
    """
    Organic Flagstone road matching Reference 1 & 2:
    Smooth rounded stone pavers with grass creeping into the natural mortar joints.
    """
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    # Soil/grass underbed
    rect(d, 0, 0, 16, 16, C_GRASS_DARK)

    # 4 distinct rounded flagstone shapes
    stones = [
        (1, 1, 6, 5),   # Top-left stone
        (8, 1, 7, 6),   # Top-right stone
        (1, 7, 6, 8),   # Bottom-left stone
        (8, 8, 7, 7)    # Bottom-right stone
    ]
    for (sx, sy, sw, sh) in stones:
        # Mortar shadow
        rect(d, sx, sy, sw, sh, C_STONE_DARK)
        # Stone body
        rect(d, sx, sy, sw - 1, sh - 1, C_STONE_MID)
        # Top-left bevel highlight
        hline(d, sx, sx + sw - 2, sy, C_STONE_LIT)
        vline(d, sx, sy, sy + sh - 2, C_STONE_LIT)
        # Bottom-right bevel shadow
        hline(d, sx + 1, sx + sw - 1, sy + sh - 1, C_STONE_DEEP)
        vline(d, sx + sw - 1, sy + 1, sy + sh - 1, C_STONE_DEEP)

    # Natural grass blades poking through gaps
    for (gx, gy) in [(0,3),(7,1),(7,7),(0,10),(15,6),(7,14),(14,14)]:
        pt(d, gx, gy, C_GRASS_BASE)
        if gy > 0: pt(d, gx, gy-1, C_GRASS_LIT)
    return img

def make_tile_castle_floor():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    rect(d, 0, 0, 16, 16, C_OBSID_DEEP)
    # 4 Large square obsidian flagstones
    for (sx, sy) in [(1,1), (9,1), (1,9), (9,9)]:
        rect(d, sx, sy, 6, 6, C_OBSID_MID)
        hline(d, sx, sx+4, sy, C_OBSID_LIT)
        vline(d, sx, sy, sy+4, C_OBSID_LIT)
        hline(d, sx+1, sx+5, sy+5, C_OBSID_DARK)
        vline(d, sx+5, sy+1, sy+5, C_OBSID_DARK)
    # Center glowing rune cross
    pt(d, 7, 7, C_MANA_PURPLE); pt(d, 8, 7, C_MANA_LIT)
    pt(d, 7, 8, C_MANA_LIT);    pt(d, 8, 8, C_MANA_PURPLE)
    return img

def make_tile_dirt():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    rect(d, 0, 0, 16, 16, (140, 102, 66, 255))
    rect(d, 1, 1, 14, 14, (155, 115, 78, 255))
    # Soft dirt grain & small pebbles
    pebbles = [(2,3),(8,2),(13,5),(4,9),(11,10),(3,13),(9,14),(14,13)]
    for (x, y) in pebbles:
        pt(d, x, y, (190, 155, 120, 255))
        if y < 15: pt(d, x, y+1, (105, 75, 45, 255))
    return img

def make_tile_stone():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    rect(d, 0, 0, 16, 16, (90, 88, 96, 255))
    rect(d, 2, 2, 12, 12, (115, 112, 122, 255))
    # Chipped stone veins
    hline(d, 3, 8, 5, (140, 138, 150, 255))
    vline(d, 8, 5, 10, (140, 138, 150, 255))
    hline(d, 8, 13, 10, (65, 62, 72, 255))
    return img

def make_tile_swamp():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    rect(d, 0, 0, 16, 16, (48, 68, 56, 255))
    # Dark stagnant pool
    rect(d, 2, 3, 12, 10, (32, 50, 42, 255))
    rect(d, 4, 5, 8, 6, (22, 38, 32, 255))
    # Glowing algae bubbles
    pt(d, 5, 6, (90, 240, 165, 255))
    pt(d, 10, 8, (90, 240, 165, 255))
    pt(d, 6, 9, (140, 255, 200, 255))
    return img

def make_tile_highlight():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    rect(d, 2, 2, 12, 12, (255, 225, 75, 35))
    gold, gold_hi = (255, 215, 50, 255), (255, 245, 180, 255)
    # 4 Corner brackets
    # TL
    pt(d, 0, 0, gold_hi); pt(d, 1, 0, gold); pt(d, 2, 0, gold); pt(d, 0, 1, gold); pt(d, 0, 2, gold)
    # TR
    pt(d, 15, 0, gold_hi); pt(d, 14, 0, gold); pt(d, 13, 0, gold); pt(d, 15, 1, gold); pt(d, 15, 2, gold)
    # BL
    pt(d, 0, 15, gold_hi); pt(d, 1, 15, gold); pt(d, 2, 15, gold); pt(d, 0, 14, gold); pt(d, 0, 13, gold)
    # BR
    pt(d, 15, 15, gold_hi); pt(d, 14, 15, gold); pt(d, 13, 15, gold); pt(d, 15, 14, gold); pt(d, 15, 13, gold)
    return img


# =============================================================================
# 2. DISTINCT BUILDINGS (32x32, Castle 48x48) — Rich Props & Unique Silhouettes
# =============================================================================

def make_building_castle(level=1):
    """
    Grand Gothic Obsidian Fortress (48x48):
    Top-down 3/4 perspective with stone coursing, archway gate, 2 sentry towers,
    red demonic banners, flame braziers, and mana spire.
    Upgraded adds 2nd tier towers, blue arcane circle altar, and golden garnishes.
    """
    size = 48
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Cast shadow on ground
    rect(d, 4, 38, 40, 8, C_SHADOW_CAST)

    # Base Keep Walls
    rect(d, 6, 22, 36, 22, C_OBSID_DARK)
    rect(d, 8, 24, 32, 19, C_OBSID_MID)

    # Stone coursing lines
    for cy in [28, 34, 40]:
        hline(d, 8, 39, cy, C_OBSID_DEEP)
        for cx in range(10 + (cy % 4), 38, 6):
            pt(d, cx, cy - 1, C_OBSID_LIT)

    # Sentry Towers (Left & Right)
    for tx in [3, 33]:
        rect(d, tx, 14, 12, 30, C_OBSID_DARK)
        rect(d, tx + 1, 16, 10, 27, C_OBSID_MID)
        # Tower battlements
        rect(d, tx, 11, 4, 4, C_OBSID_LIT)
        rect(d, tx + 8, 11, 4, 4, C_OBSID_LIT)
        rect(d, tx + 4, 13, 4, 2, C_OBSID_DARK)
        # Torch brazier on tower
        rect(d, tx + 4, 10, 4, 2, (30, 25, 40, 255))
        pt(d, tx + 5, 8, C_FIRE_YEL); pt(d, tx + 6, 8, C_FIRE_ORANGE)
        pt(d, tx + 5, 9, C_FIRE_ORANGE); pt(d, tx + 6, 9, C_FIRE_YEL)

    # Central Keep Spire (Tier 2)
    rect(d, 16, 6, 16, 20, C_OBSID_DARK)
    rect(d, 18, 8, 12, 17, C_OBSID_MID)
    # Spire Roof
    for i in range(7):
        hline(d, 24 - i, 24 + i, 6 - i, (115, 55, 155, 255))
    # Mana Crystal Tip
    pt(d, 24, -1 if level >= 2 else 0, C_MANA_LIT)
    pt(d, 23, 0, C_MANA_PURPLE); pt(d, 25, 0, C_MANA_PURPLE)

    # Grand Arched Portcullis (Castle Gate)
    rect(d, 19, 28, 10, 16, C_OBSID_DEEP)
    # Iron portcullis bars
    for bx in range(20, 29, 2):
        vline(d, bx, 29, 43, (140, 140, 155, 255))
    hline(d, 20, 28, 33, (140, 140, 155, 255))
    hline(d, 20, 28, 38, (140, 140, 155, 255))

    # Royal Crimson Banners beside gate
    rect(d, 16, 26, 2, 10, (185, 30, 45, 255))
    rect(d, 30, 26, 2, 10, (185, 30, 45, 255))
    pt(d, 16, 36, (140, 20, 30, 255)); pt(d, 30, 36, (140, 20, 30, 255))

    # Entrance Flagstone steps
    rect(d, 17, 43, 14, 3, C_STONE_MID)
    hline(d, 17, 30, 43, C_STONE_LIT)

    if level >= 2:
        # UPGRADED: Golden garnishes & Arcane Altar (Reference 3 altar)
        # Golden crenels
        rect(d, 3, 10, 4, 2, (255, 215, 60, 255))
        rect(d, 41, 10, 4, 2, (255, 215, 60, 255))
        # Blue Soul Flames on towers
        pt(d, 8, 7, C_RUNE_CORE); pt(d, 9, 7, C_RUNE_CYAN)
        pt(d, 38, 7, C_RUNE_CORE); pt(d, 39, 7, C_RUNE_CYAN)
        # Arcane rune circle before gate
        rect(d, 20, 41, 8, 4, C_RUNE_CYAN)
        pt(d, 23, 42, C_RUNE_CORE); pt(d, 24, 42, C_RUNE_CORE)

    return img

def make_building_mine(level=1):
    """
    Mine (32x32) — Tunnel Shaft & Minecart Track!
    Rock hillside with sturdy wooden beam frame, tunnel entrance leading underground,
    metal rails extending out, a wooden minecart loaded with glowing ores, and an anvil/pickaxe.
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Cast shadow
    rect(d, 2, 26, 28, 5, C_SHADOW_CAST)

    # Rock hillside excavation
    rect(d, 2, 6, 28, 22, (95, 92, 105, 255))
    rect(d, 4, 8, 24, 19, (120, 116, 130, 255))
    # Rock crags
    hline(d, 4, 12, 7, (150, 145, 160, 255))
    hline(d, 18, 26, 7, (150, 145, 160, 255))

    # Mine Shaft Tunnel Arch (Dark opening going down)
    rect(d, 8, 12, 16, 16, (20, 18, 25, 255))
    rect(d, 10, 14, 12, 14, (10, 8, 15, 255))

    # Sturdy Heavy Timber Frame (Reference 3 & 4 wood beams)
    # Left beam
    rect(d, 6, 10, 3, 19, C_WOOD_DARK)
    vline(d, 7, 10, 28, C_WOOD_LIT)
    # Right beam
    rect(d, 23, 10, 3, 19, C_WOOD_DARK)
    vline(d, 24, 10, 28, C_WOOD_LIT)
    # Top lintel beam
    rect(d, 5, 9, 22, 4, C_WOOD_DARK)
    hline(d, 5, 26, 9, C_WOOD_LIT)
    hline(d, 6, 25, 12, C_WOOD_DEEP)

    # Minecart Iron Rails running out of the tunnel
    vline(d, 12, 18, 30, (135, 135, 150, 255))
    vline(d, 19, 18, 30, (135, 135, 150, 255))
    # Wooden rail ties
    for ry in [21, 25, 29]:
        hline(d, 10, 21, ry, C_WOOD_DARK)

    # Wooden Minecart with Metal Bands
    rect(d, 11, 21, 10, 7, C_WOOD_MID)
    rect(d, 12, 22, 8, 5, C_WOOD_LIT)
    # Iron rim & wheels
    hline(d, 11, 20, 21, (70, 70, 85, 255))
    hline(d, 11, 20, 27, (70, 70, 85, 255))
    pt(d, 12, 28, (45, 45, 55, 255)); pt(d, 19, 28, (45, 45, 55, 255))

    # Glowing Purple/Gold Ores in cart
    pt(d, 13, 20, C_MANA_PURPLE); pt(d, 14, 19, C_MANA_LIT); pt(d, 15, 20, (255, 215, 50, 255))
    pt(d, 17, 20, C_MANA_PURPLE); pt(d, 18, 19, (255, 235, 100, 255))

    # Anvil & hammer beside mine (Reference 4 prop!)
    rect(d, 26, 23, 4, 3, (85, 85, 95, 255))
    rect(d, 27, 26, 2, 3, (60, 60, 70, 255))
    # Hanging lantern on timber beam
    rect(d, 5, 13, 2, 4, C_FIRE_ORANGE)
    pt(d, 5, 14, C_FIRE_YEL)

    if level >= 2:
        # UPGRADED: Derrick Crane Pulley atop mine + extra loaded cart
        # Crane A-frame
        vline(d, 16, 1, 9, C_WOOD_LIT)
        hline(d, 14, 18, 1, C_WOOD_DARK)
        # Pulley wheel & rope
        pt(d, 18, 2, (180, 180, 195, 255))
        vline(d, 18, 3, 10, (215, 195, 160, 255))
        # Large glowing crystal cluster outside
        pt(d, 3, 21, C_MANA_LIT); pt(d, 4, 20, C_MANA_PURPLE); pt(d, 4, 22, C_MANA_PURPLE)

    return img

def make_building_farm(level=1):
    """
    Mushroom Farm (32x32) — Glowing Fungal Plots & Awning!
    Soil farm rows with wooden fence, striped awning shelter (Reference 5 awning!),
    clusters of cyan, purple, and yellow bioluminescent mushrooms, water barrel & bucket.
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Cast shadow
    rect(d, 2, 26, 28, 5, C_SHADOW_CAST)

    # Farm soil plot
    rect(d, 2, 12, 28, 17, (115, 80, 52, 255))
    rect(d, 3, 13, 26, 15, (135, 96, 64, 255))
    # Furrowed soil ridges
    for fy in [16, 20, 24]:
        hline(d, 4, 27, fy, (95, 65, 42, 255))

    # Striped Awning Work Shelter (Reference 5 awning!)
    # Wooden support poles
    vline(d, 4, 5, 14, C_WOOD_DARK)
    vline(d, 27, 5, 14, C_WOOD_DARK)
    # Striped fabric awning (Purple & Cream)
    for ax in range(4, 28):
        col = (145, 65, 185, 255) if (ax // 3) % 2 == 0 else (245, 235, 210, 255)
        vline(d, ax, 3, 7, col)
    # Scalloped fringe
    for fx in range(4, 28, 3):
        pt(d, fx + 1, 8, (245, 235, 210, 255))

    # Bioluminescent Glowing Mushrooms (Cyan & Purple & Amber)
    # Plot 1: Giant Cyan Mushroom
    rect(d, 7, 18, 7, 4, (45, 185, 175, 255))
    rect(d, 8, 17, 5, 2, (95, 245, 225, 255))
    # Stem
    rect(d, 9, 22, 3, 3, (220, 245, 240, 255))
    pt(d, 9, 18, (255, 255, 255, 255)) # Spore glow dot

    # Plot 2: Violet Cluster
    rect(d, 16, 17, 6, 3, (175, 65, 225, 255))
    rect(d, 17, 16, 4, 2, (225, 145, 255, 255))
    rect(d, 18, 20, 2, 4, (240, 220, 250, 255))
    pt(d, 18, 17, (255, 255, 255, 255))

    # Plot 3: Golden Spore Mushrooms
    rect(d, 23, 19, 4, 3, (235, 175, 45, 255))
    rect(d, 24, 22, 2, 3, (250, 235, 180, 255))
    pt(d, 24, 19, (255, 250, 160, 255))

    # Water Barrel (Reference 1 & 4 barrel!)
    rect(d, 3, 19, 5, 7, C_WOOD_DARK)
    rect(d, 4, 20, 3, 5, C_WOOD_MID)
    hline(d, 3, 7, 21, (80, 75, 90, 255)) # Iron hoop
    hline(d, 3, 7, 25, (80, 75, 90, 255))
    # Water surface inside
    pt(d, 5, 19, (80, 175, 215, 255))

    if level >= 2:
        # UPGRADED: Glass Dome Greenhouse structure + Giant Grand Spore Cap
        rect(d, 12, 13, 8, 5, (235, 75, 245, 255))
        rect(d, 13, 11, 6, 3, (255, 175, 255, 255))
        rect(d, 15, 16, 2, 7, (255, 245, 255, 255))
        # Spore particles in air
        pt(d, 11, 10, C_MANA_LIT); pt(d, 21, 9, C_RUNE_CORE); pt(d, 14, 6, C_MANA_LIT)

    return img

def make_building_dormitory(level=1):
    """
    Dormitory (32x32) — Cozy 2-Story Demon Barracks!
    Wooden lodge with 2nd-floor balcony/terrace, laundry line (Reference 5!),
    glowing warm windows, entrance bench (Reference 4 bench), chimney with smoke.
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 2, 26, 28, 5, C_SHADOW_CAST)

    # 1st Floor Timber Body
    rect(d, 4, 15, 24, 13, C_WOOD_DARK)
    rect(d, 5, 16, 22, 11, C_WOOD_MID)
    # Horizontal wood planks
    for py in [18, 22, 25]:
        hline(d, 5, 26, py, C_WOOD_DEEP)

    # 2nd Floor Roof & Gable
    for i in range(8):
        hline(d, 16 - i * 2, 16 + i * 2, 7 + i, (60, 45, 75, 255))
    # Roof ridge
    hline(d, 14, 18, 6, (95, 75, 115, 255))

    # Chimney with smoke puff
    rect(d, 22, 3, 4, 7, (85, 80, 95, 255))
    pt(d, 23, 1, (220, 220, 230, 180)); pt(d, 24, 0, (240, 240, 250, 140))

    # Warm Glowing Windows (Reference 4 light!)
    rect(d, 7, 18, 4, 4, (255, 185, 55, 255))
    rect(d, 8, 19, 2, 2, (255, 235, 140, 255))
    # Window cross frame
    hline(d, 7, 10, 19, C_WOOD_DEEP); vline(d, 8, 18, 21, C_WOOD_DEEP)

    # Wooden Door
    rect(d, 14, 19, 5, 8, C_WOOD_DEEP)
    rect(d, 15, 20, 3, 7, C_WOOD_MID)
    pt(d, 17, 23, (240, 200, 60, 255)) # Brass knob

    # Wooden Bench beside door (Reference 4 bench!)
    rect(d, 21, 22, 7, 2, C_WOOD_LIT)
    rect(d, 22, 24, 5, 3, C_WOOD_DARK)
    vline(d, 22, 24, 27, C_WOOD_DEEP); vline(d, 26, 24, 27, C_WOOD_DEEP)

    # Clothesline / Bunting hanging from balcony (Reference 5 clothesline!)
    hline(d, 6, 25, 13, (180, 160, 140, 255))
    pt(d, 9, 14, (195, 45, 60, 255)); pt(d, 14, 14, (245, 240, 220, 255)); pt(d, 20, 14, (75, 145, 215, 255))

    if level >= 2:
        # UPGRADED: Rooftop Deck with Lantern + Extra Smoke
        rect(d, 7, 7, 6, 4, (195, 45, 60, 255)) # Rooftop tent
        rect(d, 25, 16, 3, 5, (255, 195, 50, 255)) # Lantern light
        pt(d, 21, -1, (240, 240, 250, 200)); pt(d, 23, -2, (240, 240, 250, 150))

    return img

def make_building_warehouse(level=1):
    """
    Warehouse (32x32) — Stacks of Crates, Barrels & Tarps!
    Completely distinct from cabins! Features wooden cargo crates with X-bracing (Ref 3, 4),
    stacked barrels, rope netting, shelter tarp, and a handcart.
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 2, 26, 28, 5, C_SHADOW_CAST)

    # Stone Raised Foundation Platform
    rect(d, 2, 20, 28, 8, C_STONE_DARK)
    rect(d, 3, 20, 26, 7, C_STONE_MID)
    hline(d, 3, 28, 20, C_STONE_LIT)

    # Shelter Roof Posts
    vline(d, 4, 5, 20, C_WOOD_DARK); vline(d, 27, 5, 20, C_WOOD_DARK)
    # Timber Roof
    for i in range(5):
        hline(d, 3 + i, 28 - i, 5 - i, C_WOOD_DARK)
        hline(d, 4 + i, 27 - i, 5 - i, C_WOOD_LIT)

    # Crates Stacked (Reference 3 & 4 X-braced Crates!)
    # Bottom Crate 1 (Left)
    rect(d, 5, 14, 8, 8, C_WOOD_DARK)
    rect(d, 6, 15, 6, 6, C_WOOD_LIT)
    # X-Brace
    d.line([(6, 15), (11, 20)], fill=C_WOOD_DARK)
    d.line([(6, 20), (11, 15)], fill=C_WOOD_DARK)

    # Bottom Crate 2 (Middle)
    rect(d, 13, 14, 8, 8, C_WOOD_DARK)
    rect(d, 14, 15, 6, 6, C_WOOD_MID)
    d.line([(14, 15), (19, 20)], fill=C_WOOD_DARK)
    d.line([(14, 20), (19, 15)], fill=C_WOOD_DARK)

    # Top Crate 3 (Stacked on top)
    rect(d, 9, 8, 7, 7, C_WOOD_DARK)
    rect(d, 10, 9, 5, 5, C_WOOD_LIT)
    d.line([(10, 9), (14, 13)], fill=C_WOOD_DARK)
    d.line([(10, 13), (14, 9)], fill=C_WOOD_DARK)

    # Wooden Barrels stacked on right (Reference 1, 3, 4 barrels!)
    rect(d, 22, 13, 6, 9, C_WOOD_DARK)
    rect(d, 23, 14, 4, 7, C_WOOD_MID)
    hline(d, 22, 27, 15, (65, 60, 75, 255))
    hline(d, 22, 27, 20, (65, 60, 75, 255))

    # Protective Green Tarp draped over side
    rect(d, 18, 11, 4, 6, (55, 95, 65, 255))
    rect(d, 18, 12, 3, 4, (75, 125, 85, 255))

    if level >= 2:
        # UPGRADED: Golden Treasure Chest atop platform (Reference 3 chest!) + Crane
        rect(d, 10, 3, 8, 6, (185, 125, 45, 255)) # Gold Chest
        rect(d, 11, 4, 6, 4, (245, 195, 65, 255))
        pt(d, 14, 5, (50, 40, 30, 255)) # Keyhole lock
        # Extra gold coins sack
        pt(d, 20, 8, (255, 215, 60, 255)); pt(d, 21, 9, (255, 225, 75, 255))

    return img

def make_building_mana_tower():
    """
    Mana Tower (32x32) — Runic Obelisk & Floating Violet Crystal (Ref 3)!
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 4, 26, 24, 5, C_SHADOW_CAST)

    # 3-Tier Stone Pedestal Base
    rect(d, 6, 24, 20, 5, C_STONE_DARK)
    rect(d, 8, 22, 16, 3, C_STONE_MID)
    rect(d, 10, 20, 12, 3, C_STONE_LIT)

    # 4 Obsidian Pillar Columns
    rect(d, 11, 9, 10, 12, C_OBSID_DARK)
    rect(d, 12, 10, 8, 10, C_OBSID_MID)
    # Glowing Cyan Runic Inscriptions on Pillar (Reference 3 Runic Monolith!)
    vline(d, 15, 11, 18, C_RUNE_CYAN)
    pt(d, 15, 13, C_RUNE_CORE); pt(d, 15, 16, C_RUNE_CORE)
    pt(d, 14, 12, C_RUNE_CYAN); pt(d, 16, 17, C_RUNE_CYAN)

    # Floating Arcane Diamond Crystal above pillars
    # Crystal top point
    pt(d, 15, 1, C_MANA_LIT); pt(d, 16, 1, C_MANA_LIT)
    rect(d, 14, 2, 4, 2, C_MANA_LIT)
    rect(d, 13, 4, 6, 3, C_MANA_PURPLE)
    rect(d, 14, 7, 4, 2, C_MANA_PURPLE)
    pt(d, 15, 9, (120, 45, 180, 255))
    # Brilliant core shine
    pt(d, 15, 4, (255, 255, 255, 255))
    pt(d, 16, 5, (255, 255, 255, 255))

    # Orbiting magical motes
    pt(d, 9, 4, C_RUNE_CORE); pt(d, 22, 5, C_MANA_LIT); pt(d, 11, 8, C_RUNE_CYAN)
    return img

def make_building_training():
    """
    Training Ground (32x32) — Archery Target & Straw Dummy (Reference 5)!
    Sand arena, wooden archery target dummy, weapon rack with spears & swords.
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 2, 26, 28, 5, C_SHADOW_CAST)

    # Sand / dirt sparring ring
    rect(d, 2, 14, 28, 14, (165, 135, 95, 255))
    rect(d, 3, 15, 26, 12, (185, 155, 115, 255))

    # Straw Dummy Target (Reference 5 Straw Dummy!)
    # Post
    rect(d, 8, 12, 3, 14, C_WOOD_DARK)
    # Cross arm
    rect(d, 5, 14, 9, 2, C_WOOD_DARK)
    # Straw torso & bullseye
    rect(d, 6, 11, 7, 8, (215, 185, 125, 255))
    rect(d, 7, 12, 5, 6, (235, 205, 145, 255))
    # Red Bullseye circle
    rect(d, 8, 14, 3, 3, (215, 45, 45, 255))
    pt(d, 9, 15, (255, 255, 255, 255))
    # Dummy head
    rect(d, 7, 7, 5, 5, (215, 185, 125, 255))
    pt(d, 8, 8, (60, 45, 30, 255)); pt(d, 10, 8, (60, 45, 30, 255))

    # Archery Target Stand on Right (Reference 5 Target!)
    rect(d, 22, 11, 7, 7, (230, 195, 130, 255))
    # Red target rings
    rect(d, 23, 12, 5, 5, (215, 45, 45, 255))
    rect(d, 24, 13, 3, 3, (255, 255, 255, 255))
    pt(d, 25, 14, (215, 45, 45, 255))
    # A-frame wooden stand
    d.line([(21, 23), (24, 17)], fill=C_WOOD_DARK)
    d.line([(29, 23), (26, 17)], fill=C_WOOD_DARK)

    # Weapon Rack in background
    rect(d, 15, 8, 6, 2, C_WOOD_DARK)
    vline(d, 16, 7, 16, (175, 175, 190, 255)) # Iron spear
    vline(d, 18, 9, 16, (175, 175, 190, 255)) # Iron sword

    return img

def make_building_construction():
    """
    Construction Site (32x32) — Wooden Scaffolding & Flag
    """
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 4, 25, 24, 5, C_SHADOW_CAST)
    # Timber frame grid
    for x in [6, 14, 22]:
        vline(d, x, 6, 26, C_WOOD_DARK)
        vline(d, x+1, 6, 26, C_WOOD_LIT)
    for y in [10, 17, 24]:
        hline(d, 5, 24, y, C_WOOD_DARK)
        hline(d, 5, 24, y-1, C_WOOD_LIT)
    # Diagonal braces
    d.line([(6, 10), (14, 17)], fill=C_WOOD_MID)
    d.line([(14, 17), (22, 24)], fill=C_WOOD_MID)
    # Pulley & rope
    pt(d, 15, 5, (190, 190, 200, 255))
    vline(d, 15, 6, 14, (215, 195, 160, 255))
    # Construction pennant flag
    rect(d, 22, 3, 5, 3, (225, 50, 60, 255))
    pt(d, 27, 4, (225, 50, 60, 255))
    return img


# =============================================================================
# 3. EXPRESSIVE MONSTERS (32x32) — 2.5D Animated Sequences (Idle, Walk, Attack)
# =============================================================================

def make_monster_imp(frame_type="main", f_idx=0):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Dynamic animation offsets
    bob = -1 if (frame_type == "walk" and f_idx % 2 == 1) else (1 if frame_type == "idle" and f_idx == 1 else 0)
    leg_f = -2 if (frame_type == "walk" and f_idx in [0, 1]) else 1 if (frame_type == "walk" and f_idx in [2, 3]) else 0

    # Shadow
    rect(d, 10, 26, 12, 3, C_SHADOW_CAST)

    # 1. Bat Wings (Dark maroon)
    d.polygon([(9, 14 + bob), (5, 9 + bob), (10, 11 + bob)], fill=(120, 25, 40, 255))
    d.polygon([(22, 14 + bob), (26, 9 + bob), (21, 11 + bob)], fill=(120, 25, 40, 255))

    # 2. Pointy Demon Tail
    d.line([(16, 23 + bob), (21, 23 + bob), (23, 20 + bob)], fill=(195, 45, 55, 255))
    pt(d, 24, 19 + bob, (240, 75, 75, 255))

    # 3. Imp Body & Overalls
    rect(d, 12, 16 + bob, 8, 8, (215, 50, 65, 255)) # Crimson skin
    rect(d, 12, 19 + bob, 8, 5, (75, 55, 40, 255))   # Leather overalls
    # Yellow suspender buckles
    pt(d, 13, 19 + bob, (255, 215, 60, 255)); pt(d, 18, 19 + bob, (255, 215, 60, 255))

    # 4. Cute Imp Head
    rect(d, 11, 9 + bob, 10, 8, (215, 50, 65, 255))
    rect(d, 12, 10 + bob, 8, 6, (240, 75, 88, 255))
    # Golden Horns
    pt(d, 11, 7 + bob, (255, 225, 75, 255)); pt(d, 10, 6 + bob, (255, 245, 160, 255))
    pt(d, 20, 7 + bob, (255, 225, 75, 255)); pt(d, 21, 6 + bob, (255, 245, 160, 255))
    # Big expressive eyes
    rect(d, 13, 12 + bob, 2, 3, (25, 15, 20, 255))
    rect(d, 17, 12 + bob, 2, 3, (25, 15, 20, 255))
    pt(d, 13, 12 + bob, (255, 255, 255, 255)); pt(d, 17, 12 + bob, (255, 255, 255, 255)) # Sparkle

    # 5. Hand holding Torch
    if frame_type == "attack":
        # Lunging torch attack
        rect(d, 20, 14 + bob, 6, 2, C_WOOD_MID)
        rect(d, 25, 11 + bob, 4, 4, C_FIRE_ORANGE)
        rect(d, 26, 12 + bob, 2, 2, C_FIRE_YEL)
    else:
        # Carrying torch upright
        rect(d, 20, 13 + bob, 2, 7, C_WOOD_MID)
        rect(d, 19, 10 + bob, 4, 4, C_FIRE_ORANGE)
        pt(d, 20, 11 + bob, C_FIRE_YEL)

    # 6. Legs
    rect(d, 12, 24, 2, 3 + leg_f, (175, 35, 45, 255))
    rect(d, 18, 24, 2, 3 - leg_f, (175, 35, 45, 255))
    return img

def make_monster_slime(frame_type="main", f_idx=0):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    squish = 2 if (frame_type == "walk" and f_idx % 2 == 1) else (1 if frame_type == "idle" and f_idx == 1 else 0)
    w = 16 + squish * 2
    h = 12 - squish
    x = 16 - w // 2
    y = 26 - h

    # Shadow
    rect(d, x - 1, 26, w + 2, 3, C_SHADOW_CAST)

    # Translucent emerald jelly body
    rect(d, x + 2, y, w - 4, h, (45, 195, 145, 255))
    rect(d, x, y + 2, w, h - 2, (35, 170, 125, 255))
    rect(d, x + 2, y + 1, w - 4, h - 3, (65, 225, 170, 255))
    # Top glossy highlight curve
    hline(d, x + 4, x + w - 5, y + 1, (165, 255, 225, 255))

    # Glowing inner soul core (cyan orb inside jelly)
    rect(d, 14, y + 4, 4, 4, C_RUNE_CYAN)
    rect(d, 15, y + 5, 2, 2, C_RUNE_CORE)

    # Cute large eyes
    rect(d, 11, y + 4, 2, 3, (15, 45, 35, 255))
    rect(d, 19, y + 4, 2, 3, (15, 45, 35, 255))
    pt(d, 11, y + 4, (255, 255, 255, 255)); pt(d, 19, y + 4, (255, 255, 255, 255))
    # Rosy cheeks
    pt(d, 10, y + 7, (255, 145, 175, 180)); pt(d, 21, y + 7, (255, 145, 175, 180))

    # Slime bubbles floating up
    pt(d, x + 3, y - 2, (125, 245, 205, 200))
    if frame_type == "attack":
        # Slime splash spikes
        pt(d, x - 2, y + 4, (45, 195, 145, 255))
        pt(d, x + w + 1, y + 4, (45, 195, 145, 255))
    return img

def make_monster_bonepup(frame_type="main", f_idx=0):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    bob = -1 if (frame_type == "walk" and f_idx % 2 == 1) else (1 if frame_type == "idle" and f_idx == 1 else 0)
    tail_w = 2 if f_idx % 2 == 1 else -2

    # Shadow
    rect(d, 8, 26, 16, 3, C_SHADOW_CAST)

    # Bone Skeleton Body
    # Ribcage
    rect(d, 12, 16 + bob, 8, 7, (225, 220, 215, 255))
    rect(d, 13, 17 + bob, 6, 5, (45, 40, 50, 255))
    # White rib rungs
    hline(d, 13, 18, 18 + bob, (245, 240, 235, 255))
    hline(d, 13, 18, 20 + bob, (245, 240, 235, 255))

    # Cute Skull Head
    rect(d, 8, 9 + bob, 10, 8, (245, 240, 235, 255))
    rect(d, 6, 12 + bob, 4, 4, (225, 220, 215, 255)) # Snout
    # Skull eye socket with blue spirit fire! (Reference 3 blue rune light!)
    rect(d, 10, 11 + bob, 3, 3, (25, 20, 35, 255))
    pt(d, 11, 12 + bob, C_RUNE_CORE) # Blazing blue spirit eye
    pt(d, 12, 12 + bob, C_RUNE_CYAN)
    # Pointy skeletal wolf ears
    d.polygon([(11, 9 + bob), (13, 5 + bob), (14, 9 + bob)], fill=(245, 240, 235, 255))
    d.polygon([(15, 9 + bob), (17, 5 + bob), (18, 9 + bob)], fill=(225, 220, 215, 255))

    # Wagging bone tail
    d.line([(20, 17 + bob), (24, 15 + bob + tail_w)], fill=(245, 240, 235, 255))
    pt(d, 25, 14 + bob + tail_w, (255, 255, 255, 255))

    # Bone legs
    rect(d, 10, 23, 2, 4, (245, 240, 235, 255))
    rect(d, 18, 23, 2, 4, (245, 240, 235, 255))
    return img

def make_monster_grimshadow(frame_type="main", f_idx=0):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    float_y = -2 if (f_idx % 2 == 1) else 0

    # Shadow on ground (faint since floating)
    rect(d, 11, 27, 10, 2, (20, 18, 30, 60))

    # Floating dark violet hooded cloak
    rect(d, 10, 9 + float_y, 12, 15, (45, 30, 65, 255))
    rect(d, 11, 10 + float_y, 10, 14, (65, 45, 95, 255))
    # Tattered cloak hem tails
    for tx in [11, 14, 17, 20]:
        vline(d, tx, 24 + float_y, 26 + float_y, (35, 20, 50, 255))

    # Dark hood void
    rect(d, 12, 11 + float_y, 8, 7, (15, 10, 25, 255))
    # Single Glowing Crimson Cyclops Eye!
    rect(d, 14, 13 + float_y, 4, 3, (255, 35, 60, 255))
    pt(d, 15, 14 + float_y, (255, 235, 240, 255))

    # Floating Arcane Wand in left hand
    rect(d, 6, 14 + float_y, 2, 8, C_WOOD_MID)
    # Glowing crystal tip
    pt(d, 6, 12 + float_y, C_MANA_LIT); pt(d, 7, 13 + float_y, C_MANA_PURPLE)

    # Swirling shadow mist particles
    pt(d, 9, 21 + float_y, (120, 65, 185, 180))
    pt(d, 22, 18 + float_y, (120, 65, 185, 180))
    return img

def make_monster_goblin(frame_type="main", f_idx=0):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    bob = -1 if (frame_type == "walk" and f_idx % 2 == 1) else (1 if frame_type == "idle" and f_idx == 1 else 0)

    # Shadow
    rect(d, 9, 26, 14, 3, C_SHADOW_CAST)

    # Heavy Tinker Backpack (Reference 3 & 4 Crate scrap!)
    rect(d, 19, 13 + bob, 6, 9, C_WOOD_DARK)
    rect(d, 20, 14 + bob, 4, 7, C_WOOD_MID)
    # Scrap metal sticking out of backpack
    vline(d, 22, 9 + bob, 13 + bob, (170, 170, 185, 255))

    # Green Goblin Body & Apron
    rect(d, 11, 15 + bob, 9, 8, (95, 165, 65, 255))
    rect(d, 12, 17 + bob, 7, 6, (85, 55, 35, 255)) # Leather apron

    # Goblin Head with Big Pointy Ears
    rect(d, 10, 8 + bob, 10, 8, (95, 165, 65, 255))
    rect(d, 11, 9 + bob, 8, 6, (125, 195, 85, 255))
    # Big horizontal goblin ears
    d.polygon([(10, 11 + bob), (5, 9 + bob), (10, 14 + bob)], fill=(95, 165, 65, 255))
    d.polygon([(20, 11 + bob), (25, 9 + bob), (20, 14 + bob)], fill=(95, 165, 65, 255))

    # Brass Tinker Goggles perched on forehead!
    rect(d, 11, 8 + bob, 4, 3, (225, 165, 45, 255))
    rect(d, 16, 8 + bob, 4, 3, (225, 165, 45, 255))
    pt(d, 12, 9 + bob, (125, 225, 255, 255)); pt(d, 17, 9 + bob, (125, 225, 255, 255))

    # Sharp inquisitive eyes
    rect(d, 12, 12 + bob, 2, 2, (20, 25, 15, 255))
    rect(d, 16, 12 + bob, 2, 2, (20, 25, 15, 255))
    pt(d, 12, 12 + bob, (255, 255, 255, 255)); pt(d, 16, 12 + bob, (255, 255, 255, 255))

    # Large Tinkering Hammer in hand
    if frame_type == "attack":
        # Swinging hammer forward
        rect(d, 4, 10 + bob, 6, 2, C_WOOD_MID)
        rect(d, 2, 7 + bob, 4, 7, (135, 135, 150, 255))
    else:
        # Resting hammer on shoulder
        rect(d, 6, 12 + bob, 2, 10, C_WOOD_MID)
        rect(d, 4, 10 + bob, 6, 4, (135, 135, 150, 255))

    # Legs
    rect(d, 12, 23, 2, 4, (75, 135, 45, 255))
    rect(d, 17, 23, 2, 4, (75, 135, 45, 255))
    return img


# =============================================================================
# 4. ENVIRONMENT PROPS (Trees, Bushes, Stone Walls, Barrels, Torches)
# Directly Benchmarked to Reference 1, 2, 4, 5
# =============================================================================

def make_prop_tree():
    """
    Lush Broadleaf Tree (32x48):
    Warm olive foliage clusters with soft top-left highlights,
    sturdy wooden trunk with roots (Matches Reference 1, 2 & 5 tree).
    """
    img = Image.new("RGBA", (32, 48), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Ground shadow
    rect(d, 8, 42, 16, 5, C_SHADOW_CAST)

    # Trunk & Roots
    rect(d, 13, 26, 6, 17, C_WOOD_DARK)
    rect(d, 14, 27, 4, 15, C_WOOD_MID)
    vline(d, 14, 28, 42, C_WOOD_LIT)
    vline(d, 17, 28, 42, C_WOOD_DEEP)
    # Buttress roots spreading out
    d.polygon([(13, 38), (9, 44), (13, 44)], fill=C_WOOD_DARK)
    d.polygon([(18, 38), (22, 44), (18, 44)], fill=C_WOOD_DARK)

    # Foliage Canopy (Organic overlapping rounded puffs)
    # Main canopy base circles
    foliage_blobs = [
        (4, 10, 24, 20),   # Central main canopy
        (2, 14, 16, 16),   # Left lower puff
        (14, 14, 16, 16),  # Right lower puff
        (6, 4, 20, 16),    # Top crown puff
    ]
    # 1. Base shadow coat
    for (bx, by, bw, bh) in foliage_blobs:
        rect(d, bx, by, bw, bh, C_GRASS_DEEP)
    # 2. Body mid tone
    for (bx, by, bw, bh) in foliage_blobs:
        rect(d, bx + 1, by + 1, bw - 2, bh - 2, C_GRASS_MID)
    # 3. Lit core patches
    for (bx, by, bw, bh) in foliage_blobs:
        rect(d, bx + 2, by + 1, bw - 4, bh - 5, C_GRASS_BASE)
        # Top-left sunlit highlights
        hline(d, bx + 3, bx + bw - 6, by + 2, C_GRASS_LIT)

    # Leaf dapples & highlights (Reference 1 style leaf clusters)
    for (lx, ly) in [(8, 8), (14, 6), (20, 9), (7, 18), (12, 15), (22, 17), (16, 22)]:
        rect(d, lx, ly, 3, 2, C_GRASS_LIT)
        pt(d, lx + 1, ly + 2, C_GRASS_DEEP)

    return img

def make_prop_bush():
    """
    Rounded Garden Shrub / Bush (16x16):
    Soft rounded olive hedge bush (Matches Reference 1 & 2 bush).
    """
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 2, 13, 12, 3, C_SHADOW_CAST)
    # Bush outline & body
    rect(d, 2, 4, 12, 10, C_GRASS_DEEP)
    rect(d, 3, 3, 10, 10, C_GRASS_MID)
    rect(d, 4, 3, 8, 7, C_GRASS_BASE)
    # Highlight crest
    hline(d, 5, 10, 4, C_GRASS_LIT)
    pt(d, 6, 5, C_GRASS_LIT); pt(d, 9, 5, C_GRASS_LIT)
    # Leaf shadow indentations
    pt(d, 4, 8, C_GRASS_DEEP); pt(d, 11, 8, C_GRASS_DEEP); pt(d, 8, 11, C_GRASS_DEEP)
    return img

def make_prop_wall():
    """
    Stone Castle Wall (16x24):
    Sturdy masonry wall with coping stone top (Reference 1 wall).
    """
    img = Image.new("RGBA", (16, 24), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    rect(d, 0, 21, 16, 3, C_SHADOW_CAST)
    # Wall body
    rect(d, 0, 6, 16, 16, C_STONE_DARK)
    rect(d, 1, 7, 14, 14, C_STONE_MID)

    # Masonry courses
    for wy in [11, 16]:
        hline(d, 1, 14, wy, C_STONE_DEEP)
    vline(d, 7, 7, 10, C_STONE_DEEP)
    vline(d, 11, 12, 15, C_STONE_DEEP)
    vline(d, 5, 17, 20, C_STONE_DEEP)

    # Top Coping Stones (Overhanging ledge)
    rect(d, 0, 3, 16, 4, C_STONE_DARK)
    rect(d, 0, 3, 16, 3, C_STONE_LIT)
    hline(d, 0, 15, 3, (215, 212, 185, 255)) # Sun edge
    hline(d, 0, 15, 6, C_STONE_DEEP)         # Under-ledge shadow
    return img

def make_prop_barrel():
    """
    Wooden Storage Barrel (16x16) — Reference 1 & 4
    """
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    rect(d, 3, 13, 10, 3, C_SHADOW_CAST)
    # Barrel barrel shape
    rect(d, 3, 3, 10, 11, C_WOOD_DARK)
    rect(d, 4, 3, 8, 11, C_WOOD_MID)
    # Iron Hoops
    hline(d, 3, 12, 4, (75, 70, 85, 255))
    hline(d, 3, 12, 8, (75, 70, 85, 255))
    hline(d, 3, 12, 12, (75, 70, 85, 255))
    # Bevel highlight
    vline(d, 5, 4, 12, C_WOOD_LIT)
    return img

def make_prop_chest():
    """
    Wooden Treasure Chest (16x16) — Reference 1 & 3
    """
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    rect(d, 2, 13, 12, 3, C_SHADOW_CAST)
    # Chest body
    rect(d, 2, 5, 12, 9, C_WOOD_DARK)
    rect(d, 3, 6, 10, 7, C_WOOD_MID)
    # Lid curved top
    hline(d, 3, 12, 5, C_WOOD_LIT)
    # Iron bands & lock
    vline(d, 4, 5, 13, (75, 70, 85, 255))
    vline(d, 11, 5, 13, (75, 70, 85, 255))
    rect(d, 7, 8, 2, 3, (245, 205, 65, 255)) # Brass clasp
    return img

def make_prop_torch():
    """
    Flaming Torch Stand (16x24) — Reference 4
    """
    img = Image.new("RGBA", (16, 24), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    rect(d, 5, 21, 6, 3, C_SHADOW_CAST)
    # Wooden post & iron bracket
    vline(d, 8, 9, 21, C_WOOD_DARK)
    vline(d, 7, 9, 21, C_WOOD_LIT)
    rect(d, 6, 21, 4, 2, (75, 70, 85, 255)) # Base
    # Torch cup
    rect(d, 6, 7, 4, 3, (65, 60, 75, 255))
    # Roaring flame
    rect(d, 6, 3, 4, 5, C_FIRE_ORANGE)
    rect(d, 7, 2, 2, 4, C_FIRE_YEL)
    pt(d, 7, 1, (255, 255, 200, 255))
    return img

# =============================================================================
# 5. MASTER GENERATION SCRIPT
# =============================================================================

def generate_all():
    print("=== Generating Master-Grade RUMORS Pixel Art Assets ===")

    # 1. Tiles
    tiles = {
        "Tile_Ground_DarkGrass.png": make_tile_dark_grass(),
        "Tile_Cobblestone.png":      make_tile_cobblestone(),
        "Tile_Castle_Floor.png":     make_tile_castle_floor(),
        "Tile_Ground_Dirt.png":      make_tile_dirt(),
        "Tile_Ground_Stone.png":     make_tile_stone(),
        "Tile_Ground_Swamp.png":     make_tile_swamp(),
        "Tile_Highlight.png":        make_tile_highlight(),
    }
    for name, img in tiles.items():
        p = os.path.join(TILES_DIR, name)
        img.save(p)
        print(f" [Tile] {name}")

    # 2. Buildings
    buildings = {
        "Building_Castle.png":            make_building_castle(level=1),
        "Building_Castle_Upgraded.png":   make_building_castle(level=2),
        "Building_Mine.png":              make_building_mine(level=1),
        "Building_Mine_Upgraded.png":     make_building_mine(level=2),
        "Building_Farm.png":              make_building_farm(level=1),
        "Building_Farm_Upgraded.png":     make_building_farm(level=2),
        "Building_Dormitory.png":         make_building_dormitory(level=1),
        "Building_Dormitory_Upgraded.png":make_building_dormitory(level=2),
        "Building_Warehouse.png":         make_building_warehouse(level=1),
        "Building_Warehouse_Upgraded.png":make_building_warehouse(level=2),
        "Building_ManaTower.png":         make_building_mana_tower(),
        "Building_Training.png":          make_building_training(),
        "Building_Construction.png":      make_building_construction(),
    }
    for name, img in buildings.items():
        p = os.path.join(BUILDINGS_DIR, name)
        img.save(p)
        print(f" [Building] {name}")

    # 3. Environment Props (NEW)
    props = {
        "Prop_Tree.png":   make_prop_tree(),
        "Prop_Bush.png":   make_prop_bush(),
        "Prop_Wall.png":   make_prop_wall(),
        "Prop_Barrel.png": make_prop_barrel(),
        "Prop_Chest.png":  make_prop_chest(),
        "Prop_Torch.png":  make_prop_torch(),
    }
    for name, img in props.items():
        p = os.path.join(PROPS_DIR, name)
        img.save(p)
        print(f" [Prop] {name}")

    # 4. Monsters + 9-Frame Animation Sequences
    monster_defs = {
        "ImpScrap":     make_monster_imp,
        "MireSlime":    make_monster_slime,
        "BonePup":      make_monster_bonepup,
        "GrimShadow":   make_monster_grimshadow,
        "GoblinTinker": make_monster_goblin,
    }

    for name, gen_fn in monster_defs.items():
        # Single main sprite
        main_img = gen_fn("main", 0)
        main_img.save(os.path.join(MONSTERS_DIR, f"Monster_{name}.png"))

        # Folder for sequences
        seq_dir = os.path.join(MONSTERS_DIR, f"Monster_{name}")
        os.makedirs(seq_dir, exist_ok=True)

        # Idle (2f)
        for i in range(2):
            gen_fn("idle", i).save(os.path.join(seq_dir, f"Idle_{i}.png"))
        # Walk (4f)
        for i in range(4):
            gen_fn("walk", i).save(os.path.join(seq_dir, f"Walk_{i}.png"))
        # Attack/Work (3f)
        for i in range(3):
            gen_fn("attack", i).save(os.path.join(seq_dir, f"Attack_{i}.png"))

        print(f" [Monster] Monster_{name} (Main + 9 Frames)")

    print("=== Complete! All assets successfully generated! ===")

if __name__ == "__main__":
    generate_all()
