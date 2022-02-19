using UnityEngine;

namespace BG.UnityUtils.Runtime
{
    [System.Serializable]
    public class Vector3Reference : VariableReference<Vector3>
    {
        public Vector3Variable VariableReference;
        public override TypeVariable<Vector3> Variable { get { return VariableReference; } }
    }
}