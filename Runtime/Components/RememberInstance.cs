
using System.Threading.Tasks;
using UnityEngine;
using System;

namespace GameCreator.Runtime.Common
{

    [AddComponentMenu("Game Creator/Save & Load/Remember Instance")]
    [Icon(RuntimePaths.GIZMOS + "GizmoRemember.png")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_LAST)]
    [DisallowMultipleComponent]
    public class RememberInstance : MonoBehaviour, IGameSave
    {
        [SerializeField]
        private SaveUniqueID m_SaveUniqueID = new SaveUniqueID(true, "undefinded-prefab-instance-id");
        [SerializeField]
        private Memories m_Memories = new Memories();
        [SerializeField, HideInInspector]
        private string m_prefabGUID;

        [field: NonSerialized] internal bool IsDestroying { get; private set; }

        public bool IsSceneLoaded => this.gameObject.scene.isLoaded;
        public string PrefabGUID => m_prefabGUID;
        public string ParentGUID
        {
            get
            {
                string parentGuid = "";
                if (transform.parent != null)
                {

                    RememberInstance parentRememberInstance = transform.parent.gameObject.GetComponent<RememberInstance>();
                    Remember parentRemember = transform.parent.gameObject.GetComponent<Remember>();
                    if (parentRememberInstance != null)
                    {
                        parentGuid = parentRememberInstance.SaveID;
                    }
                    else if (parentRemember != null)
                    {
                        parentGuid = parentRemember.SaveID;
                    }
                    else
                    {
                        parentGuid = transform.parent.gameObject.name;
                    }
                }

                return parentGuid;
            }

        }

        private void OnDestroy()
        {
            this.IsDestroying = true;
            SaveLoadManager.Unsubscribe(this);
        }

        public string SaveID => this.m_SaveUniqueID.Get.String;
        public bool IsShared => false;

        public Type SaveType => this.m_Memories.SaveType;

        public object GetSaveData(bool includeNonSavable)
        {
            return this.m_SaveUniqueID.SaveValue
                ? this.m_Memories.GetTokens(this.gameObject)
                : null;
        }

        public LoadMode LoadMode => LoadMode.Lazy;

        public Task OnLoad(object value)
        {
            this.m_Memories.OnRemember(
                this.gameObject,
                value as Tokens
            );

            return Task.FromResult(true);
        }

        public void SetupNewInstance()
        {
            m_SaveUniqueID = new SaveUniqueID(true, System.Guid.NewGuid().ToString());
            _ = SaveLoadManager.Subscribe(this);
        }
        public void SetupLoadedInstance(string rememberID)
        {
            m_SaveUniqueID = new SaveUniqueID(true, rememberID);
            _ = SaveLoadManager.Subscribe(this);
        }
        public void RefreshPrefabGUID()
        {

        }

        public void SetupPrefabGUID()
        {
            if (string.IsNullOrEmpty(PrefabGUID))
            {
                m_prefabGUID = System.Guid.NewGuid().ToString();
            }
        }

        public enum EntryParentType
        {
            None,
            RememberInstance,
            Remember,
            SceneObject
        }

        [Serializable]
        public class Entry
        {
            [SerializeField] private EntryParentType m_ParentType;
            [SerializeField] private string m_prefabID;
            [SerializeField] private string m_rememberID;
            [SerializeField] private string m_parentID;

            public EntryParentType ParentType => m_ParentType;
            /// <summary>
            /// This id represent a parent gameObject if this gameObject has one. This ID can be:
            /// <para>- A <seealso cref="RememberInstance.SaveID"/> if has one. </para>
            /// <para>- A <seealso cref="Remember.SaveID"/> if has one. </para>
            /// <para>- Or Just a GameObject name is this one is a simple Scene Object </para>
            ///  <para> Empt if this GameObject has no a Parent </para>
            /// instance not have a parent.
            /// </summary>
            public string ParentID => this.m_parentID;
            /// <summary>
            /// This is the a unique ID that referencing the instantied prefab in the scene. This ID is setted in PrefabDatabase and it is no the native unity prefab InstanceID.
            /// </summary>
            public string PrefabGUID => this.m_prefabID;
            /// <summary>
            /// This ID is the remember ID of the gameObject in the scene. This ID is setted in firstTime of the instance in the world.
            /// </summary>
            public string RememberID => this.m_rememberID;

            public Entry(string prefabID, string rememberID, string parentID, EntryParentType parentType)
            {
                this.m_prefabID = prefabID;
                this.m_rememberID = rememberID;
                this.m_parentID = parentID;
                this.m_ParentType = parentType;
            }
        }

        [Serializable]
        public class Blob
        {
            [SerializeField] private int m_slot;
            [SerializeField] private Entry[] m_Entries;

            public Entry[] Entries => this.m_Entries;
            public int Slot => this.m_slot;

            public Blob(int slotIndex, RememberInstance[] instances)
            {
                this.m_Entries = new Entry[instances.Length];
                this.m_slot = slotIndex;
                int index = 0;

                foreach (var entry in instances)
                {
                    EntryParentType ptype = EntryParentType.None;
                    if (entry.transform.parent != null)
                    {
                        if (entry.transform.parent.gameObject.GetComponent<RememberInstance>() != null)
                        {
                            ptype = EntryParentType.RememberInstance;
                        }
                        else if (entry.transform.parent.gameObject.GetComponent<Remember>() != null)
                        {
                            ptype = EntryParentType.Remember;
                        }
                        else
                        {
                            ptype = EntryParentType.SceneObject;
                        }
                    }

                    this.m_Entries[index] = new Entry(entry.PrefabGUID, entry.SaveID, entry.ParentGUID, ptype);
                    index += 1;
                }
            }
        }
    }

}