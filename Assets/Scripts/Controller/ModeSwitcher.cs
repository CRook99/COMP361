using Managers;

namespace Controller 
{
    public enum ControlMode
    {
        Move,
        Selection
    }
    
    public class ModeSwitcher : PlayerComponent
    {
        public ControlMode CurrentMode;

        private void Awake()
        {
            SwitchMode(ControlMode.Move);
        }

        public void SwitchMode(ControlMode newMode)
        {
            if (newMode == CurrentMode) return;

            CurrentMode = newMode;
            EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, CurrentMode);
        }
    }
}