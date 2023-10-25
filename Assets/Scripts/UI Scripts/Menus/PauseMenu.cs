using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;

    public void UnPause() {

        pauseMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause() {
        gameObject.SetActive(true);
        UIManager.Current.SetSelected(pauseMenuUI.transform.GetChild(1).gameObject);
        Time.timeScale = 0f;
        GameIsPaused = true;

    }


    public void LoadMenu() {
        GameManager.TogglePause();

        SceneLoadManager.instance.LoadScene("MainMenu");
    }

}
