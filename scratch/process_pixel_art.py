import os
import glob
from PIL import Image

RAW_DIR = r"C:\Users\lookm\Documents\GitHub\Rumors_Crown\Assets\Art\Raw"
PROD_DIR = r"C:\Users\lookm\Documents\GitHub\Rumors_Crown\Assets\Resources\ProductionArt\Monsters"

os.makedirs(PROD_DIR, exist_ok=True)

def remove_black_background(img, threshold=25):
    img = img.convert("RGBA")
    datas = img.getdata()
    newData = []
    for item in datas:
        # Check if color is near-black
        if item[0] < threshold and item[1] < threshold and item[2] < threshold:
            newData.append((0, 0, 0, 0))
        else:
            newData.append(item)
    img.putdata(newData)
    return img

def crop_to_content(img):
    bbox = img.getbbox()
    if bbox:
        return img.crop(bbox)
    return img

def fit_to_canvas(img, target_size=(32, 32)):
    # Aspect ratio preserving resize using NEAREST
    w, h = img.size
    max_dim = max(w, h)
    scale = (target_size[0] - 4) / max_dim
    new_w = max(1, int(w * scale))
    new_h = max(1, int(h * scale))
    
    resized = img.resize((new_w, new_h), Image.Resampling.NEAREST)
    
    canvas = Image.new("RGBA", target_size, (0, 0, 0, 0))
    # Center horizontally, place near bottom vertically
    paste_x = (target_size[0] - new_w) // 2
    paste_y = target_size[1] - new_h - 2
    canvas.paste(resized, (paste_x, paste_y), resized)
    return canvas

def generate_animation_frames(base_32, out_folder):
    os.makedirs(os.path.join(out_folder, "Idle"), exist_ok=True)
    os.makedirs(os.path.join(out_folder, "Walk"), exist_ok=True)
    os.makedirs(os.path.join(out_folder, "Attack"), exist_ok=True)

    # 1. Idle Frames (2 frames: neutral, subtle 1px breath)
    base_32.save(os.path.join(out_folder, "Idle", "Idle_0.png"))
    
    # Idle frame 1: subtle vertical shift
    f_idle1 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_idle1.paste(base_32, (0, 1), base_32)
    f_idle1.save(os.path.join(out_folder, "Idle", "Idle_1.png"))

    # 2. Walk Frames (4 frames cycle)
    # Walk 0: neutral
    base_32.save(os.path.join(out_folder, "Walk", "Walk_0.png"))
    
    # Walk 1: slight step forward & bob up 1px
    f_walk1 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_walk1.paste(base_32, (1, -1), base_32)
    f_walk1.save(os.path.join(out_folder, "Walk", "Walk_1.png"))
    
    # Walk 2: down 1px
    f_walk2 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_walk2.paste(base_32, (0, 1), base_32)
    f_walk2.save(os.path.join(out_folder, "Walk", "Walk_2.png"))
    
    # Walk 3: other step
    f_walk3 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_walk3.paste(base_32, (-1, -1), base_32)
    f_walk3.save(os.path.join(out_folder, "Walk", "Walk_3.png"))

    # 3. Attack Frames (3 frames: charge/windup, strike/lunge, recover)
    # Attack 0: windup (lean back 2px)
    f_atk0 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_atk0.paste(base_32, (-2, 0), base_32)
    f_atk0.save(os.path.join(out_folder, "Attack", "Attack_0.png"))
    
    # Attack 1: lunge strike (forward 3px, down 1px)
    f_atk1 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_atk1.paste(base_32, (3, 1), base_32)
    f_atk1.save(os.path.join(out_folder, "Attack", "Attack_1.png"))
    
    # Attack 2: recover
    f_atk2 = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    f_atk2.paste(base_32, (1, 0), base_32)
    f_atk2.save(os.path.join(out_folder, "Attack", "Attack_2.png"))

def process_all():
    raw_files = glob.glob(os.path.join(RAW_DIR, "*.png"))
    print(f"Found {len(raw_files)} raw concept images.")
    for raw_path in raw_files:
        name = os.path.basename(raw_path).replace("_Concept.png", "")
        # e.g. Monster_ImpScrap
        print(f"Processing {name}...")
        img = Image.open(raw_path)
        img_trans = remove_black_background(img)
        img_cropped = crop_to_content(img_trans)
        img_32 = fit_to_canvas(img_cropped, (32, 32))
        
        # Save primary single sprite
        primary_out = os.path.join(PROD_DIR, f"{name}.png")
        img_32.save(primary_out)
        print(f"Saved primary sprite to: {primary_out}")
        
        # Save animation frames
        anim_folder = os.path.join(PROD_DIR, name.replace("Monster_", ""))
        generate_animation_frames(img_32, anim_folder)
        print(f"Generated Idle/Walk/Attack frames in: {anim_folder}")

if __name__ == "__main__":
    process_all()
