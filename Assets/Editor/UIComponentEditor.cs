using Project;
using UnityEditor;
using UnityEngine;
using static Project.UIComponent;

[CustomEditor(typeof(UIComponent))]
public class UIComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();



        //取得 preset 欄位。
        //SerializedProperty preset = rectData.FindPropertyRelative("anchors.preset");
        // 取得 Min 欄位。
        //SerializedProperty min = property.FindPropertyRelative("Min");
        //// 取得 Max 欄位。
        //SerializedProperty max = property.FindPropertyRelative("Max");
        // 顯示原本 Inspector 欄位
        SerializedProperty barData = serializedObject.FindProperty("barList");

        GUILayout.Space(20);
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        //EditorGUILayout.PropertyField(preset);

        if (EditorGUI.EndChangeCheck()) 
        {
            // 這裡可以放置在  重繪異動後下拉修改時數值
            //for (int i = 0; i < barData.arraySize; i++)
            //{

            //    SerializedProperty bar = barData.GetArrayElementAtIndex(i);
            //    SerializedProperty rectData = bar.FindPropertyRelative("rectTransform");
            //    if (rectData == null || !rectData.hasVisibleChildren)
            //    {
            //        continue;
            //    }
            //    SerializedProperty anchors = rectData.FindPropertyRelative("anchors");
            //    if (anchors == null || !anchors.hasVisibleChildren)
            //    {
            //        continue;
            //    }
            //    SerializedProperty preset =
            //        anchors.FindPropertyRelative("preset");

            //    SerializedProperty min =
            //        anchors.FindPropertyRelative("Min");

            //    SerializedProperty max =
            //        anchors.FindPropertyRelative("Max");

            //    if (preset == null || min == null || max == null)
            //    {
            //        continue;
            //    }

            //    ApplyPreset(preset, min, max);
            //}
        }

        //按鍵生成
        if (GUILayout.Button("Generate UI", GUILayout.Height(30), GUILayout.Width(220)))
         {
            UIComponent generator = (UIComponent)target;
            generator.GenerateObjects();
         }

         serializedObject.ApplyModifiedProperties();
    }
    private void ApplyPreset(SerializedProperty preset, SerializedProperty min, SerializedProperty max)
    {
        // 取得目前選擇的 AnchorPreset。
        AnchorPreset selectedPreset = (AnchorPreset)preset.enumValueIndex;

        // 依照 preset 設定 Min 和 Max。
        switch (selectedPreset)
        {
            case AnchorPreset.TopLeft:
                min.vector2Value = new Vector2(0, 1);
                max.vector2Value = new Vector2(0, 1);
                break;
            case AnchorPreset.Top:
                min.vector2Value = new Vector2(0.5f, 1);
                max.vector2Value = new Vector2(0.5f, 1);
                break;
            case AnchorPreset.TopRight:
                min.vector2Value = new Vector2(1, 1);
                max.vector2Value = new Vector2(1, 1);
                break;
            case AnchorPreset.Left:
                min.vector2Value = new Vector2(0, 0.5f);
                max.vector2Value = new Vector2(0, 0.5f);
                break;
            case AnchorPreset.Center:
                min.vector2Value = new Vector2(0.5f, 0.5f);
                max.vector2Value = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPreset.Right:
                min.vector2Value = new Vector2(1, 0.5f);
                max.vector2Value = new Vector2(1, 0.5f);
                break;
            case AnchorPreset.BottomLeft:
                min.vector2Value = new Vector2(0, 0);
                max.vector2Value = new Vector2(0, 0);
                break;
            case AnchorPreset.Bottom:
                min.vector2Value = new Vector2(0.5f, 0);
                max.vector2Value = new Vector2(0.5f, 0);
                break;
            case AnchorPreset.BottomRight:
                min.vector2Value = new Vector2(1, 0);
                max.vector2Value = new Vector2(1, 0);
                break;
            case AnchorPreset.StretchHorizontal:
                min.vector2Value = new Vector2(0, 0.5f);
                max.vector2Value = new Vector2(1, 0.5f);
                break;
            case AnchorPreset.StretchVertical:
                min.vector2Value = new Vector2(0.5f, 0);
                max.vector2Value = new Vector2(0.5f, 1);
                break;
            case AnchorPreset.StretchAll:
                min.vector2Value = new Vector2(0, 0);
                max.vector2Value = new Vector2(1, 1);
                break;
            case AnchorPreset.Custom:
                
                break;
        }
    }
}
[CustomPropertyDrawer(typeof(Anchors))]
public class AnchorSettingDrawer2 : PropertyAttribute
{
    public AnchorSettingDrawer2() { }
}
[CustomPropertyDrawer(typeof(Anchors))]
public class AnchorSettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 取得 preset 欄位。
        SerializedProperty preset = property.FindPropertyRelative("preset");
        // 取得 Min 欄位。
        SerializedProperty min = property.FindPropertyRelative("Min");
        // 取得 Max 欄位。
        SerializedProperty max = property.FindPropertyRelative("Max");
        // 取得單一欄位的高度。
        float lineHeight = EditorGUIUtility.singleLineHeight;
        // 設定欄位之間的間距。
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        // 設定目前欄位的高度。
        position.height = lineHeight;

        // 開始監測 preset 是否被修改。
        EditorGUI.BeginChangeCheck();
        // 顯示 preset 欄位。
        EditorGUI.PropertyField(position, preset);
        // 如果 preset 被修改，就套用對應的 Min 和 Max。
        if (EditorGUI.EndChangeCheck())
        {
            ApplyPreset(preset, min, max);
        }
        bool isCustom = preset.enumValueIndex == (int)AnchorPreset.Custom;
        EditorGUI.BeginDisabledGroup(!isCustom);
        // 將下一個欄位往下移動一列。
        position.y += lineHeight + spacing;
        // 顯示 Min 欄位。
        EditorGUI.PropertyField(position, min);
        
        // 將下一個欄位往下移動一列。
        position.y += lineHeight + spacing;
        // 顯示 Max 欄位。
        EditorGUI.PropertyField(position, max);
        EditorGUI.EndDisabledGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 回傳三個欄位加上兩個間距的總高度。
        return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
    }

    private void ApplyPreset(SerializedProperty preset, SerializedProperty min, SerializedProperty max)
    {
        // 取得目前選擇的 AnchorPreset。
        AnchorPreset selectedPreset = (AnchorPreset)preset.enumValueIndex;

        // 依照 preset 設定 Min 和 Max。
        switch (selectedPreset)
        {
            case AnchorPreset.TopLeft:
                min.vector2Value = new Vector2(0, 1);
                max.vector2Value = new Vector2(0, 1);
                break;
            case AnchorPreset.Top:
                min.vector2Value = new Vector2(0.5f, 1);
                max.vector2Value = new Vector2(0.5f, 1);
                break;
            case AnchorPreset.TopRight:
                min.vector2Value = new Vector2(1, 1);
                max.vector2Value = new Vector2(1, 1);
                break;
            case AnchorPreset.Left:
                min.vector2Value = new Vector2(0, 0.5f);
                max.vector2Value = new Vector2(0, 0.5f);
                break;
            case AnchorPreset.Center:
                min.vector2Value = new Vector2(0.5f, 0.5f);
                max.vector2Value = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPreset.Right:
                min.vector2Value = new Vector2(1, 0.5f);
                max.vector2Value = new Vector2(1, 0.5f);
                break;
            case AnchorPreset.BottomLeft:
                min.vector2Value = new Vector2(0, 0);
                max.vector2Value = new Vector2(0, 0);
                break;
            case AnchorPreset.Bottom:
                min.vector2Value = new Vector2(0.5f, 0);
                max.vector2Value = new Vector2(0.5f, 0);
                break;
            case AnchorPreset.BottomRight:
                min.vector2Value = new Vector2(1, 0);
                max.vector2Value = new Vector2(1, 0);
                break;
            case AnchorPreset.StretchHorizontal:
                min.vector2Value = new Vector2(0, 0.5f);
                max.vector2Value = new Vector2(1, 0.5f);
                break;
            case AnchorPreset.StretchVertical:
                min.vector2Value = new Vector2(0.5f, 0);
                max.vector2Value = new Vector2(0.5f, 1);
                break;
            case AnchorPreset.StretchAll:
                min.vector2Value = new Vector2(0, 0);
                max.vector2Value = new Vector2(1, 1);
                break;
        }
    }
}
