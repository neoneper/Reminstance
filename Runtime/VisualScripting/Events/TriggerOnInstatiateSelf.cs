using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Title("On Instantiate Self")]
    [Category("Reminstance/On Instantiate Self")]
    [Description("This trigger will Executed when i instantied in the world")]

    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayArrowDown))]
    [Keywords("Reminstance", "Remember", "Instance", "Instantied")]

    [Serializable]
    public class TriggerOnInstatiateSelf : VisualScripting.Event
    {
        [SerializeField] private PropertyGetGameObject m_Me = GetGameObjectSelf.Create();
        [SerializeField] private bool m_allowOnLoaded = true;
        bool _instantied = false;

        protected override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            SaveLoadInstanceManager.OnRememberInstantied -= OnInstantied;
            SaveLoadInstanceManager.OnRememberInstantied += OnInstantied;

            if (m_allowOnLoaded)
            {
                SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
                SaveLoadInstanceManager.OnRememberLoaded += OnInstantied;
            }
        }
        protected override void OnDestroy(Trigger trigger)
        {
            SaveLoadInstanceManager.OnRememberInstantied -= OnInstantied;
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            base.OnDestroy(trigger);
        }

        protected override void OnDisable(Trigger trigger)
        {
            SaveLoadInstanceManager.OnRememberInstantied -= OnInstantied;
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            base.OnDisable(trigger);
        }

        private void OnInstantied(RememberInstance instance)
        {
            if (_instantied) { return; }
            if (instance == null) return;
            if (ApplicationManager.IsExiting) return;
            if (this == null) { return; }
            if (this.m_Me == null) { return; }
            if (this.m_Me.Get(new Args(m_Trigger)) != instance.gameObject) { return; }
          
            _instantied = true;
            SaveLoadInstanceManager.OnRememberInstantied -= OnInstantied;
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            _ = this.m_Trigger.Execute();

        }


    }
}