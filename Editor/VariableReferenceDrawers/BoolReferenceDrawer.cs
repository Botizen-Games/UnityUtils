using UnityEditor;
using BG.UnityUtils.Runtime;

namespace BG.UnityUtils.Editor
{
    [CustomPropertyDrawer(typeof(BoolReference))]
    public class BoolReferenceDrawer : VariableReferenceDrawer { }
}