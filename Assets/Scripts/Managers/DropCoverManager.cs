using System;
using System.Collections;
using System.Collections.Generic;

using Controller;
using DG.Tweening;
using Entities;
using UI.BottomWidgets;
using UnityEngine;
using Utility;
using World;
using Utility.Serialization;
using Entities.DTOs;

namespace Managers
{
    [Serializable]
public class DropCoverManager : MonoBehaviour, IGameSerializable
{
    public float DropDelay;
    public float DropTime;

    private Transform _camera;

    [Header("Cover Prefab")]
    public GameObject CoverPrefab;

    private readonly List<Vector2Int> _droppedCoverPositions = new List<Vector2Int>();

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    public void HandleDropCover(Cell target)
    {
        if (!TacticsGrid.Instance.GetCell(target.Position).Walkable)
        {
            HintManager.Instance.Hint("Can't drop cover on this cell", HintLevel.Normal);
            return;
        }

        StartCoroutine(DropSequence(target));
    }

    private IEnumerator DropSequence(Cell target)
    {
        InputManager.Instance.PlayerInput.Disable();
        BottomWidgetManager.Instance.Show(EBottomWidget.AirSupportBase);
        AirSupportManager.Instance.Actions.UseAction(ActionType.DropCover);
        EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.DropCover);

        yield return new WaitForSeconds(DropDelay / 2);

        Vector3 spawnPoint = _camera.position + (_camera.right * 5f) + (_camera.up * -5f);
        Vector3 dropPoint = target.Position.ToVector3XZ(1f);
        GameObject cover = Instantiate(CoverPrefab, spawnPoint, Quaternion.identity);

        cover.transform.DOMove(dropPoint, DropTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                InputManager.Instance.PlayerInput.Enable();

                target.Walkable = false;
                TacticsGrid.Instance.AddCover(target, CoverTypes.FullCover);

                // Record for save
                _droppedCoverPositions.Add(new Vector2Int(
                    target.Position.x,
                    target.Position.y));
            });
    }


    public bool Validate() => true;

    public string Serialize()
    {
        var wrapper = new DropCoverDTO {
        positions = _droppedCoverPositions
            .ConvertAll(v => new SerializableVector2Int(v))
            .ToArray()
        };
        return JsonUtility.ToJson(wrapper, true);
    }

    public void Deserialize(string json)
    {
        _droppedCoverPositions.Clear();

        var wrapper = JsonUtility.FromJson<DropCoverDTO>(json);
        if (wrapper.positions == null) return;

        foreach (var sv in wrapper.positions)
        {
            var pos = sv.ToVector2Int();
            _droppedCoverPositions.Add(pos);

            var cell = TacticsGrid.Instance.GetCell(pos.x, pos.y);
            if (cell != null)
            {
                cell.Walkable = false;
                TacticsGrid.Instance.AddCover(cell, CoverTypes.FullCover);
                Instantiate(CoverPrefab, cell.Position.ToVector3XZ(1f), Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"[DropCoverManager.Deserialize] Missing cell at {pos}");
            }
        }
    }

}

    
}