using UnityEditor;
using BG.UnityUtils.Runtime;

namespace BG.UnityUtils.Editor
{
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : VariableReferenceDrawer { }
}