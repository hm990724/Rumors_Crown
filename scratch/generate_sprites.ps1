Add-Type -AssemblyName System.Drawing

$baseDir = "C:\Users\lookm\Documents\GitHub\Rumors_Crown"
$tileDir = "$baseDir\Assets\Art\Sprites\Tiles"
$bldDir = "$baseDir\Assets\Art\Sprites\Buildings"
$monDir = "$baseDir\Assets\Art\Sprites\Monsters"
$uiDir = "$baseDir\Assets\Art\Sprites\UI"

New-Item -ItemType Directory -Force -Path $tileDir | Out-Null
New-Item -ItemType Directory -Force -Path $bldDir | Out-Null
New-Item -ItemType Directory -Force -Path $monDir | Out-Null
New-Item -ItemType Directory -Force -Path $uiDir | Out-Null

function ColorFromHex($hex) {
    return [System.Drawing.ColorTranslator]::FromHtml($hex)
}

$C_Trans = [System.Drawing.Color]::FromArgb(0, 0, 0, 0)
$C_Void = ColorFromHex("#0d0e15")
$C_StoneDark = ColorFromHex("#1a1c26")
$C_StoneMid = ColorFromHex("#2b3044")
$C_StoneLight = ColorFromHex("#485172")
$C_StoneHigh = ColorFromHex("#6b779e")

$C_GrassDark = ColorFromHex("#131c15")
$C_GrassMid = ColorFromHex("#1e2e1e")
$C_GrassLight = ColorFromHex("#324a30")
$C_GrassAccent = ColorFromHex("#4d6d45")

$C_DirtDark = ColorFromHex("#1c1612")
$C_DirtMid = ColorFromHex("#2e241c")
$C_DirtLight = ColorFromHex("#47382a")

$C_SwampDark = ColorFromHex("#111a14")
$C_SwampMid = ColorFromHex("#1a2b1f")
$C_SwampLight = ColorFromHex("#2d4833")
$C_SwampPoison = ColorFromHex("#4ea863")

$C_CrimsonDark = ColorFromHex("#360811")
$C_CrimsonMid = ColorFromHex("#6e1322")
$C_CrimsonBright = ColorFromHex("#b8273d")
$C_CrimsonGlow = ColorFromHex("#f24e64")

$C_BoneDark = ColorFromHex("#3a3532")
$C_BoneMid = ColorFromHex("#736c64")
$C_BoneLight = ColorFromHex("#b5aba0")
$C_BoneWhite = ColorFromHex("#ede6de")

$C_PurpleDark = ColorFromHex("#1e112d")
$C_PurpleMid = ColorFromHex("#3c1c5e")
$C_PurpleLight = ColorFromHex("#6a32a1")
$C_PurpleGlow = ColorFromHex("#aa5eff")

$C_GoldDark = ColorFromHex("#382309")
$C_GoldMid = ColorFromHex("#7a5118")
$C_GoldBright = ColorFromHex("#c48b29")
$C_GoldGlow = ColorFromHex("#ffd152")

$C_SlimeDark = ColorFromHex("#0b211a")
$C_SlimeMid = ColorFromHex("#164a38")
$C_SlimeBright = ColorFromHex("#258763")
$C_SlimeGlow = ColorFromHex("#4ef5a7")

$C_GoblinDark = ColorFromHex("#1b2413")
$C_GoblinMid = ColorFromHex("#374724")
$C_GoblinLight = ColorFromHex("#597536")
$C_GoblinEar = ColorFromHex("#7a4a45")

function New-ImageCanvas($w, $h, $bg) {
    $bmp = New-Object System.Drawing.Bitmap $w, $h
    for ($y = 0; $y -lt $h; $y++) {
        for ($x = 0; $x -lt $w; $x++) {
            $bmp.SetPixel($x, $y, $bg)
        }
    }
    return $bmp
}

