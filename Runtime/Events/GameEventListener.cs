using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace BG.UnityUtils
{
    public class GameEventListener : MonoBehaviour
    {
        public List<EventAndResponse> eventAndResponses = new List<EventAndResponse>();

        void OnEnable()
        {
            RegisterListeners();
        }

        void OnDisable()
        {
            UnregisterListeners();
        }

        public void RegisterListeners()
        {
            if (eventAndResponses.Count >= 1)
            {
                foreach (EventAndResponse eventAndResponse in eventAndResponses)
                {
                    eventAndResponse.Event.RegisterListener(this);
                }
            }
        }

        public void UnregisterListeners()
        {
            if (eventAndResponses.Count >= 1)
            {
                foreach (EventAndResponse eventAndResponse in eventAndResponses)
                {
                    eventAndResponse.Event.UnregisterListener(this);
                }
            }
        }

        public void OnEventRaised(GameEvent passedEvent)
        {
            for (int i = eventAndResponses.Count - 1; i >= 0; i--)
            {
                if (passedEvent == eventAndResponses[i].Event)
                {
                    eventAndResponses[i].EventRaised();
                }
            }
        }
    }


    [System.Serializable]
    public class EventAndResponse
    {
        [System.Flags]
        public enum ResponseTypes
        {
            None = 0,
            String = 1 << 0,
            Int = 1 << 1,
            Float = 1 << 2,
            Bool = 1 << 3,
            GameObject = 1 << 4,
            All = ~0
        }

        public string Name;
        public GameEvent Event;
        public ResponseTypes ResponseType;
        public UnityEvent Response;
        public ResponseWithString responseForString;
        public ResponseWithInt responseForInt;
        public ResponseWithFloat responseForFloat;
        public ResponseWithBool responseForBool;
        public ResponseWithGameObject responseForGameObject;

        public void EventRaised()
        {
            if (Response?.GetPersistentEventCount() >= 1)
            {
                Response?.Invoke();
            }

            if (responseForString?.GetPersistentEventCount() >= 1)
            {
                responseForString.Invoke(Event._String);
            }

            if (responseForInt?.GetPersistentEventCount() >= 1)
            {
                responseForInt.Invoke(Event._Int);
            }

            if (responseForFloat?.GetPersistentEventCount() >= 1)
            {
                responseForFloat.Invoke(Event._Float);
            }

            if (responseForBool?.GetPersistentEventCount() >= 1)
            {
                responseForBool.Invoke(Event._Bool);
            }

            if (responseForGameObject?.GetPersistentEventCount() >= 1)
            {
                responseForGameObject.Invoke(Event._GameObject);
            }
        }
    }

    [System.Serializable]
    public class ResponseWithString : UnityEvent<string>
    {
    }

    [System.Serializable]
    public class ResponseWithInt : UnityEvent<int>
    {
    }

    [System.Serializable]
    public class ResponseWithFloat : UnityEvent<float>
    {
    }

    [System.Serializable]
    public class ResponseWithBool : UnityEvent<bool>
    {
    }

    [System.Serializable]
    public class ResponseWithGameObject : UnityEvent<GameObject>
    {
    }
}