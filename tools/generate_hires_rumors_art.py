"""
RUMORS — High-Quality Japanese Dark Fantasy Pixel Art Asset Generator
Generates readable, rich-contrast, authentic 16-bit pixel art for:
1. Ground & Special Tiles (16x16)
2. Buildings (32x32, Castle 48x48)
3. Monsters (32x32) with Idle/Walk/Attack frame sequences
4. Gothic UI Elements (9-Slice frames, buttons, icons)
"""

from PIL import Image, ImageDraw
import os, math

ROOT = r"C:\Users\lookm\Documents\GitHub\Rumors_Crown\Assets\Resources"
TILES_DIR     = os.path.join(ROOT, "Tiles")
BUILDINGS_DIR = os.path.join(ROOT, "Buildings")
MONSTERS_DIR  = os.path.join(ROOT, "Monsters")
UI_DIR        = os.path.join(ROOT, "UI")

for d in [TILES_DIR, BUILDINGS_DIR, MONSTERS_DIR, UI_DIR]:
    os.makedirs(d, exist_ok=True)

# Helper functions
def pt(d, x, y, color):
    d.point((x, y), fill=color)

def rect(d, x, y, w, h, color):
    d.rectangle([x, y, x + w - 1, y + h - 1], fill=color)

def outline_rect(d, x, y, w, h, fill_color, border_color):
    d.rectangle([x, y, x + w - 1, y + h - 1], fill=fill_color, outline=border_color)

# =============================================================================
# 1. TILES (16x16) — Seamless, rich dark fantasy colors, no black grid borders
# =============================================================================

def make_tile_dark_grass():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    # Rich dark fantasy forest meadow (readable luminance ~45-55)
    base = (42, 68, 46, 255)
    dark = (32, 52, 36, 255)
    mid  = (56, 92, 60, 255)
    hi   = (84, 136, 88, 255)
    flower = (180, 110, 210, 255) # mystical tiny purple night-bloom

    rect(d, 0, 0, 16, 16, base)
    # Natural organic variations (dithering without harsh grid)
    for (x, y) in [(1,2),(5,1),(10,3),(14,2),(3,8),(8,6),(12,9),(2,13),(7,12),(13,14)]:
        pt(d, x, y, mid)
        if y > 0: pt(d, x, y-1, hi)
    for (x, y) in [(0,5),(6,9),(11,1),(4,14),(9,15),(15,7)]:
        pt(d, x, y, dark)
    # 2 tiny mystic flowers
    pt(d, 3, 5, flower); pt(d, 12, 11, flower)
    return img

def make_tile_dirt():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    base = (98, 70, 50, 255)
    dark = (72, 48, 34, 255)
    mid  = (124, 90, 64, 255)
    hi   = (154, 116, 84, 255)
    rect(d, 0, 0, 16, 16, base)
    # Path pebbles and dirt texture
    pebbles = [(2,3),(8,2),(13,5),(4,8),(11,9),(3,13),(9,12),(14,14)]
    for (x, y) in pebbles:
        pt(d, x, y, hi)
        if y < 15: pt(d, x, y+1, dark)
    for (x, y) in [(0,7),(6,4),(10,13),(15,2),(7,15)]:
        pt(d, x, y, mid)
    return img

def make_tile_castle_floor():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    # Polished gothic obsidian flagstones with gold/purple veins
    mortar = (36, 30, 48, 255)
    tile_a = (66, 58, 82, 255)
    tile_b = (76, 68, 94, 255)
    hi     = (102, 92, 124, 255)
    rune   = (158, 112, 212, 255)

    rect(d, 0, 0, 16, 16, mortar)
    # 4 distinct flagstones
    rect(d, 1, 1, 6, 6, tile_a)
    rect(d, 8, 1, 7, 6, tile_b)
    rect(d, 1, 8, 7, 7, tile_b)
    rect(d, 9, 8, 6, 7, tile_a)

    # Flagstone highlights
    for x in range(1, 7): pt(d, x, 1, hi)
    for x in range(8, 15): pt(d, x, 1, hi)
    for x in range(1, 8): pt(d, x, 8, hi)
    for x in range(9, 15): pt(d, x, 8, hi)

    # Tiny carved arcane glyph in center
    pt(d, 7, 7, rune); pt(d, 8, 7, rune)
    return img

def make_tile_cobblestone():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    mortar = (40, 36, 52, 255)
    stone_base = (84, 78, 98, 255)
    stone_hi   = (118, 110, 138, 255)
    stone_lit  = (144, 136, 166, 255)

    rect(d, 0, 0, 16, 16, mortar)
    # Irregular cobblestones
    stones = [
        (1, 1, 6, 4), (8, 1, 7, 4),
        (0, 6, 4, 4), (5, 6, 6, 4), (12, 6, 4, 4),
        (1, 11, 7, 4), (9, 11, 6, 4)
    ]
    for (x, y, w, h) in stones:
        rect(d, x, y, w, h, stone_base)
        for i in range(w):
            pt(d, x + i, y, stone_hi)
        pt(d, x + 1, y, stone_lit)
    return img

