//Controls enemy health

using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    Slider slider;
    public GameObject enemy;
    static RectTransform managerRect;
    RectTransform rectTransform;
    private static GameObject healthBar;

    Vector2 offset;

    [RuntimeInitializeOnLoadMethod]
    private static void LoadPrompt()
    {
        healthBar = Resources.Load("Prefabs/UI/HealthBar") as GameObject;
    }

    //Initialises values
    public void Init(Vector2 _offset) {
        rectTransform = GetComponent<RectTransform>();
        slider = gameObject.GetComponent<Slider>();
        offset = _offset;
    }

    //Creates the enemy healthbar
    public static EnemyHealth CreateHealthBar(GameObject worldObject)//, Vector2 offset)
    {
        //var managerTrsfm = UIManager.Current.transform;
        managerRect = UIManager.Current.GetComponent<RectTransform>(); 

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldObject.transform.position);
        Vector2 rectPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out rectPoint);
        //rectPoint += offset;

        healthBar.GetComponent<RectTransform>().anchoredPosition = rectPoint;

        return healthBar.GetComponent<EnemyHealth>();
    }

    //The healthbar updates to remain over the enemy
    private void Update()
    {
        rectTransform = GetComponent<RectTransform>();
        slider = gameObject.GetComponent<Slider>();

        if (enemy == null)
            return;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, enemy.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out Vector2 rectPoint);
        rectPoint += offset;
        rectTransform.anchoredPosition = rectPoint;
        //rectPoint += offset;

        
    }

    //Change the value of the slider on health change
    public void UpdateHealthBar(float maxHealth, float currentHealth) {
        if (maxHealth == currentHealth)
        {
            slider.value = 0;
        }
        else
        {
            slider.value = currentHealth / maxHealth;
        }
    }
}

