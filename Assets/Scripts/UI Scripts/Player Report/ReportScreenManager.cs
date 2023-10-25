using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReportScreenManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] ReportCard[] playerReportCards;

    [SerializeField] GameObject continueButton;

    private void Start() {
        if (LocalMultiplayerManager.Instance == null) { return; }

        if (SceneLoadManager.instance.levelPlaylist.AtEnd) {
            title.text = "You Escaped";
        }
        else if (GameManager.LevelSuccess == true) {
            title.text = "Level Cleared";
        }
        else {
            title.text = "Level Failed";
        }

        LocalMultiplayerManager.Instance.SetPlayersEnabled(false);

        EventSystem.current.SetSelectedGameObject(continueButton);

        for (int i = 0; i < LocalMultiplayerManager.Instance.players.Count; i++) {
            playerReportCards[i].gameObject.SetActive(true);
            playerReportCards[i].Init(LocalMultiplayerManager.Instance.players[i]);
        }
    }

    public void Continue() {
        if (SceneLoadManager.instance.levelPlaylist.AtEnd) {
            SceneLoadManager.instance.LoadScene("MainMenu");
        }
        else if (GameManager.LevelSuccess)
            SceneLoadManager.instance.ContinuePlaylist();
        else
            SceneLoadManager.instance.LoadScene("MainMenu");
    }
}
