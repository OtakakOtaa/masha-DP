using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Behaviours.Demo_Scripts
{
    namespace Demo
    {
        public class Rotator : MonoBehaviour
        {
            [SerializeField] float rotation_speed = -50;
            [SerializeField] Vector3 rotation_axis = Vector3.up;

            private bool rotate = true;

            private void LateUpdate()
            {
                if (rotate)
                    transform.Rotate(rotation_axis.normalized * Time.deltaTime * rotation_speed);
            }


            public void ToggleActive()
            {
                rotate = !rotate;
            }
        }
    }
}
