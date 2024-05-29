using GameCreator.Runtime.Common;
using GameCreator.Runtime.Reminstance;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Reminstance
{
    public class PrefabDatabaseWindow : UnityEditor.EditorWindow
    {
        private PrefabDatabase prefabDatabase;
        private const string prefabDatabasePathKey = "PrefabDatabasePath";
        private SerializedObject serializedPrefabDatabase;
        private SerializedProperty serializedPrefabList;

        [MenuItem("Game Creator/Prefab Database")]
        public static void ShowWindow()
        {
            GetWindow<PrefabDatabaseWindow>("Prefab Database");
        }

        private void OnEnable()
        {
            // Try to load the PrefabDatabase asset
            string savedPath = EditorPrefs.GetString(prefabDatabasePathKey, string.Empty);
            if (!string.IsNullOrEmpty(savedPath))
            {
                prefabDatabase = AssetDatabase.LoadAssetAtPath<PrefabDatabase>(savedPath);
                if (prefabDatabase == null)
                {
                    EditorPrefs.DeleteKey(prefabDatabasePathKey);
                    LoadPrefabDatabase();
                }
            }
            else
            {
                LoadPrefabDatabase();
            }
        }

        private void LoadPrefabDatabase()
        {
            string[] guids = AssetDatabase.FindAssets("t:PrefabDatabase");
            if (guids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                prefabDatabase = AssetDatabase.LoadAssetAtPath<PrefabDatabase>(assetPath);
                EditorPrefs.SetString(prefabDatabasePathKey, assetPath);
            }
        }

        private void CreatePrefabDatabase()
        {
            prefabDatabase = ScriptableObject.CreateInstance<PrefabDatabase>();
            string assetPath = "Assets/PrefabDatabase.asset";
            AssetDatabase.CreateAsset(prefabDatabase, assetPath);
            AssetDatabase.SaveAssets();
            EditorPrefs.SetString(prefabDatabasePathKey, assetPath);
            Debug.Log("Created new PrefabDatabase asset at " + assetPath);

        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Prefab Database", EditorStyles.boldLabel);
            if (prefabDatabase == null)
            {
                EditorGUILayout.HelpBox("PrefabDatabase asset not found!", MessageType.Warning);

                if (GUILayout.Button("Create PrefabDatabase"))
                {
                    CreatePrefabDatabase();
                }

                return;
            }

            GUILayout.Space(10);
            DrawDragAndDropArea();

            GUILayout.Space(10);
            DrawPrefabList();
        }

        private void DrawPrefabList()
        {
            serializedPrefabDatabase?.Update();
            if (serializedPrefabList == null)
            {
                if (prefabDatabase != null)
                {
                    serializedPrefabDatabase = new SerializedObject(prefabDatabase);
                    serializedPrefabList = serializedPrefabDatabase.FindProperty("prefabList");
                }
            }

            for (int i = 0; i < serializedPrefabList.arraySize; i++)
            {
                SerializedProperty prefabInfo = serializedPrefabList.GetArrayElementAtIndex(i);
                SerializedProperty guid = prefabInfo.FindPropertyRelative("guid");
                SerializedProperty prefab = prefabInfo.FindPropertyRelative("prefab");
         
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{prefab.objectReferenceValue.name}");
                EditorGUILayout.LabelField($"{guid.stringValue}");
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    EditorGUIUtility.PingObject(prefab.objectReferenceValue);
                }
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    serializedPrefabList.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            serializedPrefabDatabase?.ApplyModifiedProperties();
        }

        private void DrawDragAndDropArea()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Drag and Drop Prefabs Below", EditorStyles.boldLabel);

            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag and Drop Prefabs Here", EditorStyles.helpBox);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();


                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            GameObject prefab = draggedObject as GameObject;

                            if (prefab != null && prefab.GetComponent<RememberInstance>() != null)
                            {
                                AddPrefab(prefab);
                            }
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }

        private void AddPrefab(GameObject prefab)
        {
            RememberInstance rememberComponent = prefab.GetComponent<RememberInstance>();

            if (rememberComponent != null)
            {
                if (string.IsNullOrEmpty(rememberComponent.prefabGUID))
                {
                    AssingRememberInstanceID(rememberComponent);
                }

                string guid = rememberComponent.prefabGUID;


                if (!prefabDatabase.ContainsPrefabGUID(guid))
                {
                    PrefabDatabase.PrefabInfo newPrefabInfo = new PrefabDatabase.PrefabInfo
                    {
                        guid = guid,
                        prefab = prefab
                    };

                    prefabDatabase.Add(newPrefabInfo);

                    EditorUtility.SetDirty(prefabDatabase); // Mark the prefabDatabase as dirty to save changes
                }
            }
        }

        private void AssingRememberInstanceID(RememberInstance rememberInstance)
        {

            // Obtendo o tipo do componente
            System.Type rememberType = typeof(RememberInstance);

            // Obtendo o campo privado por reflexão
            System.Reflection.FieldInfo privateField = rememberType.GetField("m_prefabGUID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Modificando o valor do campo privado
            privateField.SetValue(rememberInstance, System.Guid.NewGuid().ToString());
        }

    }
}