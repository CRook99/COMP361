using UnityEngine;
using UnityEngine.UI;
using Entities;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] private Image healthFill; 
    private Transform _enemyTransform;
    private Camera _camera;
    private Enemy _enemy;

    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        _enemyTransform = enemy.transform;
        _camera = Camera.main;

        _enemy.OnHealthChanged += UpdateHealth;
        
        UpdateHealth(_enemy.CurrentHealth);
    }

    private void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.OnHealthChanged -= UpdateHealth;
        }
    }

    private void UpdateHealth(int newHealth)
    {
        healthFill.fillAmount = (float)newHealth / _enemy.Data.MaxHealth;
    }

    private void LateUpdate()
    {
        if (_enemyTransform == null) return;

        Vector3 screenPos = _camera.WorldToScreenPoint(_enemyTransform.position + Vector3.up * 2f);
        transform.position = screenPos;
    }
}
