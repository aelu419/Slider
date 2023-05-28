﻿using System.Xml;
using UnityEngine;
using static MGSimulator;

public class MGUI : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private MGUISquare[] squares;
    [SerializeField] private GameObject trackerPrefab;

    private void Start()
    {
        MGSimulator.OnUnitSpawn += OnUnitSpawn;
    }

    private void OnDisable()
    {
        MGSimulator.OnUnitSpawn -= OnUnitSpawn;
    }

    public MGUISquare GetSquare(MGSpace space)
    {
        return GetSquareFromPos(space.GetPosition());
    }

    public MGUISquare GetSquareFromPos(Vector2Int pos)
    {
        int index = pos.y * width + pos.x;
        return squares[index];
    }

    private void OnUnitSpawn(MGUnit unit)
    {
        CreateTracker(unit);
    }

    private void CreateTracker(MGUnit unit)
    {
        GameObject trackerGO = GameObject.Instantiate(trackerPrefab, this.transform);
        MGUIUnitTracker tracker = trackerGO.GetComponent<MGUIUnitTracker>();
        tracker.SetUnit(unit);
    }

    //private void DestroyTracker(MGUnitData.Data unit)
    //{
    //    MGUIUnitTracker tracker = _unitTrackers[unit];
    //    _unitTrackers.Remove(unit);
    //    Destroy(tracker.gameObject);
    //}
}
