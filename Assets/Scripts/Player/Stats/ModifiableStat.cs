//Mods to change player stats (such as health/damage etc..)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEditor;

[Serializable]
public class ModifiableStat {

    public float BaseValue;

    //When the value of the stat is called, the stat is calculated, if the list has not been changed, the stat will not be recalculated
    public float Value {
        get {
            if (isDirty || BaseValue != lastBaseValue) {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }


    protected private bool isDirty = true;
    [ReadOnly ,SerializeField] protected private float _value;
    protected private float lastBaseValue = float.MinValue;

    //List of modifiers applied to stat
    [ReadOnly, SerializeField] private protected List<StatModifer> mods = new List<StatModifer>();
    [ReadOnly, SerializeField] public ReadOnlyCollection<StatModifer> Mods;

    //Constructor - When creating a modifiable state vairable in a script, you only have to define the base stat.
    public ModifiableStat() {
        mods = new List<StatModifer>();
        Mods = mods.AsReadOnly();
    }

    public ModifiableStat(float baseValue) : this() {
        BaseValue = baseValue;
    }

    //Add a modifier too the list
    public void AddModifiers(StatModifer mod) {
        isDirty = true;
        mods.Add(mod);
        mods.Sort(CompareModifierOrder);
    }

    //Remove specific mod
    public bool RemoveModifiers(StatModifer mod) {
        isDirty = true;
        return mods.Remove(mod);
    }

    //Remove all mods from a single source (Weapon, effect, ability, etc.)
    public bool RemoveAllModifiersFromSource(object source) {
        bool didRemove = false;

        for (int i = mods.Count - 1; i >= 0; i--) {
            if (mods[i].Source == source) {
                isDirty = true;
                didRemove = true;
                mods.RemoveAt(i);
            }
        }

        return didRemove;
    }

    //Sorts the modifiers by their sort order.
    protected private int CompareModifierOrder(StatModifer a, StatModifer b) {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0;
    }

    //Calculates the final value of the stat
    protected private float CalculateFinalValue() {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < mods.Count; i++) {
            StatModifer mod = mods[i];

            //Flat - Increases add a specific ammount to the value
            switch (mod.Type) {
                case (StatModType.Flat):
                    finalValue += mod.Value;
                    break;

                //Percent Multiplication - increases multiply the stat by a given amount
                case (StatModType.PercentMult):
                    finalValue *= 1 + mod.Value;
                    break;

                //Percent Add - Increases the stat by a percentage of the value, these happen at the end
                case (StatModType.PercentAdd):
                    sumPercentAdd += mod.Value;
                    if (i + 1 >= mods.Count || mods[i + 1].Type != StatModType.PercentAdd) {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                    break;
            }
        }

        return (float)Math.Round(finalValue, 4);
    }
}
