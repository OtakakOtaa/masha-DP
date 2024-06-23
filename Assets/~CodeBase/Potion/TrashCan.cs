using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;


namespace _CodeBase.Potion
{
    public sealed class TrashCan : ObjectKeeper
    {
        protected override void OnAwake()
        {
        }

        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            var trash = _inputManager.GameplayCursor.HandleItem as ICanDropToTrash;
            trash?.HandleDropToTrashCan();
        }
    }

    public interface ICanDropToTrash
    {
        void HandleDropToTrashCan();
    }
}