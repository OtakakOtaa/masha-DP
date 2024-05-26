using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.Unity_Simple_Liquid.Scripts
{
    public class EndlessContainer : MonoBehaviour
    {
        private LiquidContainer liquidContainer;

        private void Awake()
        {
            liquidContainer = GetComponent<LiquidContainer>();
        }

        // Update is called once per frame
        void Update()
        {
            liquidContainer.FillAmountPercent = 1f;
        }
    }
}

