namespace BG.UnityUtils.Runtime
{
    [System.Serializable]
    public class StringReference : VariableReference<string>
    {
        public StringVariable VariableReference;
        public override TypeVariable<string> Variable { get { return VariableReference; } }
    }
}