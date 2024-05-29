using UnityEngine;
using UnityEditor;
using GameCreator.Runtime.Reminstance;

namespace GameCreator.Editor.Reminstance
{
    [CustomEditor(typeof(PrefabDatabase))]
    public class PrefabDatabaseEditor : UnityEditor.Editor
    {
        private PrefabDatabase prefabDatabase;

        private void OnEnable()
        {
            prefabDatabase = (PrefabDatabase)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Windows")) { PrefabDatabaseWindow.ShowWindow(); }
        }


    }
}