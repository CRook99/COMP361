using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using World;

namespace Controller
{
    public class ThrowableSelection : SelectionComponent
    {
        public ThrowableScriptableObject Throwable;
        public MoveArea throwAOE;
        private bool _active;
        public Mesh arcIndicator;
        public Material material;
        private HashSet<Cell> _throwableCells;
        private Cell cachedCell;
        private const int NUM_TRAGEC_BALLS = 3;
        private const float PARAB_TIME = 3.0f;
        private float _parabBFactor = 0f;
        private float[] _timers;
        private RenderParams rp;

        protected override void Awake()
        {
            base.Awake();

            _throwableCells = new HashSet<Cell>();
            _timers = new float[NUM_TRAGEC_BALLS];
            for (int i = 0; i < NUM_TRAGEC_BALLS; i++)
            {
                float j = i;
                _timers[i] = PARAB_TIME * j / NUM_TRAGEC_BALLS;
            }

            rp = new RenderParams(material);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            EventManager.Subscribe(EventTypes.OnPlayerBeginAbility, Activate);
            EventManager.Subscribe(EventTypes.OnPlayerEndAbility, Deactivate);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            if (!_active) return;

            base.Update();

            if (!_throwableCells.Contains(_currentCell)) return;

            //Drawing a parabola of balls to indicate throwing

            Vector3 v = ActiveAllyController.ActiveAlly.transform.position;
            Vector3 p = v;
            Vector2 start = new(v.x, v.z);
            Vector2 dist = _currentCell.Position - start;
            //Algebra magic shows that if the max height is half the length
            //then the "b" in the equation is sqrt(2 * distance)
            _parabBFactor = Mathf.Sqrt(2 * dist.magnitude);
            for (int i = 0; i < NUM_TRAGEC_BALLS; i++)
            {
                float t = _timers[i] / PARAB_TIME;
                v.x = dist.x * t + p.x;
                v.z = dist.y * t + p.z;
                //The x-intercepts of the parabola are 0 and b
                float yFactor = t * _parabBFactor;
                v.y = _parabBFactor * yFactor - Mathf.Pow(yFactor, 2f) + p.y;
                Graphics.RenderMesh(rp, arcIndicator, 0, Matrix4x4.Translate(v));

                _timers[i] += Time.deltaTime;
                _timers[i] %= PARAB_TIME;
            }

            //if (_currentCell == cachedCell) return;
            //cachedCell = _currentCell;
            
            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius, false);
            if (_affectedCells.Count == 0) return;
            
            throwAOE.GenerateMesh(_affectedCells, _currentCell.Position);
            throwAOE.transform.position = _currentCell.Position.ToVector3XZ();
        }

        private void Activate()
        {
            _active = true;

            var ability = ActiveAllyController.ActiveAlly.ChosenAbility;
            if (ability is ThrowableScriptableObject)
                Throwable = (ThrowableScriptableObject) ability;
            
            ModeSwitcher.SwitchMode(ControlMode.Selection);
            ActiveAllyController.ActiveAlly.EnableThrow(Throwable.ThrowRadius, out _throwableCells); // Placeholder value
            throwAOE.Show();
        }

        private void Deactivate()
        {
            _active = false;
            
            ModeSwitcher.SwitchMode(ControlMode.Move);
            ActiveAllyController.ActiveAlly.DisableThrow();
            throwAOE.Hide();
        }

        protected override void OnSelectTile(InputAction.CallbackContext context)
        {
            if (ModeSwitcher.CurrentMode != ControlMode.Selection || _currentCell == null || !_currentCell.Walkable) return;
            
            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius, false);
            ActiveAllyController.ActiveAlly.TryThrow(Throwable, _currentCell, _affectedCells);
        }
    }
}