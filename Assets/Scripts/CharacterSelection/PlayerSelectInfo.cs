using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectInfo : MonoBehaviour
{
    int totalPlayers=0;
    int[] playerNumber;
    int[] characterNumber;
    //Add player input path?

    GameObject[] playerCursors;

    private void Awake() {
        playerNumber = new int[4];
        characterNumber = new int[4];
        playerCursors = new GameObject[4];
    }

    public void AddPlayer() {
        totalPlayers++;
        playerCursors = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerCursors) {
            //Add input;
        }
    }
}
