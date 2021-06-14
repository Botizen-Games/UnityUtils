namespace BG.UnityUtils
{
    [System.Serializable]
    public class IntReference : VariableReference<int>
    {
        public IntVariable VariableReference;
        public override TypeVariable<int> Variable { get { return VariableReference; } }
    }
}