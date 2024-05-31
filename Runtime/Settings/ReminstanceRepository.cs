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
        public const string REPOSITORY_ID = "reminstance.general";
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        public override string RepositoryID => REPOSITORY_ID;

        [SerializeField] private PrefabsCatalogue m_catalogue = new PrefabsCatalogue();
        public PrefabsCatalogue Prefabs => this.m_catalogue;

        // EDITOR ENTER PLAYMODE: -----------------------------------------------------------------

#if UNITY_EDITOR

        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;

#endif

    }
}