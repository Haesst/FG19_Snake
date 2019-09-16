using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    /*
     * This is a "stolen" class to display some values
     * in the editor as view only. I used it to debug and
     * thought it was a very handy tool to use and will
     * probably use it in the future.
     */

    /// <summary>
    /// Get the labels height in the editor.
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    /// <summary>
    /// Disable GUI.
    /// Draw out the label
    /// Enable GUI.
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}