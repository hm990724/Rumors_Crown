"""
tools/simulate_game_view.py
Accurately simulates the in-game camera rendering of DemonCastle scene with:
- Reduced stone outcrop & dominant lush olive grasslands (WorldGrid.cs update)
- 1.4x Grand Citadel scale vs 0.85x Chibi Minions (5:1 golden ratio)
- Bottom-docked expanded Gothic Construction Menu (BuildMenuUI.cs update)
"""

from PIL import Image, ImageDraw, ImageFont
import os

BASE = r"c:\Users\lookm\Documents\GitHub\Rumors_Crown"
RES = os.path.join(BASE, "Assets", "Resources")

def load_res(rel):
    p = os.path.join(RES, rel)
    return Image.open(p).convert("RGBA")

# 1. Canvas setup: 1280 x 720 (Standard HD game view)
W, H = 1280, 720
canvas = Image.new("RGBA", (W, H), (20, 18, 30, 255))
d = ImageDraw.Draw(canvas)

# Camera setup: orthographicSize = 4.5f -> 9.0 world units vertical
# 720 / 9 = 80 px per unit. 80 / 16 = 5x clean integer pixel-perfect scale
PPU_SCALE = 5 # 16px tile -> 80px on screen
CELL_PX = 16 * PPU_SCALE # 80 px

CAM_X = 0.0 # Centered on Castle
CAM_Y = 0.0
SCREEN_CX = W // 2
SCREEN_CY = H // 2

def world_to_screen(wx, wy):
    sx = SCREEN_CX + int((wx - CAM_X) * CELL_PX)
    sy = SCREEN_CY - int((wy - CAM_Y) * CELL_PX)
    return sx, sy

# 2. Reconstruct WorldGrid 25x25 with new balanced terrain algorithm
GRID_W, GRID_H = 25, 25
CASTLE_X, CASTLE_Y = 12, 12

def calc_tile_type(x, y):
    cx, cy = CASTLE_X, CASTLE_Y
    dx, dy = abs(x - cx), abs(y - cy)
    dist = dx + dy
    if dx <= 1 and dy <= 1: return "CastleFloor"
    if dx == 4 and dy == 4: return "CastleWall"
    if (dx <= 3 and dy <= 3 and dist <= 5) or (dx <= 1 and y < cy and dy <= 6): return "Cobblestone"

    seed = (x * 7 + y * 13 + x * y * 3) % 100
    if y >= cy + 7 and dx <= 3: return "MineralVein" if seed < 40 else "Stone"
    if x <= cx - 6 and dy <= 7: return "DeepForest" if seed < 50 else "Forest"
    if y <= cy - 8 and dx <= 7: return "SwampDeep" if seed < 40 else "Swamp"
    if (dy <= 1 and x > cx + 3) or (dx <= 1 and y > cy + 3 and y < cy + 7): return "Dirt"
    if seed < 22: return "Dirt"
    if seed < 32 and (x < cx - 2 or y > cy + 2): return "Forest"
    return "DarkGrass"

tiles = {
    "DarkGrass": load_res("Tiles/Tile_Ground_DarkGrass.png"),
    "Dirt": load_res("Tiles/Tile_Ground_Dirt.png"),
    "Cobblestone": load_res("Tiles/Tile_Cobblestone.png"),
    "CastleFloor": load_res("Tiles/Tile_Castle_Floor.png"),
    "Stone": load_res("Tiles/Tile_Ground_Stone.png"),
    "MineralVein": load_res("Tiles/Tile_Ground_Stone.png"),
    "Swamp": load_res("Tiles/Tile_Ground_Swamp.png"),
    "SwampDeep": load_res("Tiles/Tile_Ground_Swamp.png"),
    "Forest": load_res("Tiles/Tile_Ground_DarkGrass.png"),
    "DeepForest": load_res("Tiles/Tile_Ground_DarkGrass.png"),
    "CastleWall": load_res("Tiles/Tile_Ground_Stone.png"),
}

# Render ground tiles (2-Layer Base + Active)
for gy in range(GRID_H):
    for gx in range(GRID_W):
        ttype = calc_tile_type(gx, gy)
        img = tiles.get(ttype, tiles["DarkGrass"])
        img_scaled = img.resize((CELL_PX, CELL_PX), Image.NEAREST)

        wx = gx - GRID_W * 0.5 + 0.5
        wy = gy - GRID_H * 0.5 + 0.5
        sx, sy = world_to_screen(wx, wy)
        px = sx - CELL_PX // 2
        py = sy - CELL_PX // 2

        if -CELL_PX <= px < W + CELL_PX and -CELL_PX <= py < H + CELL_PX:
            # Underlay base grass if not grass
            if ttype != "DarkGrass":
                base_scaled = tiles["DarkGrass"].resize((CELL_PX, CELL_PX), Image.NEAREST)
                canvas.paste(base_scaled, (px, py), base_scaled)
            canvas.paste(img_scaled, (px, py), img_scaled)

