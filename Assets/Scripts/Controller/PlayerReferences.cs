using UnityEngine;

namespace Controller
{
    public class PlayerReferences : MonoBehaviour
    {
        [SerializeField] private ActiveAllyController activeAllyController;
        [SerializeField] private AllySwitcher allySwitcher;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private MovementSelection movementSelection;

        public ActiveAllyController ActiveAllyController => activeAllyController;
        public AllySwitcher AllySwitcher => allySwitcher;
        public CameraController CameraController => cameraController;
        public MovementSelection MovementSelection => movementSelection;

        public void BuildReferences()
        {
            activeAllyController = GetComponent<ActiveAllyController>();
            allySwitcher = GetComponent<AllySwitcher>();
            cameraController = GetComponent<CameraController>();
            movementSelection = GetComponent<MovementSelection>();
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
        public MovementSelection MovementSelection => References.MovementSelection;
    }
}