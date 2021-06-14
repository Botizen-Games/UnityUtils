namespace BG.UnityUtils
{
    [System.Serializable]
    public class FloatReference : VariableReference<float>
    {
        public FloatVariable VariableReference;
        public override TypeVariable<float> Variable { get { return VariableReference; } }
    }
}