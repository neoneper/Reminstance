
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

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private SaveUniqueID m_SaveUniqueID = new SaveUniqueID(true, "undefinded-prefab-instance-id");

        [SerializeField]
        private Memories m_Memories = new Memories();

        [SerializeField,HideInInspector]
        private string m_prefabGUID;


        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] internal bool IsDestroying { get; private set; }

        public bool IsSceneLoaded => this.gameObject.scene.isLoaded;
        public string prefabGUID => m_prefabGUID;

        // INIT METHODS: --------------------------------------------------------------------------


        private void OnDestroy()
        {
            this.IsDestroying = true;
            SaveLoadManager.Unsubscribe(this);
        }

        // IGAMESAVE INTERFACE: -------------------------------------------------------------------

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

        // SETUPS: -------------------------------------------------------------------
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


        [Serializable]
        public class Entry
        {
            [SerializeField] private string m_prefabID;
            [SerializeField] private string m_rememberID;

            public string PrefabGUID => this.m_prefabID;
            public string RememberID => this.m_rememberID;

            public Entry(string prefabID, string rememberID)
            {
                this.m_prefabID = prefabID;
                this.m_rememberID = rememberID;
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
                    this.m_Entries[index] = new Entry(entry.prefabGUID, entry.SaveID);
                    index += 1;
                }
            }
        }
    }

}