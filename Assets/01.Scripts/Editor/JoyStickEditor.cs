using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JoyStickController))]
public class JoyStickEditor : Editor
{
    JoyStickController _joystickController;

    private void OnEnable()
    {
        _joystickController = target as JoyStickController;
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        UseProperty("joyStickMethod");
        if (_joystickController.joyStickMethod != JoyStickMethod.DoNotUse)
        {
            EditorGUILayout.Space();
            UseProperty("JoyStickBound");
            EditorGUILayout.Space();

            SerializedProperty tps = serializedObject.FindProperty("MoveObjectRig");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tps, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (_joystickController.MoveObjectRig != null)
                {
                    _joystickController.SetRigidBody();
                    Debug.Log("RigidBody 값을 변경합니다");
                }
            }
        }
        if (_joystickController.MoveObjectRig != null)
        {
            switch (_joystickController.joyStickMethod)
            {
                case JoyStickMethod.Fixed:
                    _joystickController.Speed = EditorGUILayout.FloatField(new GUIContent("Speed", "속도"), _joystickController.Speed);
                    _joystickController.Threshold = EditorGUILayout.FloatField(new GUIContent("Threshold", "조이스틱 최소 움직임 조건"), _joystickController.Threshold);
                    _joystickController.UseAccelerate = EditorGUILayout.Toggle(new GUIContent("UseAccelerate", "가속도를 사용"), _joystickController.UseAccelerate);
                    if (_joystickController.UseAccelerate)
                    {
                        _joystickController.Accelerate = EditorGUILayout.FloatField(new GUIContent("Accelerate", "가속도"), _joystickController.Accelerate);
                    }
                    break;
                case JoyStickMethod.HardFixed:
                    _joystickController.Speed = EditorGUILayout.FloatField(new GUIContent("Speed", "속도"), _joystickController.Speed);
                    _joystickController.Threshold = EditorGUILayout.FloatField(new GUIContent("Threshold", "조이스틱 최소 움직임 조건"), _joystickController.Threshold);
                    _joystickController.UseAccelerate = EditorGUILayout.Toggle(new GUIContent("UseAccelerate", "가속도를 사용"), _joystickController.UseAccelerate);
                    if (_joystickController.UseAccelerate)
                    {
                        _joystickController.Accelerate = EditorGUILayout.FloatField(new GUIContent("Accelerate", "가속도"), _joystickController.Accelerate);
                    }
                    break;
                case JoyStickMethod.Follow:
                    _joystickController.Speed = EditorGUILayout.FloatField(new GUIContent("Speed", "속도"), _joystickController.Speed);
                    _joystickController.Threshold = EditorGUILayout.FloatField(new GUIContent("Threshold", "조이스틱 최소 움직임 조건"), _joystickController.Threshold);
                    _joystickController.UseAccelerate = EditorGUILayout.Toggle(new GUIContent("UseAccelerate", "가속도를 사용"), _joystickController.UseAccelerate);
                    if (_joystickController.UseAccelerate)
                    {
                        _joystickController.Accelerate = EditorGUILayout.FloatField(new GUIContent("Accelerate", "가속도"), _joystickController.Accelerate);
                    }
                    break;
                case JoyStickMethod.SlowFollow:
                    _joystickController.Speed = EditorGUILayout.FloatField(new GUIContent("Speed", "속도"), _joystickController.Speed);
                    _joystickController.Threshold = EditorGUILayout.FloatField(new GUIContent("Threshold", "조이스틱 최소 움직임 조건"), _joystickController.Threshold);
                    _joystickController.UseAccelerate = EditorGUILayout.Toggle(new GUIContent("UseAccelerate", "가속도를 사용"), _joystickController.UseAccelerate);
                    if (_joystickController.UseAccelerate)
                    {
                        _joystickController.Accelerate = EditorGUILayout.FloatField(new GUIContent("Accelerate", "가속도"), _joystickController.Accelerate);
                    }
                    break;
                case JoyStickMethod.RunningGame:
                    _joystickController.Speed = EditorGUILayout.FloatField(new GUIContent("Speed", "속도"), _joystickController.Speed);
                    _joystickController.XBound = EditorGUILayout.FloatField(new GUIContent("XBound", "X 최대값"), _joystickController.XBound);
                    _joystickController.AutoRun = EditorGUILayout.Toggle(new GUIContent("AutoRun", "X 최대값"), _joystickController.AutoRun);
                    _joystickController.X_Sensitivity = EditorGUILayout.FloatField(new GUIContent("X_Sensitivity", "민감도"), _joystickController.X_Sensitivity);
                    _joystickController.X_Acceletor = EditorGUILayout.FloatField(new GUIContent("X_Acceletor", "가속도"), _joystickController.X_Acceletor);
                    break;
            }
        }
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    void UseProperty(string propertyName)
    {
        SerializedProperty tps = serializedObject.FindProperty(propertyName);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(tps, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }


    [MenuItem("MonLibrary/JoyStick")]
    public static void MakeJoyStick()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("JoyStickCanvas")) as GameObject;
        go.name = "JoyStickCanvas";
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
