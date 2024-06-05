using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

using UnityEngine;

namespace GameCreator.Runtime.Reminstance
{
    [Title("On Loaded Self")]
    [Category("Reminstance/On Loaded Self")]
    [Description("This trigger will Executed when i loaded and instantied in the world. Itens and Objects will be trigged")]

    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayArrowDown))]
    [Keywords("Reminstance", "Remember", "Instance", "Instantied", "Loaded", "Inventory", "Item")]

    [Serializable]
    public class TriggerOnLoadedSelf : VisualScripting.Event
    {
        [SerializeField] private PropertyGetGameObject m_Me = GetGameObjectSelf.Create();

        bool _instantied = false;

        protected override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            SaveLoadInstanceManager.OnRememberLoaded += OnInstantied;
        }
        protected override void OnDisable(Trigger trigger)
        {
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            base.OnDisable(trigger);
        }
        protected override void OnDestroy(Trigger trigger)
        {
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            base.OnDestroy(trigger);
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
            SaveLoadInstanceManager.OnRememberLoaded -= OnInstantied;
            _ = this.m_Trigger.Execute();
          

        }


    }
}