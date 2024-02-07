using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

enum ShowType
{
    All,
    NotAssigned,
    Assigned,
    GameObject,
    TextMesh,
    Image,
    Button,
    Text,
    None,

}


[CustomEditor(typeof(UI_Base), true)]
public class UIBaseEditor : Editor
{
    ShowType showObjectType;
    Define.ObjectType[] objectType;
    string[] objectName;
    UI_Base _uiBase;
    bool isChanged = false;
    string searchText;

    Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
    private void OnEnable()
    {
        _uiBase = target as UI_Base;

        //전체를 읽고 배열 만들어두기
        objectCount = 0;
        GetObjectCount(_uiBase.transform);
        objectType = new Define.ObjectType[objectCount];
        objectName = new string[objectCount];

        //스크립트에서 objectType 넣어주기
        GetInfoFromScript();
    }
    void OnDisable()
    {
        //저장할 것이 있으면
        if (isChanged)
        {
            //저장하냐고 물어봄
            UIChangeWindowEditor window = EditorWindow.CreateInstance<UIChangeWindowEditor>();
            window.Init(objectType, objectName, currentScript[0], target.name);
        }
    }

    private int objectCount;
    void GetObjectCount(Transform tr)
    {
        nameToIndex[tr.name] = objectCount;
        objectCount++;
        for (int i = 0; i < tr.childCount; i++)
        {
            GetObjectCount(tr.GetChild(i));
        }
    }

