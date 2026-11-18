"""
Renders a full simulated in-game visual showcase of the reworked RUMORS
combining all new high-res pixel art assets, tiles, buildings, monsters, and gothic UI.
Saves to the artifacts directory.
"""

from PIL import Image, ImageDraw, ImageFont
import os

RES_DIR = r"C:\Users\lookm\Documents\GitHub\Rumors_Crown\Assets\Resources"
ART_DIR = r"C:\Users\lookm\.gemini\antigravity\brain\6fced829-489a-4533-a807-023afcbe913a"
OUT_PATH = os.path.join(ART_DIR, "rumors_visual_rework_showcase.png")

def load(rel):
    p = os.path.join(RES_DIR, rel)
    return Image.open(p).convert("RGBA")

# 1. Canvas setup: 960x540 (half 1080p for crisp pixel doubling)
W, H = 960, 540
canvas = Image.new("RGBA", (W, H), (20, 18, 30, 255))
d = ImageDraw.Draw(canvas)

# 2. Render simulated 2D world viewport (centered around Castle)
tile_grass = load("Tiles/Tile_Ground_DarkGrass.png")
tile_cobble = load("Tiles/Tile_Cobblestone.png")
tile_floor = load("Tiles/Tile_Castle_Floor.png")
tile_dirt = load("Tiles/Tile_Ground_Dirt.png")
tile_stone = load("Tiles/Tile_Ground_Stone.png")
tile_swamp = load("Tiles/Tile_Ground_Swamp.png")

scale = 2
ts = 16 * scale # 32px per tile
cx, cy = W // 2, H // 2 + 10

# Draw 25x15 visible tile grid
for gy in range(-7, 8):
    for gx in range(-15, 16):
        dx, dy = abs(gx), abs(gy)
        dist = dx + dy
        # Pick tile
        if dx <= 1 and dy <= 1:
            t = tile_floor
        elif dx <= 4 and dy <= 4 and dist <= 6:
            t = tile_cobble
        elif gy > 3 and dx <= 5:
            t = tile_stone
        elif gy < -3 and dx <= 5:
            t = tile_swamp
        elif gx < -4:
            t = tile_grass
        elif gx > 4:
            t = tile_dirt
        else:
            t = tile_grass

        px = cx + gx * ts - ts // 2
        py = cy + gy * ts - ts // 2
        t_scaled = t.resize((ts, ts), Image.NEAREST)
        canvas.paste(t_scaled, (px, py), t_scaled)

# 3. Draw Buildings
def paste_spr(spr, wx, wy, s=2):
    sw, sh = spr.size[0] * s, spr.size[1] * s
    px = cx + wx * ts - sw // 2
    py = cy + wy * ts - sh // 2
    scaled = spr.resize((sw, sh), Image.NEAREST)
    canvas.paste(scaled, (px, py), scaled)

# Castle (center)
b_castle = load("Buildings/Building_Castle.png")
paste_spr(b_castle, 0, 0, 2)

# Mine (north-east stone)
b_mine = load("Buildings/Building_Mine.png")
paste_spr(b_mine, 5, -4, 2)

# Mushroom Farm (south-west grass)
b_farm = load("Buildings/Building_Farm.png")
paste_spr(b_farm, -4, 3, 2)

# Dormitory (west cobble)
b_dorm = load("Buildings/Building_Dormitory.png")
paste_spr(b_dorm, -3, -2, 2)

# Mana Tower (north stone)
b_mana = load("Buildings/Building_ManaTower.png")
paste_spr(b_mana, 1, -5, 2)

# Training Ground (east cobble)
b_train = load("Buildings/Building_Training.png")
paste_spr(b_train, 4, 1, 2)

# 4. Draw Animated Monsters wandering
m_imp = load("Monsters/Monster_ImpScrap.png")
paste_spr(m_imp, -1, 2, 2)

m_slime = load("Monsters/Monster_MireSlime.png")
paste_spr(m_slime, -3, 4, 2)

m_pup = load("Monsters/Monster_BonePup.png")
paste_spr(m_pup, 2, -2, 2)

m_shadow = load("Monsters/Monster_GrimShadow.png")
paste_spr(m_shadow, 1, -4, 2)

m_tinker = load("Monsters/Monster_GoblinTinker.png")
paste_spr(m_tinker, 4, -3, 2)

# 5. Render Top Bar UI
top_bar = Image.new("RGBA", (W, 44), (24, 20, 36, 245))
tb_draw = ImageDraw.Draw(top_bar)
# Gold top/bottom metallic bevel lines
tb_draw.line([(0, 0), (W, 0)], fill=(185, 145, 65, 255))
tb_draw.line([(0, 43), (W, 43)], fill=(120, 95, 45, 255))

# Resources
icons = [
    ("Icon_Gold.png", "200", (255, 220, 80)),
    ("Icon_Mana.png", "60", (200, 140, 255)),
    ("Icon_Food.png", "40", (245, 120, 120)),
    ("Icon_Worker.png", "30", (220, 215, 190)),
    ("Icon_Rumor.png", "5", (210, 160, 255))
]
ix = 20
for ic_name, val, col in icons:
    ic = load(f"UI/{ic_name}").resize((22, 22), Image.NEAREST)
    top_bar.paste(ic, (ix, 11), ic)
    tb_draw.text((ix + 28, 14), val, fill=col)
    ix += 95

# Center Date & Progress
tb_draw.text((W // 2 - 45, 8), "1년 1월 1일", fill=(255, 240, 200))
# Liquid Progress Bar
tb_draw.rectangle([W // 2 - 60, 28, W // 2 + 60, 34], fill=(35, 25, 45, 255), outline=(100, 80, 120, 255))
tb_draw.rectangle([W // 2 - 58, 30, W // 2 - 58 + 45, 32], fill=(225, 55, 85, 255))

# Speed Buttons
btn_norm = load("UI/UI_Button_Normal.png").resize((38, 26), Image.NEAREST)
btn_hov  = load("UI/UI_Button_Hover.png").resize((38, 26), Image.NEAREST)
rx = W - 230
for i, label in enumerate(["||", "1x", "2x", "4x"]):
    b = btn_hov if label == "1x" else btn_norm
    top_bar.paste(b, (rx + i * 44, 9), b)
    tb_draw.text((rx + i * 44 + 11, 15), label, fill=(255, 235, 180) if label == "1x" else (200, 190, 210))

canvas.paste(top_bar, (0, 0), top_bar)

# 6. Render Bottom Monster Dock
dock_w, dock_h = 320, 48
dock_x = (W - dock_w) // 2
dock_y = H - dock_h - 10
dock = Image.new("RGBA", (dock_w, dock_h), (22, 18, 32, 240))
dd = ImageDraw.Draw(dock)
dd.rectangle([0, 0, dock_w - 1, dock_h - 1], outline=(175, 135, 60, 255))
# 5 monster portraits
m_list = ["Monster_ImpScrap", "Monster_MireSlime", "Monster_BonePup", "Monster_GrimShadow", "Monster_GoblinTinker"]
for i, m_name in enumerate(m_list):
    bx = 14 + i * 60
    dock.paste(btn_norm.resize((48, 36), Image.NEAREST), (bx, 6), btn_norm.resize((48, 36), Image.NEAREST))
    m_spr = load(f"Monsters/{m_name}.png").resize((28, 28), Image.NEAREST)
    dock.paste(m_spr, (bx + 10, 10), m_spr)

canvas.paste(dock, (dock_x, dock_y), dock)

canvas.save(OUT_PATH)
print("Saved showcase image to:", OUT_PATH)
