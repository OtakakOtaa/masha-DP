using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Classes.Fluid_Simulation_Modules
{
    public class FluidSimModule
    {
        protected const uint READ = 0; // For accessing grid sets
        protected const uint WRITE = 1;

        [SerializeField] protected ComputeShader compute_shader;


        public FluidSimModule() { }// Not sure if default keyword is in C#


        public FluidSimModule(ComputeShader _shader)
        {
            compute_shader = _shader;
        }


        protected void Swap(ComputeBuffer[] _grid)
        {
            ComputeBuffer temp = _grid[READ];
            _grid[READ] = _grid[WRITE];
            _grid[WRITE] = temp;
        }
    }
}
