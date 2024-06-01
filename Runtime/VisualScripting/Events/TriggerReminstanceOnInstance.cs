using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Title("On Instance")]
    [Category("Reminstance/On Instance")]
    [Description("Executed when i instantied in the world")]

    [Image(typeof(IconDiskSolid), ColorTheme.Type.Yellow)]
    [Keywords("Load", "Loaded", "Reminstance", "Remember", "Instance", "Instantied", "Was", "Iwas", "Droped")]

    [Serializable]
    public class TriggerReminstanceOnInstance : VisualScripting.Event
    {
        public enum InstanceEventType
        {
            OnInstanceLoaded,
            OnInstantied,
            OnItemDroped,
            All
        }

        [SerializeField] InstanceEventType m_eventType;

        protected override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);

            switch (m_eventType)
            {
                case InstanceEventType.OnInstantied:
                    SaveLoadInstanceManager.OnRememberInstantied += OnInstantied;
                    break;
                case InstanceEventType.OnItemDroped:
                    SaveLoadInstanceManager.OnRememberDroped += OnInstantied;
                    break;
                case InstanceEventType.OnInstanceLoaded:
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

            _ = this.m_Trigger.Execute(this.Self);
        }


    }
}