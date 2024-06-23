using System.Collections;
using System.Collections.Generic;
using ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.Unity_Simple_Liquid.Scripts;
using UnityEditor;
using UnityEngine;

namespace Artngame.LIQUA.UnitySimpleLiquid
{
    [CustomEditor(typeof(SplitController))]
    public class SplitControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var split = (SplitController)target;

            GUI.enabled = false;
            EditorGUILayout.Toggle("Is Spliting", split.IsSpliting);
            GUI.enabled = true;
        }
    }
}