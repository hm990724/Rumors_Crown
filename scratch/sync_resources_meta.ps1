$res = "Assets\Resources"
$guid = [System.Guid]::NewGuid().ToString("N")
if (!(Test-Path "$res.meta")) {
    Set-Content -Path "$res.meta" -Value "fileFormatVersion: 2`nguid: $guid`nfolderAsset: yes`nDefaultImporter:`n  externalObjects: {}`n"
}
$sprites = Get-ChildItem -Path "Assets\Art\Sprites" -Recurse -Filter *.meta
foreach ($s in $sprites) {
    $rel = $s.FullName.Substring((Get-Item "Assets\Art\Sprites").FullName.Length)
    $dest = "Assets\Resources\Sprites$rel"
    Copy-Item -Path $s.FullName -Destination $dest -Force
}
Write-Host "Resources metas synced!"
