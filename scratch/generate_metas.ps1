$baseDir = "C:\Users\lookm\Documents\GitHub\Rumors_Crown"

function New-Guid32 {
    return [System.Guid]::NewGuid().ToString("N")
}

function Ensure-FolderMeta($path) {
    $metaPath = "$path.meta"
    if (!(Test-Path $metaPath)) {
        $guid = New-Guid32
        $content = @"
fileFormatVersion: 2
guid: $guid
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"@
        Set-Content -Path $metaPath -Value $content -Encoding ASCII
    }
}

Ensure-FolderMeta "$baseDir\Assets\Art"
Ensure-FolderMeta "$baseDir\Assets\Art\Sprites"
Ensure-FolderMeta "$baseDir\Assets\Art\Sprites\Buildings"
Ensure-FolderMeta "$baseDir\Assets\Art\Sprites\Monsters"
Ensure-FolderMeta "$baseDir\Assets\Art\Sprites\Tiles"
Ensure-FolderMeta "$baseDir\Assets\Art\Sprites\UI"
Ensure-FolderMeta "$baseDir\Assets\Scripts"
Ensure-FolderMeta "$baseDir\Assets\Scripts\Core"
Ensure-FolderMeta "$baseDir\Assets\Scripts\World"
Ensure-FolderMeta "$baseDir\Assets\Scripts\Buildings"
Ensure-FolderMeta "$baseDir\Assets\Scripts\Monsters"
Ensure-FolderMeta "$baseDir\Assets\Scripts\UI"
Ensure-FolderMeta "$baseDir\Assets\Scripts\Editor"

$pngs = Get-ChildItem -Path "$baseDir\Assets\Art\Sprites" -Recurse -Filter *.png
foreach ($file in $pngs) {
    $metaPath = "$($file.FullName).meta"
    if (!(Test-Path $metaPath)) {
        $guid = New-Guid32
        $isIcon = $file.Name.StartsWith("Icon_")
        $ppu = if ($isIcon) { 16 } else { 32 }
        $borderStr = if ($file.Name -match "9Slice") { "border: {x: 4, y: 4, z: 4, w: 4}" } else { "border: {x: 0, y: 0, z: 0, w: 0}" }

        $metaContent = @"
fileFormatVersion: 2
guid: $guid
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
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
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsToUnits: $ppu
  $borderStr
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationMethod: 0
  spriteTessellationDetail: -1
  spriteGeometrySubdivision: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    customData: 
    physicsShape: []
    bones: []
    spriteID: 
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    spriteCustomMetadata:
      entries: []
    nameFileIdTable: {}
  mipmapLimitGroupName: 
  pSDRemoveMatte: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"@
        Set-Content -Path $metaPath -Value $metaContent -Encoding ASCII
    }
}
Write-Host "All sprite and folder meta files created successfully!"
