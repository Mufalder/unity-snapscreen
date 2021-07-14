using System.IO;
using UnityEngine;
using UnityEditor;

namespace NorthLab
{
    [CustomEditor(typeof(Snapscreen))]
    public class EditorSnapscreen : Editor
    {

        new private SerializedProperty name;
        private SerializedProperty path;
        private SerializedProperty factor;
        private SerializedProperty useInput;
        private SerializedProperty button;
        private SerializedProperty key;
        private SerializedProperty onPrintScreen;
        private SerializedProperty dateStamp;
        private SerializedProperty useGUI;
        private SerializedProperty unhideTime;
        private SerializedProperty guiPosition;
        private SerializedProperty position;
        private SerializedProperty crop;
        private SerializedProperty left;
        private SerializedProperty right;
        private SerializedProperty top;
        private SerializedProperty bottom;
        private SerializedProperty compositionGrid;
        private SerializedProperty grid;
        private SerializedProperty gridColor;
        private Snapscreen script;

        private void OnEnable()
        {
            name = serializedObject.FindProperty("name");
            path = serializedObject.FindProperty("path");
            factor = serializedObject.FindProperty("factor");
            useInput = serializedObject.FindProperty("useInput");
            button = serializedObject.FindProperty("button");
            key = serializedObject.FindProperty("key");
            onPrintScreen = serializedObject.FindProperty("onPrintScreen");
            dateStamp = serializedObject.FindProperty("dateStamp");
            useGUI = serializedObject.FindProperty("useGUI");
            unhideTime = serializedObject.FindProperty("unhideTime");
            guiPosition = serializedObject.FindProperty("guiPosition");
            position = serializedObject.FindProperty("position");
            crop = serializedObject.FindProperty("crop");
            left = serializedObject.FindProperty("left");
            right = serializedObject.FindProperty("right");
            top = serializedObject.FindProperty("top");
            bottom = serializedObject.FindProperty("bottom");
            compositionGrid = serializedObject.FindProperty("compositionGrid");
            grid = serializedObject.FindProperty("grid");
            gridColor = serializedObject.FindProperty("gridColor");
            script = (Snapscreen)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(path);

            if (path.stringValue.Length <= 0 || !System.IO.Directory.Exists(path.stringValue))
                path.stringValue = Application.dataPath;

            if (GUILayout.Button("browse", GUILayout.MaxWidth(64)))
            {
                path.stringValue = EditorUtility.SaveFolderPanel("Screenshot directory", path.stringValue, "");
            }

            GUI.enabled = Directory.Exists(path.stringValue);
            if (GUILayout.Button("reveal", GUILayout.MaxWidth(64)))
            {
                EditorUtility.RevealInFinder(path.stringValue);
            }
            GUI.enabled = true;

            if (path.stringValue.Length > 0 && path.stringValue[path.stringValue.Length - 1] != '/')
            {
                path.stringValue += '/';
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(dateStamp);
            EditorGUILayout.PropertyField(factor);

            EditorGUILayout.PropertyField(useInput);
            if (useInput.boolValue)
            {
                EditorGUILayout.PropertyField(button);
                EditorGUILayout.PropertyField(key);
                EditorGUILayout.PropertyField(onPrintScreen);
            }

            EditorGUILayout.PropertyField(useGUI);
            EditorGUILayout.PropertyField(unhideTime);
            EditorGUILayout.PropertyField(guiPosition);
            if (guiPosition.enumValueIndex == 4)
                EditorGUILayout.PropertyField(position);

            EditorGUILayout.PropertyField(crop);
            if (crop.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(left);
                EditorGUILayout.PropertyField(top);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(right);
                EditorGUILayout.PropertyField(bottom);
                EditorGUILayout.EndHorizontal();

                if (!Application.isPlaying)
                    EditorGUILayout.HelpBox("You can't take the cropped images in the edit mode.", MessageType.Info, true);
            }

            EditorGUILayout.PropertyField(compositionGrid);
            if (compositionGrid.boolValue)
            {
                EditorGUILayout.PropertyField(grid);
                EditorGUILayout.PropertyField(gridColor);
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            if (GUILayout.Button("Take"))
            {
                script.TakeScreenshot();
            }
        }
    }
}