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
        HintManager.Instance.FulfilTutorial(TutorialSteps.Shooting);
        ShotData shot = CreateShotData(shooter, target, shooter.CurrentCell);
        StartCoroutine(FireSequence(shot));
    }

    // Useful for when we want until the shot as reached its destination
    public IEnumerator FireShotEnumerator(Entity shooter, Entity target, Cell ShootingCell)
    {
        ShotData shot = CreateShotData(shooter, target, ShootingCell);
        yield return FireSequence(shot, true);
    }

    private ShotData CreateShotData(Entity shooter, Entity target, Cell shootingCell)
    {
        var cover = CoverUtilities.GetImmediateCoverLevel(shooter.CurrentCell, target.CurrentCell, out var coverObject);

        ShotData shot = new ShotData()
        {
            Shooter = shooter,
            Target = target,
            ShootingCell = shootingCell,
            Cover = cover,
            CoverObject = coverObject,
            TotalDamage = shooter.GetModifiedWeaponDamage(),
        };
        shot.ReturnDamage = (Random.value < target.Modifiers.PercentDamageReturnChance)
            ? (int)(target.Modifiers.PercentDamageReturnAmount * shot.TotalDamage) // Change to total damage
            : 0;

        return shot;
    }

    // The fire_projectile_enumerator=false argument is useful when we want to wait until the shot
    // reached its destination. The enemy shooting sequence requires this
    private IEnumerator FireSequence(ShotData shot, bool fire_projectile_enumerator = false)
    {
        // Waits 0.1 sec before shooting
        yield return new WaitForSeconds(0.1f);
        Cell CurrentShooterCell = shot.Shooter.CurrentCell;
        if (CurrentShooterCell != shot.ShootingCell) yield return shot.Shooter.MoveToCell(shot.ShootingCell);

        // Lock player controls
        InputManager.Instance.PlayerInput.Disable();
        BottomWidgetManager.Instance.Show(EBottomWidget.Movement);

        // Move camera to focus on both shooter & target
        Vector3 focusPoint = (shot.Shooter.transform.position + shot.Target.transform.position) / 2;
        CameraController.MoveToPosition(focusPoint);
        yield return new WaitForSeconds(shotDelay / 2);

        if (!fire_projectile_enumerator)
            FireProjectile(shot);
        else
            yield return FireProjectileEnumerator(shot);

        // Waits 0.4 sec after shooting
        yield return new WaitForSeconds(0.4f);
        if (CurrentShooterCell != shot.ShootingCell) yield return shot.Shooter.MoveToCell(CurrentShooterCell);

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

            // stats manager
            if (shot.Cover == CoverTypes.HalfCover)
            {
                EventManager.TriggerEvent(EventTypes.OnChanceShotDodged, 1);
            }
        }

        shot.Shooter.Actions.UseAction(ActionType.Weapon);
        EventManager.TriggerEvent(EventTypes.OnShotTaken, 1);   // stats manager

        GameObject projectile = Instantiate(projectilePrefab, shooterPos, Quaternion.identity);
        float duration = Vector3.Distance(shot.Shooter.transform.position, shot.Target.transform.position) /
                         projectileSpeed;
        projectile.transform.DOMove(targetPos, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (hit)
                {
                    EventManager.TriggerEvent(EventTypes.OnShotLanded, 1);  // stats manager
                    shot.Target.TakeDamage(shot.TotalDamage);
                    shot.Shooter.TakeDamage(shot.ReturnDamage);
                }
                Destroy(projectile);
                ReturnToNormalState(shot.Shooter);
            });
    }

    // Same as FireProjectile() but returns a IEnumerator
    private IEnumerator FireProjectileEnumerator(ShotData shot)
    {
        // Ensure CenterOfMass exists, otherwise use transform position as fallback
        Vector3 shooterPos = shot.Shooter.CenterOfMass != null
            ? shot.Shooter.CenterOfMass.position
            : shot.Shooter.transform.position;

        Vector3 targetPos;
        bool hit = DetermineHit(shot);
        if (hit)
        {
            targetPos = shot.Target.CenterOfMass != null
                ? shot.Target.CenterOfMass.position
                : shot.Target.transform.position;
        }
        else
        {
            targetPos = shot.CoverObject.transform.Find("CenterOfMass").position;

            // stats manager
            if (shot.Cover == CoverTypes.HalfCover)
            {
                EventManager.TriggerEvent(EventTypes.OnChanceShotDodged, 1);
            }
        }

        shot.Shooter.Actions.UseAction(ActionType.Weapon);
        EventManager.TriggerEvent(EventTypes.OnShotTaken, 1);   // stats manager

        GameObject projectile = Instantiate(projectilePrefab, shooterPos, Quaternion.identity);
        float duration = Vector3.Distance(shot.Shooter.transform.position, shot.Target.transform.position) / projectileSpeed;

        // Create and start the tween
        Tween moveTween = projectile.transform.DOMove(targetPos, duration)
            .SetEase(Ease.Linear);

        // Wait until the tween is complete
        yield return moveTween.WaitForCompletion();

        // After the projectile reaches its target, handle hit logic
        if (hit)
        {
            EventManager.TriggerEvent(EventTypes.OnShotLanded, 1);   // stats manager
            shot.Target.TakeDamage(shot.TotalDamage);
            shot.Shooter.TakeDamage(shot.ReturnDamage);
        }
        Destroy(projectile);
        ReturnToNormalState(shot.Shooter);
    }


    private bool DetermineHit(ShotData shot)
    {
        if (shot.Cover == CoverTypes.FullCover) return false; // Shot blocked
        if (shot.Cover == CoverTypes.HalfCover) return Random.value > 0.5f + (shot.Target.Modifiers.EvasionBonusPercent / 100f); // 50 + evasion% chance to hit
        return Random.value > (shot.Target.Modifiers.EvasionBonusPercent / 100f); // Only consider evasion
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
