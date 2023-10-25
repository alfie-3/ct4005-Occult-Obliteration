using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[Serializable]
public class LevelEnemyInfo {
    public int waveNumber;
    public GameObject[] enemyPrefabs;
    public int[] enemyNumbers;
}

public class EnemyWavesScript : MonoBehaviour
{
    public GameObject[] enemySpawnPoints;
    public LevelEnemyInfo[] waveInfo;
}
