using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Title("On Droped Self")]
    [Category("Reminstance/On Droped Self")]
    [Description("This trigger will Executed when i Droped from Inventory instantiation in the world")]

    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayArrowDown))]
    [Keywords("Reminstance", "Remember", "Instance", "Instantied", "Droped", "Inventory", "Item")]

    [Serializable]
    public class TriggerOnDropedSelf : VisualScripting.Event
    {

        bool _instantied = false;

        protected override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            SaveLoadInstanceManager.OnRememberDroped += OnInstantied;
        }

        protected override void OnDisable(Trigger trigger)
        {
            SaveLoadInstanceManager.OnRememberDroped -= OnInstantied;
            base.OnDisable(trigger);
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