using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.DEMOS.Demo_Assets_Scripts
{
    public class healthSphereLIQUA : MonoBehaviour
    {
        public float health = 100;
        public float shaderMaxHeightFront = 0.30f;
        public float shaderMaxHeightBack = 0.15f;
        public Material healthFront;
        public Material healthBack;

        public bool sinMotion = false;
        public float sinMax = 100;
        public float sinOffset = 50;
        //public float sinAmp = 1;
        public float sinFreq = 1;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (sinMotion)
            {
                health = (sinMax / 2) + (sinMax / 2) * Mathf.Sin(Time.fixedTime * sinFreq) - sinOffset;
            }

            if (healthFront != null)
            {
                healthFront.SetFloat("_fill_Level", (health / 100) * shaderMaxHeightFront);
            }
            if (healthBack != null)
            {
                healthBack.SetFloat("_fill_Level", (health / 100) * shaderMaxHeightBack);
            }
        }
    }
}