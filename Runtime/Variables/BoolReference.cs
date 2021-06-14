namespace BG.UnityUtils
{
    [System.Serializable]
    public class BoolReference : VariableReference<bool>
    {
        public BoolVariable VariableReference;
        public override TypeVariable<bool> Variable { get { return VariableReference; } }
    }
}