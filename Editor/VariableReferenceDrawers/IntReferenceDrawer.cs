using UnityEditor;
using BG.UnityUtils.Runtime;

namespace BG.UnityUtils.Editor
{
    [CustomPropertyDrawer(typeof(IntReference))]
    public class IntReferenceDrawer : VariableReferenceDrawer { }
}