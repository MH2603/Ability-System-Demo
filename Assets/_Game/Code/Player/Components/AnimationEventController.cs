using Foundation.EntitySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH.Character
{
    [Serializable]
    public class AnimatioEvent
    {
        [field: SerializeField] public string Name { get; private set; }
        public Action CallBack;
    }

    public class AnimationEventController : EntityComponent
    {
        #region -------------------- Inspector -------------------

        [SerializeField] private AnimatioEvent[] events;

        #endregion



        #region -------------------- Properties -------------------
        private Dictionary<string, AnimatioEvent> eventMap;
        #endregion



        #region -------------------- Public Methods -------------------

        public override void ManualStart()
        {
            base.ManualStart();
            SetUpEventMap();
        }


        public void RegisterEvent(string nameEvent, Action callback)
        {
            if ( !eventMap.ContainsKey(nameEvent) )
            {
                Debug.Log($" BUG Animation event: Not found to register with name = {nameEvent}");
                return;
            }

            eventMap[nameEvent].CallBack += callback;
        }

        public void UnRegisterEvent(string nameEvent, Action callback)
        {
            if (!eventMap.ContainsKey(nameEvent))
            {
                Debug.Log($" BUG Animation event: Not found to unregister with name = {nameEvent}");
                return;
            }

            eventMap[nameEvent].CallBack -= callback;
        }

        public void InvokeAction(string nameEvent)
        {
            if (!eventMap.ContainsKey(nameEvent))
            {
                Debug.Log($" BUG Animation event: Not found to Invoke with name = {nameEvent}");
                return;
            }

            eventMap[nameEvent].CallBack.Invoke();
        }

        #endregion

        private void SetUpEventMap()
        {
            eventMap = new();

            foreach (AnimatioEvent animatioEvent in events)
            {
                eventMap[animatioEvent.Name] = animatioEvent;
            }
        }

    }

}
