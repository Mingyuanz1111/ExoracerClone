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
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (obj.name.StartsWith("Platform") || obj.name.StartsWith("Ice") || obj.name.StartsWith("Booster"))
            {
                Transform hitboxTrans = obj.transform.Find("Hitbox");
                Transform blockTrans = obj.transform.Find("Block");
                SpriteRenderer srBlock = blockTrans.gameObject.GetComponent<SpriteRenderer>();

                srBlock.size = (Vector2)(hitboxTrans.localScale);

                if (obj.name.StartsWith("Platform")) continue;

                Transform topTrans = obj.transform.Find("Top");
                SpriteRenderer srTop = topTrans.gameObject.GetComponent<SpriteRenderer>();

                Data data = hitboxTrans.gameObject.GetComponent<Data>();

                float height;
                if (obj.name.StartsWith("Booster")) height = 0.4f;
                else height = 0.8f;

                srTop.size = new Vector3((data.dir == "L")?(-1f):(1f) * hitboxTrans.localScale.x, height, 1f);
                topTrans.localPosition = new Vector3(0, (hitboxTrans.localScale.y - height) / 2f, 1f);
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