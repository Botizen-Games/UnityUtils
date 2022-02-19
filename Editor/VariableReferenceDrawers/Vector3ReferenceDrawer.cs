using UnityEditor;
using BG.UnityUtils.Runtime;

namespace BG.UnityUtils.Editor
{
    [CustomPropertyDrawer(typeof(Vector3Reference))]
    public class Vector3ReferenceDrawer : VariableReferenceDrawer { }
}