# 3. Render Props
prop_tree = load_res("Props/Prop_Tree.png")
prop_bush = load_res("Props/Prop_Bush.png")
prop_torch = load_res("Props/Prop_Torch.png")
prop_barrel = load_res("Props/Prop_Barrel.png")
prop_wall = load_res("Props/Prop_Wall.png")

def paste_sprite(spr, wx, wy, scale_mod=1.0, ox=0, oy=0):
    sw = int(spr.size[0] * PPU_SCALE * scale_mod)
    sh = int(spr.size[1] * PPU_SCALE * scale_mod)
    scaled = spr.resize((sw, sh), Image.NEAREST)
    sx, sy = world_to_screen(wx, wy)
    px = sx - sw // 2 + ox
    py = sy - sh // 2 + oy
    if -sw <= px < W + sw and -sh <= py < H + sh:
        canvas.paste(scaled, (px, py), scaled)

for gy in range(GRID_H):
    for gx in range(GRID_W):
        ttype = calc_tile_type(gx, gy)
        wx = gx - GRID_W * 0.5 + 0.5
        wy = gy - GRID_H * 0.5 + 0.5
        cx, cy = CASTLE_X, CASTLE_Y
        dx, dy = abs(gx - cx), abs(gy - cy)

        if ttype == "CastleWall":
            paste_sprite(prop_wall, wx, wy + 0.25)
        elif (dx == 2 and dy == 2) or (dx == 3 and dy == 0) or (dx == 0 and dy == 3):
            paste_sprite(prop_torch, wx + 0.35, wy + 0.2)
        elif dx == 3 and dy == 1:
            paste_sprite(prop_barrel, wx - 0.2, wy)
        elif ttype in ["Forest", "DeepForest"]:
            h = (gx * 17 + gy * 31) % 10
            if h < 6:
                paste_sprite(prop_tree, wx, wy + 0.8)
            else:
                paste_sprite(prop_bush, wx, wy)
        elif ttype == "DarkGrass":
            h = (gx * 23 + gy * 7) % 15
            if h == 0 and (dx > 3 or dy > 3):
                paste_sprite(prop_bush, wx + 0.2, wy - 0.1)

# 4. Render Buildings
# Castle at center with 1.4x scale and upward offset
b_castle = load_res("Buildings/Building_Castle.png")
paste_sprite(b_castle, 0.0, 0.0 + 0.45, scale_mod=1.4)

# Mine at North-East outcrop
b_mine = load_res("Buildings/Building_Mine.png")
paste_sprite(b_mine, 3.0, 7.0)

# Mushroom Farm at South-West
b_farm = load_res("Buildings/Building_Farm.png")
paste_sprite(b_farm, -4.0, -3.0)

# Dormitory at West
b_dorm = load_res("Buildings/Building_Dormitory.png")
paste_sprite(b_dorm, -4.0, 2.0)

# 5. Render 2.5D Chibi Monsters (0.85x scale for 5:1 ratio vs Castle)
m_imp = load_res("Monsters/Monster_ImpScrap.png")
paste_sprite(m_imp, -1.0, 0.8, scale_mod=0.85)

m_slime = load_res("Monsters/Monster_MireSlime.png")
paste_sprite(m_slime, -3.2, -2.2, scale_mod=0.85)

m_pup = load_res("Monsters/Monster_BonePup.png")
paste_sprite(m_pup, 1.2, -1.5, scale_mod=0.85)

m_goblin = load_res("Monsters/Monster_GoblinTinker.png")
paste_sprite(m_goblin, 2.8, 5.8, scale_mod=0.85)

m_grim = load_res("Monsters/Monster_GrimShadow.png")
paste_sprite(m_grim, 0.3, 1.8, scale_mod=0.85)

# 6. Top Bar UI
top_bar_h = 44
rect_top = Image.new("RGBA", (W, top_bar_h), (20, 18, 28, 240))
canvas.paste(rect_top, (0, 0), rect_top)
d.line([(0, top_bar_h), (W, top_bar_h)], fill=(120, 100, 45, 255))

