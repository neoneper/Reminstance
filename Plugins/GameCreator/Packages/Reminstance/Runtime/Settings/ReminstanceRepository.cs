using GameCreator.Runtime.Common;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameCreator.Runtime.Reminstance
{
    [Serializable]
    public class ReminstanceRepository : TRepository<ReminstanceRepository>
    {
        public const string REPOSITORY_ID = "core.reminstance"; 
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        public override string RepositoryID => REPOSITORY_ID;
        [SerializeField] private PrefabDatabase database;
        public PrefabDatabase Database => database;

    }
}