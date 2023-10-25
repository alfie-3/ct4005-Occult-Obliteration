//Infomation and code for controlling the wave spawning

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WaveInfo { 
    public List<WaveGroup> groups = new();
    public bool willCharonSpawn;
    public GunPool gunPool;
}

[Serializable]
public class WaveGroup {
    public GameObject enemyPrefab;
    public int enemyAmount;
    public float spawnRate;
    public bool isEnemyMultiplierActive;
    public bool isEnemyHealthMultiplierActive;
}

public class WaveManager : MonoBehaviour {

    [Header("Enemy wave info")]
    public int currentWaveNumber;
    //List of spawn points for enemies
    public GameObject[] enemySpawns;
    public List<WaveInfo> waves = new();
    public List<GameObject> enemyList = new();
    [SerializeField] float enemyMultiplier;
    [SerializeField] AudioClip charonArival;

    [Space]
    //Enemy variables
    [Header("Wave bool checks")]
    int randomSpawnPoint;
    [SerializeField] bool[] enemySpawnFinished;
    [SerializeField] bool startedCharonChecker = false;
    public bool finalCharonCheck = false;
    public bool enemySpawnFinCheck;
    GameObject tempEnemyObj;

    [Space]
    [Header("Charon information")]
    public GameObject charon;
    public List<GameObject> charonWeaponSpawns;
    public GunPool charonWeapons;
    [SerializeField] GameObject[] currentDisplayedWeapons;
    GameObject[] allWeapons;

    private void Start() {

        if (LocalMultiplayerManager.Instance != null)
        {
            enemyMultiplier = LocalMultiplayerManager.Instance.GetComponent<LocalMultiplayerManager>().playerMultipliers[LocalMultiplayerManager.Instance.GetComponent<LocalMultiplayerManager>().players.Count - 1];
        }
        UIManager.Current?.UpdateWave(currentWaveNumber + 1);
        currentDisplayedWeapons = new GameObject[charonWeaponSpawns.Count];
    }

    private void Update() {
        //Check that all enemy waves are done spawning
        enemySpawnFinCheck=true;
        if (enemySpawnFinished.Length > 0) {
            for (int i = 0; i < enemySpawnFinished.Length; i++)
            {
                if (!enemySpawnFinished[i])
                {
                    enemySpawnFinCheck = false;
                }
            }
        }
        else { enemySpawnFinCheck = false; }
        if (enemyList.Count == 0 && enemySpawnFinCheck && !startedCharonChecker) {
            startedCharonChecker = true;
            StartCoroutine(CharonEvent());
        }
        enemyList.RemoveAll(x => x == null);
    }

    //For each group in a wave, set up the group spawn coroutine
    public void GroupSorter(int waveID) {
        enemySpawnFinCheck = false;
        startedCharonChecker = false;
        enemySpawnFinished = new bool[waves[currentWaveNumber].groups.Capacity];
        //Sets spawn finished checks to all false
        for (int i = 0; i < enemySpawnFinished.Length; i++) {
            enemySpawnFinished[i] = false;
        }

        //For loop to spawn all groups of enemies
        for (int i = 0; i < waves[waveID].groups.Capacity; i++) {
            //if checks for if player multiplier is active
            if (waves[waveID].groups[i].isEnemyMultiplierActive) {
                StartCoroutine(GroupSpawner(waves[waveID].groups[i].enemyPrefab, i, (int)Math.Ceiling(waves[waveID].groups[i].enemyAmount * enemyMultiplier), waves[waveID].groups[i].spawnRate, waves[waveID].groups[i].isEnemyHealthMultiplierActive));
            }
            else {
                StartCoroutine(GroupSpawner(waves[waveID].groups[i].enemyPrefab, i, waves[waveID].groups[i].enemyAmount, waves[waveID].groups[i].spawnRate, waves[waveID].groups[i].isEnemyHealthMultiplierActive));
            }
        }
    }

    //Spawn each enemy in the group
    IEnumerator GroupSpawner(GameObject enemy, int enemyWaveID, int enemySpawnNum, float spawnRate, bool isHealthMult) {

        for (int i = 0; i < enemySpawnNum; i++) {
            yield return new WaitForSeconds(spawnRate);
            SpawnEnemy(enemy, isHealthMult);
        }
        enemySpawnFinished[enemyWaveID] = true;
    }

    //Sets enemy random spawn position
    private void SpawnEnemy(GameObject enemy, bool isHealthMult) {
        randomSpawnPoint = UnityEngine.Random.Range(0, enemySpawns.Length);
        tempEnemyObj = Instantiate(enemy, enemySpawns[randomSpawnPoint].transform.position, Quaternion.identity);
        if (isHealthMult) {
            tempEnemyObj.GetComponent<EnemyManager>().Init(enemyMultiplier);
        }
        enemyList.Add(tempEnemyObj);
    }

    IEnumerator CharonEvent() {
        //Spwans Charon if it's true for current wave
        if (waves[currentWaveNumber].willCharonSpawn) {            

            //Sets up Charon and random weapons
            charon.SetActive(true);
            charon.GetComponent<AudioSource>().PlayOneShot(charonArival);

            currentDisplayedWeapons = new GameObject[charonWeaponSpawns.Count];
            for (int i = 0; i < charonWeaponSpawns.Count; i++) {
                yield return new WaitForSeconds(0.3f);
                //GunBase gun = charonWeapons.gunList[UnityEngine.Random.Range(0, charonWeapons.gunList.Count)];
                GunBase gun = waves[currentWaveNumber].gunPool.gunList[UnityEngine.Random.Range(0, waves[currentWaveNumber].gunPool.gunList.Count)];
                currentDisplayedWeapons[i] = Instantiate(GunPickup.CreateWeaponDrop(new(gun), true), charonWeaponSpawns[i].transform.position, Quaternion.identity);
            }
            //Choose weapons until time is over               
            StartCoroutine(UIManager.Current?.UpdateCharonTimer(15));
            yield return new WaitForSeconds(15);

            //Despawn all purchasable weapons
            for (int i = 0; i < charonWeaponSpawns.Count; i++) {
                if (currentDisplayedWeapons[i] != null) {
                    Destroy(currentDisplayedWeapons[i]);
                }
            }

            charon.GetComponent<AudioSource>().PlayOneShot(charonArival);
            charon.SetActive(false);
        }
        //Find all gun pickups and destroy them
        allWeapons = GameObject.FindGameObjectsWithTag("Item");
        foreach (var weapon in allWeapons) {
            Destroy(weapon);
        }

        currentWaveNumber++;
        UIManager.Current?.UpdateWave(currentWaveNumber + 1);
        enemyList.Clear();

        if (currentWaveNumber == waves.Capacity) {
            finalCharonCheck=true;
        }
        else {
            GroupSorter(currentWaveNumber);
        }
        yield return null;
    }
}