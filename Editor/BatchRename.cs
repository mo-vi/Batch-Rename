using UnityEditor;
using UnityEngine;

namespace ExtendedEditor.BatchRename
{
    public enum SeparatorOptions
    {
        None,
        Dot,
        Hyphen,
        Parenthesis,
        Underscore
    }
    
    public class BatchRename : EditorWindow
    {
        public GameObject[] gameObjects = {};
        public SeparatorOptions separatorOptions;

        bool useNumbering;
        int startingNumber;
        SerializedObject serializedObject;
        static readonly Vector2Int size = new(250, 175);
        string newName;
        
        [MenuItem("GameObject/Extended Editor/Batch Rename")]
        public static void ShowWindow()
        {
            var window = GetWindow<BatchRename>("Batch Rename");
            
            window.minSize = size;
        }
     
        void OnEnable()
        {
            ScriptableObject target = this;
            
            serializedObject = new SerializedObject(target);
         
            startingNumber = 1;
        }
     
        void OnGUI()
        {
            gameObjects = Selection.gameObjects;
            
            EditorGUILayout.Space(4);
            
            EditorGUILayout.HelpBox("Select all the game objects in hierarchy you want to rename and then press 'Batch Rename'", MessageType.Info);
            
            EditorGUILayout.Space(4);
            
            DrawHorizontalLine();

            EditorGUILayout.Space(4);

            newName = EditorGUILayout.TextField("New Name", newName);
            
            useNumbering = EditorGUILayout.Toggle("Use Numbering", useNumbering);

            if (useNumbering)
            {
                startingNumber = EditorGUILayout.IntField("Starting Number", startingNumber);
                
                separatorOptions = (SeparatorOptions)EditorGUILayout.EnumPopup("Separator", separatorOptions);
            }
            
            EditorGUILayout.Space(4);

            DrawHorizontalLine();
            
            EditorGUILayout.Space(4);

            if (GUILayout.Button("Batch Rename"))
            {
                if (gameObjects.Length == 0)
                {
                    Debug.LogWarning("No GameObjects to rename. Select at least one GameObject in the hierarchy.");
                    
                    return;    
                }

                if (string.IsNullOrEmpty(newName))
                {
                    Debug.LogWarning("New name is not set. Provide a new one.");

                    return;
                }
                
                var serializedProperty = serializedObject.FindProperty("gameObjects");

                for (int i = 0, n = startingNumber; i < serializedProperty.arraySize; i++)
                {
                    var gameObjectAtIndex = serializedProperty.GetArrayElementAtIndex(i);

                    if (useNumbering)
                    {
                        switch (separatorOptions)
                        {
                            case SeparatorOptions.None:
                                gameObjectAtIndex.objectReferenceValue.name = $"{newName}{n++}";
                                break;
                            case SeparatorOptions.Dot:
                                gameObjectAtIndex.objectReferenceValue.name = $"{newName}.{n++}";
                                break;
                            case SeparatorOptions.Hyphen:
                                gameObjectAtIndex.objectReferenceValue.name = $"{newName}-{n++}";
                                break;
                            case SeparatorOptions.Parenthesis:
                                gameObjectAtIndex.objectReferenceValue.name = $"{newName} ({n++})";
                                break;
                            case SeparatorOptions.Underscore:
                                gameObjectAtIndex.objectReferenceValue.name = $"{newName}_{n++}";
                                break;
                        }
                    }
                    else
                    {
                        gameObjectAtIndex.objectReferenceValue.name = $"{newName}";
                    }
                }
            }

            Undo.RecordObjects(gameObjects, "Renamed Game Objects");

            serializedObject.Update();

            serializedObject.ApplyModifiedProperties();
        }

        static void DrawHorizontalLine()
        {
            var rect = EditorGUILayout.BeginHorizontal();
            
            Handles.color = new Color(0.35f, 0.35f, 0.35f);
            
            Handles.DrawLine(new Vector2(rect.x + 4, rect.y), new Vector2(rect.width - 4, rect.y));
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
