using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Title("On Instantiate Self")]
    [Category("Reminstance/On Instantiate Self")]
    [Description("This trigger will Executed when i instantied in the world and this is trigged just one time.\n TIP: If you choose all of them in the event type parameter, this component will only fire once when one of the chosen types is satisfied. In other words, it will not be triggered once for each type chosen.")]

    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayArrowDown))]
    [Keywords("Load", "Loaded", "Reminstance", "Remember", "Instance", "Instantied", "Was", "Iwas", "Droped")]

    [Serializable]
    public class TriggerReminstanceOnInstance : VisualScripting.Event
    {
        public enum InstanceEventType
        {
            OnInstantiedAfterLoadGame,
            OnInstantiate,
            OnDroped,
            All
        }
        [Tooltip("The Instacing stages type:\n"
        + "     - <b>OnInstantiedAfterLoadGame:</b>\nWill tigger only after load game. In this momenti, this instance has been loaded and instantied in the world\n"
        + "     - <b>OnInstantiate:</b>\nUsed for any RememberInstance Objects instiated in the world. Dont trigger after load game. Dont work for Inventory Items\n"
        + "     - <b>OnDroped:</b>\nUsed for any Inventory Item instiated in the world. Dont trigger after load game. Dont work for Other Instances, just for inventory items\n"
        + "     - <b>All:</b>\nTrigged in any instancing type.\n")]
        [SerializeField] private InstanceEventType m_eventType;

        bool _instantied = false;

        protected override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);

            switch (m_eventType)
            {
                case InstanceEventType.OnInstantiate:
                    SaveLoadInstanceManager.OnRememberInstantied += OnInstantied;
                    break;
                case InstanceEventType.OnDroped:
                    SaveLoadInstanceManager.OnRememberDroped += OnInstantied;
                    break;
                case InstanceEventType.OnInstantiedAfterLoadGame:
                    SaveLoadInstanceManager.OnRememberLoaded += OnInstantied;
                    break;
                case InstanceEventType.All:
                    SaveLoadInstanceManager.OnRememberInstantied += OnInstantied;
                    SaveLoadInstanceManager.OnRememberDroped += OnInstantied;
                    SaveLoadInstanceManager.OnRememberLoaded += OnInstantied;
                    break;
            }

            SaveLoadInstanceManager.OnRememberInstantied += OnInstantied;
        }

        protected override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);

            SaveLoadInstanceManager.OnRememberInstantied -= OnInstantied;
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            SaveLoadInstanceManager.OnRememberDroped -= OnInstantied;

        }

        private void OnInstantied(RememberInstance instance)
        {
            if (instance == null) return;
            if (ApplicationManager.IsExiting) return;

            if (this.Self != instance.gameObject) { return; }
            if (_instantied) { return; }
            _instantied = true;
            _ = this.m_Trigger.Execute();
            OnDisable(m_Trigger);

        }


    }
}