icon_gold = load_res("UI/Icon_Gold.png").resize((24, 24), Image.NEAREST)
icon_food = load_res("UI/Icon_Food.png").resize((24, 24), Image.NEAREST)
icon_mana = load_res("UI/Icon_Mana.png").resize((24, 24), Image.NEAREST)
icon_worker = load_res("UI/Icon_Worker.png").resize((24, 24), Image.NEAREST)
icon_rumor = load_res("UI/Icon_Rumor.png").resize((24, 24), Image.NEAREST)

canvas.paste(icon_gold, (30, 10), icon_gold)
canvas.paste(icon_food, (160, 10), icon_food)
canvas.paste(icon_mana, (290, 10), icon_mana)
canvas.paste(icon_worker, (420, 10), icon_worker)
canvas.paste(icon_rumor, (550, 10), icon_rumor)

# 7. Render Bottom Expanded Construction Menu (BuildMenuUI - 340px height)
menu_h = 320
menu_w = int(W * 0.90)
menu_x = (W - menu_w) // 2
menu_y = H - menu_h + 10

panel_bg = Image.new("RGBA", (menu_w, menu_h), (25, 22, 34, 245))
canvas.paste(panel_bg, (menu_x, menu_y), panel_bg)
d.rectangle([menu_x, menu_y, menu_x + menu_w, menu_y + menu_h], outline=(150, 125, 50, 255), width=2)

# Title
d.text((menu_x + menu_w // 2 - 80, menu_y + 12), "✦ 영지 건설 ✦ (Grid: 13, 14)", fill=(250, 225, 110, 255))

# Draw 4 Cards horizontally (215px width each)
cards = [
    ("Building_Mine.png", "어둠 광산", "광석과 금화를 채굴한다.\n일꾼 1인당 금화 15/일.", "금 80  광 10", "일꾼 3명 | 유지 4/일", True),
    ("Building_Farm.png", "독버섯 농장", "발광 버섯을 재배해\n식량을 생산한다. 식량 5/일.", "금 60  마 20", "일꾼 3명 | 유지 3/일", True),
    ("Building_Dormitory.png", "몬스터 숙소", "피로를 회복한다.\n최대 4마리 수용.", "금 70  광 5", "수용 4마리 | 유지 2/일", True),
    ("Building_ManaTower.png", "마력 정제소", "마력을 채취·정제한다.\n일꾼 1인당 마력 8/일.", "금 140  광 20", "🔒 명성 20 달성 필요", False),
]

card_w = 215
card_h = 245
start_cx = menu_x + 25
card_cy = menu_y + 45

for i, (spr_file, name, desc, cost, upkeep, unlocked) in enumerate(cards):
    cx = start_cx + i * (card_w + 16)
    c_bg = (35, 32, 45, 250) if unlocked else (25, 24, 30, 210)
    d.rectangle([cx, card_cy, cx + card_w, card_cy + card_h], fill=c_bg, outline=(110, 95, 45, 255) if unlocked else (60, 55, 65, 255), width=2)

    # Preview sprite
    c_spr = load_res(f"Buildings/{spr_file}").resize((64, 64), Image.NEAREST)
    canvas.paste(c_spr, (cx + (card_w - 64) // 2, card_cy + 12), c_spr)

    # Name
    d.text((cx + 18, card_cy + 82), name, fill=(255, 230, 115, 255) if unlocked else (140, 140, 140, 255))
    # Desc
    d.text((cx + 18, card_cy + 108), desc, fill=(190, 190, 205, 255) if unlocked else (100, 100, 105, 255))
    # Cost
    d.text((cx + 18, card_cy + 152), cost, fill=(255, 215, 80, 255) if unlocked else (120, 120, 120, 255))
    # Upkeep
    d.text((cx + 18, card_cy + 174), upkeep, fill=(200, 200, 220, 255) if unlocked else (240, 110, 100, 255))

    # Action Button
    if unlocked:
        btn_y = card_cy + 198
        d.rectangle([cx + 12, btn_y, cx + card_w - 12, btn_y + 36], fill=(70, 60, 85, 255), outline=(210, 175, 70, 255), width=2)
        d.text((cx + 55, btn_y + 8), "✦ 건 설 하 기 ✦", fill=(255, 245, 220, 255))

out_path = os.path.join(BASE, "scratch", "simulated_game_view.png")
canvas.save(out_path)
art_out = os.path.join(r"C:\Users\lookm\.gemini\antigravity\brain\7d93dca4-954a-4419-93c6-181e064436c4", "simulated_game_view.png")
canvas.save(art_out)
print(f"Simulated game view saved to: {out_path} and {art_out}")