function FillRect($bmp, $x, $y, $w, $h, $col) {
    for ($py = $y; $py -lt ($y + $h); $py++) {
        for ($px = $x; $px -lt ($x + $w); $px++) {
            if ($px -ge 0 -and $px -lt $bmp.Width -and $py -ge 0 -and $py -lt $bmp.Height) {
                $bmp.SetPixel($px, $py, $col)
            }
        }
    }
}

function DrawOutline($bmp, $x, $y, $w, $h, $col) {
    for ($px = $x; $px -le ($x + $w); $px++) {
        if ($px -ge 0 -and $px -lt $bmp.Width) {
            if ($y -ge 0 -and $y -lt $bmp.Height) { $bmp.SetPixel($px, $y, $col) }
            if (($y + $h) -ge 0 -and ($y + $h) -lt $bmp.Height) { $bmp.SetPixel($px, $y + $h, $col) }
        }
    }
    for ($py = $y; $py -le ($y + $h); $py++) {
        if ($py -ge 0 -and $py -lt $bmp.Height) {
            if ($x -ge 0 -and $x -lt $bmp.Width) { $bmp.SetPixel($x, $py, $col) }
            if (($x + $w) -ge 0 -and ($x + $w) -lt $bmp.Width) { $bmp.SetPixel($x + $w, $py, $col) }
        }
    }
}

function FillCircle($bmp, $cx, $cy, $r, $col) {
    for ($y = -$r; $y -le $r; $y++) {
        for ($x = -$r; $x -le $r; $x++) {
            if (($x*$x + $y*$y) -le ($r*$r)) {
                $px = $cx + $x
                $py = $cy + $y
                if ($px -ge 0 -and $px -lt $bmp.Width -and $py -ge 0 -and $py -lt $bmp.Height) {
                    $bmp.SetPixel($px, $py, $col)
                }
            }
        }
    }
}

function DrawPixelSafe($bmp, $x, $y, $col) {
    if ($x -ge 0 -and $x -lt $bmp.Width -and $y -ge 0 -and $y -lt $bmp.Height) {
        $bmp.SetPixel($x, $y, $col)
    }
}

# --- 1. TILES ---
# Tile_Ground_DarkGrass.png
$t1 = New-ImageCanvas 32 32 $C_GrassMid
for ($y = 0; $y -lt 32; $y++) {
    for ($x = 0; $x -lt 32; $x++) {
        if (($x * 7 + $y * 13) % 5 -eq 0) { $t1.SetPixel($x, $y, $C_GrassDark) }
        elseif (($x * 11 + $y * 3) % 7 -eq 0) { $t1.SetPixel($x, $y, $C_GrassLight) }
    }
}
DrawOutline $t1 0 0 31 31 $C_Void
DrawPixelSafe $t1 6 8 $C_GrassAccent; DrawPixelSafe $t1 6 9 $C_GrassAccent
DrawPixelSafe $t1 19 14 $C_GrassAccent; DrawPixelSafe $t1 20 15 $C_GrassAccent
DrawPixelSafe $t1 12 24 $C_GrassAccent; DrawPixelSafe $t1 13 25 $C_GrassAccent
$t1.Save("$tileDir\Tile_Ground_DarkGrass.png", [System.Drawing.Imaging.ImageFormat]::Png)
$t1.Dispose()

# Tile_Ground_Dirt.png
$t2 = New-ImageCanvas 32 32 $C_DirtMid
for ($y = 0; $y -lt 32; $y++) {
    for ($x = 0; $x -lt 32; $x++) {
        if (($x * 13 + $y * 7) % 4 -eq 0) { $t2.SetPixel($x, $y, $C_DirtDark) }
        elseif (($x * 5 + $y * 11) % 6 -eq 0) { $t2.SetPixel($x, $y, $C_DirtLight) }
    }
}
DrawOutline $t2 0 0 31 31 $C_Void
DrawPixelSafe $t2 8 12 $C_StoneLight; DrawPixelSafe $t2 9 12 $C_StoneMid
DrawPixelSafe $t2 22 20 $C_StoneLight
$t2.Save("$tileDir\Tile_Ground_Dirt.png", [System.Drawing.Imaging.ImageFormat]::Png)
$t2.Dispose()

