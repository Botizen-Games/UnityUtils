using UnityEngine;
using System.Collections.Generic;

namespace BG.UnityUtils
{
    public abstract class RuntimeCollection<T> : ScriptableObject
    {
        public List<T> Items { get => items; }

        private List<T> items = new List<T>();

        public void Add(T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }

        public void Remove(T item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
            }
        }
    }
}