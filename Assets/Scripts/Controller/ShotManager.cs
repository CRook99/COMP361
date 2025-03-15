using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Entities;
using Managers;
using UnityEngine;

public class ShotManager : PlayerComponent
{
    public static ShotManager Instance { get; private set; }
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shotDelay = 0.5f;
    [SerializeField] private float projectileSpeed = 1.5f;

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

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnPlayerConfirmShot, FireShot);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnPlayerConfirmShot, FireShot);
    }

    public void FireShot(object data)
    {
        if (data is not ShotData shot) return;
        if (shot.Shooter == null || shot.Target == null) return;

        // Get cover info
        shot.Cover = CoverUtilities.GetImmediateCoverOfTargetFromOrigin(shot.Shooter.CurrentCell, shot.Target.CurrentCell);

        // Lock player controlls
        InputManager.Instance.PlayerInput.Disable();

        // Move camera to focus on both shooter & target
        Vector3 focusPoint = (shot.Shooter.transform.position + shot.Target.transform.position) / 2;
        CameraController.MoveToPosition(focusPoint);

        // Delay before firing the projectile
        DOVirtual.DelayedCall(shotDelay / 2, () =>
        {
            if (CalculateHit(shot)) FireProjectile(shot);
        });

    }

    private void FireProjectile(ShotData shot)
    {
        // Ensure CenterOfMass exists, otherwise use transform position as fallback
        Vector3 shooterPos = shot.Shooter.CenterOfMass != null ? shot.Shooter.CenterOfMass.position : shot.Shooter.transform.position;
        Vector3 targetPos = shot.Target.CenterOfMass != null ? shot.Target.CenterOfMass.position : shot.Target.transform.position;

        GameObject projectile = Instantiate(projectilePrefab, shot.Shooter.CenterOfMass.position, Quaternion.identity);
        projectile.transform.DOMove(shot.Target.CenterOfMass.position, projectileSpeed).OnComplete(() =>
        {
            shot.Target.TakeDamage(shot.Damage);
            Destroy(projectile);
            ReturnToNormalState(shot.Shooter);
        });
    }

    private bool CalculateHit(ShotData shot)
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
            TurnManager.Instance.EndEnemyTurn(); // right?
        }
    }
}
