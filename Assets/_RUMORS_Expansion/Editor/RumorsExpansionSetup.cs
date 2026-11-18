#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rumors.Expansion.Editor
{
    public static class RumorsExpansionSetup
    {
        [MenuItem("RUMORS/Expansion/Create Isolated Demo Scene")]
        public static void Create()
        {
            // New scene on purpose: does not mutate the existing DemonCastle scene.
            var scene=EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Single);
            var go=new GameObject("RUMORS Expansion Simulation");
            go.AddComponent<RumorsWorldClock>();
            EditorSceneManager.MarkSceneDirty(scene);
            string path="Assets/_RUMORS_Expansion/RumorsExpansionDemo.unity";
            EditorSceneManager.SaveScene(scene,path);
            Debug.Log("Created isolated expansion demo scene: "+path);
        }
    }
}
#endif
