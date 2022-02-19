namespace BG.UnityUtils.Runtime
{
    public abstract class VariableReference<T>
    {
        public bool UseConstant = true;
        public T ConstantValue;
        public virtual TypeVariable<T> Variable { get { return null; } }

        public T Value
        {
            get { return UseConstant ? ConstantValue : Variable.RuntimeValue; }
            set
            {
                if (UseConstant)
                {
                    ConstantValue = value;
                }
                else
                {
                    Variable.RuntimeValue = value;
                }
            }
        }
    }
}