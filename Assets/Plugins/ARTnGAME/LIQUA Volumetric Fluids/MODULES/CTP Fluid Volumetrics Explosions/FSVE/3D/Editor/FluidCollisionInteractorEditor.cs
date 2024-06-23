using ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Behaviours.Fluid_Interactors;
using ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Behaviours.Fluid_Simulators;
using UnityEditor;


namespace Artngame.LIQUA.FSVE
{
    [CustomEditor(typeof(FluidCollisionInteractor))]
    public class FluidCollisionInteractorEditor : UnityEditor.Editor
    {
        FluidSimulation3D simulation = null;
        FluidCollisionInteractor collision_interactor = null;

        public override void OnInspectorGUI()
        {
            collision_interactor = (FluidCollisionInteractor)target;
            simulation = collision_interactor.GetComponent<FluidSimulation3D>();

            EditorGUI.BeginDisabledGroup(true);// Default script ref
            EditorGUILayout.ObjectField("Script:", 
                MonoScript.FromMonoBehaviour(collision_interactor), typeof(FluidCollisionInteractor), false);
            EditorGUI.EndDisabledGroup();

            if (simulation == null)// Only allow transform setting if not on a fluid sim
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fluid_simulation"), false);
        }
    }
}
