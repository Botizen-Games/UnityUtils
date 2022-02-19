using UnityEngine;
using System;
using System.Collections.Generic;

namespace BG.UnityUtils.Runtime
{
    public abstract class RuntimeCollection<T> : ScriptableObject
    {
        public List<T> Items { get => items; }
        public T this[int index] { get => items[index]; set => items[index] = value; }
        public Type Type { get => typeof(T); }

        private List<T> items;

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

        public void Initialize() => items = new List<T>();
        public void Clear() => items.Clear();
        public bool Contains(T item) => items.Contains(item);
        public int IndexOf(T item) => items.IndexOf(item);
        public void RemoveAt(int index) => items.RemoveAt(index);
        public void Insert(int index, T item) => items.Insert(index, item);
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
        public T[] ToArray() => items.ToArray();
    }
}