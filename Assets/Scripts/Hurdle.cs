using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HurdleData
{
    [SerializeField] private Hurdle.Types type;
    [SerializeField] private Vector3Int position;
}

public class Hurdle : MonoBehaviour
{
    public enum Types {Basic, StepOnce, Timed};
    private HurdleData hurdleData;
}