# Tile_Ground_Stone.png
$t3 = New-ImageCanvas 32 32 $C_StoneMid
for ($y = 0; $y -lt 32; $y++) {
    for ($x = 0; $x -lt 32; $x++) {
        if (($x * 17 + $y * 5) % 5 -eq 0) { $t3.SetPixel($x, $y, $C_StoneDark) }
        elseif (($x * 7 + $y * 19) % 7 -eq 0) { $t3.SetPixel($x, $y, $C_StoneLight) }
    }
}
DrawOutline $t3 0 0 31 31 $C_Void
FillRect $t3 2 10 28 1 $C_StoneDark
FillRect $t3 2 21 28 1 $C_StoneDark
FillRect $t3 14 2 1 8 $C_StoneDark
FillRect $t3 20 11 1 10 $C_StoneDark
FillRect $t3 8 22 1 9 $C_StoneDark
$t3.Save("$tileDir\Tile_Ground_Stone.png", [System.Drawing.Imaging.ImageFormat]::Png)
$t3.Dispose()

# Tile_Ground_Swamp.png
$t4 = New-ImageCanvas 32 32 $C_SwampMid
for ($y = 0; $y -lt 32; $y++) {
    for ($x = 0; $x -lt 32; $x++) {
        if (($x * 9 + $y * 17) % 5 -eq 0) { $t4.SetPixel($x, $y, $C_SwampDark) }
        elseif (($x * 13 + $y * 7) % 6 -eq 0) { $t4.SetPixel($x, $y, $C_SwampLight) }
    }
}
DrawOutline $t4 0 0 31 31 $C_Void
FillCircle $t4 16 16 6 $C_SwampMid
FillCircle $t4 16 16 3 $C_SwampPoison
DrawPixelSafe $t4 15 17 $C_SlimeGlow
DrawPixelSafe $t4 7 8 $C_SwampPoison
DrawPixelSafe $t4 25 22 $C_SwampPoison
$t4.Save("$tileDir\Tile_Ground_Swamp.png", [System.Drawing.Imaging.ImageFormat]::Png)
$t4.Dispose()

# Tile_Castle_Floor.png
$t5 = New-ImageCanvas 32 32 $C_StoneDark
DrawOutline $t5 0 0 31 31 $C_StoneMid
FillRect $t5 2 16 28 1 $C_Void
FillRect $t5 16 2 1 28 $C_Void
for ($d = -4; $d -le 4; $d++) {
    $span = 4 - [Math]::Abs($d)
    FillRect $t5 (16 - $span) (16 + $d) (2 * $span + 1) 1 $C_CrimsonMid
}
for ($d = -2; $d -le 2; $d++) {
    $span = 2 - [Math]::Abs($d)
    FillRect $t5 (16 - $span) (16 + $d) (2 * $span + 1) 1 $C_CrimsonBright
}
DrawPixelSafe $t5 16 16 $C_CrimsonGlow
$t5.Save("$tileDir\Tile_Castle_Floor.png", [System.Drawing.Imaging.ImageFormat]::Png)
$t5.Dispose()

# Tile_Highlight.png
$t6 = New-ImageCanvas 32 32 $C_Trans
DrawOutline $t6 1 1 29 29 $C_CrimsonBright
DrawOutline $t6 2 2 27 27 $C_CrimsonGlow
DrawPixelSafe $t6 1 1 $C_GoldGlow; DrawPixelSafe $t6 30 1 $C_GoldGlow
DrawPixelSafe $t6 1 30 $C_GoldGlow; DrawPixelSafe $t6 30 30 $C_GoldGlow
$t6.Save("$tileDir\Tile_Highlight.png", [System.Drawing.Imaging.ImageFormat]::Png)
$t6.Dispose()

