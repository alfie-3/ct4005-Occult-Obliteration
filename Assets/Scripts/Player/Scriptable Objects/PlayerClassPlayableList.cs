//Pool for available player classes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Playable List", menuName ="Class/Playable Class List", order = 1)]
public class PlayerClassPlayableList : ScriptableObject
{
    public List<PlayerClass> playableClasses;
}
