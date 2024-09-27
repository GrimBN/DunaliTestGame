using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private Vector3Int startPosition;
    [SerializeField] private Vector3Int winAreaPosition;
    [SerializeField] private Vector3Int[] enemyPositions;
    [SerializeField] private Vector3Int[] floorPositions;
    [SerializeField] private HurdleData[] hurdles;

    public Vector3Int StartPosition { get => startPosition; }
    public Vector3Int WinAreaPosition { get => winAreaPosition; }
    public Vector3Int[] EnemyPositions { get => enemyPositions; }
    public Vector3Int[] FloorPositions { get => floorPositions; }
    public HurdleData[] Hurdles { get => hurdles; }
}
