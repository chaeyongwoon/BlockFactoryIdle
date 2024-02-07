using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

[InitializeOnLoad]
public class CheckInitEditor : Editor
{
    private static int maxLayers = 31;
    static AddRequest Request;

    static CheckInitEditor()
    {
        //레이어 생성
        if (LayerMask.NameToLayer("Walkable") == -1)
        {
            CreateLayer("Walkable");
        }

        //2d sprite 다운, Cinemachine 다운
        Request = Client.Add("com.unity.2d.sprite");
        EditorApplication.update += Progress2DSprite;
    }
    static void Progress2DSprite()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= Progress2DSprite;

            //Cinemachine 다운
            Request = Client.Add("com.unity.cinemachine");
            EditorApplication.update += ProgressCine;

        }
    }
    static void ProgressCine()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= ProgressCine;
        }
    }

    /// <summary>
    /// 레이어 추가
    /// </summary>
    /// <returns><c>true</c>, if layer was added, <c>false</c> otherwise.</returns>
    /// <param name="layerName">Layer name.</param>
    public static bool CreateLayer(string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        if (!PropertyExists(layersProp, 0, maxLayers, layerName))
        {
            SerializedProperty sp;

            sp = layersProp.GetArrayElementAtIndex(3);
            if (sp.stringValue == "")
            {
                // Assign string value to layer
                sp.stringValue = layerName;
                Debug.Log("Layer: " + layerName + " 추가됨");
                // Save settings
                tagManager.ApplyModifiedProperties();
                return true;
            }
            else Debug.Log("3번에 넣을려고 했는데 뭔가 있네?");
        }
        else
        {
            //Debug.Log ("Layer: " + layerName + " already exists");
        }
        return false;
    }
    /// <summary>
    /// Checks if the value exists in the property.
    /// </summary>
    /// <returns><c>true</c>, if exists was propertyed, <c>false</c> otherwise.</returns>
    /// <param name="property">Property.</param>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="value">Value.</param>
    private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
    {
        for (int i = start; i < end; i++)
        {
            SerializedProperty t = property.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(value))
            {
                return true;
            }
        }
        return false;
    }
}