def make_tile_stone():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    base = (78, 76, 90, 255)
    dark = (52, 50, 62, 255)
    mid  = (102, 100, 116, 255)
    hi   = (136, 134, 152, 255)
    ore  = (182, 118, 244, 255) # amethyst mana crystal vein
    ore_hi = (226, 184, 255, 255)

    rect(d, 0, 0, 16, 16, base)
    # Crag lines
    for (x, y) in [(2,2),(3,3),(4,3),(5,4),(10,7),(11,8),(12,8),(13,9)]:
        pt(d, x, y, dark)
        pt(d, x, y-1, hi)
    # Sparkling crystal vein running diagonally
    for (x, y) in [(11,2),(10,3),(9,4),(8,5),(7,6),(6,7)]:
        pt(d, x, y, ore)
    pt(d, 9, 3, ore_hi); pt(d, 7, 5, ore_hi)
    return img

def make_tile_swamp():
    img = Image.new("RGBA", (16, 16))
    d = ImageDraw.Draw(img)
    base  = (48, 66, 56, 255)
    water = (32, 50, 48, 255)
    moss  = (68, 102, 78, 255)
    glow  = (86, 214, 158, 255) # toxic swamp bioluminescence

    rect(d, 0, 0, 16, 16, base)
    rect(d, 3, 4, 10, 8, water)
    rect(d, 5, 6, 6, 4, (24, 40, 38, 255))
    # Moss clumps
    for (x, y) in [(1,1),(2,2),(13,2),(14,1),(2,13),(12,13),(13,14)]:
        pt(d, x, y, moss)
    # Glowing swamp fireflies / bubbles
    pt(d, 6, 7, glow); pt(d, 10, 8, glow); pt(d, 4, 11, glow)
    return img

def make_tile_highlight():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    # Crisp glowing gold corner brackets with faint inner fill
    gold = (255, 218, 68, 240)
    gold_hi = (255, 245, 160, 255)
    inner = (255, 218, 68, 45)

    rect(d, 2, 2, 12, 12, inner)
    # 4 corner brackets (3x3 each)
    # Top-Left
    pt(d, 0, 0, gold_hi); pt(d, 1, 0, gold); pt(d, 2, 0, gold)
    pt(d, 0, 1, gold); pt(d, 0, 2, gold)
    # Top-Right
    pt(d, 15, 0, gold_hi); pt(d, 14, 0, gold); pt(d, 13, 0, gold)
    pt(d, 15, 1, gold); pt(d, 15, 2, gold)
    # Bottom-Left
    pt(d, 0, 15, gold_hi); pt(d, 1, 15, gold); pt(d, 2, 15, gold)
    pt(d, 0, 14, gold); pt(d, 0, 13, gold)
    # Bottom-Right
    pt(d, 15, 15, gold_hi); pt(d, 14, 15, gold); pt(d, 13, 15, gold)
    pt(d, 15, 14, gold); pt(d, 15, 13, gold)
    return img

# =============================================================================
# 2. BUILDINGS (32x32, Castle 48x48) — Rich silhouettes, gothic detailing
# =============================================================================

