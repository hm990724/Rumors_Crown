import os
import uuid

BASE_DIR = r"c:\Users\lookm\Documents\GitHub\Rumors_Crown"

META_TEMPLATE = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: {ppu}
  spriteBorder: {border}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationMethod: 0
  spriteTessellationDetail: -1
  spriteGeometrySubdivision: -1
  textureType: 8
  textureShape: 1
  singleDimension: 0
  flipbook: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
"""

FOLDER_META_TEMPLATE = """fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

def get_existing_guid(meta_path):
    if os.path.exists(meta_path):
        try:
            with open(meta_path, "r", encoding="utf-8") as f:
                for line in f:
                    if line.startswith("guid:"):
                        g = line.split(":", 1)[1].strip()
                        if len(g) >= 32:
                            return g
        except:
            pass
    return uuid.uuid4().hex

def ensure_folder_meta(folder_path):
    meta_path = folder_path + ".meta"
    if not os.path.exists(meta_path) or os.path.getsize(meta_path) == 0:
        g = get_existing_guid(meta_path)
        with open(meta_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(FOLDER_META_TEMPLATE.format(guid=g))

def sync():
    targets = [
        os.path.join(BASE_DIR, "Assets", "Resources"),
        os.path.join(BASE_DIR, "Assets", "Art"),
    ]

    for t in targets:
        for root, dirs, files in os.walk(t):
            ensure_folder_meta(root)
            for d in dirs:
                ensure_folder_meta(os.path.join(root, d))
            for f in files:
                if f.endswith(".png"):
                    png_path = os.path.join(root, f)
                    meta_path = png_path + ".meta"
                    guid = get_existing_guid(meta_path)
                    
                    is_monster = "Monsters" in root
                    is_9slice = "9Slice" in f
                    ppu = 32 if is_monster else 16
                    border = "{x: 6, y: 6, z: 6, w: 6}" if is_9slice else "{x: 0, y: 0, z: 0, w: 0}"
                    
                    content = META_TEMPLATE.format(guid=guid, ppu=ppu, border=border)
                    with open(meta_path, "w", encoding="utf-8", newline="\n") as mf:
                        mf.write(content)

    print("All folder and texture metas synchronized with Point filter & exact PPU!")

if __name__ == "__main__":
    sync()
