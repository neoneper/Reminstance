using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Serializable]
    public class PrefabsCatalogue
    {
       

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Dictionary<string, GameObject> m_Map;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GameObject[] m_Prefabs = Array.Empty<GameObject>();

        // PROPERTIES: ----------------------------------------------------------------------------

        public GameObject[] List => this.m_Prefabs;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject Get(string prefabGUID)
        {
            this.RequireInitialize();
            return this.m_Map.TryGetValue(prefabGUID, out GameObject gameObject) ? gameObject : null;
        }
        public bool TryGet(string prefabGUID, out GameObject gameObject)
        {
            this.RequireInitialize();
            return this.m_Map.TryGetValue(prefabGUID, out gameObject); 
        }
        public bool Contains(string prefabGUID)
        {
            this.RequireInitialize();
            return this.m_Map.ContainsKey(prefabGUID);
        }
        private void RequireInitialize()
        {
            if (this.m_Map != null) return;

            this.m_Map = new Dictionary<string, GameObject>();
            foreach (GameObject prefab in this.m_Prefabs) this.m_Map[prefab.GetComponent<RememberInstance>().PrefabGUID] = prefab;
        }

        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal void Set(GameObject[] prefabs)
        {
            this.m_Prefabs = prefabs;
        }



    }
}