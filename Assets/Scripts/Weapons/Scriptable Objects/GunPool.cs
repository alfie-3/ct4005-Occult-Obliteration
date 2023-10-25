//Simple scriptable object script for holding a pooled group of guns 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Pool", menuName = "Weapons/Gun/Gun Pool", order = 0)]
public class GunPool : ScriptableObject
{
    public List<GunBase> gunList;
}

