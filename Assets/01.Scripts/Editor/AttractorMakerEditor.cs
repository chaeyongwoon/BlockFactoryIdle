using System.IO;
using UnityEngine;
using UnityEditor;

public class UIAttractorMakeEditor
{
    [MenuItem("MonLibrary/Particle Attractor")]
    static void CreateUIAttractor()
    {
        if (Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject) && Selection.gameObjects.Length == 1)
        {
            GameObject go = GameObject.Instantiate(Resources.Load("UIAttractor"), Selection.activeGameObject.transform) as GameObject;
            go.name = "UIAttractor";

            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            //Prefab이면 Prefab저장
            if (prefabStage != null)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
            //Scene이면 Scene저장
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }
        else
        {
            Debug.LogError("Hierarchy에서 부모 오브젝트를 선택 한 후에 클릭하세요!");
        }
    }
}
