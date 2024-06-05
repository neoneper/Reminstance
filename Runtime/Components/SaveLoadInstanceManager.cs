using GameCreator.Runtime.Common;
using GameCreator.Runtime.Inventory;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using System;

namespace GameCreator.Runtime.Reminstance
{
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_FIRST_EARLIER)]
    public class SaveLoadInstanceManager : Singleton<SaveLoadInstanceManager>
    {

#if UNITY_EDITOR

        [UnityEditor.InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            OnRememberDroped = null;
            OnRememberInstantied = null;
            OnRememberLoaded = null;
            LastRememberInstantiated = null;
        }

#endif


        public static RememberInstance LastRememberInstantiated { get; private set; }

        /// <summary>
        /// Chamado quando um novo rememberInstance é dropado apartir de qualquer chamada de instancia de ITEM, tal como drop do mochile ou uma instancia proveninente de algum item de inventario
        /// </summary>
        public static event Action<RememberInstance> OnRememberDroped;

        /// <summary>
        /// Chamado quando um novo rememberInstance é instanciado na cena
        /// </summary>
        public static event Action<RememberInstance> OnRememberInstantied;
        /// <summary>
        /// Chamado quando um remember é carregado e instanciado na cena pos o carregamento ter sido finalizado
        /// </summary>
        public static event Action<RememberInstance> OnRememberLoaded;

        public const string FILENAME = "save_instances.json";
        private List<RememberInstance.Entry> _cache_EntrysBeforeSceneLoad = new List<RememberInstance.Entry>();
        private Dictionary<string, RememberInstance> _cache_InstancesBeforeSceneLoad = new Dictionary<string, RememberInstance>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void InitializeOnLoad()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SaveLoadManager.Instance.EventBeforeSave += OnSaving;
            SaveLoadManager.Instance.EventBeforeLoad += OnLoading;
            SaveLoadManager.Instance.EventBeforeDelete += OnDeleting;
            Item.EventInstantiate += OnItemInstantiate;

        }
        private void OnDestroy()
        {
            OnRememberDroped = null;
            OnRememberInstantied = null;
            OnRememberLoaded = null;
            LastRememberInstantiated = null;
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
        private void OnLoading(int slotIndex)
        {
            if (slotIndex == -1) { return; }//Restart Scene
            ProcessLoading(slotIndex);


        }
        private void OnDeleting(int slotIndex)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "slot" + slotIndex + "_" + FILENAME);
            if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }
        }

        private void OnItemInstantiate()
        {
            GameObject instantiedItem = Item.LastItemInstanceInstantiated;
            if (!instantiedItem.TryGetComponent<RememberInstance>(out RememberInstance remembderInstance)) { return; }
            remembderInstance.SetupNewInstance();
            LastRememberInstantiated = remembderInstance;
            OnRememberDroped?.Invoke(remembderInstance);
        }

        private void OnSceneLoaded(Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            ProcessOnLoaded();
        }
        private void ProcessLoading(int slotIndex)
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
                _cache_EntrysBeforeSceneLoad.Clear();
                _cache_EntrysBeforeSceneLoad.AddRange(content.Entries);
            }
        }
        private void ProcessOnLoaded()
        {
            if (!SaveLoadManager.Instance.IsLoading) { return; }
            if (ReminstanceRepository.Get.Prefabs.List.Count() == 0) { return; }

            //1: First we instantiating the prefabs
            foreach (var entry in _cache_EntrysBeforeSceneLoad)
            {

                if (ReminstanceRepository.Get.Prefabs.TryGet(entry.PrefabGUID, out GameObject prefab))
                {
                    GameObject go = Instantiate(prefab);
                    RememberInstance rememberInstance = go.GetComponent<RememberInstance>();
                    rememberInstance.SetupLoadedInstance(entry.RememberID);
                    _cache_InstancesBeforeSceneLoad[entry.RememberID] = rememberInstance;
                }
            }

            //2: We will keep the hierach of the instantieds
            foreach (var entry in _cache_EntrysBeforeSceneLoad)
            {
                if (string.IsNullOrEmpty(entry.PrefabGUID)) { continue; }
                if (entry.ParentType == RememberInstance.EntryParentType.None) { continue; }

                RememberInstance fromInstance = _cache_InstancesBeforeSceneLoad[entry.RememberID];

                switch (entry.ParentType)
                {
                    case RememberInstance.EntryParentType.RememberInstance:
                        RememberInstance toRememberInstance = _cache_InstancesBeforeSceneLoad[entry.ParentID];
                        fromInstance.transform.SetParent(toRememberInstance.transform, true);
                        break;
                    case RememberInstance.EntryParentType.Remember:
                        Remember toRemember = FindObjectsOfType<Remember>(true).FirstOrDefault(r => r.SaveID == entry.ParentID);
                        if (toRemember != null)
                        {
                            fromInstance.transform.SetParent(toRemember.transform, true);
                        }
                        else
                        {
                            Debug.LogError($"Loaded Remember Instance - {fromInstance.gameObject.name} with parent Remember not found in the scene");
                        }
                        break;
                    case RememberInstance.EntryParentType.SceneObject:
                        GameObject toSceneObject = GameObject.Find(entry.ParentID);
                        if (toSceneObject != null)
                        {
                            fromInstance.transform.SetParent(toSceneObject.transform, true);
                        }
                        else
                        {
                            Debug.LogError($"Loaded Remember Instance - {fromInstance.gameObject.name} with parent SceneObject not found in the scene");
                        }
                        break;
                    default:
                        Debug.LogError($"Loaded Remember Instance - {fromInstance.gameObject.name} with parent type not recogonized by RememberInstance.EntryParentType");
                        break;
                }

            }

            foreach (var entry in _cache_InstancesBeforeSceneLoad.Values)
            {
                LastRememberInstantiated = entry;
                OnRememberLoaded?.Invoke(LastRememberInstantiated);
            }
            _cache_EntrysBeforeSceneLoad.Clear();
            _cache_InstancesBeforeSceneLoad.Clear();
        }

        // PUBLICS -----------------------------------------

        public RememberInstance InstantiatePrefab(RememberInstance _rememberPrefab, Vector3 _position, Quaternion _rotation, Transform _parent)
        {
            RememberInstance instance = Instantiate(_rememberPrefab, _position, _rotation, _parent);
            instance.SetupNewInstance();
            LastRememberInstantiated = instance;
            OnRememberInstantied?.Invoke(instance);

            return instance;
        }
    }
}