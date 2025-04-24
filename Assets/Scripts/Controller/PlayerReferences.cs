using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Controller
{
    public class PlayerReferences : MonoBehaviour
    {
        [SerializeField] private ActiveAllyController activeAllyController;
        [SerializeField] private AllySwitcher allySwitcher;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private CoverIndicator coverIndicator;
        [SerializeField] private ModeSwitcher modeSwitcher;
        [SerializeField] private MovementSelection movementSelection;
        [SerializeField] private ShotManager shotManager;
        [SerializeField] private TargetingSystem targetingSystem;
        [SerializeField] private ThrowableSelection throwableSelection;
        [SerializeField] private ThrowManager throwManager;

        public ActiveAllyController ActiveAllyController => activeAllyController;
        public AllySwitcher AllySwitcher => allySwitcher;
        public CameraController CameraController => cameraController;
        public CoverIndicator CoverIndicator => coverIndicator;
        public ModeSwitcher ModeSwitcher => modeSwitcher;
        public MovementSelection MovementSelection => movementSelection;
        public ShotManager ShotManager => shotManager;
        public TargetingSystem TargetingSystem => targetingSystem;
        public ThrowableSelection ThrowableSelection => throwableSelection;
        public ThrowManager ThrowManager => throwManager;

        public void BuildReferences()
        {
            activeAllyController = GetComponent<ActiveAllyController>();
            allySwitcher = GetComponent<AllySwitcher>();
            cameraController = GetComponent<CameraController>();
            coverIndicator = GetComponent<CoverIndicator>();
            modeSwitcher = GetComponent<ModeSwitcher>();
            movementSelection = GetComponent<MovementSelection>();
            shotManager = GetComponent<ShotManager>();
            targetingSystem= GetComponent<TargetingSystem>();
            throwableSelection = GetComponent<ThrowableSelection>();
            throwManager = GetComponent<ThrowManager>();
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
        public CoverIndicator CoverIndicator => References.CoverIndicator;
        public ModeSwitcher ModeSwitcher => References.ModeSwitcher;
        public MovementSelection MovementSelection => References.MovementSelection;
        public ShotManager ShotManager => References.ShotManager;
        public TargetingSystem TargetingSystem => References.TargetingSystem;
        public ThrowableSelection ThrowableSelection => References.ThrowableSelection;
        public ThrowManager ThrowManager => References.ThrowManager;
    }
}