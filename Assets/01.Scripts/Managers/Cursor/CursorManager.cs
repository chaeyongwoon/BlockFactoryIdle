
using UnityEngine;
using UnityEditor;
using System.Collections;
public class CursorManager : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
    bool isShowCursor = false;
    [System.Serializable]
    struct CursorSet
    {
        public Texture2D cursorTex;
        public Texture2D cursorClickedTex;
        public Vector2 hotSpot;
    }
    [SerializeField]
    CursorSet[] _cursorSet;
    int currentCursorIndex = 0;
    bool isNumClicked = false;
    // public CursorType _cursorType = new CursorType();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnClickNum(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnClickNum(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnClickNum(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnClickNum(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            OnClickNum(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            OnClickNum(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            OnClickNum(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            OnClickNum(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            OnClickNum(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            OnClickNum(9);
        }
        if (isNumClicked)
        {
            isNumClicked = false;
            Cursor.SetCursor(isShowCursor == true ? _cursorSet[currentCursorIndex].cursorTex : null, _cursorSet[currentCursorIndex].hotSpot, CursorMode.Auto);
        }

        if (Input.GetMouseButtonDown(0) && isShowCursor)
        {
            Cursor.SetCursor(_cursorSet[currentCursorIndex].cursorClickedTex, _cursorSet[currentCursorIndex].hotSpot, CursorMode.Auto);
        }
        if (Input.GetMouseButtonUp(0) && isShowCursor)
        {
            Cursor.SetCursor(_cursorSet[currentCursorIndex].cursorTex, _cursorSet[currentCursorIndex].hotSpot, CursorMode.Auto);
        }
    }

    void OnClickNum(int num)
    {
        if (currentCursorIndex != num)
        {
            currentCursorIndex = num;
            if (isShowCursor)
                isShowCursor = false;
        }
        isShowCursor = !isShowCursor;
        isNumClicked = true;
    }
#endif
}
#if UNITY_EDITOR
public class CursorEditor
{
    [MenuItem("MonLibrary/CursorManager")]
    public static void MakeCursor()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("CursorManager")) as GameObject;
        go.name = "@CursorManager";
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
#endif