# --- 2. BUILDINGS ---
# Building_Castle.png (48x48)
$bc = New-ImageCanvas 48 48 $C_Trans
FillRect $bc 6 36 36 10 $C_StoneDark
DrawOutline $bc 6 36 35 9 $C_Void
FillRect $bc 12 10 24 26 $C_StoneMid
DrawOutline $bc 12 10 23 25 $C_Void
FillRect $bc 8 14 8 22 $C_StoneDark
DrawOutline $bc 8 14 7 21 $C_Void
FillRect $bc 32 14 8 22 $C_StoneDark
DrawOutline $bc 32 14 7 21 $C_Void
# Tower spires
for ($i = 0; $i -lt 5; $i++) {
    FillRect $bc (12 - $i) (14 - $i) (2 * $i + 1) 1 $C_CrimsonDark
    FillRect $bc (36 - $i) (14 - $i) (2 * $i + 1) 1 $C_CrimsonDark
}
# Center Demonic Crest
for ($i = 0; $i -lt 9; $i++) {
    FillRect $bc (24 - $i) (10 - $i) (2 * $i + 1) 1 $C_CrimsonMid
}
DrawPixelSafe $bc 24 1 $C_CrimsonGlow
# Gate
FillRect $bc 21 36 6 10 $C_Void
FillRect $bc 21 35 6 1 $C_CrimsonBright
# Red Demonic Eye
FillCircle $bc 24 24 4 $C_CrimsonDark
FillCircle $bc 24 24 2 $C_CrimsonBright
DrawPixelSafe $bc 24 24 $C_CrimsonGlow
# Slit windows
DrawPixelSafe $bc 11 24 $C_CrimsonGlow
DrawPixelSafe $bc 35 24 $C_CrimsonGlow
DrawPixelSafe $bc 18 18 $C_CrimsonGlow
DrawPixelSafe $bc 29 18 $C_CrimsonGlow
$bc.Save("$bldDir\Building_Castle.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bc.Dispose()

# Building_Mine.png (32x32)
$bm = New-ImageCanvas 32 32 $C_Trans
FillCircle $bm 16 18 13 $C_StoneDark
FillRect $bm 10 16 12 14 $C_Void
DrawOutline $bm 9 15 13 15 $C_DirtDark
FillRect $bm 10 16 2 14 $C_DirtLight
FillRect $bm 20 16 2 14 $C_DirtLight
FillRect $bm 10 15 12 2 $C_DirtLight
# Mine cart
FillRect $bm 4 23 6 5 $C_DirtLight
DrawOutline $bm 4 23 5 4 $C_StoneDark
DrawPixelSafe $bm 5 24 $C_GoldBright; DrawPixelSafe $bm 7 24 $C_GoldGlow
DrawPixelSafe $bm 23 16 $C_GoldBright; DrawPixelSafe $bm 24 15 $C_GoldGlow
DrawPixelSafe $bm 8 10 $C_PurpleGlow
DrawPixelSafe $bm 11 18 $C_GoldGlow
$bm.Save("$bldDir\Building_Mine.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bm.Dispose()

# Building_Farm.png (32x32)
$bf = New-ImageCanvas 32 32 $C_Trans
FillRect $bf 4 22 24 8 $C_DirtDark
DrawOutline $bf 4 22 23 7 $C_Void
FillRect $bf 4 16 2 12 $C_BoneMid
FillRect $bf 26 16 2 12 $C_BoneMid
FillRect $bf 4 20 24 2 $C_BoneDark
# Giant Purple Mushroom
FillRect $bf 15 15 3 9 $C_BoneLight
FillCircle $bf 16 12 7 $C_PurpleMid
FillCircle $bf 16 12 5 $C_PurpleLight
DrawPixelSafe $bf 14 10 $C_PurpleGlow; DrawPixelSafe $bf 18 11 $C_PurpleGlow
DrawPixelSafe $bf 16 13 $C_PurpleGlow
# Small Red Mushroom
FillRect $bf 9 18 2 6 $C_BoneMid
FillCircle $bf 10 17 4 $C_CrimsonBright
DrawPixelSafe $bf 9 16 $C_BoneWhite
# Small Green Mushroom
FillRect $bf 23 19 2 5 $C_BoneMid
FillCircle $bf 24 18 4 $C_SlimeBright
DrawPixelSafe $bf 23 17 $C_SlimeGlow
DrawPixelSafe $bf 12 5 $C_PurpleGlow; DrawPixelSafe $bf 21 7 $C_SlimeGlow
$bf.Save("$bldDir\Building_Farm.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bf.Dispose()

# Building_Training.png (32x32)
$bt = New-ImageCanvas 32 32 $C_Trans
FillCircle $bt 16 18 13 $C_DirtMid
DrawOutline $bt 4 6 23 23 $C_Void
# Training dummy
FillRect $bt 15 11 3 14 $C_DirtLight
FillRect $bt 12 13 9 7 $C_BoneMid
FillCircle $bt 16 7 4 $C_BoneWhite
DrawPixelSafe $bt 15 7 $C_Void; DrawPixelSafe $bt 17 7 $C_Void
FillRect $bt 10 14 13 2 $C_DirtLight
# Weapon rack & blood
FillRect $bt 6 15 2 10 $C_StoneLight
FillRect $bt 8 16 2 9 $C_CrimsonBright
DrawPixelSafe $bt 18 22 $C_CrimsonMid; DrawPixelSafe $bt 19 23 $C_CrimsonBright
$bt.Save("$bldDir\Building_Training.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bt.Dispose()

# Building_Construction.png (32x32)
$bco = New-ImageCanvas 32 32 $C_Trans
FillRect $bco 4 26 24 4 $C_DirtDark
FillRect $bco 6 6 2 24 $C_DirtLight
FillRect $bco 24 6 2 24 $C_DirtLight
FillRect $bco 15 12 2 18 $C_DirtLight
FillRect $bco 6 10 20 2 $C_DirtLight
FillRect $bco 6 20 20 2 $C_DirtLight
FillRect $bco 11 16 4 3 $C_StoneLight
FillRect $bco 18 23 6 6 $C_DirtMid
DrawPixelSafe $bco 20 25 $C_PurpleGlow
$bco.Save("$bldDir\Building_Construction.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bco.Dispose()

# --- 3. MONSTERS ---
# 1. Monster_ImpScrap.png (32x32)
$m1 = New-ImageCanvas 32 32 $C_Trans
# Bat wings
FillCircle $m1 8 13 4 $C_CrimsonDark
FillCircle $m1 24 13 4 $C_CrimsonDark
# Body & Head
FillRect $m1 12 15 8 9 $C_CrimsonMid
FillCircle $m1 16 12 5 $C_CrimsonMid
# Horns
DrawPixelSafe $m1 13 7 $C_BoneLight; DrawPixelSafe $m1 12 6 $C_BoneWhite
DrawPixelSafe $m1 19 7 $C_BoneLight; DrawPixelSafe $m1 20 6 $C_BoneWhite
# Eyes & Smile
DrawPixelSafe $m1 14 11 $C_GoldGlow; DrawPixelSafe $m1 18 11 $C_GoldGlow
DrawPixelSafe $m1 16 14 $C_BoneWhite
# Feet
DrawPixelSafe $m1 13 25 $C_StoneDark; DrawPixelSafe $m1 18 25 $C_StoneDark
# Tail
DrawPixelSafe $m1 21 21 $C_CrimsonBright; DrawPixelSafe $m1 22 19 $C_CrimsonBright; DrawPixelSafe $m1 23 17 $C_CrimsonGlow
$m1.Save("$monDir\Monster_ImpScrap.png", [System.Drawing.Imaging.ImageFormat]::Png)
$m1.Dispose()

# 2. Monster_MireSlime.png (32x32)
$m2 = New-ImageCanvas 32 32 $C_Trans
FillCircle $m2 16 19 9 $C_SlimeDark
FillCircle $m2 16 18 7 $C_SlimeMid
FillCircle $m2 16 17 5 $C_SlimeBright
DrawPixelSafe $m2 16 9 $C_SlimeBright; DrawPixelSafe $m2 16 8 $C_SlimeGlow
FillCircle $m2 16 19 2 $C_SlimeGlow
DrawPixelSafe $m2 16 19 $C_BoneWhite
# Eyes
DrawPixelSafe $m2 13 17 $C_Void; DrawPixelSafe $m2 13 16 $C_SlimeGlow
DrawPixelSafe $m2 19 17 $C_Void; DrawPixelSafe $m2 19 16 $C_SlimeGlow
$m2.Save("$monDir\Monster_MireSlime.png", [System.Drawing.Imaging.ImageFormat]::Png)
$m2.Dispose()

# 3. Monster_BonePup.png (32x32)
$m3 = New-ImageCanvas 32 32 $C_Trans
FillRect $m3 10 16 11 7 $C_BoneDark
FillRect $m3 11 17 9 2 $C_BoneMid
FillRect $m3 12 18 7 1 $C_BoneLight
DrawPixelSafe $m3 13 19 $C_BoneWhite; DrawPixelSafe $m3 15 19 $C_BoneWhite; DrawPixelSafe $m3 17 19 $C_BoneWhite
FillRect $m3 11 23 2 5 $C_BoneWhite
FillRect $m3 18 23 2 5 $C_BoneWhite
FillCircle $m3 20 13 5 $C_BoneMid
FillCircle $m3 20 13 4 $C_BoneWhite
FillRect $m3 22 14 4 3 $C_BoneWhite
DrawPixelSafe $m3 19 12 (ColorFromHex "#38bdf8") # Cyan soul eye
DrawPixelSafe $m3 9 19 $C_BoneLight; DrawPixelSafe $m3 8 17 $C_BoneWhite; DrawPixelSafe $m3 7 15 (ColorFromHex "#38bdf8")
$m3.Save("$monDir\Monster_BonePup.png", [System.Drawing.Imaging.ImageFormat]::Png)
$m3.Dispose()

# 4. Monster_GrimShadow.png (32x32)
$m4 = New-ImageCanvas 32 32 $C_Trans
FillCircle $m4 16 15 9 $C_PurpleDark
FillCircle $m4 16 15 6 $C_PurpleMid
FillCircle $m4 16 15 3 $C_CrimsonBright
DrawPixelSafe $m4 16 15 $C_CrimsonGlow; DrawPixelSafe $m4 16 14 $C_BoneWhite
# Tendrils
FillRect $m4 12 23 2 6 $C_PurpleDark
FillRect $m4 15 23 2 7 $C_PurpleMid
FillRect $m4 18 23 2 5 $C_PurpleDark
DrawPixelSafe $m4 8 11 $C_PurpleGlow; DrawPixelSafe $m4 24 13 $C_PurpleGlow
$m4.Save("$monDir\Monster_GrimShadow.png", [System.Drawing.Imaging.ImageFormat]::Png)
$m4.Dispose()

# 5. Monster_GoblinTinker.png (32x32)
$m5 = New-ImageCanvas 32 32 $C_Trans
FillRect $m5 7 14 5 7 $C_DirtDark
DrawPixelSafe $m5 8 14 $C_StoneLight
FillRect $m5 12 16 8 8 $C_DirtMid
FillRect $m5 13 24 2 5 $C_GoblinDark
FillRect $m5 17 24 2 5 $C_GoblinDark
FillCircle $m5 16 13 5 $C_GoblinMid
FillCircle $m5 16 13 4 $C_GoblinLight
DrawPixelSafe $m5 10 12 $C_GoblinEar; DrawPixelSafe $m5 9 11 $C_GoblinLight
DrawPixelSafe $m5 22 12 $C_GoblinEar; DrawPixelSafe $m5 23 11 $C_GoblinLight
DrawPixelSafe $m5 14 13 $C_GoldGlow; DrawPixelSafe $m5 18 13 $C_GoldGlow
DrawPixelSafe $m5 16 16 $C_BoneWhite
FillRect $m5 21 17 2 7 $C_StoneLight
FillRect $m5 20 16 4 2 $C_StoneMid
$m5.Save("$monDir\Monster_GoblinTinker.png", [System.Drawing.Imaging.ImageFormat]::Png)
$m5.Dispose()

# --- 4. UI ---
# UI_Panel_9Slice.png (32x32)
$up = New-ImageCanvas 32 32 (ColorFromHex "#12141c")
DrawOutline $up 0 0 31 31 $C_Void
DrawOutline $up 1 1 29 29 $C_StoneDark
DrawOutline $up 2 2 27 27 $C_StoneMid
DrawPixelSafe $up 2 2 $C_GoldBright; DrawPixelSafe $up 29 2 $C_GoldBright
DrawPixelSafe $up 2 29 $C_GoldBright; DrawPixelSafe $up 29 29 $C_GoldBright
$up.Save("$uiDir\UI_Panel_9Slice.png", [System.Drawing.Imaging.ImageFormat]::Png)
$up.Dispose()

# Icon_Gold.png (16x16)
$ig = New-ImageCanvas 16 16 $C_Trans
FillCircle $ig 8 8 5 $C_GoldDark
FillCircle $ig 8 8 4 $C_GoldBright
DrawPixelSafe $ig 7 7 $C_GoldGlow; DrawPixelSafe $ig 8 8 $C_BoneWhite
$ig.Save("$uiDir\Icon_Gold.png", [System.Drawing.Imaging.ImageFormat]::Png)
$ig.Dispose()

# Icon_Mana.png (16x16)
$im = New-ImageCanvas 16 16 $C_Trans
for ($d = -4; $d -le 4; $d++) {
    $span = 4 - [Math]::Abs($d)
    FillRect $im (8 - $span) (8 + $d) (2 * $span + 1) 1 $C_PurpleMid
}
for ($d = -2; $d -le 2; $d++) {
    $span = 2 - [Math]::Abs($d)
    FillRect $im (8 - $span) (8 + $d) (2 * $span + 1) 1 $C_PurpleGlow
}
DrawPixelSafe $im 8 8 $C_BoneWhite
$im.Save("$uiDir\Icon_Mana.png", [System.Drawing.Imaging.ImageFormat]::Png)
$im.Dispose()

# Icon_Food.png (16x16)
$if = New-ImageCanvas 16 16 $C_Trans
FillRect $if 7 7 2 6 $C_BoneLight
FillCircle $if 8 5 4 $C_SlimeMid
FillCircle $if 8 5 2 $C_SlimeGlow
DrawPixelSafe $if 8 5 $C_BoneWhite
$if.Save("$uiDir\Icon_Food.png", [System.Drawing.Imaging.ImageFormat]::Png)
$if.Dispose()

# Icon_Worker.png (16x16)
$iw = New-ImageCanvas 16 16 $C_Trans
FillCircle $iw 8 5 3 $C_CrimsonMid
DrawPixelSafe $iw 6 2 $C_BoneLight; DrawPixelSafe $iw 10 2 $C_BoneLight
DrawPixelSafe $iw 7 5 $C_GoldGlow; DrawPixelSafe $iw 9 5 $C_GoldGlow
FillRect $iw 5 9 7 5 $C_StoneDark
$iw.Save("$uiDir\Icon_Worker.png", [System.Drawing.Imaging.ImageFormat]::Png)
$iw.Dispose()

Write-Host "All procedural pixel art generated successfully!"
