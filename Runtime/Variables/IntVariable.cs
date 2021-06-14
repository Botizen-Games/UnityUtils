using UnityEngine;

namespace BG.UnityUtils
{
    [CreateAssetMenu(fileName = "IntVariable", menuName = "Variables/Int")]
    public class IntVariable : TypeVariable<int>
    {
        public bool EqualTo(int value) => base.RuntimeValue == value;
        public bool NotEqualTo(int value) => base.RuntimeValue != value;
        public bool LessThan(int value) => base.RuntimeValue < value;
        public bool LessThanOrEqualTo(int value) => base.RuntimeValue <= value;
        public bool GreaterThan(int value) => base.RuntimeValue > value;
        public bool GreaterThanOrEqualTo(int value) => base.RuntimeValue >= value;
    }
}