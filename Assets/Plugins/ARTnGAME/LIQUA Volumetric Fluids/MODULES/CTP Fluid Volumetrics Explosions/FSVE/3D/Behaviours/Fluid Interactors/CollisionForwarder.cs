using System;
using UnityEngine;
using UnityEngine.Events;

namespace ARTnGAME.LIQUA_Volumetric_Fluids.MODULES.CTP_Fluid_Volumetrics_Explosions.FSVE._3D.Behaviours.Fluid_Interactors
{
    [Serializable] public class CollisionEvent : UnityEvent<Collision> {}
    [Serializable] public class ColliderEvent : UnityEvent<Collider> { }

    public class CollisionForwarder : MonoBehaviour
    {
        public ColliderEvent on_trigger_enter = new ColliderEvent();
        public ColliderEvent on_trigger_stay = new ColliderEvent();
        public ColliderEvent on_trigger_exit = new ColliderEvent();

        public CollisionEvent on_collision_enter = new CollisionEvent();
        public CollisionEvent on_collision_stay = new CollisionEvent();
        public CollisionEvent on_collision_exit = new CollisionEvent();


        private void OnTriggerEnter(Collider _other)
        {
            on_trigger_enter.Invoke(_other);
        }
    

        private void OnTriggerStay(Collider _other)
        {
            on_trigger_stay.Invoke(_other);
        }


        private void OnTriggerExit(Collider _other)
        {
            on_trigger_exit.Invoke(_other);
        }


        private void OnCollisionEnter(Collision _collision)
        {
            on_collision_enter.Invoke(_collision);
        }


        private void OnCollisionStay(Collision _collision)
        {
            on_collision_stay.Invoke(_collision);
        }


        private void OnCollisionExit(Collision _collision)
        {
            on_collision_exit.Invoke(_collision);
        }

    }
}
