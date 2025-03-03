using UnityEngine;

namespace Controller
{
    public class PlayerReferences : MonoBehaviour
    {
        [SerializeField] private ActiveAllyController activeAllyController;
        [SerializeField] private AllySwitcher allySwitcher;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ModeSwitcher modeSwitcher;
        [SerializeField] private MovementSelection movementSelection;
        [SerializeField] private TargetingSystem targetingSystem;

        public ActiveAllyController ActiveAllyController => activeAllyController;
        public AllySwitcher AllySwitcher => allySwitcher;
        public CameraController CameraController => cameraController;
        public ModeSwitcher ModeSwitcher => modeSwitcher;
        public MovementSelection MovementSelection => movementSelection;
        public TargetingSystem TargetingSystem => targetingSystem;

        public void BuildReferences()
        {
            activeAllyController = GetComponent<ActiveAllyController>();
            allySwitcher = GetComponent<AllySwitcher>();
            cameraController = GetComponent<CameraController>();
            modeSwitcher = GetComponent<ModeSwitcher>();
            movementSelection = GetComponent<MovementSelection>();
            targetingSystem= GetComponent<TargetingSystem>();
        }
    }

    public abstract class PlayerComponent : MonoBehaviour
    {
        private PlayerReferences _playerReferences;

        public PlayerReferences References
        {
            get
            {
                if (_playerReferences == null && !TryGetComponent(out _playerReferences))
                    _playerReferences = GetComponentInChildren<PlayerReferences>();

                if (_playerReferences == null)
                    _playerReferences = GetComponentInParent<PlayerReferences>();

                return _playerReferences;
            }
        }

        public ActiveAllyController ActiveAllyController => References.ActiveAllyController;
        public AllySwitcher AllySwitcher => References.AllySwitcher;
        public CameraController CameraController => References.CameraController;
        public ModeSwitcher ModeSwitcher => References.ModeSwitcher;
        public MovementSelection MovementSelection => References.MovementSelection;
        public TargetingSystem TargetingSystem => References.TargetingSystem;
    }
}