using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public static class PromptUtil
{
    private static GameObject InteractionPrompt;
    private static GameObject CircularProgressBar;

    [RuntimeInitializeOnLoadMethod]
    private static void LoadPrompt() {
        InteractionPrompt = Resources.Load("Prefabs/UI/BindingPrompt") as GameObject;
        CircularProgressBar = Resources.Load("Prefabs/UI/Circular Progress Bar") as GameObject;
    }

    public static GameObject CreateInteractionPrompt(GameObject worldObject, Vector2 offset , PlayerInput viewerInput) {
        //var managerTrsfm = UIManager.Current.transform;
        RectTransform managerRect = UIManager.Current.GetComponent<RectTransform>(); ;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldObject.transform.position);
        Vector2 rectPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out rectPoint);
        rectPoint += offset;

        InputBinding binding = viewerInput.actions["Interact"].bindings[(int)InputIconTool.RetrieveDeviceType(viewerInput.currentControlScheme)];

        InteractionPrompt.GetComponent<RectTransform>().anchoredPosition = rectPoint;
        InteractionPrompt.GetComponent<TextMeshProUGUI>().text = InputIconTool.RetrieveSprite(binding, viewerInput.currentControlScheme);

        return InteractionPrompt;
    }

    public static CircularProrgressBar CreateCircularProgressBar(GameObject worldObject, Vector2 offset, float size = 100) {
        //var managerTrsfm = UIManager.Current.transform;
        RectTransform managerRect = UIManager.Current.GetComponent<RectTransform>(); ;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldObject.transform.position);
        Vector2 rectPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out rectPoint);
        rectPoint += offset;

        CircularProgressBar.GetComponent<RectTransform>().anchoredPosition = rectPoint;
        CircularProgressBar.GetComponent<RectTransform>().sizeDelta = new(size, size);

        CircularProgressBar.GetComponent<CircularProrgressBar>().offset = offset;
        return CircularProgressBar.GetComponent<CircularProrgressBar>();
    }
}
