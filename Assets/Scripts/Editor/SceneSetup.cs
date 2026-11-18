#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Rumors.Core;
using Rumors.World;

namespace Rumors.Editor
{
    public static class SceneSetup
    {
        [MenuItem("RUMORS/Setup Scene")]
        public static void CreateOrSetupScene()
        {
            // Camera
            var camObj = GameObject.FindWithTag("MainCamera");
            if (camObj == null)
            {
                camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                var cam = camObj.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = 4.5f;
                cam.backgroundColor = new Color(0.08f, 0.07f, 0.12f, 1f);
                cam.clearFlags = CameraClearFlags.SolidColor;
                camObj.transform.position = new Vector3(0f, 0f, -10f);
            }

            // GameBootstrapper
            if (Object.FindAnyObjectByType<GameBootstrapper>() == null)
            {
                var go = new GameObject("[GameBootstrapper]");
                go.AddComponent<GameBootstrapper>();
                Debug.Log("[RUMORS] GameBootstrapper added to scene.");
            }

            Debug.Log("[RUMORS] Scene setup complete. Press Play to start.");
        }

        [MenuItem("RUMORS/Run Tests")]
        public static void RunVerificationTests()
        {
            bool ok = true;

            // Grid size
            if (WorldGrid.GridWidth != 25 || WorldGrid.GridHeight != 25)
            {
                Debug.LogError($"[RUMORS] Grid size mismatch: expected 25×25, got {WorldGrid.GridWidth}×{WorldGrid.GridHeight}");
                ok = false;
            }

            // Castle center
            if (WorldGrid.CastleGridPos != new Vector2Int(12, 12))
            {
                Debug.LogError($"[RUMORS] CastleGridPos mismatch: expected (12,12), got {WorldGrid.CastleGridPos}");
                ok = false;
            }

            // Verify Sprite Assets
            string[] testSprites = new string[]
            {
                "Tiles/Tile_Ground_DarkGrass",
                "Tiles/Tile_Ground_Dirt",
                "Tiles/Tile_Cobblestone",
                "Tiles/Tile_Castle_Floor",
                "Tiles/Tile_Ground_Stone",
                "Tiles/Tile_Ground_Swamp",
                "Tiles/Tile_Highlight",
                "Buildings/Building_Castle",
                "Buildings/Building_Mine",
                "Buildings/Building_Farm",
                "Buildings/Building_Dormitory",
                "Buildings/Building_Warehouse",
                "Buildings/Building_ManaTower",
                "Buildings/Building_Training",
                "Props/Prop_Tree",
                "Props/Prop_Bush",
                "Props/Prop_Torch",
                "Props/Prop_Barrel",
                "Props/Prop_Wall",
                "Monsters/Monster_ImpScrap",
                "Monsters/Monster_MireSlime",
                "Monsters/Monster_BonePup",
                "Monsters/Monster_GrimShadow",
                "Monsters/Monster_GoblinTinker",
                "UI/UI_Panel_9Slice",
                "UI/Icon_Gold",
                "UI/Icon_Food",
                "UI/Icon_Mana"
            };

            foreach (var sp in testSprites)
            {
                var s = SpriteRegistry.Load(sp);
                if (s == null)
                {
                    Debug.LogError($"[RUMORS] Failed to load sprite: Resources/{sp}");
                    ok = false;
                }
            }

            if (ok) Debug.Log("[RUMORS] All tests and sprite verifications passed successfully! ✓");
        }

        [MenuItem("RUMORS/Reimport All Sprites")]
        public static void ReimportAllSprites()
        {
            AssetDatabase.StartAssetEditing();
            string[] folders = new string[] { "Assets/Resources", "Assets/Art/Sprites" };
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", folders);
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
            Debug.Log($"[RUMORS] Successfully reimported {guids.Length} sprites with Point filter.");
        }

        [MenuItem("RUMORS/Open Project Folder")]
        public static void OpenProjectFolder()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
        }
    }
}
#endif
