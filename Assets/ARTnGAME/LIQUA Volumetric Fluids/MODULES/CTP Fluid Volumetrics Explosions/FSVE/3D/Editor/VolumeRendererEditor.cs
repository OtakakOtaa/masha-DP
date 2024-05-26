using ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Behaviours.Volume_Renderer;
using UnityEditor;
using UnityEngine;


namespace Artngame.LIQUA.FSVE
{
    [CustomEditor(typeof(VolumeRenderer))]
    public class VolumeRendererEditor : UnityEditor.Editor
    {
        VolumeRenderer renderer;

        private void GUIStart()
        {
            serializedObject.Update();
            renderer = (VolumeRenderer)target;
        }


        private void GUIEnd()
        {
            serializedObject.ApplyModifiedProperties();// Apply the changed properties
        }


        public override void OnInspectorGUI()
        {
            GUIStart();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("texture"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomise_colour"), true);

            if (renderer.randomise_colour && Application.isPlaying)// If randomise colour is enabled
            {
                if (GUILayout.Button("Randomise Colour"))// Add a color re-roll button
                    renderer.RandomiseColour();
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("on_colour_change"), true);
            GUIEnd();
        }

    }
}
