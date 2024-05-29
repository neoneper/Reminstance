using GameCreator.Runtime.Common;
using GameCreator.Runtime.Inventory;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_FIRST_EARLIER)]
    public class SaveLoadInstanceManager : Singleton<SaveLoadInstanceManager>
    {
        public const string FILENAME = "save_instances.json";
        private PrefabDatabase m_PrefabDatabase;
        private List<RememberInstance.Entry> lastInstances = new List<RememberInstance.Entry>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void InitializeOnLoad()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.m_PrefabDatabase = ReminstanceRepository.Get.Database;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SaveLoadManager.Instance.EventBeforeSave += OnSaving;
            SaveLoadManager.Instance.EventBeforeLoad += OnLoading;
            SaveLoadManager.Instance.EventBeforeDelete += OnDeleting;
            Item.EventInstantiate += OnItemInstantiate;

        }

        private void OnDeleting(int slotIndex)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "slot" + slotIndex + "_" + FILENAME);
            if (System.IO.File.Exists(path)){ System.IO.File.Delete(path); }
        }

        private void OnLoading(int slotIndex)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "slot" + slotIndex + "_" + FILENAME);
            RememberInstance.Blob content = null;

            if (System.IO.File.Exists(path))
            {
                try
                {
                    string json;
                    using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            json = reader.ReadToEnd();
                        }
                    }
                    content = JsonUtility.FromJson<RememberInstance.Blob>(json);
                }
                catch (System.Exception exception)
                {
                    Debug.LogError($"Error trying to load data instances: {exception}");
                }
            }

            if (content != null)
            {
                lastInstances.Clear();
                lastInstances.AddRange(content.Entries);
            }

        }

        private void OnSaving(int slotIndex)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "slot" + slotIndex + "_" + FILENAME);
            RememberInstance.Blob content = new RememberInstance.Blob(slotIndex, GameObject.FindObjectsOfType<RememberInstance>(true));

            try
            {
                string directory = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
                System.IO.Directory.CreateDirectory(directory);
                string json = JsonUtility.ToJson(content, false);
                using System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Create);
                using System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
                writer.Write(json);
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"Error trying to save instance data: {exception}");
            }
        }

        private void OnItemInstantiate()
        {
            GameObject instantiedItem = Item.LastItemInstanceInstantiated;
            if (!instantiedItem.TryGetComponent<RememberInstance>(out RememberInstance remembderInstance)) { return; }
            remembderInstance.SetupNewInstance();
        }

        private void OnSceneLoaded(Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            if (!SaveLoadManager.Instance.IsLoading) { return; }
            if (m_PrefabDatabase == null) { return; }
            Debug.Log(lastInstances.Count);
            foreach (var entry in lastInstances)
            {
                PrefabDatabase.PrefabInfo info;
                if (m_PrefabDatabase.TryGetPrefabInfo(entry.PrefabGUID, out info))
                {
                    GameObject go = Instantiate(info.prefab);
                    RememberInstance rememberInstance = go.GetComponent<RememberInstance>();
                    rememberInstance.SetupLoadedInstance(entry.RememberID);

                }
            }
        }

    }
}