using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Entities;
using Managers;
using UI.BottomWidgets;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotManager : PlayerComponent
{
    public static ShotManager Instance { get; private set; }
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shotDelay = 0.5f;
    [SerializeField] private float projectileSpeed = 4f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void FireShot(Entity shooter, Entity target)
    {
        if (shooter == null || target == null) return;

        var cover = CoverUtilities.GetImmediateCoverLevel(shooter.CurrentCell, target.CurrentCell, out var coverObject);
        
        ShotData shot = new ShotData()
        {
            Shooter = shooter,
            Target = target,
            Cover = cover,
            CoverObject = coverObject,
            TotalDamage = 20,
        };
        shot.ReturnDamage = (Random.value < target.Modifiers.PercentDamageReturnChance)
            ? (int)(target.Modifiers.PercentDamageReturnAmount * shot.TotalDamage) // Change to total damage
            : 0;

        StartCoroutine(FireSequence(shot));
    }

    private IEnumerator FireSequence(ShotData shot)
    {
        // Lock player controls
        InputManager.Instance.PlayerInput.Disable();
        BottomWidgetManager.Instance.Show(EBottomWidget.Movement);
        
        // Move camera to focus on both shooter & target
        Vector3 focusPoint = (shot.Shooter.transform.position + shot.Target.transform.position) / 2;
        CameraController.MoveToPosition(focusPoint);

        yield return new WaitForSeconds(shotDelay / 2);

        FireProjectile(shot);
    }

    private void FireProjectile(ShotData shot)
    {
        // Ensure CenterOfMass exists, otherwise use transform position as fallback
        Vector3 shooterPos = shot.Shooter.CenterOfMass != null ? shot.Shooter.CenterOfMass.position : shot.Shooter.transform.position;
        
        Vector3 targetPos;
        bool hit = DetermineHit(shot);
        if (hit)
        {
            targetPos = shot.Target.CenterOfMass != null ? shot.Target.CenterOfMass.position : shot.Target.transform.position;
        }
        else
        {
            targetPos = shot.CoverObject.transform.Find("CenterOfMass").position;
        }

        shot.Shooter.Actions.UseAction(ActionType.Weapon);
        
        GameObject projectile = Instantiate(projectilePrefab, shooterPos, Quaternion.identity);
        float duration = Vector3.Distance(shot.Shooter.transform.position, shot.Target.transform.position) /
                         projectileSpeed;
        projectile.transform.DOMove(targetPos, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (hit)
                {
                    shot.Target.TakeDamage(shot.TotalDamage);
                    shot.Shooter.TakeDamage(shot.ReturnDamage);
                }
                Destroy(projectile);
                ReturnToNormalState(shot.Shooter);
            });
    }

    private bool DetermineHit(ShotData shot)
    {
        if (shot.Cover == CoverTypes.FullCover) return false; // Shot blocked
        if (shot.Cover == CoverTypes.HalfCover) return UnityEngine.Random.value > 0.5f; // 50% chance to hit
        return true; // Always hit if no cover
    }

    private void ReturnToNormalState(Entity shooter)
    {
        // Unlock player controls
        InputManager.Instance.PlayerInput.Enable();

        if (shooter is Ally)
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerShotFired, shooter);
        }
        else
        {
            // TurnManager.Instance.EndEnemyTurn(); // right?
        }
    }
}
