using GameCreator.Runtime.Common;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Icon(RuntimePaths.GIZMOS + "GizmoRemember.png")]
    [CreateAssetMenu(fileName = "PrefabDatabase", menuName = "Game Creator/SaveLoad/PrefabDatabase")]
    [System.Serializable]
    public class PrefabDatabase : ScriptableObject
    {
        [System.Serializable]
        public struct PrefabInfo
        {
            public string guid;
            public GameObject prefab;
        }

        [SerializeField]
        private List<PrefabInfo> prefabList = new List<PrefabInfo>();

        [System.NonSerialized]
        private Dictionary<string, PrefabInfo> _map;

        public Dictionary<string, PrefabInfo> PrefabsMap
        {
            get
            {
                if (_map == null)
                {
                    _map = new Dictionary<string, PrefabInfo>();
                    foreach (var info in prefabList)
                    {
                        _map[info.guid] = info;
                    }
                }

                return _map;
            }
        }

        public PrefabInfo GetPrefabInfo(string guid)
        {
            if (PrefabsMap.ContainsKey(guid))
            {
                return PrefabsMap[guid];
            }

            return default(PrefabInfo);
        }

        public bool TryGetPrefabInfo(string guid, out PrefabInfo info)
        {
            bool result = false;
            if (PrefabsMap.ContainsKey(guid))
            {
                info = PrefabsMap[guid];
                result = true;
            }
            else
            {
                info = default(PrefabInfo);
            }

            return result;
        }

        public bool ContainsPrefabGUID(string guid)
        {
            if (string.IsNullOrEmpty(guid)) { return false; }
            if (Application.isPlaying)
            {
                return PrefabsMap.ContainsKey(guid);
            }
            return prefabList.Exists(r => r.guid == guid);
        }
        public void Add(PrefabInfo info)
        {
            prefabList.Add(info);
        }
    }




}
