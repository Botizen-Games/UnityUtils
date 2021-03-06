using UnityEngine;

namespace BG.UnityUtils.Runtime
{
    public abstract class TypeVariable<T> : ScriptableObject
    {
        public T DefaultValue;
        public T RuntimeValue { get; set; }

        protected void OnEnable()
        {
            RuntimeValue = DefaultValue;
        }
    }
}