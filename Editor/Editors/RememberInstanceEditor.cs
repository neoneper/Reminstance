using GameCreator.Runtime.Common;
using GameCreator.Runtime.Reminstance;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Reminstance
{
    [CustomEditor(typeof(RememberInstance))]
    public class RememberInstanceEditor : UnityEditor.Editor
    {
        private static readonly Length DEFAULT_MARGIN_TOP = new Length(5, LengthUnit.Pixel);
        private VisualElement m_Root;

        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement
            {
                style =
                {
                    marginTop = DEFAULT_MARGIN_TOP
                }
            };

            SerializedProperty prefabID = this.serializedObject.FindProperty("m_prefabGUID");
            SerializedProperty memories = this.serializedObject.FindProperty("m_Memories");
            SerializedProperty saveUniqueID = this.serializedObject.FindProperty("m_SaveUniqueID");

            PropertyField fieldMemories = new PropertyField(memories);
            PropertyField fieldUniqueID = new PropertyField(saveUniqueID);
            Label label = new Label($"Prfab GUID: {prefabID.stringValue}");

            this.m_Root.Add(fieldMemories);
            this.m_Root.Add(fieldUniqueID);
            this.m_Root.Add(label);

            return this.m_Root;
        }

      
    }
}
