using UnityEngine;
using System.Collections.Generic;

namespace BG.UnityUtils.Runtime
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {
        public string _String { get; set; }
        public int _Int { get; set; }
        public float _Float { get; set; }
        public bool _Bool { get; set; }
        public GameObject _GameObject { get; set; }

        private List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(this);
            }
        }

        public void RegisterListener(GameEventListener listener)
        {

            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}