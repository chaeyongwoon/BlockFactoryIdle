using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIChangeWindowEditor : EditorWindow
{
    const int WIDTH = 500;
    const int HEIGHT = 100;

    public Define.ObjectType[] _objectType;
    public string[] _objectName;
    string _currentScript;
    string _scriptName;
    public void Init(Define.ObjectType[] objectType, string[] objectName, string currentScript, string scriptName)
    {
        _scriptName = scriptName;
        _objectName = objectName;
        _currentScript = currentScript;
        _objectType = objectType;

        Rect main = EditorGUIUtility.GetMainWindowPosition();
        Rect pos = this.position;
        pos.width = WIDTH;
        pos.height = HEIGHT;
        float centerWidth = (main.width - pos.width) * 0.5f;
        float centerHeight = (main.height - pos.height) * 0.5f;
        pos.x = main.x + centerWidth;
        pos.y = main.y + centerHeight;
        this.Show();
        this.position = pos;
    }

    Rect buttonRect;
    void OnGUI()
    {
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("저장 되지 않은 데이터가 있습니다", EditorStyles.boldLabel);
            GUILayout.Label("저장하시겠습니까", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            //바뀐 목록 보여주기

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;
            if (GUILayout.Button("네", GUILayout.Width(200)))
            {
                RefreshScript();
                this.Close();
            }
            GUI.color = Color.white;
            if (GUILayout.Button("아니오", GUILayout.Width(200)))
            {
                this.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();



            if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
        }
    }

    private void RefreshScript()
    {
        var allLines = File.ReadAllLines(AssetDatabase.GUIDToAssetPath(_currentScript)).ToList();

        var awakePosition = allLines.FindIndex(f => f.Contains("Awake()"));

        var insertList = new List<string>();

        //만약 Awake 없으면 만들어줌
        if (awakePosition < 0)
        {
            awakePosition = allLines.FindIndex(f => f.Contains($"public class {_scriptName}")) + 2;
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
                startPosition = allLines.FindIndex(f => f.Contains($"public class {_scriptName}")) + 2;
            }

            //모든 애들 돌면서 타입이랑 같으면 insertlist 넣어주기            
            for (int j = 0; j < _objectType.Length; j++)
            {
                //타입이랑 겹치면
                if (typeName == _objectType[j].ToString())
                {
                    insertList.Add($"        {_objectName[j]},");
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
        File.WriteAllLines(AssetDatabase.GUIDToAssetPath(_currentScript), allLines, System.Text.Encoding.UTF8);
        AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(_currentScript));
    }
}