using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.DEMOS.Demo_Assets_Scripts
{
    public class enableWithDelayLIQUA : MonoBehaviour
    {
        public GameObject objectToEnable;
        public float enableAfter = 2;
        bool enabledA = false;

        public bool enableMeshRenderer = false;
        public MeshRenderer rendererA;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!enabledA && Time.fixedTime > enableAfter)
            {
                if (enableMeshRenderer)
                {
                    if(rendererA != null && !rendererA.enabled)
                    {
                        rendererA.enabled = true;
                    }
                }
                else
                {
                    if (!objectToEnable.activeInHierarchy)
                    {
                        objectToEnable.SetActive(true);
                    }
                }
                enabledA = true;
            }
        }
    }
}