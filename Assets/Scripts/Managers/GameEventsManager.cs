//Easily editable game structure code. Controls waves, checkpoints etc...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class GameEventInfo {
    public enum EventType { Checkpoint, Waves, TimeConsumer, LevelRecap }; //0=CheckPoints, 1=Waves
    public EventType eventType;
    public GameObject eventReference;
    public List<GameObject> playerEnvironmentContainment;
}

[Serializable]
public class PlayerGameInfo {
    public GameObject player;
    public enum StatType { Dash, EnemyKill, Revive, HealthRecovery };
    public int dashNumber=0;
    public int enemyFinishNumber=0;
    public int friendsReviveNumber = 0;
    public float healthRecoveredNumber=0;
}

public class GameEventsManager : MonoBehaviour
{
    [SerializeField] int nextLevel;

    public List<GameEventInfo> events;
    public GameObject currentEvent;

    LocalMultiplayerManager multiplayerManager;

    WaveManager _waveManager;
    bool continueChecker;
    //bool charonEventEnd=false;

    // Set up events list, player list and the run through of events
    private void Start() {
        foreach (GameEventInfo singleEvent in events) {
            if (singleEvent.eventReference != null) {
                singleEvent.eventReference.SetActive(false);
            }
        }

        multiplayerManager = LocalMultiplayerManager.Instance;

        SetBarriers(true);
        EventRunThrough();
    }

    //If an event is finished, check what the next event is
    async void EventRunThrough() {
        foreach (GameEventInfo gameEvent in events) {
            currentEvent = gameEvent.eventReference;
            if (currentEvent != null) {
                if (gameEvent.eventType == GameEventInfo.EventType.Checkpoint) {
                    await CheckpointFinder(gameEvent.eventReference);
                }
                else if (gameEvent.eventType == GameEventInfo.EventType.Waves) {
                    await WaveSystem(gameEvent.eventReference);
                }
                else if (gameEvent.eventType == GameEventInfo.EventType.TimeConsumer) {
                    await TimePassEvent(currentEvent, gameEvent);
                }
                else if (gameEvent.eventType == GameEventInfo.EventType.LevelRecap) {
                    LevelRecap();
                }
            }
            await Task.Delay((int)1000);
        }
    }
    
    //Checks if all players are near the checkpoint to go to the next event
    async Task CheckpointFinder(GameObject checkpoint) {
        continueChecker = false;
        checkpoint.SetActive(true);
        SetBarriers(false);
        while (true) {
            continueChecker = true;
            //Checks distance to point of all players
            foreach (Player player in multiplayerManager.players) {
                if (Vector3.Distance(player.PlayerGameObject.transform.position, checkpoint.transform.position) > 10) {
                    continueChecker = false;
                }
            }
            if (continueChecker) { //If all players within distance, continue to next event
                checkpoint.SetActive(false);
                await Task.Delay((int)1000);
                return;
            }
            await Task.Delay((int)100);
        }
    }
    
    //Checks if waves have finished, otherwise just continues
    async Task WaveSystem(GameObject waveSystem) {
        waveSystem.SetActive(true);
        _waveManager = waveSystem.GetComponent<WaveManager>();
        _waveManager.enemyList.Clear();
        _waveManager.GroupSorter(0);
        while ((_waveManager.currentWaveNumber != _waveManager.waves.Capacity) || !_waveManager.enemySpawnFinCheck || !_waveManager.finalCharonCheck) {

            if (!Application.isPlaying)
                break;
            await Task.Delay((int)100);
        }
        waveSystem.SetActive(false);
    }

    //Event that passes certain amount of time
    async Task TimePassEvent(GameObject currentObject, GameEventInfo gameEvent) {
        gameEvent.eventReference.SetActive(true);
        StartCoroutine(UIManager.Current?.UpdateCharonTimer(currentEvent.GetComponent<EmptyEventTime>().timeEventTakes));
        await Task.Delay((int)currentEvent.GetComponent<EmptyEventTime>().timeEventTakes * 1000);
        gameEvent.eventReference.SetActive(false);
    }

    void LevelRecap() {
        //Stops controls for each player
        foreach (Player player in multiplayerManager.players) {
            player.PlayerGameObject.GetComponent<PlayerManager>().Pause();
        }
        GameManager.LevelSuccess = true;
        StartCoroutine(UIManager.Current.GetComponentInChildren<EndLevelScreen>(true).EndLevelScreenCoroutine());
    }

    //Removes barriers that enemies can go through but players can't
    void SetBarriers(bool setActive) {
        foreach (GameEventInfo singleEvent in events) {
            foreach (GameObject barrier in singleEvent.playerEnvironmentContainment) {
                barrier.SetActive(setActive);
            }
        }
    }

    //On player death check all other players to see if everyone is dead
    public void CheckRemainingPlayers() {
        int playersDead = 0;

        foreach (Player player in multiplayerManager.players) {
            if (!player.PlayerGameObject.GetComponent<PlayerManager>().AliveState) {
                playersDead++;
            }
        }

        if (playersDead == multiplayerManager.players.Count) {
            GameManager.LevelSuccess = false;
            StartCoroutine(UIManager.Current.GetComponentInChildren<EndLevelScreen>(true).EndLevelScreenCoroutine());
        }
    }

    //Selects next chosen level
    public void LoadNextScene() {
        GameManager.LevelSuccess = true;
        SceneLoadManager.instance.LoadScene("LevelReport");
    }
}

