using UnityEngine;

namespace BG.UnityUtils.Runtime
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = "Variables/Float")]
    public class FloatVariable : TypeVariable<float>
    {
        public bool EqualTo(float value) => base.RuntimeValue == value;
        public bool NotEqualTo(float value) => base.RuntimeValue != value;
        public bool LessThan(float value) => base.RuntimeValue < value;
        public bool LessThanOrEqualTo(float value) => base.RuntimeValue <= value;
        public bool GreaterThan(float value) => base.RuntimeValue > value;
        public bool GreaterThanOrEqualTo(float value) => base.RuntimeValue >= value;
    }
}