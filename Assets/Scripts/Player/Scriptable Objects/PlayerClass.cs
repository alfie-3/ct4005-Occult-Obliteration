//Info for player scriptable object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new Class", menuName = "Class/Class", order = 1)]
[System.Serializable]
public class PlayerClass : ScriptableObject
{
    public GameObject classGameObject;

    public ClassInfo ClassInfo;
    public PlayerStats stats;
    public Color classColour = Color.red;
}

[System.Serializable]
public class PlayerStats
{
    public float moveSpeedMultiplier;
    public float maxHealthMultiplier;
}

[System.Serializable]
public class ClassInfo
{
    public string name;
    [TextArea]
    public string description;
    public Sprite selectImage;
}