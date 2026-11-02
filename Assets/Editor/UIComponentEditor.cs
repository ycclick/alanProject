using UnityEngine;
using UnityEditor;
using Project;

[CustomEditor(typeof(UIComponent))]
public class UIComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 顯示原本 Inspector 欄位
        DrawDefaultInspector();

        GUILayout.Space(10);
        SerializedProperty preset =
           serializedObject.FindProperty("Preset");
        UIComponent generator = (UIComponent)target;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(preset);

        if (GUILayout.Button("Generate UI", GUILayout.Height(20), GUILayout.Width(120)))
        {
            generator.GenerateObjects();
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            Debug.Log("Anchor Preset 已變更");

            // 這裡執行你的切換邏輯
        }
        serializedObject.ApplyModifiedProperties();
    }
}
