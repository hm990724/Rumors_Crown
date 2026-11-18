using UnityEngine;
using Rumors.Buildings;
using Rumors.Monsters;
using Rumors.World;
using Rumors.UI;

namespace Rumors.Core
{
    /// <summary>
    /// Auto-bootstraps on first scene load. Creates all manager GameObjects.
    /// Uses legacy Input API (activeInputHandler = Both).
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBootstrap()
        {
            if (FindAnyObjectByType<GameBootstrapper>() != null) return;
            var root = new GameObject("[GameBootstrapper]");
            DontDestroyOnLoad(root);
            root.AddComponent<GameBootstrapper>();
        }

        private void Awake()
        {
            SetupCamera();
            SetupCoreManagers();
            SetupWorld();
            SetupUI();
        }

        private void SetupCamera()
        {
            var mainCam = Camera.main;
            if (mainCam == null)
            {
                var camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            mainCam.orthographic = true;
            mainCam.orthographicSize = 4.5f;
            mainCam.backgroundColor = new Color(0.08f, 0.07f, 0.12f, 1f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.transform.position = new Vector3(0f, 0f, -10f);

            if (mainCam.GetComponent<CameraController>() == null)
                mainCam.gameObject.AddComponent<CameraController>();
        }

        private void SetupCoreManagers()
        {
            Ensure<GameManager>("GameManager");
            Ensure<TimeManager>("TimeManager");
            Ensure<ResourceManager>("ResourceManager");
            Ensure<SaveLoadManager>("SaveLoadManager");
            Ensure<RumorManager>("RumorManager");
            Ensure<DemonLordManager>("DemonLordManager");
            Ensure<HeroManager>("HeroManager");
        }

        private void SetupWorld()
        {
            // Ensure 2D Global Light if running with Universal 2D
            try
            {
                var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
                if (lightType != null && FindAnyObjectByType(lightType) == null)
                {
                    var lightGo = new GameObject("Global Light 2D");
                    lightGo.AddComponent(lightType);
                }
            }
            catch { }

            if (WorldGrid.Instance == null)
            {
                var wgObj = new GameObject("[WorldGrid]");
                wgObj.AddComponent<WorldGrid>();
                wgObj.AddComponent<TilemapVisualizer>();
            }

            if (BuildingManager.Instance == null)
            {
                var bmObj = new GameObject("[BuildingManager]");
                bmObj.AddComponent<BuildingManager>();
            }

            if (MonsterManager.Instance == null)
            {
                var mmObj = new GameObject("[MonsterManager]");
                mmObj.AddComponent<MonsterManager>();
            }

            if (WorldClickController.Instance == null)
            {
                var wccObj = new GameObject("[WorldClickController]");
                wccObj.AddComponent<WorldClickController>();
            }
        }

        private void SetupUI()
        {
            if (UIManager.Instance == null)
            {
                var uiObj = new GameObject("[UIManager]");
                uiObj.AddComponent<UIManager>();
            }
        }

        private static T Ensure<T>(string name) where T : MonoBehaviour
        {
            var found = FindAnyObjectByType<T>();
            if (found != null) return found;
            var go = new GameObject(name);
            DontDestroyOnLoad(go);
            return go.AddComponent<T>();
        }
    }
}
