//Circular progress bar to show progress for different tasks/events

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircularProrgressBar : MonoBehaviour {
    [SerializeField] Slider slider;
    public Vector2 offset;

    public enum CircularProgressType {
        Increase,
        Decrease
    }

    //Set up the circular bar
    public void Init(float duration, Transform track = null, CircularProgressType progressType = CircularProgressType.Decrease, Color? colour = null) {
        if (colour != null)
            GetComponentInChildren<Image>().color = colour.GetValueOrDefault();

        switch (progressType) {
            case CircularProgressType.Decrease:
                StartCoroutine(CircularProgressDecreaseCoroutine(duration, track));
                break;
            case CircularProgressType.Increase:
                StartCoroutine(CircularProgressIncreaseCoroutine(duration, track));
                break;
        }
    }

    //If the interaction is interrupted stop the circular bar
    public void Cancel() {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    //progress bar decreases on interact
    private IEnumerator CircularProgressDecreaseCoroutine(float duration, Transform track = null) {

        slider.value = slider.maxValue;

        while (slider.value > slider.minValue) {

            RectTransform managerRect = UIManager.Current.GetComponent<RectTransform>();
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (track != null) {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, track.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out Vector2 rectPoint);
                rectPoint += offset;

                rectTransform.anchoredPosition = rectPoint;
            }

            slider.value -= Time.deltaTime / duration;

            yield return null;
        }

        Destroy(gameObject);

        yield return null;
    }

    //Progress bar increases on interact
    private IEnumerator CircularProgressIncreaseCoroutine(float duration, Transform track = null) {

        slider.value = slider.minValue;

        while (slider.value < slider.maxValue) {

            RectTransform managerRect = UIManager.Current.GetComponent<RectTransform>(); ;
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (track != null) {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, track.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out Vector2 rectPoint);
                rectPoint += offset;

                rectTransform.anchoredPosition = rectPoint;
            }

            slider.value += Time.deltaTime / duration;

            yield return null;
        }

        Destroy(gameObject);

        yield return null;
    }
}
