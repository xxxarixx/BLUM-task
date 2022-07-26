using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CustomEditorAssistance
{
    public static class CustomEditorAssistance_
    {
        public static void _DrawText(string _text, Color _newColor, int _fontSize = 13, bool wordWrap = false)
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.textColor = _newColor;
            guiStyle.fontSize = _fontSize;
            guiStyle.wordWrap = wordWrap;
            GUILayout.Label(_text, guiStyle);
        }
        public static void _DrawText(string _text, Color _newColor, TextAnchor alignment, int _fontSize = 13, bool wordWrap = false)
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.textColor = _newColor;
            guiStyle.alignment = alignment;
            guiStyle.fontSize = _fontSize;
            guiStyle.wordWrap = wordWrap;
            GUILayout.Label(_text, guiStyle);
        }
        #if UNITY_EDITOR
        public static void _DrawProperty(SerializedObject serializedObject, string _propertyName)
        {
            var property = serializedObject.FindProperty(_propertyName);
            EditorGUILayout.PropertyField(property);
        }
        #endif
    }
}