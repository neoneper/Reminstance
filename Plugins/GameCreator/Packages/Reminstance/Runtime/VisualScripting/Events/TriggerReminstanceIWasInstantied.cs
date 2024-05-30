using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Title("I Was Instantied")]
    [Category("Reminstance/I Was Instantied")]
    [Description("Executed when i was loaded and instantied in the world")]

    [Image(typeof(IconDiskSolid), ColorTheme.Type.Blue)]
    [Keywords("Load", "Reminstance", "Remember", "Instance", "Instantied", "Was", "Iwas")]

    [Serializable]
    public class TriggerReminstanceIWasInstantied : VisualScripting.Event
    {

        protected override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            SaveLoadInstanceManager.Instance.OnRememberInstanceLoadedAndInstantied += OnInstantied;
        }

        protected override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            if (ApplicationManager.IsExiting) return;

            SaveLoadInstanceManager.Instance.OnRememberInstanceLoadedAndInstantied -= OnInstantied;

        }

        private void OnInstantied(RememberInstance instance)
        {
            if (instance == null) return;

            if (this.Self == instance.gameObject)
            {
                _ = this.m_Trigger.Execute(this.Self);
            }
        }


    }
}