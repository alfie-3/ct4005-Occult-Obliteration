//Script that holds multiplayer information and isn't deleted between scenes

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class LocalMultiplayerManager : MonoBehaviour {
    private static LocalMultiplayerManager instance;
    public static LocalMultiplayerManager Instance { get { return instance; } }

    private PlayerInputManager inputManager;

    public List<Player> players;
    public List<float> playerMultipliers;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        inputManager = GetComponent<PlayerInputManager>();
    }

    //Adds player when it detects the right input
    public void OnPlayerJoined(PlayerInput playerInput) {
        //Checks to see if the index is already added
        if (playerInput.playerIndex < players.Count)
            return;

        //Adds a new Player class, and includes the player index for easier tracking
        players.Add(new(playerInput.playerIndex, playerInput.devices.ToArray()));
    }

    //Spawns new player at given location
    public void SpawnPlayer(Vector3 location, int playerIndex) {
        GameObject playerPrefab = players[playerIndex].PlayerClass.classGameObject;
        playerPrefab.transform.position = location;
        playerPrefab.name = players[playerIndex].PlayerClass.name;

        //InputUser user = playerPrefab.GetComponent<PlayerInput>().user;

        players[playerIndex].PlayerGameObject = PlayerInput.Instantiate(playerPrefab, playerIndex, pairWithDevices: players[playerIndex].devices).gameObject;
    }

    //Enables player input
    public void SetPlayersEnabled(bool enabled) {
        foreach (var player in players) {
            player.PlayerGameObject.SetActive(enabled);
        }
    }

    //Destroy all players
    public void ResetPlayers() {
        foreach (var player in players) {
            Destroy(player.PlayerGameObject);
        }
    }

    public void OnDestroy() {
        foreach (var player in players) {
            Destroy(player.PlayerGameObject);
        }
    }
}

//Player class stored information reguarding each player, making them much easier to track
[System.Serializable]
public class Player {
    [ReadOnly] public int PlayerIndex;
    [ReadOnly] public PlayerClass PlayerClass;
    [ReadOnly] public GameObject PlayerGameObject;

    public TrackedPlayerStats TrackedPlayerStats = new();

    public InputDevice[] devices;

    public Player(int playerIndex, InputDevice[] devices) {
        this.PlayerIndex = playerIndex;
        this.devices = devices;
    }
}