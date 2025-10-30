using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelSetupTool : EditorWindow
{
    [MenuItem("Tools/Adjust Obstacles")]
    static void AdjustObstacles()
    {
        GameObject levelObject = GameObject.Find("Level");

        foreach (Transform trans in levelObject.transform)
        {
            GameObject obj = trans.gameObject;
            bool adjusted = false;
            Data data = obj.GetComponent<Data>();

            if ((obj.name.StartsWith("Ice") || obj.name.StartsWith("Booster")) && data != null && data.trans != null)
            {
                float height = 0.4f / obj.transform.localScale.y; Debug.Log(height);
                data.trans.localScale = new Vector3(1f, height, 1f);
                data.trans.localPosition = new Vector3(0f, (1 - height) / 2f, 0f);
                adjusted = true;
            }
            else if (obj.name.StartsWith("Glider"))
            {
                Transform areaTrans = obj.transform.Find("Area");
                Transform baseTrans = obj.transform.Find("Base");

                float height = 0.5f / obj.transform.localScale.x;
                baseTrans.localScale = new Vector3(height, 1f, 1f);
                baseTrans.localPosition = new Vector3(-0.5f, 0f, 0f);
                adjusted = true;
            }

            if (adjusted) Undo.RecordObject(obj.transform, "Adjust Components");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Adjust Obstacles complete and saved.");
    }
}