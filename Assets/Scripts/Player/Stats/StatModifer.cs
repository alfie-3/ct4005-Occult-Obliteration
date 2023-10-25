using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300
}

[Serializable]
public class StatModifer
{
    [ReadOnly] public float Value;
    [ReadOnly] public StatModType Type;
    [ReadOnly] public int Order;
    [ReadOnly] public object Source;

    [SerializeField] public StatModifer(float value, StatModType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatModifer(float value, StatModType type) : this(value, type, (int)type, null) { }
}
