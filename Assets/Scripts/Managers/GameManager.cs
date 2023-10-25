//Controls some of the mechanics of the game scenes

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Current { get { return current; } }
    private static GameManager current;

    public static bool LevelSuccess;

    public delegate void Pause();
    public static event Pause OnPause;

    public delegate void Unpause();
    public static event Unpause OnUnpause;

    public static bool IsPaused = false;

    private void Awake() {
        InputIconTool.LoadSpriteListIfNotAlreadyLoaded();

        if (current != null && current != this) {
            Destroy(this.gameObject);
        }
        else {
            current = this;
        }
    }

    //Pause/Unpause the game
    public static void TogglePause() {
        if (IsPaused) {
            IsPaused = false;
            OnUnpause();
        }
        else {
            IsPaused = true;
            OnPause();
        }
    }

    //When a player gets a kill, share the souls
    public static void ShareSouls(GameObject source, int soulsShared) {
        LocalMultiplayerManager lMM = LocalMultiplayerManager.Instance;

        for (int i = 0; i < lMM.players.Count; i++) {
            if (source != lMM.players[i].PlayerGameObject) {
                lMM.players[i].PlayerGameObject.GetComponent<PlayerManager>().ModifySouls(soulsShared, PlayerManager.SoulModification.add);
            }
        }
    }

    //Loads Recap scene for results
    [ContextMenu("Skip To Report")]
    public void LoadRecap() {
        GameManager.LevelSuccess = true;
        SceneManager.LoadScene("LevelReport");
    }
}

