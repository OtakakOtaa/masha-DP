using System.Collections;
using System.Collections.Generic;
using ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.Unity_Simple_Liquid.Scripts;
using UnityEditor;
using UnityEngine;

namespace Artngame.LIQUA.UnitySimpleLiquid
{
    [CustomEditor(typeof(LiquidContainer))]
    public class LiquidContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var liquid = (LiquidContainer)target;

            if (liquid.CustomVolume)
            {
                var newVolume = EditorGUILayout.FloatField("Volume (liters):", liquid.Volume);
                liquid.Volume = newVolume > 0 ? newVolume : 0.01f;
            }
            else
            {
                GUI.enabled = false;

                var calculatedVolume = liquid.CalculateVolume();
                EditorGUILayout.FloatField("Volume (liters):", calculatedVolume);
                liquid.Volume = calculatedVolume;

                GUI.enabled = true;
            }

 

        }
    }
}