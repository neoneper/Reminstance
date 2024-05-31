using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Reminstance;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
namespace GameCreator.Editor.Reminstance
{
    public class ReminstancePostProcessor : AssetPostprocessor
    {
        public static event Action EventRefresh;

        // PROCESSORS: ----------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SettingsWindow.InitRunners.Add(new InitRunner(
                SettingsWindow.INIT_PRIORITY_LOW,
                CanRefreshItems,
                RefreshItems
            ));
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0 && deletedAssets.Length == 0) return;
            RefreshItems();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static bool CanRefreshItems()
        {
            return true;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void RefreshItems()
        {

            ISearchList searchSettings = SearchService.Request($"p: t:ReminstanceSettings", SearchFlags.Synchronous);
            if (searchSettings.Count == 0) { return; }

            ReminstanceSettings itemSettings = searchSettings.First().ToObject<ReminstanceSettings>();
            if (itemSettings == null) return;

            ISearchList searchInstances = SearchService.Request($"p: t:RememberInstance", SearchFlags.Synchronous);
            RememberInstance[] instances = new RememberInstance[searchInstances.Count];

            for (int i = 0; i < searchInstances.Count; i++)
            {
                instances[i] = searchInstances[i].ToObject<GameObject>().GetComponent<RememberInstance>();
            }

            SerializedObject itemSettingsSerializedObject = new SerializedObject(itemSettings);

            SerializedProperty globalVariablesProperty = itemSettingsSerializedObject
                .FindProperty(TAssetRepositoryEditor.NAMEOF_MEMBER)
                .FindPropertyRelative("m_catalogue")
                .FindPropertyRelative("m_Prefabs");

            globalVariablesProperty.arraySize = instances.Length;

            for (int i = 0; i < instances.Length; ++i)
            {
                globalVariablesProperty.GetArrayElementAtIndex(i).objectReferenceValue = instances[i].gameObject;
                instances[i].SetupPrefabGUID();
            }

            itemSettingsSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            EventRefresh?.Invoke();
        }
    }
}
