using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._2D.Behaviours
{
    public class DemoReloader : MonoBehaviour
    {
        private bool loading = false;


        void Update()
        {
            if (Input.GetKey(KeyCode.R))
                ReloadDemo();
        }


        public void ReloadDemo()
        {
            if (loading)
                return;

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            loading = true;
        }
    }
}