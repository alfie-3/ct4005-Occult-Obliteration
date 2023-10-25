//Controls the next opened scene from current scene - undestroyable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour {
    public static SceneLoadManager instance;
    public LevelPlaylist levelPlaylist;
    [Space]
    [SerializeField] float transitionEffectTime = 0.4f;
    bool sceneIsLoading;

    Canvas transitionCanvas;

    Material transitionMaterial;

    private void Awake() {
        if (instance == null) {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        transitionMaterial = GetComponentInChildren<Image>(true).material;

        transitionCanvas = GetComponentInChildren<Canvas>(true);
    }

    private void OnEnable() {
        SceneManager.activeSceneChanged += TargetCamera;
    }

    private void OnDisable() {
        SceneManager.activeSceneChanged -= TargetCamera;
    }

    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneRoutine(sceneName));
    public void ContinuePlaylist() => StartCoroutine(LoadSceneRoutine(levelPlaylist.NextPlaylist()));

    //Load scene with transition
    private IEnumerator LoadSceneRoutine(string sceneName) {
        if (sceneIsLoading) { yield break; }
        sceneIsLoading = true;

        transitionCanvas.gameObject.SetActive(true);
        yield return LerpTransition(true);

        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!loadAsync.isDone) { yield return null; }

        yield return new WaitForSeconds(0.2f);

        sceneIsLoading = false;
        yield return LerpTransition(false);
        transitionCanvas.gameObject.SetActive(false);
    }

    public void UnloadGameplayManagerScene() {
        SceneManager.UnloadSceneAsync("GameplayManagers");
    }

    public void TargetCamera(Scene current, Scene next) {
        transitionCanvas.worldCamera = Camera.main;
    }

    //Lerp scene transition aesthetic
    private IEnumerator LerpTransition(bool enabled) {
        float elapsedTime = 0;

        if (enabled) {
            while (elapsedTime < transitionEffectTime) {
                transitionMaterial.SetFloat("_Cutoff", Mathf.Lerp(0f, 1.4f, elapsedTime / transitionEffectTime));

                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else {
            while (elapsedTime < transitionEffectTime) {
                transitionMaterial.SetFloat("_Cutoff", Mathf.Lerp(1.4f, 0f, elapsedTime / transitionEffectTime));
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        transitionMaterial.SetFloat("_Cutoff", enabled ? 1.4f : 0);
    }
}

[System.Serializable]
public class LevelPlaylist {
    public List<string> levelNames;
    int currentLevel;

    public LevelPlaylist(string[] levelNames) {
        this.levelNames = levelNames.ToList<String>();
    }

    public string NextPlaylist() {
        if (currentLevel > levelNames.Count) {
            return "EndScreen";
        }

        string levelName = levelNames[currentLevel];
        currentLevel++;

        return levelName;
    }

    public string ResetPlaylsit() {
        currentLevel = 0;

        if (LocalMultiplayerManager.Instance)
            LocalMultiplayerManager.Instance.ResetPlayers();

        return levelNames[currentLevel];
    }

    public bool AtEnd => currentLevel >= levelNames.Count;
}