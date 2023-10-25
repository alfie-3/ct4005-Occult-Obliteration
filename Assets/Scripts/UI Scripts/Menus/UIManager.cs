//Manage player and game UI in scenes

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {
    private static UIManager current;
    public static UIManager Current { get { return current; } }

    [Header("UIElements")]
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] OptionsMenu optionsMenu;
    [Space]
    [SerializeField] TextMeshProUGUI waveNumber;
    public TextMeshProUGUI charonTimer;

    private Canvas canvas;
    public PlayerInfoUI[] playerInfoUIArray;

    //Update players UI info when bullet number or health changes
    public void UpdatePlayerInfo(int playerIndex, PlayerManager player)
    {
        if (playerInfoUIArray[playerIndex].isActiveAndEnabled == false)
        {
            playerInfoUIArray[playerIndex].gameObject.SetActive(true);
            playerInfoUIArray[playerIndex].InitUI(player);
        }

        playerInfoUIArray[playerIndex].UpdatePlayerInfo(player);
    }

    //Wave number change
    public void UpdateWave(int wave) {
        waveNumber.text = $"WAVE {wave}";
    }

    //Current time until next event
    public IEnumerator UpdateCharonTimer(int time) {
        for (int i = time; i>0; i--) {
            charonTimer.text = i + "s";
            yield return new WaitForSeconds(1);
        }
        charonTimer.text = "";
    }

    private void Awake() {
        if (current != null && current != this) {
            Destroy(this.gameObject);
        }
        else {
            current = this;
        }

        canvas = GetComponent<Canvas>();
    }

    private void Start() {
        canvas.worldCamera = Camera.main;
    }

    private void OnEnable()
    {
        GameManager.OnPause += Pause;
        GameManager.OnUnpause += UnPause;
    }

    private void OnDisable()
    {
        GameManager.OnPause -= Pause;
        GameManager.OnUnpause -= UnPause;
    }

    public void Pause() => pauseMenu.Pause();
    public void UnPause() => pauseMenu.UnPause();

    public void UITogglePause() => GameManager.TogglePause();

    public void SetSelected(GameObject selectable) {
        if (selectable.GetComponent<Selectable>() != null)
            selectable.GetComponent<Selectable>().Select();
    }
}
