using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelScreen : MonoBehaviour 
{
    [SerializeField] float transitionDuration;
    [SerializeField] GameObject title;

    Material mat;

    private void Awake() {
        mat = GetComponent<Image>().material;
    }

    public IEnumerator EndLevelScreenCoroutine() {
        gameObject.SetActive(true);

        title.SetActive(false);

        if (GameManager.LevelSuccess == true) {
            title.GetComponent<TextMeshProUGUI>().text = "You Survived";
        }
        else {
            title.GetComponent<TextMeshProUGUI>().text = "You have fallen";
        }

        float elapsedTime = 0;
        mat.SetFloat("_Colour", 1);

        while (elapsedTime < transitionDuration) {
            mat.SetFloat("_Colour", Mathf.Lerp(1, 0, elapsedTime / transitionDuration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        mat.SetFloat("_Colour", 0);
        yield return new WaitForSeconds(1);

        title.SetActive(true);

        yield return new WaitForSeconds(2);

        SceneLoadManager.instance.LoadScene("LevelReport");

        yield return null;
    }
}
