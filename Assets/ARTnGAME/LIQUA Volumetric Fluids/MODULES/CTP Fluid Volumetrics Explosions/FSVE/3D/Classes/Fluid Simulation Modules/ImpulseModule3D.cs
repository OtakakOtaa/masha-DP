using System;
using UnityEngine;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Classes.Fluid_Simulation_Modules
{
    [Serializable]
    public class ImpulseModule3D : FluidSimModule
    {
        public void ApplyImpulse(float _dt, Vector3 _size, float _amount, float _impulse_radius,
            Vector3 _impulse_position, ComputeBuffer[] _grids, intVector3 _thread_count)
        {
            compute_shader.SetVector("size", _size);
            compute_shader.SetFloat("radius", _impulse_radius);
            compute_shader.SetFloat("source_amount", _amount);
            compute_shader.SetFloat("dt", _dt);
            compute_shader.SetVector("source_pos", _impulse_position);

            int kernel_id = compute_shader.FindKernel("Impulse");
            compute_shader.SetBuffer(kernel_id, "read_R", _grids[READ]);
            compute_shader.SetBuffer(kernel_id, "write_R", _grids[WRITE]);
            compute_shader.Dispatch(kernel_id, _thread_count.x, _thread_count.y, _thread_count.z);
            Swap(_grids);
        }

        //void ApplyExtinguishmentImpulse(float dt, float amount, ComputeBuffer[] buffer)
        public void ApplyExtinguishmentImpulse(float _dt, Vector3 _size, float _amount, float _impulse_radius,
            Vector3 _impulse_position, ComputeBuffer[] _grids, intVector3 _thread_count)
        {
            compute_shader.SetVector("size", _size);
            compute_shader.SetFloat("radius", _impulse_radius);
            compute_shader.SetFloat("source_amount", _amount);
            compute_shader.SetFloat("dt", _dt);
            compute_shader.SetVector("source_pos", _impulse_position);

            int kernel_id = compute_shader.FindKernel("Impulse");
            compute_shader.SetBuffer(kernel_id, "read_R", _grids[READ]);
            compute_shader.SetBuffer(kernel_id, "write_R", _grids[WRITE]);
            compute_shader.Dispatch(kernel_id, _thread_count.x, _thread_count.y, _thread_count.z);
            Swap(_grids);

            //m_applyImpulse.SetVector("_Size", m_size);
            //m_applyImpulse.SetFloat("_Radius", m_inputRadius);
            //m_applyImpulse.SetFloat("_Amount", amount);
            //m_applyImpulse.SetFloat("_DeltaTime", dt);
            //m_applyImpulse.SetVector("_Pos", m_inputPos);
            //m_applyImpulse.SetFloat("_Extinguishment", m_reactionExtinguishment);

            //m_applyImpulse.SetBuffer(1, "_Read", buffer[READ]);
            //m_applyImpulse.SetBuffer(1, "_Write", buffer[WRITE]);
            //m_applyImpulse.SetBuffer(1, "_Reaction", m_reaction[READ]);

            //m_applyImpulse.Dispatch(1, (int)m_size.x / NUM_THREADS, (int)m_size.y / NUM_THREADS, (int)m_size.z / NUM_THREADS);

            //Swap(buffer);
        }

    }
}
