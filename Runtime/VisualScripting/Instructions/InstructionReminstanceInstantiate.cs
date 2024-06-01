using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Reminstance;
using UnityEngine;
using UnityEngine.Search;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Remember Instantiate")]
    [Description("Creates a new instance of a referenced game object")]

    [Category("Game Objects/Reminstance/Instantiate")]

    [Parameter("Position", "The position of the new game object instance")]
    [Parameter("Rotation", "The rotation of the new game object instance")]
    [Parameter("Save", "Optional value where the newly instantiated game object is stored")]

    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayPlus))]

    [Keywords("Create", "New", "Game Object")]
    [Serializable]
    public class InstructionReminstanceInstantiate : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField, SearchContext("p: t:RememberInstance", SearchViewFlags.DisableQueryHelpers | SearchViewFlags.DisableSavedSearchQuery)]
        private RememberInstance m_Prefab;

        [SerializeField]
        private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetRotation m_Rotation = GetRotationCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetGameObject m_Parent = GetGameObjectNone.Create();

        [SerializeField]
        private PropertySetGameObject m_Save = SetGameObjectNone.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Instantiate {this.m_Prefab?.name}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Vector3 position = this.m_Position.Get(args);
            Quaternion rotation = this.m_Rotation.Get(args);

            Transform parent = this.m_Parent?.Get<Transform>(args);

            if (m_Prefab != null && SaveLoadManager.Instance.IsLoading == false)
            {
                var instance = SaveLoadInstanceManager.Instance.InstantiatePrefab(m_Prefab, position, rotation, parent);
                this.m_Save.Set(instance.gameObject, args);
            }

            return DefaultResult;
        }
    }
}