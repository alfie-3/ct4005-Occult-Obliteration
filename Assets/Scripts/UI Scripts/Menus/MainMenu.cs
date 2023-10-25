//Contains button functions for main menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private void Start() {
        if (LocalMultiplayerManager.Instance) {
            Destroy(LocalMultiplayerManager.Instance.gameObject);
        }

        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);
    }

    //Starts game
    public void PlayGame(string sceneRef)
    {
        if (sceneRef == "Game") {
            SceneLoadManager.instance.levelPlaylist = new(new string[] { "Courtyard", "Underground" });
        }
        else if (sceneRef == "Tutorial") {
            SceneLoadManager.instance.levelPlaylist = new(new string[] { "PlayerTutorial" });
        }

        SceneLoadManager.instance.LoadScene("Character Selection");
    }

    //Quits game
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSelected(GameObject selectable) {
        if (selectable.GetComponent<Selectable>() != null)
            selectable.GetComponent<Selectable>().Select();
    }

    //Returns to menu from other locations such as settings
    public void MainMenuReturn() {
        Destroy(GameObject.Find("Players Manager"));
        SceneManager.LoadScene(0);
    }
}