    void FindChild(Transform tr)
    {
        //show type과 같은경우에만 실행
        if (tr != _uiBase.transform && showObjectType == ShowType.All || showObjectType.ToString() == objectType[nameToIndex[tr.name]].ToString() ||
        showObjectType == ShowType.Assigned && objectType[nameToIndex[tr.name]] != Define.ObjectType.NotAssigned)
        {
            if (nameToIndex.TryGetValue(tr.name, out int nameIndex))
            {
                //search
                if (searchText == null || tr.name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    objectName[nameIndex] = tr.name;
                    if (objectType[nameIndex] == Define.ObjectType.NotAssigned)
                        GUI.color = Color.gray;
                    else
                    {
                        GUI.color = Color.white;
                    }

                    EditorGUI.BeginChangeCheck();
                    objectType[nameIndex] = (Define.ObjectType)EditorGUILayout.EnumPopup(tr.name, objectType[nameIndex]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        //괄호가 있거나 그 오브젝트에 컴포넌트가 없으면 NotAssigned 으로 돌아감
                        if (Regex.IsMatch(tr.name, @"[^a-zA-Z0-9_]"))
                        {
                            Debug.Log($"<color=red>{tr.name}</color> 이름 예쁘게 만들고 하세요 (띄어쓰기, 특수문자 금지)");
                            objectType[nameIndex] = Define.ObjectType.NotAssigned;
                        }
                        else
                        {
                            bool isComponentExist = false;
                            switch (objectType[nameIndex])
                            {
                                case Define.ObjectType.Button:
                                    if (tr.GetComponent<Button>() != null) isComponentExist = true;
                                    break;
                                case Define.ObjectType.GameObject:
                                    isComponentExist = true;
                                    break;
                                case Define.ObjectType.Image:
                                    if (tr.GetComponent<Image>() != null) isComponentExist = true;
                                    break;
                                case Define.ObjectType.NotAssigned:
                                    isComponentExist = true;
                                    break;
                                case Define.ObjectType.Text:
                                    if (tr.GetComponent<Text>() != null) isComponentExist = true;
                                    break;
                                case Define.ObjectType.TextMesh:
                                    if (tr.GetComponent<TMPro.TextMeshProUGUI>() != null) isComponentExist = true;
                                    break;
                            }

                            if (!isComponentExist)
                            {
                                if (objectType[nameIndex] != Define.ObjectType.NotAssigned)
                                {
                                    isChanged = true;
                                }
                                Debug.Log($"<color=red>{tr.name}</color> 여긴 그 컴포넌트가 없어요");
                                objectType[nameIndex] = Define.ObjectType.NotAssigned;
                            }
                            else
                            {
                                isChanged = true;
                                Debug.Log($"Change <color=cyan>{tr.name}</color> to {objectType[nameIndex]}");
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < tr.childCount; i++)
        {
            FindChild(tr.GetChild(i));
        }
    }
    string[] currentScript;
    private void RefreshScript()
    {
        var allLines = File.ReadAllLines(AssetDatabase.GUIDToAssetPath(currentScript[0])).ToList();

        var awakePosition = allLines.FindIndex(f => f.Contains("Awake()"));

        var insertList = new List<string>();

        //만약 Awake 없으면 만들어줌
        if (awakePosition < 0)
        {
            awakePosition = allLines.FindIndex(f => f.Contains($"public class {target.name}")) + 2;
            insertList.Add("    private void Awake()");
            insertList.Add("    {");
            insertList.Add("    }");
        }
        allLines.InsertRange(awakePosition, insertList);

        //모든 타입마다 enum 만들기 none 제외
        for (int i = 1; i < Enum.GetValues(typeof(Define.ObjectType)).Length; i++)
        {

            string typeName = Enum.GetNames(typeof(Define.ObjectType))[i];
            insertList.Clear();
            insertList.Add($"    enum {typeName}s");
            insertList.Add("    {");

            var startPosition = allLines.FindIndex(f => f.Contains($"enum {typeName}s"));
            var endPosition = 0;
            //이미 있는 경우 지워주기
            if (startPosition >= 0)
            {
                endPosition = allLines.FindIndex(startPosition, f => f.Contains("}"));
                allLines.RemoveRange(startPosition, endPosition - startPosition + 1);
            }
            //없는경우 startposition 잡아주기
            else
            {
                startPosition = allLines.FindIndex(f => f.Contains($"public class {target.name}")) + 2;
            }

            //모든 애들 돌면서 타입이랑 같으면 insertlist 넣어주기            
            for (int j = 0; j < objectType.Length; j++)
            {
                //타입이랑 겹치면
                if (typeName == objectType[j].ToString())
                {
                    insertList.Add($"        {objectName[j]},");
                }
            }

            insertList.Add("    }");

            //뭔가 있으면 넣어주기
            if (insertList.Count > 3)
            {
                allLines.InsertRange(startPosition, insertList);
            }


            //바인드
            int bindPosition;
            string bindString;
            if (typeName == Define.ObjectType.GameObject.ToString())
            {
                bindString = $"        Bind<GameObject>(typeof({typeName}s));";
                bindPosition = allLines.FindIndex(f => f.Contains($"Bind<GameObject>(typeof({typeName}s));"));
            }
            else if (typeName == Define.ObjectType.TextMesh.ToString())
            {
                bindString = $"        Bind<TMPro.TextMeshProUGUI>(typeof({typeName}s));";
                bindPosition = allLines.FindIndex(f => f.Contains($"Bind<TMPro.TextMeshProUGUI>(typeof({typeName}s));"));
            }
            else
            {
                bindString = $"        Bind<UnityEngine.UI.{typeName}>(typeof({typeName}s));";
                bindPosition = allLines.FindIndex(f => f.Contains($"Bind<UnityEngine.UI.{typeName}>(typeof({typeName}s));"));
            }

            //먼저 삭제
            if (bindPosition >= 0)
                allLines.RemoveAt(bindPosition);

            //뭔가 있으면 바인드 해주기
            if (insertList.Count > 3)
            {
                startPosition = allLines.FindIndex(f => f.Contains("Awake()"));
                if (startPosition > 0)
                {
                    startPosition += 2;
                }
                allLines.Insert(startPosition, bindString);
            }
        }
        File.WriteAllLines(AssetDatabase.GUIDToAssetPath(currentScript[0]), allLines, System.Text.Encoding.UTF8);
        AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(currentScript[0]));
    }

    private void GetInfoFromScript()
    {
        currentScript = AssetDatabase.FindAssets($"t:Script {target.name}");

        var allLines = File.ReadAllLines(AssetDatabase.GUIDToAssetPath(currentScript[0])).ToList();

        //각 enum 찾아서 넣어주기
        for (int i = 1; i < Enum.GetValues(typeof(Define.ObjectType)).Length; i++)
        {
            string typeName = Enum.GetNames(typeof(Define.ObjectType))[i];
            var startPosition = allLines.FindIndex(f => f.Contains($"enum {typeName}s"));
            if (startPosition >= 0)
            {
                var endPosition = allLines.FindIndex(startPosition, f => f.Contains("}"));

                for (int j = startPosition + 2; j < endPosition; j++)
                {
                    string objectName = allLines[j].Replace(',', ' ').Trim();
                    if (nameToIndex.TryGetValue(objectName, out int index))
                    {
                        objectType[index] = (Define.ObjectType)i;
                    }
                    else
                    {
                        Debug.Log($"{target.name} 스크립트에는 {objectName}가 있지만 하이어라키에는 없어요");
                    }
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying) return;

        EditorGUILayout.Space();

        showObjectType = (ShowType)EditorGUILayout.EnumPopup("Show Hierarchy", showObjectType);


        // GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"), GUILayout.Height(50));
        GUILayout.BeginHorizontal();
        searchText = GUILayout.TextField(searchText, GUI.skin.FindStyle("ToolbarSeachTextField"));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            searchText = "";
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (isChanged)
        {
            GUI.color = Color.green;

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("ReBind Hierarchy", GUILayout.Width(300), GUILayout.Height(50)))
            {
                RefreshScript();
                isChanged = false;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

        }

        //모든 게임오브젝트 나열하기
        FindChild(_uiBase.transform);

        if (isChanged)
        {
            GUI.color = Color.green;

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ReBind Hierarchy", GUILayout.Width(300), GUILayout.Height(50)))
            {
                RefreshScript();
                isChanged = false;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
