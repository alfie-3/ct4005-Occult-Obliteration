//Allows players to select their character in character selection scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour {
    private static CharacterSelectionManager instance;
    public static CharacterSelectionManager Instance { get { return instance; } }

    [SerializeField] int countdownDuration;
    [SerializeField] TextMeshProUGUI countDownText;

    public List<PlayerCharacterSelect> playerSelectors;


    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
    }

    //Checks if all players are ready
    public void PlayerReadyStatusChanged() {
        foreach (var player in playerSelectors) {
            player.PopulateSelectorCard();
        }

        //Start countdown for game when all players are ready
        if (CheckPlayersReady()) {
            StartCoroutine(nameof(Countdown));
            countDownText.enabled = true;
        }
        //Stop countdown if a player isn't ready
        else {
            countDownText.enabled = false;
            StopAllCoroutines();
        }
    }

    //Activates timer before the next level is loaded
    IEnumerator Countdown() {
        int counter = countdownDuration;
        countDownText.text = countdownDuration.ToString();

        while(counter > 0 && CheckPlayersReady()) {
            yield return new WaitForSeconds(1);
            counter--; countDownText.text = counter.ToString();
        }

        LoadFirstLevel(); yield return null;
    }

    //Load first level scene
    public void LoadFirstLevel() {
        SceneLoadManager.instance.ContinuePlaylist();
    }

    //Checks player ready states and returns true if all players are ready
    public bool CheckPlayersReady() {
        int numOfPlayers = playerSelectors.Count;
        int i = 0;

        foreach (PlayerCharacterSelect selector in playerSelectors) {
            if (selector.HasSelected())
                i++;
        }

        if (i == numOfPlayers)
            return true;
        else
            return false;
    }

    //Locks characters that have been selected already
    public bool ClassAlreadySelected(PlayerClass playerClass, int playerIndex) {
        for (int i = 0; i < LocalMultiplayerManager.Instance.players.Count; i++) {
            if (i != playerIndex) {
                if (LocalMultiplayerManager.Instance.players[i].PlayerClass == playerClass)
                    return true;
            }
        }

        return false;
    }
}