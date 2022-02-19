using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BG.UnityUtils.Runtime
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
                    eventAndResponse.GameEvent.RegisterListener(this);
                }
            }
        }

        public void UnregisterListeners()
        {
            if (eventAndResponses.Count >= 1)
            {
                foreach (EventAndResponse eventAndResponse in eventAndResponses)
                {
                    eventAndResponse.GameEvent.UnregisterListener(this);
                }
            }
        }

        public void OnEventRaised(GameEvent gameEvent)
        {
            for (int i = eventAndResponses.Count - 1; i >= 0; i--)
            {
                if (gameEvent == eventAndResponses[i].GameEvent)
                {
                    if (eventAndResponses[i].IsDelayed)
                    {
                        StartCoroutine(InvokeDelayedEvent(eventAndResponses[i]));
                    }
                    else
                    {
                        eventAndResponses[i].EventRaised();
                    }
                }
            }
        }

        IEnumerator InvokeDelayedEvent(EventAndResponse eventAndResponse)
        {
            yield return new WaitForSeconds(eventAndResponse.Delay);
            eventAndResponse.EventRaised();
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

        public string Name { get => name; set => name = value; }
        public GameEvent GameEvent { get => gameEvent; set => gameEvent = value; }
        public bool IsDelayed { get => isDelayed; set => isDelayed = value; }
        public float Delay { get => delay; set => delay = value; }
        public ResponseTypes ResponseType { get => responseType; set => responseType = value; }
        public UnityEvent Response { get => response; set => response = value; }
        public ResponseWithString ResponseForString { get => responseForString; set => responseForString = value; }
        public ResponseWithInt ResponseForInt { get => responseForInt; set => responseForInt = value; }
        public ResponseWithFloat ResponseForFloat { get => responseForFloat; set => responseForFloat = value; }
        public ResponseWithBool ResponseForBool { get => responseForBool; set => responseForBool = value; }
        public ResponseWithGameObject ResponseForGameObject { get => responseForGameObject; set => responseForGameObject = value; }

        [SerializeField] private string name;
        [SerializeField] private GameEvent gameEvent;
        [SerializeField] private bool isDelayed;
        [SerializeField] private float delay;
        [SerializeField] private ResponseTypes responseType;
        [SerializeField] private UnityEvent response;
        [SerializeField] private ResponseWithString responseForString;
        [SerializeField] private ResponseWithInt responseForInt;
        [SerializeField] private ResponseWithFloat responseForFloat;
        [SerializeField] private ResponseWithBool responseForBool;
        [SerializeField] private ResponseWithGameObject responseForGameObject;

        public void EventRaised()
        {
            InvokeResponse(response);
            InvokeResponse(responseForString, gameEvent._String);
            InvokeResponse(responseForInt, gameEvent._Int);
            InvokeResponse(responseForFloat, gameEvent._Float);
            InvokeResponse(responseForBool, gameEvent._Bool);
            InvokeResponse(responseForGameObject, gameEvent._GameObject);
        }

        void InvokeResponse(UnityEvent response)
        {
            if (response?.GetPersistentEventCount() >= 1)
            {
                response.Invoke();
            }
        }

        void InvokeResponse<T>(UnityEvent<T> response, T obj)
        {
            if (response?.GetPersistentEventCount() >= 1)
            {
                response.Invoke(obj);
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