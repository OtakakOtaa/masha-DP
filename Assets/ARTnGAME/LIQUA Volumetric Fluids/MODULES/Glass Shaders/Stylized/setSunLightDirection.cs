using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.Glass_Shaders.Stylized
{
    [ExecuteInEditMode]
    public class setSunLightDirection : MonoBehaviour
    {
        void Update()
        {
            Shader.SetGlobalVector("_LightDirectionVec", -transform.forward);

        }
    }
}