def make_building_castle():
    size = 48
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Obsidian stone palette
    stone_dark = (28, 24, 38, 255)
    stone_mid  = (50, 44, 66, 255)
    stone_hi   = (84, 76, 110, 255)
    roof_dark  = (38, 16, 52, 255)
    roof_mid   = (72, 32, 98, 255)
    roof_hi    = (112, 54, 152, 255)
    gold       = (230, 185, 60, 255)
    crimson    = (185, 34, 48, 255)
    crimson_hi = (225, 65, 75, 255)
    fire_core  = (255, 230, 120, 255)
    fire_glow  = (185, 80, 230, 255)

    # 1. Base castle courtyard walls
    rect(d, 6, 26, 36, 20, stone_dark)
    rect(d, 8, 28, 32, 18, stone_mid)
    # Flagstone lines on base
    for y in [34, 40]:
        d.line([(8, y), (39, y)], fill=stone_dark)

    # 2. Left & Right watchtowers
    for tx in [4, 34]:
        rect(d, tx, 18, 10, 28, stone_dark)
        rect(d, tx + 1, 20, 8, 26, stone_mid)
        # Battlements on tower top
        rect(d, tx, 15, 3, 4, stone_hi)
        rect(d, tx + 7, 15, 3, 4, stone_hi)
        rect(d, tx + 2, 17, 6, 2, stone_dark)
        # Tower arrow slit with glow
        rect(d, tx + 4, 25, 2, 5, (15, 12, 22, 255))
        pt(d, tx + 4, 27, fire_glow)
        # Brazier of demonic purple flame on towers
        rect(d, tx + 3, 14, 4, 2, (18, 16, 24, 255))
        pt(d, tx + 4, 12, fire_glow); pt(d, tx + 5, 12, fire_glow)
        pt(d, tx + 4, 13, fire_core); pt(d, tx + 5, 13, fire_core)

    # 3. Central Great Tower (Dominating gothic keep)
    rect(d, 16, 8, 16, 30, stone_dark)
    rect(d, 18, 10, 12, 28, stone_mid)
    # Vertical stone pillars
    d.line([(18, 10), (18, 38)], fill=stone_hi)
    d.line([(29, 10), (29, 38)], fill=stone_dark)

    # 4. Spire roof
    for row in range(7):
        w = max(2, 14 - row * 2)
        rect(d, 24 - w // 2, 8 - row, w, 1, roof_mid)
        pt(d, 24 - w // 2, 8 - row, roof_hi)
    # Demon King Crown ornament at top
    rect(d, 23, 0, 3, 2, gold)
    pt(d, 24, -1, fire_core)

    # 5. Glowing stained-glass rose window
    rect(d, 22, 14, 5, 7, (20, 10, 30, 255))
    rect(d, 23, 15, 3, 5, fire_glow)
    pt(d, 24, 17, fire_core)

    # 6. Castle Gate (Reinforced iron arch with spikes)
    rect(d, 20, 34, 9, 13, (12, 10, 18, 255))
    # Iron portcullis bars
    for bx in [22, 24, 26]:
        d.line([(bx, 34), (bx, 46)], fill=(80, 85, 95, 255))
    # Red royal banners flanking the gate
    for bx in [16, 31]:
        rect(d, bx, 26, 3, 10, crimson)
        rect(d, bx, 35, 3, 1, crimson_hi)
        pt(d, bx + 1, 28, gold)

    return img

def make_building_mine():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    rock_dark = (36, 34, 44, 255)
    rock_mid  = (62, 58, 72, 255)
    rock_hi   = (96, 92, 112, 255)
    timber    = (92, 60, 40, 255)
    timber_hi = (134, 92, 62, 255)
    ore_gold  = (240, 195, 50, 255)
    ore_gem   = (175, 110, 240, 255)
    lantern   = (255, 210, 80, 255)

    # Craggy mountain rock mound
    rect(d, 3, 8, 26, 22, rock_dark)
    rect(d, 5, 10, 22, 20, rock_mid)
    for x in range(6, 26, 3):
        pt(d, x, 9, rock_hi)

    # Mine tunnel entrance (dark opening)
    rect(d, 9, 14, 14, 16, (14, 12, 18, 255))

    # Timber beam support frame
    rect(d, 8, 12, 3, 18, timber); d.line([(8, 12), (8, 29)], fill=timber_hi)
    rect(d, 21, 12, 3, 18, timber); d.line([(21, 12), (21, 29)], fill=timber_hi)
    rect(d, 8, 12, 16, 3, timber); d.line([(8, 12), (23, 12)], fill=timber_hi)

    # Wooden minecart with tracks
    d.line([(6, 28), (26, 28)], fill=(60, 65, 75, 255)) # Rail
    # Cart
    rect(d, 12, 22, 9, 6, timber)
    # Wheels
    pt(d, 13, 28, (40, 40, 45, 255)); pt(d, 19, 28, (40, 40, 45, 255))
    # Glowing ore heap inside cart
    pt(d, 14, 21, ore_gold); pt(d, 15, 20, ore_gold); pt(d, 16, 21, ore_gem)
    pt(d, 17, 20, ore_gem); pt(d, 18, 21, ore_gold)

    # Hanging oil lantern
    pt(d, 10, 15, (60, 60, 60, 255))
    rect(d, 9, 16, 3, 3, lantern)
    return img

def make_building_farm():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    wood = (84, 56, 36, 255)
    wood_hi = (120, 84, 54, 255)
    soil = (60, 42, 30, 255)
    shroom_stem = (195, 180, 160, 255)
    shroom_purple = (185, 60, 175, 255)
    shroom_hi = (235, 115, 225, 255)
    spore_glow = (90, 230, 180, 255)

    # Wooden fence enclosure
    rect(d, 2, 8, 28, 22, wood)
    rect(d, 4, 10, 24, 18, soil)
    # Fence posts
    for x in [2, 10, 20, 28]:
        rect(d, x, 6, 2, 5, wood_hi)

    # Giant Mother Mushroom (center)
    # Stem
    rect(d, 14, 16, 4, 10, shroom_stem)
    # Cap (rounded umbrella dome)
    rect(d, 10, 12, 12, 6, shroom_purple)
    rect(d, 12, 10, 8, 3, shroom_purple)
    d.line([(10, 12), (21, 12)], fill=shroom_hi)
    # White bioluminescent spots
    pt(d, 13, 11, (255, 255, 255, 255))
    pt(d, 18, 12, (255, 255, 255, 255))
    pt(d, 15, 14, (255, 255, 255, 255))

    # Baby mushrooms (flanking)
    # Left baby (cyan glowing)
    rect(d, 6, 22, 2, 4, shroom_stem)
    rect(d, 5, 20, 4, 2, (50, 180, 160, 255))
    pt(d, 6, 19, (120, 240, 210, 255))
    # Right baby (orange glowing)
    rect(d, 24, 21, 2, 5, shroom_stem)
    rect(d, 23, 19, 4, 2, (220, 110, 45, 255))
    pt(d, 24, 18, (255, 180, 80, 255))

    # Spore mist particles
    pt(d, 8, 13, spore_glow); pt(d, 22, 14, spore_glow); pt(d, 15, 7, spore_glow)
    return img

def make_building_dormitory():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    wall = (75, 54, 38, 255)
    wall_hi = (112, 82, 58, 255)
    roof = (42, 36, 54, 255)
    roof_hi = (74, 66, 92, 255)
    stone = (80, 78, 88, 255)
    warm_light = (255, 205, 75, 255)
    smoke = (160, 155, 175, 180)

    # Chimney with soft pixel smoke
    rect(d, 22, 4, 4, 10, stone)
    pt(d, 23, 2, smoke); pt(d, 24, 1, smoke); pt(d, 25, 0, smoke)

    # Main cabin wall
    rect(d, 4, 16, 24, 14, wall)
    for y in range(18, 29, 3):
        d.line([(4, y), (27, y)], fill=wall_hi)

    # Slanted shingle roof
    for r in range(8):
        w = max(2, 28 - r * 2)
        rect(d, 16 - w // 2, 16 - r, w, 1, roof)
        pt(d, 16 - w // 2, 16 - r, roof_hi)

    # Warm glowing window
    rect(d, 7, 19, 5, 5, (20, 16, 24, 255))
    rect(d, 8, 20, 3, 3, warm_light)
    # Window wooden mullion
    pt(d, 9, 21, (60, 40, 25, 255))

    # Wooden door with iron hinges
    rect(d, 17, 20, 6, 10, (48, 30, 20, 255))
    pt(d, 18, 23, (140, 145, 155, 255)) # Iron handle
    return img

def make_building_warehouse():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    stone_wall = (64, 60, 74, 255)
    timber = (86, 58, 40, 255)
    timber_hi = (126, 88, 62, 255)
    metal = (140, 145, 158, 255)
    gold = (230, 180, 50, 255)

    # Sturdy stone warehouse base
    rect(d, 3, 14, 26, 16, stone_wall)
    # Timber crossbeams
    rect(d, 3, 14, 26, 2, timber)
    d.line([(3, 14), (28, 14)], fill=timber_hi)
    rect(d, 3, 14, 2, 16, timber)
    rect(d, 27, 14, 2, 16, timber)

    # Roof
    for r in range(6):
        w = max(2, 28 - r * 3)
        rect(d, 16 - w // 2, 14 - r, w, 1, timber)
        pt(d, 16 - w // 2, 14 - r, timber_hi)

    # Hoist pulley crane from gable
    rect(d, 15, 6, 2, 4, metal)
    d.line([(16, 10), (16, 14)], fill=(180, 160, 130, 255)) # rope

    # Heavy iron-bound double doors
    rect(d, 8, 18, 10, 12, timber)
    rect(d, 8, 21, 10, 1, metal) # top hinge
    rect(d, 8, 27, 10, 1, metal) # bottom hinge
    pt(d, 13, 24, gold) # brass lock

    # Barrels & crates stacked outside
    rect(d, 20, 22, 6, 7, (110, 75, 50, 255))
    rect(d, 21, 23, 4, 5, (90, 60, 38, 255))
    rect(d, 20, 24, 6, 1, metal) # barrel hoop
    return img

def make_building_mana_tower():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    obsidian = (26, 22, 34, 255)
    obsidian_hi = (75, 65, 95, 255)
    rune_gold = (245, 190, 60, 255)
    crystal_dark = (120, 45, 180, 255)
    crystal_core = (235, 160, 255, 255)
    lightning = (180, 230, 255, 255)

    # Tower base
    rect(d, 8, 24, 16, 7, obsidian)
    d.line([(8, 24), (23, 24)], fill=obsidian_hi)
    # Tapered tower shaft
    for y in range(12, 24):
        w = 12 - (24 - y) // 3
        rect(d, 16 - w // 2, y, w, 1, obsidian)
        pt(d, 16 - w // 2, y, obsidian_hi)
        # Golden arcane spiral runes
        if (y + x_offset(y)) % 4 == 0:
            pt(d, 16, y, rune_gold)

    # 4 Crystal prongs at tower top holding the sphere
    rect(d, 10, 9, 2, 4, obsidian_hi)
    rect(d, 20, 9, 2, 4, obsidian_hi)

    # Levitating pulsing mana sphere (at top)
    rect(d, 13, 3, 6, 6, crystal_dark)
    rect(d, 14, 4, 4, 4, crystal_core)
    pt(d, 15, 5, (255, 255, 255, 255)) # sparkle shine
    # Electric lightning sparks
    pt(d, 12, 4, lightning); pt(d, 19, 5, lightning); pt(d, 16, 1, lightning)
    return img

def x_offset(y):
    return (y * 2) % 3

def make_building_training():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    dirt = (92, 66, 46, 255)
    fence = (110, 78, 52, 255)
    straw = (210, 180, 90, 255)
    straw_hi = (245, 215, 120, 255)
    steel = (165, 170, 185, 255)
    crimson = (185, 40, 50, 255)

    # Dirt arena floor
    rect(d, 2, 10, 28, 20, dirt)
    # Wooden barrier fence at back
    for x in range(2, 30, 4):
        rect(d, x, 7, 2, 6, fence)
    d.line([(2, 9), (29, 9)], fill=(135, 100, 68, 255))

    # Straw training dummy (Center)
    # Stand post
    rect(d, 15, 18, 2, 10, (60, 40, 25, 255))
    # Straw torso
    rect(d, 13, 14, 6, 8, straw)
    # Straw head
    rect(d, 14, 10, 4, 4, straw_hi)
    # Red bullseye target on chest
    rect(d, 15, 16, 2, 2, crimson)

    # Weapon rack on the left (wooden rack with iron swords)
    rect(d, 4, 15, 2, 12, fence)
    rect(d, 8, 15, 2, 12, fence)
    d.line([(4, 17), (9, 17)], fill=fence)
    d.line([(4, 22), (9, 22)], fill=fence)
    # Slanted swords
    d.line([(5, 15), (7, 25)], fill=steel)
    d.line([(7, 15), (9, 25)], fill=steel)

    # Archery target bale on the right
    rect(d, 23, 17, 6, 8, straw)
    rect(d, 24, 19, 4, 4, crimson)
    pt(d, 25, 20, (255, 255, 255, 255)) # center bullseye
    return img

def make_building_construction():
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    scaffold = (145, 105, 65, 255)
    rope = (195, 170, 120, 255)
    flag = (235, 60, 60, 255)

    # Half-built stone foundation
    rect(d, 4, 20, 24, 10, (70, 68, 78, 255))
    for x in [4, 12, 20, 26]:
        rect(d, x, 17, 4, 4, (85, 82, 95, 255))

    # Wooden scaffolding structure
    for x in [6, 14, 22]:
        rect(d, x, 4, 2, 20, scaffold)
    # Horizontal boards
    rect(d, 5, 8, 20, 2, scaffold)
    rect(d, 5, 14, 20, 2, scaffold)
    # Diagonal braces
    d.line([(6, 8), (14, 14)], fill=scaffold)
    d.line([(14, 8), (22, 14)], fill=scaffold)

    # Construction warning flag on top
    d.line([(22, 1), (22, 5)], fill=(40, 40, 40, 255))
    rect(d, 23, 1, 4, 3, flag)
    # Mortar bucket
    rect(d, 8, 12, 3, 2, (100, 100, 105, 255))
    return img

# =============================================================================
# 3. MONSTERS (32x32) — Distinct high-readability silhouettes & animations
# =============================================================================

def make_monster_impscrap(frame="idle_0"):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    body = (215, 65, 50, 255)
    body_hi = (255, 115, 90, 255)
    dark = (145, 38, 30, 255)
    horn = (245, 195, 55, 255)
    eye = (255, 240, 90, 255)
    wing = (165, 45, 65, 255)
    bag = (115, 80, 45, 255)

    y_off = 1 if "1" in frame or "walk_1" in frame or "walk_3" in frame else 0
    atk_off = 4 if "attack" in frame else 0

    # Bat wings (spread on back)
    rect(d, 8, 11 - y_off, 4, 5, wing)
    rect(d, 20, 11 - y_off, 4, 5, wing)

    # Horns (sharp curved gold)
    pt(d, 12, 7 - y_off, horn); pt(d, 11, 6 - y_off, horn); pt(d, 10, 5 - y_off, horn)
    pt(d, 19, 7 - y_off, horn); pt(d, 20, 6 - y_off, horn); pt(d, 21, 5 - y_off, horn)

    # Imp Chubby Body & Head
    rect(d, 12, 9 - y_off, 8, 7, body) # Head
    rect(d, 13, 10 - y_off, 6, 2, body_hi)
    rect(d, 11, 16 - y_off, 10, 9, body) # Belly
    rect(d, 13, 18 - y_off, 6, 6, body_hi)

    # Glowing yellow slit eyes & cute fangs
    pt(d, 14, 12 - y_off, eye); pt(d, 17, 12 - y_off, eye)
    pt(d, 14, 14 - y_off, (255, 255, 255, 255)); pt(d, 17, 14 - y_off, (255, 255, 255, 255))

    # Scrap metal shoulder bag
    d.line([(12, 16 - y_off), (18, 22 - y_off)], fill=(50, 35, 20, 255))
    rect(d, 18 + atk_off, 20 - y_off, 5, 5, bag)
    pt(d, 20 + atk_off, 22 - y_off, (220, 190, 80, 255))

    # Feet
    if "walk_1" in frame:
        rect(d, 12, 25, 3, 2, dark); rect(d, 17, 24, 3, 2, dark)
    elif "walk_3" in frame:
        rect(d, 12, 24, 3, 2, dark); rect(d, 17, 25, 3, 2, dark)
    else:
        rect(d, 12, 25, 3, 2, dark); rect(d, 17, 25, 3, 2, dark)

    return img

def make_monster_mireslime(frame="idle_0"):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    base = (45, 190, 110, 220)
    dark = (25, 120, 70, 240)
    hi   = (140, 245, 175, 255)
    shine = (220, 255, 235, 255)

    squish = 1 if "1" in frame or "walk" in frame else 0
    # Jellied dome body
    w = 16 + squish * 2
    h = 12 - squish
    x = 16 - w // 2
    y = 26 - h

    rect(d, x + 2, y, w - 4, h, base)
    rect(d, x, y + 3, w, h - 3, base)
    rect(d, x + 1, y + h - 2, w - 2, 2, dark)

    # Highlight bubble shine
    d.line([(x + 3, y + 2), (x + 7, y + 2)], fill=hi)
    pt(d, x + 4, y + 3, shine)

    # Two big cute glossy eyes
    pt(d, x + 5, y + 5, (15, 50, 30, 255)); pt(d, x + 9, y + 5, (15, 50, 30, 255))
    pt(d, x + 5, y + 4, (255, 255, 255, 255)); pt(d, x + 9, y + 4, (255, 255, 255, 255))

    # Tiny skull floating inside translucent jelly!
    pt(d, x + 12, y + 6, (220, 220, 205, 200))
    pt(d, x + 13, y + 6, (220, 220, 205, 200))
    return img

def make_monster_bonepup(frame="idle_0"):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    bone = (235, 228, 210, 255)
    dark = (165, 155, 138, 255)
    cyan_eye = (70, 225, 255, 255)

    y_off = 1 if "1" in frame else 0
    # Skull Head
    rect(d, 15, 11 - y_off, 9, 7, bone)
    rect(d, 18, 14 - y_off, 7, 4, bone) # Snout
    # Jaw
    rect(d, 19, 17 - y_off, 5, 2, dark)
    # Cyan burning soul eye
    pt(d, 18, 13 - y_off, cyan_eye); pt(d, 19, 13 - y_off, (255, 255, 255, 255))

    # Ribcage spine body
    rect(d, 10, 16 - y_off, 6, 6, dark)
    for ry in [17, 19, 21]:
        d.line([(10, ry - y_off), (15, ry - y_off)], fill=bone)

    # Wagging bone tail
    tail_y = 14 if "1" in frame else 12
    pt(d, 8, tail_y, bone); pt(d, 7, tail_y - 1, bone); pt(d, 6, tail_y - 2, cyan_eye)

    # 4 little skeleton legs
    rect(d, 10, 22, 2, 5, bone)
    rect(d, 15, 22, 2, 5, bone)
    rect(d, 20, 22, 2, 5, bone)
    return img

def make_monster_grimshadow(frame="idle_0"):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    robe_dark = (32, 18, 48, 255)
    robe_mid  = (68, 38, 98, 255)
    robe_hi   = (125, 75, 175, 255)
    eye_red   = (255, 50, 70, 255)
    wisp_cyan = (90, 220, 255, 200)

    y_off = 1 if "1" in frame else 0
    # Floating spectral hood & robe
    rect(d, 12, 8 - y_off, 8, 8, robe_dark)
    d.line([(12, 8 - y_off), (19, 8 - y_off)], fill=robe_hi)

    # Flowing spectral robe bottom (drifting smoke)
    rect(d, 10, 15 - y_off, 12, 10, robe_mid)
    for x in range(10, 22, 2):
        pt(d, x, 25 - y_off + (x % 3), robe_dark)
        pt(d, x, 27 - y_off, (60, 30, 85, 100))

    # Malevolent glowing red eye in dark hood
    rect(d, 15, 12 - y_off, 3, 2, eye_red)
    pt(d, 16, 12 - y_off, (255, 255, 255, 255))

    # Floating ghostly soul orb
    pt(d, 7, 16 - y_off, wisp_cyan); pt(d, 6, 17 - y_off, wisp_cyan)
    return img

def make_monster_goblintinker(frame="idle_0"):
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    skin = (125, 175, 60, 255)
    skin_dark = (85, 120, 40, 255)
    goggle_gold = (235, 195, 45, 255)
    goggle_lens = (80, 210, 240, 255)
    leather = (110, 70, 40, 255)
    wrench = (165, 170, 185, 255)

    y_off = 1 if "1" in frame else 0
    # Big pointy goblin ears
    d.line([(8, 12 - y_off), (11, 14 - y_off)], fill=skin)
    d.line([(23, 12 - y_off), (20, 14 - y_off)], fill=skin)

    # Head
    rect(d, 11, 10 - y_off, 10, 8, skin)
    # Brass Tinker Goggles on forehead
    rect(d, 12, 9 - y_off, 3, 3, goggle_gold); pt(d, 13, 10 - y_off, goggle_lens)
    rect(d, 17, 9 - y_off, 3, 3, goggle_gold); pt(d, 18, 10 - y_off, goggle_lens)

    # Eyes & grin
    pt(d, 13, 14 - y_off, (30, 40, 20, 255)); pt(d, 18, 14 - y_off, (30, 40, 20, 255))
    d.line([(14, 16 - y_off), (17, 16 - y_off)], fill=(40, 20, 20, 255))

    # Leather apron body
    rect(d, 12, 18 - y_off, 8, 7, leather)
    rect(d, 13, 19 - y_off, 6, 5, (140, 95, 55, 255))

    # Giant mechanical cogwrench on back
    d.line([(19, 11), (25, 23)], fill=wrench)
    rect(d, 23, 9, 4, 4, (120, 125, 140, 255))
    pt(d, 24, 10, (230, 200, 50, 255)) # brass gear bolt

    # Boots
    rect(d, 12, 25, 3, 2, (50, 35, 25, 255))
    rect(d, 17, 25, 3, 2, (50, 35, 25, 255))
    return img

# =============================================================================
# 4. GOTHIC UI ELEMENTS (9-Slice panel, buttons, icons)
# =============================================================================

def make_ui_panel_9slice():
    # 24x24 authentic gothic dark fantasy slate panel with brass rivets & gold filigree
    size = 24
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Slate core
    core = (22, 20, 32, 250)
    border_dark = (14, 12, 20, 255)
    metal_dark  = (65, 52, 75, 255)
    gold_edge   = (175, 140, 65, 255)
    gold_hi     = (240, 205, 95, 255)

    # Background
    rect(d, 2, 2, 20, 20, core)
    # Dark outer bevel
    outline_rect(d, 0, 0, 24, 24, (0,0,0,0), border_dark)
    outline_rect(d, 1, 1, 22, 22, (0,0,0,0), metal_dark)
    outline_rect(d, 2, 2, 20, 20, (0,0,0,0), gold_edge)

    # 4 Ornate corner brackets
    for (cx, cy) in [(2,2), (19,2), (2,19), (19,19)]:
        rect(d, cx, cy, 3, 3, gold_hi)
        pt(d, cx + 1, cy + 1, (255, 240, 150, 255)) # glowing rivet
    return img

def make_ui_icon_gold():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    gold_dark = (165, 115, 25, 255)
    gold_mid  = (235, 185, 45, 255)
    gold_hi   = (255, 235, 120, 255)
    # Stack of 3 coins
    for y in [10, 7, 4]:
        rect(d, 3, y, 10, 4, gold_dark)
        rect(d, 4, y, 8, 3, gold_mid)
        d.line([(4, y), (10, y)], fill=gold_hi)
        pt(d, 5, y, (255, 255, 255, 255)) # glint shine
    return img

def make_ui_icon_mana():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    c_dark = (85, 30, 130, 255)
    c_mid  = (175, 80, 240, 255)
    c_hi   = (230, 160, 255, 255)
    # Radiant faceted gem
    rect(d, 6, 2, 4, 3, c_hi)
    rect(d, 3, 5, 10, 5, c_mid)
    rect(d, 5, 10, 6, 3, c_dark)
    rect(d, 7, 13, 2, 2, c_dark)
    pt(d, 7, 4, (255, 255, 255, 255))
    pt(d, 8, 7, (255, 255, 255, 255))
    return img

def make_ui_icon_food():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    bone = (240, 235, 220, 255)
    meat = (175, 45, 45, 255)
    meat_hi = (220, 85, 75, 255)
    # Roasted meat on the bone
    rect(d, 4, 4, 8, 8, meat)
    rect(d, 5, 5, 6, 6, meat_hi)
    d.line([(5, 5), (9, 5)], fill=(255, 140, 125, 255))
    # Bone ends
    rect(d, 1, 9, 3, 3, bone); pt(d, 11, 3, bone); pt(d, 12, 2, bone)
    return img

def make_ui_icon_worker():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    iron = (150, 155, 170, 255)
    iron_hi = (215, 220, 235, 255)
    wood = (120, 80, 45, 255)
    ore  = (220, 175, 50, 255)
    # Pickaxe head & handle
    d.line([(4, 12), (13, 3)], fill=wood)
    # Curved pick head
    d.line([(2, 2), (8, 4)], fill=iron)
    d.line([(8, 4), (10, 10)], fill=iron)
    pt(d, 3, 2, iron_hi)
    # Ore chunk on bottom right
    rect(d, 10, 10, 5, 5, ore)
    pt(d, 11, 11, (255, 235, 120, 255))
    return img

def make_ui_icon_rumor():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    scroll = (215, 195, 145, 255)
    scroll_hi = (245, 235, 190, 255)
    ribbon = (165, 55, 215, 255)
    # Ancient parchment scroll with glowing ribbon
    rect(d, 3, 4, 10, 8, scroll)
    d.line([(3, 4), (12, 4)], fill=scroll_hi)
    rect(d, 7, 3, 2, 10, ribbon) # Purple magic seal ribbon
    pt(d, 8, 8, (240, 190, 255, 255))
    return img

# =============================================================================
# MAIN EXECUTOR
# =============================================================================

def generate_all():
    print("=== Generating High-Quality Dark Fantasy Pixel Assets ===")

    # 1. Tiles
    tiles = {
        "Tile_Ground_DarkGrass.png": make_tile_dark_grass(),
        "Tile_Ground_Dirt.png":      make_tile_dirt(),
        "Tile_Castle_Floor.png":     make_tile_castle_floor(),
        "Tile_Cobblestone.png":      make_tile_cobblestone(),
        "Tile_Ground_Stone.png":     make_tile_stone(),
        "Tile_Ground_Swamp.png":     make_tile_swamp(),
        "Tile_Highlight.png":        make_tile_highlight(),
    }
    for name, img in tiles.items():
        img.save(os.path.join(TILES_DIR, name))
        print(f" [Tile] Saved {name}")

    # 2. Buildings
    buildings = {
        "Building_Castle.png":       make_building_castle(),
        "Building_Mine.png":         make_building_mine(),
        "Building_Farm.png":         make_building_farm(),
        "Building_Dormitory.png":    make_building_dormitory(),
        "Building_Warehouse.png":    make_building_warehouse(),
        "Building_ManaTower.png":    make_building_mana_tower(),
        "Building_Training.png":     make_building_training(),
        "Building_Construction.png": make_building_construction(),
    }
    for name, img in buildings.items():
        img.save(os.path.join(BUILDINGS_DIR, name))
        print(f" [Building] Saved {name}")

    # 3. Monsters (Main & Frames)
    monsters = {
        "Monster_ImpScrap": make_monster_impscrap,
        "Monster_MireSlime": make_monster_mireslime,
        "Monster_BonePup": make_monster_bonepup,
        "Monster_GrimShadow": make_monster_grimshadow,
        "Monster_GoblinTinker": make_monster_goblintinker,
    }
    for mname, mfunc in monsters.items():
        # Main sprite
        img = mfunc("idle_0")
        img.save(os.path.join(MONSTERS_DIR, f"{mname}.png"))
        # Frames folder
        fdir = os.path.join(MONSTERS_DIR, mname)
        os.makedirs(fdir, exist_ok=True)
        img.save(os.path.join(fdir, "Idle_0.png"))
        mfunc("idle_1").save(os.path.join(fdir, "Idle_1.png"))
        mfunc("walk_0").save(os.path.join(fdir, "Walk_0.png"))
        mfunc("walk_1").save(os.path.join(fdir, "Walk_1.png"))
        mfunc("walk_2").save(os.path.join(fdir, "Walk_2.png"))
        mfunc("walk_3").save(os.path.join(fdir, "Walk_3.png"))
        mfunc("attack_0").save(os.path.join(fdir, "Attack_0.png"))
        mfunc("attack_1").save(os.path.join(fdir, "Attack_1.png"))
        mfunc("attack_2").save(os.path.join(fdir, "Attack_2.png"))
        print(f" [Monster] Saved {mname} + 9 animation frames")

    # 4. UI
    ui_elements = {
        "UI_Panel_9Slice.png": make_ui_panel_9slice(),
        "Icon_Gold.png":       make_ui_icon_gold(),
        "Icon_Mana.png":       make_ui_icon_mana(),
        "Icon_Food.png":       make_ui_icon_food(),
        "Icon_Worker.png":     make_ui_icon_worker(),
        "Icon_Rumor.png":      make_ui_icon_rumor(),
    }
    for name, img in ui_elements.items():
        img.save(os.path.join(UI_DIR, name))
        print(f" [UI] Saved {name}")

    print("=== All Assets Successfully Generated! ===")

if __name__ == "__main__":
    generate_all()
