using UnityEngine;

namespace BG.UnityUtils
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = "Variables/String")]
    public class StringVariable : TypeVariable<string>
    {
        public bool EqualTo(string value) => base.RuntimeValue == value;
        public bool NotEqualTo(string value) => base.RuntimeValue != value;
    }
}