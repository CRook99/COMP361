using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class TargetingUIManager : MonoBehaviour
{
    public static TargetingUIManager Instance { get; private set; }

    [SerializeField] private Reticle _reticle;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowTarget(Enemy target)
    {
        if (target == null)
        {
            _reticle.Hide();
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
        _reticle.SetPosition(screenPosition);
    }

    public void HideReticle() { _reticle.Hide(); }  
    
}
