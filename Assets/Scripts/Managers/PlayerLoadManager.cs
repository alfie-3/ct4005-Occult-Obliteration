//Script to not get destroyed and hold player load info

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerLoadManager : MonoBehaviour
{
    PlayerInputManager _playerInputManager;
    public PlayerJoinBehavior _playerJoinBehavior;

    public GameObject[] playerSpawns;

    private void Start() {

        if (LocalMultiplayerManager.Instance != null)

        _playerInputManager = LocalMultiplayerManager.Instance.GetComponent<PlayerInputManager>();

        _playerInputManager.joinBehavior = _playerJoinBehavior;

        LocalMultiplayerManager manager = LocalMultiplayerManager.Instance;

        //Uses the playerindex to join the player using the correct index
        for (int i = 0; i < manager.players.Count; i++) {
            if (manager.players[i].PlayerGameObject == null)
                LocalMultiplayerManager.Instance.SpawnPlayer(playerSpawns[i].transform.position, i);
            else {
                if (manager.players[i].PlayerGameObject.activeSelf == false)
                    manager.players[i].PlayerGameObject.SetActive(true);

                manager.players[i].PlayerGameObject.GetComponent<PlayerInput>().SwitchCurrentControlScheme(manager.players[i].devices);

                manager.players[i].PlayerGameObject.GetComponent<PlayerControlScript>().TeleportPlayer(playerSpawns[i].transform.position);
            }
        }
    }
}